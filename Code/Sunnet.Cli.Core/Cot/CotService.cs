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
using Sunnet.Cli.Core.Cot.Entities;
using Sunnet.Cli.Core.Cot.Interfaces;
using Sunnet.Cli.Core.Cot.Models;
using Sunnet.Framework.Core.Base;
using Sunnet.Framework.Core.Tool;
using Sunnet.Framework.EmailSender;
using Sunnet.Framework.Encrypt;
using Sunnet.Framework.File;
using Sunnet.Framework.Log;

namespace Sunnet.Cli.Core.Cot
{
    internal class CotService : CoreServiceBase, ICotContract
    {
        private ICotAssessmentRpst _assessmentRpst;
        private ICotAssessmentItemRpst _itemRpst;
        private ICotAssessmentWaveItemRpst _waveItemRpst;
        private ICotWaveRpst _waveRpst;
        private ICotStgReportRpst _reportRpst;
        private ICotStgGroupRpst _cotStgGroupRpst;
        private ICotStgGroupItemRpst _cotStgGroupItemRpst;

        public IQueryable<CotAssessmentEntity> Assessments
        {
            get { return _assessmentRpst.Entities; }
        }

        public IQueryable<CotAssessmentWaveItemEntity> WaveItems
        {
            get { return _waveItemRpst.Entities; }
        }

        public IQueryable<CotAssessmentItemEntity> Items
        {
            get { return _itemRpst.Entities; }
        }
        public IQueryable<CotWaveEntity> Waves
        {
            get { return _waveRpst.Entities; }
        }
        public IQueryable<CotStgReportEntity> Reports
        {
            get { return _reportRpst.Entities; }
        }
        public IQueryable<CotStgGroupEntity> CotStgGroups
        {
            get { return _cotStgGroupRpst.Entities; }
        }
        public IQueryable<CotStgGroupItemEntity> CotStgGroupItems
        {
            get { return _cotStgGroupItemRpst.Entities; }
        }


        public CotService(ISunnetLog log, IFile fileHelper, IEmailSender emailSender, IEncrypt encrypt,
            ICotAssessmentRpst assessmentRpst,
            ICotAssessmentItemRpst itemRpst,
            ICotAssessmentWaveItemRpst waveItemRpst,
            ICotWaveRpst waveRpst,
            ICotStgReportRpst reportRpst,
            ICotStgGroupRpst cotStgGroupRpst,
            ICotStgGroupItemRpst cotStgGroupItemRpst,
            IUnitOfWork unit)
            : base(log, fileHelper, emailSender, encrypt)
        {
            _assessmentRpst = assessmentRpst;
            _waveItemRpst = waveItemRpst;
            _itemRpst = itemRpst;
            _waveRpst = waveRpst;
            _reportRpst = reportRpst;
            _cotStgGroupRpst = cotStgGroupRpst;
            _cotStgGroupItemRpst = cotStgGroupItemRpst;
        }

        public CotAssessmentEntity NewCotAssessmentEntity()
        {
            return _assessmentRpst.Create();
        }

        public CotAssessmentEntity GetCotAssessmentEntity(int id)
        {
            return _assessmentRpst.GetByKey(id);
        }

        public OperationResult InsertCotAssessmentEntity(CotAssessmentEntity entity)
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

        public OperationResult UpdateCotAssessmentEntity(CotAssessmentEntity entity)
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

        public CotWaveEntity NewCotWaveEntity()
        {
            return _waveRpst.Create();
        }

        public CotAssessmentItemEntity NewCotAssessmentItemEntity()
        {
            return _itemRpst.Create();
        }

        public CotAssessmentWaveItemEntity NewCotAssessmentWaveItemEntity()
        {
            return _waveItemRpst.Create();
        }

        public CotStgReportEntity NewCotStgReportEntity()
        {
            return _reportRpst.Create();
        }

        public OperationResult InsertCotStgReportEntity(CotStgReportEntity entity)
        {
            var result = new OperationResult(OperationResultType.Success);
            try
            {
                _reportRpst.Insert(entity);
            }
            catch (Exception ex)
            {
                result = ResultError(ex);
            }
            return result;
        }

        public OperationResult UpdateCotStgReportEntity(CotStgReportEntity entity)
        {
            var result = new OperationResult(OperationResultType.Success);
            try
            {
                _reportRpst.Update(entity);
            }
            catch (Exception ex)
            {
                result = ResultError(ex);
            }
            return result;
        }

        public CotStgReportEntity GetCotStgReportEntity(int id)
        {
            return _reportRpst.GetByKey(id);
        }

        #region COT STG Report Group

        public CotStgGroupEntity NewCotStgGroupEntity()
        {
            return _cotStgGroupRpst.Create();
        }
        public OperationResult InsertCotStgGroupEntity(CotStgGroupEntity entity)
        {
            var result = new OperationResult(OperationResultType.Success);
            try
            {
                _cotStgGroupRpst.Insert(entity);
            }
            catch (Exception ex)
            {
                result = ResultError(ex);
            }
            return result;
        }
        public OperationResult UpdateCotStgGroupEntity(CotStgGroupEntity entity)
        {
            var result = new OperationResult(OperationResultType.Success);
            try
            {
                _cotStgGroupRpst.Update(entity);
            }
            catch (Exception ex)
            {
                result = ResultError(ex);
            }
            return result;
        }
        public CotStgGroupEntity GetCotStgGroupEntity(int id)
        {
            return _cotStgGroupRpst.GetByKey(id);
        }

