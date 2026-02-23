using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Diagnostics;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.FileProviders;
using Microsoft.AspNetCore.Mvc.Razor;
using CommandLine;
using Newtonsoft.Json;
using Microsoft.Extensions.Logging;

namespace CoreORM
{
    class Program
    {
        public class Options
        {
            [Option('c', "config name", Required = false, HelpText = "enter the config name defined in your config file")]
            public string ConfigName { get; set; }
        }

        static async Task Main(string[] args)
        {

            AppDomain.CurrentDomain.UnhandledException += (sender, e) =>
            {
                Console.WriteLine("UNHANDLED EXCEPTION:");
                Console.WriteLine(e.ExceptionObject.ToString());
            };


            TaskScheduler.UnobservedTaskException += (sender, e) =>
            {
                Console.WriteLine("UNOBSERVED TASK EXCEPTION:");
                Console.WriteLine(e.Exception.ToString());
                e.SetObserved();
            };

            List<ORMConfig> configs = [];

            string configPath = Path.Combine(CoreUtils.IO.CurrentDirectory(), "config.json");
            if (File.Exists(configPath))
            {
                Console.WriteLine($"using config file: {configPath}");
                string json = CoreUtils.IO.ReadFile(configPath);
                configs = Newtonsoft.Json.JsonConvert.DeserializeObject<List<ORMConfig>>(json);

                await Parser.Default.ParseArguments<Options>(args)
                    .WithParsedAsync(async (Options opts) =>
                    {
                        var config = configs.Where(i => i.ConfigName == opts.ConfigName).FirstOrDefault();
                        if (config == null)
                        {
                            //get default
                            var defaultConfig = configs.Where(i => i.IsDefault).FirstOrDefault();

                            while (config == null)
                            {
                                Console.Write($"Enter a config name: [{defaultConfig?.ConfigName}] ");
                                var configname = Console.ReadLine();
                                if (string.IsNullOrEmpty(configname) && defaultConfig != null)
                                {
                                    configname = defaultConfig.ConfigName;
                                }
                                config = configs.Where(i => i.ConfigName == configname).FirstOrDefault();
                                if (config != null)
                                {
                                    break;
                                }
                            }
                        }
                        await RunInteractiveMenu(config);
                    })
                    .ContinueWith(t =>
                    {
                        if (t.IsFaulted)
                        {
                            Console.WriteLine("ERROR:");
                            Console.WriteLine(t.Exception.ToString());
                        }
                    });
            }
            else
            {
                Console.WriteLine($"Config file does not exist: {configPath}");
            }
        }

        static async Task RunInteractiveMenu(ORMConfig config)
        {
            Console.WriteLine("\nPlease select an action:");
            Console.WriteLine("(1) Generate DB Schema (Fetch from DB and save to schema.json)");
            Console.WriteLine("(2) Process Existing Schema (Load schema.json and execute Views)");
            Console.WriteLine("(3) Full Sync (1 then 2)");
            Console.WriteLine("(4) Process OpenAPI (Parse OpenAPI JSON and execute Views)");
            Console.Write("Enter choice: ");
            string choice = Console.ReadLine();

            DBDatabase dbMap = null;
            string schemaFilePath = Path.Combine(config.DirOutDir, "schema.json");
            
            
            // Ensure output directory exists for schema file
            if (!string.IsNullOrEmpty(config.DirOutDir) && !Directory.Exists(config.DirOutDir))
            {
                Directory.CreateDirectory(config.DirOutDir);
            }

            switch (choice)
            {
                case "1": // Generate DB Schema
                    dbMap = await GenerateSchema(config);
                    SaveSchemaToFile(schemaFilePath, dbMap);
                    Console.WriteLine($"Schema saved to {schemaFilePath}");
                    break;
                case "2": // Process Existing Schema
                    dbMap = LoadSchemaFromFile(schemaFilePath);
                    if (dbMap != null)
                    {
                        await ProcessSchema(config, dbMap);
                    }
                    else
                    {
                        Console.WriteLine("Schema file not found.");
                    }
                    break;
                case "3": // Full Sync
                    dbMap = await GenerateSchema(config);
                    SaveSchemaToFile(schemaFilePath, dbMap);
                    Console.WriteLine($"Schema saved to {schemaFilePath}");
                    await ProcessSchema(config, dbMap);
                    break;
                case "4": // Process OpenAPI
                    string openAPIFilePath = Path.Combine(CoreUtils.IO.CurrentDirectory(), "openapi.json");
                    string openAPISchemaPath = Path.Combine(CoreUtils.IO.CurrentDirectory(), "openapi_schema.json");

                    dbMap = await ProcessOpenAPI(openAPIFilePath, config);
                    if (dbMap != null)
                    {
                        SaveSchemaToFile(openAPISchemaPath, dbMap);
                        Console.WriteLine($"Schema from OpenAPI saved to {openAPISchemaPath}");
                        await ProcessSchema(config, dbMap);
                    }
                    break;
                default:
                    Console.WriteLine("Invalid choice.");
                    break;
            }
        }

