using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sunnet.Cli.Core.Cot.Models
{
    public  class TeacherMissingSTGRModel
    {
        public string CommunityName { get; set; }

        public int CommunityID { get; set; }

        public string SchoolName { get; set; }

        public int SchoolID { get; set; }

        public string TeacherFirstName { get; set; }

        public string TeacherLastName { get; set; }

        public int CoachId { get; set; }

        public string CoachFirstName { get; set; }

        public string CoachLastName { get; set; }

        public string PM { get; set; }
    }
}
