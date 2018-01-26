using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/**************************************************************************
 * Developer: 		Lee
 * Computer:		Lee-PC
 * Domain:			Lee-pc
 * CreatedOn:		2014/8/25 20:27:20
 * Description:		Create LanguageEntity
 * Version History:	Created,2014/8/25 20:27:20
 * 
 * 
 **************************************************************************/
using Sunnet.Cli.Core.Classes.Entites;
using Sunnet.Cli.Core.Common.Enums;
using Sunnet.Framework.Core.Base;
namespace Sunnet.Cli.Core.MasterData.Entities
{
    public class LanguageEntity : EntityBase<int>
    {
        [Required]
        [StringLength(50)]
        public string Language { get; set; }
        public EntityStatus Status { get; set; }

        public virtual ICollection<ClassEntity> Classeses { get; set; } 
    }
}
