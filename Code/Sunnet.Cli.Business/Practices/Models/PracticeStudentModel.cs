using Sunnet.Cli.Business.Common;
using Sunnet.Cli.Core.Ade;
using Sunnet.Cli.Core.Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sunnet.Cli.Business.Practices.Models
{
    public class PracticeStudentModel
    {
        public int ID { get; set; }
        public int StudentId { get; set; }
        public string StudentName { get; set; }
        public DateTime StudentDob
        {
            get
            {
                var time = CommonAgent.GetStartDateForAge(CommonAgent.SchoolYear);
                return time.AddYears(-StudentAgeYear).AddMonths(-StudentAgeMonth);
            }
        }
        public int StudentAgeYear { get; set; }
        public int StudentAgeMonth { get; set; }


        public StudentAssessmentLanguage AssessmentLanguage { get; set; }
        public string DataFrom { get; set; }
        public string Remark { get; set; }
        public int AssessmentId { get; set; }
        public EntityStatus Status { get; set; }
    }
}
