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
    public class PositionEntity : EntityBase<Int32>
    {
        [Required]
        public int UserType { get; set; }
        public EntityStatus Status { get; set; }
        [Required]
        [StringLength(100)]
        public string Title { get; set; }
    }
}
