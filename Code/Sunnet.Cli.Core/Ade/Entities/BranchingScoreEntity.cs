using Sunnet.Framework.Core.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sunnet.Cli.Core.Ade.Entities
{
    public class BranchingScoreEntity : EntityBase<int>
    {
        public decimal From { get; set; }

        public decimal To { get; set; }
        
        public int ItemId { get; set; }

        public int SkipItemId { get; set; }

        public bool IsDeleted { get; set; }

        public virtual ItemBaseEntity ItemBase { get; set; }
    }
}
