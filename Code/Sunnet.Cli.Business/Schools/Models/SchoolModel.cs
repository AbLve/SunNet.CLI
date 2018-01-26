using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/**************************************************************************
 * Developer: 		Sam
 * Computer:		SAM-PC
 * Domain:			sam-pc
 * CreatedOn:		2014/8/28 18:26:10
 * Description:		Please input class summary
 * Version History:	Created,2014/8/28 18:26:10
 * 
 * 
 **************************************************************************/
using Newtonsoft.Json;
using Sunnet.Cli.Business.Common;
using Sunnet.Cli.Core.Common.Enums;
using Sunnet.Cli.Core.Communities.Entities;
using Sunnet.Cli.Core.Schools.Entities;
using Sunnet.Cli.Core.Schools.Enums;
using Sunnet.Cli.Core.Trs;
using Sunnet.Cli.Core.Users.Entities;
using Sunnet.Framework.Mvc;
using Sunnet.Framework.Extensions;

namespace Sunnet.Cli.Business.Schools.Models
{
    public class SchoolModel
    {
        private TRSStarEnum _vsDesignation;
        public int ID { get; set; }

        [DisplayName("Name")]
        public string SchoolName { get; set; }

        public string SchoolType { get; set; }
        [DisplayName("Community ")]
        public string CommunityName { get; set; }
        [DisplayName("Communities ")]
        [JsonIgnore]
        public IEnumerable<string> CommunityNames { get; set; }
        [DisplayName("Community")]
        public string CommunityNameText
        {
            get
            {
                if (CommunityNames != null && CommunityNames.Any())
                    return CommunityNames.Aggregate<string, string>(null, (current, next) => (current == null ? "" : current + ", ") + next);
                return "";
            }
        }
        /// <summary>
        /// 默认某一个communityId 主要为新建
        /// </summary>
        [DisplayName("Community/District")]
        public int CommunityId { get; set; }

        [DisplayName("School Engage ID")]
        public string SchoolId { get; set; }
        [Required]
        [DisplayName("School Name")]
        public int BasicSchoolId { get; set; }
        [Required]
        [DisplayName("Status")]
        public SchoolStatus Status { get; set; }
        [DisplayName("Status Date")]
        public DateTime StatusDate { get; set; }
        [Required]
        [DisplayName("School Year")]
        public string SchoolYear { get; set; }
        [DisplayName("ESC Name")]
        public int EscName { get; set; }
        [Required]
        [DisplayName("Parent Agency")]
        public int ParentAgencyId { get; set; }
        [Required]
        [DisplayName("School Physical Address1")]
        public string PhysicalAddress1 { get; set; }
        [EensureEmptyIfNull]
        [DisplayName("School Physical Address2")]
        public string PhysicalAddress2 { get; set; }
        [Required]
        [DisplayName("School City")]
        public string City { get; set; }
        [Required]
        [DisplayName("School County")]
        public int CountyId { get; set; }
        [Required]
        [DisplayName("School State")]
        public int StateId { get; set; }
        [Required]
        [DisplayName("School Zip")]
        public string Zip { get; set; }
        [Required]
        [DisplayName("Phone #")]
        public string PhoneNumber { get; set; }
        [Required]
        [DisplayName("Phone # Type")]
        public PhoneType PhoneType { get; set; }

        [DisplayName("School Type")]
        public int SchoolTypeId { get; set; }
        [EensureEmptyIfNull]
        [DisplayName("Sub School Type")]
        public int SubTypeId { get; set; }
        [DisplayName("Pre-school (3 year-old classroom) -- CLASSROOM COUNT")]
        public int ClassroomCount3Years { get; set; }
        [DisplayName("Pre-school (4 year-old classroom) -- CLASSROOM COUNT")]
        public int ClassroomCount4Years { get; set; }
        [DisplayName("Pre-school (mixed 3 and 4 year-old classroom) -- CLASSROOM COUNT")]
        public int ClassroomCount34Years { get; set; }
        [DisplayName("Kindergarten -- CLASSROOM COUNT")]
        public int ClassroomCountKinder { get; set; }
        [DisplayName("1st grade+ --  CLASSROOM COUNT")]
        public int ClassroomCountgrade { get; set; }
        [DisplayName("Other -- CLASSROOM COUNT")]
        public int ClassroomCountOther { get; set; }
        [DisplayName("# of early Head Start classrooms -- CLASSROOM COUNT")]
        public int ClassroomCountEarly { get; set; }
        [DisplayName("Toddler -- CLASSROOM COUNT")]
        public int ClassroomCountToddler { get; set; }
        [DisplayName("Infant -- CLASSROOM COUNT")]
        public int ClassroomCountInfant { get; set; }
        [DisplayName("Pre-school -- CLASSROOM COUNT")]
        public int ClassroomCountPreSchool { get; set; }
        [Required]
        [Range(0, 100)]
        [DisplayName("% of At-Risk students in school (Approximately)")]
        public int AtRiskPercent { get; set; }
        [DisplayName("TEA Project Funding")]
        public int FundingId { get; set; }
        //[Required]
        //[DisplayName("Texas Rising Star (TRS) Provider?")]
        //public int TrsProviderId { get; set; }

