using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StructureMap;
using Sunnet.Framework;
using Sunnet.Framework.Log;

namespace ReportQueue
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

        public string MainSiteDomain
        {
            get { return ConfigurationManager.AppSettings["MainSiteDomain"]; }
        }

        public string AssessmentDomain
        {
            get { return ConfigurationManager.AppSettings["AssessmentDomain"]; }
        }

        public string SsoDomain
        {
            get { return ConfigurationManager.AppSettings["SsoDomain"]; }
        }

        public string LmsDomain
        {
            get { return ConfigurationManager.AppSettings["LMSDomain"]; }
        }

        public string VcwDomain
        {
            get { return ConfigurationManager.AppSettings["VcwDomain"]; }
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

        public ISunnetLog Logger
        {
            get { return ObjectFactory.GetInstance<ISunnetLog>(); }
        }

        public string TemplatePath
        {
            get { return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources"); }
        }

        /// <summary>
        /// 这个应用程序处理哪些类型的报表
        /// </summary>
        /// Author : JackZhang
        /// Date   : 6/26/2015 14:19:20
        public List<int> ProcessTypes
        {
            get
            {
                var types = ConfigurationManager.AppSettings["ProcessTypes"];
                return types.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToList();
            }
        }

    }
}
