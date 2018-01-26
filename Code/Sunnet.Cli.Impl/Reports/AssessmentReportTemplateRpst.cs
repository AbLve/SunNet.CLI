using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sunnet.Cli.Core.Reports.Entities;
using Sunnet.Cli.Core.Reports.Interfaces;
using Sunnet.Framework.Core.Base;

namespace Sunnet.Cli.Impl.Reports
{
    public class AssessmentReportTemplateRpst
        : EFRepositoryBase<AssessmentReportTemplateEntity, int>, IAssessmentReportTemplateRpst
    {
        public AssessmentReportTemplateRpst(IUnitOfWork unit)
        {
            this.UnitOfWork = unit;
        }
    }
}