        static async Task<DBDatabase> GenerateSchema(ORMConfig config)
        {
            IDBMapper mapper = config.DatabaseType switch
            {
                "mysql" => new ORMMapperMySQL(),
                "pgsql" => new ORMMapperPostgreSQL(),
                _ => new ORMMapperSQLServer(),
            };
            Console.WriteLine("Generating schema from database...");
            DBDatabase dbMap = await mapper.GetMapping(config.DBName, config.NameSpace, config.ConnectionString, new List<DBORMMappings>());
            Console.WriteLine("Schema generation complete.");
            return dbMap;
        }

        static void SaveSchemaToFile(string path, DBDatabase dbMap)
        {
            var originalDbConnection = dbMap.DB;
            dbMap.DB = null; // Not serializable

            var settings = new JsonSerializerSettings
            {
                PreserveReferencesHandling = PreserveReferencesHandling.Objects,
                Formatting = Formatting.Indented
            };
            string json = JsonConvert.SerializeObject(dbMap, settings);
            File.WriteAllText(path, json);

            dbMap.DB = originalDbConnection; // Restore it for subsequent operations (like in Mode 3)
        }

        static DBDatabase LoadSchemaFromFile(string path)
        {
            if (!File.Exists(path)) { Console.WriteLine($"Schema file not found: {path}"); return null; }
            Console.WriteLine($"Loading schema from {path}...");
            string json = File.ReadAllText(path);
            var settings = new JsonSerializerSettings { PreserveReferencesHandling = PreserveReferencesHandling.Objects };
            var dbMap = JsonConvert.DeserializeObject<DBDatabase>(json, settings);
            Console.WriteLine("Schema loaded.");
            return dbMap;
        }

        static async Task<DBDatabase> ProcessOpenAPI(string openApiJsonPath, ORMConfig config)
        {
            string path = Path.IsPathRooted(openApiJsonPath) ? openApiJsonPath : Path.Combine(CoreUtils.IO.CurrentDirectory(), openApiJsonPath);
            if (string.IsNullOrEmpty(openApiJsonPath) || !File.Exists(path))
            {
                Console.WriteLine($"OpenAPI JSON file path is not configured or file does not exist: {path}");
                return null;
            }
            Console.WriteLine($"Processing OpenAPI file: {path}");
            string apiJson = await File.ReadAllTextAsync(path);
            var dbMap = OpenApiParser.MapToDBDatabase(apiJson, config.NameSpace);
            Console.WriteLine("OpenAPI processing complete.");
            return dbMap;
        }

