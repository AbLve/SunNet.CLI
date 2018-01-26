using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sunnet.Cli.Core.Trs
{
    public enum TRSStarDisplayEnum
    {
        /// <summary>
        /// 1星(没有星级)
        /// </summary>
        [Description("Below 2")]
        One = 1,

        /// <summary>
        /// 2星
        /// </summary>
        [Description("2")]
        Two = 2,

        /// <summary>
        /// 3星
        /// </summary>
        [Description("3")]
        Three = 3,

        /// <summary>
        /// 4星
        /// </summary>
        [Description("4")]
        Four = 4,

        [Description("N/A")]
        NA = 10,

        [Description("Auto Assign")]
        AutoAssign = 20
    }
}
