using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sunnet.Cli.Core.Ade;

namespace Sunnet.Cli.Business.Communities.Models
{
    public class AssessmentFeatureClassLevel
    {
        public int AssessmentId { get; set; }
        public List<int> ClassLevels { get; set; }
    }
    public class AssessmentFeatureModel
    {
        public string ID { get; set; }

        public string Name { get; set; }

        public AssessmentLanguage Language { get; set; }
    }
}
