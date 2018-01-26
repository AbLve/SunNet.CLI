using Sunnet.Cli.Core.Cot.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sunnet.Cli.Business.Cot.Models
{
    public class CotStgGroupModel
    {
        public int ID { get; set; }
        public int CotStgReportId { get; set; }
        public string GroupName { get; set; }
        public string OnMyOwn { get; set; }
        public string WithSupport { get; set; }
        
        public List<CotStgGroupItemModel> StgGroupItems { get; set; }
    }
}
