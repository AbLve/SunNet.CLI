using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using Sunnet.Cli.Assessment.Controllers;
using Sunnet.Cli.Business.Ade;
using Sunnet.Cli.Business.Ade.Models;
using Sunnet.Cli.Business.Classes;
using Sunnet.Cli.Business.Communities;
using Sunnet.Cli.Business.Cot;
using Sunnet.Cli.Business.Cpalls.Models;
using Sunnet.Cli.Business.Permission;
using Sunnet.Cli.Business.Schools;
using Sunnet.Cli.Business.Students;
using Sunnet.Cli.Business.Tsds.Models;
using Sunnet.Cli.Business.Tsds;
using Sunnet.Cli.Core.Ade;
using Sunnet.Cli.Core.Common.Enums;
using Sunnet.Cli.Core.Communities.Entities;
using Sunnet.Cli.Core.Schools.Entities;
using Sunnet.Cli.Core.Tsds.Entities;
using Sunnet.Cli.Core.Users.Enums;
using Sunnet.Cli.UIBase;
using Sunnet.Cli.UIBase.Filters;
using Sunnet.Cli.UIBase.Models;
using Sunnet.Framework;
using Sunnet.Framework.Core.Tool;
using Sunnet.Framework.Extensions;
using Sunnet.Framework.Helpers;
using Sunnet.Framework.Permission;

namespace Sunnet.Cli.Assessment.Areas.Tsds.Controllers
{
    public class TsdsController : BaseController
    {

        private AdeBusiness _adeBusiness;
        private StudentBusiness _studentBusiness;
        private CommunityBusiness _communityBusiness;
        private SchoolBusiness _schoolBusiness;
        private ClassBusiness _classBusiness;
        private ObservableBusiness _observableBusiness;
        private TsdsBusiness _tsdsBusiness;
        public TsdsController()
        {
            _adeBusiness = new AdeBusiness(AdeUnitWorkContext);
            _communityBusiness = new CommunityBusiness(UnitWorkContext);
            _studentBusiness = new StudentBusiness(UnitWorkContext);
            _schoolBusiness = new SchoolBusiness(UnitWorkContext);
            _classBusiness = new ClassBusiness(UnitWorkContext);
            _tsdsBusiness = new TsdsBusiness(AdeUnitWorkContext);
            _observableBusiness = new ObservableBusiness(AdeUnitWorkContext);
        }


        // GET: /Observable/Observable/
        [CLIUrlAuthorize(Account = Authority.All, PageId = PagesModel.TSDS, Anonymity = Anonymous.Verified)]
        public ActionResult Index(string showmessage = "")
        {
            List<int> accountPageIds = new PermissionBusiness().CheckPage(UserInfo);

            ViewBag.ShowTSDS = accountPageIds.Contains((int)PagesModel.TSDS);
            List<int> pageIds = accountPageIds.FindAll(r => r > SFConfig.AssessmentPageStartId);


            HttpContext.Response.Cache.SetNoStore();
            var assessmentIds = _tsdsBusiness.mapList.Select(t => t.AssessmentId).Distinct();
            List<CpallsAssessmentModel> list = _adeBusiness.GetAssessmentList(o => o.Type == AssessmentType.Cpalls).Where(t => assessmentIds.Contains(t.ID)).ToList();
            List<CpallsAssessmentModel> accessTCDList = list.FindAll(r => pageIds.Contains(r.ID + SFConfig.AssessmentPageStartId));
            ViewBag.List = accessTCDList;
            return View();
        }


        #region List Page

        public ActionResult RequestList(int assessmentId)
        {
            var assessment = _adeBusiness.GetAssessment(assessmentId);
            ViewBag.AssessmentName = assessment.Name + " - " + assessment.Language.ToString();
            ViewBag.AssessmentId = assessmentId;

            /*  David 06/20/2017 Hard code the options, only 6 measures are availiable for download.
            var measureSelectList = new List<SelectListItem>();

            var measures = _adeBusiness.GetMeasureReport(assessmentId); //new List<SelectListItem>();
            foreach (MeasureReportModel measure in measures)
            {
                measureSelectList.Add(new SelectListItem { Text = measure.Name, Value = measure.ID.ToString() });
            }*/

            ViewBag.Measures = _tsdsBusiness.GetAvailableMeasures(assessmentId);


            List<SelectListItem> communityList = new List<SelectListItem>();
            IEnumerable<CommunitySelectItemModel> communityModelList =
                _communityBusiness.GetCommunitySelectList(UserInfo, PredicateHelper.True<CommunityEntity>(), true)
                    .ToList();

            List<SelectListItem> schoolList = new List<SelectListItem>();
            var expression = PredicateHelper.True<SchoolEntity>();
            expression = expression.And(o => o.SchoolType.Name != "Demo");
            IEnumerable<SchoolSelectItemModel> schoolModelList =
                _schoolBusiness.GetSchoolsSelectList(UserInfo, expression, true).ToList();

            foreach (CommunitySelectItemModel com in communityModelList)
            {
                communityList.Add(new SelectListItem { Text = com.Name, Value = com.ID.ToString() });

                SelectListGroup group = new SelectListGroup();
                group.Name = com.Name;
                var comId = com.ID;
                if (schoolModelList != null)
                {
                    var list = schoolModelList.ToList().Where(c => c.CommunityIds.Contains(comId)).ToList();
                    foreach (SchoolSelectItemModel item in list)
                    {
                        schoolList.Add(new SelectListItem { Text = item.Name, Value = item.ID.ToString(), Group = group });
                    }
                }
            }
            ViewBag.Communities = communityList;
            ViewBag.Schools = schoolList;

            return View();
        }

