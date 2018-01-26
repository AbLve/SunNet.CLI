using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sunnet.Cli.Core.Trs.Entities;
using Sunnet.Cli.Core.Trs.Interfaces;
using Sunnet.Framework.Core.Base;

namespace Sunnet.Cli.Impl.TRS
{
    public class TRSNotificationRpst : EFRepositoryBase<TRSNotificationEntity, int>, ITRSNotificationRpst
    {
        public TRSNotificationRpst(IUnitOfWork unit)
        {
            UnitOfWork = unit;
        }
    }
}
