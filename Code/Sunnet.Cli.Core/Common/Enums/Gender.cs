using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sunnet.Cli.Core.Common.Enums
{
    public enum Gender : byte
    {
        [Description("Male")]
        Male = 1,
        [Description("Female")]
        Female = 2
    }
}
