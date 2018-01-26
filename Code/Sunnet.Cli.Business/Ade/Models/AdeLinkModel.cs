using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
/**************************************************************************
 * Developer: 		JackZhang
 * Computer:		JACKZ
 * Domain:			JackZ
 * CreatedOn:		1/23 2015 10:32:44
 * Description:		Please input class summary
 * Version History:	Created,1/23 2015 10:32:44
 * 
 * 
 **************************************************************************/
using Sunnet.Cli.Core.Ade.Entities;

namespace Sunnet.Cli.Business.Ade.Models
{
    public class AdeLinkModel
    {
        public int ID { get; set; }

        /// <summary>
        /// 宿主类型.
        /// </summary>
        public string HostType { get; set; }

        /// <summary>
        /// 宿主ID.
        /// </summary>
        public int HostId { get; set; }

        /// <summary>
        /// 链接类型(Activity|Course|File...).
        /// </summary>
        public byte LinkType { get; set; }

        /// <summary>
        /// 链接地址.
        /// </summary>
        public string Link { get; set; }

        /// <summary>
        /// Display Text
        /// </summary>
        public string DisplayText { get; set; }

    }

}
