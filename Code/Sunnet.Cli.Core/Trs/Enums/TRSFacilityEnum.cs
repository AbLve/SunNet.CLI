using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sunnet.Cli.Core.Trs
{
    public enum TRSFacilityEnum : byte
    {
        [Description("N/A")]
        N_A = 1,

        [Description("Met/Not Met")]
        Met_NotMet = 2,

        [Description("0-3")]
        Zero_Three = 3

    }
}
