using Sunnet.Cli.Core.Vcw.Enums;
using Sunnet.Framework;
using Sunnet.Framework.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sunnet.Cli.Business.Vcw.Models
{
    public class TeacherVIPFileModel
    {
        public int ID { get; set; }

        /// <summary>
        /// 格式为ECT######
        /// </summary>
        [Display(Name = "ID")]
        public string Number
        {
            get
            {
                return ConvertNumber.ConverIDToNumber(ID);
            }
        }

        [Required]
        public int AssignmentID { get; set; }

        [Display(Name = "Date Video Recorded")]
        public DateTime? DateRecorded { get; set; }

        public int? LanguageId { get; set; }

        public string LanguageText { get; set; }

        /// <summary>
        /// Teacher Comments 与 Description 共用一个字段
        /// </summary>
        [MaxLength(500)]
        [EensureEmptyIfNullAttribute]
        public string Description { get; set; }

        /// <summary>
        /// 文件所有者
        /// </summary>
        [Required]
        public int OwnerId { get; set; }

        /// <summary>
        /// 上传者；如Coach 替 Teacher 上传文件。
        /// </summary>
        [Required]
        public int UploadUserId { get; set; }

        public UploadUserTypeEnum UploadUserType { get; set; }

        [Required]
        public DateTime UploadDate { get; set; }

        [Required]
        public FileTypeEnum VideoType { get; set; }

        [EensureEmptyIfNullAttribute]
        [StringLength(200)]
        public string FileName { get; set; }

        [EensureEmptyIfNullAttribute]
        [StringLength(500)]
        public string FilePath { get; set; }

        public string DownLoadFilePath
        {
            get
            {
                if (!string.IsNullOrEmpty(FilePath))
                {
                    return "/DownLoadFile/DownLoadFile?filepath=" +
                        System.Web.HttpContext.Current.Server.UrlEncode(FilePath)
                        + "&filename=" + System.Web.HttpContext.Current.Server.UrlEncode(FileName) + "";
                }
                else
                {
                    return "";
                }
            }
        }

        [Required]
        public FileStatus Status { get; set; }

        [Display(Name = "TBRS Date")]
        public DateTime? TBRSDate { get; set; }

        [Required]
        [Display(Name = "File Name")]
        [MaxLength(50)]
        [EensureEmptyIfNullAttribute]
        public string IdentifyFileName { get; set; }

        public string FileExtension
        {
            get
            {
                if (string.IsNullOrEmpty(FileName)) return string.Empty;
                else
                {
                    string file_name = string.Empty;
                    // 此类型文件名称格式为 test.mp4(17.70M)
                    if (FileName.LastIndexOf(')') > 0 && FileName.LastIndexOf(')') > FileName.LastIndexOf('.'))
                        file_name = FileName.Substring(0, FileName.LastIndexOf('('));
                    else
                        file_name = FileName;

                    return file_name.Substring(file_name.LastIndexOf('.') + 1);
                }
            }
            set { }
        }

        public IEnumerable<int> SelectionIds { get; set; }
    }
}
