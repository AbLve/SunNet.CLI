using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/**************************************************************************
 * Developer: 		Joe
 * Computer:		JOE-PC
 * Domain:			Joe
 * CreatedOn:		2015/11/8
 * Description:		Add TxkeaReceptive Item
 * Version History:	Created,2015/11/8
 * 
 * 
 **************************************************************************/
using Sunnet.Cli.Core.Ade.Entities;
using Sunnet.Cli.Core.Ade.Enums;

namespace Sunnet.Cli.Business.Ade.Models
{
    public class TxkeaLayoutModel : TxkeaLayoutEntity
    {
        public string CreatedUserName { get; set; }

        public string UpdatedUserName { get; set; }

        public string UpdatedOnConvert { get; set; }

        public int RelatedItemsCount { get; set; }
        
        //缩略图显示的Html
        public string LayoutHtml { get; set; }

        public decimal layoutWidth
        {
            get { return 100 / (600 / base.ScreenWidth); }
        }

        //生成缩略图地址
        public string LayoutPng {
            get { return string.Format("{0}.png", ID); }
        }
    }
}
