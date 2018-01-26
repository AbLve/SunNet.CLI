using Sunnet.Cli.Business;
using Sunnet.Cli.Business.Ade;
using Sunnet.Cli.Business.Ade.Models;
using Sunnet.Cli.Business.Common;
using Sunnet.Cli.Business.Cpalls.Models;
using Sunnet.Cli.Business.Users;
using Sunnet.Cli.Core.Ade;
using Sunnet.Cli.Core.Users.Entities;
using Sunnet.Cli.Core.Users.Enums;
using Sunnet.Cli.UIBase.Filters;
using Sunnet.Cli.UIBase.Models;
using Sunnet.Framework.Permission;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Sunnet.Framework.Resources;
using Sunnet.Cli.Business.Permission;
using Sunnet.Framework;
using Sunnet.Cli.Business.Communities;
using Sunnet.Cli.Business.Schools;
using Sunnet.Cli.Business.Classes;
namespace Sunnet.Cli.Assessment.Controllers
{
    internal class Test
    {
        public int Value { get; set; }
    }

    public class DashboardController : BaseController
    {
        AdeBusiness _adeBusiness;
        CommunityBusiness _community;
        SchoolBusiness _schoolBusiness;
        ClassBusiness _classBusiness;
        public DashboardController()
        {
            _adeBusiness = new AdeBusiness(AdeUnitWorkContext);
            _community = new CommunityBusiness(UnitWorkContext);
            _schoolBusiness = new SchoolBusiness(UnitWorkContext);
            _classBusiness = new ClassBusiness(UnitWorkContext);
        }

        // GET: /Dashboard/
        [CLIUrlAuthorizeAttribute(Account = Authority.All, PageId = PagesModel.Assessment, Anonymity = Anonymous.Verified)]
        public ActionResult Index(string showmessage = "")
        {
            List<int> accountPageId = new PermissionBusiness().CheckPage(UserInfo);

            ViewBag.ShowCPALLS = accountPageId.Contains((int)PagesModel.CPALLS);
            ViewBag.ShowCOT = accountPageId.Contains((int)PagesModel.COT);
            ViewBag.ShowCEC = accountPageId.Contains((int)PagesModel.CEC);
            ViewBag.ShowTCD = accountPageId.Contains((int)PagesModel.TCD); 
            List<int> pageIds = accountPageId.FindAll(r => r > SFConfig.AssessmentPageStartId);
            List<int> featureAssessmentIds = _community.GetFeatureAssessmentIds(UserInfo);

            HttpContext.Response.Cache.SetNoStore();
            ViewBag.School = false;
            if (!string.IsNullOrEmpty(showmessage))
            {
                ViewBag.Message = ResourceHelper.GetRM().GetInformation(showmessage);
            }
            List<CpallsAssessmentModel> list = _adeBusiness.GetAssessmentCpallsList();
            List<CpallsAssessmentModel> accessList = list.FindAll(r => pageIds.Contains(r.ID + SFConfig.AssessmentPageStartId));
            if (UserInfo.Role > Role.Mentor_coach)
                accessList = accessList.FindAll(r => featureAssessmentIds.Contains(r.ID));

            List<CpallsAssessmentModel> English = accessList.FindAll(r => r.Language == AssessmentLanguage.English);
            List<CpallsAssessmentModel> newList = new List<CpallsAssessmentModel>();

            foreach (CpallsAssessmentModel model in accessList)
            {
                if (model.Language == AssessmentLanguage.Spanish)
                {
                    if (English.Find(r => r.Name == model.Name) != null)
                        continue;
                }
                newList.Add(model);
            }

            ViewBag.List = newList;

            List<CpallsAssessmentModel> cecList = _adeBusiness.GetAssessmentList(o => o.Type == AssessmentType.Cec);
            List<CpallsAssessmentModel> accessCECList = cecList.FindAll(r => pageIds.Contains(r.ID + SFConfig.AssessmentPageStartId));
            if (UserInfo.Role > Role.Mentor_coach)
                accessCECList = accessCECList.FindAll(r => featureAssessmentIds.Contains(r.ID));
            ViewBag.CECList = accessCECList;

            List<CpallsAssessmentModel> cotList = _adeBusiness.GetAssessmentList(o => o.Type == AssessmentType.Cot);
            List<CpallsAssessmentModel> accessCOTList = cotList.FindAll(r => pageIds.Contains(r.ID + SFConfig.AssessmentPageStartId));
            if (UserInfo.Role > Role.Mentor_coach)
                accessCOTList = accessCOTList.FindAll(r => featureAssessmentIds.Contains(r.ID));
            ViewBag.COTList = accessCOTList;

            //Observable

            List<CpallsAssessmentModel> obsevList = _adeBusiness.GetAssessmentList(o => o.Type == AssessmentType.UpdateObservables);
            List<CpallsAssessmentModel> accessTCDList = obsevList.FindAll(r => pageIds.Contains(r.ID + SFConfig.AssessmentPageStartId));
            ViewBag.TCDList = accessTCDList;

            //if (UserInfo.IsCLIUser
            //    || UserInfo.Role == Role.Community
            //    || UserInfo.Role == Role.District_Community_Delegate

            //    || UserInfo.Role == Role.District_Community_Specialist
            //    || UserInfo.Role == Role.Community_Specialist_Delegate

            //    || UserInfo.Role == Role.Principal
            //    || UserInfo.Role == Role.Principal_Delegate
            //    )
            //{
            //    ViewBag.COTList = accessCOTList;
            //}
            //else
            //    ViewBag.COTList = new List<CpallsAssessmentModel>();




            _schoolBusiness.SetUserSchoolIdsCache(UserInfo);
            _classBusiness.SetClassIdCache(UserInfo);
            return View();
        }

    }
}