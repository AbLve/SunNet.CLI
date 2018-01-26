using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sunnet.Cli.Core.Users.Enums
{
    public enum Ethnicity : byte
    {
        [Description("African American")]
        African_American = 1,
        [Description("Alaskan")]
        Alaskan = 2,
        [Description("Native American")]
        Native_American = 3,
        [Description("Indian")]
        Indian = 4,
        [Description("Asian")]
        Asian = 5,
        [Description("White")]
        White = 6,// Caucasian = 6,
        [Description("Hispanic")]
        Hispanic = 7,
        [Description("Multiracial")]
        Multiracial = 8,
        [Description("Other")]
        Other = 9
    }
}
