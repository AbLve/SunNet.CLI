using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Sunnet.Cli.Core.DataProcess;
using Sunnet.Cli.Core;
using Sunnet.Cli.Core.DataProcess.Entities;
using Sunnet.Framework.Core.Extensions;
using Sunnet.Framework.Core.Tool;
using Sunnet.Cli.Business.DataProcess.Models;
using Sunnet.Cli.Core.DataProcess.Models;
using Sunnet.Cli.Core.Users.Entities;
using Sunnet.Cli.Business.Communities;
using System.Linq.Expressions;
using Sunnet.Framework.Helpers;
using Sunnet.Cli.Core.Communities.Entities;
using Sunnet.Cli.Core.Users.Enums;

namespace Sunnet.Cli.Business.DataProcess
{
    public class DataProcessBusiness
    {
        //public DataProcessBusiness(

        private readonly IDataProcessContract _process;
        private readonly CommunityBusiness _communityBusiness;

        public DataProcessBusiness(EFUnitOfWorkContext unit = null)
        {
            _process = DomainFacade.CreateDataProcessService(unit);
            _communityBusiness = new CommunityBusiness(unit);
        }

        public List<DataGroupEntity> GetGroupList(UserBaseEntity user, string sort, string order, int first, int count, out int total)
        {
            IEnumerable<int> communityIds = _communityBusiness.GetCommunitySelectList(user, PredicateHelper.True<CommunityEntity>())
                .Select(r => r.ID);
            var query = _process.Groups.Where(r => communityIds.Contains(r.CommunityId));
            total = query.Count();
            var list = query.OrderBy(sort, order).Skip(first).Take(count);
            return list.ToList();
        }

        public List<DataGroupEntity> GetDataGroupListByProcessStatus(ProcessStatus statu)
        {
            var query = _process.Groups.Where(r => r.Status ==statu);
            return query.ToList();
        }

        public DataGroupEntity GetDataGroupEntity(int dataGroupId)
        {
            var query = _process.Groups.SingleOrDefault(g=>g.ID==dataGroupId);

            return query;
        }

        public OperationResult InsertGroup(DataGroupEntity entity)
        {
            return _process.InsertGroup(entity);
        }

        public OperationResult UpdateGrop(DataGroupEntity entity)
        {
            return _process.UpdateGroup(entity);
        }

        public bool DeleteGroup(int id)
        {
            return _process.DeleteGroup(id);
        }

        public string ImportData(string sql)
        {
            return _process.ImportData(sql);
        }


        public static void Start(int id, int createdBy)
        {
            //System.Threading.Thread.Sleep(10000);
            DomainFacade.CreateDataProcessService().Start(id, createdBy);
        }

        public List<RecordRemarkModel> GetRemarks(string sql)
        {
            return _process.GetRemarks(sql);
        }
    }
}