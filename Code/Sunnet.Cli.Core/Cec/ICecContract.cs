using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sunnet.Cli.Core.Cec.Entities;
using Sunnet.Framework.Core.Tool;
using Sunnet.Cli.Core.Cpalls;
using Sunnet.Cli.Core.Cec.Models;
/**************************************************************************
 * Developer: 		Leason
 * Computer:		Leason-PC
 * Domain:			Leason
 * CreatedOn:		2014/12/1 11:20:00
 * Description:		ICecContract
 * Version History:	Created,2014/12/1 11:20:00
 * 
 * 
 **************************************************************************/
namespace Sunnet.Cli.Core.Cec
{
    public interface ICecContract
    {
        IQueryable<CecHistoryEntity> CecHistoryEntities { get; }
        CecHistoryEntity NewCecHistoryEntity();
        OperationResult InsertCecHistory(CecHistoryEntity entity);
        OperationResult InsertCecHistory(List<CecHistoryEntity> list);
        OperationResult DeleteCecHistory(int id);
        OperationResult UpdateCecHistory(CecHistoryEntity entity);
        CecHistoryEntity GetCecHistory(int id);

        IQueryable<CecResultEntity> CecResultEntities { get; }
        CecResultEntity NewCecResultEntity();
        OperationResult InsertCecResult(CecResultEntity entity);
        OperationResult DeleteCecResult(int id);
        OperationResult UpdateCecResult(CecResultEntity entity);
        CecResultEntity GetCecResult(int id);

        OperationResult Reset(int assessmentId, int teacherId, Wave wave, string schoolYear);

        List<CECTeacherModel> GetCECTeacherList(int assessmentId, string schoolYear, int? coachId, List<int> communities,
            List<int> schoolIds, string firstname,
            string lastname, string teacherId,
            string sort, string order, int first, int count, out int total);


        /// <summary>
        /// List of teachers with missing CEC: 
        /// </summary>
        /// <param name="cecAssessmentId"></param>
        /// <param name="schoolYear"></param>
        /// <param name="wave"></param>
        /// <returns></returns>
        List<TeacherMissingCECModel> GetTeacherMissingCEC(int cecAssessmentId, string schoolYear, Wave wave);

        /// <summary>
        /// EOY CEC Assessment Completion Report
        /// </summary>
        /// <param name="cecAssessmentId"></param>
        /// <param name="schoolYear"></param>
        /// <returns></returns>
        List<CECCompletionModel> GetEOYCECCompletion(int cecAssessmentId, string schoolYear);
    }
}
