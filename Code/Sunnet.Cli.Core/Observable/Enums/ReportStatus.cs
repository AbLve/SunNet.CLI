using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/**************************************************************************
 * Developer: 		Leason
 * Computer:		Leason-PC
 * Domain:			Leason-pc
 * CreatedOn:		2014/8/27 14:48:00
 * Description:		Create DayType
 * Version History:	Created,2014/8/27 14:48:00
 * 
 * 
 **************************************************************************/
namespace Sunnet.Cli.Core.Observable.Enums
{
    public enum ReportStatus : byte
    {
        [Description("Show All")]
        ShowAll=1,

        [Description("Show most recent")]
        Showmostrecent=2,

        [Description("Show first")]
        Showfirst=3 
    }
}
