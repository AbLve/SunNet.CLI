using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using DataManagement.Business;

namespace DataManagement.WindowService
{
    partial class DataManageMentService : ServiceBase
    {
        System.Timers.Timer processingForRosterTimer;
        System.Timers.Timer processingForTaskTimer;

        public DataManageMentService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            #region processingForRoster

            processingForRosterTimer = new System.Timers.Timer();
            processingForRosterTimer.Interval = GetTimerIntervalForMillisecond();
            processingForRosterTimer.Enabled = true;
            processingForRosterTimer.Elapsed += new System.Timers.ElapsedEventHandler(ProcessingForRoster);
            processingForRosterTimer.Start();

            #endregion

            #region processingForTask

            processingForTaskTimer = new System.Timers.Timer();
            processingForTaskTimer.Interval = GetTimerIntervalForMillisecond();
            processingForTaskTimer.Enabled = true;
            processingForTaskTimer.Elapsed += new System.Timers.ElapsedEventHandler(ProcessingForTask);
            processingForTaskTimer.Start();

            #endregion
        }

        protected override void OnStop()
        {
            if (processingForRosterTimer != null)
            {
                processingForRosterTimer.Close();
                processingForRosterTimer.Dispose();
            }

            if (processingForTaskTimer != null)
            {
                processingForTaskTimer.Close();
                processingForTaskTimer.Dispose();
            }
        }

        #region

        void ProcessingForRoster(object sender, System.Timers.ElapsedEventArgs e)
        {
            BusinessFacade.GetRosterManager().QueuedSingleRoster();
        }

        void ProcessingForTask(object sender, System.Timers.ElapsedEventArgs e)
        {
            BusinessFacade.GetTaskManager().ExecuteTask();
        }

        /// <summary>
        /// 配置文件中获取执行时间间隔的毫秒数
        /// </summary>
        /// <returns>毫秒数</returns>
        long GetTimerIntervalForMillisecond()
        {
          return  AppConfigManager.ExecutionTimeInterval * 60 * 1000;
        }

        #endregion
    }
}
