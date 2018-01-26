using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sunnet.Cli.Core.Users.Enums
{
    public enum EducationLevel : byte
    {
        [Description("High School or  High School Equivalent")]
        High_School_or_High_School_Equivalent = 1,
        [Description("Some College")]
        Some_College = 3,
        [Description("Associate's Degree")]
        Associate_Degree = 4,
        [Description("Bachelor's Degree")]
        Bachelor_Degree = 5,
        [Description("Some Graduate Work")]
        Some_Graduate_Work = 6,
        [Description("Master's Degree")]
        Master_Degree = 7,
        [Description("Doctoral Degree")]
        Doctoral_Degree = 8
    }
}
