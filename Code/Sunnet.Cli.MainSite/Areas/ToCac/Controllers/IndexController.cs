using System.IO;
using Newtonsoft.Json;
using StructureMap;
using Sunnet.Cli.Business.Communities;
using Sunnet.Cli.Business.Communities.Enums;
using Sunnet.Cli.Business.Permission;
using Sunnet.Cli.Business.StatusTracking;
using Sunnet.Cli.Business.Users;
using Sunnet.Cli.Core.Users.Enums;
using Sunnet.Cli.MainSite.Controllers;
using Sunnet.Cli.UIBase.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Sunnet.Cli.Business.Cpalls;
using Sunnet.Cli.Business.Schools;
using Sunnet.Cli.Core.CAC.Entities;
using Sunnet.Cli.Core.Common.Enums;
using Sunnet.Cli.UIBase.Models;
using Sunnet.Framework.Core.Tool;
using Sunnet.Framework.Encrypt;
using Sunnet.Framework.Log;
using Sunnet.Framework.Permission;
using Sunnet.Framework.SSO;
using Sunnet.Cli.Business.CAC.Models;
using Sunnet.Framework.Helpers;
using Sunnet.Framework.Extensions;
using System.Collections;
using System.Globalization;
using System.Text;
using Sunnet.Framework;
using Sunnet.Framework.PDF;

namespace Sunnet.Cli.MainSite.Areas.ToCac.Controllers
{
    public class IndexController : BaseController
    {
        // 
        ISunnetLog _log;
        IEncrypt _encrypt;
        UserBusiness _userBusiness;
        PermissionBusiness _permissionBusiness;
        StatusTrackingBusiness statusTrackingBusiness;
        CommunityBusiness _communityBusiness;
        CACBusiness _cacBusiness;
        public IndexController()
        {
            _userBusiness = new UserBusiness(UnitWorkContext);
            _log = ObjectFactory.GetInstance<ISunnetLog>();
            _encrypt = ObjectFactory.GetInstance<IEncrypt>();
            _permissionBusiness = new PermissionBusiness(UnitWorkContext);
            statusTrackingBusiness = new StatusTrackingBusiness(UnitWorkContext);
            _communityBusiness = new CommunityBusiness(UnitWorkContext);
            _cacBusiness = new CACBusiness(UnitWorkContext);
        }

