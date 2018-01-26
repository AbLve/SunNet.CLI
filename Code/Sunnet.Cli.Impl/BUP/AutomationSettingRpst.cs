using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Sunnet.Framework.Core.Base;
using Sunnet.Cli.Core.BUP.Entities;
using Sunnet.Cli.Core.BUP.Interfaces;

namespace Sunnet.Cli.Impl.BUP
{
    public class AutomationSettingRpst : EFRepositoryBase<AutomationSettingEntity, Int32>, IAutomationSettingRpst
    {
        public AutomationSettingRpst(IUnitOfWork unit)
        {
            UnitOfWork = unit;
        }
    }
}
