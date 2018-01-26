using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Sunnet.Framework.Core.Base;
using Sunnet.Cli.Core.Trs.Enums;

namespace Sunnet.Cli.Core.Trs.Entities
{
    public class TRSSubcategoryEntity : EntityBase<int>
    {
        public string Name { get; set; }

        public TRSCategoryEnum Category { get; set; }

        public TRSItemType Type { get; set; }

        public int Sort { get; set; }

        public virtual ICollection<TRSItemEntity> Items { get; set; }
    }
}
