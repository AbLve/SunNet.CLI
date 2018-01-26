using System.ComponentModel;

/**************************************************************************
 * Developer: 		JackZhang
 * Computer:		JACKZ
 * Domain:			Jackz
 * CreatedOn:		2014/9/4 4:59:18
 * Description:		Please input class summary
 * Version History:	Created,2014/9/4 4:59:18
 * JS:Sunnet.Cli.Static\Content\Scripts_cliProject\Module_Ade.js
 * 
 **************************************************************************/

namespace Sunnet.Cli.Core.Cpalls
{
    public enum Wave : byte
    {
        /// <summary>
        /// Sep 15 - Oct 15
        /// 9.15 - 10.15
        /// </summary>
        [Description("1")]
        BOY = 1,

        /// <summary>
        /// Jan 15 - Feb 15
        /// 1.15-2.15
        /// </summary>
        [Description("2")]
        MOY = 2,

        /// <summary>
        /// Mar 15 - May 15
        /// 3.15-4.15
        /// </summary>
        [Description("3")]
        EOY = 3
    }

    public enum QueryLevel : byte
    {
        Community,
        School,
        Class,
        Student
    }
}
