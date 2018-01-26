using Sunnet.Cli.Core.MasterData.Entities;
using Sunnet.Cli.Core.MasterData.Interfaces;
using Sunnet.Framework.Core.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sunnet.Cli.Impl.MasterData
{
    class CountryRpst : EFRepositoryBase<CountryEntity, Int32>, ICountryRpst
    {
        public CountryRpst(IUnitOfWork unit)
        {
            UnitOfWork = unit;
        }
    }
}
