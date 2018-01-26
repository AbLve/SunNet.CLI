using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sunnet.Cli.Core.Trs
{
    public enum TRSStatusEnum : byte
    {
        Initialized = 1,

        Saved = 20,

        Completed = 30
    }
}
