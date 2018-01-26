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
 * CreatedOn:		2014/8/19 9:01:35
 * Description:		Please input class summary
 * Version History:	Created,2014/8/19 9:01:35
 * 
 * 
 **************************************************************************/
using Sunnet.Cli.Core.Classes.Entites;
using Sunnet.Cli.Core.Common.Enums;
using Sunnet.Cli.Core.Communities.Entities;
using Sunnet.Cli.Core.MasterData.Entities;
using Sunnet.Cli.Core.Schools.Enums;
using Sunnet.Cli.Core.Students.Entities;
using Sunnet.Cli.Core.Trs;
using Sunnet.Cli.Core.TRSClasses.Entites;
using Sunnet.Framework.Core.Base;
using Sunnet.Framework.Mvc;
using Sunnet.Cli.Core.Users.Entities;
using Sunnet.Cli.Core.StatusTracking.Entities;
using Sunnet.Cli.Core.Permission.Entities;

namespace Sunnet.Cli.Core.Schools.Entities
{
    public class SchoolEntity : EntityBase<int>
    {

        public SchoolEntity()
        {
            StateId = 0;
            TrsAssessorId = 0;
        }

        /// <summary>
        /// 使用触发器
        /// </summary>
        [EensureEmptyIfNull]
        [DisplayName("School Engage ID")]
        public string SchoolId { get; set; }

        [EensureEmptyIfNull]
        [DisplayName("School Name")]
        public string Name { get; set; }

        [Required]
        [DisplayName("School Name")]
        public int BasicSchoolId { get; set; }

        [Required]
        [DisplayName("School Status")]
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


        [DisplayName("School State")]
        public int? StateId { get; set; }

        [Required]
        [DisplayName("School Zip")]
        public string Zip { get; set; }

        [Required]
        [DisplayName("Phone #")]
        public string PhoneNumber { get; set; }

        [Required]
        [DisplayName("Phone # Type")]
        public PhoneType PhoneType { get; set; }

        [Required]
        [DisplayName("School Type")]
        public int SchoolTypeId { get; set; }

        [EensureEmptyIfNull]
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
        [DisplayName("% of At-Risk students in school (Approximately)")]
        [Range(0, 100)]
        public int AtRiskPercent { get; set; }

        [DisplayName("TEA Project Funding")]
        public int FundingId { get; set; }


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
        [DataType(DataType.EmailAddress)]
        public string PrimaryEmail { get; set; }

        [DisplayName("Salutation")]
        public UserSalutation SecondarySalutation { get; set; }

        [DisplayName("Name")]
        public string SecondaryName { get; set; }

        [EensureEmptyIfNull]
        [DisplayName("Title")]
        public int SecondaryTitleId { get; set; }

        [EensureEmptyIfNull]
        [DisplayName("Phone #")]
        public string SecondaryPhoneNumber { get; set; }

        [Required]
        [DisplayName("Phone # Type")]
        public PhoneType SecondaryPhoneType { get; set; }

        [EensureEmptyIfNull]
        [DisplayName("Email")]
        [DataType(DataType.EmailAddress)]
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


        //[Required]
        //[DisplayName("Community/District")]
        //public int CommunityId { get; set; }

        [EensureEmptyIfNull]
        [DisplayName("School Number")]
        public string SchoolNumber { get; set; }

        public int CreateBy { get; set; }
        public int UpdateBy { get; set; }
        [EensureEmptyIfNull]
        public string CreateFrom { get; set; }
        [EensureEmptyIfNull]
        public string UpdateFrom { get; set; }



        #region TRS


        /// <summary>
        /// SchoolSpecialist.UserInfo.ID
        /// </summary>
        [DisplayName("TRS Assessor")]
        public int? TrsAssessorId { get; set; }

        [DisplayName("TRS TA Status")]
        [EensureEmptyIfNull]
        public string TrsTaStatus { get; set; }

        [DisplayName("Recertification By")]
        public DateTime RecertificatedBy { get; set; }

        [DisplayName("Enable Auto-Assign 4 ★")]
        public bool EnableAutoAssign4Star { get; set; }

