using Sunnet.Cli.Business.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LinqKit;
using Newtonsoft.Json;
using Sunnet.Cli.Business.Cpalls.Models;
using Sunnet.Cli.Business.Schools.Models;
using Sunnet.Cli.Core.Ade;
using Sunnet.Cli.Core.Common.Enums;
using Sunnet.Cli.Core.Cpalls;
using Sunnet.Framework.Extensions;

namespace Sunnet.Cli.Business.Practices.Models
{
    public class DemoStudentModel
    {
        public DemoStudentModel()
        {
            CachedOn = DateTime.Now;
        }
        private List<DemoStudentMeasureRecordModel> _measureList;
        private int? _schoolYear;
      
        public int ID { get; set; }
        public int StudentId { get; set; }
        public string StudentName { get; set; }
        public DateTime StudentDob { get; set; }
        public int StudentAgeYear { get; set; }
        public int StudentAgeMonth { get; set; }
        public StudentAssessmentLanguage AssessmentLanguage { get; set; }
        public string DataFrom { get; set; }
        public string Remark { get; set; }
        public int AssessmentId { get; set; }
        public EntityStatus Status { get; set; }
        public int StudentAssessmentId { get; set; }
        public int SchoolYear
        {
            get
            {
                if (_schoolYear == null)
                    _schoolYear = CommonAgent.Year;
                return _schoolYear.Value;
            }
            set { _schoolYear = value; }
        }

        [JsonIgnore]
        public List<DemoStudentMeasureRecordModel> MeasureList
        {
            get { return _measureList ?? (_measureList = new List<DemoStudentMeasureRecordModel>()); }
            set { _measureList = value; }
        }

        public Dictionary<int, DemoStudentMeasureRecordModel> DicMeasure
        {
            get
            {
                if (MeasureList == null || MeasureList.Count() == 0) return null;
                Dictionary<int, DemoStudentMeasureRecordModel> dic = new Dictionary<int, DemoStudentMeasureRecordModel>();
                foreach (DemoStudentMeasureRecordModel item in MeasureList)
                {
                    if (item == null)
                        continue;
                    if (dic.ContainsKey(item.MeasureId))
                        dic[item.MeasureId] = item;
                    else
                        dic.Add(item.MeasureId, item);
                }
                return dic;
            }
        }

        public DateTime CachedOn { get; set; }
    }

    public class DemoStudentMeasureRecordModel
    {
        public int ID { get; set; }

        public int StudentAssessmentId { get; set; }

        public int StudentId { get; set; }

        public int MeasureId { get; set; }

        public string MeasureName { get; set; }

        public CpallsStatus Status { get; set; }

        public string SchoolYear { get; set; }

        public Wave Wave { get; set; }

        /// <summary>
        /// 专为Total服务务
        /// </summary>
        public decimal? GoalForTotal
        {
            get { return TotalScored ? Goal : null; }
        }

        /// <summary>
        /// 及格级 
        /// </summary>
        public decimal Benchmark { get; set; }

        public decimal LowerScore { get; set; }

        public decimal HigherScore { get; set; }

        public int BenchmarkId { get; set; }

        public string BenchmarkColor { get; set; }

        public bool ShowOnGroup { get; set; }

        /// <summary>
        /// 最后得分
        /// </summary>
        public decimal? Goal { get; set; }

        public bool IsTotal { get; set; }

        public string ShowText
        {
            get
            {
                if (Goal == null || Goal.Value < 0)
                    return "-";
                return Goal.Value.ToPrecisionString(2);
            }
        }

        /// <summary>
        /// 岁数
        /// </summary>
        public double Age { get; set; }

        /// <summary>
        /// 产生assessment 的时间，用来计划当时的岁数
        /// </summary>
        public DateTime UpdatedDate { get; set; }

        public bool TotalScored { get; set; }

        public string Color
        {
            get
            {
                if (Wave == Wave.BOY)
                    HasCutOffScores = BOYHasCutOffScores;
                else if (Wave == Wave.MOY)
                    HasCutOffScores = MOYHasCutOffScores;
                else if (Wave == Wave.EOY)
                    HasCutOffScores = EOYHasCutOffScores;

                if (!HasCutOffScores)
                    return ReportTheme.Missing_Bentchmark_ClassName;
                if (Benchmark < 0)
                    return LightColor ? ReportTheme.TE3_Light_ClassName : ReportTheme.TE3_ClassName;
                if (Goal < Benchmark)
                {
                    if (Age >= 4)
                        return LightColor ? ReportTheme.GE4_Light_ClassName : ReportTheme.GE4_ClassName;
                    else
                        return LightColor ? ReportTheme.TE4_Light_ClassName : ReportTheme.TE4_ClassName;
                }
                return LightColor ? ReportTheme.Passed_Light_ClassName : ReportTheme.Passed_ClassName;
            }
        }

        public bool LightColor { get; set; }

        //Measure是否有及格线
        public bool HasCutOffScores { get; set; }

        /// <summary>
        /// 表示wave1下这个Measure是否有Score
        /// </summary>
        public bool BOYHasCutOffScores { get; set; }

        /// <summary>
        /// 表示wave2下这个Measure是否有Score
        /// </summary>
        public bool MOYHasCutOffScores { get; set; }

        /// <summary>
        /// 表示wave3下这个Measure是否有Score
        /// </summary>
        public bool EOYHasCutOffScores { get; set; }

        public bool GroupByParentMeasure { get; set; }

        public string PercentileRank { get; set; }
        public string DataFrom { get; set; }
    }
}