        [CLIUrlAuthorizeAttribute(Account = Authority.All, PageId = PagesModel.CAC, Anonymity = Anonymous.Verified)]
        public ActionResult Index(string targetUrl = "")
        {
            HttpContext.Response.Cache.SetNoStore();
            if (UserAuthentication == AuthenticationStatus.LostSession)
                return new RedirectResult(string.Format("{0}home/LostSession", DomainHelper.SsoSiteDomain));
            if (UserAuthentication != AuthenticationStatus.Login)
                return new RedirectResult("/home/");
            if (CanCAC())
            {
                string cacUrl = GenerateCacUrl(targetUrl);
                if (String.IsNullOrEmpty(targetUrl))
                {
                    Response.Redirect("/");
                }
                else
                    Response.Redirect(cacUrl);
            }
            else
            {
                return new RedirectResult("/error/nonauthorized");
            }
            return View();
        }
        public bool CanCAC()
        {
            var canAccessCAC = _permissionBusiness.CheckMenu(UserInfo, (int)PagesModel.CAC);
            if (UserInfo.Role > Role.Mentor_coach)
            {
                int cacId = (int)LocalAssessment.CIRCLEActivityCollection;
                switch (UserInfo.Role)
                {
                    case Role.Statewide:
                    case Role.Community:
                    case Role.District_Community_Specialist:
                        canAccessCAC = canAccessCAC && UserInfo.UserCommunitySchools.Where(o => o.CommunityId > 0)
                            .Any(r => (r.Community.CommunityAssessmentRelations.Where(a => a.Isrequest == false).Select(a => a.AssessmentId).Contains(cacId)));
                        break;
                    case Role.District_Community_Delegate:
                    case Role.Community_Specialist_Delegate:
                        var parentCommunityUser = _userBusiness.GetUser(UserInfo.CommunityUser.ParentId);
                        canAccessCAC = canAccessCAC && parentCommunityUser.UserCommunitySchools.Where(o => o.CommunityId > 0)
                            .Any(r => (r.Community.CommunityAssessmentRelations.Where(a => a.Isrequest == false).Select(a => a.AssessmentId).Contains(cacId)));
                        break;
                    case Role.Principal:
                    case Role.School_Specialist:
                    case Role.TRS_Specialist:
                        canAccessCAC = canAccessCAC && UserInfo.UserCommunitySchools.Where(o => o.SchoolId > 0)
                            .Any(e => e.School.CommunitySchoolRelations
                                .Any(n => (n.Community.CommunityAssessmentRelations.Where(a => a.Isrequest == false).Select(o => o.AssessmentId).Contains(cacId))));
                        break;
                    case Role.Principal_Delegate:
                    case Role.School_Specialist_Delegate:
                    case Role.TRS_Specialist_Delegate:
                        var parentPrincipalUser = _userBusiness.GetUser(UserInfo.Principal.ParentId);
                        canAccessCAC = canAccessCAC && parentPrincipalUser.UserCommunitySchools.Where(o => o.SchoolId > 0)
                            .Any(e => e.School.CommunitySchoolRelations
                                .Any(n => (n.Community.CommunityAssessmentRelations.Where(a => a.Isrequest == false).Select(o => o.AssessmentId).Contains(cacId))));
                        break;
                    case Role.Teacher:
                        canAccessCAC = canAccessCAC && UserInfo.UserCommunitySchools.Where(o => o.SchoolId > 0)
                            .Any(e => e.School.CommunitySchoolRelations
                                .Any(n => (n.Community.CommunityAssessmentRelations.Where(a => a.Isrequest == false).Select(o => o.AssessmentId).Contains(cacId))));
                        break;
                }
            }
            return canAccessCAC;
        }
        public string GenerateCacUrl(string paramStr)
        {
            string urlStr = "";
            string paramStrs = "";
            CACUser cacUser = new CACUser();
            cacUser.LoginName = UserInfo.FirstName + UserInfo.ID;
            cacUser.FirstName = UserInfo.FirstName;
            cacUser.LastName = UserInfo.LastName;
            cacUser.Email = UserInfo.Gmail;
            cacUser.TimeNow = DateTime.Now;
            cacUser.RoleName = UserInfo.Role.ToString();
            cacUser.CliUserId = Encrypt(UserInfo.ID.ToString());
            paramStrs = "?key=" + Encrypt(HttpUtility.UrlEncode(JsonConvert.SerializeObject(cacUser)));
            if (!String.IsNullOrWhiteSpace(paramStr))
            {
                paramStrs += "&targetUrl=" + paramStr;
            }

            urlStr = Path.Combine(DomainHelper.CACDomain.AbsoluteUri, paramStrs);
            return urlStr;
        }

