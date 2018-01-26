using Sunnet.Cli.Business.Users;
using Sunnet.Cli.Business.Users.Models;
using Sunnet.Cli.MainSite.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Sunnet.Framework.Extensions;
using Sunnet.Cli.UIBase.Models;
using Sunnet.Framework.Core.Tool;
using Sunnet.Cli.Core.Users.Enums;
using Sunnet.Framework.SSO;
using Sunnet.Cli.Business.Communities.Models;
using Sunnet.Cli.Business.Communities;
using Sunnet.Cli.UIBase;

namespace Sunnet.Cli.MainSite.Areas.Signup.Controllers
{
    public class CommunityController : BaseController
    {
        private readonly UserBusiness userBusiness;
        private readonly CommunityBusiness communityBusiness;
        public CommunityController()
        {
            userBusiness = new UserBusiness(UnitWorkContext);
            communityBusiness = new CommunityBusiness(UnitWorkContext);
        }
        //
        // GET: /Signup/Community/
        public ActionResult Index()
        {
            if (UserAuthentication != AuthenticationStatus.Login)
            {
                ViewBag.LoginUrl = BuilderLoginUrl();
                ViewBag.CLIUserLogin = BuilderLoginUrl(LoginUserType.UTACCESSMANAGER);
            }
            ViewBag.StateOptions = userBusiness.GetStates().ToSelectList(ViewTextHelper.DefaultStateText, "");
            ApplicantCommunityModel applicantCommunity = new ApplicantCommunityModel();
            return View(applicantCommunity);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public string CommunityApplicant(ApplicantCommunityModel applicantCommunity, string txtCommunity)
        {
            var response = new PostFormResponse();
            if (response.Success = ModelState.IsValid)
            {
                OperationResult result = new OperationResult(OperationResultType.Success);
                applicantCommunity.RoleType = Role.Community;
                int basicCommunityId = applicantCommunity.CommunityId.Value;
                CommunityModel communityModel = communityBusiness.IsVerified(applicantCommunity.CommunityId.Value);
                if (communityModel == null)   //如果选择的BasicCommunity没有认证,则不发送邮件，且跳转到确认页面
                {
                    applicantCommunity.CommunityId = 0;
                }
                else
                    applicantCommunity.CommunityId = communityModel.ID;
                result = userBusiness.CommunityApplicant(applicantCommunity, basicCommunityId, txtCommunity);
                response.Success = result.ResultType == OperationResultType.Success;
                response.Data = applicantCommunity;
                response.Message = result.Message;
            }
            response.ModelState = ModelState;
            return JsonHelper.SerializeObject(response);
        }

        public ActionResult Confirmation()
        {
            return View();
        }
    }
}