using System;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Internal;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.ObjectPool;
using System.Collections.Generic;
using System.Linq;
using CommandLine;
using static Org.BouncyCastle.Math.EC.ECCurve;

namespace CoreORM
{
    class Program
    {
        public class Options
        {
            [Option('c', "config name", Required = false, HelpText = "enter the config name defined in your config file")]
            public string ConfigName { get; set; }

        }

        static void Main(string[] args)
        {
            List<ORMConfig> configs = new List<ORMConfig>();
            string json = CoreUtils.IO.ReadFile(Path.Join(CoreUtils.IO.CurrentDirectory(), "config.json"));
            configs = Newtonsoft.Json.JsonConvert.DeserializeObject<List<ORMConfig>>(json);

            CommandLine.Parser.Default.ParseArguments<Options>(args)
                .WithParsed((Options opts) =>
                {
                    var config = configs.Where(i => i.ConfigName == opts.ConfigName).FirstOrDefault();
                    if (config != null)
                    {
                        GenFiles(config);
                    }
                    else
                    {
                        //get default
                        var defaultConfig = configs.Where(i=> i.IsDefault).FirstOrDefault();

                        while (config == null)
                        {
                            Console.WriteLine($"Enter a config name: [{defaultConfig.ConfigName}]");
                            
                            var configname = Console.ReadLine();
                            if (string.IsNullOrEmpty(configname))
                            {
                                configname = defaultConfig.ConfigName;
                            }

                            config = configs.Where(i => i.ConfigName == configname).FirstOrDefault();
                            if (config != null)
                            {
                                GenFiles(config);
                                break;
                            }
                        }
                    }

                })
                .WithNotParsed((IEnumerable<Error> errors) =>
                {
                    Debug.WriteLine(errors);
                });
        }

        static void GenFiles(ORMConfig config)
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

            DBDatabase dbMap = mapper.GetMapping(config.DBName, config.NameSpace, config.ConnectionString, new List<DBORMMappings>());

            string appDirectory = Directory.GetCurrentDirectory();
            string dllName = Assembly.GetEntryAssembly().GetName().Name;

            var services = new ServiceCollection();

            HostingEnvironment environment = new HostingEnvironment
            {
                WebRootFileProvider = new PhysicalFileProvider(appDirectory),
                ApplicationName = dllName
            };

            services.Configure<RazorViewEngineOptions>(options =>
            {
                options.FileProviders.Clear();
                options.FileProviders.Add(new PhysicalFileProvider(appDirectory));
            });

            DiagnosticListener diagnosticSource = new DiagnosticListener("Microsoft.AspNetCore");

            services.AddSingleton<IHostingEnvironment>(environment);
            services.AddSingleton<ObjectPoolProvider, DefaultObjectPoolProvider>();
            services.AddSingleton<DiagnosticSource>(diagnosticSource);
            services.AddLogging();
            services.AddMvc();
            services.AddSingleton<RazorViewToStringRenderer>();

            var provider = services.BuildServiceProvider();

            var renderer = provider.GetRequiredService<RazorViewToStringRenderer>();

            string txt = string.Empty;
            DateTime startTime = DateTime.Now;
            Console.WriteLine("BEGIN Code Generation " + startTime);

            Console.WriteLine($"DirOutDir={config.DirOutDir}");
            Console.WriteLine($"ViewsDirectory={config.ViewsDirectory}");

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

            if(config.PostProcess != null)
            {
                foreach(var postProcess in config.PostProcess) {

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

                    process.OutputDataReceived += new DataReceivedEventHandler((object sender, DataReceivedEventArgs e) => {
                        Console.Write(e.Data);
                    });

                    process.ErrorDataReceived += new DataReceivedEventHandler((object sender, DataReceivedEventArgs e) => {
                        Console.WriteLine("ERROR:" + e.Data);
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
