using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sunnet.Cli.Core.UpdateClusters.Entities;
using Sunnet.Cli.Core.UpdateClusters.Interfaces;
using Sunnet.Framework.Core.Base;
using Sunnet.Framework.Core.Tool;
using Sunnet.Framework.EmailSender;
using Sunnet.Framework.Encrypt;
using Sunnet.Framework.File;
using Sunnet.Framework.Log;

namespace Sunnet.Cli.Core.UpdateClusters
{
    internal class UpdateClusterService : CoreServiceBase, IUpdateClusterContract
    {
        private readonly ISystemUpdateRpst _systemUpdateRpst;
        private readonly IMessageCenterRpst _messageCenterRpst;
        private readonly INewFeaturedRpst _newFeaturedRpst;

        public UpdateClusterService(ISunnetLog log, IFile fileHelper, IEmailSender emailSender, IEncrypt encrypt,
            ISystemUpdateRpst systemUpdateRpst,
            IMessageCenterRpst messageCenterRpst,
            INewFeaturedRpst newFeaturedRpst,
            IUnitOfWork unit)
            : base(log, fileHelper, emailSender, encrypt)
        {
            _systemUpdateRpst = systemUpdateRpst;
            _messageCenterRpst = messageCenterRpst;
            _newFeaturedRpst = newFeaturedRpst;
            UnitOfWork = unit;
        }

        #region System Update
        public IQueryable<SystemUpdateEntity> SystemUpdates
        {
            get
            {
                return _systemUpdateRpst.Entities;
            }
        }

        public OperationResult InsertSystemUpdate(SystemUpdateEntity entity)
        {
            var result = new OperationResult(OperationResultType.Success);
            try
            {
                _systemUpdateRpst.Insert(entity);
            }
            catch (Exception ex)
            {
                result = ResultError(ex);
            }
            return result;
        }

        public OperationResult DeleteSystemUpdate(int id)
        {
            var result = new OperationResult(OperationResultType.Success);
            try
            {
                _systemUpdateRpst.Delete(id);
            }
            catch (Exception ex)
            {
                result = ResultError(ex);
            }
            return result;
        }

        public OperationResult UpdateSystemUpdate(SystemUpdateEntity entity)
        {
            var result = new OperationResult(OperationResultType.Success);
            try
            {
                _systemUpdateRpst.Update(entity);
            }
            catch (Exception ex)
            {
                result = ResultError(ex);
            }
            return result;
        }
        #endregion

        #region Message Center
        public IQueryable<MessageCenterEntity> MessageCenters
        {
            get
            {
                return _messageCenterRpst.Entities;
            }
        }

        public OperationResult InsertMessageCenter(MessageCenterEntity entity)
        {
            var result = new OperationResult(OperationResultType.Success);
            try
            {
                _messageCenterRpst.Insert(entity);
            }
            catch (Exception ex)
            {
                result = ResultError(ex);
            }
            return result;
        }

        public OperationResult DeleteMessageCenter(int id)
        {
            var result = new OperationResult(OperationResultType.Success);
            try
            {
                _messageCenterRpst.Delete(id);
            }
            catch (Exception ex)
            {
                result = ResultError(ex);
            }
            return result;
        }

        public OperationResult UpdateMessageCenter(MessageCenterEntity entity)
        {
            var result = new OperationResult(OperationResultType.Success);
            try
            {
                _messageCenterRpst.Update(entity);
            }
            catch (Exception ex)
            {
                result = ResultError(ex);
            }
            return result;
        }
        #endregion

        #region New and Featured
        public IQueryable<NewFeaturedEntity> NewFeatureds
        {
            get
            {
                return _newFeaturedRpst.Entities;
            }
        }

        public OperationResult InsertNewFeatured(NewFeaturedEntity entity)
        {
            var result = new OperationResult(OperationResultType.Success);
            try
            {
                _newFeaturedRpst.Insert(entity);
            }
            catch (Exception ex)
            {
                result = ResultError(ex);
            }
            return result;
        }

        public OperationResult DeleteNewFeatured(int id)
        {
            var result = new OperationResult(OperationResultType.Success);
            try
            {
                _newFeaturedRpst.Delete(id);
            }
            catch (Exception ex)
            {
                result = ResultError(ex);
            }
            return result;
        }

        public OperationResult UpdateNewFeatured(NewFeaturedEntity entity)
        {
            var result = new OperationResult(OperationResultType.Success);
            try
            {
                _newFeaturedRpst.Update(entity);
            }
            catch (Exception ex)
            {
                result = ResultError(ex);
            }
            return result;
        }
        #endregion
    }
}
