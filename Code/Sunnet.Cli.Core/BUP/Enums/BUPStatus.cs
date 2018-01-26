using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sunnet.Cli.Core.BUP
{
    public enum BUPStatus : byte
    {
        /// <summary>
        /// 等待处理
        /// </summary>
        Pending = 0,

        /// <summary>
        /// 处理队列中
        /// </summary>
        Queued = 1,

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
        [Description("Error-Dup")]
        Error = 4,

        /// <summary>
        /// 数据源不正确
        /// </summary>
        DataError  = 5
    }
}
