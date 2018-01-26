using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Sunnet.Cli.Core.Common.Enums;
using Sunnet.Cli.Core.Schools.Entities;
using Sunnet.Cli.Core.TRSClasses.Enums;
using Sunnet.Framework.Core.Base;
using Sunnet.Framework.Mvc;
using Sunnet.Cli.Core.Users.Entities;

namespace Sunnet.Cli.Core.TRSClasses.Entites
{
    public class TRSClassEntity : EntityBase<int>
    {
        [Required]
        [DisplayName("School Name")]
        public int SchoolId { get; set; }

        /// <summary>
        /// 使用触发器来处理值
        /// </summary>
        [EensureEmptyIfNull]
        [StringLength(32)]
        [DisplayName("Class Engage ID")]
        public string TRSClassId { get; set; }

        [Required]
        [StringLength(150)]
        [DisplayName("Class Name")]
        public string Name { get; set; }

        [Required]
        [DisplayName("Class Status")]
        public EntityStatus Status { get; set; }

        [EensureEmptyIfNull]
        [DisplayName("Class Status Date")]
        public DateTime StatusDate { get; set; }

        [EensureEmptyIfNull]
        [StringLength(5)]
        [DisplayName("School Year")]
        public string SchoolYear { get; set; }

        /// <summary>
        /// Teacher.Id 
        /// </summary>
        [DisplayName("Homeroom Teacher")]
        public int HomeroomTeacherId { get; set; }

        public int PlaygroundId { get; set; }

        /// <summary>
        /// SchoolSpecialist.UserInfo.ID
        /// </summary>
        [DisplayName("TRS Class Assessor")]
        public int TrsAssessorId { get; set; }

        /// <summary>
        /// SchoolSpecialist.UserInfo.ID
        /// </summary>
        [DisplayName("TRS Class Mentor")]
        public int? TrsMentorId { get; set; }

        [DisplayName("Type of Class")]
        public TRSClassofType TypeOfClass { get; set; }

        [EensureEmptyIfNull]
        [StringLength(600)]
        [DisplayName("Class Notes")]
        public string Notes { get; set; }

        public virtual SchoolEntity School { get; set; }

        public virtual TeacherEntity HomeroomTeacher { get; set; }
    }
}