        private string Encrypt(string source)
        {
            if (string.IsNullOrEmpty(source))
                return source;
            var encrypt = ObjectFactory.GetInstance<IEncrypt>();
            return encrypt.Encrypt(source);
        }
        private string Descrypt(string source)
        {
            if (string.IsNullOrEmpty(source))
                return source;
            var encrypt = ObjectFactory.GetInstance<IEncrypt>();
            return encrypt.Decrypt(source);
        }
        [HttpPost]
        public string AddMyActivity(int a, string b, string c, string d, string e, string f, string g, string h)
        {
            var response = new PostFormResponse();
            var res = new OperationResult(OperationResultType.Success);
            var userIdStr = Descrypt(c);
            var userId = 0;
            int.TryParse(userIdStr, out userId);
            var user = _userBusiness.GetUser(userId);
            var domain = Server.UrlDecode(e);
            var subDomain = Server.UrlDecode(f);
            if (user != null && UserInfo == null)
            {
                UserInfo = user;
            }
            if (CanCAC() && UserInfo != null)
            {
                var bus = new CACBusiness(UnitWorkContext);
                var myActivity = new MyActivityEntity();
                myActivity.UserId = UserInfo.ID;
                myActivity.ActivityId = a;
                myActivity.ActivityName = Server.UrlDecode(b);
                myActivity.CreatedBy = UserInfo.ID;
                myActivity.Status = EntityStatus.Active;
                myActivity.UpdatedBy = UserInfo.ID;
                myActivity.Url = Server.UrlDecode(d);
                if (domain == "")
                {
                    myActivity.Domain = subDomain;
                    myActivity.SubDomain = "";

                }
                else
                {
                    myActivity.Domain = domain;
                    myActivity.SubDomain = subDomain;
                }
                string url = myActivity.Url.ToLower();
                if (url.Contains("pre-k/en"))
                {
                    myActivity.CollectionType = "Pre-K English";
                }
                else if (url.Contains("pre-k/sp"))
                {
                    myActivity.CollectionType = "Pre-K Spanish";
                }
                else if (url.Contains("family/en"))
                {
                    myActivity.CollectionType = "Parents English";
                }
                else if (url.Contains("family/sp"))
                {
                    myActivity.CollectionType = "Parents Spanish";
                }
                else if (url.Contains("infant-toddler/en"))
                {
                    myActivity.CollectionType = "Infant & Toddler English";
                }
                else if (url.Contains("infant-toddler/sp"))
                {
                    myActivity.CollectionType = "Infant & Toddler Spanish";
                }
                myActivity.Objective = Server.UrlDecode(g);
                myActivity.AgeGroup = Server.UrlDecode(h);

                res = bus.AddMyActivity(myActivity);
            }
            else
            {
                res.Message = "Access Denied";
                res.ResultType = OperationResultType.Error;
            }
            response.Success = res.ResultType == OperationResultType.Success;
            response.Message = res.Message;
            return JsonConvert.SerializeObject(response);
        }

        [HttpPost]
        public string RemoveMyActivity(int a, string c)
        {
            var response = new PostFormResponse();
            var res = new OperationResult(OperationResultType.Success);
            var userIdStr = Descrypt(c);
            var userId = 0;
            int.TryParse(userIdStr, out userId);
            var user = _userBusiness.GetUser(userId);
            if (user != null && UserInfo == null)
            {
                UserInfo = user;
            }
            if (CanCAC() && UserInfo != null)
            {
                var item = _cacBusiness.GetMyActivity(a, userId);
                if (item != null)
                {
                    CpallsBusiness cpallsBUS = new CpallsBusiness();
                    cpallsBUS.DeleteGroupActivities(item.ID, UserInfo.ID);//删除my activity 的同时 删除 Group 相关的activity
                    res = _cacBusiness.DeleteActivity(item);
                }
                else
                {
                    res.Message = "My Activity is null.";
                    res.ResultType = OperationResultType.Error;
                }

            }
            else
            {
                res.Message = "Access Denied";
                res.ResultType = OperationResultType.Error;
            }
            response.Success = res.ResultType == OperationResultType.Success;
            response.Message = res.Message;
            return JsonConvert.SerializeObject(response);
        }

