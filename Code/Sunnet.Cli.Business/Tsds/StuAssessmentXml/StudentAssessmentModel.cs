using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sunnet.Cli.Business.Tsds.Models
{
    public class StudentAssessmentModel
    {
        public string id { get; set; }

        public string AdministrationDate { get; set; }

        public string AdministrationLanguage { get; set; }

        public string AssessmentReportingMethod { get; set; }

        public string Result { get; set; }

        public string GradeLevelWhenAssessed { get; set; }

        public string Description { get; set; }

        public string PerformanceLevelMet { get; set; }

        public string StudentUniqueStateId { get; set; }

        public string AssessmentReference { get; set; }
    }
}
