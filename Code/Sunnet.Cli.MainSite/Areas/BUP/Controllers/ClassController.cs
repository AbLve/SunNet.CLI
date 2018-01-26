using Newtonsoft.Json;
using StructureMap;
using Sunnet.Cli.Business.BUP;
using Sunnet.Cli.Core.BUP;
using Sunnet.Cli.Core.BUP.Entities;
using Sunnet.Cli.MainSite.Controllers;
using Sunnet.Cli.UIBase;
using Sunnet.Cli.UIBase.Filters;
using Sunnet.Cli.UIBase.Models;
using Sunnet.Framework.Log;
using Sunnet.Framework.Permission;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace Sunnet.Cli.MainSite.Areas.BUP.Controllers
{
    public class ClassController : BaseController
    {
        ISunnetLog _log = ObjectFactory.GetInstance<ISunnetLog>();
        private readonly BUPTaskBusiness _bupTaskBusiness;
        private readonly BUPProcessBusiness _bupProcessBusiness;

        public ClassController()
        {
            _bupTaskBusiness = new BUPTaskBusiness(UnitWorkContext);
            _bupProcessBusiness = new BUPProcessBusiness(UnitWorkContext);
        }

        [CLIUrlAuthorizeAttribute(Account = Authority.Index, PageId = PagesModel.BUP_Class, Anonymity = Anonymous.Verified)]
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [CLIUrlAuthorizeAttribute(Account = Authority.Index, PageId = PagesModel.BUP_Class, Anonymity = Anonymous.Verified)]
        public ActionResult DataProcess()
        {
            HttpPostedFileBase postFileBase = Request.Files["dataFile"];
            string errorMsg = _bupProcessBusiness.InvalidateFile(postFileBase);
            if (!string.IsNullOrEmpty(errorMsg))
            {
                ModelState.AddModelError("", errorMsg);
                return View("index");
            }

            string virtualPath = FileHelper.SaveProtectedFile(postFileBase, "data_Process");
            string uploadPath = FileHelper.GetProtectedFilePhisycalPath(virtualPath);
            DataTable dt = new DataTable();
            errorMsg = _bupProcessBusiness.InvalidateFile(uploadPath, 12, BUPType.Class, out dt);
            if (!string.IsNullOrEmpty(errorMsg))
            {
                ModelState.AddModelError("", errorMsg);
                return View("index");
            }

            string originFileName = Path.GetFileName(postFileBase.FileName);

            int identity = 0;
            errorMsg = _bupProcessBusiness.ProcessClass(dt, UserInfo.ID, originFileName, virtualPath, UserInfo, out identity);
            if (!string.IsNullOrEmpty(errorMsg))
            {
                ModelState.AddModelError("", errorMsg);
                return View("index");
            }

            return new RedirectResult("index");
        }


        [CLIUrlAuthorizeAttribute(Account = Authority.Index, PageId = PagesModel.BUP_Class, Anonymity = Anonymous.Verified)]
        public string BUPData(string sort, string order, int first, int count)
        {
            int total = 0;
            List<BUPTaskEntity> list = _bupTaskBusiness.GetBupTasks(BUPType.Class, UserInfo, sort, order, first, count, out total);
            return JsonHelper.SerializeObject(new { total = total, data = list });
        }

        [CLIUrlAuthorizeAttribute(Account = Authority.Index, PageId = PagesModel.BUP_Class, Anonymity = Anonymous.Verified)]
        public string Delete(int ID)
        {
            var response = new PostFormResponse();
            StringBuilder sql = new StringBuilder();
            sql.AppendFormat("delete BUP_Tasks where ID = {0};", ID)
                .AppendFormat(" delete BUP_Classes where TaskId = {0};", ID);
            _bupTaskBusiness.ExecuteSqlCommand(sql.ToString());
            response.Success = true;
            return JsonConvert.SerializeObject(response);
        }
    }
}