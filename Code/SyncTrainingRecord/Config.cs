using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sunnet.Framework.Log;

namespace SyncTrainingRecord
{
    internal class Config
    {
        private Config()
        {

        }

        internal static Config Instance
        {
            get
            {
                return new Config();
            }
        }
        
        public int Internal
        {
            get
            {
                var internalSec = 60 * 3;
                int.TryParse(ConfigurationManager.AppSettings["Interval"], out internalSec);
                return internalSec * 1000;
            }
        }

        public int WatchInternal
        {
            get
            {
                var internalSec = 5;
                int.TryParse(ConfigurationManager.AppSettings["WatchInternal"], out internalSec);
                return internalSec * 1000;
            }
        }

        public string LogFile
        {
            get
            {
                return ConfigurationManager.AppSettings["LogFile"];
            }
        }

        public log4netSync Logger
        {
            get { return new log4netSync(); }
        }

        public string TemplatePath
        {
            get { return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources"); }
        }
        public string DataPath
        {
            get { return ConfigurationManager.AppSettings["DataPath"]; }
        }
        public string UploadFile
        {
            get { return ConfigurationManager.AppSettings["UploadFile"]; }
        }
    }
}
