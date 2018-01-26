using System.ComponentModel.DataAnnotations;
using Sunnet.Cli.Core.Cpalls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sunnet.Cli.Business.Cpalls.Models;
/**************************************************************************
 * Developer: 		JackZhang
 * Computer:		JACKZ
 * Domain:			Jackz
 * CreatedOn:		-
 * Description:		Please input class summary
 * Version History:	Created,-
 * Change:          Delete old class 2014-11-20
 * Create:          new Class
 * 
 **************************************************************************/
using Sunnet.Cli.Core.Cpalls.Entities;
using Sunnet.Framework.Extensions;

namespace Sunnet.Cli.Business.Practices.Models
{
    public class PracticeStudentMeasureModel
    {
        private decimal? _goal;
        public int StudentId { get; set; }

        #region StudentMeasureEntity Member
        public int ID { get; set; }
        /// <summary>
        ///  StudentAssessment Id 
        /// </summary>
        public int SAId { get; set; }
        public int MeasureId { get; set; }

        public CpallsStatus Status { get; set; }

        public int PauseTime { get; set; }

        /// <summary>
        /// 及格线, 从ADE读取 
        /// </summary>
        public decimal Benchmark { get; set; }

        /// <summary>
        /// Gets or sets the age group (Only for offline now).
        /// </summary>
        public string AgeGroup { get; set; }

        /// <summary>
        /// Measure 总分, 从ADE读取
        /// </summary>
        public decimal? TotalScore { get; set; }
        /// <summary>
        /// 是否打分, 从ADE读取
        /// </summary>
        public bool TotalScored { get; set; }

        /// <summary>
        /// 最后得分总计
        /// </summary>
        public decimal? Goal
        {
            get { return _goal.HasValue && _goal.Value >= 0 ? _goal.Value : (decimal?)null; }
            set { _goal = value; }
        }

        public DateTime CreatedOn { get; set; }

        public DateTime UpdatedOn { get; set; }

        [Required]
        [StringLength(150)]
        public string Comment { get; set; }

        #endregion

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

        public int Age { get; set; }
        public string Color
        {
            get
            {
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

        public Wave Wave { get; set; }

        //Measure是否有及格线
        public bool HasCutOffScores
        {
            get
            {
                if (Wave == Wave.BOY)
                    return BOYHasCutOffScores;
                else if (Wave == Wave.MOY)
                    return MOYHasCutOffScores;
                else if (Wave == Wave.EOY)
                    return EOYHasCutOffScores;
                else
                    return false;
            }
        }

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

        public bool LightColor { get; set; }

        public IEnumerable<PracticeStudentItemModel> Items { get; set; }

        public int BenchamrkId { get; set; }

        public decimal LowerScore { get; set; }

        public decimal HigherScore { get; set; }

        public string BenchmarkColor { get; set; }
    }
}
