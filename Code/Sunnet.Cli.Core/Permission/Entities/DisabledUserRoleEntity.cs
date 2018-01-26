using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/**************************************************************************
 * Developer: 		Joe
 * Computer:		Joe-PC
 * Domain:			Joe-pc
 * CreatedOn:		2015/7/22
 * Description:		Create CountyEntity
 * Version History:	Created,2015/7/22
 * 
 * 
 **************************************************************************/
using Sunnet.Framework.Core.Base;
using System.ComponentModel.DataAnnotations;
using Sunnet.Cli.Core.Users.Entities;

namespace Sunnet.Cli.Core.Permission.Entities
{
    public class DisabledUserRoleEntity : EntityBase<int>
    {
        [Required]
        public int UserId { get; set; }

        [Required]
        public int RoleId { get; set; }

        public virtual UserBaseEntity User { get; set; }
    }
}
