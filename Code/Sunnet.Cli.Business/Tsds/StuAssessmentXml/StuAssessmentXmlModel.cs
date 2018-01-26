using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sunnet.Cli.Business.Tsds.Models
{
    public class StuAssessmentXmlModel
    {
        public List<StudentReferenceModel> students { get; set; }

        public List<AssessmentReferenceModel> assessments { get; set; }

        public List<StudentAssessmentModel> studentAssessments { get; set; }

        public List<StudentAssessmentItemModel> items { get; set; }
    }
}
