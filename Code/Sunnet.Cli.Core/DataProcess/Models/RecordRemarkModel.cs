using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sunnet.Cli.Core.DataProcess.Models
{
    public class RecordRemarkModel
    {
        public string CommunityName { get; set; }

        public string CommunityInternalID { get; set; }

        public string SchoolName { get; set; }

        public string SchoolInternalID { get; set; }

        public string TeacherFirstName { get; set; }

        public string TeacherLastName { get; set; }

        public string TeacherInternalID { get; set; }      

        public string TeacherPrimaryEmail { get; set; }
       
        public string StudentFirstName { get; set; }

        public string StudentLastName { get; set; }

        public string StudentInternalID { get; set; }

        public string StudentTsdsID { get; set; }

        public DateTime StudentBirthDate { get; set; }

        public int LineNum { get; set; }

        public string Remark { get; set; }
    }
}
