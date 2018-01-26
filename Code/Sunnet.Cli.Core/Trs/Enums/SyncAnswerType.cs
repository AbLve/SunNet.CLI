using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sunnet.Cli.Core.Trs.Enums
{
    public enum SyncAnswerType : byte
    {
        /// <summary>
        /// 无
        /// </summary>
        None = 1,

        /// <summary>
        /// 同一学校的相同Playground的Classroom需要共享答案, 没有Playground的Class不需要显示
        /// </summary>
        SamePlayground = 10,

        /// <summary>
        /// 同一学校所有的Classroom需要共享答案
        /// </summary>
        BetweenClass = 20
    }
}
