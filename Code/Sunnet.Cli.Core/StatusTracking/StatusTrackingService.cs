using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/**************************************************************************
 * Developer: 		Joe
 * Computer:		Joe-PC
 * Domain:			Joe-pc
 * CreatedOn:		2015/06/03
 * Description:		Create AuthorityEntity
 * Version History:	Created,2015/06/03
 * 
 * 
 **************************************************************************/
using Sunnet.Framework.Core.Base;
using Sunnet.Cli.Core.StatusTracking.Interfaces;
using Sunnet.Framework.Log;
using Sunnet.Framework.File;
using Sunnet.Framework.EmailSender;
using Sunnet.Framework.Encrypt;
using Sunnet.Cli.Core.StatusTracking.Entities;
using Sunnet.Framework.Core.Tool;

namespace Sunnet.Cli.Core.StatusTracking
{
    internal class StatusTrackingService : CoreServiceBase, IStatusTrackingContract
    {
        readonly IStatusTrackingRpst _statusTrackingRpst;

        public StatusTrackingService(ISunnetLog log,
            IFile fileHelper,
            IEmailSender emailSender,
            IEncrypt encrypt,
            IStatusTrackingRpst statusTrackingRpst,
            IUnitOfWork unit)
            : base(log, fileHelper, emailSender, encrypt)
        {
            _statusTrackingRpst = statusTrackingRpst;
            UnitOfWork = unit;
        }


        public IQueryable<StatusTrackingEntity> StatusTrackings
        {
            get { return _statusTrackingRpst.Entities; }
        }

        public OperationResult AddStatusTracking(StatusTrackingEntity entity)
        {
            OperationResult result = new OperationResult(OperationResultType.Success);
            try
            {
                _statusTrackingRpst.Insert(entity);
            }
            catch (Exception ex)
            {
                result = ResultError(ex);
            }
            return result;
        }

        public OperationResult UpdateStatusTracking(StatusTrackingEntity entity)
        {
            OperationResult result = new OperationResult(OperationResultType.Success);
            try
            {
                _statusTrackingRpst.Update(entity);
            }
            catch (Exception ex)
            {
                result = ResultError(ex);
            }
            return result;
        }

        public StatusTrackingEntity GetStatusTracking(int id)
        {
            return _statusTrackingRpst.GetByKey(id);
        }
    }
}
