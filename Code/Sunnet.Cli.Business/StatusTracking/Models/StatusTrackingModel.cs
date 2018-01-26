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
using Sunnet.Cli.Core.StatusTracking.Entities;
using Sunnet.Cli.Core.StatusTracking.Enums;
using System.ComponentModel.DataAnnotations;
using Sunnet.Cli.Core.Common.Enums;
using Sunnet.Cli.Core.Users.Enums;
using Sunnet.Cli.Core.Users.Entities;

namespace Sunnet.Cli.Business.StatusTracking.Models
{
    public class StatusTrackingModel : StatusTrackingEntity
    {
        [Display(Name = "Operator")]
        public string ApproverName { get; set; }

        [Display(Name = "Requestor")]
        public string RequestorName { get; set; }

        [Display(Name = "Requestor Email")]
        public string RequestorEmail { get; set; }

        [Display(Name = "Status")]
        public string RealStatus
        {
            get
            {
                if (Status == StatusEnum.Pending && DateTime.Now > ExpiredTime)
                {
                    return "Expired";
                }
                else
                {
                    return Status.ToString();
                }
            }
        }

        public string Days
        {
            get
            {
                TimeSpan ts = DateTime.Now - RequestTime;
                return ts.Days.ToString();
            }
        }

        [Display(Name = "Requestor Community/ies")]
        public string RequestorCommunities { get; set; }

        [Display(Name = "Supposed Approver")]
        public string SApproverName
        {
            get
            {
                return SApproverFirstName + " " + SApproverLastName;
            }
        }

        [Display(Name = "First Name")]
        public string SApproverFirstName { get; set; }

        [Display(Name = "Last Name")]
        public string SApproverLastName { get; set; }

        [Display(Name = "Primary Phone Number")]
        public string SApproverPNumber { get; set; }

        [Display(Name = "Primary Phone Type")]
        public PhoneType SApproverPType { get; set; }

        [Display(Name = "Secondary Phone Number")]
        public string SApproverSNumber { get; set; }

        [Display(Name = "Secondary Number Type")]
        public PhoneType SApproverSType { get; set; }

        [Display(Name = "Fax Number")]
        public string SApproverFaxNumber { get; set; }

        [Display(Name = "Primary Email")]
        public string SApproverPEAddress { get; set; }

        [Display(Name = "Secondary Email")]
        public string SApproverSEAddress { get; set; }

        public string RequestTimeConvert
        {
            get
            {
                return DateTimeConvert(RequestTime);
            }
        }

        public string ResendTimeConvert
        {
            get
            {
                return DateTimeConvert(ResendTime);
            }
        }

        public string ExpiredTimeConvert
        {
            get
            {
                return DateTimeConvert(ExpiredTime);
            }
        }

        private string DateTimeConvert(DateTime dt)
        {
            if (dt > new DateTime(1900, 1, 1))
                return dt.ToString("MM/dd/yyyy HH:mm:ss");
            else
                return "";
        }

        public bool IfCanOperate { get; set; }

        public bool IfCanEmail { get; set; }

        public virtual ICollection<UserComSchRelationEntity> SupposedUserCommunitySchools { get; set; }

    }
}
