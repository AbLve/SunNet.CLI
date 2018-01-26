using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sunnet.Cli.Core.Trs.Interfaces;
using Sunnet.Cli.Core.Tsds.Entities;
using Sunnet.Cli.Core.Tsds.Interfaces;
using Sunnet.Framework.Core.Base;
using Sunnet.Framework.Core.Tool;
using Sunnet.Framework.EmailSender;
using Sunnet.Framework.Encrypt;
using Sunnet.Framework.File;
using Sunnet.Framework.Log;

namespace Sunnet.Cli.Core.Tsds
{
    internal class TsdsService : CoreServiceBase, ITsdsContract
    {
        private readonly ITsdsAssessmentFileRpst _tsdsAssessmentFileRpst;
        private readonly ITsdsRpst _tsdsRpst;
        private readonly ITsdsMapRpst _tsdsMapRpst;


        public TsdsService(ISunnetLog log, IFile fileHelper, IEmailSender emailSender, IEncrypt encrypt,
            ITsdsAssessmentFileRpst tsdsAssessmentFileRpst,
            ITsdsRpst tsdsRpst,
            ITsdsMapRpst tsdsMapRpst,
            IUnitOfWork unit)
            : base(log, fileHelper, emailSender, encrypt)
        {
            _tsdsAssessmentFileRpst = tsdsAssessmentFileRpst;
            _tsdsRpst = tsdsRpst;
            _tsdsMapRpst = tsdsMapRpst;
            UnitOfWork = unit;
        }

        #region TsdsAssessmentFiles

        public IQueryable<TsdsAssessmentFileEntity> TsdsAssessmentFiles
        {
            get
            {
                return _tsdsAssessmentFileRpst.Entities;
            }
        }

        public OperationResult InsertTsdsAssessmentFile(TsdsAssessmentFileEntity entity, bool isSave = true)
        {
            var result = new OperationResult(OperationResultType.Success);
            try
            {
                _tsdsAssessmentFileRpst.Insert(entity, isSave);
            }
            catch (Exception ex)
            {
                result = ResultError(ex);
            }
            return result;
        }

        public OperationResult DeleteTsdsAssessmentFile(int id)
        {
            var result = new OperationResult(OperationResultType.Success);
            try
            {
                _tsdsAssessmentFileRpst.Delete(id);
            }
            catch (Exception ex)
            {
                result = ResultError(ex);
            }
            return result;
        }

        public OperationResult UpdateTsdsAssessmentFile(TsdsAssessmentFileEntity entity, bool isSave = true)
        {
            var result = new OperationResult(OperationResultType.Success);
            try
            {
                _tsdsAssessmentFileRpst.Update(entity, isSave);
            }
            catch (Exception ex)
            {
                result = ResultError(ex);
            }
            return result;
        }

        public TsdsAssessmentFileEntity GetTsdsAssessmentFile(int id)
        {
            return _tsdsAssessmentFileRpst.GetByKey(id);
        }

        #endregion

        #region TSDS

        public IQueryable<TsdsEntity> Tsdses
        {
            get
            {
                return _tsdsRpst.Entities;
            }
        }
        public IQueryable<TsdsMapEntity> TsdsMaps
        {
            get
            {
                return _tsdsMapRpst.Entities;
            }
        }

        public OperationResult InsertTsds(List<TsdsEntity> entityList, bool isSave = true)
        {
            var result = new OperationResult(OperationResultType.Success);
            try
            {
                _tsdsRpst.Insert(entityList, isSave);
            }
            catch (Exception ex)
            {
                result = ResultError(ex);
            }
            return result;
        }
        public OperationResult InsertTsds(TsdsEntity  entity, bool isSave = true)
        {
            var result = new OperationResult(OperationResultType.Success);
            try
            {
                _tsdsRpst.Insert(entity, isSave);
            }
            catch (Exception ex)
            {
                result = ResultError(ex);
            }
            return result;
        }
        public OperationResult UpdateTsds(TsdsEntity entity, bool isSave = true)
        {
            var result = new OperationResult(OperationResultType.Success);
            try
            {
                _tsdsRpst.Update(entity, isSave);
            }
            catch (Exception ex)
            {
                result = ResultError(ex);
            }
            return result;
        }

        public TsdsEntity GetTsds(int id)
        {
            return _tsdsRpst.GetByKey(id);
        }

        #endregion
    }
}
