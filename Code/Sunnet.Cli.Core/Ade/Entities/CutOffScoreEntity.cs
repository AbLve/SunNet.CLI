using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Sunnet.Cli.Core.Cpalls;
using Sunnet.Framework.Core.Base;

/**************************************************************************
 * Developer: 		JackZhang
 * Computer:		JACKZ
 * Domain:			Jackz
 * CreatedOn:		2014/8/11 1:00:41
 * Description:		Please input class summary
 * Version History:	Created,2014/8/11 1:00:41
 * 
 * 
 **************************************************************************/

namespace Sunnet.Cli.Core.Ade.Entities
{
    /// <summary>
    /// 有CutOffScore的实体
    /// </summary>
    public interface ICutOffScoreProperties
    {
    }

    public class CutOffScoreEntity : EntityBase<int>
    {
        /// <summary>
        /// 宿主类型.
        /// </summary>
        [StringLength(100)]
        [Required]
        public string HostType { get; set; }
        /// <summary>
        /// 宿主ID.
        /// </summary>
        public int HostId { get; set; }

        public int FromYear { get; set; }
        public int FromMonth { get; set; }
        public int ToYear { get; set; }
        public int ToMonth { get; set; }

        public decimal CutOffScore { get; set; }

        public Wave Wave { get; set; }

        public int BenchmarkId { get; set; }

        public decimal LowerScore { get; set; }

        public decimal HigherScore { get; set; }

        public bool ShowOnGroup { get; set; }

        public virtual BenchmarkEntity Benchmark { get; set; }
    }

}
