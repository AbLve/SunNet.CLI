using Sunnet.Cli.Core.Common.Enums;
using Sunnet.Cli.Core.Users.Enums;
using Sunnet.Framework.Core.Base;
using Sunnet.Framework.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sunnet.Cli.Core.Users.Entities
{
    public class CoordCoachEntity : EntityBase<int>
    {
        public CoordCoachEntity()
        {
            OfficeAddress1 = "";
            OfficeCity = "";
            OfficeCountyId = 0;
            OfficeStateId = 0;
            OfficeZip = "";
            HomeMailingAddress1 = "";
            HomeMailingCity = "";
            HomeMailingCountyId = 0;
            HomeMailingStateId = 0;
            HomeMailingZip = "";
        }
        [EensureEmptyIfNullAttribute]
        [StringLength(5)]
        [DisplayName("School Year")]
        public string SchoolYear { get; set; }

        [DisplayName("Birth Date")]
        public DateTime? BirthDate { get; set; }

        [Required]
        [DisplayName("Gender")]
        public Gender Gender { get; set; }

        [Required(AllowEmptyStrings = true)]
        [StringLength(200)]
        [DisplayName("Office Address")]
        public string OfficeAddress1 { get; set; }

        [EensureEmptyIfNullAttribute]
        [StringLength(200)]
        [DisplayName("Office Address2")]
        public string OfficeAddress2 { get; set; }

        [Required(AllowEmptyStrings = true)]
        [StringLength(200)]
        [DisplayName("Office City")]
        public string OfficeCity { get; set; }

        [Required(AllowEmptyStrings = true)]
        [DisplayName("Office County")]
        public int OfficeCountyId { get; set; }

        [Required(AllowEmptyStrings = true)]
        [DisplayName("Office State")]
        public int OfficeStateId { get; set; }

        [Required(AllowEmptyStrings = true)]
        [StringLength(5)]
        public string OfficeZip { get; set; }

        [Required]
        [DisplayName("Office Can Receive FedEx Deliveries?")]
        public bool OfficeIsReceiveFedEx { get; set; }

        [Required]
        [DisplayName("Office Can Receive Regular Mail Deliveries?")]
        public bool OfficeIsReceiveMail { get; set; }

        [Required(AllowEmptyStrings = true)]
        [StringLength(200)]
        [DisplayName("Home Mailing Address1")]
        public string HomeMailingAddress1 { get; set; }

        [EensureEmptyIfNullAttribute]
        [StringLength(200)]
        [DisplayName("Home Mailing Address2")]
        public string HomeMailingAddress2 { get; set; }

        [Required(AllowEmptyStrings = true)]
        [StringLength(200)]
        [DisplayName("Home Mailing City")]
        public string HomeMailingCity { get; set; }

        [Required(AllowEmptyStrings = true)]
        [DisplayName("Home Mail County")]
        public int HomeMailingCountyId { get; set; }

        [Required(AllowEmptyStrings = true)]
        [DisplayName("Home Mail State")]
        public int HomeMailingStateId { get; set; }

        [Required(AllowEmptyStrings = true)]
        [StringLength(5)]
        public string HomeMailingZip { get; set; }

        [Required]
        [DisplayName("Home Can Receive FedEx Deliveries?")]
        public bool HomeIsReceiveFedEx { get; set; }

        [Required]
        [DisplayName("Home Can Receive Regular Mail Deliveries?")]
        public bool HomeIsReceiveMail { get; set; }

        [DisplayName("Ethnicity")]
        public Ethnicity Ethnicity { get; set; }

        [EensureEmptyIfNullAttribute]
        [StringLength(100)]
        public string EthnicityOther { get; set; }

        [DisplayName("Primary Language")]
        public int PrimaryLanguageId { get; set; }

        [EensureEmptyIfNullAttribute]
        [StringLength(100)]
        public string PrimaryLanguageOther { get; set; }

        [DisplayName("Secondary Language")]
        public int SecondaryLanguageId { get; set; }

        [EensureEmptyIfNullAttribute]
        [StringLength(100)]
        public string SecondaryLanguageOther { get; set; }

        [DisplayName("Total Years Coaching")]
        public int TotalYearsCoaching { get; set; }

        [DisplayName("Highest Education Level Attained")]
        public EducationLevel EducationLevel { get; set; }

        [DisplayName("Expected FTF Mentoring Hours")]
        public decimal? F2FAvailableHours { get; set; }

        [DisplayName("Expected Remote Coaching Cycles")]
        public decimal? RemAvailableCycle { get; set; }
        
        [DisplayName("Coaching Type")]
        public AssignmentType CoachingType { get; set; }

        [EensureEmptyIfNullAttribute]
        [StringLength(100)]
        public string CoachingTypeOther { get; set; }

        [DisplayName("Vendor Code")]
        public int VendorCode { get; set; }

        [Required]
        [DisplayName("Full-Time Equivalent (FTE)")]
        public decimal FTE { get; set; }

        [DisplayName("CLI Funding")]
        public int CLIFundingId { get; set; }

        [DisplayName("Funded (Employed) Through")]
        public FundedThrough FundedThrough { get; set; }

        [EensureEmptyIfNullAttribute]
        [StringLength(100)]
        public string FundedThroughOther { get; set; }

        [EensureEmptyIfNullAttribute]
        [DisplayName("Guest Account Sponsor")]
        [StringLength(50)]
        public string AccountSponsor { get; set; }

        [EensureEmptyIfNullAttribute]
        [StringLength(140)]
        [DisplayName("Employer")]
        public string Employer { get; set; }

        public bool IsAssessmentEquipment { get; set; }

        [EensureEmptyIfNullAttribute]
        [StringLength(600)]
        [DisplayName("Coach Notes")]
        public string CoordCoachNotes { get; set; }

        [Required]
        public virtual UserBaseEntity User { get; set; }

        public virtual ICollection<CoordCoachEquipmentEntity> CoordCoachEquipments { get; set; }
    }
}
