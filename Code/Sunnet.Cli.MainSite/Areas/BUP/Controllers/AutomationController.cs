using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using StructureMap;
using Sunnet.Framework.Encrypt;
using Sunnet.Framework.Helpers;
using Sunnet.Cli.Core.Communities.Entities;
using Sunnet.Cli.MainSite.Controllers;
using Sunnet.Cli.UIBase.Models;
using Sunnet.Cli.Business.Communities;
using Sunnet.Cli.Core.BUP.Entities;
using Sunnet.Cli.Business.BUP;
using Sunnet.Cli.Core.Common.Enums;
using Sunnet.Framework.Extensions;
using Sunnet.Framework.Core.Tool;
using Sunnet.Cli.Business.Communities.Enums;
using Sunnet.Cli.UIBase.Filters;
using Sunnet.Framework.Permission;
using Sunnet.Framework.SFTP;
using Sunnet.Framework.Log;

namespace Sunnet.Cli.MainSite.Areas.BUP.Controllers
{
    public class AutomationController : BaseController
    {
        private readonly CommunityBusiness _communityBusiness;
        private readonly AutomationSettingBusiness _automationSettingBusiness;
        ISunnetLog _log;

        public AutomationController()
        {
            _communityBusiness = new CommunityBusiness(UnitWorkContext);
            _automationSettingBusiness = new AutomationSettingBusiness(UnitWorkContext);
            _log = ObjectFactory.GetInstance<ISunnetLog>();
        }

        //
        // GET: /BUP/Automation/
        [CLIUrlAuthorizeAttribute(Account = Authority.All, PageId = PagesModel.BUP, Anonymity = Anonymous.Verified)]
        public ActionResult Index()
        {
            return View();
        }

        [CLIUrlAuthorizeAttribute(Account = Authority.All, PageId = PagesModel.BUP, Anonymity = Anonymous.Verified)]
        public string Search(string communityName = "", int communityId = -1, int status = -1,
            string sort = "ID", string order = "Desc", int first = 0, int count = 10)
        {
            var total = 0;
            var expression = PredicateHelper.True<AutomationSettingEntity>();
            if (communityId >= 1)
                expression = expression.And(o => o.CommunityId == communityId);
            else if (communityName != null && communityName.Trim() != string.Empty)
                expression = expression.And(o => o.Community.Name.Contains(communityName.Trim()));
            if (status >= 0)
                expression = expression.And(o => (int)o.Status == status);

            var list = _automationSettingBusiness.GetAutomationSettings(UserInfo, expression, sort, order, first, count, out total);
            var result = new { total = total, data = list };
            return JsonHelper.SerializeObject(result);
        }

        [CLIUrlAuthorizeAttribute(Account = Authority.All, PageId = PagesModel.BUP, Anonymity = Anonymous.Verified)]
        public string GetCommunitySelectListForSearch(string keyword, int communityId = -1, bool isActiveCommunity = true)
        {
            var expression = PredicateHelper.True<CommunityEntity>();
            if (communityId > 0)
                expression = expression.And(o => o.ID == communityId);
            var list = _communityBusiness.GetCommunitySelectList(UserInfo, expression, isActiveCommunity);
            return JsonHelper.SerializeObject(list);
        }

        [CLIUrlAuthorizeAttribute(Account = Authority.All, PageId = PagesModel.BUP, Anonymity = Anonymous.Verified)]
        public ActionResult New()
        {
            var expression = PredicateHelper.True<CommunityEntity>();
            expression = expression.And(o => o.CommunityAssessmentRelations.Any(c => c.AssessmentId == ((int)LocalAssessment.Automation)));
            expression = expression.And(o => o.Status == EntityStatus.Active);
            ViewBag.Communities = _communityBusiness.GetCommunitySelectList(UserInfo, expression)
                .ToSelectList(ViewTextHelper.DefaultPleaseSelectText, "");
            ViewBag.Status = EntityStatus.Active.ToSelectList();
            return View();
        }

        [HttpPost]
        [CLIUrlAuthorizeAttribute(Account = Authority.All, PageId = PagesModel.BUP, Anonymity = Anonymous.Verified)]
        public string New(AutomationSettingEntity entity)
        {
            entity.CreatedBy = UserInfo.ID;
            entity.UpdatedBy = UserInfo.ID;
            IEncrypt encrypt = ObjectFactory.GetInstance<IEncrypt>();
            entity.PassWord = encrypt.Encrypt(entity.PassWord);
            var response = new PostFormResponse();

            OperationResult result = _automationSettingBusiness.Insert(entity);
            response.Success = result.ResultType == OperationResultType.Success;
            response.Message = result.Message;

            return JsonHelper.SerializeObject(response);
        }

        [CLIUrlAuthorizeAttribute(Account = Authority.All, PageId = PagesModel.BUP, Anonymity = Anonymous.Verified)]
        public ActionResult ViewInfo(int id)
        {
            AutomationSettingEntity entity = _automationSettingBusiness.GetEntity(id);
            ViewBag.CommunityName = entity.Community == null ? "" : entity.Community.Name;
            return View(entity);
        }

        [CLIUrlAuthorizeAttribute(Account = Authority.All, PageId = PagesModel.BUP, Anonymity = Anonymous.Verified)]
        public ActionResult Edit(int id)
        {
            var expression = PredicateHelper.True<CommunityEntity>();
            expression = expression.And(o => o.CommunityAssessmentRelations.Any(c => c.AssessmentId == ((int)LocalAssessment.Automation)));
            expression = expression.And(o => o.Status == EntityStatus.Active);
            ViewBag.Communities = _communityBusiness.GetCommunitySelectList(UserInfo, expression)
                .ToSelectList(ViewTextHelper.DefaultPleaseSelectText, "");
            ViewBag.Status = EntityStatus.Active.ToSelectList();
            AutomationSettingEntity entity = _automationSettingBusiness.GetEntity(id);
            IEncrypt encrypt = ObjectFactory.GetInstance<IEncrypt>();
            entity.PassWord = encrypt.Decrypt(entity.PassWord);
            return View(entity);
        }

        [HttpPost]
        [CLIUrlAuthorizeAttribute(Account = Authority.All, PageId = PagesModel.BUP, Anonymity = Anonymous.Verified)]
        public string Edit(AutomationSettingEntity entity)
        {
            entity.UpdatedBy = UserInfo.ID;
            IEncrypt encrypt = ObjectFactory.GetInstance<IEncrypt>();
            entity.PassWord = encrypt.Encrypt(entity.PassWord);
            var response = new PostFormResponse();

            OperationResult result = _automationSettingBusiness.Update(entity);
            response.Success = result.ResultType == OperationResultType.Success;
            response.Message = result.Message;

            return JsonHelper.SerializeObject(response);
        }

        [HttpPost]
        [CLIUrlAuthorizeAttribute(Account = Authority.All, PageId = PagesModel.BUP, Anonymity = Anonymous.Verified)]
        public string TestConnect(string ip, string port, string username, string password)
        {
            int convertPort = 0;
            if (!string.IsNullOrEmpty(ip) && int.TryParse(port, out convertPort))
            {
                SFTPHelper sftp = new SFTPHelper(ip, int.Parse(port), username, password, _log);
                if (sftp.Connect())
                {
                    return "true";
                }
                else
                {
                    return "false";
                }
            }
            else
            {
                return "false";
            }

        }

    }
}