using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sunnet.Cli.Business.Tsds.Models
{
    public class StudentAssessmentItemModel
    {
        public string AssessmentResponse { get; set; }

        public string AssessmentItemResult { get; set; }

        public string RawScoreResult { get; set; }

        public string StudentTestAssessmentReference { get; set; }

        public string AssessmentItemIdentificationCode { get; set; }
    }
}
