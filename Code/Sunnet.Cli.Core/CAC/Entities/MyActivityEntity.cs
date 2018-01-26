using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sunnet.Cli.Core.Common.Enums;
using Sunnet.Framework.Core.Base;

namespace Sunnet.Cli.Core.CAC.Entities
{
    public class MyActivityEntity : EntityBase<int>
    {
        public int ActivityId { get; set; }
        public int UserId { get; set; }

        public string ActivityName { get; set; }
        public EntityStatus Status { get; set; }

        public int CreatedBy { get; set; }
        public int UpdatedBy { get; set; }

        public string Remark { get; set; }

        public string Url { get; set; }
        public string Domain { get; set; }
        public string SubDomain { get; set; }

        public string Objective { get; set; }

        public string CollectionType { get; set; }

        public string AgeGroup { get; set; }
    }
}
