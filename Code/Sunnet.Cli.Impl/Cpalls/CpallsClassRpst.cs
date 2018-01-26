using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Sunnet.Cli.Core.Cpalls.Interfaces;
using Sunnet.Framework.Core.Base;
using Sunnet.Cli.Core.Cpalls.Entities;

namespace Sunnet.Cli.Impl.Cpalls
{
    public class CpallsClassRpst : EFRepositoryBase<CpallsClassEntity, int>, ICpallsClassRpst
    {
        public CpallsClassRpst(IUnitOfWork unit)
        {
            this.UnitOfWork = unit;
        }
    }
}
