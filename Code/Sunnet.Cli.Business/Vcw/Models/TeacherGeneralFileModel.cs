using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Sunnet.Cli.Core.Vcw.Enums;
using System.ComponentModel.DataAnnotations;
using Sunnet.Framework.Mvc;
using Sunnet.Framework.Extensions;
using System.ComponentModel;
using Sunnet.Framework;

namespace Sunnet.Cli.Business.Vcw.Models
{
    /// <summary>
    /// 添加，编辑 Upload General File
    /// </summary>
    public class TeacherGeneralFileModel
    {
        public int ID { get; set; }

        public int OwnerId { get; set; }

        /// <summary>
        /// 格式为ECT######
        /// </summary>
        public string Number
        {
            get
            {
                return ConvertNumber.ConverIDToNumber(ID);
            }
        }

        [Display(Name = "File Name")]
        [MaxLength(50)]
        [Required]
        public string IdentifyFileName { get; set; }

        public DateTime? DateVieoRecorded { get; set; }

        public int? ContextId { get; set; }

        [MaxLength(100)]
        [EensureEmptyIfNullAttribute]
        public string ContextOther { get; set; }

        public string ContextText { get; set; }

        public string Context
        {
            get
            {
                if (ContextText != null)
                {
                    if (ContextText.Trim().ToLower() == SFConfig.ContextOther)
                    {
                        return ContextOther;
                    }
                    else
                    {
                        return ContextText;
                    }
                }
                else
                {
                    return "";
                }
            }
        }


        [MaxLength(100)]
        [EensureEmptyIfNullAttribute]
        public string ContentOther { get; set; }

        [EensureEmptyIfNullAttribute]
        [MaxLength(500)]
        public string Description { get; set; }

        [MaxLength(200)]
        [EensureEmptyIfNullAttribute]
        public string FileName { get; set; }

        [MaxLength(500)]
        [EensureEmptyIfNullAttribute]
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

        #region View时用到的属性

        public string Content
        {
            get
            {
                string _content = "";
                if (Contents != null)
                {
                    foreach (SelectItemModel item in Contents)
                    {
                        if (item.Name.Trim().ToLower() == SFConfig.VideoContentOther)
                        {
                            if (!string.IsNullOrEmpty(ContentOther) && !string.IsNullOrEmpty(ContentOther.Trim()))
                                _content += ContentOther.Trim() + " / ";
                        }
                        else
                            _content += item.Name + " / ";
                    }
                    if (_content.EndsWith(" / "))
                    {
                        _content = _content.Remove(_content.Length - 3, 3);
                    }
                }

                return _content;
            }
        }

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

        [MaxLength(500)]
        [EensureEmptyIfNullAttribute]
        public string FeedbackText { get; set; }

        [MaxLength(200)]
        [EensureEmptyIfNullAttribute]
        public string FeedbackFileName { get; set; }

        [MaxLength(500)]
        [EensureEmptyIfNullAttribute]
        public string FeedbackFilePath { get; set; }

        public DateTime UploadDate { get; set; }

        public int UploadUserId { get; set; }

        public UploadUserTypeEnum UploadUserType { get; set; }

        public IEnumerable<SelectItemModel> Contents { get; set; }

        public IEnumerable<int> ContentIds { get; set; }

        #endregion
    }
}
