using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sunnet.Cli.Core.Users.Enums
{
    public enum TeacherType : byte
    {
        [Description("Lead Teacher")]
        Lead_Teacher = 1,
        [Description("Co-Teacher")]
        Co_Teacher = 2,
        [Description("Assistant Teacher")]
        Assistant_Teacher = 3,
        [Description("Other")]
        Other = 4
    }
}
