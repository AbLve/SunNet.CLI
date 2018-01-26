using Sunnet.Framework.Core.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Sunnet.Cli.Core.MasterData.Entities;
using Sunnet.Cli.Core.MasterData.Interfaces;

namespace Sunnet.Cli.Impl.MasterData
{
    public class LanguageRpst : EFRepositoryBase<LanguageEntity, Int32>, ILanguageRpst
    {
        public LanguageRpst(IUnitOfWork unit)
        {
            UnitOfWork = unit;
        }
    }
}
