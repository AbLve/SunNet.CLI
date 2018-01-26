using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/**************************************************************************
 * Developer: 		Leason
 * Computer:		Leason-PC
 * Domain:			Leason-pc
 * CreatedOn:		2014/8/27 16:16:16
 * Description:		Create MonitoringToolEntity
 * Version History:	Created,2014/8/27 16:16:16
 * 
 * 
 **************************************************************************/
using Sunnet.Cli.Core.Common.Enums;
using Sunnet.Framework.Core.Base;

namespace Sunnet.Cli.Core.Classes.Entites
{
    public class MonitoringToolEntity:EntityBase<Int32>
    {
        [Required]
        [StringLength(150)]
        public string Name { get; set; }
        public EntityStatus Status { get; set; }
        public virtual ICollection<ClassEntity> Classes { get; set; } 
    }
}
