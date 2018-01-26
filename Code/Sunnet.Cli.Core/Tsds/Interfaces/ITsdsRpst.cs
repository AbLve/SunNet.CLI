using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sunnet.Framework.Core.Base;
using Sunnet.Cli.Core.Tsds.Entities;

namespace Sunnet.Cli.Core.Tsds.Interfaces
{
    public interface ITsdsRpst : IRepository<TsdsEntity, Int32>
    {
    }
}
