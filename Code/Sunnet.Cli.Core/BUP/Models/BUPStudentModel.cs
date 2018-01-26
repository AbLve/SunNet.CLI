using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sunnet.Cli.Core.Ade;

namespace Sunnet.Cli.Core.BUP.Models
{
    public class BUPStudentModel
    {
        public int TaskId { get; set; }

        public BUPAction Action { get; set; }

        public string CommunityName { get; set; }

        public string CommunityEngageID { get; set; }

        public string SchoolName { get; set; }

        public string SchoolEngageID { get; set; }

        public string FirstName { get; set; }

        public string MiddleName { get; set; }

        public string LastName { get; set; }

        public string StudentEngageId { get; set; }

        /// <summary>
        /// Student_Internal_ID
        /// </summary>
        public string LocalStudentID { get; set; }

        public DateTime BirthDate { get; set; }

        public byte Gender { get; set; }

        public byte Ethnicity { get; set; }

        public string TSDSStudentID { get; set; }

        public byte GradeLevel { get; set; }

        public string ClassName { get; set; }

        public string ClassEngageID { get; set; }

        public string ClassroomName { get; set; }

        public string ClassroomEngageId { get; set; }

        public BUPStatus Status { get; set; }

        public string Remark { get; set; }

        public int LineNum { get; set; }

        public StudentAssessmentLanguage AssessmentLanguage { get; set; }

        public BUPEntityStatus StudentStatus { get; set; }
    }
}
