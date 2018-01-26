using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sunnet.Cli.Core.Users.Enums
{
    public enum EquipmentEnum : byte
    {
        [Description("1) Netbook")]
        Netbook = 1,
        [Description("2) Tripod")]
        Tripod = 2,
        [Description("3) Camera")]
        Camera = 3,
        [Description("4) SD Card(s)")]
        SD_Card = 4,
        [Description("5) Envelope(s)")]
        Envelope = 5,
        [Description("6) Tablet(s)")]
        Tablet = 6
    }
}
