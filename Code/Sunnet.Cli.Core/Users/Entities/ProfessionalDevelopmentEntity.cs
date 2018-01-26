using Sunnet.Cli.Core.Common.Enums;
using Sunnet.Cli.Core.Users.Entities;
using Sunnet.Framework.Core.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sunnet.Cli.Core.Users.Entities
{
    public class ProfessionalDevelopmentEntity : EntityBase<int>
    {
        public string ProfessionalDevelopment { get; set; }
        public EntityStatus Status { get; set; }
        public virtual ICollection<UserBaseEntity> Users { get; set; }
    }
}
