using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sunnet.Cli.Business.Tsds.Models
{
    public class AssessmentReferenceModel
    {
        public string ReferenceId { get; set; }

        public string AssessmentTitle { get; set; }

        public string AssessmentCategory { get; set; }

        public string AcademicSubject { get; set; }

        public string GradeLevelAssessed { get; set; }

        public string Version { get; set; }
    }
}
