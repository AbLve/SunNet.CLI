using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sunnet.Cli.Core.Trs.Enums
{
    //修改此枚举时，需要修改Module_TRS_Offline.js   ItemType
    public enum TRSItemType : byte
    {
        Structural = 1,
        Process = 2
    }
}
