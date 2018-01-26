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
    public class CountriesRpst : EFRepositoryBase<CountriesEntity, Int32>, ICountriesRpst
    {
        public CountriesRpst(IUnitOfWork unit)
        {
            UnitOfWork = unit;
        }
    }
}
