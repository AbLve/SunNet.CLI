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
 * Description:		Create AuthorityEntity
 * Version History:	Created,2014/9/2
 * 
 * 
 **************************************************************************/
using Sunnet.Framework.Core.Base;
using System.ComponentModel.DataAnnotations;

namespace Sunnet.Cli.Core.Permission.Entities
{
    public class AuthorityEntity : EntityBase<int>
    {
        [Required]
        [StringLength(50)]
        public string Authority { get; set; }

        [Required]
        [StringLength(100)]
        public string Descriptions { get; set; }


        //权限和页面  生成一个中间表  Permission_PageAuthorities
        public virtual ICollection<PageEntity> Pages { get; set; }
    }
}
