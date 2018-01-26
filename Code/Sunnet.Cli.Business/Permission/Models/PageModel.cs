using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/**************************************************************************
 * Developer: 		Joe
 * Computer:		JOE-PC
 * Domain:			Joe-pc
 * CreatedOn:		2014/9/2 12:15:10
 * Description:		Please input class summary
 * Version History:	Created,2014/9/2 12:15:10
 * 
 * 
 **************************************************************************/
using Sunnet.Cli.Core.Permission.Entities;
using System.ComponentModel.DataAnnotations;

namespace Sunnet.Cli.Business.Permission.Models
{
    public class PageModel
    {
        public PageModel()
        {
            Authorities = new List<AuthorityEntity>();
        }
        public int ID { get; set; }

        public string Name { get; set; }

        [Display(Name="Type")]
        public bool IsPage { get; set; }

        [Display(Name = "ParentMenu")]
        public int ParentID { get; set; }

        public string Url { get; set; }

        public int Sort { get; set; }

        public bool IsShow { get; set; }

        public string Descriptions { get; set; }

        public bool IsSelected { get; set; }

        public int TmpCount { get; set; }

        //多对多关系生成的外键关联表
        public ICollection<AuthorityEntity> Authorities { get; set; }
    }
}
