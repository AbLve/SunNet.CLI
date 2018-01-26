using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sunnet.Cli.Core.Reports.Models
{
    public  class Community_Mentor_TeacherModel
    {
        public int CommunityID { get; set; }

        public string CommunityName { get; set; }

        public string PM_FirstName { get; set; }

        public string PM_LastName { get; set; }

        public int PMID { get; set; }

        public int CoachId { get; set; }

        public string Coach_FirstName { get; set; }

        public string Coach_lastName { get; set; }

        public int Total { get; set; }
    }
}
