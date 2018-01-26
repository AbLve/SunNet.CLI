using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sunnet.Cli.Core.Trs.Enums
{
    public enum ItemAgeGroup : byte
    {
        [Description("0-11 months")]
        m_0_11 = 1,

        [Description("12-17 months")]
        m_12_17 = 2,

        [Description("18-23 months")]
        m_18_23 = 3,

        [Description("2 years")]
        y_2 = 4,

        [Description("3 years")]
        y_3 = 5,

        [Description("4 years")]
        y_4 = 6,

        [Description("5 years")]
        y_5 = 7,

        [Description("6-8 years")]
        y_6_8 = 8,

        [Description("9-13 years")]
        y_9_13 = 9
    }
}
