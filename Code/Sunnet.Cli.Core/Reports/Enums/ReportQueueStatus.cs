using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sunnet.Cli.Core.Reports
{
    public enum ReportQueueStatus : byte
    {
        /// <summary>
        /// 保存
        /// </summary>
        Saved = 1,

        /// <summary>
        /// 正在处理
        /// </summary>
        Processing = 10,

        /// <summary>
        /// 已经计算报表结果
        /// </summary>
        Processed = 20,

        /// <summary>
        /// 邮件已发送
        /// </summary>
        Sent = 25,

        /// <summary>
        /// 已下载, PDF文件已生成(可直接获取)
        /// </summary>
        Downloaded = 30,

        Deleted = 40,

        /// <summary>
        /// 文件传输到SFTP时出错
        /// </summary>
        SftpError = 50,
        /// <summary>
        /// 处理的时候出错
        /// </summary>
        ProcessError = 60
    }
}
