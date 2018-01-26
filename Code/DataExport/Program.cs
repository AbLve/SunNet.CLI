using System.Data;
using System.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;

using Sunnet.Cli.Business.Export;
using Sunnet.Cli.Core;
using Sunnet.Cli.Core.Export.Entities;
using Sunnet.Framework;
using Sunnet.Cli.Core.Export.Enums;
using Sunnet.Framework.Core.Tool;
using Sunnet.Framework.StringZipper;
using Sunnet.Framework.File;
using Sunnet.Framework.Helpers;
using Sunnet.Framework.EmailSender;
using StructureMap;
using Sunnet.Framework.Encrypt;
using Sunnet.Framework.Log;
using Sunnet.Framework.Service;
using System.ServiceProcess;
using System.Threading;
using System.Diagnostics;

namespace DataExport
{
    class Program
    {
        private static string ServiceKey
        {
            get
            {
                return "CLI_DataExport_Service(ExportInfo)";
            }
        }

        private static string ServiceName
        {
            get
            {
                return "CLI_DataExport_Service(ExportInfo)";
            }
        }

        private static string ServiceDescription
        {
            get
            {
                return "CLI Data Export Service, keep running to generate csv and send email or sftp to User. --07/29/2016";
            }
        }

        private static void Test()
        {
            QueueManager manager = new QueueManager();
            manager.Start();
        }

        static void Main(string[] args)
        {
            IoC.Init();

            string runningMode = ConfigurationManager.AppSettings["RunningMode"];//David   08/05/2016

            if (runningMode == "Debug")//Debug Mode
            {
                Console.WriteLine("Running with debug mode...");
                Config.Instance.Logger.Info("Running with Windows Service mode...");
                Test();
            }
            else //Windows Service Mode
            {
                Console.WriteLine("Running with Windows Service mode...");
                Config.Instance.Logger.Info("Running with Windows Service mode...");

                //如果传递了"s"参数就启动服务
                if (args.Length > 0 && args[0] == "s")
                {
                    //启动服务的代码，可以从其它地方拷贝
                    ServiceBase[] ServicesToRun;
                    ServicesToRun = new ServiceBase[]
                    {
                    new DataExportService(),
                    };
                    ServiceBase.Run(ServicesToRun);
                }
                else if (args.Length > 0 && args[0] == "t")
                {
                    Test();
                }
                else
                {
                    Console.WriteLine(ServiceName);
                    CheckStatus();
                }

            }
        }



        private static int option = 0;
        private static void CheckStatus()
        {
        Start:
            var status = ISWindowsServiceInstalled(ServiceKey);
            Console.WriteLine("\r\n\r\nPlease choose an option below:");
            var options = "Options:\r\n";
            var command = "";
            switch (status)
            {
                case CliServiceStatus.NotInstalled:
                    options += "[I] Install Service\r\n";
                    break;
                case CliServiceStatus.Running:
                case CliServiceStatus.StartPending:
                case CliServiceStatus.ContinuePending:
                    options += "[S] Stop Service\r\n";
                    break;
                case CliServiceStatus.StopPending:
                    options += "[S] Start Service\r\n";
                    break;
                case CliServiceStatus.Stopped:
                    options += "[S] Start Service\r\n";
                    options += "[U] Uninstall Service\r\n";
                    break;
            }
            options += "[Q] Quit";
            Console.WriteLine(options);
            option = Console.Read();
            switch (option)
            {
                case 'I':
                case 'i':
                    // s : Running from Windows Service Call
                    var path = Process.GetCurrentProcess().MainModule.FileName + " s";
                    command =
                        string.Format("create {0} binpath= \"{1}\" displayName= \"{2}\" start= delayed-auto",
                            ServiceKey, path, ServiceName, ServiceDescription);
                    Process.Start("sc", command);

                    // Setting Description
                    command = string.Format("description {0} \"{1}\"", ServiceKey, ServiceDescription);
                    Process.Start("sc", command);

                    Console.WriteLine("Operation successful.");
                    break;
                case 'S':
                case 's':
                    if (status == CliServiceStatus.Running || status == CliServiceStatus.StartPending || status == CliServiceStatus.ContinuePending)
                    {
                        // Stop Service
                        command = string.Format("stop {0}", ServiceKey);
                        Process.Start("sc", command);
                    }
                    if (status == CliServiceStatus.StopPending || status == CliServiceStatus.Stopped)
                    {
                        // Start Service
                        command = string.Format("start {0}", ServiceKey);
                        Process.Start("sc", command);
                    }
                    Console.WriteLine("Operation successful.");
                    break;
                case 'U':
                case 'u':
                    // Uninstall Service
                    command = string.Format("delete {0}", ServiceKey);
                    Process.Start("sc", command);
                    Console.WriteLine("Operation successful.");
                    break;
                case 'Q':
                case 'q':
                    Environment.Exit(0);
                    break;
                default: break;
            }

            Console.WriteLine("Please wait...");
            Thread.Sleep(3000);
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine("Can you continue? (Y/N)");
            Console.ReadLine();
            string s = Console.ReadLine();
            if (s == "Y" || s == "y")
                goto Start;
        }


        static CliServiceStatus ISWindowsServiceInstalled(string serviceName)
        {
            // get list of Windows services
            ServiceController[] services = ServiceController.GetServices();
            var cliService =
                services.FirstOrDefault(
                    x => x.ServiceName.Equals(serviceName, StringComparison.CurrentCultureIgnoreCase));
            if (cliService == null)
            {
                return CliServiceStatus.NotInstalled;
            }
            return (CliServiceStatus)cliService.Status;
        }
    }
}
