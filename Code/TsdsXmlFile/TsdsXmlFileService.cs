using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace TsdsXmlFile
{
    partial class TsdsXmlFileService : ServiceBase
    {
        private static Timer timerWatcher;
        private static Timer mainTimer;
        private bool executing = false;
        private QueueManager queueManager;

        public TsdsXmlFileService()
        {
            InitializeComponent();
            queueManager = new QueueManager();
            queueManager.BeforeProcessQueues += queueManager_BeforeProcessQueues;
            queueManager.AfterProcessQueues += queueManager_AfterProcessQueues;
        }

        void queueManager_BeforeProcessQueues()
        {
            executing = true;
        }

        void queueManager_AfterProcessQueues()
        {
            executing = false;
        }

        protected override void OnStart(string[] args)
        {
            Config.Instance.Logger.Info("Service Started");

            timerWatcher = new Timer(Config.Instance.WatchInternal);
            timerWatcher.Enabled = true;
            timerWatcher.Elapsed += timer_Elapsed;


            mainTimer = new Timer(Config.Instance.Internal);
            mainTimer.Enabled = true;
            mainTimer.Elapsed += MainTimerOnElapsed;

            MainTimerOnElapsed(null, null);
        }

        private void MainTimerOnElapsed(object sender, ElapsedEventArgs elapsedEventArgs)
        {
            if (executing)
                return;
            queueManager.Start();
        }

        void timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            Config.Instance.Logger.Info("Service running");
        }

        protected override void OnStop()
        {
            Config.Instance.Logger.Info("Service stopped");
        }
    }
}
