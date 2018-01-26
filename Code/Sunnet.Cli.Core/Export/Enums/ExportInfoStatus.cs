using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sunnet.Cli.Core.Export.Enums
{
    public enum ExportInfoStatus : byte
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
        /// 邮件已发送||已上传到ftp
        /// </summary>
        Sent = 4,

        /// <summary>
        /// 出错
        /// </summary>
        Error = 5,

        /// <summary>
        /// 文件传输到sftp时出错
        /// </summary>
        SftpError=50
    }
}
