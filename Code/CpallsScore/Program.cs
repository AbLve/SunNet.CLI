using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Sunnet.Framework;
using Sunnet.Framework.Service;
using System.Configuration;

namespace CpallsScore
{
    class Program
    {
        private static string ServiceKey
        {
            get
            {
                return "CLI_CpallsScore_Service";
            }
        }

        private static string ServiceName
        {
            get
            {
                return "CLI_CpallsScore_Service";
            }
        }

        private static string ServiceDescription
        {
            get
            {
                return "CLI Cpalls Score Service,keep running to change score in cpalls assessment when student's dob changed or assessment's benchmark changed --07/29/2016";
            }
        }

        static void Main(string[] args)
        {
            IoC.Init();

            string runningMode = ConfigurationManager.AppSettings["RunningMode"];//David   08/05/2016
            #region Debug on local console program
            if (runningMode == "Debug")//Debug Mode
            {
                Console.WriteLine("Running with debug mode...");
                Config.Instance.Logger.Info("Running with Windows Service mode...");
                Execute();
            }
            #endregion
            #region Start Service
            else //Windows Service Mode
            {

                Console.WriteLine("Running with Windows Service mode...");
                Config.Instance.Logger.Info("Running with Windows Service mode...");

                if (args.Length > 0 && args[0] == "s")
                {
                    ServiceBase[] ServiceToRun;
                    ServiceToRun = new ServiceBase[]
                    {
                    new CpallsScoreService(),
                    };
                    ServiceBase.Run(ServiceToRun);
                }
                else if (args.Length > 0 && args[0] == "t")
                {
                    Execute();
                }
                else
                {
                    Console.WriteLine(ServiceName);
                    CheckStatus();
                }

            }
            #endregion
        }

        private static void Execute()
        {
            QueueManager manager = new QueueManager();
            manager.Start();
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
            Console.WriteLine("Continue? (Y/N)");
            Console.ReadLine();
            string s = Console.ReadLine();
            if (s == "Y" || s == "y")
                goto Start;
        }

        static CliServiceStatus ISWindowsServiceInstalled(string serviceName)
        {
            ServiceController[] services = ServiceController.GetServices();
            var cliService = services.FirstOrDefault(
                x => x.ServiceName.Equals(ServiceName, StringComparison.CurrentCultureIgnoreCase));
            if (cliService == null)
            {
                return CliServiceStatus.NotInstalled;
            }
            return (CliServiceStatus)cliService.Status;
        }
    }
}
