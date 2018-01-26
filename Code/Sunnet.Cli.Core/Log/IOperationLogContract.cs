using Sunnet.Cli.Core.Log.Entities;
using Sunnet.Framework.Core.Tool;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sunnet.Cli.Core.Log
{
   public interface IOperationLogContract
    {
       OperationResult AddOperationLog(OperationLogEntity entity);
    }
}
