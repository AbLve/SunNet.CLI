using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/**************************************************************************
 * Developer: 		Damon
 * Computer:		Damon-PC
 * Domain:			Damon-pc
 * CreatedOn:		2014/8/8 10:58:18
 * Description:		Please input class summary
 * Version History:	Created,2014/8/8 10:58:18
 * 
 * 
 **************************************************************************/
using Sunnet.Framework.Core.Base;
using System.ComponentModel.DataAnnotations;
using Sunnet.Framework.Mvc;
using System.ComponentModel;
using Sunnet.Cli.Core.Communities.Entities;
using Sunnet.Cli.Core.Common.Enums;
using Sunnet.Cli.Core.Users.Enums;

namespace Sunnet.Cli.Core.Users.Entities
{
    public class CommunityUserEntity : EntityBase<int>
    {
        public CommunityUserEntity()
        {
            ParentId = 0;
            SchoolYear = "";
            Gender = 0;
            PositionId = 0;
            PositionOther = "";
            IsSameAddress = 0;
            Address = "";
            Address2 = "";
            City = "";
            CountyId = 0;
            StateId = 0;
            Zip = "";
            PrimaryLanguageId = 0;
            PrimaryLanguageOther = "";
            TotalYearCurrentRole = 0;
            EducationLevel = 0;
            CommunityNotes = "";
            CommunityLevelRequest = false;
            SchoolLevelRequest = false;
        }

        [EensureEmptyIfNullAttribute]
        [StringLength(50)]
        [DisplayName("Community/District User Engage ID")]
        public string CommunityUserId { get; set; }

        /// <summary>
        /// User表ID
        /// </summary>
        [Required]
        public int ParentId { get; set; }

        [EensureEmptyIfNullAttribute]
        [StringLength(5)]
        [DisplayName("School Year")]
        public string SchoolYear { get; set; }

        [DisplayName("Birth Date")]
        public DateTime? BirthDate { get; set; }

        [DisplayName("Gender")]
        public Gender Gender { get; set; }

        [Required]
        [DisplayName("Title/Role")]
        public int PositionId { get; set; }

        [EensureEmptyIfNullAttribute]
        [StringLength(100)]
        public string PositionOther { get; set; }

        [Required]
        [DisplayName("Work address the same as Community/District Physical Address? ")]
        public int IsSameAddress { get; set; }

        [Required(AllowEmptyStrings = true)]
        [StringLength(500)]
        [DisplayName("Work Site Physical Address1 (No P.O. Boxes)")]
        public string Address { get; set; }

        [EensureEmptyIfNullAttribute]
        [StringLength(500)]
        public string Address2 { get; set; }

        [Required(AllowEmptyStrings = true)]
        [StringLength(200)]
        public string City { get; set; }

        [Required]
        [DisplayName("County")]
        public int CountyId { get; set; }

        [Required]
        [DisplayName("State")]
        public int StateId { get; set; }

        [Required(AllowEmptyStrings = true)]
        [StringLength(5)]
        public string Zip { get; set; }

        [DisplayName("Primary Language")]
        public int PrimaryLanguageId { get; set; }

        [EensureEmptyIfNullAttribute]
        [StringLength(100)]
        [DisplayName("Primary Language Other")]
        public string PrimaryLanguageOther { get; set; }

        [DisplayName("Total Years in Current Role")]
        public int TotalYearCurrentRole { get; set; }

        [DisplayName("Highest Education Level Attained")]
        public EducationLevel EducationLevel { get; set; }

        [EensureEmptyIfNullAttribute]
        [StringLength(600)]
        [DisplayName("Community/District Notes")]
        public string CommunityNotes { get; set; }

        //Community User级别用户注册时是否发送邮件
        [DisplayName("Community/District level Requests")]
        public bool CommunityLevelRequest { get; set; }

        //Principal级别用户注册时是否发送邮件
        [DisplayName("School level Requests")]
        public bool SchoolLevelRequest { get; set; }

        [Required]
        public virtual UserBaseEntity UserInfo { get; set; }
    }

}
