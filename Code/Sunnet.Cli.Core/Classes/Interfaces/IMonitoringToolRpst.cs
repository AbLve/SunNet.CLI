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
 * Description:		Create IMonitoringToolRpst
 * Version History:	Created,2014/8/27 16:16:16
 * 
 * 
 **************************************************************************/
using Sunnet.Cli.Core.Classes.Entites;
using Sunnet.Framework.Core.Base;

namespace Sunnet.Cli.Core.Classes.Interfaces
{
    public interface IMonitoringToolRpst:IRepository<MonitoringToolEntity,int>
    {
    }
}
