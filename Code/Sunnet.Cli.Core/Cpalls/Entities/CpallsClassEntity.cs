using Sunnet.Framework.Core.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sunnet.Cli.Core.Cpalls.Entities
{
    /// <summary>
    /// 历史数据归档
    /// </summary>
   public  class CpallsClassEntity : EntityBase<int>
    {
        public int CommunityId { get; set; }

        public int SchoolId { get; set; }

        public int ClassId { get; set; }

        public int AssessmentId { get; set; }

        /// <summary>
        /// 根据 学年的分隔月（一般是8月）来保存年 ; 如 2014年6月，则保存2013 ; 如 2014年9月，则保存2014
        /// </summary>
        public string SchoolYear { get; set; }

        public Wave Wave { get; set; }
        
        public virtual ICollection<CpallsClassMeasureEntity> CpallsClassMeasures { get; set; }
    }
}
