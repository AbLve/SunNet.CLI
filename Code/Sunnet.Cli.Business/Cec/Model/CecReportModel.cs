using Sunnet.Cli.Business.Ade.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sunnet.Cli.Business.Cpalls.Models;

namespace Sunnet.Cli.Business.Cec.Model
{
    public class CecReportModel
    {
        public int AssessmentId { get; set; }

        public DateTime AssessmentDate { get; set; }

        public List<CecItemModel> Items { get; set; }

        public List<MeasureHeaderModel> MeasureList { get; set; }

        public List<MeasureHeaderModel> ParentMeasureList { get; set; }
    }
}
