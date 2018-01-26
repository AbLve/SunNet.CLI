using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/**************************************************************************
 * Developer: 		Leason
 * Computer:		Leason-PC
 * Domain:			Leason-pc
 * CreatedOn:		2014/8/27 16:16:16
 * Description:		Create MonitoringToolRpst
 * Version History:	Created,2014/8/27 16:16:16
 * 
 * 
 **************************************************************************/
using Sunnet.Cli.Core.Classes.Entites;
using Sunnet.Cli.Core.Classes.Interfaces;
using Sunnet.Framework.Core.Base;

namespace Sunnet.Cli.Impl.Classes
{
    public class MonitoringToolRpst : EFRepositoryBase<MonitoringToolEntity, Int32>, IMonitoringToolRpst
    {
        public MonitoringToolRpst(IUnitOfWork unit)
        {
            UnitOfWork = unit;
        }
    }
}
