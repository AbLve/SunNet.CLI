using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/**************************************************************************
 * Developer: 		Sam
 * Computer:		SAM-PC
 * Domain:			sam-pc
 * CreatedOn:		2014/8/5 16:10:18
 * Description:		Please input class summary
 * Version History:	Created,2014/8/5 16:10:18
 * 
 * 
 **************************************************************************/
using Sunnet.Framework.Core.Base;
using System.ComponentModel.DataAnnotations;
using Sunnet.Cli.Core.Users.Enums;
using Sunnet.Cli.Core.Common.Enums;
using System.ComponentModel;
using Sunnet.Framework.Mvc;
using Sunnet.Cli.Core.Communities.Entities;
using Sunnet.Cli.Core.Schools.Entities;
using Sunnet.Cli.Core.Classes.Entites;
using Sunnet.Cli.Core.Students.Enums;

namespace Sunnet.Cli.Core.Users.Entities
{
    public class TeacherEntity : EntityBase<int>
    {
        public TeacherEntity()
        {
            TeacherId = "";
            SchoolYear = "";
            Gender = 0;
            HomeMailingAddress = "";
            HomeMailingAddress2 = "";
            City = "";
            CountyId = 0;
            StateId = 0;
            Zip = string.Empty;
            Ethnicity = 0;
            EthnicityOther = "";
            PrimaryLanguageId = 0;
            PrimaryLanguageOther = "";
            SecondaryLanguageId = 0;
            SecondaryLanguageOther = "";
            TotalTeachingYears = 0;
            AgeGroupOther = "";
            CurrentAgeGroupTeachingYears = 0;
            TeacherType = 0;
            TeacherTypeOther = "";
            PDOther = "";
            EducationLevel = 0;
            CoachId = 0;
            CoachAssignmentWay = 0;
            CoachAssignmentWayOther = "";
            ECIRCLEAssignmentWay = 0;
            ECIRCLEAssignmentWayOther = "";
            YearsInProjectId = 0;
            CoreAndSupplemental = "";
            CoreAndSupplemental2 = "";
            CoreAndSupplemental3 = "";
            CoreAndSupplemental4 = "";
            VendorCode = 0;
            CoachingHours = 0;
            EmployedBy = 0;
            CLIFundingID = 0;
            MediaRelease = 0;
            IsAssessmentEquipment = false;
            TeacherNotes = "";
            TeacherTSDSID = "";
        }

        /// <summary>
        /// 使用触发器
        /// </summary>
        [EensureEmptyIfNullAttribute]
        [StringLength(50)]
        [DisplayName("Teacher Engage ID")]
        public string TeacherId { get; set; }

        [EensureEmptyIfNullAttribute]
        [StringLength(5)]
        [DisplayName("School Year")]
        public string SchoolYear { get; set; }

        //[Range(typeof(DateTime), "1753-1-1", "2100-1-1")]
        [DisplayName("Birth Date")]
        public DateTime? BirthDate { get; set; }

        [DisplayName("Gender")]
        public Gender Gender { get; set; }

        [EensureEmptyIfNullAttribute]
        [StringLength(200)]
        [DisplayName("Home Mailing Address")]
        public string HomeMailingAddress { get; set; }

        [EensureEmptyIfNullAttribute]
        [StringLength(200)]
        [DisplayName("Home Mailing Address2")]
        public string HomeMailingAddress2 { get; set; }

        [EensureEmptyIfNullAttribute]
        [StringLength(200)]
        public string City { get; set; }

        [EensureEmptyIfNullAttribute]
        public int CountyId { get; set; }

        [EensureEmptyIfNullAttribute]
        public int StateId { get; set; }

        [EensureEmptyIfNullAttribute]
        [StringLength(5)]
        public string Zip { get; set; }

        [DisplayName("Ethnicity")]
        public Ethnicity Ethnicity { get; set; }

        [EensureEmptyIfNullAttribute]
        [StringLength(100)]
        public string EthnicityOther { get; set; }

        /// <summary>
        /// Languages 表
        /// </summary>
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

        [DisplayName("Total Years Teaching")]
        public int TotalTeachingYears { get; set; }


