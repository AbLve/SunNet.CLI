using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sunnet.Cli.Core.UpdateClusters.Entities;
using Sunnet.Cli.Core.UpdateClusters.Interfaces;
using Sunnet.Framework.Core.Base;

namespace Sunnet.Cli.Impl.UpdateClusters
{
    public class SystemUpdateRpst : EFRepositoryBase<SystemUpdateEntity, Int32>, ISystemUpdateRpst
    {
        public SystemUpdateRpst(IUnitOfWork unit)
        {
            UnitOfWork = unit;
        }
    }
}
