using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using Sunnet.Cli.Business.Users;
using Sunnet.Cli.Core.Users.Entities;
using Sunnet.Framework.Core.Tool;
using Sunnet.Framework.Extensions;
using Sunnet.Cli.Core.Users.Enums;
using Sunnet.Cli.MainSite.Controllers;
using Sunnet.Cli.UIBase.Models;
using Sunnet.Cli.Business.Common;
using Sunnet.Cli.Core.Common.Enums;
using Sunnet.Framework;
using Sunnet.Framework.Helpers;
using StructureMap;
using Sunnet.Framework.Encrypt;
using Sunnet.Framework.SSO;
using Sunnet.Cli.Business.Students;
using Sunnet.Cli.Core.Students.Entities;
using Sunnet.Cli.UIBase;
using Sunnet.Cli.Business.MasterData;

namespace Sunnet.Cli.MainSite.Areas.Signup.Controllers
{
    public class ParentController : BaseController
    {
        private readonly UserBusiness userBusiness;
        private readonly StudentBusiness studentBusiness;
        private readonly MasterDataBusiness _masterBusiness;
        public ParentController()
        {
            userBusiness = new UserBusiness(UnitWorkContext);
            studentBusiness = new StudentBusiness(UnitWorkContext);
            _masterBusiness = new MasterDataBusiness(UnitWorkContext);
        }
        //
        // GET: /Signup/Parent/
        public ActionResult Index()
        {
            string googleId;
            int userId;
            Authentication.ValidateGlobalCookie(out userId, out googleId);
            if (googleId.Trim() == string.Empty)
            {
                string loginUrl = string.Format("{0}home/Index?{1}", DomainHelper.SsoSiteDomain, BuilderLoginUrl(LoginUserType.GOOGLEACCOUNT, LoginIASID.ParentSign));
                return new RedirectResult(loginUrl);
            }

            if (UserAuthentication != AuthenticationStatus.Login)
            {
                ViewBag.LoginUrl = BuilderLoginUrl();
                ViewBag.CLIUserLogin = BuilderLoginUrl(LoginUserType.UTACCESSMANAGER);
            }
            ViewBag.RelationList = ParentRelation.Father.ToSelectList().AddDefaultItem(ViewTextHelper.DefaultPleaseSelectText, "");
            var States = _masterBusiness.GetStateSelectList().ToSelectList("State*", "");
            ViewBag.StateOptions = States;
            var list= new SelectList(_masterBusiness.GetAllCountries(), "ID", "Name");
      
            foreach (var item in list)
	        {
                if (item.Value == "188")
                {
                    item.Selected = true; 
                    break;
                }
	        }
            ViewBag.CountryList = list;
            ParentEntity parent = new ParentEntity();
            parent.CountryId = 188;
            return View(parent);
        }

        public ActionResult ParentRegister(ParentEntity parent, string parentCode, string childFirstName, string childLastName,List<int> chkSetting,string OtherSetting)
        {
            if (UserAuthentication != AuthenticationStatus.Login)
            {
                ViewBag.LoginUrl = BuilderLoginUrl();
                ViewBag.CLIUserLogin = BuilderLoginUrl(LoginUserType.UTACCESSMANAGER);
            }
            var response = new PostFormResponse();
          //  if (response.Success == ModelState.IsValid)
            {
                if (UserInfo == null)
                {
                    string googleId = "";
                    int userId = 0;
                    Authentication.ValidateGlobalCookie(out userId, out googleId);
                    parent.UserInfo.GoogleId = googleId;
                    parent.UserInfo.PrimaryNumberType = 0;
                    parent.UserInfo.SecondaryNumberType = 0;
                    parent.UserInfo.SecondaryPhoneNumber = "";
                    parent.UserInfo.SecondaryEmailAddress = "";
                    parent.UserInfo.Status = EntityStatus.Active;
                    parent.UserInfo.Role = Role.Parent;
                    parent.UserInfo.StatusDate = DateTime.Now;
                    parent.UserInfo.FaxNumber = "";
                    parent.UserInfo.MiddleName = "";
                    parent.UserInfo.PreviousLastName = "";
                    parent.ParentId = "";

                    if (parent.CountryId !=188)
                    {
                        parent.StateId = 0;
                    }
                    parent.SettingIds = string.Join(",",chkSetting);
                    parent.OtherSetting = OtherSetting;

                    OperationResult result = new OperationResult(OperationResultType.Success);
                    parent.UserInfo.Role = Role.Parent;
                    result = userBusiness.RegisterParent(parent, parentCode, childFirstName, childLastName);
                    response.Success = result.ResultType == OperationResultType.Success;
                    response.Data = parent;
                    if (response.Success)
                    {
                        UserBaseEntity user = parent.UserInfo;
                        if (user != null && user.Status == EntityStatus.Active)
                        {
                            LocalSignIn(user.ID, user.GoogleId, user.FirstName);
                        }
                        return new RedirectResult(string.Format("{0}home/Dashboard/sign-in", DomainHelper.MainSiteDomain.ToString()));
                    }
                }
                else
                {
                    return new RedirectResult(string.Format("{0}home/switches", DomainHelper.SsoSiteDomain.ToString()));
                }
            }
            return new RedirectResult(string.Format("{0}home/Signup", DomainHelper.MainSiteDomain.ToString()));
        }

        public int GetStudentByCode(string parentCode, string childFirstName, string childLastName)
        {
            StudentEntity student = studentBusiness.GetStudentByCode(parentCode, childFirstName, childLastName);
            if (student != null)
                return student.ID;
            return 0;
        }
    }
}