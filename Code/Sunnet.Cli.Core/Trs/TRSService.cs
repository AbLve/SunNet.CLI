using Sunnet.Cli.Core.Students.Interfaces;
using Sunnet.Cli.Core.Trs.Entities;
using Sunnet.Cli.Core.Trs.Interfaces;
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

namespace Sunnet.Cli.Core.Trs
{
    internal class TRSService : CoreServiceBase, ITRSContract
    {
        private readonly ITRSAnswerRpst _answerRpst;
        private readonly ITRSAssessmentItemRpst _assessmentItemRpst;
        private readonly ITRSAssessmentRpst _assessmentRpst;
        private readonly ITrsStarRpst _trsStarRpst;
        private readonly ITRSItemRpst _itemRpst;
        private readonly ITRSItemAnswerRpst _itemAnserRpst;
        private readonly ITRSSubcategoryRpst _subcategoryRpst;
        private readonly ITRSAssessmentClassRpst _assessmentClassRpst;
        private readonly ITRSEventLogRpst _eventLogRpst;
        private readonly ITRSNotificationRpst _notificationRpst;
        private readonly ITRSEventLogFileRpst _eventLogFileRpst;

        internal TRSService(
            ISunnetLog log,
            IFile fileHelper,
            IEmailSender emailSender,
            IEncrypt encrypt,
            ITRSAnswerRpst answerRpst,
            ITRSAssessmentItemRpst assessmentItemRpst,
            ITrsStarRpst trsStarRpst,
            ITRSAssessmentRpst assessmentRpst,
            ITRSItemRpst itemRpst,
            ITRSItemAnswerRpst itemAnserRpst,
            ITRSSubcategoryRpst subcategoryRpst,
            ITRSAssessmentClassRpst assessmentClassRpst,
            ITRSEventLogRpst eventLogRpst,
            ITRSNotificationRpst notificationRpst,
            ITRSEventLogFileRpst eventLogFileRpst,
        IUnitOfWork unit
        )
            : base(log, fileHelper, emailSender, encrypt)
        {
            _answerRpst = answerRpst;
            _assessmentItemRpst = assessmentItemRpst;
            _trsStarRpst = trsStarRpst;
            _assessmentRpst = assessmentRpst;
            _itemRpst = itemRpst;
            _itemAnserRpst = itemAnserRpst;
            _subcategoryRpst = subcategoryRpst;
            _assessmentClassRpst = assessmentClassRpst;
            _eventLogRpst = eventLogRpst;
            _notificationRpst = notificationRpst;
            _eventLogFileRpst = eventLogFileRpst;
            UnitOfWork = unit;
        }

        public IQueryable<TRSAssessmentEntity> Assessments
        {
            get { return _assessmentRpst.Entities.Where(ass => ass.IsDeleted == false); }
        }

        public IQueryable<TrsStarEntity> Stars
        {
            get { return _trsStarRpst.Entities; }
        }

        public IQueryable<TRSItemEntity> Items
        {
            get { return _itemRpst.Entities; }
        }


        public IQueryable<TRSAnswerEntity> Answers
        {
            get { return _answerRpst.Entities; }
        }

        public IQueryable<TRSSubcategoryEntity> Subcategories
        {
            get { return _subcategoryRpst.Entities; }
        }

        public IQueryable<TRSEventLogEntity> TRSEventLogs
        {
            get { return _eventLogRpst.Entities; }
        }

        public TRSAssessmentEntity NewAssessmentEntity()
        {
            return _assessmentRpst.Create();
        }

        public IQueryable<TRSNotificationEntity> Notifications
        {
            get { return _notificationRpst.Entities; }
        }

        public IQueryable<TRSEventLogFileEntity> TRSEventLogFiles
        {
            get { return _eventLogFileRpst.Entities; }
        }

