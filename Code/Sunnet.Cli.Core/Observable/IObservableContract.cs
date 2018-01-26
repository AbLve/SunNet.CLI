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
using Sunnet.Cli.Core.Observable.Entities;
using Sunnet.Framework.Core.Tool;

namespace Sunnet.Cli.Core.Observable
{
    public interface IObservableContract
    {
        ObservableAssessmentEntity NewObservableAssessmentEntity();
        ObservableAssessmentEntity GetObservableAssessmentEntity(int id);
        OperationResult InsertObservableAssessmentEntity(ObservableAssessmentEntity entity);
        OperationResult UpdateObservableAssessmentEntity(ObservableAssessmentEntity entity);

        ObservableAssessmentItemEntity NewObservableAssessmentItemEntity();
        ObservableAssessmentItemEntity GetObservableAssessmentItemEntity(int id);
        OperationResult InsertObservableAssessmentItemEntity(ObservableAssessmentItemEntity entity);
        OperationResult UpdateObservableAssessmentItemEntity(ObservableAssessmentItemEntity entity);

        ObservableItemsHistoryEntity NewObservableItemsHistoryEntity();
        ObservableItemsHistoryEntity GetObservableItemsHistoryEntity(int id);
        OperationResult InsertObservableItemsHistoryEntity(ObservableItemsHistoryEntity entity);
        OperationResult UpdateObservableItemsHistoryEntity(ObservableItemsHistoryEntity entity);

        IQueryable<ObservableAssessmentItemEntity> ObservableAssessmentItems { get; }
        IQueryable<ObservableAssessmentEntity> ObservableAssessments { get; }
    }
}
