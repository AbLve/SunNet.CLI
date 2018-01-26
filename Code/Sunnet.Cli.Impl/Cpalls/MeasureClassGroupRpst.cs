using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sunnet.Cli.Core.Cpalls.Entities;
using Sunnet.Cli.Core.Cpalls.Interfaces;
using Sunnet.Framework.Core.Base;

namespace Sunnet.Cli.Impl.Cpalls
{
    public class MeasureClassGroupRpst : EFRepositoryBase<MeasureClassGroupEntity, int>, IMeasureClassGroupRpst
    {
        public MeasureClassGroupRpst(IUnitOfWork unit)
        {
            this.UnitOfWork = unit;
        }
    }
}
