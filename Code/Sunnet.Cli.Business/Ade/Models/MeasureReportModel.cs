using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sunnet.Cli.Core.Cpalls;

namespace Sunnet.Cli.Business.Ade.Models
{
    public class MeasureReportModel
    {
        public int ID { get; set; }

        public string Name { get; set; }

        public string ShortName { get; set; }

        public int ParentId { get; set; }

        /// <summary>
        /// Related MeasureId
        /// </summary>
        public int RelatedMeasureId { get; set; }

        public int Sort { get; set; }

        public string ApplyToWave { get; set; }
    }
}