        public CotStgGroupItemEntity NewCotStgGroupItemEntity()
        {
            return _cotStgGroupItemRpst.Create();
        }
        public OperationResult InsertCotStgGroupItemEntity(CotStgGroupItemEntity entity)
        {
            var result = new OperationResult(OperationResultType.Success);
            try
            {
                _cotStgGroupItemRpst.Insert(entity);
            }
            catch (Exception ex)
            {
                result = ResultError(ex);
            }
            return result;
        }
        public OperationResult UpdateCotStgGroupItemEntity(CotStgGroupItemEntity entity)
        {
            var result = new OperationResult(OperationResultType.Success);
            try
            {
                _cotStgGroupItemRpst.Update(entity);
            }
            catch (Exception ex)
            {
                result = ResultError(ex);
            }
            return result;
        }
        public CotStgGroupItemEntity GetCotStgGroupItemEntity(int id)
        {
            return _cotStgGroupItemRpst.GetByKey(id);
        }

        public List<CotStgGroupItemModel> GetCotStgGroupItems(int cotStgReportId)
        {
            return _cotStgGroupItemRpst.GetCotStgGroupItems(cotStgReportId);
        }

        public OperationResult DeleteCotStgGroupItems(ICollection<CotStgGroupItemEntity> cotStgGroupItems)
        {
            var result = new OperationResult(OperationResultType.Success);
            try
            {
                _cotStgGroupItemRpst.Delete(cotStgGroupItems);
            }
            catch (Exception ex)
            {
                result = ResultError(ex);
            }
            return result;
        }
        public OperationResult InsertCotStgGroupItems(List<CotStgGroupItemEntity> entities)
        {
            var result = new OperationResult(OperationResultType.Success);
            try
            {
                _cotStgGroupItemRpst.Insert(entities);
            }
            catch (Exception ex)
            {
                result = ResultError(ex);
            }
            return result;
        }

        public OperationResult DeleteCotStgGroupItem(int id)
        {
            var result = new OperationResult(OperationResultType.Success);
            try
            {
                _cotStgGroupItemRpst.Delete(id);
            }
            catch (Exception ex)
            {
                result = ResultError(ex);
            }
            return result;
        }
        #endregion

        /// <summary>
        /// Gets the teachers.
        /// </summary>
        /// <param name="assessmentId">The assessment identifier.</param>
        /// <param name="schoolYear">The school year.</param>
        /// <param name="coachId">The coach identifier, 如果不需要过滤分配了Coach的Teacher,请传入null.</param>
        /// <param name="communityId">The community identifier.</param>
        /// <param name="schoolIds">The school ids.</param>
        /// <param name="firstname">The firstname.</param>
        /// <param name="lastname">The lastname.</param>
        /// <param name="teacherId">The teacher identifier.</param>
        /// <param name="searchExistingCot">if set to <c>true</c> [search existing cot].</param>
        /// <param name="sort">The sort.</param>
        /// <param name="order">The order.</param>
        /// <param name="first">The first.</param>
        /// <param name="count">The count.</param>
        /// <param name="total">The total.</param>
        /// <returns></returns>
        public List<CotTeacherModel> GetTeachers(int assessmentId, string schoolYear, int? coachId, List<int> communities, List<int> schoolIds, string firstname, string lastname,
            string teacherId, bool searchExistingCot, string sort, string order, int first, int count, out int total)
        {
            try
            {
                return _assessmentRpst.GetTeachers(assessmentId, schoolYear, coachId, communities, schoolIds, firstname, lastname,
                    teacherId, searchExistingCot, sort, order, first, count, out total);
            }
            catch (Exception ex)
            {
                total = 0;
                ResultError(ex);
                return null;
            }
        }

        public List<STGR_CompletionModel> GetSTGR_Completion_Report(int assessmentId)
        {
            return _reportRpst.GetSTGR_Completion_Report(assessmentId);
        }

        public List<TeacherMissingSTGRModel> GetTeacher_Missing_STGR(int assessmentId)
        {
            return _reportRpst.GetTeacher_Missing_STGR(assessmentId);
        }

        public List<COTCECCompletionModel> GetCOTCECCompletion(int cotAssessmentId, int cecAssessmentId, string schoolYear)
        {
            return _waveRpst.GetCOTCECCompletion(cotAssessmentId, cecAssessmentId, schoolYear);
        }

        public List<TeacherMissingCOTModel> GetTeacherMissingMOYCOT(int cotAssessmentId, string schoolYear)
        {
            return _waveRpst.GetTeacherMissingMOYCOT(cotAssessmentId, schoolYear);
        }
    }
}
