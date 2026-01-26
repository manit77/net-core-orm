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
                        if (config != null)
                        {
                            await GenFiles(config);
                        }
                        else
                        {
                            //get default
                            var defaultConfig = configs.Where(i => i.IsDefault).FirstOrDefault();

                            while (config == null)
                            {
                                Console.WriteLine($"Enter a config name: [{defaultConfig?.ConfigName}]");

                                var configname = Console.ReadLine();
                                if (string.IsNullOrEmpty(configname))
                                {
                                    configname = defaultConfig.ConfigName;
                                }

                                config = configs.Where(i => i.ConfigName == configname).FirstOrDefault();
                                if (config != null)
                                {
                                    await GenFiles(config);
                                    break;
                                }
                            }
                        }

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

        static async Task GenFiles(ORMConfig config)
        {
            IDBMapper mapper;

            if (config.DatabaseType == "mysql")
            {
                mapper = new ORMMapperMySQL();
            }
            else
            {
                mapper = new ORMMapperSQLServer();
            }

            DBDatabase dbMap = await mapper.GetMapping(config.DBName, config.NameSpace, config.ConnectionString, new List<DBORMMappings>());

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

            CoreUtils.IO.DeleteFiles(config.DirOutDir);

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
                                ExecPostProcess(postProcess);
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
                        ExecPostProcess(postProcess);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("ERROR executing post process:" + ex.ToString());
                    }
                }
            }

            Console.WriteLine($"Code Generation Done. {(DateTime.Now - startTime).TotalSeconds} secs");
        }

        static void ExecPostProcess(ORMPostProcess postProcess)
        {
            if (!string.IsNullOrEmpty(postProcess.PostProcessExec))
            {
                ProcessStartInfo startInfo = new ProcessStartInfo(postProcess.PostProcessExec, postProcess.PostProcessArgs);
                startInfo.WorkingDirectory = postProcess.PostProcessWorkingDir;
                startInfo.UseShellExecute = false;
                startInfo.RedirectStandardOutput = true;
                startInfo.RedirectStandardError = true;

                using (Process process = Process.Start(startInfo))
                {
                    Console.WriteLine($"Post Process starting: {postProcess.PostProcessExec} {postProcess.PostProcessArgs}");

                    process.OutputDataReceived += new DataReceivedEventHandler((object sender, DataReceivedEventArgs e) =>
                    {
                        if (!string.IsNullOrEmpty(e.Data))
                        {
                            Console.Write(e.Data);
                        }
                    });

                    process.ErrorDataReceived += new DataReceivedEventHandler((object sender, DataReceivedEventArgs e) =>
                    {
                        if (!string.IsNullOrEmpty(e.Data))
                        {
                            Console.WriteLine("ERROR:" + e.Data);
                        }
                    });

                    process.BeginOutputReadLine();
                    process.BeginErrorReadLine();
                    process.WaitForExit();

                    Console.WriteLine("Post Process completed with exit code: " + process.ExitCode);
                }
            }
        }
    }
}
