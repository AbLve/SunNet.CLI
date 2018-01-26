using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sunnet.Cli.Core.Common.Enums;
using Sunnet.Cli.Core.Students.Enums;
using Sunnet.Cli.Core.Users.Enums;
using Sunnet.Framework.Mvc;

namespace Sunnet.Cli.Business.Students.Models
{
    public class StudentListModel
    {
        public int StudentId { get; set; }

        [Display(Name = "PIN Code")]
        public string ParentId { get; set; }

        [Required]
        [Display(Name = "Child's First Name")]
        public string ChildFirstName { get; set; }

        [Required]
        [Display(Name = "Child's Last Name")]
        public string ChildLastName { get; set; }

        [EensureEmptyIfNull]
        [Display(Name = "Relationship to Child")]
        public ParentRelation Relation { get; set; }

        [EensureEmptyIfNull]
        public string RelationOther { get; set; }

        [Required]
        [Display(Name = "Child's date of birth")]
        public DateTime BirthDate { get; set; }

        [EensureEmptyIfNull]
        [Display(Name = "Child's Primary Language")]
        public int PrimaryLanguageId { get; set; }

        [EensureEmptyIfNull]
        [Display(Name = "Child's Primary Language")]
        public string PrimaryLanguage { get; set; }

        [EensureEmptyIfNull]
        public string PrimaryLanguageOther { get; set; }

        [EensureEmptyIfNull]
        [Display(Name = "Grade level")]
        public StudentGradeLevel GradeLevel { get; set; }

        public EntityStatus Status { get; set; }
    }
}
