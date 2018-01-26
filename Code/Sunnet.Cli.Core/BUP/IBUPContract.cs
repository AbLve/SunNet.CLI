using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sunnet.Cli.Core.BUP.Entities;
using Sunnet.Framework.Core.Tool;

namespace Sunnet.Cli.Core.BUP
{
    public interface IBUPContract
    {
        IQueryable<BUPTaskEntity> Tasks { get; }

        OperationResult InsertTask(BUPTaskEntity entity);

        OperationResult UpdateTask(BUPTaskEntity entity);

        string ExecuteCommunitySql(string sql);

        int ExecuteSqlCommand(string sql);

        dynamic ExecuteSqlQuery(string sql, BUPType type);

        IQueryable<AutomationSettingEntity> AutomationSettings { get; }

        OperationResult InsertAutomationSetting(AutomationSettingEntity entity);

        OperationResult UpdateAutomationSetting(AutomationSettingEntity entity);

        AutomationSettingEntity GetAutomationSetting(int Id);

        void Start(int id, int createdBy);
    }
}
