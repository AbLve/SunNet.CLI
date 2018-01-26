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
 * Description:		Create CountyEntity
 * Version History:	Created,2014/9/2
 * 
 * 
 **************************************************************************/
using Sunnet.Framework.Core.Base;
using System.ComponentModel.DataAnnotations;
using Sunnet.Framework.Mvc;


namespace Sunnet.Cli.Core.Permission.Entities
{
    public class PageEntity : EntityBase<int>
    {
        [StringLength(100)]
        [Required]
        public string Name { get; set; }

        [Required]
        [Display(Name = "Type")]
        public bool IsPage { get; set; }

        [Required]
        [Display(Name="ParentMenu")]
        public int ParentID { get; set; }

        [EensureEmptyIfNull]
        [StringLength(100)]
        public string Url { get; set; }

        public int Sort { get; set; }

        public bool IsShow { get; set; }

        [EensureEmptyIfNull]
        [StringLength(200)]
        public string Descriptions { get; set; }

        //权限和页面  生成一个中间表  Permission_PageAuthorities
        public virtual ICollection<AuthorityEntity> Authorities { get; set; }
    }
}
