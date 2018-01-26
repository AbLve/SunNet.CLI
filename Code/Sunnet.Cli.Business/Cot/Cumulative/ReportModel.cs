using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/**************************************************************************
 * Developer: 		JackZhang
 * Computer:		JACKZ
 * Domain:			JackZ
 * CreatedOn:		2/7 2015 13:59:00
 * Description:		Please input class summary
 * Version History:	Created,2/7 2015 13:59:00
 * 
 * 
 **************************************************************************/
using LinqKit;
using Sunnet.Cli.Business.Common;
using Sunnet.Cli.Core.Users.Enums;

namespace Sunnet.Cli.Business.Cot.Cumulative
{
    /// <summary>
    /// 请勿随意修改此枚举的值，需要根据值判断Teacher范围
    /// </summary>
    public enum CumulativeReportType
    {
        [Description("Teacher Observed and Met Strategies")]
        [Display(Name = "Teacher Item Report")]
        SingleTeacher = 1,

        [Description("Teacher Observed and Met Strategies")]
        [Display(Name = "Teacher Item Report")]
        AssignedTeachers = 10,

        [Description("Teacher Observed and Gained Strategies")]
        [Display(Name = "Assigned Teachers Item Report")]
        AssignedTeachersCumulative = 15,


        [Description("Teacher Observed and Gained Strategies")]
        [Display(Name = "School Teachers Item Report")]
        SchoolCumulative = 20,

        [Description("Teacher Observed and Gained Strategies")]
        [Display(Name = "School Teachers Item Report")]
        AllAssignedSchoolCumulative = 21,

        [Description("Teacher Observed and Gained Strategies")]
        [Display(Name = "Community/District Teachers Item Report")]
        DistrictCumulative = 30
    }

    public class ReportModel
    {
        public CumulativeReportType Type { get; set; }

        [DisplayName("Community/District")]
        public string Community { get; set; }

        private string _teacher;

        [DisplayName("Teacher")]
        public string Teacher
        {
            get
            {
                switch (Type)
                {
                    case CumulativeReportType.SingleTeacher:
                    case CumulativeReportType.AssignedTeachers:
                        return _teacher;
                    case CumulativeReportType.AssignedTeachersCumulative:
                        return "Assigned Teachers";
                    default:
                        return "All";
                }
                return _teacher;
            }
            set { _teacher = value; }
        }

        [DisplayName("Observer")]
        public string Mentor { get; set; }

        private string _school;

        [DisplayName("School")]
        public string School
        {
            get
            {
                return _school;
            }
            set { _school = value; }
        }

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

        public IEnumerable<MeasureModel> Measures { get; set; }

        public int ItemCount
        {
            get
            {
                var count = 0;
                if (Measures != null)
                    Measures.ForEach(x => count += x.Items != null && x.Items.Any() ? x.Items.Count() : 0);
                return count;
            }
        }
    }
}
