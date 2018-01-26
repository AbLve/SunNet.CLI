using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using LinqKit;
using Sunnet.Cli.Business.Communities.Models;
using Sunnet.Cli.Business.UpdateCluster.Models;
using Sunnet.Cli.Core;
using Sunnet.Cli.Core.UpdateClusters;
using Sunnet.Cli.Core.UpdateClusters.Entities;
using Sunnet.Framework.Core.Tool;
using Sunnet.Framework.Core.Extensions;

namespace Sunnet.Cli.Business.UpdateCluster
{
    public class UpdateClusterBusiness
    {
        private readonly IUpdateClusterContract _updateClusterContract;
        public UpdateClusterBusiness(EFUnitOfWorkContext unit = null)
        {
            _updateClusterContract = DomainFacade.CreateUpdateClusterService(unit);
        }

        #region System Update

        public SystemUpdateEntity GetSystemUpdate(int id)
        {
            return _updateClusterContract.SystemUpdates.FirstOrDefault(e => e.ID == id);
        }

        public List<UpdateClusterModel> SearchSystemUpdates(string sort, string order, int first, int count,
            out int total)
        {
            var query = _updateClusterContract.SystemUpdates.AsExpandable()
                .Select(e => new UpdateClusterModel()
                {
                    ID = e.ID,
                    Description = e.Description,
                    Date = e.Date,
                    CreatedOn = e.CreatedOn,
                    UpdatedOn = e.UpdatedOn
                });
            total = query.Count();
            var list = query.OrderBy(sort, order).Skip(first).Take(count);
            return list.ToList();
        }

        public OperationResult InsertSystemUpdate(SystemUpdateEntity entity)
        {
            return _updateClusterContract.InsertSystemUpdate(entity);
        }

        public OperationResult DeleteSystemUpdate(int id)
        {
            return _updateClusterContract.DeleteSystemUpdate(id);
        }

        public OperationResult UpdateSystemUpdate(SystemUpdateEntity entity)
        {
            return _updateClusterContract.UpdateSystemUpdate(entity);
        }

        #endregion

        #region Message Center
        public MessageCenterEntity GetMessageCenter(int id)
        {
            return _updateClusterContract.MessageCenters.FirstOrDefault(e => e.ID == id);
        }

        public List<UpdateClusterModel> SearchMessageCenters(string sort, string order, int first, int count,
            out int total)
        {
            var query = _updateClusterContract.MessageCenters.AsExpandable()
                .Select(e => new UpdateClusterModel()
                {
                    ID = e.ID,
                    Description = e.Description,
                    Date = e.Date,
                    HyperLink = e.HyperLink,
                    CreatedOn = e.CreatedOn,
                    UpdatedOn = e.UpdatedOn
                });
            total = query.Count();
            var list = query.OrderBy(sort, order).Skip(first).Take(count);
            return list.ToList();
        }

        public OperationResult InsertMessageCenter(MessageCenterEntity entity)
        {
            return _updateClusterContract.InsertMessageCenter(entity);
        }

        public OperationResult DeleteMessageCenter(int id)
        {
            return _updateClusterContract.DeleteMessageCenter(id);
        }

        public OperationResult UpdateMessageCenter(MessageCenterEntity entity)
        {
            return _updateClusterContract.UpdateMessageCenter(entity);
        }
        #endregion

        #region New and Featured
        public NewFeaturedEntity GetNewFeatured(int id)
        {
            return _updateClusterContract.NewFeatureds.FirstOrDefault(e => e.ID == id);
        }

        public List<UpdateClusterModel> SearchNewFeatureds(string sort, string order, int first, int count,
            out int total)
        {
            var query = _updateClusterContract.NewFeatureds.AsExpandable()
                .Select(e => new UpdateClusterModel()
                {
                    ID = e.ID,
                    Description = e.Description,
                    HyperLink = e.HyperLink,
                    Title = e.Title,
                    ThumbnailName = e.ThumbnailName,
                    ThumbnailPath = e.ThumbnailPath,
                    CreatedOn = e.CreatedOn,
                    UpdatedOn = e.UpdatedOn
                });
            total = query.Count();
            var list = query.OrderBy(sort, order).Skip(first).Take(count);
            return list.ToList();
        }

        public OperationResult InsertNewFeatured(NewFeaturedEntity entity)
        {
            return _updateClusterContract.InsertNewFeatured(entity);
        }

        public OperationResult DeleteNewFeatured(int id)
        {
            return _updateClusterContract.DeleteNewFeatured(id);
        }

        public OperationResult UpdateNewFeatured(NewFeaturedEntity entity)
        {
            return _updateClusterContract.UpdateNewFeatured(entity);
        }
        #endregion
    }
}
