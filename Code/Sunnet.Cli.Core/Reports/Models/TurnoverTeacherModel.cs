using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sunnet.Cli.Core.Common.Enums;
using Sunnet.Cli.Core.Users.Enums;

namespace Sunnet.Cli.Core.Reports.Models
{
    public class TurnoverTeacherModel
    {
        [DisplayName("Community Name")]
        public string CommunityName { get; set; }

        [DisplayName("School Name")]
        public string SchoolName { get; set; }

        [DisplayName("School Type")]
        public string SchoolType { get; set; }

        [DisplayName("School Status")]
        public EntityStatus SchoolStatus { get; set; }

        [DisplayName("Teacher First Name")]
        public string FirstName { get; set; }

        [DisplayName("Teacher Last Name")]
        public string LastName { get; set; }


        [DisplayName("Teacher ID")]
        public string TeacherId { get; set; }


        [DisplayName("Teacher Type")]
        public TeacherType TeacherType { get; set; }

         [DisplayName("Teacher Type")]
        public string TeacherTypeOther { get; set; }

        [DisplayName("Coaching Model")]
        public AssignmentType CoachAssignmentWay { get; set; }

        [DisplayName("Coaching Model")]
        public string CoachAssignmentWayOther { get; set; }

        public int YearsInProjectId { get; set; }

        [DisplayName("Years in Project")]
        public string YearsInProject { get; set; }

        [DisplayName("Teacher Status")]
        public EntityStatus TeacherStatus { get; set; }

        [DisplayName("Inactive Date")]
        public DateTime InactiveDate { get; set; }


        [DisplayName("Creation Date")]
        public DateTime CreationDate { get; set; }

        //Teacher Email (Teacher Primary email form Teacher page)
        [DisplayName("Teacher Email")]
        public string PrimaryEmailAddress { get; set; }

        //Admin Email (Primary Contact email from School page) 
        [DisplayName("Admin Email")]
        public string PrimaryEmail { get; set; }
    }
}