        public OperationResult InsertAssessment(TRSAssessmentEntity entity)
        {
            OperationResult result = new OperationResult(OperationResultType.Success);
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

        public OperationResult InsertAssessment(List<TRSAssessmentEntity> entities, bool isSave = true)
        {
            OperationResult result = new OperationResult(OperationResultType.Success);
            try
            {
                _assessmentRpst.Insert(entities, isSave);
            }
            catch (Exception ex)
            {
                result = ResultError(ex);
            }
            return result;
        }


        public OperationResult UpdateAssessment(TRSAssessmentEntity entity)
        {
            OperationResult result = new OperationResult(OperationResultType.Success);
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

        public TRSAssessmentEntity GetAssessment(int id)
        {
            return _assessmentRpst.GetByKey(id);
        }

        public TRSItemEntity GetItem(int id)
        {
            return _itemRpst.GetByKey(id);
        }

        public OperationResult UpdateItem(TRSItemEntity entity)
        {
            OperationResult result = new OperationResult(OperationResultType.Success);
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

        public OperationResult DeleteAssessmentClasses(IEnumerable<TRSAssessmentClassEntity> entities, bool isSave)
        {
            OperationResult result = new OperationResult(OperationResultType.Success);
            try
            {
                _assessmentClassRpst.Delete(entities, isSave);
            }
            catch (Exception ex)
            {
                result = ResultError(ex);
            }
            return result;
        }

        public int DeleteOfflineAssessment(List<int> assessmentIds)
        {
            return _assessmentRpst.DeleteOfflineAssessment(assessmentIds);
        }

        public TRSEventLogEntity GetEventLogById(int id)
        {
            return _eventLogRpst.GetByKey(id);
        }

        public OperationResult InsertEventLog(TRSEventLogEntity entity)
        {
            OperationResult result = new OperationResult(OperationResultType.Success);
            try
            {
                _eventLogRpst.Insert(entity);
            }
            catch (Exception ex)
            {
                result = ResultError(ex);
            }
            return result;
        }

        public OperationResult UpdateEventLog(TRSEventLogEntity entity)
        {
            OperationResult result = new OperationResult(OperationResultType.Success);
            try
            {
                _eventLogRpst.Update(entity);
            }
            catch (Exception ex)
            {
                result = ResultError(ex);
            }
            return result;
        }

        public OperationResult DeleteEventLog(int id)
        {
            OperationResult result = new OperationResult(OperationResultType.Success);
            try
            {
                _eventLogRpst.Delete(id);
            }
            catch (Exception ex)
            {
                result = ResultError(ex);
            }
            return result;
        }

        public OperationResult InsertNotifications(List<TRSNotificationEntity> entities)
        {
            OperationResult result = new OperationResult(OperationResultType.Success);
            try
            {
                _notificationRpst.Insert(entities);
            }
            catch (Exception ex)
            {
                result = ResultError(ex);
            }
            return result;
        }

        public OperationResult DeleteNotifications(List<TRSNotificationEntity> entities, bool isSave = true)
        {
            OperationResult result = new OperationResult(OperationResultType.Success);
            try
            {
                _notificationRpst.Delete(entities, isSave);
            }
            catch (Exception ex)
            {
                result = ResultError(ex);
            }
            return result;
        }

        public OperationResult UpdateAssessmentItem(TRSAssessmentItemEntity entity)
        {
            OperationResult result = new OperationResult(OperationResultType.Success);
            try
            {
                _assessmentItemRpst.Update(entity);
            }
            catch (Exception ex)
            {
                result = ResultError(ex);
            }
            return result;
        }

        public int UpdateItemAnswer(int trsAssessmentItemId, int AnswerId)
        {
            int count = 0;
            try
            {
                count = _assessmentItemRpst.UpdateItemAnswer(trsAssessmentItemId, AnswerId);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return count;
        }

        public int DelAssessmentItems(List<int> ids)
        {
            int count = 0;
            try
            {
                count = _assessmentItemRpst.DelAssessmentItems(ids);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return count;
        }

        public int UpdateTrsAssessmentStar(int trsAssessmentId, byte newStar)
        {
            int count = 0;
            try
            {
                count = _assessmentRpst.UpdateTrsAssessmentStar(trsAssessmentId, newStar);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return count;
        }

        public int UpdateTrsStar(int trsStarId, byte newStar)
        {
            int count = 0;
            try
            {
                count = _trsStarRpst.UpdateTrsStar(trsStarId, newStar);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return count;
        }

        public TRSEventLogFileEntity GetEventLogFileById(int id)
        {
            return _eventLogFileRpst.GetByKey(id);
        }

        public OperationResult InsertEventLogFile(TRSEventLogFileEntity entity)
        {
            OperationResult result = new OperationResult(OperationResultType.Success);
            try
            {
                _eventLogFileRpst.Insert(entity);
            }
            catch (Exception ex)
            {
                result = ResultError(ex);
            }
            return result;
        }

        public OperationResult UpdateEventLogFile(TRSEventLogFileEntity entity)
        {
            OperationResult result = new OperationResult(OperationResultType.Success);
            try
            {
                _eventLogFileRpst.Update(entity);
            }
            catch (Exception ex)
            {
                result = ResultError(ex);
            }
            return result;
        }

        public OperationResult DeleteEventLogFile(int id)
        {
            OperationResult result = new OperationResult(OperationResultType.Success);
            try
            {
                _eventLogFileRpst.Delete(id);
            }
            catch (Exception ex)
            {
                result = ResultError(ex);
            }
            return result;
        }

        public OperationResult InsertEventLogFiles(List<TRSEventLogFileEntity> entities)
        {
            OperationResult result = new OperationResult(OperationResultType.Success);
            try
            {
                _eventLogFileRpst.Insert(entities);
            }
            catch (Exception ex)
            {
                result = ResultError(ex);
            }
            return result;
        }
    }
}
