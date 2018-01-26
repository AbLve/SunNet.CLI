using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sunnet.Cli.Core.Ade;
using Sunnet.Cli.Core.Cpalls;

namespace Sunnet.Cli.Business.Practices.Models
{
   public class ExcelModel
    {
        public string MeaureName { get; set; }
        public string ItemLabel { get; set; }
        public string StudentName { get; set; }
       public Wave Wave { get; set; }
       public int AgeYear { get; set; }
        public int AgeMonth { get; set; }
        public int ItemValue { get; set; }
        public StudentAssessmentLanguage AssessmentLanguage { get; set; } 
    }
}
