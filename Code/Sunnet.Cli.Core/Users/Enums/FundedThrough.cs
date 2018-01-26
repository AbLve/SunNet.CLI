using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sunnet.Cli.Core.Users.Enums
{
    public enum FundedThrough : byte
    {
        [Description("ESC Contract")]
        ESC_Contract = 1,
        [Description("School District Contract")]
        School_District_Contract = 2,
        [Description("School District Employee")]
        School_District_Employee = 3,
        [Description("UTH Employee")]
        UTH_Employee = 4,
        [Description("Other")]
        Other = 5
    }
}
