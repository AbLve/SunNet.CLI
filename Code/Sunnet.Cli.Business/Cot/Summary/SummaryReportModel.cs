using Sunnet.Cli.Core.Users.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Sunnet.Cli.Business.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sunnet.Cli.Business.Cot.Summary
{
    public enum SummaryReportType
    {
        [Description("Teacher Summary Report")]
        SingleTeacher = 1,

        [Description("Teacher Summary Report")]
        AssignedTeachers = 10,

        [Description("Teacher Summary Report")]
        [Display(Name = "Assigned Teachers Average")]
        AssignedTeachersAverage = 15,

        [Description("Teacher Summary Report")]
        [Display(Name = "School Average")]
        SchoolAverage = 20,

        [Description("Teacher Summary Report")]
        [Display(Name = "School Average")]
        AllAssignedSchoolAverage = 21,

        [Description("Teacher Summary Report")]
        [Display(Name = "Community/District Average")]
        DistrictAverage = 30
    }
    public class SummaryReportModel
    {
        private List<AssignmentType> _coachModels;
        private List<AssignmentType> _eCircles;
        private List<string> _yearsInProject;
        public SummaryReportType Type { get; set; }

        [DisplayName("Community/District")]
        public string Community { get; set; }

        [DisplayName("Teacher")]
        public string Teacher { get; set; }

        [DisplayName("Observer")]
        public string Mentor { get; set; }

        [DisplayName("School")]
        public string School { get; set; }

        /// <summary>
        /// 年份
        /// </summary>
        [DisplayName("School Year")]
        public int SchoolYear { get; set; }

        [DisplayName("School Year")]
        public string FullSchoolYear
        {
            get { return SchoolYear.ToFullSchoolYearString(); }
        }

        [DisplayName("First BOY Date")]
        public DateTime BoyDate { get; set; }

        [DisplayName("Last MOY Date")]
        public DateTime MoyDate { get; set; }

        [DisplayName("Last Met Date")]
        public DateTime MetDate { get; set; }


        [DisplayName("Years In Project")]
        public List<string> YearsInProject
        {
            get { return _yearsInProject ?? (_yearsInProject = new List<string>()); }
            set { _yearsInProject = value; }
        }

        [DisplayName("Coaching Model")]
        public List<AssignmentType> CoachModels
        {
            get { return _coachModels ?? (_coachModels = new List<AssignmentType>()); }
            set { _coachModels = value; }
        }

        [DisplayName("eCircle Assignment")]
        public List<AssignmentType> ECircles
        {
            get { return _eCircles ?? (_eCircles = new List<AssignmentType>()); }
            set { _eCircles = value; }
        }

        public int Teachers { get; set; }

        public IEnumerable<SummaryMeasureModel> Measures { get; set; }

    }
}
