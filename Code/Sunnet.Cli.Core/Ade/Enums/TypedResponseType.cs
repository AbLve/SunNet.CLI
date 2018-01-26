using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sunnet.Cli.Core.Ade
{
    public enum TypedResponseType : byte
    {
        Text = 1,
        Numerical = 2,
        Radionbox = 3,
        Checkbox = 4
    }
}
