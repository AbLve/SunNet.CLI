using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sunnet.Cli.Core.Common.Enums
{
    /// <summary>
    /// 下拉框搜索类型
    /// </summary>
    public enum IdSearchType
    {
        /// <summary>
        /// 不启用该搜索条件
        /// </summary>
        Disabled = 0,
        /// <summary>
        /// 用户选择的的选项为N/A
        /// </summary>
        None = 1,
        /// <summary>
        /// 用户选择的的选项为All
        /// </summary>
        All = 2,
        /// <summary>
        /// 用户选择的选项为具体的某个选项
        /// </summary>
        Single = 3
    }
}
