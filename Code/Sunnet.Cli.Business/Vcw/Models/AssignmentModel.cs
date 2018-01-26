using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/**************************************************************************
 * Developer: 		Joe
 * Computer:		JOE-PC
 * Domain:			Joe-pc
 * CreatedOn:		2014/10/22 12:15:10
 * Description:		Please input class summary
 * Version History:	Created,2014/10/22 12:15:10
 * 
 * 
 **************************************************************************/
using System.ComponentModel;
using Sunnet.Cli.Core.Vcw.Enums;
using Sunnet.Cli.Core.Vcw.Entities;
using System.ComponentModel.DataAnnotations;
using Sunnet.Framework.Mvc;
using Sunnet.Framework.Extensions;
using Sunnet.Framework;

namespace Sunnet.Cli.Business.Vcw.Models
{
    public class AssignmentModel
    {
        public int ID { get; set; }

        public int ReceiveUserId { get; set; }

        [DisplayName("Assignment Type")]
        public AssignmentTypeEnum Type { get; set; }

        [Required]
        [Display(Name = "Assignment Due Date")]
        public DateTime DueDate { get; set; }

        [Display(Name = "Feedback Call Date")]
        public DateTime? CallDate { get; set; }

        [Display(Name = "Wave")]
        public int WaveId { get; set; }

        public string WaveText { get; set; }

        [Display(Name = "Session")]
        public int SessionId { get; set; }

        public string SessionText { get; set; }

        public AssignmentStatus Status { get; set; }

        [MaxLength(500)]
        [EensureEmptyIfNullAttribute]
        public string Description { get; set; }


        [MaxLength(500)]
        [EensureEmptyIfNullAttribute]
        [Display(Name = "Coach Feedback")]
        public string FeedbackText { get; set; }

        /// <summary>
        /// 用户上传文件时，在本地的文件名，显示数据时，要用此名
        /// </summary>
        [EensureEmptyIfNullAttribute]
        public string FeedbackFileName { get; set; }

        /// <summary>
        /// 用户上传的文件存在服务器上的路径与名称
        /// </summary>
        [EensureEmptyIfNullAttribute]
        public string FeedbackFilePath { get; set; }

        [EensureEmptyIfNullAttribute]
        public string StrategyOther { get; set; }

        [MaxLength(100)]
        [EensureEmptyIfNullAttribute]
        public string ContextOther { get; set; }

        [MaxLength(100)]
        [EensureEmptyIfNullAttribute]
        public string ContentOther { get; set; }

        public IEnumerable<int> ContextIds { get; set; }

        public IEnumerable<SelectItemModel> Contexts { get; set; }

        public IEnumerable<int> ContentIds { get; set; }

        public IEnumerable<SelectItemModel> Contents { get; set; }

        public IEnumerable<int> UploadTypeIds { get; set; }

        public IEnumerable<SelectItemModel> UploadTypes { get; set; }

        public IEnumerable<int> StrategyIds { get; set; }

        public IEnumerable<SelectItemModel> Strategies { get; set; }

        public ICollection<AssignmentFileEntity> AssignmentFiles { get; set; }

        public IEnumerable<AssignmentReportEntity> Reports { get; set; }

        public string Content
        {
            get
            {
                string _content = "";
                if (Contents != null)
                {
                    foreach (SelectItemModel item in Contents)
                    {
                        if (item.Name.Trim().ToLower() == SFConfig.AssignmentContentOther)
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
        /// File Type
        /// </summary>
        public string UploadType
        {
            get
            {
                string _uploadtype = "";
                if (UploadTypes != null)
                {
                    foreach (SelectItemModel item in UploadTypes)
                    {
                        _uploadtype += item.Name + " / ";
                    }
                    if (_uploadtype.EndsWith(" / "))
                    {
                        _uploadtype = _uploadtype.Remove(_uploadtype.Length - 3, 3);
                    }
                }

                return _uploadtype;
            }
        }

        public string Context
        {
            get
            {
                string _context = "";
                if (Contexts != null)
                {
                    foreach (SelectItemModel item in Contexts)
                    {
                        if (item.Name.Trim().ToLower() == SFConfig.ContextOther)
                        {
                            if (!string.IsNullOrEmpty(ContextOther) && !string.IsNullOrEmpty(ContextOther.Trim()))
                                _context += ContextOther.Trim() + " / ";
                        }
                        else
                            _context += item.Name + " / ";
                    }
                    if (_context.EndsWith(" / "))
                    {
                        _context = _context.Remove(_context.Length - 3, 3);
                    }
                }

                return _context;
            }
        }


        public string Strategy
        {
            get
            {
                string _strategy = "";
                if (Strategies != null)
                {
                    foreach (SelectItemModel strategy in Strategies)
                    {
                        _strategy += strategy.Name + " / ";
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
        [MaxLength(700)]
        public string Watch { get; set; }


        public string WatchHyperLinked
        {
            get
            {
                if (!string.IsNullOrEmpty(Watch))
                {
                    string watch_hyper = "";
                    string[] str_watch = Watch.Split('\n');
                    foreach (string item in str_watch)
                    {
                        if (item.StartsWith("http://") || item.StartsWith("https://")
                            || item.StartsWith("HTTP://") || item.StartsWith("HTTPS://"))
                        {
                            watch_hyper += "<a href='" + item + "' class='form-link4' target='_blank'>" + item + "</a><br />";
                        }
                        else if (item.StartsWith("www."))
                        {
                            watch_hyper += "<a href='http://" + item + "' class='form-link4' target='_blank'>" + item + "</a><br />";
                        }
                        else
                        {
                            watch_hyper += item + "<br />";
                        }
                    }
                    return watch_hyper;
                }
                else
                {
                    return "";
                }
            }
        }

        public bool IsVisited { get; set; }
    }
}
