using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sunnet.Cli.Core.Trs
{
    public enum EventLogType : byte
    {
        [Description("Star Level Change")]
        Star_Level_Change = 1,

        [Description("Auto Assign")]
        Auto_Assign = 2
    }
}
