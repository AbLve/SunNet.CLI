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
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using Sunnet.Cli.Core.Vcw.Enums;
using Sunnet.Cli.Core.Vcw.Entities;
using Sunnet.Framework.Extensions;
using Newtonsoft.Json;
using Sunnet.Framework.Mvc;
using Sunnet.Framework;

namespace Sunnet.Cli.Business.Vcw.Models
{
    public class AssignmentListModel
    {
        public int ID { get; set; }

        public int SendUserId { get; set; }

        public string SendUserName { get; set; }

        public string CommunityName { get; set; }

        [DisplayName("School Name")]
        public string SchoolName { get; set; }

        [DisplayName("Teacher Name")]
        public string TeacherName { get; set; }

        public int TeacherId { get; set; }

        [DisplayName("Coach Name")]
        public string CoachName { get; set; }

        public int CoachId { get; set; }

        [DisplayName("Due Date")]
        public DateTime DueDate { get; set; }

        [DisplayName("Call Date")]
        public DateTime CallDate { get; set; }

        public int SessionId { get; set; }

        public string SessionText { get; set; }

        public int WaveId { get; set; }

        public string WaveText { get; set; }

        public DateTime FeedbackCalllDate { get; set; }

        [StringLength(100)]
        [EensureEmptyIfNullAttribute]
        public string StrategyOther { get; set; }

        public string ContentOther { get; set; }

        public string ContextOther { get; set; }

        public AssignmentStatus Status { get; set; }

        public IEnumerable<int> ContextIds { get; set; }

        public IEnumerable<SelectItemModel> Contexts { get; set; }

        public IEnumerable<int> ContentIds { get; set; }

        public IEnumerable<SelectItemModel> Contents { get; set; }

        public IEnumerable<int> UploadTypeIds { get; set; }

        public IEnumerable<SelectItemModel> UploadTypes { get; set; }

        public IEnumerable<int> StrategyIds { get; set; }

        public IEnumerable<SelectItemModel> Strategies { get; set; }


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

        [DisplayName("Uplaod Type")]
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

        [DisplayName("Strategies")]
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
    }

    /// <summary>
    /// 用于LinkToAssignment功能
    /// </summary>
    public class LinkToAssignmentModel
    {
        public int ID { get; set; }

        public int ReceiveUserId { get; set; }
    }
}
