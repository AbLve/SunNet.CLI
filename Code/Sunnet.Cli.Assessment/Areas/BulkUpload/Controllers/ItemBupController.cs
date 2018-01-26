using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
/**************************************************************************
 * Developer: 		Joe
 * Computer:		JOE-PC
 * Domain:			Joe
 * CreatedOn:		2015/12/16
 * Description:		
 * Version History:	Created,2015/12/16
 * 
 * 
 **************************************************************************/
using Sunnet.Cli.Assessment.Controllers;
using Sunnet.Cli.UIBase.Models;
using Sunnet.Framework.Helpers;
using Sunnet.Cli.Core.Ade.Entities;
using Sunnet.Cli.Business.Ade;
using Sunnet.Cli.Core.Common.Enums;
using Sunnet.Cli.UIBase;
using System.Data;
using Sunnet.Cli.Core.Ade.Enums;
using System.IO;
using Sunnet.Cli.Business.Ade.Models;
using Sunnet.Cli.UIBase.Filters;
using Sunnet.Framework.Permission;
using Newtonsoft.Json;
using Sunnet.Framework.Core.Tool;
using Sunnet.Cli.Core.Ade;

namespace Sunnet.Cli.Assessment.Areas.BulkUpload.Controllers
{
    public class ItemBupController : BaseController
    {
        private AdeBusiness _adeBusiness;
        public ItemBupController()
        {
            _adeBusiness = new AdeBusiness(AdeUnitWorkContext);
        }

        // GET: BulkUpload/ItemBup
        [CLIUrlAuthorizeAttribute(Account = Authority.All, PageId = PagesModel.BulkUpload, Anonymity = Anonymous.Verified)]
        public ActionResult Index()
        {
            return View();
        }

        [CLIUrlAuthorizeAttribute(Account = Authority.All, PageId = PagesModel.BulkUpload, Anonymity = Anonymous.Verified)]
        public ActionResult UploadList(string type)
        {
            if (type != ItemType.TxkeaReceptive.ToString() && type != ItemType.TxkeaExpressive.ToString())
                return View("Index");

            if (type == ItemType.TxkeaReceptive.ToString())
                ViewBag.Type = ItemType.TxkeaReceptive;
            if (type == ItemType.TxkeaExpressive.ToString())
                ViewBag.Type = ItemType.TxkeaExpressive;
            return View();
        }

        [CLIUrlAuthorizeAttribute(Account = Authority.All, PageId = PagesModel.BulkUpload, Anonymity = Anonymous.Verified)]
        public ActionResult UploadReceptive(int AssessmentId, int MeasureId, string ResourcePath)
        {
            ViewBag.Type = ItemType.TxkeaReceptive;
            AssessmentEntity assessment = _adeBusiness.GetAssessment(AssessmentId);
            if (assessment == null)
            {
                ModelState.AddModelError("", "Assessment does not exist.");
                return View("UploadList");
            }
            HttpPostedFileBase postFileBase = Request.Files["dataFile"];
            string errorMsg = _adeBusiness.InvalidateFile(postFileBase);
            if (!string.IsNullOrEmpty(errorMsg))
            {
                ModelState.AddModelError("", errorMsg);
                return View("UploadList");
            }

            string virtualPath = FileHelper.SaveProtectedFile(postFileBase, "data_TxkeaBup");
            string uploadPath = FileHelper.GetProtectedFilePhisycalPath(virtualPath);
            string originFileName = Path.GetFileName(postFileBase.FileName);

            DataTable dt = new DataTable();
            errorMsg = _adeBusiness.InvalidateFile(uploadPath, 27, TxkeaBupType.TxkeaReceptive, out dt);
            if (!string.IsNullOrEmpty(errorMsg))
            {
                ModelState.AddModelError("", errorMsg);
                return View("UploadList");
            }

            errorMsg = _adeBusiness.ProcessReceptiveItem(dt, UserInfo.ID, originFileName, virtualPath, AssessmentId, MeasureId, ResourcePath);
            if (!string.IsNullOrEmpty(errorMsg))
            {
                ModelState.AddModelError("", errorMsg);
                return View("UploadList");
            }

            return new RedirectResult("UploadList?type=" + ItemType.TxkeaReceptive);
        }

