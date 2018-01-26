using Sunnet.Cli.Core.Ade.Entities;
using Sunnet.Framework.Core.Tool;
using System.Collections.Generic;
using System.Linq;

namespace Sunnet.Cli.Core.Ade
{
    public partial interface IAdeContract
    {
        IList<TxkeaReceptiveItemEntity> GetTxkeaReceptiveItemsForPlayMeasure(List<int> measureIds);

        IList<ItemBaseEntity> GetItems(List<int> measureIds);

        IList<ObservableChoiceEntity> GetObserveChoiceItems(List<int> itemIds);

        IList<AnswerEntity> GetAnswers(List<int> itemIds);

        int GetIsExistMobileAudio(List<int> measureIds);
    }
}
