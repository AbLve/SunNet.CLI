using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/**************************************************************************
 * Developer: 		JackZhang
 * Computer:		JACKZ
 * Domain:			Jackz
 * CreatedOn:		2014/12/16 9:56:16
 * Description:		Please input class summary
 * Version History:	Created,2014/12/16 9:56:16
 * 
 * 
 **************************************************************************/
using Sunnet.Cli.Core.Cot.Entities;
using Sunnet.Cli.Core.Cot.Models;
using Sunnet.Framework.Core.Tool;

namespace Sunnet.Cli.Core.Cot
{
    public interface ICotContract
    {
        #region IQueryable
        IQueryable<CotAssessmentEntity> Assessments { get; }
        IQueryable<CotAssessmentItemEntity> Items { get; }
        IQueryable<CotAssessmentWaveItemEntity> WaveItems { get; }
        IQueryable<CotWaveEntity> Waves { get; }
        IQueryable<CotStgReportEntity> Reports { get; }
        IQueryable<CotStgGroupEntity> CotStgGroups { get; }
        IQueryable<CotStgGroupItemEntity> CotStgGroupItems { get; }
        #endregion


        CotAssessmentEntity NewCotAssessmentEntity();
        CotAssessmentEntity GetCotAssessmentEntity(int id);
        OperationResult InsertCotAssessmentEntity(CotAssessmentEntity entity);
        OperationResult UpdateCotAssessmentEntity(CotAssessmentEntity entity);

        CotWaveEntity NewCotWaveEntity();


        CotAssessmentItemEntity NewCotAssessmentItemEntity();

        CotAssessmentWaveItemEntity NewCotAssessmentWaveItemEntity();

        CotStgReportEntity NewCotStgReportEntity();
        OperationResult InsertCotStgReportEntity(CotStgReportEntity entity);
        OperationResult UpdateCotStgReportEntity(CotStgReportEntity entity);
        CotStgReportEntity GetCotStgReportEntity(int id);


        CotStgGroupEntity NewCotStgGroupEntity();
        OperationResult InsertCotStgGroupEntity(CotStgGroupEntity entity);
        OperationResult UpdateCotStgGroupEntity(CotStgGroupEntity entity);
        CotStgGroupEntity GetCotStgGroupEntity(int id);

        CotStgGroupItemEntity NewCotStgGroupItemEntity();
        OperationResult InsertCotStgGroupItemEntity(CotStgGroupItemEntity entity);
        OperationResult UpdateCotStgGroupItemEntity(CotStgGroupItemEntity entity);
        CotStgGroupItemEntity GetCotStgGroupItemEntity(int id);
        List<CotStgGroupItemModel> GetCotStgGroupItems(int cotStgReportId);
        OperationResult DeleteCotStgGroupItems(ICollection<CotStgGroupItemEntity> cotStgGroupItems);
        OperationResult InsertCotStgGroupItems(List<CotStgGroupItemEntity> entities);
        OperationResult DeleteCotStgGroupItem(int id);

        List<CotTeacherModel> GetTeachers(int assessmentId, string schoolYear, int? coachId, List<int> communities, List<int> schoolIds, string firstname,
            string lastname, string teacherId, bool searchExistingCot,
            string sort, string order, int first, int count, out int total);


        List<STGR_CompletionModel> GetSTGR_Completion_Report(int assessmentId);

        List<TeacherMissingSTGRModel> GetTeacher_Missing_STGR(int assessmentId);

        /// <summary>
        /// BOY & MOY - COT & CEC Assessment Completion Report
        /// </summary>
        /// <param name="cotAssessmentId"></param>
        /// <param name="cecAssessmentId"></param>
        /// <param name="schoolYear"></param>
        /// <returns></returns>
        List<COTCECCompletionModel> GetCOTCECCompletion(int cotAssessmentId, int cecAssessmentId, string schoolYear);

        /// <summary>
        /// List of teachers with missing COT: 
        /// </summary>
        List<TeacherMissingCOTModel> GetTeacherMissingMOYCOT(int cotAssessmentId, string schoolYear);


    }
}
