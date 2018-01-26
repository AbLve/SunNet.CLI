using System.Drawing;
using Sunnet.Cli.Core.Ade;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sunnet.Cli.Business.Cpalls.Models
{
    public class CpallsAssessmentModel
    {
        public int ID { get; set; }

        public string Name { get; set; }
        public string DisplayName { get; set; }
        public AssessmentType Type { get; set; }
        public AssessmentLanguage Language { get; set; }
        public string Description { get; set; }
    }
}
 