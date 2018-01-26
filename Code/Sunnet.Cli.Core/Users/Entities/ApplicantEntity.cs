using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/**************************************************************************
 * Developer: 		Sam
 * Computer:		SAM-PC
 * Domain:			sam-pc
 * CreatedOn:		2014/8/13 11:44:23
 * Description:		Please input class summary
 * Version History:	Created,2014/8/13 11:44:23
 * 
 * 
 **************************************************************************/
using Sunnet.Framework.Core.Base;
using Sunnet.Cli.Core.Users.Enums;
using System.ComponentModel;

namespace Sunnet.Cli.Core.Users.Entities
{
    public class ApplicantEntity : EntityBase<int>
    {
        public ApplicantEntity()
        {
            ApplicantEmails = new List<ApplicantEmailEntity>();
            ApplicantContacts = new List<ApplicantContactEntity>();
            Title = UserTitle.Dr;
            FirstName = "";
            LastName = "";
            Email = "";
            SchoolId = 0;
            RoleType = Role.Teacher;
            WorkPhone = "";
            OtherPhone = "";
            CommunityId = 0;
            Address = "";
            Address2 = "";
            City = "";
            StateId = 0;
            Zip = "";
            PositionId = 0;
            PositionOther = "";
            Status = ApplicantStatus.Pending;
            InvitedOn = DateTime.Parse("01/01/1753");
            VerifiedOn = DateTime.Parse("01/01/1753");
            //InviteeId = 0;
            SponsorId = 0;
            IsDeleted = false;
        }

        public UserTitle Title { get; set; }

        [DisplayName("First Name")]
        public string FirstName { get; set; }

        [DisplayName("Last Name")]
        public string LastName { get; set; }

        [RegularExpression(@"^[A-Za-z0-9](([_\.\-]?[a-zA-Z0-9]+)*)@([A-Za-z0-9]+)(([\.\-‌​]?[a-zA-Z0-9]+)*)\.([A-Za-z]{2,})$"
            , ErrorMessage = "Your Email Address is incorrectly formatted. Please enter a valid address .")]
        [StringLength(50)]
        public string Email { get; set; }

        public int SchoolId { get; set; }

        [DisplayName("User Type")]
        public Role RoleType { get; set; }

        public string WorkPhone { get; set; }

        public string OtherPhone { get; set; }

        public int CommunityId { get; set; }

        public string Address { get; set; }

        public string Address2 { get; set; }

        public string City { get; set; }

        public int StateId { get; set; }

        public string Zip { get; set; }

        public int PositionId { get; set; }

        public string PositionOther { get; set; }

        public ApplicantStatus Status { get; set; }

        public bool IsDeleted { get; set; }

        public DateTime InvitedOn { get; set; }

        public DateTime VerifiedOn { get; set; }

        public int InviteeId { get; set; }

        public int SponsorId { get; set; }

        public virtual ICollection<ApplicantEmailEntity> ApplicantEmails { get; set; }

        public virtual ICollection<ApplicantContactEntity> ApplicantContacts { get; set; }
    }
}
