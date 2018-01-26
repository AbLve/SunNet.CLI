using Sunnet.Cli.Core.Common.Enums;
using Sunnet.Cli.Core.Users.Entities;
using Sunnet.Framework.Core.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sunnet.Cli.Core.Users.Entities
{
    public class CertificateEntity : EntityBase<int>
    {
        public string Certificate { get; set; }

        public bool IsShow { get; set; }

        public EntityStatus Status { get; set; }

        public virtual ICollection<UserBaseEntity> Users { get; set; }
    }
}
