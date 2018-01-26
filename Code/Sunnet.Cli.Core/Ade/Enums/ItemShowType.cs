using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/**************************************************************************
 * Developer: 		JackZhang
 * Computer:		JACKZ
 * Domain:			Jackz
 * CreatedOn:		2014/8/12 3:14:10
 * Description:		Please input class summary
 * Version History:	Created,2014/8/12 3:14:10
 * 
 * 
 **************************************************************************/
namespace Sunnet.Cli.Core.Ade
{
    /* *******************************************************************************
     * *******************************************************************************
     * 更新时注意更新 Sunnet.Cli.Static\Content\Scripts_cliProject\Module_Cpalls.js *****
     * ********************************************************************************
     *********************************************************************************/

    /// <summary>
    /// Item展示类型(Cot, Cec, Checklist 是List; Cpalls:使用OrderType)
    /// </summary>
    public enum ItemShowType : byte
    {
        Sequenced = 2,
        List = 1
    }
}
