using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sunnet.Cli.Core.Common.Enums
{
    public enum AgeGroup : byte
    {
        [Description("Infant (6 wks - 17 mos)")]
        Infant = 10,

        [Description("Toddler (18 mos - 35 mos)")]
        Toddler = 20,

        [Description("Pre-School (3 yrs - 5 yrs)")]
        Pre_School = 30,

        [Description("School Age (6 yrs -12 yrs)")]
        SchoolAge = 31,

        [Description("Other")]
        Other = 40
    }
}
