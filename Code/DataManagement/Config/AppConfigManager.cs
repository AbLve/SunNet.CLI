using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataManagement
{
    public static class AppConfigManager
    {
        /// <summary>
        /// 执行时间间隔
        /// （接收单位：分钟，配置项无效则默认为20）
        /// </summary>
        public static int ExecutionTimeInterval
        {
            get
            {
                int timeInterval = 0;

                if (!int.TryParse(ConfigurationManager.AppSettings["ExecutionTimeInterval"], out timeInterval))
                {
                    timeInterval = 20;
                }

                return timeInterval;
            }
        }

        public static string ProgramRunModel
        {
            get
            {
                return ConfigurationManager.AppSettings["RunningMode"]?.Trim() ?? "WindowService";
            }
        }
    }
}
