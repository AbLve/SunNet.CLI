using Sunnet.Cli.Core.Ade.Entities;
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
    public class CpallsSchoolMeasureEntity:EntityBase<int>
    {
        public int CpallsSchoolId { get; set; }

        public int MeasureId { get; set; }

        public decimal TotalScore { get; set; }
        public int Count { get; set; }
        public virtual CpallsSchoolEntity CpallsSchool { get; set; }

        public virtual MeasureEntity Measure { get; set; }
    }
}
