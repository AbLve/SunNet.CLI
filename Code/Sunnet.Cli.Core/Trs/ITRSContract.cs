using Sunnet.Cli.Core.Trs.Entities;
using Sunnet.Framework.Core.Tool;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


/**************************************************************************
 * Developer: 		JackZhang
 * Computer:		JACKZ
 * Domain:			JackZ
 * CreatedOn:		1/9 2015 13:50:11
 * Description:		Please input class summary
 * Version History:	Created,1/9 2015 13:50:11
 * 
 * 
 **************************************************************************/

namespace Sunnet.Cli.Core.Trs
{
    public interface ITRSContract
    {
        IQueryable<TRSAssessmentEntity> Assessments { get; }

        IQueryable<TrsStarEntity> Stars { get; }

        IQueryable<TRSItemEntity> Items { get; }

        IQueryable<TRSAnswerEntity> Answers { get; }

        IQueryable<TRSSubcategoryEntity> Subcategories { get; }

        IQueryable<TRSEventLogEntity> TRSEventLogs { get; }

        IQueryable<TRSNotificationEntity> Notifications { get; }

        IQueryable<TRSEventLogFileEntity> TRSEventLogFiles { get; }


        TRSAssessmentEntity NewAssessmentEntity();
        OperationResult InsertAssessment(TRSAssessmentEntity entity);
        OperationResult InsertAssessment(List<TRSAssessmentEntity> entities, bool isSave);
        OperationResult UpdateAssessment(TRSAssessmentEntity entity);

        TRSAssessmentEntity GetAssessment(int id);

        TRSItemEntity GetItem(int id);
        OperationResult UpdateItem(TRSItemEntity entity);

        OperationResult DeleteAssessmentClasses(IEnumerable<TRSAssessmentClassEntity> entities, bool isSave);

        int DeleteOfflineAssessment(List<int> assessmentIds);

        TRSEventLogEntity GetEventLogById(int id);
        OperationResult InsertEventLog(TRSEventLogEntity entity);
        OperationResult UpdateEventLog(TRSEventLogEntity entity);
        OperationResult DeleteEventLog(int id);

        OperationResult InsertNotifications(List<TRSNotificationEntity> entities);
        OperationResult DeleteNotifications(List<TRSNotificationEntity> entities, bool isSave = true);

        OperationResult UpdateAssessmentItem(TRSAssessmentItemEntity entity);

        int UpdateItemAnswer(int trsAssessmentItemId, int AnswerId);

        int DelAssessmentItems(List<int> ids);

        int UpdateTrsAssessmentStar(int trsAssessmentId, byte newStar);

        int UpdateTrsStar(int trsStarId, byte newStar);

        TRSEventLogFileEntity GetEventLogFileById(int id);
        OperationResult InsertEventLogFile(TRSEventLogFileEntity entity);
        OperationResult UpdateEventLogFile(TRSEventLogFileEntity entity);
        OperationResult DeleteEventLogFile(int id);
        OperationResult InsertEventLogFiles(List<TRSEventLogFileEntity> entities);
    }
}
