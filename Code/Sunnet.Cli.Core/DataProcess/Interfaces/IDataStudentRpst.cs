using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Sunnet.Cli.Core.DataProcess.Entities;
using Sunnet.Framework.Core.Base;

namespace Sunnet.Cli.Core.DataProcess.Interfaces
{
    public  interface IDataStudentRpst : IRepository<DataStudentEntity, Int64>
    {
    }
}
