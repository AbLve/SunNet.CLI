using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sunnet.Cli.Core.Export.Enums;

namespace Sunnet.Cli.Business.Export.Models
{
    public class ExportInfoModel
    {
        [Required]
        [DisplayName("Community/District")]
        public int CommunityId { get; set; }

        [Required]
        public string FieldList { get; set; }

        [DisplayName("Start Date")]
        public DateTime? StartDate { get; set; }

        [DisplayName("Stop Date")]
        public DateTime? StopDate { get; set; }

        [DisplayName("Frequency")]
        [Range(1, 10)]
        public int? Frequency { get; set; }

        [Required]
        [DisplayName("Frequency Unit")]
        public FrequencyUnitType FrequencyUnit { get; set; }

        [Required]
        [DisplayName("File Separate By")]
        public ExportFileType FileType { get; set; }

        [Required]
        [DisplayName("Receive File By")]
        public ReceiveFileBy ReceiveFileBy { get; set; }

        [Required]
        [DisplayName("SFTP Host IP")]
        public string FtpHostIp { get; set; }

        [DisplayName("SFTP Port")]
        public int FtpPort { get; set; }

        [DisplayName("SFTP UserName")]
        public string FtpUserName { get; set; }

        [DisplayName("SFTP Password")]
        [DataType(DataType.Password)]
        public string FtpPassword { get; set; }

        [DisplayName("SFTP EnableSsl")]
        public bool FtpEnableSsl { get; set; }

        [DisplayName("SFTP File Path")]
        public string FtpFilePath { get; set; }
    }
}
