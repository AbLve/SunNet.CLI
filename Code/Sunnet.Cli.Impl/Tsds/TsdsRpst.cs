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
    public class TsdsRpst : EFRepositoryBase<TsdsEntity, Int32>, ITsdsRpst
    {
        public TsdsRpst(IUnitOfWork unit)
        {
            UnitOfWork = unit;
        }
    }
}
