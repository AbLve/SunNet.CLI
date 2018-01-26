using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sunnet.Cli.Core.Cot.Models
{
    public class CotStgGroupItemModel
    {
        public int CotStgReportId { get; set; }
        public int CotStgGroupId { get; set; }
        public string GroupName { get; set; }
        public int ItemId { get; set; }
        public byte Level { get; set; }
        public string ShortTargetText { get; set; }
        public string FullTargetText { get; set; }
        public string CotItemId { get; set; }
        public int Sort { get; set; }
    }
}
