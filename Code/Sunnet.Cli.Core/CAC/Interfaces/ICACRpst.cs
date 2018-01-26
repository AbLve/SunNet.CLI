using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sunnet.Cli.Core.CAC.Entities;
/**************************************************************************
 * Developer: 		Leason
 * Computer:		Leason-PC
 * Domain:			Leason-pc
 * CreatedOn:		2014/9/10 16:21:20
 * Description:		Create IClassRoleRpst
 * Version History:	Created,2014/9/10 16:21:20
 * 
 * 
 **************************************************************************/
using Sunnet.Cli.Core.Classes.Entites;
using Sunnet.Framework.Core.Base;

namespace Sunnet.Cli.Core.CAC.Interfaces
{
    public interface IMyActivityRpst : IRepository<MyActivityEntity, int>
    {

    }
    public interface IActivityHistoryRpst : IRepository<ActivityHistoryEntity, int>
    {

    }
}
