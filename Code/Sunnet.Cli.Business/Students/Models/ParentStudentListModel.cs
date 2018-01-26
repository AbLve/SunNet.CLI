using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sunnet.Cli.Core.Common.Enums;
using Sunnet.Cli.Core.Students.Enums;
using Sunnet.Cli.Core.Users.Entities;
using Sunnet.Cli.Core.Users.Enums;
using Sunnet.Framework.Mvc;

namespace Sunnet.Cli.Business.Students.Models
{
    public class ParentStudentListModel
    {
        /// <summary>
        /// Parent和Student关系表的Id，在Add Parents中用来判断更新还是添加操作
        /// </summary>
        public int ID { get; set; }

        public int StudentId { get; set; }

        public int ParentId { get; set; }

        [Required]
        [Display(Name = "Child First Name")]
        public string ChildFirstName { get; set; }

        [Required]
        [Display(Name = "Child Last Name")]
        public string ChildLastName { get; set; }

        [Required]
        [Display(Name = "Child DOB")]
        public DateTime BirthDate { get; set; }

        [Display(Name = "Parent Pin For Progress Monitoring")]
        public string ParentCode { get; set; }

        [Required]
        [Display(Name = "Parent/guardian First Name")]
        public string ParentFirstName { get; set; }

        [Required]
        [Display(Name = "Parent/guardian Last Name")]
        public string ParentLastName { get; set; }

        [Required]
        [Display(Name = "Parent/guardian Email")]
        public string ParentPrimaryEmail { get; set; }

        [Display(Name = "Status")]
        public ParentStatus ParentStatus { get; set; }

        public string Color { get; set; }
        public string Goal { get; set; }
    }
}
