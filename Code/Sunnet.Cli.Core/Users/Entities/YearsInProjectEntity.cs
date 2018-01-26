using Sunnet.Cli.Core.Common.Enums;
using Sunnet.Framework.Core.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sunnet.Cli.Core.Users.Entities
{
    public class YearsInProjectEntity : EntityBase<int>
    {
        [Required]
        [StringLength(50)]
        public string YearsInProject { get; set; }
        public EntityStatus Status { get; set; }
    }
}
