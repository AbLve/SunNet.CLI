using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Sunnet.Cli.Core.Ade.Entities;
using Sunnet.Framework.Core.Base;

/**************************************************************************
 * Developer: 		Jack
 * Computer:		Jackz
 * Domain:			Jackz
 * CreatedOn:		08/11/2014 03:49:39
 * Description:		ItemBaseEntity's IRepository
 * Version History:	Created,08/11/2014 03:49:39
 * 
 * 
 **************************************************************************/
namespace Sunnet.Cli.Core.Ade.Interfaces
{
    /// <summary>
    /// ItemBaseEntity's IRepository
    /// </summary>
    public interface IItemBaseRpst : IRepository<ItemBaseEntity, int>
    {
        bool AdjustOrder(List<int> items);
        IList<TxkeaReceptiveItemEntity> GetTxkeaReceptiveItemsForPlayMeasure(List<int> measureIds);
        IList<ItemBaseEntity> GetItems(List<int> measureIds);
        IList<ObservableChoiceEntity> GetObserveChoiceItems(List<int> itemIds);
        IList<AnswerEntity> GetAnswers(List<int> itemIds);
        int GetIsExistMobileAudio(List<int> measureIds);
        bool ExecuteSql(string strSql);
    }
}
