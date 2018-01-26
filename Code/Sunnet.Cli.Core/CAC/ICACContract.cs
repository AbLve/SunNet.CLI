using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sunnet.Cli.Core.CAC.Entities;
using Sunnet.Framework.Core.Tool;

namespace Sunnet.Cli.Core.CAC
{
   public  interface ICACContract
    {
       IQueryable<MyActivityEntity> MyActivities { get; }
       IQueryable<ActivityHistoryEntity> ActivityHistory { get; }
       MyActivityEntity NewMyActivityEntity();
       OperationResult InsertMyActivity(MyActivityEntity entity, bool isSave = true);
       OperationResult UpdateMyActivity(MyActivityEntity entity, bool isSave = true);
       OperationResult DeleteMyActivity(int id);
        OperationResult DeleteMyActivity(MyActivityEntity entity);
        bool IsMyActivity(int userId, int activityId, string url);
       ActivityHistoryEntity NewActivityHistoryEntity();
       OperationResult InsertActivityHistory(ActivityHistoryEntity entity, bool isSave = true);
    }
}
