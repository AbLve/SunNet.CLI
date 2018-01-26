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
   public  class UserShownMeasuresEntity : EntityBase<int>
    {
       public int UserId { get; set; }

       public int AssessmentId { get; set; }

       public Wave Wave { get; set; }

       public int Year { get; set; }

        public string Measures { get; set; }
    }
}
