using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sunnet.Cli.Core.Trs
{
    //修改此枚举时，需要修改Module_TRS_Offline.js  Star
    public enum TRSStarEnum : byte
    {
        /// <summary>
        /// 1星(没有星级)
        /// </summary>
        [Description("Below 2 ★")]
        One = 1,

        /// <summary>
        /// 2星
        /// </summary>
        [Description("2 ★")]
        Two = 2,

        /// <summary>
        /// 3星
        /// </summary>
        [Description("3 ★")]
        Three = 3,

        /// <summary>
        /// 4星
        /// </summary>
        [Description("4 ★")]
        Four = 4
    }
}
