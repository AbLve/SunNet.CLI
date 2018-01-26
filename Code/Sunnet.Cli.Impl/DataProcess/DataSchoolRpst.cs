using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Sunnet.Cli.Core.DataProcess.Entities;
using Sunnet.Cli.Core.DataProcess.Interfaces;
using Sunnet.Framework.Core.Base;

namespace Sunnet.Cli.Impl.DataProcess
{
    public class DataSchoolRpst : EFRepositoryBase<DataSchoolEntity, Int32>, IDataSchoolRpst
    {
        public DataSchoolRpst(IUnitOfWork unit)
        {
            UnitOfWork = unit;
        }
    }
}