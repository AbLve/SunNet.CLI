using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sunnet.Cli.Business.Trs.Models
{
    public class TrsAssessmentClassModel
    {
        public int ID { get; set; }

        public int AssessmentId { get; set; }

        public int ClassId { get; set; }

        public decimal ObservationLength { get; set; }
    }
}