        public string Search(int assessmentId, string fileName = "", int fileId = 0, string communityName = "", int communityId = 0,
           string schoolName = "", int schoolId = 0, string userName = "", int userId = 0,
            string sort = "ID", string order = "Asc", int first = 0, int count = 10)
        {

            int totalCount = 0;
            List<DownloadListModel> list = new List<DownloadListModel>();
            var expression = PredicateHelper.True<TsdsEntity>();

            /*David 06/20/2017 */
            if (assessmentId == 0) assessmentId = 9;//Default to 9

            expression = expression.And(o => o.AssessmentId == assessmentId);

            if (fileName != "")
            {
                expression = expression.And(o => o.FileName.Contains(fileName));
            }
            if (communityId > 0)
            {
                expression = expression.And(o => o.CommunityId == communityId);
            }
            else if (communityName != "")
            {
                expression = expression.And(o => o.Community.Name.Contains(communityName));
            }
            if (schoolId > 0)
            {
                expression = expression.And(o => o.SchoolIds.Contains(schoolId.ToString() + ","));
            }
            else if (schoolName != "")
            {
                expression = expression.And(o => o.SchoolNames.Contains(schoolName));
            }

            if (userId > 0)
            {
                expression = expression.And(o => o.DownloadBy == userId);
            }
            else if (userName != "")
            {
                expression = expression.And(o => (o.DownloadUser.FirstName + " " + o.DownloadUser.LastName).Contains(userName));
            }


            list = _tsdsBusiness.SearchTsdses(UserInfo, expression, sort, order, first, count, out totalCount);
            var result = new { total = totalCount, data = list };
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
            expression = expression.And(c => c.SchoolType.Name != "Demo");
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
            List<SelectItemModel> list = new List<SelectItemModel>();
            list = _tsdsBusiness.GetDownLoadUsers();
            return JsonHelper.SerializeObject(list);
        }

        [CLIUrlAuthorize(Account = Authority.All, PageId = PagesModel.TSDS, Anonymity = Anonymous.Verified)]
        public ActionResult DownloadXmlFile(int id)
        {
            var accessTSDS = true;
            var userSchoolList = _schoolBusiness.SearchSchoolIdsByUserIds(UserInfo);

            var tsds = _tsdsBusiness.GetTsds(id);
            if (tsds != null && tsds.SchoolIds != null)
            {
                List<string> schoolIds = tsds.SchoolIds.Trim(',').Split(',').ToList();
                foreach (var schoolIdstr in schoolIds)
                {
                    int schoolId = 0;
                    int.TryParse(schoolIdstr, out schoolId);
                    if (schoolId > 0 && !userSchoolList.Contains(schoolId))
                    {
                        accessTSDS = false;
                        break;
                    }
                }
                if (accessTSDS)
                {

                    var localFile = Path.Combine(SFConfig.ProtectedFiles, "TSDS/", tsds.FileName);
                    FileHelper.ResponseFile(localFile, tsds.FileName);
                    return new EmptyResult();
                }
            }
            else
            {
                accessTSDS = false;
            }
            return View();
        }

