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
    public  class TRSItemAnswerRpst: EFRepositoryBase<TRSItemAnswerEntity, int>, ITRSItemAnswerRpst
    {
        public TRSItemAnswerRpst(IUnitOfWork unit)
        {
            UnitOfWork = unit;
        }
   }
}