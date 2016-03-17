﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Lime.Protocol.Serialization;

namespace Takenet.MessagingHub.Client.Host
{
    /// <summary>
    /// Host utility program for Messaging Hub clients.
    /// </summary>
    class Program
    {
        static void Main(string[] args)
        {
            MainAsync(args).Wait();
        }

        static async Task MainAsync(string[] args)
        {
            try
            {                
                var applicationFileName = Bootstrapper.DefaultApplicationFileName;
                if (args.Length > 0)
                {
                    applicationFileName = "application.json";
                    if (!File.Exists(applicationFileName))
                    {
                        applicationFileName = args[0];
                        if (applicationFileName.Trim('-', '/').Equals("help", StringComparison.OrdinalIgnoreCase))
                        {
                            WriteLine();
                            WriteLine("Messaging Hub client host");
                            WriteLine();
                            WriteLine("Usage: mhh <path>");
                            WriteLine();
                            WriteLine("- path: The path of the application host JSON file");
                            WriteLine();
                            return;
                        }
                    }
                }

                if (!File.Exists(applicationFileName))
                {
                    WriteLine($"Could not find the {applicationFileName} file", ConsoleColor.Red);
                    return;
                }

                WriteLine("Starting application...", ConsoleColor.Blue);
                ConfigureWorkingDirectory(applicationFileName);
                var application = Application.ParseFromJsonFile(applicationFileName);
                var stopabble = await Bootstrapper.StartAsync(application);
                WriteLine("Application started. Press any key to stop.", ConsoleColor.Blue);
                Console.Read();
                WriteLine("Stopping application...", ConsoleColor.Blue);
                await stopabble.StopAsync();
                WriteLine("Application stopped.", ConsoleColor.Blue);
            }
            catch (Exception ex)
            {
                WriteLine("Application failed:");
                WriteLine(ex.ToString(), ConsoleColor.Red);
#if DEBUG
                Console.ReadKey(true);
#endif
            }
        }

        private static void ConfigureWorkingDirectory(string applicationFileName)
        {
            var path = Path.GetDirectoryName(applicationFileName);
            if (!string.IsNullOrWhiteSpace(path))
            {
                Directory.SetCurrentDirectory(path);
            }
            else
            {
                path = Environment.CurrentDirectory;
            }

            AppDomain.CurrentDomain.AssemblyResolve += (sender, eventArgs) =>
            {
                var assemblyName = new AssemblyName(eventArgs.Name);
                var filePath = Path.Combine(path, $"{assemblyName.Name}.dll");
                return File.Exists(filePath) ? Assembly.LoadFile(filePath) : null;
            };
        }

        static void WriteLine(string value = "", ConsoleColor color = ConsoleColor.White)
        {
            var foregroundColor = Console.ForegroundColor;
            Console.ForegroundColor = color;
            Console.WriteLine(value);
            Console.ForegroundColor = foregroundColor;
            Console.Out.Flush();
        }
    }
}