/**************************************************************************
 * Developer: 		JackZhang
 * Computer:		JACKZ
 * Domain:			JackZ
 * CreatedOn:		1/30 2015 10:55:29
 * Description:		Please input class summary
 * Version History:	Created,1/30 2015 10:55:29
 * 
 * 
 **************************************************************************/
using Sunnet.Cli.Business.Common;
using Sunnet.Cli.Core.Users.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Sunnet.Cli.Business.Cot.Report
{
    /// <summary>
    /// 请勿随意修改此枚举的值，需要根据值判断Teacher范围
    /// </summary>
    public enum ObservedReportType
    {
        [Description("Teacher Observed Strategies Report")]
        SingleTeacher = 1,

        [Description("Teacher Observed Strategies Report")]
        AssignedTeachers = 10,

        [Description("Assigned Teachers Average Report")]
        AssignedTeachersAverage = 15,

        [Description("School Average Report")]
        SchoolAverage = 20,

        [Description("School Average Report")]
        AllSchoolAverage = 21,

        [Description("Community/District Average Report")]
        DistrictAverage = 30
    }
    public class CotReportModel
    {
        public ObservedReportType Type { get; set; }

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

        public List<CotReportMeasureModel> Measures { get; set; }


        private List<AssignmentType> _coachModels;
        private List<AssignmentType> _eCircles;
        private List<string> _yearsInProject;

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

    }
}
