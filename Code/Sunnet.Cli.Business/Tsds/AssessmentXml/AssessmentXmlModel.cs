using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sunnet.Cli.Business.Tsds.AssessmentXml
{
    public class AssessmentXmlModel
    {
        public int id { get; set; }

        public List<MeasureXmlModel> Measures { get; set; }

        public List<ItemXmlModel> Items { get; set; }
    }
}
