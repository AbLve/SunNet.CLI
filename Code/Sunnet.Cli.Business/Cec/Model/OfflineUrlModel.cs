using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sunnet.Cli.Business.Cec.Model
{
    public  class OfflineUrlModel
    {
        public int assessmentId { get; set; }
        public string teacherCode { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public int year { get; set; }
        public int communityId { get; set; }
        public string communityName { get; set; }
        public string schoolName { get; set; }
        public int schoolId { get; set; }
        public string sort { get; set; }
        public string order { get; set; }
        public int first { get; set; }
        public int count { get; set; }
    }
}
