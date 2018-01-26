using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Sunnet.Framework.Core.Base;
using Sunnet.Cli.Core.Export.Entities;
using Sunnet.Cli.Core.Export.Interfaces;

namespace Sunnet.Cli.Impl.Export
{
    public class ReportTemplateRpst : EFRepositoryBase<ReportTemplateEntity, Int32>, IReportTemplateRpst
    {
        public ReportTemplateRpst(IUnitOfWork unit)
        {
            UnitOfWork = unit;
        }
    }
}
