using Sunnet.Framework.Core.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sunnet.Cli.Core.Cot.Entities
{
    public class CotStgGroupItemEntity : EntityBase<int>
    {
        public int CotStgGroupId { get; set; }
        public int ItemId { get; set; }
        public int Sort { get; set; }

        public virtual CotStgGroupEntity CotStgGroup { get; set; }

        public virtual CotAssessmentItemEntity Item { get; set; }
    }
}
