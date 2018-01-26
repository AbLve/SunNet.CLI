using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/**************************************************************************
 * Developer: 		JackZhang
 * Computer:		JACKZ
 * Domain:			Jackz
 * CreatedOn:		2014/8/27 20:05:25
 * Description:		Please input class summary
 * Version History:	Created,2014/8/27 20:05:25
 * 
 * 
 **************************************************************************/
using Newtonsoft.Json;

namespace Sunnet.Framework.Extensions
{
    /// <summary>
    /// 下拉框的选择项
    /// </summary>
    public class SelectItemModel
    {
        private List<dynamic> _props;

        public int ID { get; set; }

        public string Name { get; set; }

        public bool Selected { get; set; }

        public dynamic Props { get; set; }

    }
}
