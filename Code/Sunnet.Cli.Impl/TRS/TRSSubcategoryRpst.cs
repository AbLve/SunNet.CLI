using Sunnet.Cli.Core.Trs.Entities;
using Sunnet.Cli.Core.Trs.Interfaces;
using Sunnet.Framework.Core.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sunnet.Cli.Impl.TRS
{
    public class TRSSubcategoryRpst : EFRepositoryBase<TRSSubcategoryEntity, int>, ITRSSubcategoryRpst
    {
        public TRSSubcategoryRpst(IUnitOfWork unit)
        {
            UnitOfWork = unit;
        }
    }
}
