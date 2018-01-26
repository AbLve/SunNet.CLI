using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sunnet.Cli.Core.Common.Enums;
using Sunnet.Framework.Core.Base;

namespace Sunnet.Cli.Core.CAC.Entities
{
    public class ActivityHistoryEntity : EntityBase<int>
    {
        public int ActivityId { get; set; }
        public int EngageID { get; set; }
        public string GoogleID { get; set; }
        public string ActivityName { get; set; }  

        public string Remark { get; set; }

    }
}
