using System;

using Sunnet.Cli.Core.Reports;
using Sunnet.Framework.Core.Base;
using Sunnet.Cli.Core.Export.Enums;

namespace Sunnet.Cli.Core.Export.Entities
{
    public class ExportInfoEntity : EntityBase<int>
    {
        public ExportInfoStatus Status { get; set; }

        public string ExecuteSQL { get; set; }

        public string CreaterMail { get; set; }

        public string CreaterFirstName { get; set; }

        public string CreaterLastName { get; set; }

        public string FileName { get; set; }

        public ExportFileType FileType { get; set; }

        public string FileUrl { get; set; }

        public string DownloadUrl { get; set; }

        public int CreatedBy { get; set; }

        public string GroupName { get; set; }

        public DateTime ExcuteTime { get; set; }

        public ReceiveFileBy ReceiveFileBy { get; set; }

        public string FtpHostIp { get; set; }

        public int FtpPort { get; set; }

        public string FtpUserName { get; set; }

        public string FtpPassword { get; set; }

        public bool FtpEnableSsl { get; set; }

        public string FtpFilePath { get; set; }
    }
}
