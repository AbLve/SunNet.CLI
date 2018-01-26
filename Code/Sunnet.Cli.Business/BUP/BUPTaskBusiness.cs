using Sunnet.Cli.Core;
using Sunnet.Cli.Core.BUP;
using Sunnet.Cli.Core.BUP.Entities;
using Sunnet.Framework.Core.Tool;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sunnet.Framework.Core.Extensions;
using Sunnet.Cli.Core.Users.Entities;

namespace Sunnet.Cli.Business.BUP
{
    public class BUPTaskBusiness
    {
        private readonly IBUPContract _contract;
        public BUPTaskBusiness(EFUnitOfWorkContext unit = null)
        {
            _contract = DomainFacade.CreateBUPService(unit);
        }

        public List<BUPTaskEntity> GetList(BUPType type)
        {
            return _contract.Tasks.Where(r => r.Type == type).OrderByDescending(r => r.ID).ToList();
        }

        public List<BUPTaskEntity> GetPendingList()
        {
            return _contract.Tasks.Where(task=>task.Status==BUPStatus.Queued).OrderByDescending(r => r.ID).ToList();
        }

        public List<BUPTaskEntity> GetBupTasks(BUPType type, UserBaseEntity userInfo, string sort, string order, int first, int count, out int total)
        {
            var query = _contract.Tasks.Where(r => r.Type == type);
            if (userInfo.Role >= Core.Users.Enums.Role.Auditor)
                query = query.Where(r => r.CreatedBy == userInfo.ID);
            total = query.Count();
            var list = query.OrderBy(sort, order).Skip(first).Take(count);
            return list.ToList();
        }

        public BUPTaskEntity GetBupTask(int taskId)
        {
            return _contract.Tasks.SingleOrDefault(t=>t.ID==taskId);
        }

        public string ExecuteCommunitySql(string sql)
        {
            return _contract.ExecuteCommunitySql(sql);
        }

        public OperationResult Insert(BUPTaskEntity entity)
        {
            return _contract.InsertTask(entity);
        }

        public OperationResult Update(BUPTaskEntity entity)
        {
            return _contract.UpdateTask(entity);
        }

        public int ExecuteSqlCommand(string sql)
        {
            return _contract.ExecuteSqlCommand(sql);
        }

        public dynamic ExecuteSqlQuery(string sql, BUPType type)
        {
            return _contract.ExecuteSqlQuery(sql, type);
        }

        public static void Start(int id, int createdBy)
        {
            DomainFacade.CreateBUPService().Start(id, createdBy);
        }
    }
}
