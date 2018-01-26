using Sunnet.Framework.Core.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sunnet.Cli.Core.Trs.Entities
{
    public class TRSAssessmentClassEntity : EntityBase<int>
    {
        public int AssessmentId { get; set; }

        public int ClassId { get; set; }

        public decimal ObservationLength { get; set; }

        public virtual TRSAssessmentEntity Assessment { get; set; }
    }
}
