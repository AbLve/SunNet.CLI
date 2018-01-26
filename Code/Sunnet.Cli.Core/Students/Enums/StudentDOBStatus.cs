using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sunnet.Cli.Core.Students.Enums
{
    public enum StudentDOBStatus : byte
    {
        /// <summary>
        /// 保存
        /// </summary>
        Save = 1,

        /// <summary>
        /// 处理中
        /// </summary>
        Processing = 2,

        /// <summary>
        /// 处理过的
        /// </summary>
        Processed = 3,

        /// <summary>
        /// 出错
        /// </summary>
        Error = 4
    }
}
