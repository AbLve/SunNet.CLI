using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Sunnet.Cli.Core.BUP;
using Sunnet.Cli.Core.DataProcess;

namespace Sunnet.Cli.Business.DataProcess.Models
{
    public class DataProcessModel
    {
        public DataProcessAction Action { get; set; }

        public string CommunityName { get; set; }

        public string CommunityInternalID { get; set; }

        public string SchoolName { get; set; }

        public string SchoolInternalID { get; set; }

        public string Teacher_FirstName { get; set; }

        public string Teacher_MiddleName { get; set; }

        public string Teacher_LastName { get; set; }

        public string Teacher_InternalID { get; set; }

        public string Teacher_PhoneNumber { get; set; }

        public byte Teacher_PhoneType { get; set; }

        public string Teacher_PrimaryEmail { get; set; }

        public byte Class_DayType { get; set; }

        public int Class_Level { get; set; }

        public string Student_FirstName { get; set; }

        public string Student_MiddleName { get; set; }

        public string Student_LastName { get; set; }

        public string Student_InternalID { get; set; }

        public string Student_TsdsID { get; set; }

        public byte Student_GradeLevel { get; set; }

        public byte Student_AssessmentLanguage { get; set; }

        public DateTime Student_BirthDate { get; set; }

        public byte Student_Gender { get; set; }

        public byte Student_Ethnicity { get; set; }

        public int LineNum { get; set; }

        public string Remark { get; set; }

        // 1:School Error   2:Teacher Error  3:Student Error  4:Record Error
        public byte RemarkType { get; set; }
    }
}
