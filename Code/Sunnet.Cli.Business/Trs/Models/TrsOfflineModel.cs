using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sunnet.Cli.Business.Trs.Models
{
    public class TrsOfflineModel
    {
        public TrsOfflineModel()
        {
            Schools = new List<TrsOfflineSchoolModel>();
        }

        public List<TrsOfflineSchoolModel> Schools { get; set; }

        //Assessment通用结构
        public object AssessmentStructure { get; set; }
    }
}
