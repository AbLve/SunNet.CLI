using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/**************************************************************************
 * CreatedOn:		2014/8/23 17:36:23
 * Description:		Please input class summary
 * Version History:	Created,2014/8/23 17:36:23
 **************************************************************************/
using Sunnet.Cli.Core.Ade;
using Sunnet.Cli.Core.Classes.Entites;
using Sunnet.Cli.Core.Communities.Entities;
using Sunnet.Cli.Core.Schools.Entities;
using Sunnet.Cli.Core.Students.Enums;
using Sunnet.Cli.Core.Users.Entities;
using Sunnet.Cli.Core.Users.Enums;
using Sunnet.Framework.Core.Base;
using Sunnet.Cli.Core.Common.Enums;
using Sunnet.Framework.Mvc;

namespace Sunnet.Cli.Core.Students.Entities
{
    public class StudentEntity : EntityBase<int>
    {

        //[Required]
        //[DisplayName("School Name")]
        //public int SchoolId { get; set; }

        //[Required]
        //[DisplayName("Community/District")]
        //public int CommunityId { get; set; }

        /// <summary>
        /// 使用触发器
        /// </summary>
        [EensureEmptyIfNull]
        [DisplayName("Student Engage ID")]
        [StringLength(50)]
        public string StudentId { get; set; }

        [Required]
        [DisplayName("First Name")]
        [StringLength(140)]
        public string FirstName { get; set; }

        [EensureEmptyIfNull]
        [DisplayName("Middle Name")]
        [StringLength(140)]
        public string MiddleName { get; set; }

        [Required]
        [DisplayName("Last Name")]
        [StringLength(140)]
        public string LastName { get; set; }

        [Required]
        [DisplayName("Status")]
        public EntityStatus Status { get; set; }

        [DisplayName("Status Date")]
        public DateTime StatusDate { get; set; }

        [EensureEmptyIfNull]
        [StringLength(50)]
        [DisplayName("School Year")]
        public string SchoolYear { get; set; }
        [Required]
        [DisplayName("Birth Date")]
        public DateTime BirthDate { get; set; }

        [Required]
        [DisplayName("Gender")]
        public Gender Gender { get; set; }

        [DisplayName("Ethnicity")]
        public Ethnicity Ethnicity { get; set; }

        [EensureEmptyIfNull]
        [DisplayName("Ethnicity Other")]
        [StringLength(100, ErrorMessage = "This field is limited to 100 characters.")]
        public string EthnicityOther { get; set; }

        [Required]
        [DisplayName("Primary Language")]
        public int PrimaryLanguageId { get; set; }

        [EensureEmptyIfNull]
        [DisplayName("Primary Other")]
        [StringLength(100, ErrorMessage = "This field is limited to 100 characters.")]
        public string PrimaryLanguageOther { get; set; }

        [Required]
        [DisplayName("Secondary Language")]
        public int SecondaryLanguageId { get; set; }

        [EensureEmptyIfNull]
        [DisplayName("Secondary Other")]
        [StringLength(100)]
        public string SecondaryLanguageOther { get; set; }

        [Required]
        [DisplayName("Send parent invitation?")]
        public bool IsSendParent { get; set; }

        [DisplayName("Media Release?")]
        public MediaRelease IsMediaRelease { get; set; }

        [EensureEmptyIfNull]
        [DisplayName("Student Notes")]
        [StringLength(600, ErrorMessage = "This field is limited to 600 characters.")]
        public string Notes { get; set; }

        [DisplayName("Parent Code")]
        public string ParentCode { get; set; }

        [EensureEmptyIfNull]
        [DisplayName("Local Student ID")]
        [StringLength(100, ErrorMessage = "This field is limited to 100 characters.")]
        public string LocalStudentID { get; set; }


        [EensureEmptyIfNull]
        [DisplayName("TSDS Student ID")]
        [StringLength(100, ErrorMessage = "This field is limited to 100 characters.")]
        public string TSDSStudentID { get; set; }

        [DisplayName("Grade Level")]
        public StudentGradeLevel GradeLevel { get; set; }

        [Required]
        [DisplayName("Assessment Language")]
        public StudentAssessmentLanguage AssessmentLanguage { get; set; }

        public bool IsDeleted { get; set; }

        //public virtual CommunityEntity Community { get; set; }

        //public virtual SchoolEntity School { get; set; }

        public virtual ICollection<SchoolStudentRelationEntity> SchoolRelations { get; set; }

        public virtual ICollection<ClassEntity> Classes { get; set; }

        public virtual ICollection<ParentStudentRelationEntity> ParentStudents { get; set; }

    }
}
