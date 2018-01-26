using Sunnet.Framework.Core.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sunnet.Cli.Core.Trs.Entities
{
    public class TrsStarEntity : EntityBase<int>
    {
        public int AssessmentId { get; set; }

        public int ClassId { get; set; }

        public TRSStarEnum Star { get; set; }

        public TRSCategoryEnum Category { get; set; }

        public virtual TRSAssessmentEntity Assessment { get; set; }

        public bool Retain { get; set; }

        public bool AutoAssign { get; set; }
    }
}
