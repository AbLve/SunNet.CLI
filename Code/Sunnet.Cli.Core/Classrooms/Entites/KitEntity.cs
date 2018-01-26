using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sunnet.Cli.Core.Classrooms.Entites;
using Sunnet.Cli.Core.Common.Enums;
using Sunnet.Framework.Core.Base;

namespace Sunnet.Cli.Core.Classrooms.Entites
{
    public class KitEntity:EntityBase<int>
    {
        [Required]
        [StringLength(150)]
        public string Name { get; set; }
        public EntityStatus Status { get; set; }

        public virtual ICollection<ClassroomEntity> Classrooms { get; set; } 

    }
}
