using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Sunnet.Cli.Core.Trs;
using Sunnet.Cli.Core.Trs.Enums;

namespace Sunnet.Cli.Business.Trs.Models
{
    public class TrsOfflineSubcategoryModel
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public TRSCategoryEnum Category { get; set; }

        public TRSItemType Type { get; set; }

        public int Sort { get; set; }

        public IEnumerable<TrsItemModel> Items { get; set; }
    }
}
