using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Sunnet.Cli.Core.Vcw.Enums;
using Sunnet.Framework.Extensions;
using Newtonsoft.Json;
using Sunnet.Framework;
using System.IO;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web.UI;
using System.Web;

namespace Sunnet.Cli.Business.Vcw.Models
{
    /// <summary>
    /// Vcw_File List Model 专给 List 使用
    /// </summary>
    public class FileListModel
    {
        public int ID { get; set; }

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

        [Description("File Name")]
        [MaxLength(50)]
        [Required]
        public string IdentifyFileName { get; set; }

        public FileTypeEnum VideoType { get; set; }

        public DateTime UploadDate { get; set; }

        public DateTime DateRecorded { get; set; }


        /// <summary>
        /// Due Date
        /// </summary>
        public DateTime UpdatedOn { get; set; }

        public int? ContextId { get; set; }

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

        public string ContextOther { get; set; }

        public string ContentOther { get; set; }

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

        /// <summary>
        /// 不用在列表显示的字段
        /// </summary>
        [JsonIgnore]
        public string FileName { get; set; }

        public string FileExtension
        {
            get
            {
                if (string.IsNullOrEmpty(FileName)) return string.Empty;
                else
                {
                    string file_name = string.Empty;
                    // 此类型文件名称格式为 test.mp4(17.70M)
                    if (FileName.LastIndexOf(')') > 0 && FileName.LastIndexOf(')') > FileName.LastIndexOf('.') && FileName.LastIndexOf('(') > 0)
                        file_name = FileName.Substring(0, FileName.LastIndexOf('('));
                    else
                        file_name = FileName;

                    return file_name.Substring(file_name.LastIndexOf('.') + 1);
                }
            }
            set { }
        }

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


        public FileStatus Status { get; set; }

        public int? LanguageId { get; set; }

        public string LanguageText { get; set; }

        public string CommunityName { get; set; }

        [DisplayName("School Name")]
        public string SchoolName { get; set; }

        [DisplayName("Teacher Name")]
        public string TeacherName { get; set; }

        public int TeacherId { get; set; }

        [DisplayName("Coach Name")]
        public string CoachName { get; set; }

        public int CoachId { get; set; }

        public string StrategyOther { get; set; }

        public string Strategy
        {
            get
            {
                string _strategy = "";
                if (Strategies != null)
                {
                    foreach (SelectItemModel item in Strategies)
                    {
                        if (item.Name.Trim().ToLower() == SFConfig.StrategyOther)
                        {
                            if (!string.IsNullOrEmpty(StrategyOther) && !string.IsNullOrEmpty(StrategyOther.Trim()))
                                _strategy += StrategyOther.Trim() + " / ";
                        }
                        else
                            _strategy += item.Name + " / ";
                    }
                    if (_strategy.EndsWith(" / "))
                    {
                        _strategy = _strategy.Remove(_strategy.Length - 3, 3);
                    }
                }

                return _strategy;
            }
        }

        public string FeedbackText { get; set; }

        public string FeedbackFileName { get; set; }

        public string AssignmentFeedbackText { get; set; }

        public string AssignmentFeedbackFileName { get; set; }

        //是否有Feedback
        public bool HasFeedback
        {
            get
            {
                if (!string.IsNullOrEmpty(FeedbackText) || !string.IsNullOrEmpty(FeedbackFileName)
                    || !string.IsNullOrEmpty(AssignmentFeedbackText) || !string.IsNullOrEmpty(AssignmentFeedbackFileName))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public DateTime AssignmentDueDate { get; set; }

        public int AssignmentId { get; set; }

        public IEnumerable<SelectItemModel> Contents { get; set; }

        public IEnumerable<int> ContentIds { get; set; }

        public IEnumerable<SelectItemModel> Strategies { get; set; }

        public IEnumerable<int> StrategyIds { get; set; }
    }
}
