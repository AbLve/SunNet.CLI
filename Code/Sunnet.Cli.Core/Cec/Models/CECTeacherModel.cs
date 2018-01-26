using Sunnet.Cli.Core.Cot.Models;
using Sunnet.Cli.Core.Cpalls;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sunnet.Cli.Core.Cec.Models
{
    public class CECTeacherModel : AssessmentTeacherModel
    {
        public CECTeacherModel()
        {
        }

        public CECTeacherModel(IDataReader reader, string schoolYear)
        {
            ID = (int)reader["ID"];
            
            FirstName = (string)reader["FirstName"];
            LastName = (string)reader["LastName"];
            TeacherID = (string)reader["TeacherID"];
            CoachId = (int)reader["CoachId"];
            CoachFirstName = reader["CoachFirstName"].ToString();
            CoachLastName = reader["CoachLastName"].ToString();
            YearsInProjectId = (int)reader["YearsInProjectId"];
            YearsInProject = (string)reader["YearsInProject"];
            SchoolYear = schoolYear;
            BOY = (string)reader["BOY"];
            MOY = (string)reader["MOY"];
            EOY = (string)reader["EOY"];
        }


        public string SchoolYear { get; set; }

        /// <summary>
        /// 日期，可为空字符
        /// </summary>
        public string BOY { get; set; }

        public string MOY { get; set; }

        public string EOY { get; set; }


        public string CoachName
        {
            get { return string.Format("{0} {1}", CoachFirstName, CoachLastName); }
        }
    }


    public class CECTeacherModel_VCW
    {
        public int AssessmentId { get; set; }

        public DateTime AssessmentDate { get; set; }

        public Wave Wave { get; set; }

        public int TeacherId { get; set; }
    }
}

