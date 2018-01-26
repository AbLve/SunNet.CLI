using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Sunnet.Cli.Business.UpdateCluster;
using Sunnet.Cli.Business.UpdateCluster.Models;
using Sunnet.Cli.Core.UpdateClusters.Entities;
using Sunnet.Cli.UIBase.Models;
using Sunnet.Framework.Core.Tool;
using Sunnet.Framework.Helpers;

namespace Sunnet.Cli.MainSite.Controllers
{
    public class UpdateClusterController : BaseController
    {
        private UpdateClusterBusiness _updateClusterBusiness = null;

        public UpdateClusterController()
        {
            _updateClusterBusiness = new UpdateClusterBusiness(UnitWorkContext);
        }

        // GET: UpdateCluster

        public ActionResult Index()
        {

            return View();
        }

        public ActionResult View(string type,int id)
        {
            if (type.ToLower() == "systemupdate")
            {
                var entity = _updateClusterBusiness.GetSystemUpdate(id);
                ViewBag.Messages = entity.Description;
            }
            else if (type.ToLower() == "messagecenter")
            {
                var entity = _updateClusterBusiness.GetMessageCenter(id);
                ViewBag.Messages = entity.Description;
            }
            else if (type.ToLower() == "newfeatured")
            {
                var entity = _updateClusterBusiness.GetNewFeatured(id);
                ViewBag.Messages = entity.Description;
            }
            return View();
        }

        #region System Updates
        public string SearchSystemUpdates(string sort = "UpdatedOn", string order = "Asc", int first = 0, int count = 10)
        {
            var total = 0;
            var list = _updateClusterBusiness.SearchSystemUpdates(sort, order, first, count, out total);
            var result = new { total = total, data = list };
            return JsonHelper.SerializeObject(result);
        }

