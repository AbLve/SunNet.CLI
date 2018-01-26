using Sunnet.Cli.Core.Common.Enums;
using Sunnet.Cli.Core.Communities.Entities;
using Sunnet.Framework.Core.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sunnet.Cli.Core.Schools.Entities
{
    public class CommunitySchoolRelationsEntity : EntityBase<int>
    {
        public int CommunityId { get; set; }
        public int SchoolId { get; set; }
        public EntityStatus Status { get; set; }
        public int CreatedBy { get; set; } 
        public int UpdatedBy { get; set; }
        public virtual CommunityEntity Community { get; set; }
        public virtual SchoolEntity School { get; set; }
    }
}
