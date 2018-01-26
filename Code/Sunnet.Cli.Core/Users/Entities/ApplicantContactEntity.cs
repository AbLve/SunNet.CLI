using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/**************************************************************************
 * Developer: 		Damon
 * Computer:		Damon-PC
 * Domain:			Damon-pc
 * CreatedOn:		2014/8/13 11:59:23
 * Description:		Please input class summary
 * Version History:	Created,2014/8/13 11:44:23
 * 
 * 
 **************************************************************************/
using Sunnet.Cli.Core.Schools.Entities;
using Sunnet.Framework.Core.Base;
using Sunnet.Cli.Core.Users.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using Sunnet.Framework.Mvc;

namespace Sunnet.Cli.Core.Users.Entities
{
    public class ApplicantContactEntity : EntityBase<int>
    {
        public ApplicantContactEntity()
        {
            CommunityId = null;
            CommunityName = "";
            SchoolTypeId = 0;
            SchoolName = "";
            Address = "";
            Address2 = "";
            City = "";
            StateId = 0;
            Zip = "";
            FirstName = "";
            LastName = "";
            WorkPhone = "";
            OtherPhone = "";
            Email = "";
            RoleType = 0;
        }

        public int ApplicantId { get; set; }

        [Required]
        [DisplayName("Community/District")]
        public int? CommunityId { get; set; }

        [Required(AllowEmptyStrings = true)]
        [DisplayName("Community/District")]
        public string CommunityName { get; set; }

        [DisplayName("School Type")]
        public int SchoolTypeId { get; set; }

        [Required(AllowEmptyStrings = true)]
        [DisplayName("School")]
        [StringLength(200)]
        public string SchoolName { get; set; }

        [Required(AllowEmptyStrings = true)]
        [StringLength(200)]
        public string Address { get; set; }

        [EensureEmptyIfNullAttribute]
        [StringLength(200)]
        public string Address2 { get; set; }

        [Required(AllowEmptyStrings = true)]
        [StringLength(100)]
        public string City { get; set; }

        [Required(AllowEmptyStrings = true)]
        public int StateId { get; set; }

        [Required(AllowEmptyStrings = true)]
        [StringLength(5)]
        public string Zip { get; set; }

        [Required(AllowEmptyStrings = true)]
        [StringLength(140)]
        [DisplayName("First Name")]
        public string FirstName { get; set; }

        [Required(AllowEmptyStrings = true)]
        [StringLength(140)]
        [DisplayName("Last Name")]
        public string LastName { get; set; }

        [Required(AllowEmptyStrings = true)]
        [StringLength(50)]
        [DisplayName("Work Phone")]
        public string WorkPhone { get; set; }

        [EensureEmptyIfNullAttribute]
        [DisplayName("Other Phone")]
        [StringLength(50)]
        public string OtherPhone { get; set; }

        [Required(AllowEmptyStrings = true)]
        [StringLength(50)]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        public DistrictInviteRole RoleType { get; set; }

        public virtual ApplicantEntity Applicant { get; set; }
    }
}
