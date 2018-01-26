using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/**************************************************************************
 * Developer: 		Leason
 * Computer:		Leason-PC
 * Domain:			Leason-pc
 * CreatedOn:		2014/8/18 16:27:20
 * Description:		Create CommunitiesRspt
 * Version History:	Created,2014/8/18 16:27:20
 * 
 * 
 **************************************************************************/
using Sunnet.Cli.Core.Classes.Entites;
using Sunnet.Cli.Core.Common.Enums;
using Sunnet.Cli.Core.MasterData.Entities;
using Sunnet.Cli.Core.Schools.Entities;
using Sunnet.Cli.Core.Students.Entities;
using Sunnet.Framework.Core.Base;
using Sunnet.Framework.Mvc;
using Sunnet.Cli.Core.Users.Entities;
using Sunnet.Cli.Core.Permission.Entities;
using Sunnet.Cli.Core.BUP.Entities;

namespace Sunnet.Cli.Core.Communities.Entities
{
    public class CommunityEntity : EntityBase<int>
    {
        /// <summary>
        /// 触发生成
        /// </summary>
        [EensureEmptyIfNull]
        [StringLength(32)]
        [DisplayName("Engage ID")]
        public string CommunityId { get; set; }

        [Required]
        [DisplayName("Community Name")]
        public string Name { get; set; }

        [Required]
        [DisplayName("Name")]
        public int BasicCommunityId { get; set; }

        [Required]
        [DisplayName("Status")]
        public EntityStatus Status { get; set; }

        [EensureEmptyIfNull]
        [DisplayName("Status Date")]
        public DateTime StatusDate { get; set; }

        [EensureEmptyIfNull]
        [StringLength(5)]
        [DisplayName("School Year")]
        public string SchoolYear { get; set; }

        [Required]
        [DisplayName("Type (Funded by)")]
        public int FundingId { get; set; }

        [Required]
        [StringLength(500)]
        [DisplayName("Physical Address1 ")]
        public string PhysicalAddress1 { get; set; }

        [EensureEmptyIfNull]
        [StringLength(500)]
        [DisplayName("Physical Address2")]
        public string PhysicalAddress2 { get; set; }

        [Required]
        [StringLength(140)]
        [DisplayName("City")]
        public string City { get; set; }

        [Required]
        [DisplayName("County")]
        public int CountyId { get; set; }

        [Required]
        [DisplayName("State")]
        public int StateId { get; set; }

        [Required]
        [StringLength(10)]
        [DisplayName("ZIP Code")]
        public string Zip { get; set; }

        [Required]
        [StringLength(15)]
        [DisplayName("Phone #")]
        public string PhoneNumber { get; set; }

        [Required]
        [DisplayName("Phone # Type")]
        public PhoneType PhoneNumberType { get; set; }


        [Required]
        [DisplayName("Salutation")]
        public UserSalutation PrimarySalutation { get; set; }

        [Required]
        [StringLength(150)]
        [DisplayName("Name")]
        public string PrimaryName { get; set; }

        [Required]
        [DisplayName("Title")]
        public int PrimaryTitleId { get; set; }

        [Required]
        [StringLength(150)]
        [DisplayName("Phone #")]
        public string PrimaryPhone { get; set; }

        [Required]
        [DisplayName("Phone # Type")]
        public PhoneType PrimaryPhoneType { get; set; }

        [Required]
        [StringLength(150)]
        [DataType(DataType.EmailAddress)]
        [DisplayName("Email")]
        public string PrimaryEmail { get; set; }

        [Required]
        [DisplayName("Salutation")]
        public UserSalutation SecondarySalutation { get; set; }

        [EensureEmptyIfNull]
        [StringLength(150)]
        [DisplayName("Name")]
        public string SecondaryName { get; set; }

        [EensureEmptyIfNull]
        [DisplayName("Title")]
        public int SecondaryTitleId { get; set; }

        [EensureEmptyIfNull]
        [StringLength(150)]
        [DisplayName("Phone #")]
        public string SecondaryPhone { get; set; }

        [EensureEmptyIfNull]
        [DisplayName("Phone # Type")]
        public PhoneType SecondaryPhoneType { get; set; }

        [EensureEmptyIfNull]
        [StringLength(150)]
        [DataType(DataType.EmailAddress)]
        [DisplayName("Email")]
        public string SecondaryEmail { get; set; }

        [EensureEmptyIfNull]
        [StringLength(500)]
        [DataType(DataType.Url)]
        [DisplayName("Web Address")]
        public string WebAddress { get; set; }

        [Required]
        [DisplayName("MOU Status")]
        public bool MouStatus { get; set; }

        [EensureEmptyIfNull]
        [DisplayName("MOU Document")]
        public string MouDocument { get; set; }

        [EensureEmptyIfNull]
        [StringLength(600)]
        [DisplayName("Notes")]
        public string Notes { get; set; }

        [EensureEmptyIfNull]
        [StringLength(100)]
        [DisplayName("District Number")]
        public string DistrictNumber { get; set; }

        [EensureEmptyIfNull]
        [DisplayName("Logo")]
        public string LogoUrl { get; set; }


        #region  virtual
        public virtual FundingEntity Funding { get; set; }
        public virtual CountyEntity County { get; set; }
        public virtual StateEntity State { get; set; }
        public virtual TitleEntity PrimaryTitle { get; set; }
        public virtual TitleEntity SecondaryTitle { get; set; }
        public virtual BasicCommunityEntity BasicCommunity { get; set; }
        #endregion

        public virtual ICollection<UserComSchRelationEntity> UserCommunitySchools { get; set; }

        public virtual ICollection<CommunitySchoolRelationsEntity> CommunitySchoolRelations { get; set; }

        public virtual ICollection<CommunityAssessmentRelationsEntity> CommunityAssessmentRelations { get; set; }

        public virtual ICollection<AssignedPackageEntity> AssignedPackages { get; set; }

        public virtual ICollection<AutomationSettingEntity> AutomationSettings { get; set; }

    }

    public enum MouStatus : byte
    {
        [Description("Not Signed")]
        NotSigned = 0,
        [Description("Signed")]
        Signed = 1
    }

    public enum ModelName
    {
        Community,
        School,
        Classroom,
        Class,
        Student,
        Teacher
    }
}