        /// <summary>
        /// AgeGroup 选 Other时，填值，其它值写入多对多关系表 TeacherAgeGroups
        /// </summary>
        [EensureEmptyIfNullAttribute]
        [StringLength(100)]
        public string AgeGroupOther { get; set; }


        [DisplayName("Total Years Teaching Current Age Group (e.g. pre-K)")]
        public int CurrentAgeGroupTeachingYears { get; set; }

        [DisplayName("What type of teacher are you?")]
        public TeacherType TeacherType { get; set; }

        [EensureEmptyIfNullAttribute]
        [StringLength(100)]
        public string TeacherTypeOther { get; set; }

        /// <summary>
        /// ProfessionalDevelopments 表记录选 Other时，填值，其它值写入多对多关系表  UserPDRelations
        /// </summary>
        [EensureEmptyIfNullAttribute]
        [StringLength(100)]
        public string PDOther { get; set; }

        [DisplayName("Highest Education Level Attained")]
        public EducationLevel EducationLevel { get; set; }

        /// <summary>
        /// 绑定的是UserId
        /// </summary>
        [DisplayName("Coach Assignment (if any)")]
        public int CoachId { get; set; }

        [DisplayName("Coaching Model")]
        public AssignmentType CoachAssignmentWay { get; set; }

        [EensureEmptyIfNullAttribute]
        [StringLength(100)]
        public string CoachAssignmentWayOther { get; set; }

        [DisplayName("eCIRCLE Assignment (if any)")]
        public AssignmentType ECIRCLEAssignmentWay { get; set; }

        [EensureEmptyIfNullAttribute]
        [StringLength(100)]
        public string ECIRCLEAssignmentWayOther { get; set; }

        /// <summary>
        /// YearsInProjects 表
        /// </summary>
        [EensureEmptyIfNullAttribute]
        [DisplayName("Years in Project")]
        public int YearsInProjectId { get; set; }

        [EensureEmptyIfNullAttribute]
        [StringLength(140)]
        [DisplayName("Core and supplemental curriculum used")]
        public string CoreAndSupplemental { get; set; }

        [EensureEmptyIfNullAttribute]
        [StringLength(140)]
        [DisplayName("Core and supplemental curriculum used")]
        public string CoreAndSupplemental2 { get; set; }

        [EensureEmptyIfNullAttribute]
        [StringLength(140)]
        [DisplayName("Core and supplemental curriculum used")]
        public string CoreAndSupplemental3 { get; set; }

        [EensureEmptyIfNullAttribute]
        [StringLength(140)]
        [DisplayName("Core and supplemental curriculum used")]
        public string CoreAndSupplemental4 { get; set; }

        [DisplayName("Vendor Code")]
        public int VendorCode { get; set; }

        [DisplayName("Number of coaching hours received (per month)")]
        public decimal CoachingHours { get; set; }

        [DisplayName("Number of remote coaching cycles received (per month)")]
        public decimal? ReqCycles { get; set; }

        [EensureEmptyIfNullAttribute]
        [DisplayName("Employed By")]
        public EmployedBy EmployedBy { get; set; }

        /// <summary>
        /// Fundings 表
        /// </summary>
        [DisplayName("CLI Funding")]
        public int CLIFundingID { get; set; }

        [DisplayName("Media Release?")]
        public MediaRelease MediaRelease { get; set; }

        [DisplayName("UT Health issued assessment equipment?(e.g. netbook/ipad/cameras/etc.)")]
        public bool IsAssessmentEquipment { get; set; }

        [EensureEmptyIfNullAttribute]
        [StringLength(600)]
        [DisplayName("Teacher Notes")]
        public string TeacherNotes { get; set; }

        [EensureEmptyIfNull]
        [StringLength(50)]
        [DisplayName("Teacher TSDS ID")]
        public string TeacherTSDSID { get; set; }

        [Required]
        public virtual UserBaseEntity UserInfo { get; set; }

        public virtual ICollection<TeacherAgeGroupEntity> TeacherAgeGroups { get; set; }

        public virtual ICollection<TeacherEquipmentRelationEntity> TeacherEquipmentRelations { get; set; }

        public virtual ICollection<TeacherTransactionEntity> TeacherTransactions { get; set; }

        public virtual ICollection<ClassEntity> Classes { get; set; }
    }
}