using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


/**************************************************************************
 * Developer: 		JackZhang
 * Computer:		JACKZ
 * Domain:			Jackz
 * CreatedOn:		2014/10/9 2:50:19
 * Description:		报表模块的Measure 头信息
 * Version History:	Created,2014/10/9 2:50:19
 * 
 * 
 **************************************************************************/

namespace Sunnet.Cli.Core.Cpalls.Models.Report
{
    public class ReportMeasureHeaderModel
    {
        /// <summary>
        /// Measure ID
        /// </summary>
        public int ID { get; set; }

        public int ParentId { get; set; }
        public string ParentName { get; set; }
        public bool ParentScored { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string ApplyToWave { get; set; }
        public bool TotalScored { get; set; }

        public decimal TotalScore { get; set; }
        /// <summary>
        /// 包含的子Measure数量
        /// </summary>
        public int Count { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this measure is last measure of parent's children.
        /// </summary>
        public bool IsLastOfChildren { get; set; }

        public bool LightColor { get; set; }

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
    }
}
