using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sunnet.Cli.Core.Reports.Models
{
    public  class CountbyCommunityModel
    {
        public string CommunityName { get; set; }

        public int CommunityId { get; set; }

        public int TeacherTotal { get; set; }

        public int ClassroomTotal { get; set; }

        public int FundingId { get; set; }

        public int StudentTotal { get; set; }
    }
}
