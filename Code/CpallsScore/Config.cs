using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StructureMap;
using Sunnet.Framework.Log;

namespace CpallsScore
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
    }
}
