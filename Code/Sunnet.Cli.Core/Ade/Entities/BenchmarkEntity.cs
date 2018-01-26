using Sunnet.Cli.Core.Cpalls;
using Sunnet.Framework.Core.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sunnet.Cli.Core.Ade.Entities
{
    public class BenchmarkEntity : EntityBase<int>
    {
        public int AssessmentId { get; set; }

        public string LabelText { get; set; }

        public string Color { get; set; }

        public BlackWhiteStyle BlackWhite { get; set; }
    }
}
