using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/**************************************************************************
 * Developer: 		JackZhang
 * Computer:		JACKZ
 * Domain:			Jackz
 * CreatedOn:		2014/12/16 9:29:22
 * Description:		Please input class summary
 * Version History:	Created,2014/12/16 9:29:22
 * 
 * 
 **************************************************************************/
using Sunnet.Cli.Core.Cot.Entities;
using Sunnet.Cli.Core.Observable.Entities;
using Sunnet.Framework.Core.Base;

namespace Sunnet.Cli.Core.Observable.Interfaces
{
    public interface IObservableAssessmentRpst : IRepository<ObservableAssessmentEntity, int>
    {

    }
    public interface IObservableAssessmentItemRpst : IRepository<ObservableAssessmentItemEntity, int>
    {

    }
    public interface IObservableItemsHistoryRpst : IRepository<ObservableItemsHistoryEntity, int>
    {

    }
}
