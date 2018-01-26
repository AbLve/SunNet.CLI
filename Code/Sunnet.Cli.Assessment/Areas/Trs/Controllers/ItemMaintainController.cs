using Sunnet.Cli.Assessment.Controllers;
using Sunnet.Cli.Business.Trs;
using Sunnet.Cli.Core;
using Sunnet.Cli.Core.Trs;
using Sunnet.Cli.Core.Trs.Entities;
using Sunnet.Cli.UIBase.Filters;
using Sunnet.Cli.UIBase.Models;
using Sunnet.Framework.Core.Tool;
using Sunnet.Framework.Permission;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Sunnet.Cli.Assessment.Areas.Trs.Controllers
{
    public class ItemMaintainController : BaseController
    {
        private TrsBusiness _trsBusiness;
        public ItemMaintainController()
        {
            _trsBusiness = new TrsBusiness(AdeUnitWorkContext);
        }
        //
        // GET: /Trs/ItemMaintain/
        [CLIUrlAuthorizeAttribute(Account = Authority.All, PageId = PagesModel.TRS, Anonymity = Anonymous.Verified)]
        public ActionResult Edit(int id)
        {
            TRSItemEntity item = new TRSItemEntity();
            if (id > 0)
            {
                item = _trsBusiness.GetItem(id);
            }
            return View(item);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        [CLIUrlAuthorizeAttribute(Account = Authority.All, PageId = PagesModel.TRS, Anonymity = Anonymous.Verified)]
        public string Edit(TRSItemEntity model)
        {
            TRSItemEntity entity = _trsBusiness.GetItem(model.ID);
            entity.Item = model.Item;
            entity.Text = model.Text;
            entity.Description = model.Description;
            var response = new PostFormResponse();
            OperationResult result = _trsBusiness.UpdateItem(entity);
            response.Success = result.ResultType == OperationResultType.Success;
            response.Message = result.Message;
            return JsonHelper.SerializeObject(response);
        }
    }
}