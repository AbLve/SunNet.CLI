using Sunnet.Cli.Core.Cot.Entities;
using Sunnet.Cli.Core.Cot.Interfaces;
using Sunnet.Framework.Core.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sunnet.Cli.Impl.Cot
{
    public class CotStgGroupRpst : EFRepositoryBase<CotStgGroupEntity, int>, ICotStgGroupRpst
    {
        public CotStgGroupRpst(IUnitOfWork unit)
        {
            this.UnitOfWork = unit;
        }
    }
}