        [HttpPost]
        public string IsMyActivity(int a, string b, string c)
        {
            var response = new PostFormResponse();
            var res = new OperationResult(OperationResultType.Success);
            var userIdStr = Descrypt(c);
            var userId = 0;
            int.TryParse(userIdStr, out userId);
            var user = _userBusiness.GetUser(userId);
            if (user != null && UserInfo == null)
            {
                UserInfo = user;
            }
            if (CanCAC() && UserInfo != null)
            {
                var bus = new CACBusiness(UnitWorkContext);
                try
                {
                    ActivityHistoryEntity history = new ActivityHistoryEntity();
                    history.ActivityId = a;
                    history.ActivityName = Server.UrlDecode(b);
                    history.EngageID = user.ID;
                    history.GoogleID = user.GoogleId;
                    history.Remark = "";
                    bus.AddActivityHistory(history);

                    var item = bus.GetMyActivity(a, Server.UrlDecode(b), userId);
                    if (item != null)
                    {


                        res.Message = item.Remark;
                        res.ResultType = OperationResultType.Success;
                    }
                    else
                    {
                        res.ResultType = OperationResultType.Error;
                        res.Message = "My Activity is null.";
                    }
                }
                catch (Exception ex)
                {
                    res.ResultType = OperationResultType.Error;
                    res.Message = ex.Message;
                }


            }
            else
            {
                res.Message = "Access Denied";
                res.ResultType = OperationResultType.Error;
            }
            response.Success = res.ResultType == OperationResultType.Success;
            response.Message = res.Message;
            return JsonConvert.SerializeObject(response);
        }


        [HttpPost]
        public string AddMyActivityNote(int a, string b, string c, string d)
        {
            var response = new PostFormResponse();
            var res = new OperationResult(OperationResultType.Success);
            var userIdStr = Descrypt(c);
            var userId = 0;
            int.TryParse(userIdStr, out userId);
            var user = _userBusiness.GetUser(userId);
            if (user != null && UserInfo == null)
            {
                UserInfo = user;
            }
            if (CanCAC() && UserInfo != null)
            {
                var bus = new CACBusiness(UnitWorkContext);
                try
                {
                    var item = bus.GetMyActivity(a, Server.UrlDecode(b), userId);
                    if (item != null)
                    {
                        item.Remark = d;
                        res = bus.UpdateMyActivity(item);
                    }
                    else
                    {
                        res.ResultType = OperationResultType.Error;
                        res.Message = "My Activity is null.";
                    }
                }
                catch (Exception ex)
                {
                    res.ResultType = OperationResultType.Error;
                    res.Message = ex.Message;
                }


            }
            else
            {
                res.Message = "Access Denied";
                res.ResultType = OperationResultType.Error;
            }
            response.Success = res.ResultType == OperationResultType.Success;
            response.Message = res.Message;
            return JsonConvert.SerializeObject(response);
        }

        #region MyActivity功能模块

        public ActionResult MyActivities()
        {
            return View();
        }

        [CLIUrlAuthorize(Account = Authority.All, PageId = PagesModel.CAC, Anonymity = Anonymous.Verified)]
        public string SearchMyActivities(string collectionType, string domain = "ALL DOMAINS", string sort = "ActivityName", string order = "Asc", int first = 0, int count = 5)
        {
            int total = 0;
            var expression = PredicateHelper.True<MyActivityEntity>();
            expression = expression.And(it => it.UserId == UserInfo.ID);
            expression = expression.And(it => it.CollectionType == collectionType);
            if (domain != "ALL DOMAINS")
                expression = expression.And(it => it.Domain.Contains(domain) || it.SubDomain.Contains(domain));
            List<MyActivityViewModel> list = _cacBusiness.GetMyActivitysPageList(expression, sort, order, first, count, out total);
            var domainList = _cacBusiness.GetAllDomain(collectionType, UserInfo.ID);
            var domainField = "LEARNING DOMAIN";
            var subDomainField = "SECONDARY LEARNING DOMAIN";
            var allDomainField = "ALL DOMAINS";
            switch (collectionType)
            {
                case "Pre-K English":
                    domainField = "LEARNING DOMAIN";
                    subDomainField = "SUBDOMAIN";
                    allDomainField = "ALL DOMAINS";
                    break;
                case "Infant & Toddler English":
                    domainField = "PRIMARY LEARNING DOMAIN";
                    subDomainField = "PRIMARY SUBDOMAIN";
                    allDomainField = "ALL DOMAINS";
                    break;
                case "Parents English":
                    domainField = "PRIMARY LEARNING DOMAIN";
                    subDomainField = "SECONDARY LEARNING DOMAIN";
                    allDomainField = "ALL DOMAINS";
                    break;
                case "Pre-K Spanish":
                    domainField = "DOMINIO";
                    subDomainField = "SUBDOMINIO";
                    allDomainField = "TODOS LOS DOMINIOS";
                    break;
                case "Infant & Toddler Spanish":
                    domainField = "PRIMER DOMINIO";
                    subDomainField = "PRIMER SUBDOMINIO";
                    allDomainField = "TODOS LOS DOMINIOS";
                    break;
                case "Parents Spanish":
                    domainField = "PRIMER ÁREA";
                    subDomainField = "SEGUNDO ÁREA";
                    allDomainField = "TODAS LAS AREAS";
                    break;
            }
            return JsonHelper.SerializeObject(new
            {
                total = total,
                data = list,
                domains = domainList.Where(e => e != "").Distinct(),
                IsEnglish = collectionType.Contains("English"),
                domainField = domainField,
                subDomainField = subDomainField,
                allDomainField= allDomainField
            }, "MM/dd/yyyy HH:mm");
        }

