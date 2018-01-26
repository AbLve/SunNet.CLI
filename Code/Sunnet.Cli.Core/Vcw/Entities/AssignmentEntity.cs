using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/**************************************************************************
 * Developer: 		Lee
 * Computer:		Lee-PC
 * Domain:			Lee-pc
 * CreatedOn:		2014/10/21
 * Description:		Create AuthorityEntity
 * Version History:	Created,2014/10/21
 * 
 * 
 **************************************************************************/
using Sunnet.Cli.Core.Vcw.Enums;
using Sunnet.Framework.Core.Base;
using System.ComponentModel.DataAnnotations;
using Sunnet.Cli.Core.Users.Enums;
using Sunnet.Framework.Mvc;
using System.ComponentModel;

namespace Sunnet.Cli.Core.Vcw.Entities
{
    public class AssignmentEntity : EntityBase<int>
    {
        /// <summary>
        /// 初始化可为空的字段
        /// </summary>
        public AssignmentEntity()
        {
            SessionId = 0;
            WaveId = 0;
        }



        [Required]
        public AssignmentStatus Status { get; set; }

        [Required]
        public AssignmentTypeEnum AssignmentType { get; set; }

        [Required]
        public int SendUserId { get; set; }

        [Required]
        public int ReceiveUserId { get; set; }

        [Required]
        [Display(Name = "Assignment Due Date")]
        [DataType(DataType.DateTime)]
        public DateTime DueDate { get; set; }

        [Required]
        [Display(Name = "Feedback Call Date")]
        [DataType(DataType.DateTime)]
        public DateTime FeedbackCalllDate { get; set; }

        public int? SessionId { get; set; }

        public int? WaveId { get; set; }

        [StringLength(100)]
        [EensureEmptyIfNullAttribute]
        public string ContextOther { get; set; }

        [StringLength(100)]
        [EensureEmptyIfNullAttribute]
        public string ContentOther { get; set; }

        [StringLength(500)]
        [EensureEmptyIfNullAttribute]
        public string FeedbackText { get; set; }

        /// <summary>
        /// 用户上传文件时，在本地的文件名，显示数据时，要用此名
        /// </summary>
        [StringLength(200)]
        [EensureEmptyIfNullAttribute]
        public string FeedbackFileName { get; set; }

        /// <summary>
        /// 用户上传的文件存在服务器上的路径与名称
        /// </summary>
        [StringLength(500)]
        [EensureEmptyIfNullAttribute]
        public string FeedbackFilePath { get; set; }

        [StringLength(500)]
        [EensureEmptyIfNullAttribute]
        public string Description { get; set; }

        [StringLength(100)]
        [EensureEmptyIfNullAttribute]
        public string StrategyOther { get; set; }

        [Required]
        public int CreatedBy { get; set; }

        [Required]
        public int UpdatedBy { get; set; }

        public bool IsDelete { get; set; }

        [EensureEmptyIfNullAttribute]
        public string Watch { get; set; }

        [Required]
        public bool IsVisited { get; set; }

        /// <summary>
        /// Coach assignment File
        /// </summary>
        public virtual ICollection<AssignmentFileEntity> AssignmentFiles { get; set; }

        public virtual ICollection<AssignmentContentEntity> AssignmentContents { get; set; }

        public virtual ICollection<AssignmentContextEntity> AssignmentContexts { get; set; }

        public virtual ICollection<AssignmentUploadTypeEntity> AssignmentUploadTypes { get; set; }

        public virtual ICollection<AssignmentStrategyEntity> AssignmentStrategies { get; set; }

        public virtual ICollection<AssignmentReportEntity> AssignmentReports { get; set; }

        public virtual ICollection<Vcw_FileEntity> Files { get; set; }

        public virtual WaveEntity Wave { get; set; }

        public virtual SessionEntity Session { get; set; }

        public virtual V_UserEntity UserInfo { get; set; }
    }   
}
