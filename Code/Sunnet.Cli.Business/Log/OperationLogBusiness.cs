using Sunnet.Cli.Core;
using Sunnet.Cli.Core.Log;
using Sunnet.Cli.Core.Log.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sunnet.Cli.Core.Users.Entities;

namespace Sunnet.Cli.Business.Log
{
    public class OperationLogBusiness
    {
        private readonly IOperationLogContract _log;

        public OperationLogBusiness(EFUnitOfWorkContext unit = null)
        {
            _log = DomainFacade.CreateOperationLogService(unit);
        }

        public void InsertLog(OperationEnum logType, string logModule, string description, string ip, UserBaseEntity user)
        {
            _log.AddOperationLog(new OperationLogEntity()
            {
                CreatedBy   = user.ID,
                CreatedOn   = DateTime.Now,
                Description = description,
                Email       = user.Gmail,
                IP          = ip,
                Model       = logModule,
                Operation   = logType,
                UpdatedOn   = DateTime.Now
            });
        }

        public void InsertLogingLog(OperationLogEntity log)
        {
            _log.AddOperationLog(log);
        }

    }
}