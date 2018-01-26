using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sunnet.Cli.Core.TRSClasses.Enums
{
    public enum TRSClassofType : byte
    {
        [Description("Non-mixed Age Classroom (NMAC)")]
        NMAC = 1,

        [Description("Mixed Age Classroom(MAC)")]
        MAC = 2
    }
}
