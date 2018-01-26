using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/**************************************************************************
 * Developer: 		Leason
 * Computer:		Leason-PC
 * Domain:			Leason
 * CreatedOn:		2014/12/1 11:20:00
 * Description:		CecService
 * Version History:	Created,2014/12/1 11:20:00
 * 
 * 
 **************************************************************************/
using Sunnet.Cli.Core.Cec.Entities;
using Sunnet.Cli.Core.Cec.Interfaces;
using Sunnet.Framework.Core.Base;
using Sunnet.Framework.Core.Tool;
using Sunnet.Framework.EmailSender;
using Sunnet.Framework.Encrypt;
using Sunnet.Framework.File;
using Sunnet.Framework.Log;
using Sunnet.Cli.Core.Cpalls;
using Sunnet.Cli.Core.Cec.Models;

namespace Sunnet.Cli.Core.Cec
{
    internal class CecService : CoreServiceBase, ICecContract
    {

        private readonly ICecHistoryRpst _cecHistoryRpst;
        private readonly ICecResultRpst _cecResultRpst;

        public CecService(ISunnetLog log, IFile fileHelper, IEmailSender emailSender, IEncrypt encrypt,
            ICecHistoryRpst cecHistoryRpst, ICecResultRpst cecResultRpst, IUnitOfWork unit)
            : base(log, fileHelper, emailSender, encrypt)
        {
            _cecHistoryRpst = cecHistoryRpst;
            _cecResultRpst = cecResultRpst;

            UnitOfWork = unit;
        }


        public IQueryable<CecResultEntity> CecResultEntities
        {
            get { return _cecResultRpst.Entities; }
        }

        public CecResultEntity NewCecResultEntity()
        {
            return _cecResultRpst.Create();
        }

        public OperationResult InsertCecResult(CecResultEntity entity)
        {
            var result = new OperationResult(OperationResultType.Success);
            try
            {
                _cecResultRpst.Insert(entity);
            }
            catch (Exception ex)
            {
                result = ResultError(ex);
            }
            return result;
        }

        public OperationResult InsertCecHistory(List<CecHistoryEntity> list)
        {
            var result = new OperationResult(OperationResultType.Success);
            try
            {
                _cecHistoryRpst.Insert(list);
            }
            catch (Exception ex)
            {
                result = ResultError(ex);
            }
            return result;
        }

        public OperationResult DeleteCecResult(int id)
        {
            var result = new OperationResult(OperationResultType.Success);
            try
            {
                _cecResultRpst.Delete(id);
            }
            catch (Exception ex)
            {
                result = ResultError(ex);
            }
            return result;
        }

        public OperationResult UpdateCecResult(CecResultEntity entity)
        {
            var result = new OperationResult(OperationResultType.Success);
            try
            {
                _cecResultRpst.Update(entity);
            }
            catch (Exception ex)
            {
                result = ResultError(ex);
            }
            return result;
        }

        public CecResultEntity GetCecResult(int id)
        {
            return _cecResultRpst.GetByKey(id);
        }

        public IQueryable<CecHistoryEntity> CecHistoryEntities
        {
            get { return _cecHistoryRpst.Entities; }
        }

        public CecHistoryEntity NewCecHistoryEntity()
        {
            return _cecHistoryRpst.Create();
        }

        public OperationResult InsertCecHistory(CecHistoryEntity entity)
        {
            var result = new OperationResult(OperationResultType.Success);
            try
            {
                _cecHistoryRpst.Insert(entity);
            }
            catch (Exception ex)
            {
                result = ResultError(ex);
            }
            return result;
        }

        public OperationResult DeleteCecHistory(int id)
        {
            var result = new OperationResult(OperationResultType.Success);
            try
            {
                _cecHistoryRpst.Delete(id);
            }
            catch (Exception ex)
            {
                result = ResultError(ex);
            }
            return result;
        }

        public OperationResult UpdateCecHistory(CecHistoryEntity entity)
        {
            var result = new OperationResult(OperationResultType.Success);
            try
            {
                _cecHistoryRpst.Update(entity);
            }
            catch (Exception ex)
            {
                result = ResultError(ex);
            }
            return result;
        }

        public CecHistoryEntity GetCecHistory(int id)
        {

            return _cecHistoryRpst.GetByKey(id);
        }

        public OperationResult Reset(int assessmentId, int teacherId, Wave wave, string schoolYear)
        {
            var result = new OperationResult(OperationResultType.Success);
            try
            {
                _cecHistoryRpst.Result(assessmentId, wave, schoolYear, teacherId);
            }
            catch (Exception ex)
            {
                result = ResultError(ex);
            }
            return result;
        }

        public List<CECTeacherModel> GetCECTeacherList(int assessmentId, string schoolYear, int? coachId,
            List<int> communities, List<int> schoolIds, string firstname, string lastname,
            string teacherId, string sort, string order, int first, int count, out int total)
        {
            List<CECTeacherModel> list = new List<CECTeacherModel>();
            total = 0;
            try
            {
                list = _cecHistoryRpst.GetCECTeacherList(assessmentId, schoolYear, coachId, communities, schoolIds,
                    firstname, lastname,
                    teacherId, sort, order, first, count, out total);

            }
            catch (Exception ex)
            {
                ResultError(ex);
            }
            return list;
        }

        /// <summary>
        /// List of teachers with missing CEC: 
        /// </summary>
        /// <param name="cecAssessmentId"></param>
        /// <param name="schoolYear"></param>
        /// <param name="wave"></param>
        /// <returns></returns>
        public List<TeacherMissingCECModel> GetTeacherMissingCEC(int cecAssessmentId, string schoolYear, Wave wave)
        {
            return _cecHistoryRpst.GetTeacherMissingCEC(cecAssessmentId, schoolYear, wave);
        }

        /// <summary>
        /// EOY CEC Assessment Completion Report
        /// </summary>
        /// <param name="cecAssessmentId"></param>
        /// <param name="schoolYear"></param>
        /// <returns></returns>
        public List<CECCompletionModel> GetEOYCECCompletion(int cecAssessmentId, string schoolYear)
        {
            return _cecHistoryRpst.GetEOYCECCompletion(cecAssessmentId, schoolYear);
        }
    }
}
