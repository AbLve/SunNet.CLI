using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sunnet.Cli.Core.UpdateClusters.Entities;
using Sunnet.Framework.Core.Tool;

namespace Sunnet.Cli.Core.UpdateClusters
{
    public interface IUpdateClusterContract
    {
        #region System Update
        IQueryable<SystemUpdateEntity> SystemUpdates { get; }

        OperationResult InsertSystemUpdate(SystemUpdateEntity entity);

        OperationResult DeleteSystemUpdate(int id);

        OperationResult UpdateSystemUpdate(SystemUpdateEntity entity);

        #endregion
        #region Message Center
        IQueryable<MessageCenterEntity> MessageCenters { get; }

        OperationResult InsertMessageCenter(MessageCenterEntity entity);

        OperationResult DeleteMessageCenter(int id);

        OperationResult UpdateMessageCenter(MessageCenterEntity entity);
        #endregion

        #region New and Featured
        IQueryable<NewFeaturedEntity> NewFeatureds { get; }

        OperationResult InsertNewFeatured(NewFeaturedEntity entity);

        OperationResult DeleteNewFeatured(int id);

        OperationResult UpdateNewFeatured(NewFeaturedEntity entity);
        #endregion
    }
}
