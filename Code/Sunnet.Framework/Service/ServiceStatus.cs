using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sunnet.Framework.Service
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
}
