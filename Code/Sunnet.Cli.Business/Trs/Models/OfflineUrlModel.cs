using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sunnet.Cli.Business.Trs.Models
{
    public  class OfflineUrlModel
    {
        public int communityId { get; set; }
        public int schoolId { get; set; }
        public string schoolName { get; set; }
        public string director { get; set; }
        public string sort { get; set; }
        public string order { get; set; }
        public int first { get; set; }
        public int count { get; set; }
    }
}
