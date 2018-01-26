using Sunnet.Cli.Core.Log.Entities;
using Sunnet.Cli.Core.Log.Interfaces;
using Sunnet.Framework.Core.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sunnet.Cli.Impl.Log
{
    public class OperationLogRpst : EFRepositoryBase<OperationLogEntity, Int64>, IOperationLogRpst
    {
        public OperationLogRpst(IUnitOfWork unit)
        {
            UnitOfWork = unit;
        }
    }
}
