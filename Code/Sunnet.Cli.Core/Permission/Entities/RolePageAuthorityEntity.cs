using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/**************************************************************************
 * Developer: 		Joe
 * Computer:		Joe-PC
 * Domain:			Joe-pc
 * CreatedOn:		2014/9/2
 * Description:		Create RolePageAuthorityEntity
 * Version History:	Created,2014/9/2
 * 
 * 
 **************************************************************************/
using Sunnet.Framework.Core.Base;
using System.ComponentModel.DataAnnotations;

namespace Sunnet.Cli.Core.Permission.Entities
{
    public class RolePageAuthorityEntity : EntityBase<int>
    {
        [Required]
        public int RoleId { get; set; }

        [Required]
        public int PageId { get; set; }

        [Required]
        [StringLength(100)]
        public string PageAction { get; set; }
    }
}
