using Sunnet.Cli.Core.Ade.Entities;
using Sunnet.Cli.Core.Ade.Interfaces;
using Sunnet.Framework.Core.Base;
using Sunnet.Framework.Core.Tool;
using Sunnet.Framework.EmailSender;
using Sunnet.Framework.Encrypt;
using Sunnet.Framework.File;
using Sunnet.Framework.Log;
using System;
using System.Collections.Generic;
using System.Linq;
using Sunnet.Cli.Core.Common.Enums;

namespace Sunnet.Cli.Core.Ade
{
    internal partial class AdeService
    {
        public IList<TxkeaReceptiveItemEntity> GetTxkeaReceptiveItemsForPlayMeasure(List<int> measureIds)
        {
            return _itemBaseRpst.GetTxkeaReceptiveItemsForPlayMeasure(measureIds);
        }

        public IList<ItemBaseEntity> GetItems(List<int> measureIds)
        {
            return _itemBaseRpst.GetItems(measureIds);
        }

        public IList<ObservableChoiceEntity> GetObserveChoiceItems(List<int> itemIds)
        {
            return _itemBaseRpst.GetObserveChoiceItems(itemIds);
        }

        public IList<AnswerEntity> GetAnswers(List<int> itemIds)
        {
            return _itemBaseRpst.GetAnswers(itemIds);
        }

        public int GetIsExistMobileAudio(List<int> measureIds)
        {
            return _itemBaseRpst.GetIsExistMobileAudio(measureIds);
        }
    }
}
