using Sunnet.Cli.Core.Ade;
using Sunnet.Cli.Core.Cpalls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sunnet.Cli.Business.Ade.Models
{
    public class BenchmarkModel
    {
        public int ID { get; set; }
        public int AssessmentId { get; set; }

        public string LabelText { get; set; }

        public string Color { get; set; }

        public BlackWhiteStyle BlackWhite { get; set; }
    }
}