        static async Task ProcessSchema(ORMConfig config, DBDatabase dbMap)
        {
            if (dbMap.DB == null)
            {
                if (config.DatabaseType == "postgres")
                {
                    dbMap.DB = new CoreUtils.PostgresDatabase(config.ConnectionString);
                }
            }
                        
            string appDirectory = AppContext.BaseDirectory;
            string dllName = Assembly.GetEntryAssembly().GetName().Name;

            CoreUtils.ConsoleLogger.Warn($"App Directory: {appDirectory}");

            var builder = WebApplication.CreateBuilder();

            builder.Environment.ApplicationName = dllName;
            builder.Environment.ContentRootPath = appDirectory;
            builder.Environment.ContentRootFileProvider = new PhysicalFileProvider(appDirectory);

            builder.Services.AddLogging(logging =>
            {
                logging.AddConsole();
                logging.SetMinimumLevel(LogLevel.Debug);
            });

            builder.Services.AddControllersWithViews()
                .AddRazorRuntimeCompilation()
                .AddRazorOptions(options =>
                {
                    options.ViewLocationFormats.Clear();
                    options.ViewLocationFormats.Add("/Views/{1}/{0}.cshtml");
                    options.ViewLocationFormats.Add("/Views/Shared/{0}.cshtml");
                    options.ViewLocationFormats.Add("/Views/{0}.cshtml");
                });

            builder.Services.AddSingleton<RazorViewToStringRenderer>();

            var provider = builder.Services.BuildServiceProvider();
            var renderer = provider.GetRequiredService<RazorViewToStringRenderer>();
            string txt = string.Empty;

            DateTime startTime = DateTime.Now;
            Console.WriteLine("BEGIN Code Generation " + startTime);
            Console.WriteLine($"DirOutDir={config.DirOutDir}");
            Console.WriteLine($"ViewsDirectory={config.ViewsDirectory}");
            Console.WriteLine($"ViewsCount={config.Views?.Count}");
            Console.WriteLine("ContentRootPath = " + builder.Environment.ContentRootPath);

            CoreUtils.IO.DeleteFolder(config.DirOutDir);

            if (config.PreProcess != null)
            {
                foreach (var preProcess in config.PreProcess)
                {
                    try
                    {
                        ExecProcess(preProcess);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("ERROR executing pre process:" + ex.ToString());
                    }
                }
            }


            if (config.Views != null)
            {
                Console.WriteLine($"Executing using Views configs");

                //process views provided in the config file
                foreach (var view in config.Views)
                {
                    var physicalViewPath = Path.Combine(AppContext.BaseDirectory, "Views", config.ViewsDirectory, view.ViewFileName);
                    if (!File.Exists(physicalViewPath))
                    {
                        CoreUtils.ConsoleLogger.Warn($"Physical file NOT found: {physicalViewPath}");
                        // list dir contents for debug:
                        var dir = Path.Combine(AppContext.BaseDirectory, "Views", config.ViewsDirectory);
                        if (Directory.Exists(dir))
                        {
                            CoreUtils.ConsoleLogger.Info("Files in dir: " + string.Join(", ", Directory.GetFiles(dir)));
                        }
                        else
                        {
                            CoreUtils.ConsoleLogger.Warn("Views subdir missing entirely.");
                        }
                        throw new FileNotFoundException("View file missing on disk", physicalViewPath);
                    }
                    else
                    {
                        CoreUtils.ConsoleLogger.Info($"Physical file found: {physicalViewPath}");
                    }

                    string outfilepath = view.ViewOutputFilePath;
                    string razorPath = "/Views/" + config.ViewsDirectory + "/" + view.ViewFileName;
                    dbMap.param0 = "";
                  
                    if (!string.IsNullOrEmpty(view.ViewParams))
                    {
                        dbMap.param0 = view.ViewParams;
                    }
                    txt = renderer.RenderViewToStringAsync(razorPath, dbMap).GetAwaiter().GetResult();

                    CoreUtils.IO.CreateTextFile(outfilepath, txt);
                    Console.WriteLine($"Generated {outfilepath}");

                    if (view.ViewPostProcess != null)
                    {
                        foreach (var postProcess in view.ViewPostProcess)
                        {
                            try
                            {
                                ExecProcess(postProcess);
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine("ERROR executing view post process:" + ex.ToString());
                            }
                        }
                    }
                }

            }
            else
            {
                Console.WriteLine($"Executing using Views Directory");
                //process all views in the Views Directory
                var files = CoreUtils.IO.GetFiles(Path.Join(CoreUtils.IO.CurrentDirectory(), "Views", config.ViewsDirectory));
                foreach (var razorFilePath in files)
                {
                    string newFileNamePath = string.Empty;

                    foreach (var findR in config.RegExReplace)
                    {
                        newFileNamePath = CoreUtils.Data.FindReplaceEx(razorFilePath, findR.FindRegEx, findR.Replace);
                    }

                    var outfilepath = Path.Combine(config.DirOutDir, Path.GetFileName(newFileNamePath));

                    //absolute path of the razor view not the directory path
                    string razorPath = "/Views/" + config.ViewsDirectory + "/" + Path.GetFileName(razorFilePath);
                    txt = renderer.RenderViewToStringAsync(razorPath, dbMap).GetAwaiter().GetResult();

                    CoreUtils.IO.CreateTextFile(outfilepath, txt);
                    Console.WriteLine($"Generated {outfilepath}");
                }
            }


            if (config.PostProcess != null)
            {
                foreach (var postProcess in config.PostProcess)
                {
                    try
                    {
                        ExecProcess(postProcess);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("ERROR executing post process:" + ex.ToString());
                    }
                }
            }

            Console.WriteLine($"Code Generation Done. {(DateTime.Now - startTime).TotalSeconds} secs");
        }


        static void ExecProcess(ORMProcess process)
        {
            if (!string.IsNullOrEmpty(process.ProcessExec))
            {
                ProcessStartInfo startInfo = new ProcessStartInfo(process.ProcessExec, process.ProcessArgs);
                startInfo.WorkingDirectory = process.ProcessWorkingDir;
                startInfo.UseShellExecute = false;
                startInfo.RedirectStandardOutput = true;
                startInfo.RedirectStandardError = true;

                using (Process osProcess = Process.Start(startInfo))
                {
                    Console.WriteLine($"Process starting: {process.ProcessExec} {process.ProcessArgs}");

                    osProcess.OutputDataReceived += new DataReceivedEventHandler((object sender, DataReceivedEventArgs e) =>
                    {
                        if (!string.IsNullOrEmpty(e.Data))
                        {
                            Console.Write(e.Data);
                        }
                    });

                    osProcess.ErrorDataReceived += new DataReceivedEventHandler((object sender, DataReceivedEventArgs e) =>
                    {
                        if (!string.IsNullOrEmpty(e.Data))
                        {
                            Console.WriteLine("ERROR:" + e.Data);
                        }
                    });

                    osProcess.BeginOutputReadLine();
                    osProcess.BeginErrorReadLine();
                    osProcess.WaitForExit();

                    Console.WriteLine("Post Process completed with exit code: " + osProcess.ExitCode);
                }
            }
        }
    }
}
