using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sunnet.Cli.Core.BUP
{
    public enum BUPEntityStatus : byte
    {
        /// <summary>
        /// 活动状态
        /// </summary>
        Activate = 1,

        /// <summary>
        /// 非活动状态
        /// </summary>
        Inactivate = 2,

        /// <summary>
        /// 主要目的是删除Entity下相关数据的关系 
        /// 如：删除Class与Student的关系，School与Class的关系等
        /// </summary>
        Delete = 3
    }
}
