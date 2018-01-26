using Sunnet.Cli.Core.Log.Entities;
using Sunnet.Framework.Core.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sunnet.Cli.Core.Log.Interfaces
{
    public interface IOperationLogRpst : IRepository<OperationLogEntity,Int64>
    {
    }
}
