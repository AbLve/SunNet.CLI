using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/**************************************************************************
 * Developer: 		JackZhang
 * Computer:		JACKZ
 * Domain:			Jackz
 * CreatedOn:		2014/12/16 9:56:31
 * Description:		Please input class summary
 * Version History:	Created,2014/12/16 9:56:31
 * 
 * 
 **************************************************************************/
using Sunnet.Cli.Core.Cot.Interfaces;
using Sunnet.Cli.Core.Observable.Entities;
using Sunnet.Cli.Core.Observable.Interfaces;
using Sunnet.Framework.Core.Base;
using Sunnet.Framework.Core.Tool;
using Sunnet.Framework.EmailSender;
using Sunnet.Framework.Encrypt;
using Sunnet.Framework.File;
using Sunnet.Framework.Log;

namespace Sunnet.Cli.Core.Observable
{
    internal class ObservableService : CoreServiceBase, IObservableContract
    {
        private IObservableAssessmentRpst _assessmentRpst;
        private IObservableAssessmentItemRpst _itemRpst;
        private IObservableItemsHistoryRpst _historyRpst;

        public ObservableService(ISunnetLog log, IFile fileHelper, IEmailSender emailSender, IEncrypt encrypt,
            IObservableAssessmentRpst assessmentRpst,
            IObservableAssessmentItemRpst itemRpst,
            IObservableItemsHistoryRpst historyRpst,
            IUnitOfWork unit)
            : base(log, fileHelper, emailSender, encrypt)
        {
            _assessmentRpst = assessmentRpst;
            _itemRpst = itemRpst;
            _historyRpst = historyRpst;

        }

        public IQueryable<ObservableAssessmentItemEntity> ObservableAssessmentItems
        {
            get { return _itemRpst.Entities; }
        }
        public IQueryable<ObservableAssessmentEntity> ObservableAssessments
        {
            get { return _assessmentRpst.Entities; }
        } 

        public ObservableAssessmentEntity NewObservableAssessmentEntity()
        {
            return _assessmentRpst.Create();
        }
        public ObservableAssessmentEntity GetObservableAssessmentEntity(int id)
        {
            return _assessmentRpst.GetByKey(id);
        }
        public OperationResult InsertObservableAssessmentEntity(ObservableAssessmentEntity entity)
        {
            var result = new OperationResult(OperationResultType.Success);
            try
            {
                _assessmentRpst.Insert(entity);
            }
            catch (Exception ex)
            {
                result = ResultError(ex);
            }
            return result;
        }
        public OperationResult UpdateObservableAssessmentEntity(ObservableAssessmentEntity entity)
        {
            var result = new OperationResult(OperationResultType.Success);
            try
            {
                _assessmentRpst.Update(entity);
            }
            catch (Exception ex)
            {
                result = ResultError(ex);
            }
            return result;
        }



        public ObservableAssessmentItemEntity NewObservableAssessmentItemEntity()
        {
            return _itemRpst.Create();
        }
        public ObservableAssessmentItemEntity GetObservableAssessmentItemEntity(int id)
        {
            return _itemRpst.GetByKey(id);
        }
        public OperationResult InsertObservableAssessmentItemEntity(ObservableAssessmentItemEntity entity)
        {
            var result = new OperationResult(OperationResultType.Success);
            try
            {
                _itemRpst.Insert(entity);
            }
            catch (Exception ex)
            {
                result = ResultError(ex);
            }
            return result;
        }
        public OperationResult UpdateObservableAssessmentItemEntity(ObservableAssessmentItemEntity entity)
        {
            var result = new OperationResult(OperationResultType.Success);
            try
            {
                _itemRpst.Update(entity);
            }
            catch (Exception ex)
            {
                result = ResultError(ex);
            }
            return result;
        }



        public ObservableItemsHistoryEntity NewObservableItemsHistoryEntity()
        {
            return _historyRpst.Create();
        }
        public ObservableItemsHistoryEntity GetObservableItemsHistoryEntity(int id)
        {
            return _historyRpst.GetByKey(id);
        }
        public OperationResult InsertObservableItemsHistoryEntity(ObservableItemsHistoryEntity entity)
        {
            var result = new OperationResult(OperationResultType.Success);
            try
            {
                _historyRpst.Insert(entity);
            }
            catch (Exception ex)
            {
                result = ResultError(ex);
            }
            return result;
        }
        public OperationResult UpdateObservableItemsHistoryEntity(ObservableItemsHistoryEntity entity)
        {
            var result = new OperationResult(OperationResultType.Success);
            try
            {
                _historyRpst.Update(entity);
            }
            catch (Exception ex)
            {
                result = ResultError(ex);
            }
            return result;
        }
    }
}
