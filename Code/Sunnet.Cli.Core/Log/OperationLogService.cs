using Sunnet.Cli.Core.Log.Entities;
using Sunnet.Cli.Core.Log.Interfaces;
using Sunnet.Framework.Core.Base;
using Sunnet.Framework.Core.Tool;
using Sunnet.Framework.EmailSender;
using Sunnet.Framework.Encrypt;
using Sunnet.Framework.File;
using Sunnet.Framework.Log;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sunnet.Cli.Core.Log
{
    internal class OperationLogService : CoreServiceBase, IOperationLogContract
    {
        IOperationLogRpst LogRpst;
        public OperationLogService(ISunnetLog log,
            IFile fileHelper,
            IEmailSender emailSender,
            IEncrypt encrypt,
            IOperationLogRpst rpst,
            IUnitOfWork unit)
            : base(log, fileHelper, emailSender, encrypt)
        {
            LogRpst = rpst;
            UnitOfWork = unit;
        }

        public OperationResult AddOperationLog(OperationLogEntity entity)
        {
            var result = new OperationResult(OperationResultType.Success);
            try
            {
                LogRpst.Insert(entity);
            }
            catch (Exception ex)
            {
                result = ResultError(ex);
            }
            return result;
        }
    }
}