        [CLIUrlAuthorize(Account = Authority.All, PageId = PagesModel.CAC, Anonymity = Anonymous.Verified)]
        public ActionResult MyActivitiesIndex()
        {
            var collectionTypes = new List<string>() { "Pre-K English", "Pre-K Spanish", "Parents English", "Parents Spanish", "Infant & Toddler English", "Infant & Toddler Spanish" };
            ViewBag.CollectionTypeSource = collectionTypes.Select(item => new SelectListItem() { Text = item, Value = item }).ToList();
            ViewBag.CACDomain = SFConfig.CACDomain;
            return View("MyActivityIndex");
        }

        [CLIUrlAuthorize(Account = Authority.All, PageId = PagesModel.CAC, Anonymity = Anonymous.Verified)]
        public string GetMyActivitysPageList(MyActivityQueryModel query, string sort = "ActivityName", string order = "Asc", int first = 0, int count = 10)
        {
            int total = 0;
            var whereExpr = PredicateHelper.True<MyActivityEntity>();
            whereExpr = whereExpr.And(it => it.UserId == UserInfo.ID);

            if (!query.SearchCollectionType.IsNullOrEmpty())
                whereExpr = whereExpr.And(it => it.CollectionType == query.SearchCollectionType);

            if (!query.SearchActivityName.IsNullOrEmpty())
                whereExpr = whereExpr.And(it => it.ActivityName.ToLower().Contains(query.SearchActivityName.ToLower()));

            if (!query.SearchDomainOrSubDomain.IsNullOrEmpty())
                whereExpr = whereExpr.And(it => it.Domain.ToLower().Contains(query.SearchDomainOrSubDomain.ToLower()) || it.SubDomain.ToLower().Contains(query.SearchDomainOrSubDomain.ToLower()));

            List<MyActivityViewModel> target = _cacBusiness.GetMyActivitysPageList(whereExpr, sort, order, first, count, out total);
            return JsonHelper.SerializeObject(new
            {
                total = total,
                data = target
            }, "MM/dd/yyyy HH:mm");
        }

        [HttpPost]
        public string DeleteMyActivity(int id)
        {
            var response = new PostFormResponse();
            var res = new OperationResult(OperationResultType.Success);
            var item = _cacBusiness.GetMyActivitieById(id);
            CpallsBusiness cpallsBUS = new CpallsBusiness();
            cpallsBUS.DeleteGroupActivities(item.ID, UserInfo.ID); //删除my activity 的同时 删除 Group 相关的activity
            res = _cacBusiness.DeleteActivity(item);
            response.Success = res.ResultType == OperationResultType.Success;
            response.Message = res.Message;
            return JsonConvert.SerializeObject(response);
        }