        [CLIUrlAuthorize(Account = Authority.All, PageId = PagesModel.TSDS, Anonymity = Anonymous.Verified)]
        public ActionResult DownloadErrorFile(int id)
        {
            var accessTSDS = true;
            var userSchoolList = _schoolBusiness.SearchSchoolIdsByUserIds(UserInfo);

            var tsds = _tsdsBusiness.GetTsds(id);
            if (tsds != null && tsds.SchoolIds != null)
            {
                List<string> schoolIds = tsds.SchoolIds.Trim(',').Split(',').ToList();
                foreach (var schoolIdstr in schoolIds)
                {
                    int schoolId = 0;
                    int.TryParse(schoolIdstr, out schoolId);
                    if (schoolId > 0 && !userSchoolList.Contains(schoolId))
                    {
                        accessTSDS = false;
                        break;
                    }
                }
                if (accessTSDS)
                {

                    var localFile = Path.Combine(SFConfig.ProtectedFiles, "TSDS/MissingID/", tsds.ErrorFileName);
                    FileHelper.ResponseFile(localFile, tsds.ErrorFileName, "csv");
                    return new EmptyResult();
                }
            }
            else
            {
                accessTSDS = false;
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [CLIUrlAuthorize(Account = Authority.All, PageId = PagesModel.TSDS, Anonymity = Anonymous.Verified)]
        public string RequestTsds(int assessmentId, int[] Measures, int[] Communities, int[] Schools, DateTime? DOBStartDate, DateTime? DOBEndDate)
        {
            OperationResult res = new OperationResult(OperationResultType.Success);
            var response = new PostFormResponse();
            if (assessmentId < 0)
            {
                res.ResultType = OperationResultType.Error;
                res.Message = "Assessment Family can not be null.";
            }
            else if (Measures == null || Measures.Length <= 0)
            {
                res.ResultType = OperationResultType.Error;
                res.Message = "Measure can not be null.";
            }
            else if (Communities == null || Communities.Length <= 0)
            {
                res.ResultType = OperationResultType.Error;
                res.Message = "Community can not be null.";
            }

            else if (Schools == null || Schools.Length <= 0)
            {
                res.ResultType = OperationResultType.Error;
                res.Message = "School can not be null.";
            }
            if (res.ResultType == OperationResultType.Error)
            {
                response.Success = res.ResultType == OperationResultType.Success;
                response.Data = null;
                response.Message = res.Message;
                return JsonConvert.SerializeObject(response);
            }

            string schoolIdstr = string.Join(",", Schools);
            string measureIdstr = string.Join(",", Measures);
            var schoolList = _schoolBusiness.GetSchoolModels(UserInfo, c => Schools.Contains(c.ID));
            var schoolName = string.Join(",", schoolList.Select(c => c.Name).ToList());
            List<TsdsEntity> list = new List<TsdsEntity>();

            foreach (var community in Communities)
            {
                TsdsEntity tsds = new TsdsEntity();
                tsds.AssessmentId = assessmentId;
                tsds.MeasureIds = measureIdstr;
                tsds.FileName = "";
                tsds.CommunityId = community;
                tsds.SchoolIds = schoolIdstr;
                tsds.DownloadBy = UserInfo.ID;
                tsds.DownloadOn = DateTime.Now;
                tsds.Status = TsdsStatus.Pending;
                tsds.Comment = "";
                tsds.CreatedOn = DateTime.Now;
                tsds.UpdatedOn = DateTime.Now;
                tsds.SchoolNames = schoolName;
                tsds.DOBStartDate = DOBStartDate;
                tsds.DOBEndDate = DOBEndDate;
                list.Add(tsds);
            }

            // CreateAssessmentXml(assessmentId, Communities[0]);
            res = _tsdsBusiness.InsertTSDS(list);
            response.Success = res.ResultType == OperationResultType.Success;
            response.Data = null;
            response.Message = res.Message;
            return JsonConvert.SerializeObject(response);
        }

        public string GetCommunityChange(string communityIds)
        {
            var condition = PredicateHelper.True<CommunityEntity>();
            var schoolCondition = PredicateHelper.True<SchoolEntity>();
            if (communityIds != "")
            {
                var communityIdList = communityIds.Split(',').Select(x => int.Parse(x)).ToList();
                condition = condition.And(c => communityIdList.Contains(c.ID));
                schoolCondition =
                    schoolCondition.And(
                        s => s.CommunitySchoolRelations.Any(r => communityIdList.Contains(r.CommunityId))
                             && s.SchoolType.Name != "Demo");
            }

            List<MultiselectGroupModel> communityList = new List<MultiselectGroupModel>();
            IEnumerable<CommunitySelectItemModel> communityModelList =
                _communityBusiness.GetCommunitySelectList(UserInfo, condition, true).ToList();

            IEnumerable<SchoolSelectItemModel> schoolModelList =
                _schoolBusiness.GetSchoolsSelectList(UserInfo, schoolCondition, true).ToList();
            if (!communityModelList.Any())
                return "";
            foreach (CommunitySelectItemModel com in communityModelList)
            {
                MultiselectGroupModel group = new MultiselectGroupModel();
                List<MultiselectModel> models = new List<MultiselectModel>();
                group.label = com.Name;
                group.children = models;

                var comId = com.ID;
                if (schoolModelList != null)
                {
                    var list = schoolModelList.ToList().Where(c => c.CommunityIds.Contains(comId)).ToList();
                    list.ForEach(x => models.Add(new MultiselectModel
                    {
                        label = x.Name,
                        value = x.ID.ToString()
                    }));
                }
                communityList.Add(group);
            }
            return JsonHelper.SerializeObject(communityList);
        }
        #endregion

        #region xml
        public void CreateAssessmentXml(int assessmentId, int communityId)
        {
            //string fileName = "";
            //var bytes = _tsdsBusiness.CreateTsdsAssessmentFile(assessmentId, communityId,UserInfo, out fileName);
            //Response.ContentType = "text/xml";
            //Response.AddHeader("Content-Disposition",
            //    "attachment;  filename=" +
            //    HttpUtility.UrlEncode(fileName,
            //        System.Text.Encoding.UTF8));
            //Response.BinaryWrite(bytes);
            //Response.Flush();
            //Response.End();
        }
        #endregion
    }
}