        [DisplayName("DFPS License #/Operator #")]
        [EensureEmptyIfNull]
        public string DfpsNumber { get; set; }

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
        [EensureEmptyIfNull]
        public FacilityType FacilityType { get; set; }

        [DisplayName("Regulating Entity")]
        [EensureEmptyIfNull]
        public Regulating RegulatingEntity { get; set; }

        [DisplayName("National Association for the Education of Young Children (NAEYC)")]
        [EensureEmptyIfNull]
        public bool NAEYC { get; set; }

        [DisplayName("Council of Accreditation - National After School Association (COA)")]
        [EensureEmptyIfNull]
        public bool CANASA { get; set; }

        [DisplayName("National Early Childhood Program Accreditation (NECPA)")]
        [EensureEmptyIfNull]
        public bool NECPA { get; set; }

        [DisplayName("National Accreditation Commission for Early Child Care and Education (NAC)")]
        [EensureEmptyIfNull]
        public bool NACECCE { get; set; }

        [DisplayName("National Association of Family Child Care (NAFCC)")]
        [EensureEmptyIfNull]
        public bool NAFCC { get; set; }

        [DisplayName("Association of Chistion Schools International (ACSI)")]
        [EensureEmptyIfNull]
        public bool ACSI { get; set; }


        [DisplayName("U.S. Military")]
        [EensureEmptyIfNull]
        public bool USMilitary { get; set; }

        [DisplayName("AdvancED Quality Early Learning Standards QELS (QELS)")]
        public bool QELS { get; set; }

        /// <summary>
        /// Verified Star.
        /// </summary>
        [DisplayName("Verified Star Designation")]
        [EensureEmptyIfNull]
        public TRSStarEnum VSDesignation { get; set; }

        /// <summary>
        /// Verified Star Updated On
        /// </summary>
        [DisplayName("TRS Last Status Date Change")]
        public DateTime TrsLastStatusChange { get; set; }

        /// <summary>
        /// Calculated Star Updated On.
        /// </summary>
        [DisplayName("Star designation date")]
        public DateTime StarDate { get; set; }

        /// <summary>
        /// Calculated Star
        /// </summary>
        [DisplayName("Calculated Star Designation")]
        [EensureEmptyIfNull]
        public TRSStarEnum StarStatus { get; set; }

        /// <summary>
        /// Calculated Star Assessment Type
        /// </summary>
        public StarAssessmentType StarAssessmentType { get; set; }
        #endregion

        public virtual FundingEntity Funding { get; set; }
        public virtual CountyEntity County { get; set; }
        public virtual CountyEntity MailCounty { get; set; }
        public virtual StateEntity State { get; set; }
        public virtual StateEntity MailState { get; set; }
        public virtual BasicSchoolEntity BasicSchool { get; set; }
        public virtual SchoolTypeEntity SchoolType { get; set; }


        /// <summary>
        /// 对象有可能为空
        /// </summary>
        public virtual UserBaseEntity Assessor { get; set; }


        public virtual ICollection<ClassEntity> Classes { get; set; }

        public virtual ICollection<TRSClassEntity> TRSClasses { get; set; }

        public virtual ICollection<SchoolStudentRelationEntity> StudentRelations { get; set; }


        public virtual ICollection<UserComSchRelationEntity> UserCommunitySchools { get; set; }

        public virtual ICollection<CommunitySchoolRelationsEntity> CommunitySchoolRelations { get; set; }

        public virtual ICollection<StatusTrackingEntity> StatusTrackings { get; set; }

        public virtual ICollection<AssignedPackageEntity> AssignedPackages { get; set; }
    }

    public enum Regulating : byte
    {
        [Description("State of Texas (DFPS)")]
        StateOfTX = 1,

        [Description("U.S. Military")]
        USMilitary = 2
    }

    public class SchoolSelectItemEntity
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string City { get; set; }


        public string County { get; set; }
        public string SchoolId { get; set; }
        public int CommunityId { get; set; }
        public string Address { get; set; }
        public int CountyId { get; set; }
        public int StateId { get; set; }
        public string Zip { get; set; }
        public string Phone { get; set; }
        public string SchoolNumber { get; set; }
        public string State { get; set; }
    }
}
