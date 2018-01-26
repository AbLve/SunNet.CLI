using Sunnet.Cli.Core.Users.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Sunnet.Framework.Extensions;
using Sunnet.Framework.Core.Tool;
using Sunnet.Cli.Business.Users;
using Sunnet.Cli.Business.Schools;
using Sunnet.Cli.Business.MasterData;
using Sunnet.Cli.MainSite.Controllers;
using Sunnet.Cli.Core.Users.Enums;
using Sunnet.Cli.UIBase.Models;
using Sunnet.Cli.Business.Users.Models;
using Sunnet.Cli.Business.Communities;
using Sunnet.Cli.Business.Communities.Models;
using Sunnet.Framework.SSO;
using Sunnet.Cli.UIBase;

namespace Sunnet.Cli.MainSite.Areas.Signup.Controllers
{
    public class PrincipalController : BaseController
    {
        private readonly UserBusiness userBusiness;
        private readonly SchoolBusiness schoolBusiness;
        private readonly CommunityBusiness communityBusiness;
        public PrincipalController()
        {
            userBusiness = new UserBusiness(UnitWorkContext);
            schoolBusiness = new SchoolBusiness(UnitWorkContext);
            communityBusiness = new CommunityBusiness(UnitWorkContext);
        }
        //
        // GET: /Signup/Principal/

        public ActionResult CommunityInvitedRegister(int applicantId, string communityName)
        {
            if (UserAuthentication != AuthenticationStatus.Login)
            {
                ViewBag.LoginUrl = BuilderLoginUrl();
                ViewBag.CLIUserLogin = BuilderLoginUrl(LoginUserType.UTACCESSMANAGER);
            }
            ViewBag.SchoolTypeOptions = schoolBusiness.GetSchoolTypeList(0).Select(o => new SelectListItem
            {
                Text = o.Name,
                Value = o.ID.ToString()
            }).AddDefaultItem(ViewTextHelper.DefaultPleaseSelectText, "");
            ViewBag.StateOptions = userBusiness.GetStates().ToSelectList(ViewTextHelper.DefaultStateText, "");
            ApplicantContactEntity applicantContact = new ApplicantContactEntity();
            applicantContact.ApplicantId = applicantId;
            applicantContact.CommunityName = communityName;
            applicantContact.CommunityId = 0;
            return View(applicantContact);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public string PrincipalApplicant(ApplicantTeacherModel applicantPrincipal, bool chkNoDistrict)
        {
            bool isCommunityVerified = true;
            var response = new PostFormResponse();
            if (response.Success = ModelState.IsValid)
            {
                OperationResult result = new OperationResult(OperationResultType.Success);
                applicantPrincipal.RoleType = Role.Principal;
                CommunityModel communityModel = communityBusiness.IsVerified(applicantPrincipal.CommunityId == null ? 0
                    : applicantPrincipal.CommunityId.Value);
                if (communityModel == null)   //如果选择的community没有认证,则不发送邮件，且跳转到确认页面
                {
                    chkNoDistrict = true;
                    isCommunityVerified = false;
                    applicantPrincipal.SchoolId = 0;
                }
                else
                    applicantPrincipal.CommunityId = communityModel.ID;
                result = userBusiness.PrincipalApplicant(applicantPrincipal, chkNoDistrict);
                response.Success = result.ResultType == OperationResultType.Success;
                response.Data = new { entity = applicantPrincipal, isCommunityVerified = isCommunityVerified };
                response.Message = result.Message;
            }
            response.ModelState = ModelState;
            return JsonHelper.SerializeObject(response);
        }

        public string InvitationCommunityRegister(ApplicantContactEntity applicantContact)
        {
            string emailTemplateName = "";
            var response = new PostFormResponse();
            if (response.Success = ModelState.IsValid)
            {
                OperationResult result = new OperationResult(OperationResultType.Success);
                if (applicantContact.SchoolTypeId == 1)//类型为 public 发送5A邮件
                {
                    emailTemplateName = "PrincipalApplicant_5A_Template.xml";
                }
                //Head Start or Child Care Center-Based 发送5B邮件，其余类别不发送邮件
                if (applicantContact.SchoolTypeId == 2 || applicantContact.SchoolTypeId == 3)   
                {
                    emailTemplateName = "PrincipalApplicant_5B_Template.xml";
                }

                result = userBusiness.SaveApplicantContact(applicantContact, emailTemplateName);
                response.Success = result.ResultType == OperationResultType.Success;
                response.Message = result.Message;
            }
            response.ModelState = ModelState;
            return JsonHelper.SerializeObject(response);
        }        
    }
}