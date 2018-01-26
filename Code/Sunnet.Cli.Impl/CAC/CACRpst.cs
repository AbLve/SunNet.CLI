using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sunnet.Cli.Core.CAC.Entities;
using Sunnet.Cli.Core.CAC.Interfaces;
/**************************************************************************
 * Developer: 		Sam
 * Computer:		SAM-PC
 * Domain:			sam-pc
 * CreatedOn:		2014/8/19 12:15:10
 * Description:		Please input class summary
 * Version History:	Created,2014/8/19 12:15:10
 * 
 * 
 **************************************************************************/
using Sunnet.Cli.Core.Schools.Entities;
using Sunnet.Cli.Core.Schools.Interfaces;
using Sunnet.Framework.Core.Base;

namespace Sunnet.Cli.Impl.CAC
{
    public class MyActivityRpst : EFRepositoryBase<MyActivityEntity, Int32>, IMyActivityRpst
    {
        public MyActivityRpst(IUnitOfWork unit)
        {
            this.UnitOfWork = unit;
        }
    }
    public class ActivityHistoryRpst : EFRepositoryBase<ActivityHistoryEntity, Int32>, IActivityHistoryRpst
    {
        public ActivityHistoryRpst(IUnitOfWork unit)
        {
            this.UnitOfWork = unit;
        }
    }
}
