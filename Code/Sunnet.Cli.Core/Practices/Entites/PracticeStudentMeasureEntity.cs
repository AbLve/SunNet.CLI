using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sunnet.Cli.Core.Ade.Entities;
using Sunnet.Framework.Core.Base;
using Sunnet.Cli.Core.Students.Entities;
using Sunnet.Cli.Core.Cpalls;

/**************************************************************************
 * Developer: 		JackZhang
 * Computer:		JACKZ
 * Domain:			Jackz
 * CreatedOn:		2014/9/4 4:22:23
 * Description:		Please input class summary
 * Version History:	Created,2014/9/4 4:22:23
 * 
 * 
 **************************************************************************/


namespace Sunnet.Cli.Core.Practices.Entities
{
    public class PracticeStudentMeasureEntity : EntityBase<int>
    {
        private decimal _goal;
        private string _comment;
        private string _dataFrom;
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
        /// Measure 总分, 从ADE读取
        /// </summary>
        public decimal TotalScore { get; set; }
        /// <summary>
        /// 是否打分, 从ADE读取
        /// </summary>
        public bool TotalScored { get; set; }

        /// <summary>
        /// 显示百分比等级,由 Subject age和Raw score一起通过匹配PercentileRankLookup表共同计算产生
        /// Subject Age=Number of days in between student birthdate and date of measure(mesaure updatedOn)
        /// </summary>
        public string PercentileRank { get; set; }

        /// <summary>
        /// 最后得分总计
        /// </summary>
        public decimal Goal
        {
            get
            {
                if (_goal < 0)
                    return -1;
                return _goal;
            }
            set { _goal = value; }
        }

        public string Comment
        {
            get { return _comment ?? (_comment = string.Empty); }
            set { _comment = value; }
        }

        /// <summary>
        /// 学生生日改变引起Benchmark的改变，0没变,1变了
        /// </summary>
        public bool BenchmarkChanged { get; set; }
        public int BenchmarkId { get; set; }

        public decimal LowerScore { get; set; }

        public decimal HigherScore { get; set; }

        public bool ShowOnGroup { get; set; }

        public string DataFrom
        {
            get { return _dataFrom ?? (_dataFrom = string.Empty); }
            set { _dataFrom = value; }
        }

        public virtual ICollection<PracticeStudentItemEntity> Items { get; set; }

        public virtual PracticeStudentAssessmentEntity Assessment { get; set; }

        public virtual CliAdeMeasureEntity Measure { get; set; }




    }
}
