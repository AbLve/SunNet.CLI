using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/**************************************************************************
 * Developer: 		JackZhang
 * Computer:		JACKZ
 * Domain:			Jackz
 * CreatedOn:		2014/8/12 22:47:10
 * Description:		Please input class summary
 * Version History:	Created,2014/8/12 22:47:10
 * 
 * 
 **************************************************************************/
using Sunnet.Cli.Core.Ade;
using Sunnet.Cli.Core.Ade.Entities;
using Sunnet.Cli.Core.Common.Enums;
using Sunnet.Framework.Mvc;

namespace Sunnet.Cli.Business.Ade.Models
{
    public class AssessmentModel
    {
        private List<CutOffScoreEntity> _cutOffScores;
        private string _createdByName;

        public AssessmentModel()
        {
            Status = EntityStatus.Inactive;
            OrderType = OrderType.Sequenced;
        }
        public int ID { get; set; }

        [DisplayName("Display Name")]
        [StringLength(100)]
        [Required]
        public string Name { get; set; }

        [DisplayName("Assessment Label")]
        [StringLength(100)]
        [Required]
        public string Label { get; set; }

        [DisplayName("Administration Order")]
        public OrderType OrderType { get; set; }

        [DisplayName("Total Scored")]
        public bool TotalScored { get; set; }

        [DisplayName("Template")]
        public AssessmentType Type { get; set; }

        [Required]
        public EntityStatus Status { get; set; }
        public AssessmentLanguage Language { get; set; }

        public bool Locked { get; set; }
        /// <summary>
        /// 添加时临时存储添加的值
        /// </summary>
        [DisplayName("Cutoff Scores")]
        public List<CutOffScoreEntity> CutOffScores
        {
            get { return _cutOffScores ?? (_cutOffScores = new List<CutOffScoreEntity>()); }
            set { _cutOffScores = value; }
        }

        [DisplayName("Created By")]
        public int CreatedBy { get; set; }

        [DisplayName("Created By")]
        public string CreatedByName
        {
            get { return _createdByName ?? (_createdByName = ""); }
            set { _createdByName = value; }
        }

        [DisplayName("Updated By")]
        public int UpdatedBy { get; set; }
        [DisplayName("Created")]
        public DateTime CreatedOn { get; set; }
        [DisplayName("Updated")]
        public DateTime UpdatedOn { get; set; }

        public int MeasureCount { get; set; }
        [EensureEmptyIfNull]
        [DisplayName("Description")]
        public string Description { get; set; }

        private List<BenchmarkEntity> _benchmarks;

        public List<BenchmarkEntity> Benchmarks
        {
            get { return _benchmarks ?? (_benchmarks = new List<BenchmarkEntity>()); }
            set { _benchmarks = value; }
        }

        [DisplayName("Parent Report Cover Sheet")]
        public string ParentReportCoverPath { get; set; }

        [DisplayName("Parent Report Cover Sheet")]
        public string ParentReportCoverName { get; set; }

        [DisplayName("Display Percentile Rank Toggle")]
        public bool DisplayPercentileRank { get; set; }
    }
}