        [DisplayName("TRS Last Status Date Change")]
        public DateTime TrsLastStatusChange { get; set; }

        public string TrsLastStatusChangeFormat
        {
            get
            {
                if (TrsLastStatusChange < new DateTime(1900, 1, 1))
                    return "";
                return TrsLastStatusChange.ToString("MM/dd/yyyy");
            }
        }

        //[DisplayName("TRS Review Date")]
        //public DateTime TrsReviewDate { get; set; }
        [DisplayName("Salutation")]
        public UserSalutation PrimarySalutation { get; set; }
        [Required]
        [DisplayName("Name")]
        public string PrimaryName { get; set; }
        [Required]
        [DisplayName("Title")]
        public int PrimaryTitleId { get; set; }
        [Required]
        [DisplayName("Phone #")]
        public string PrimaryPhone { get; set; }
        [Required]
        [DisplayName("Phone # Type")]
        public PhoneType PrimaryPhoneType { get; set; }
        [Required]
        [DisplayName("Email")]
        public string PrimaryEmail { get; set; }
        [DisplayName("Salutation")]
        public UserSalutation SecondarySalutation { get; set; }
        [EensureEmptyIfNull]
        [DisplayName("Name")]
        public string SecondaryName { get; set; }

        [DisplayName("Title")]
        public int SecondaryTitleId { get; set; }
        [EensureEmptyIfNull]
        [DisplayName("Phone #")]
        public string SecondaryPhoneNumber { get; set; }

        [DisplayName("Phone # Type")]
        public PhoneType SecondaryPhoneType { get; set; }
        [EensureEmptyIfNull]
        [DisplayName("Email")]
        public string SecondaryEmail { get; set; }
        [EensureEmptyIfNull]
        public string Latitude { get; set; }
        [EensureEmptyIfNull]
        public string Longitude { get; set; }
        [Required]
        [DisplayName("Same as School Physical Address")]
        public bool IsSamePhysicalAddress { get; set; }
        [Required]
        [DisplayName("School Mailing Address1")]
        public string MailingAddress1 { get; set; }
        [EensureEmptyIfNull]
        [DisplayName("School Mailing Address2")]
        public string MailingAddress2 { get; set; }
        [Required]
        [DisplayName("School Mailing City")]
        public string MailingCity { get; set; }
        [Required]
        [DisplayName("School Mailing County")]
        public int MailingCountyId { get; set; }
        [Required]
        [DisplayName("School Mailing State")]
        public int MailingStateId { get; set; }
        [Required]
        [DisplayName("School Mailing Zip")]
        public string MailingZip { get; set; }

        [DisplayName("School size")]
        [Range(0, 99999)]
        public int SchoolSize { get; set; }
        [DisplayName("Internet Service Provider (ISP)")]
        public int IspId { get; set; }

        [EensureEmptyIfNull]
        [DisplayName("Other")]
        public string IspOther { get; set; }
        [DisplayName("Internet Speed (in Main Office)")]
        public InternetSpeed InternetSpeed { get; set; }
        [DisplayName("Internet Type (in Main Office)")]
        public InternetType InternetType { get; set; }
        [DisplayName("Type of Wireless")]
        public WireLessType WirelessType { get; set; }
        [StringLength(600, ErrorMessage = "This field is limited to 600 characters.")]
        [DisplayName("School Notes")]
        [EensureEmptyIfNull]
        public string Notes { get; set; }

        [EensureEmptyIfNull]
        [DisplayName("School Number")]
        public string SchoolNumber { get; set; }

        public DateTime CreatedOn { get; set; }

        public DateTime UpdatedOn { get; set; }

        public int PrimaryCommunityId { get; set; }
        public AccessType SchoolAccess { get; set; }

        public int CreateBy { get; set; }
        public int UpdateBy { get; set; }
        [EensureEmptyIfNull]
        public string CreateFrom { get; set; }
        [EensureEmptyIfNull]
        public string UpdateFrom { get; set; }

        [DisplayName("TRS Status")]
        public string TrsStatus
        {
            get
            {
                if (TrsAssessorId == 0 && VSDesignation == 0)
                {
                    return "Not Participating";
                }
                else
                {
                    return "Participating";
                }
            }
        }

