using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sunnet.Cli.Core.Export.Enums
{
    public enum ReceiveFileBy : byte
    {
        /// <summary>
        /// 邮件附件
        /// </summary>
        //Attachments = 1,

        /// <summary>
        /// 邮件下载链接
        /// </summary>
        DownloadLink = 2,

        /// <summary>
        /// 上传到ftp
        /// </summary>
        SFTP = 3
    }
}
