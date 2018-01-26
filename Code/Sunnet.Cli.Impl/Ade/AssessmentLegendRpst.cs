using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sunnet.Cli.Core;
using Sunnet.Cli.Core.Ade.Entities;
using Sunnet.Cli.Core.Ade.Interfaces;
using Sunnet.Framework.Core.Base;
namespace Sunnet.Cli.Impl.Ade
{
    public class AssessmentLegendRpst : EFRepositoryBase<AssessmentLegendEntity, int>, IAssessmentLegendRpst
    {
        public AssessmentLegendRpst(IUnitOfWork unit)
        {
            this.UnitOfWork = unit;
        }
    }
}