        [CLIUrlAuthorizeAttribute(Account = Authority.All, PageId = PagesModel.BulkUpload, Anonymity = Anonymous.Verified)]
        public ActionResult UploadExpressive(int AssessmentId, int MeasureId, string ResourcePath)
        {
            ViewBag.Type = ItemType.TxkeaExpressive;
            AssessmentEntity assessment = _adeBusiness.GetAssessment(AssessmentId);
            if (assessment == null)
            {
                ModelState.AddModelError("", "Assessment does not exist.");
                return View("UploadList");
            }
            HttpPostedFileBase postFileBase = Request.Files["dataFile"];
            string errorMsg = _adeBusiness.InvalidateFile(postFileBase);
            if (!string.IsNullOrEmpty(errorMsg))
            {
                ModelState.AddModelError("", errorMsg);
                return View("UploadList");
            }

            string virtualPath = FileHelper.SaveProtectedFile(postFileBase, "data_TxkeaBup");
            string uploadPath = FileHelper.GetProtectedFilePhisycalPath(virtualPath);
            string originFileName = Path.GetFileName(postFileBase.FileName);

            DataTable dt = new DataTable();
            errorMsg = _adeBusiness.InvalidateFile(uploadPath, 40, TxkeaBupType.TxkeaExpressive, out dt);
            if (!string.IsNullOrEmpty(errorMsg))
            {
                ModelState.AddModelError("", errorMsg);
                return View("UploadList");
            }

            errorMsg = _adeBusiness.ProcessExpressiveItem(dt, UserInfo.ID, originFileName, virtualPath, AssessmentId, MeasureId, ResourcePath);
            if (!string.IsNullOrEmpty(errorMsg))
            {
                ModelState.AddModelError("", errorMsg);
                return View("UploadList");
            }

            return new RedirectResult("UploadList?type=" + ItemType.TxkeaExpressive);
        }

        [CLIUrlAuthorizeAttribute(Account = Authority.All, PageId = PagesModel.BulkUpload, Anonymity = Anonymous.Verified)]
        public string Search(string fileName, bool isReceptive, string sort = "ID", string order = "Desc", int first = 0, int count = 10)
        {
            var total = 0;
            var expression = PredicateHelper.True<TxkeaBupTaskEntity>();
            if (!string.IsNullOrEmpty(fileName))
                expression = expression.And(r => r.OriginFileName.Contains(fileName));
            if (isReceptive) //类型为Receptive
                expression = expression.And(r => r.Type == TxkeaBupType.TxkeaReceptive);
            else
                expression = expression.And(r => r.Type == TxkeaBupType.TxkeaExpressive);

            var list = _adeBusiness.SearchTxkeaBupTasks(expression, out total, sort, order, first, count);

            var result = new { total = total, data = list };
            return JsonHelper.SerializeObject(result);
        }

        [CLIUrlAuthorizeAttribute(Account = Authority.All, PageId = PagesModel.BulkUpload, Anonymity = Anonymous.Verified)]
        public ActionResult ViewDetail(int taskId)
        {
            if (taskId > 0)
            {
                TxkeaBupTaskModel taskEntity = _adeBusiness.GetTxkeaBupTaskModel(taskId);
                if (taskEntity != null)
                {
                    return View(taskEntity);
                }
            }
            return View();
        }

        [CLIUrlAuthorizeAttribute(Account = Authority.All, PageId = PagesModel.BulkUpload, Anonymity = Anonymous.Verified)]
        public string DeleteTask(int ID)
        {
            var response = new PostFormResponse();
            response.Success = false;
            if (ID > 0)
            {
                TxkeaBupTaskEntity entity = _adeBusiness.GetTxkeaBupTask(ID);
                if (entity != null)
                {
                    entity.IsDeleted = true;
                    OperationResult result = _adeBusiness.UpdateBupTask(entity);
                    response.Update(result);
                }
            }
            return JsonConvert.SerializeObject(response);
        }
    }
}