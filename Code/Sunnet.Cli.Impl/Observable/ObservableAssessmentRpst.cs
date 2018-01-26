using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/**************************************************************************
 * Developer: 		JackZhang
 * Computer:		JACKZ
 * Domain:			Jackz
 * CreatedOn:		2014/12/16 13:58:24
 * Description:		Please input class summary
 * Version History:	Created,2014/12/16 13:58:24
 * 
 * 
 **************************************************************************/
 
using Sunnet.Cli.Core.Observable.Entities;
using Sunnet.Cli.Core.Observable.Interfaces;
using Sunnet.Framework.Core.Base;

namespace Sunnet.Cli.Impl.Observable
{
    public class ObservableAssessmentRpst: EFRepositoryBase<ObservableAssessmentEntity, int>, IObservableAssessmentRpst
    {
        public ObservableAssessmentRpst(IUnitOfWork unit)
        {
            this.UnitOfWork = unit;
        }
    }
    public class ObservableAssessmentItemRpst : EFRepositoryBase<ObservableAssessmentItemEntity, int>, IObservableAssessmentItemRpst
    {
        public ObservableAssessmentItemRpst(IUnitOfWork unit)
        {
            this.UnitOfWork = unit;
        }
    }
    public class ObservableItemsHistoryRpst : EFRepositoryBase<ObservableItemsHistoryEntity, int>, IObservableItemsHistoryRpst
    {
        public ObservableItemsHistoryRpst(IUnitOfWork unit)
        {
            this.UnitOfWork = unit;
        }
    }
}
