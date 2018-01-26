using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/**************************************************************************
 * Developer: 		Joe
 * Computer:		Joe-PC
 * Domain:			Joe-pc
 * CreatedOn:		2015/06/03
 * Description:		Create AuthorityEntity
 * Version History:	Created,2015/06/03
 * 
 * 
 **************************************************************************/
using Sunnet.Framework.Core.Base;
using System.ComponentModel.DataAnnotations;
using Sunnet.Cli.Core.StatusTracking.Enums;
using Sunnet.Cli.Core.Users.Entities;
using Sunnet.Framework.Mvc;
using Sunnet.Cli.Core.Schools.Entities;

namespace Sunnet.Cli.Core.StatusTracking.Entities
{
    public class StatusTrackingEntity : EntityBase<int>
    {
        private readonly DateTime _minDateTime = DateTime.Parse("01/01/1753");

        public StatusTrackingEntity()
        {
            ApproverId = 0;
            RequestorId = 0;
            SupposedApproverId = 0;
            Status = StatusEnum.Pending;
            RequestTime = DateTime.Now;
            ResendNumber = 0;
            CommunityId = 0;
            SchoolId = 0;
            ApprovedTime = _minDateTime;
            DeniedTime = _minDateTime;
            ResendTime = _minDateTime;
            ProcessAddress = "";
            SupposedApproverIds = "";
        }


        public int? ApproverId { get; set; }

        [Required]
        public int RequestorId { get; set; }

        [Required]
        public int SupposedApproverId { get; set; }

        [Required]
        public StatusEnum Status { get; set; }

        [Required]
        [Display(Name = "Request Time")]
        public DateTime RequestTime { get; set; }

        [Required]
        [Display(Name = "Approved Time")]
        public DateTime ApprovedTime { get; set; }

        [Required]
        [Display(Name = "Denied Time")]
        public DateTime DeniedTime { get; set; }

        [Required]
        [Display(Name = "Expired Time")]
        public DateTime ExpiredTime { get; set; }

        [Required]
        [Display(Name = "Resend Time")]
        public DateTime ResendTime { get; set; }

        [Required]
        [Display(Name = "Resend Number")]
        public int ResendNumber { get; set; }

        [Required]
        public int CommunityId { get; set; }

        public int? SchoolId { get; set; }

        [Required]
        public int CreatedBy { get; set; }

        [Required]
        public int UpdatedBy { get; set; }

        [Required]
        public StatusType Type { get; set; }

        [EensureEmptyIfNull]
        [StringLength(1000)]
        public string ProcessAddress { get; set; }

        [EensureEmptyIfNull]
        [StringLength(500)]
        //用于New School时，存储多个收件人ID，格式如：1,2,3
        public string SupposedApproverIds { get; set; }

        public virtual UserBaseEntity UserInfo_Approver { get; set; }

        public virtual UserBaseEntity UserInfo_Requestor { get; set; }

        public virtual UserBaseEntity UserInfo_SupposedApprover { get; set; }

        public virtual SchoolEntity SchoolInfo { get; set; }
    }
}
