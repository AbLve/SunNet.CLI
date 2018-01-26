using System;
using System.Collections.Generic;
using System.Linq;
using Sunnet.Cli.Core.Ade.Entities;

namespace Sunnet.Cli.Business.Ade
{
    public partial class AdeBusiness
    {
        public IList<ObservableChoiceEntity> GetObserveChoiceItems(List<int> itemIds)
        {
            return _adeContract.GetObserveChoiceItems(itemIds);
        }

        public int GetIsExistMobileAudio(List<int> measureIds)
        {
            return _adeContract.GetIsExistMobileAudio(measureIds);
        }
    }
}