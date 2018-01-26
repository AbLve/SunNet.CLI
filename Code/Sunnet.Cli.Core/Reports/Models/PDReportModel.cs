using Sunnet.Cli.Core.Students.Enums;
using Sunnet.Framework.Core.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sunnet.Cli.Core.Reports.Models
{
    public class PDReportModel
    {
        public string CommunityDistrict { get; set; }

        public string SchoolName { get; set; }

        public string TeacherFirstName { get; set; }

        public string TeacherLastName { get; set; }

        public string TeacherID { get; set; }

        public string TeacherEmail { get; set; }

        public int CircleCourseId { get; set; }

        public string CircleCourse { get; set; }

        public DateTime? StartDate { get; set; }

        public string Status { get; set; }

        public string TimeSpentInCourse { get; set; }

        public int CountofPosts { get; set; }

        public int CourseViewed { get; set; }
    }

    public class SummaryReportModel
    {
        public string CommunityDistrict { get; set; }
        public string CircleCourse { get; set; }
        public int CountofTeachersComplete { get; set; }
        public int CountofTeachersinProgress { get; set; }
        public int CountofTeachersNotStarted { get; set; }
        public int TotalTeachers { get; set; }
        public int CountofPosts { get; set; }
        public int CountofCourseViewed { get; set; }
    }
}
