using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using StructureMap;
using Sunnet.Framework;
using Sunnet.Framework.Core.Tool;
using Sunnet.Framework.EmailSender;
using Sunnet.Framework.Encrypt;
using Sunnet.Framework.Extensions;
using Sunnet.Framework.Core.Extensions;
using LinqKit;
using Sunnet.Cli.Business.Communities;
using Sunnet.Cli.Core;
using Sunnet.Cli.Core.CAC;
using Sunnet.Cli.Core.CAC.Entities;
using Sunnet.Framework.Helpers;
using Sunnet.Framework.Log;
using Sunnet.Cli.Business.CAC.Models;

namespace Sunnet.Cli.Business.Schools
{
    public class CACBusiness
    {
        private readonly ICACContract _cacService;

        public CACBusiness(EFUnitOfWorkContext unit = null)
        {
            _cacService = DomainFacade.CreateCACService(unit);
        }

        public MyActivityEntity GetMyActivitieById(int id)
        {
            return _cacService.MyActivities.FirstOrDefault(c => c.ID == id);
        }

        public IList<MyActivityEntity> GetMyActivities(int userId)
        {
            return _cacService.MyActivities.Where(c => c.UserId == userId).ToList();
        }

        public OperationResult AddMyActivity(MyActivityEntity entity)
        {
            return _cacService.InsertMyActivity(entity);
        }
        public OperationResult UpdateMyActivity(MyActivityEntity entity)
        {
            return _cacService.UpdateMyActivity(entity);
        }
        public MyActivityEntity GetMyActivity(int activityId, int userId)
        {
            return _cacService.MyActivities.FirstOrDefault(c => c.ActivityId == activityId && c.UserId == userId);
        }
        public MyActivityEntity GetMyActivity(int activityId, string name, int userId)
        {
            return _cacService.MyActivities.FirstOrDefault(c => c.ActivityId == activityId && c.ActivityName == name && c.UserId == userId);
        }
        public OperationResult DeleteActivity(int id)
        {
            return _cacService.DeleteMyActivity(id);
        }
        public OperationResult DeleteActivity(MyActivityEntity entity)
        {
            return _cacService.DeleteMyActivity(entity);
        }

        public OperationResult AddActivityHistory(ActivityHistoryEntity entity)
        {
            return _cacService.InsertActivityHistory(entity);
        }

        public List<MyActivityViewModel> GetMyActivitysPageList(Expression<Func<MyActivityEntity, bool>> condition, string sort, string order, int first, int count, out int total)
        {
            var query = this._cacService.MyActivities.AsExpandable().Where(condition).Select(it => new MyActivityViewModel()
            {
                ID = it.ID,
                CollectionType = it.CollectionType,
                ActivityName = it.ActivityName,
                Domain = it.Domain,
                SubDomain = it.SubDomain,
                Note = it.Remark,
                Url = it.Url,
                Objective = it.Objective,
                AgeGroup = it.AgeGroup
            });
            total = query.Count();
            var target = query.OrderBy(sort, order).Skip(first).Take(count).ToList();
            return target;
        }

        public List<string> GetAllDomain(string collectionType, int userId)
        {
            return
                _cacService.MyActivities.Where(e => e.CollectionType == collectionType && e.UserId == userId).Select(e => e.Domain).ToList();
        }
    }

}
