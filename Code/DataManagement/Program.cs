using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DataManagement.App;
using DataManagement.Business;
using DataManagement.WindowService;
using Sunnet.Framework;
using Sunnet.Framework.Log;
using Sunnet.Framework.Service;

namespace DataManagement
{
    class Program
    {
        private static string ServiceKey
        {
            get
            {
                return "CLI_Managment_Service";
            }
        }

        private static string ServiceName
        {
            get
            {
                return "CLI DataManageMent Service";
            }
        }

        private static string ServiceDescription
        {
            get
            {
                return "CLI DataManagerment Automatic Execution Service";
            }
        }

        private static int _serviceOption = 0;

        static void Main(string[] args)
        {
            IoC.Init();

            LogManager.InfoAndConsole("Program startup");

            if (AppConfigManager.ProgramRunModel == "Debug")
            {
                RunByConsole();
            }
            else
            {
                if (args.Length > 0 && args[0] == "s")
                {
                    RunByService();
                }
                else if (args.Length > 0 && args[0] == "t")
                {
                    RunByConsole();
                }
                else
                {
                    CheckStatus();
                }
            }
        }

        /// <summary>
        /// Run with "Window service"
        /// </summary>
        private static void RunByService()
        {
            try
            {
                LogManager.Info("Run in a 'windows service' way and start creating a service.");

                ServiceBase[] servicesToRun = new ServiceBase[] { new DataManageMentService() };
                ServiceBase.Run(servicesToRun);

                LogManager.Info("The windows service has been successfully created.");
            }
            catch(Exception servicException)
            {
                LogManager.Info("When the service is created, there is an exception：" + servicException.Message);
            }
            finally
            {
                LogManager.Info("Service is running...");
            }
        }

        /// <summary>
        /// Run with "Console"
        /// </summary>
        private static void RunByConsole()
        {
            try
            {
                LogManager.Info("Run with 'Console'");

                while (true)
                {
                    LogManager.Info("Process begins");

                    #region Processing SingleRoster
                    LogManager.InfoAndConsole("QueuedSingleRoster Run");
                    ProcessingSingleRoster();
                    LogManager.InfoAndConsole("QueuedSingleRoster End");
                    #endregion


                    #region Processing Queued Tasks
                    Console.WriteLine("ProcessingPendingTasks Run");
                    ProcessingPendingTasks();
                    LogManager.InfoAndConsole("ProcessingPendingTasks End");
                    #endregion

                    LogManager.Info("Process completion");

                    LogManager.InfoAndConsole("Programe sleep " + AppConfigManager.ExecutionTimeInterval + " Minutes");
                    Thread.Sleep(AppConfigManager.ExecutionTimeInterval * 60 * 1000);
                }
            }
            catch (Exception e)
            {
                LogManager.InfoAndConsole("Exception in the run of the program, " + e.Message);

                //重新执行
                RunByConsole();
            }
        }

        #region Winodow service operation
        private static void CheckStatus()
        {
            Start:
            var status = IsWindowsServiceInstalled(ServiceKey);
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
            _serviceOption = Console.Read();
            switch (_serviceOption)
            {
                case 'I':
                case 'i':
                    // s : Running from Windows Service Call
                    var path = Process.GetCurrentProcess().MainModule.FileName + " s";
                    command = string.Format("create {0} binpath= \"{1}\" displayName= \"{2}\" start= delayed-auto", ServiceKey, path, ServiceName);
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
            {
                goto Start;
            }
        }

        private static CliServiceStatus IsWindowsServiceInstalled(string serviceName)
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

        #endregion

        #region RunByConsole Funciton

        private static bool ProcessingSingleRoster()
        {
            SingleRosterManager rosterManager = new SingleRosterManager();
            return  rosterManager.QueuedSingleRoster();
        }

        private static bool ProcessingPendingTasks()
        {
            TaskManager taskManager = new TaskManager();
            return taskManager.ExecuteTask();
        }
        #endregion
    }
}
