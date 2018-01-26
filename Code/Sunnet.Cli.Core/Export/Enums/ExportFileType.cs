using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sunnet.Cli.Core.Export.Enums
{
    public enum ExportFileType : byte
    {
        /// <summary>
        /// 逗号分隔的csv
        /// </summary>
        Comma = 1,

        /// <summary>
        /// 管道分隔的csv
        /// </summary>
        Pipe = 2,

        /// <summary>
        /// Tab符分隔的csv
        /// </summary>
        Tab = 3
    }
}
