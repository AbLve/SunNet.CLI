using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Sunnet.Cli.Core.Trs;

namespace Sunnet.Cli.Business.Trs.Models
{
    public class TrsCategoryStarModel
    {
        public int AssessmentId { get; set; }

        public TRSStarEnum Star { get; set; }

        public TRSCategoryEnum Category { get; set; }

        public bool Retain { get; set; }

        public bool AutoAssign { get; set; }
    }
}
