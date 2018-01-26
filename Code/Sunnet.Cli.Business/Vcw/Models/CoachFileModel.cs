using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/**************************************************************************
 * Developer: 		Joe
 * Computer:		Joe-PC
 * Domain:			Joe-pc
 * CreatedOn:		2014/11/8
 * Description:		Create AuthorityEntity
 * Version History:	Created,2014/11/8
 * 
 * 
 **************************************************************************/
using System.ComponentModel.DataAnnotations;
using Sunnet.Cli.Core.Vcw.Entities;
using Sunnet.Cli.Core.Vcw.Enums;
using Sunnet.Framework.Mvc;
using Sunnet.Framework.Extensions;
using Sunnet.Framework;

namespace Sunnet.Cli.Business.Vcw.Models
{
    public class CoachFileModel
    {
        public int ID { get; set; }

        public int AssignmentId { get; set; }

        [Display(Name = "File Name")]
        [MaxLength(50)]
        [Required]
        public string IdentifyFileName { get; set; }

        public DateTime UploadDate { get; set; }

        public DateTime? DateRecorded { get; set; }

        public int? ContextId { get; set; }

        [EensureEmptyIfNullAttribute]
        [MaxLength(100)]
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

        [EensureEmptyIfNullAttribute]
        [MaxLength(100)]
        public string ContentOther { get; set; }

        [EensureEmptyIfNullAttribute]
        [MaxLength(100)]
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

        [EensureEmptyIfNullAttribute]
        [MaxLength(500)]
        public string Objectives { get; set; }

        [EensureEmptyIfNullAttribute]
        [MaxLength(500)]
        public string Effectiveness { get; set; }

        [EensureEmptyIfNullAttribute]
        [MaxLength(200)]
        public string FileName { get; set; }

        [EensureEmptyIfNullAttribute]
        [MaxLength(500)]
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

        public string FeedbackText { get; set; }

        public string FeedbackFileName { get; set; }

        public string FeedbackFilePath { get; set; }

        public int UploadUserId { get; set; }

        [Required]
        public int OwnerId { get; set; }

        public int? LanguageId { get; set; }

        public string LanguageText { get; set; }

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

        public virtual ICollection<FileSharedEntity> FileShareds { get; set; }

        public FileStatus Status { get; set; }

        public IEnumerable<int> SelectionIds { get; set; }

        public IEnumerable<SelectItemModel> Contents { get; set; }

        public IEnumerable<int> ContentIds { get; set; }

        public IEnumerable<SelectItemModel> Strategies { get; set; }

        public IEnumerable<int> StrategyIds { get; set; }
    }
}
