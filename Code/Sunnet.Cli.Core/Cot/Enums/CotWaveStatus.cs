using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


/**************************************************************************
 * Developer: 		JackZhang
 * Computer:		JACKZ
 * Domain:			Jackz
 * CreatedOn:		2014/12/16 9:09:06
 * Description:		Please input class summary
 * Version History:	Created,2014/12/16 9:09:06
 * 
 * 
 **************************************************************************/

namespace Sunnet.Cli.Core.Cot
{
    public enum CotWaveStatus : byte
    {
        Initialised = 1,
        OldData = 5,
        Saved = 10,
        Finalized = 20,
        Completed = 30
        
    }
}
