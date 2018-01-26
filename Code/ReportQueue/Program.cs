using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Mime;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using Sunnet.Cli.Core.Reports;
using Sunnet.Framework;
using System.Threading;
using System.Configuration;

//sc delete CLI_Report_Queue_Service
using Sunnet.Framework.Extensions;
/*FTP host: katy.sunnet.us
USER: clibup
password: Abcd1234
SFTP*/
namespace ReportQueue
{
    /*
    System.ServiceProcess.ServiceControllerStatus
    { 
    Stopped = 1,
    StartPending = 2,
    StopPending = 3,
    Running = 4,
    ContinuePending = 5,
    PausePending = 6,
    Paused = 7
    }
     */
    public enum CliServiceStatus
    {
        /// <summary>
        /// 服务未安装
        /// </summary>
        NotInstalled = 0,
        /// <summary>
        /// 服务未运行。 这对应于 Win32 SERVICE_STOPPED 常数，该常数定义为 0x00000001。
        /// </summary>
        Stopped = 1,
        /// <summary>
        /// 服务正在启动。 这对应于 Win32 SERVICE_START_PENDING 常数，该常数定义为 0x00000002。
        /// </summary>
        StartPending = 2,
        /// <summary>
        /// 服务正在停止。 这对应于 Win32 SERVICE_STOP_PENDING 常数，该常数定义为 0x00000003。
        /// </summary>
        StopPending = 3,
        /// <summary>
        /// 服务正在运行。 这对应于 Win32 SERVICE_RUNNING 常数，该常数定义为 0x00000004。
        /// </summary>
        Running = 4,
        /// <summary>
        /// 服务即将继续。 这对应于 Win32 SERVICE_CONTINUE_PENDING 常数，该常数定义为 0x00000005。
        /// </summary>
        ContinuePending = 5,
        /// <summary>
        /// 服务即将暂停。 这对应于 Win32 SERVICE_PAUSE_PENDING 常数，该常数定义为 0x00000006。
        /// </summary>
        PausePending = 6,
        /// <summary>
        /// 服务已暂停。 这对应于 Win32 SERVICE_PAUSED 常数，该常数定义为 0x00000007。
        /// </summary>
        Paused = 7
    }

    class Program
    {
        private static string ServiceKey
        {
            get
            {
                return string.Format("CLI_Report_Queue_Service_({0})", string.Join("_", Config.Instance.ProcessTypes)); //David 08262016 THe ServiceKey MUST start with 'CLI'
            }
        }

        private static string ServiceName
        {
            get
            {
                return string.Format("CLI_Report_Queue_Service_({0})", string.Join(" ", Config.Instance.ProcessTypes)); //David 08262016 THe ServiceKey MUST start with 'CLI'
            }
        }

        private static string ServiceDescription
        {
            get
            {
                var types = Config.Instance.ProcessTypes.Select(t => ((ReportQueueType)t).ToDescription()).ToList();
                return string.Format("CLI Report Queue Service, keep running to generate pdf and send email to Request User. This service instance will process type{0}: {1}.--07/29/2016",
                    types.Count > 1 ? "s" : "",
                    string.Join(", ", types));
            }
        }

        private static int option = 0;
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
                // 如果传递了"s"参数就启动服务
                if (args.Length > 0 && args[0] == "s")
                {
                    //启动服务的代码，可以从其它地方拷贝
                    ServiceBase[] ServicesToRun;
                    ServicesToRun = new ServiceBase[]
                    {
                    new MainService(),
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
