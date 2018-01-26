using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/**************************************************************************
 * Developer: 		Joe
 * Computer:		JOE-PC
 * Domain:			Joe
 * CreatedOn:		2015/10/23
 * Description:		
 * Version History:	Created,2015/10/23
 * 
 * 
 **************************************************************************/

namespace Sunnet.Cli.Core.Ade.Enums
{
    //修改此枚举时，要修改Scripts_cliProject目录下Module_Ade.js的ImageType对象
    public enum ImageType : byte
    {
        Selectable = 1,
        [Description("Non-Selectable")]
        NonSelectable = 2
    }
}
