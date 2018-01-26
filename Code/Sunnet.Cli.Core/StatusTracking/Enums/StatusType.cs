using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sunnet.Cli.Core.StatusTracking.Enums
{

    public enum StatusType : byte
    {
        [Description("Invitation")]
        Invitation = 1,

        [Description("New School")]
        AddSchool = 2
    }
}
