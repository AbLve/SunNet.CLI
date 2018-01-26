using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using Sunnet.Cli.Assessment.Controllers;
using Sunnet.Cli.Business.Cpalls.Models;
using Sunnet.Cli.Business.Ade;
using Sunnet.Framework;
using Sunnet.Cli.Business.Permission;
using Sunnet.Cli.Core.Users.Enums;
using Sunnet.Cli.Business.Communities;
using Sunnet.Cli.Core.Ade;
using Sunnet.Cli.Business.Schools;
using Sunnet.Framework.Helpers;
using Sunnet.Cli.Core.Communities.Entities;
using Sunnet.Cli.Core.Schools.Entities;
using Sunnet.Cli.UIBase.Models;
using Sunnet.Cli.Business.Tsds.Models;


namespace Sunnet.Cli.Assessment.Areas.Tsds.Controllers
{
    public class IndexController : BaseController
    {
        private readonly AdeBusiness _adeBusiness;
        private readonly CommunityBusiness _communityBusiness;
        private readonly SchoolBusiness _schoolBusiness;

        public IndexController()
        {
            _adeBusiness = new AdeBusiness(AdeUnitWorkContext);
            _communityBusiness = new CommunityBusiness(UnitWorkContext);
            _schoolBusiness = new SchoolBusiness(UnitWorkContext);
        }

        public ActionResult Dashboard()
        {
            List<int> featureAssessmentIds = _communityBusiness.GetFeatureAssessmentIds(UserInfo);
            List<int> accountPageId = new PermissionBusiness().CheckPage(UserInfo);
            List<CpallsAssessmentModel> list = _adeBusiness.GetAssessmentCpallsList();
            List<int> pageIds = accountPageId.FindAll(r => r > SFConfig.AssessmentPageStartId);
            List<CpallsAssessmentModel> accessList = list.FindAll(r => pageIds.Contains(r.ID + SFConfig.AssessmentPageStartId));
            if (UserInfo.Role > Role.Mentor_coach)
                accessList = accessList.FindAll(r => featureAssessmentIds.Contains(r.ID));
            List<CpallsAssessmentModel> English = accessList.FindAll(r => r.Language == AssessmentLanguage.English);
            List<SelectListItem> assessmentList = new List<SelectListItem>();

            foreach (CpallsAssessmentModel model in accessList)
            {
                if (model.Language == AssessmentLanguage.Spanish)
                {
                    if (English.Find(r => r.Name == model.Name) != null)
                        continue;
                }
                assessmentList.Add(new SelectListItem { Text = model.Name, Value = model.ID.ToString() });
            }
            ViewBag.Assessments = assessmentList;
            return View();
        }

        // GET: Tsds/Index
        public ActionResult DownloadIndex(int assessmentId)
        {
            ViewBag.Measures = new List<SelectListItem>();

            List<SelectListItem> communityList = new List<SelectListItem>();
            IEnumerable<CommunitySelectItemModel> communityModelList =
                _communityBusiness.GetCommunitySelectList(UserInfo, PredicateHelper.True<CommunityEntity>(), true);
            foreach (CommunitySelectItemModel item in communityModelList)
            {
                communityList.Add(new SelectListItem { Text = item.Name, Value = item.ID.ToString() });
            }
            ViewBag.Communities = communityList;

            List<SelectListItem> schoolList = new List<SelectListItem>();
            IEnumerable<SchoolSelectItemModel> schoolModelList =
                _schoolBusiness.GetSchoolsSelectList(UserInfo, PredicateHelper.True<SchoolEntity>(), true);
            foreach (SchoolSelectItemModel item in schoolModelList)
            {
                schoolList.Add(new SelectListItem { Text = item.Name, Value = item.ID.ToString() });
            }
            ViewBag.Schools = schoolList;

            return View();
        }

        public string Search(string fileName = "", int fileId = 0, string communityName = "", int communityId = 0,
           string schoolName = "", int schoolId = 0, string userName = "", int userId = 0)
        {
            List<DownloadListModel> list = new List<DownloadListModel>();
            var result = new { total = 0, data = list };
            return JsonHelper.SerializeObject(result);
        }

        /// <summary>
        /// 返回Community搜索框的值
        /// </summary>
        /// <param name="exception"></param>
        /// <returns></returns>
        public string GetCommunitySelectListForSearch()
        {
            var expression = PredicateHelper.True<CommunityEntity>();
            var list = _communityBusiness.GetCommunitySelectList(UserInfo, expression, true);
            return JsonHelper.SerializeObject(list);
        }

        /// <summary>
        /// 返回school搜索框的值
        /// </summary>
        /// <param name="communityId"></param>
        /// <param name="schoolName"></param>
        /// <returns></returns>
        public string GetSchoolSelectList(int communityId = 0)
        {
            var expression = PredicateHelper.True<SchoolEntity>();
            if (communityId > 0)
                expression = expression.And(o => o.CommunitySchoolRelations.Count(r => r.CommunityId == communityId) > 0);
            var schoolList = _schoolBusiness.GetSchoolsSelectList(UserInfo, expression, true);
            return JsonHelper.SerializeObject(schoolList);
        }

        public string GetFileList()
        {
            List<SelectListItem> list = new List<SelectListItem>();
            list.Add(new SelectListItem { Text = "File1", Value = "1" });
            list.Add(new SelectListItem { Text = "File2", Value = "2" });
            return JsonHelper.SerializeObject(list);
        }

        public string GetDowndedUserList()
        {
            List<SelectListItem> list = new List<SelectListItem>();
            list.Add(new SelectListItem { Text = "User1", Value = "1" });
            list.Add(new SelectListItem { Text = "User2", Value = "2" });
            return JsonHelper.SerializeObject(list);
        }
    }
}