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
using Sunnet.Framework.Core.Base;
using System.ComponentModel.DataAnnotations;
using Sunnet.Cli.Core.Vcw.Enums;
using Sunnet.Framework.Mvc;

namespace Sunnet.Cli.Core.Vcw.Entities
{
    public class Vcw_FileEntity : EntityBase<int>
    {
        public Vcw_FileEntity()
        {
            AssignmentId = 0;
            LanguageId = 0;
            ContextId = 0;
        }

        public int? AssignmentId { get; set; }

        [Required]
        public FileStatus Status { get; set; }

        /// <summary>
        /// 文件所有者
        /// </summary>
        [Required]
        public int OwnerId { get; set; }

        /// <summary>
        /// 上传者；如Coach 替 Teacher 上传文件。 UserID
        /// </summary>
        [Required]
        public int UploadUserId { get; set; }

        [Required]
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

        [EensureEmptyIfNullAttribute]
        [StringLength(500)]
        public string FeedbackText { get; set; }

        [EensureEmptyIfNullAttribute]
        [StringLength(200)]
        public string FeedbackFileName { get; set; }

        [EensureEmptyIfNullAttribute]
        [StringLength(500)]
        public string FeedbackFilePath { get; set; }

        [Required]
        public DateTime DateRecorded { get; set; }

        public int? ContextId { get; set; }

        [EensureEmptyIfNullAttribute]
        [StringLength(100)]
        public string ContextOther { get; set; }

        [EensureEmptyIfNullAttribute]
        [StringLength(100)]
        public string ContentOther { get; set; }


        [EensureEmptyIfNullAttribute]
        [StringLength(100)]
        public string StrategyOther { get; set; }

        public int? LanguageId { get; set; }

        [EensureEmptyIfNullAttribute]
        [StringLength(500)]
        public string Objectives { get; set; }

        [EensureEmptyIfNullAttribute]
        [StringLength(500)]
        public string Effectiveness { get; set; }

        [Required]
        public DateTime TBRSDate { get; set; }

        /// <summary>
        /// Teacher Comments 与 Description 共用一个字段
        /// </summary>
        [EensureEmptyIfNullAttribute]
        [StringLength(500)]
        public string Description { get; set; }

        /// <summary>
        /// Users.ID
        /// </summary>
        [Required]
        public int CreatedBy { get; set; }

        /// <summary>
        /// Users.ID
        /// </summary>
        [Required]
        public int UpdatedBy { get; set; }

        public bool IsDelete { get; set; }

        public string IdentifyFileName { get; set; }

        public virtual ICollection<FileSelectionEntity> FileSelections { get; set; }

        public virtual ICollection<FileSharedEntity> FileShareds { get; set; }

        public virtual ICollection<FileContentEntity> FileContents { get; set; }

        public virtual ICollection<FileStrategyEntity> FileStrategies { get; set; }

        public virtual AssignmentEntity Assignment { get; set; }

        public virtual Video_Language_DataEntity Language { get; set; }

        public virtual Context_DataEntity Context { get; set; }

        /// <summary>
        /// 文件所有者
        /// </summary>
        public virtual V_UserEntity UserInfo { get; set; }
    }
}