        [DisplayName("Star Rating")]
        public string StarRating
        {
            get
            {
                if (VSDesignation == 0)
                {
                    return "No Star Level Available";
                }
                else
                {
                    return VSDesignation.ToDescription();
                }
            }
        }

        #region TRS
        [DisplayName("TRS Assessor")] //ticket 1944
        public int TrsAssessorId { get; set; }

        [DisplayName("TRS TA Status")]
        [EensureEmptyIfNull]
        public string TrsTaStatus { get; set; }

        [DisplayName("Calculated Star Designation")]
        [EensureEmptyIfNull]
        public TRSStarEnum StarStatus { get; set; }

        public string StrStarStatus
        {
            get
            {
                if (StarStatus > 0)
                    return StarStatus.ToDescription();
                else
                    return "";
            }
        }

        [DisplayName("DFPS License #/Operator #")]
        [EensureEmptyIfNull]
        public string DfpsNumber { get; set; }

        [DisplayName("Recertification By")]
        public DateTime RecertificatedBy { get; set; }

        [DisplayName("Enable Auto-Assign 4 ★")]
        public bool EnableAutoAssign4Star { get; set; }

        [DisplayName("Owner First Name")]
        [EensureEmptyIfNull]
        public string OwnerFirstName { get; set; }

        [DisplayName("Owner Last Name")]
        [EensureEmptyIfNull]
        public string OwnerLastName { get; set; }

        [DisplayName("Owner Email")]
        [EensureEmptyIfNull]
        public string OwnerEmail { get; set; }

        [DisplayName("Owner Phone")]
        [EensureEmptyIfNull]
        public string OwnerPhone { get; set; }

        [DisplayName("Facility Type")]
        public FacilityType FacilityType { get; set; }

        [DisplayName("Regulating Entity")]
        public Regulating RegulatingEntity { get; set; }

        [DisplayName("National Association for the Education of Young Children (NAEYC)")]
        public bool NAEYC { get; set; }

        [DisplayName("Council of Accreditation - National After School Association (COA)")]
        public bool CANASA { get; set; }

        [DisplayName("National Early Childhood Program Accreditation (NECPA)")]
        public bool NECPA { get; set; }

        [DisplayName("National Accreditation Commission for Early Child Care and Education (NAC)")]
        public bool NACECCE { get; set; }

        [DisplayName("National Association of Family Child Care (NAFCC)")]
        public bool NAFCC { get; set; }

        [DisplayName("Association of Chistion Schools International (ACSI)")]
        public bool ACSI { get; set; }


        [DisplayName("U.S. Military")]
        public bool USMilitary { get; set; }

        [DisplayName("AdvancED Quality Early Learning Standards QELS (QELS)")]
        public bool QELS { get; set; }

        [DisplayName("Verified Star Designation")]
        public TRSStarEnum VSDesignation { get; set; }

        public string StrVSDesignation
        {
            get
            {
                if (VSDesignation > 0)
                    return VSDesignation.ToDescription();
                else
                    return "";
            }
        }

        [DisplayName("Star designation date")]
        public DateTime StarDate { get; set; }

        public string StarDateFormat
        {
            get
            {
                if (StarDate < new DateTime(1900, 1, 1))
                    return "";
                return StarDate.ToString("MM/dd/yyyy");
            }
        }

        /// <summary>
        /// Calculated Star Assessment Type
        /// </summary>
        public StarAssessmentType StarAssessmentType { get; set; }

        public string StrStarAssessmentType
        {
            get
            {
                if (StarAssessmentType > 0)
                    return StarAssessmentType.ToDescription();
                else
                    return "";
            }
        }
        #endregion


    }
    public class SchoolListModel
    {

        public int ID { get; set; }

        [DisplayName("Name")]
        public string SchoolName { get; set; }

        public string SchoolType { get; set; }
        [DisplayName("Community ")]
        public string CommunityName { get; set; }
        [DisplayName("Communities ")]
        [JsonIgnore]
        public IEnumerable<string> CommunityNames { get; set; }

        public string CommunityNameText
        {
            get
            {
                if (CommunityNames != null && CommunityNames.Any())
                    return CommunityNames.Aggregate<string, string>(null, (current, next) => (current == null ? "" : current + ", ") + next);
                return "";
            }
        }
        /// <summary>
        /// 默认某一个communityId 主要为新建
        /// </summary>
        [DisplayName("Community/District")]
        public int CommunityId { get; set; }

        [DisplayName("School ID")]
        public string SchoolId { get; set; }
        [DisplayName("School Number")]
        public string SchoolNumber { get; set; }

        [Required]
        [DisplayName("Status")]
        public SchoolStatus Status { get; set; }

        public bool IsTrsAccess { get; set; }

        public int PrimaryCommunityId { get; set; }
        public AccessType SchoolAccess { get; set; }
    }
}
