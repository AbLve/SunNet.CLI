using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sunnet.Cli.Core.Export.Enums;
using Sunnet.Framework.Core.Base;

namespace Sunnet.Cli.Core.Reports.Entities
{
    public class ReportQueueEntity : EntityBase<int>
    {
        public ReportQueueEntity()
        {
            QueryParams = string.Empty;
            Result = string.Empty;
            MergeLabelResult = string.Empty;
            DownloadUrl = string.Empty;
            Report = string.Empty;
            Title = string.Empty;
            ExcuteTime = DateTime.Parse("1753-1-1");
            ReceiveFileBy = ReceiveFileBy.DownloadLink;
            SFTPHostIp = string.Empty;
            SFTPFilePath = string.Empty;
            SFTPUserName = string.Empty;
            SFTPPassword = string.Empty;
            FileType = ExportFileType.Comma;
        }

        public int CreatedBy { get; set; }

        public ReportQueueStatus Status { get; set; }

        public ReportQueueType Type { get; set; }

        /// <summary>
        /// 报表参数
        /// </summary>
        public string QueryParams { get; set; }

        /// <summary>
        /// 报表结果集.
        /// </summary>
        public string Result { get; set; }

        /// <summary>
        /// 报表结果集，用于保存Cpalls Benchmark Report中合并相同Label后的结果
        /// </summary>
        public string MergeLabelResult { get; set; }


        /// <summary>
        /// 报表下载链接,({ID} 作为占位符)
        /// </summary>
        public string DownloadUrl { get; set; }

        /// <summary>
        /// 已生成报表文件, 如果Status值为Downloaded, 则表示用户已经下载过报表, 会保存在服务器
        /// </summary>
        public string Report { get; set; }

        /// <summary>
        /// 报表标题, 发送邮件时使用
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 执行时间,Schedle中使用
        /// </summary>
        public DateTime ExcuteTime { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public ReceiveFileBy ReceiveFileBy { get; set; }

        public string SFTPHostIp { get; set; }

        public int SFTPPort { get; set; }

        public string SFTPUserName { get; set; }

        public string SFTPPassword { get; set; }

        public string SFTPFilePath { get; set; }

        public ExportFileType FileType { get; set; }
    }
}
