using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sunnet.Cli.Core.TRSClasses.Enums
{
    public enum ClassCountType:byte
    {
        [Description("Infant (6 wks - 17 mos)")]
        Infant=1,
        [Description("Toddler (18 mos - 35 mos)")]
        Toddler,
        [Description("Pre-School (3 yrs - 5 yrs)")]
        PreSchool,
        Other
    }
}
