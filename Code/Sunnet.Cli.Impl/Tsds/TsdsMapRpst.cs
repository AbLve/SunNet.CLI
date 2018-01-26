using System;
using Sunnet.Cli.Core.Tsds.Entities;
using Sunnet.Cli.Core.Tsds.Interfaces;
using Sunnet.Framework.Core.Base;

namespace Sunnet.Cli.Impl.Tsds
{
    public class TsdsMapRpst : EFRepositoryBase<TsdsMapEntity, Int32>, ITsdsMapRpst
    {
        public TsdsMapRpst(IUnitOfWork unit)
        {
            UnitOfWork = unit;
        }
    }
}