        [HttpPost]
        public string SaveMyNote(int id, string note)
        {
            var response = new PostFormResponse();
            var res = new OperationResult(OperationResultType.Success);
            var myActivity = _cacBusiness.GetMyActivitieById(id);
            myActivity.Remark = note;
            res = _cacBusiness.UpdateMyActivity(myActivity);
            response.Success = res.ResultType == OperationResultType.Success;
            response.Message = res.Message;
            return JsonConvert.SerializeObject(response);
        }

        public ActionResult MyActivitiesPdf(string collectionType, string domain = "ALL DOMAINS",
            string sort = "ActivityName", string order = "Asc")
        {
            int total = 0;
            var expression = PredicateHelper.True<MyActivityEntity>();
            expression = expression.And(it => it.UserId == UserInfo.ID);
            expression = expression.And(it => it.CollectionType == collectionType);
            if (domain != "ALL DOMAINS")
                expression = expression.And(it => it.Domain.Contains(domain) || it.SubDomain.Contains(domain));
            List<MyActivityViewModel> list = _cacBusiness.GetMyActivitysPageList(expression, sort, order, 0,
                int.MaxValue, out total);
            ViewBag.MyActivities = list;

            var domainField = "LEARNING DOMAIN";
            var subDomainField = "SECONDARY LEARNING DOMAIN";
            switch (collectionType)
            {
                case "Pre-K English":
                    domainField = "LEARNING DOMAIN";
                    subDomainField = "SUBDOMAIN";
                    break;
                case "Infant & Toddler English":
                    domainField = "PRIMARY LEARNING DOMAIN";
                    subDomainField = "PRIMARY SUBDOMAIN";
                    break;
                case "Parents English":
                    domainField = "PRIMARY LEARNING DOMAIN";
                    subDomainField = "SECONDARY LEARNING DOMAIN";
                    break;
                case "Pre-K Spanish":
                    domainField = "DOMINIO";
                    subDomainField = "SUBDOMINIO";
                    break;
                case "Infant & Toddler Spanish":
                    domainField = "PRIMER DOMINIO";
                    subDomainField = "PRIMER SUBDOMINIO";
                    break;
                case "Parents Spanish":
                    domainField = "PRIMER ÁREA";
                    subDomainField = "SEGUNDO ÁREA";
                    break;
            }
            ViewBag.IsEnglish = collectionType.Contains("English");
            ViewBag.DomainField = domainField;
            ViewBag.SubDomainField = subDomainField;
            GetPdf(GetViewHtml("MyActivitiesPdf"), "MyActivities.pdf");
            return View();
        }

        private string GetViewHtml(string viewName)
        {
            ViewBag.Pdf = true;
            var resultHtml = "";
            ViewEngineResult result = ViewEngines.Engines.FindView(ControllerContext, viewName, null);
            if (null == result.View)
            {
                throw new InvalidOperationException(FormatErrorMessage(viewName, result.SearchedLocations));
            }
            try
            {
                ViewContext viewContext = new ViewContext(ControllerContext, result.View, this.ViewData, this.TempData,
                    Response.Output);
                var textWriter = new StringWriter();
                result.View.Render(viewContext, textWriter);
                resultHtml = textWriter.ToString();
            }
            finally
            {
                result.ViewEngine.ReleaseView(ControllerContext, result.View);
            }
            return resultHtml;
        }

        private void GetPdf(string html, string fileName, PdfType type = PdfType.Assessment_Landscape)
        {
            PdfProvider pdfProvider = new PdfProvider(type);
            pdfProvider.GeneratePDF(html, fileName);
        }

        private string FormatErrorMessage(string viewName, IEnumerable<string> searchedLocations)
        {
            string format =
                "The view '{0}' or its master was not found or no view engine supports the searched locations. The following locations were searched:{1}";
            StringBuilder builder = new StringBuilder();
            foreach (string str in searchedLocations)
            {
                builder.AppendLine();
                builder.Append(str);
            }
            return string.Format(CultureInfo.CurrentCulture, format, viewName, builder);
        }
        #endregion

    }
}