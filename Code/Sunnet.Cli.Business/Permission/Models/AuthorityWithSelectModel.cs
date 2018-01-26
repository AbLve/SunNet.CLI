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

namespace Sunnet.Cli.Business.Permission.Models
{
    /// <summary>
    /// 页面权限Model，带IsSelected 属性
    /// </summary>
    public class AuthorityWithSelectModel
    {
        public int ID { get; set; }

        public string Authority { get; set; }

        public int RoleId { get; set; }

        public int PageId { get; set; }

        /// <summary>
        /// 是否选中权限复选框
        /// </summary>
        public bool IsSelected { get; set; }
    }
}
