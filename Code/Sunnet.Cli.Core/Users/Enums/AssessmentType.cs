using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sunnet.Cli.Core.Users.Enums
{
    public enum AssignmentType : byte
    {
        [Description("Face-to-Face")]
        Face_to_Face = 1,
        [Description("Remote")]
        Remote = 2,
        [Description("Other")]
        Other = 3
    }
}
