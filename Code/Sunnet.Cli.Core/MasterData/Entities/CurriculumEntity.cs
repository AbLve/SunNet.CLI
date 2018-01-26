using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/**************************************************************************
 * Developer: 		Sam
 * Computer:		SAM-PC
 * Domain:			sam-pc
 * CreatedOn:		2014/8/27 12:11:39
 * Description:		Please input class summary
 * Version History:	Created,2014/8/27 12:11:39
 * 
 * 
 **************************************************************************/
using Sunnet.Cli.Core.Classes.Entites;
using Sunnet.Cli.Core.Classrooms.Entites;
using Sunnet.Cli.Core.Common.Enums;
using Sunnet.Framework.Core.Base;

namespace Sunnet.Cli.Core.MasterData.Entities
{
    public class CurriculumEntity : EntityBase<int>
    {
        [Required]
        [StringLength(150)]
        public string Name { get; set; }

        public EntityStatus Status { get; set; }

        public virtual ICollection<ClassroomEntity> Classrooms { get; set; }
        public virtual ICollection<ClassEntity> Classes { get; set; }
    }
}