        [HttpPost]
        [ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public string SaveSystemUpdate(UpdateClusterModel updateCluster)
        {
            var response = new PostFormResponse();
            OperationResult result = new OperationResult(OperationResultType.Success);
            SystemUpdateEntity entity = new SystemUpdateEntity();
            if (updateCluster.ID > 0)
            {
                entity = _updateClusterBusiness.GetSystemUpdate(updateCluster.ID);
                entity.Date = updateCluster.Date;
                entity.Description = updateCluster.Description;
                entity.UpdatedBy = UserInfo.ID;
                entity.UpdatedOn = DateTime.Now;
                result = _updateClusterBusiness.UpdateSystemUpdate(entity);
            }
            else
            {
                entity.Date = updateCluster.Date;
                entity.Description = updateCluster.Description;
                entity.CreatedBy = UserInfo.ID;
                entity.UpdatedBy = UserInfo.ID;
                entity.CreatedOn = DateTime.Now;
                entity.UpdatedOn = DateTime.Now;
                result = _updateClusterBusiness.InsertSystemUpdate(entity);
            }
            response.Success = result.ResultType == OperationResultType.Success;
            response.Message = result.Message;
            return JsonHelper.SerializeObject(response);
        }

        public string DeleteSystemUpdate(int id)
        {
            var response = new PostFormResponse();
            OperationResult result = new OperationResult(OperationResultType.Success);
            result = _updateClusterBusiness.DeleteSystemUpdate(id);
            response.Success = result.ResultType == OperationResultType.Success;
            response.Message = result.Message;
            response.ModelState = ModelState;
            return JsonHelper.SerializeObject(response);
        }

        #endregion

        #region Message Center
        public string SearchMessageCenters(string sort = "UpdatedOn", string order = "Asc", int first = 0, int count = 10)
        {
            var total = 0;
            var list = _updateClusterBusiness.SearchMessageCenters(sort, order, first, count, out total);
            var result = new { total = total, data = list };
            return JsonHelper.SerializeObject(result);
        }

        [HttpPost]
        [ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public string SaveMessageCenter(UpdateClusterModel updateCluster)
        {
            var response = new PostFormResponse();
            OperationResult result = new OperationResult(OperationResultType.Success);
            MessageCenterEntity entity = new MessageCenterEntity();
            if (updateCluster.ID > 0)
            {
                entity = _updateClusterBusiness.GetMessageCenter(updateCluster.ID);
                entity.Date = updateCluster.Date;
                entity.Description = updateCluster.Description;
                entity.HyperLink = updateCluster.HyperLink ?? "";
                entity.UpdatedBy = UserInfo.ID;
                entity.UpdatedOn = DateTime.Now;
                result = _updateClusterBusiness.UpdateMessageCenter(entity);
            }
            else
            {
                entity.Date = updateCluster.Date;
                entity.Description = updateCluster.Description;
                entity.HyperLink = updateCluster.HyperLink ?? "";
                entity.CreatedBy = UserInfo.ID;
                entity.UpdatedBy = UserInfo.ID;
                entity.CreatedOn = DateTime.Now;
                entity.UpdatedOn = DateTime.Now;
                result = _updateClusterBusiness.InsertMessageCenter(entity);
            }
            response.Success = result.ResultType == OperationResultType.Success;
            response.Message = result.Message;
            return JsonHelper.SerializeObject(response);
        }
        public string DeleteMessageCenter(int id)
        {
            var response = new PostFormResponse();
            OperationResult result = new OperationResult(OperationResultType.Success);
            result = _updateClusterBusiness.DeleteMessageCenter(id);
            response.Success = result.ResultType == OperationResultType.Success;
            response.Message = result.Message;
            response.ModelState = ModelState;
            return JsonHelper.SerializeObject(response);
        }
        #endregion

        #region New Featured
        public string SearchNewFeatureds(string sort = "UpdatedOn", string order = "Asc", int first = 0, int count = 10)
        {
            var total = 0;
            var list = _updateClusterBusiness.SearchNewFeatureds(sort, order, first, count, out total);
            var result = new { total = total, data = list };
            return JsonHelper.SerializeObject(result);
        }

        [HttpPost]
        [ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public string SaveNewFeatured(UpdateClusterModel updateCluster)
        {
            var response = new PostFormResponse();
            OperationResult result = new OperationResult(OperationResultType.Success);
            NewFeaturedEntity entity = new NewFeaturedEntity();
            if (updateCluster.ID > 0)
            {
                entity = _updateClusterBusiness.GetNewFeatured(updateCluster.ID);
                entity.Description = updateCluster.Description;
                entity.Title = updateCluster.Title;
                entity.HyperLink = updateCluster.HyperLink;
                entity.ThumbnailPath = updateCluster.ThumbnailPath;
                entity.ThumbnailName = updateCluster.ThumbnailName;
                entity.UpdatedBy = UserInfo.ID;
                entity.UpdatedOn = DateTime.Now;
                result = _updateClusterBusiness.UpdateNewFeatured(entity);
            }
            else
            {
                entity.Description = updateCluster.Description;
                entity.Title = updateCluster.Title;
                entity.HyperLink = updateCluster.HyperLink;
                entity.ThumbnailPath = updateCluster.ThumbnailPath;
                entity.ThumbnailName = updateCluster.ThumbnailName;
                entity.CreatedBy = UserInfo.ID;
                entity.UpdatedBy = UserInfo.ID;
                entity.CreatedOn = DateTime.Now;
                entity.UpdatedOn = DateTime.Now;
                result = _updateClusterBusiness.InsertNewFeatured(entity);
            }
            response.Success = result.ResultType == OperationResultType.Success;
            response.Message = result.Message;
            return JsonHelper.SerializeObject(response);
        }
        public string DeleteNewFeatured(int id)
        {
            var response = new PostFormResponse();
            OperationResult result = new OperationResult(OperationResultType.Success);
            result = _updateClusterBusiness.DeleteNewFeatured(id);
            response.Success = result.ResultType == OperationResultType.Success;
            response.Message = result.Message;
            response.ModelState = ModelState;
            return JsonHelper.SerializeObject(response);
        }
        #endregion
    }
}