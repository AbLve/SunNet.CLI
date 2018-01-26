using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sunnet.Framework.Mvc;

namespace Sunnet.Cli.Business.Students.Models
{
    public class ChildListModel
    {
        public int ID { get; set; }

        [Required]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Required]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Required]
        [Display(Name = "Birth Date")]
        public DateTime BirthDate { get; set; }

        [EensureEmptyIfNullAttribute]
        [Display(Name = "School City")]
        public string SchoolCity { get; set; }

        [EensureEmptyIfNullAttribute]
        [Display(Name = "School Zip")]
        public string SchoolZip { get; set; }

        public int SchoolId { get; set; }

        [EensureEmptyIfNull]
        [Display(Name = "School your child attends")]
        public string SchoolName { get; set; }

        [EensureEmptyIfNullAttribute]
        [Display(Name = "Enter PIN provided by Teacher")]
        public string PINCode { get; set; }

        [Display(Name = "Primary Language")]
        public string PrimaryLanguage { get; set; }

        [Display(Name = "Other Language")]
        public string OtherLanguage { get; set; }

        public int StudentId { get; set; }
    }
}
