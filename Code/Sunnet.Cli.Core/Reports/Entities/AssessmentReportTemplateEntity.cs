using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sunnet.Cli.Core.Common.Enums;
using Sunnet.Framework.Core.Base;

namespace Sunnet.Cli.Core.Reports.Entities
{
    public class AssessmentReportTemplateEntity : EntityBase<int>
    {
        public string Name { get; set; }

        public int AssessmentId { get; set; }

        public EntityStatus Status { get; set; }

        public int CreatedBy { get; set; }

        public int UpdatedBy { get; set; }

        public string Ids { get; set; }
    }
}
