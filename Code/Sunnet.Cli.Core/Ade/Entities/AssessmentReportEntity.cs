using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Sunnet.Cli.Core.Common.Enums;
using Sunnet.Framework.Core.Base;
using Sunnet.Framework.Mvc;

namespace Sunnet.Cli.Core.Ade.Entities
{
    public class AssessmentReportEntity : EntityBase<int>
    {
        public int AssessmentId { get; set; }

        public ReportTypeEnum ReportType { get; set; }

        public ReportEnum Report { get; set; }

        public virtual AssessmentEntity Assessment { get; set; }
    }
}
