using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sunnet.Cli.Core.Tsds.Entities;
using Sunnet.Cli.Core.Tsds.Interfaces;
using Sunnet.Framework.Core.Base;

namespace Sunnet.Cli.Impl.Tsds
{
    public class TsdsAssessmentFileRpst : EFRepositoryBase<TsdsAssessmentFileEntity, Int32>, ITsdsAssessmentFileRpst
    {
        public TsdsAssessmentFileRpst(IUnitOfWork unit)
        {
            UnitOfWork = unit;
        }
    }
}
