using Sunnet.Cli.Assessment.Areas.Cot.Models;
using Sunnet.Cli.Assessment.Controllers;
using Sunnet.Cli.Business.Ade;
using Sunnet.Cli.Business.Common;
using Sunnet.Cli.Business.Cot;
using Sunnet.Cli.Business.Cot.Models;
using Sunnet.Cli.Business.Users;
using Sunnet.Cli.Core.Cot;
using Sunnet.Cli.Core.Cot.Entities;
using Sunnet.Cli.Core.Users.Entities;
using Sunnet.Cli.UIBase.Filters;
using Sunnet.Cli.UIBase.Models;
using Sunnet.Framework.PDF;
using Sunnet.Framework.Permission;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Web.Mvc;

namespace Sunnet.Cli.Assessment.Areas.Cot.Controllers
{
    public class StgReportController : BaseController
    {
        private readonly AdeBusiness _adeBusiness;
        private readonly UserBusiness _userBusiness;
        private readonly CotBusiness _cotBusiness;
        public StgReportController()
        {
            _adeBusiness = new AdeBusiness(AdeUnitWorkContext);
            _userBusiness = new UserBusiness(UnitWorkContext);
            _cotBusiness = new CotBusiness(AdeUnitWorkContext);
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

        private void GetPdf(string html, string fileName, PdfType type = PdfType.COT_Portrait)
        {
            string userName = UserInfo.FirstName + " " + UserInfo.LastName;
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

        [CLIUrlAuthorize(Account = Authority.All, PageId = PagesModel.COT, Anonymity = Anonymous.Verified)]
        public ActionResult Edit(int id, int year = 0)
        {
            if (year == 0) year = CommonAgent.Year;
            ViewBag.SpentTimeOptions = CotHelper.SpentTimes;
            CotAssessmentModel assessmentModel = _cotBusiness.GetAssessmentModelForStg(id);
            if (assessmentModel == null)
            {
                return RedirectToAction("Index", "Dashboard", new { Area = "" });
            }
            ViewBag.Json = JsonHelper.SerializeObject(assessmentModel);

            var baseAssessment = _adeBusiness.GetBaseAssessmentModel(assessmentModel.AssessmentId);
            if (baseAssessment == null) return new EmptyResult();
            ViewBag.AssessmentName = baseAssessment.Name;

            ViewBag.assessmentId = assessmentModel.AssessmentId;
            ViewBag.teacherId = assessmentModel.TeacherId;
            TeacherEntity teacher = _userBusiness.GetTeacher(assessmentModel.TeacherId, UserInfo);
            ViewBag.teacher = string.Format("{0} {1}", teacher.UserInfo.FirstName, teacher.UserInfo.LastName);
            if (assessmentModel.Report.Status == CotStgReportStatus.Deleted)
            {
                return RedirectToAction("Index", "Teacher",
                    new { id = assessmentModel.TeacherId, assessmentId = assessmentModel.AssessmentId, year = year });
            }

            ViewBag.LastReport = false;
            CotStgReportEntity lastReport = _cotBusiness.GetLastReport(assessmentModel.AssessmentId, year, assessmentModel.TeacherId);
            if (lastReport != null)
            {
                ViewBag.LastReport = lastReport.ID == id;
            }
            ViewBag.ShowFullText = assessmentModel.Report.ShowFullText ? "1" : "0";
            return View(assessmentModel.Report);
        }

        [CLIUrlAuthorize(Account = Authority.All, PageId = PagesModel.COT, Anonymity = Anonymous.Verified)]
        [ValidateInput(false)]
        public string Save(CotStgReportEntity entity, string items)
        {
            var itemIds = JsonHelper.DeserializeObject<List<int>>(items);
            var response = new PostFormResponse();
            response.Update(_cotBusiness.SaveStgReport(entity, itemIds));
            return JsonHelper.SerializeObject(response);
        }

        [CLIUrlAuthorize(Account = Authority.All, PageId = PagesModel.COT, Anonymity = Anonymous.Verified)]
        public ActionResult Pdf(int id, string type, bool export = true, bool fullItemText = true)
        {
            var assessment = _cotBusiness.GetAssessmentModelForStg(id);
            if (assessment == null || assessment.Report == null)
            {
                return RedirectToAction("Index", "Dashboard", new { Area = "" });
            }

            var baseAssessment = _adeBusiness.GetBaseAssessmentModel(assessment.AssessmentId);
            if (baseAssessment == null) return new EmptyResult();
            ViewBag.AssessmentName = baseAssessment.Name;


            ViewBag.Model = assessment;

            var isTeacher = type == "teacher";
            ViewBag.isTeacher = isTeacher;
            var filename = isTeacher ? "Teacher_STGReport" : "Mentor_STGReport";
            ViewBag.fullText = fullItemText;
            ViewBag.observerName = "";
            CotStgReportEntity cotStgReport = _cotBusiness.GetLastReport(assessment.AssessmentId, assessment.SchoolYear, assessment.TeacherId);
            if (cotStgReport != null)
            {
                var observer = _userBusiness.GetUserBaseModel(cotStgReport.CreatedBy);
                ViewBag.observerName = observer.FullName;
            }
            if (export)
            {
                GetPdf(GetViewHtml("Pdf"), filename +
                    (assessment.Report.GoalMetDate > CommonAgent.MinDate ? assessment.Report.GoalMetDate.ToString("_yyyy_MM_dd") : ""));
            }
            return View();
        }

        [CLIUrlAuthorize(Account = Authority.All, PageId = PagesModel.COT, Anonymity = Anonymous.Verified)]
        [HttpPost]
        public string SaveOrders(int id, int[] itemOrders)
        {
            PostFormResponse response = new PostFormResponse();
            response.Update(_cotBusiness.SaveStgReportItemOrders(id, itemOrders));
            return JsonHelper.SerializeObject(response);
        }

        [CLIUrlAuthorize(Account = Authority.All, PageId = PagesModel.COT, Anonymity = Anonymous.Verified)]
        [HttpPost]
        public string NewStgGroup(int stgReportId, string groupName, int[] groupItemOrders)
        {
            PostFormResponse response = new PostFormResponse();
            response.Update(_cotBusiness.SaveStgGroup(stgReportId, groupName, groupItemOrders));
            return JsonHelper.SerializeObject(response);
        }

        [CLIUrlAuthorize(Account = Authority.All, PageId = PagesModel.COT, Anonymity = Anonymous.Verified)]
        [HttpGet]
        public string GetStgGroups(int stgReportId)
        {
            List<CotStgGroupModel> list = _cotBusiness.GetStgGroupByReportId(stgReportId);
            return JsonHelper.SerializeObject(list);
        }

        [CLIUrlAuthorize(Account = Authority.All, PageId = PagesModel.COT, Anonymity = Anonymous.Verified)]
        [HttpPost]
        public string DelStgGroup(int stgGroupId)
        {
            PostFormResponse response = new PostFormResponse();
            response.Update(_cotBusiness.DelStgGroup(stgGroupId));
            return JsonHelper.SerializeObject(response);
        }

        [CLIUrlAuthorize(Account = Authority.All, PageId = PagesModel.COT, Anonymity = Anonymous.Verified)]
        [HttpPost]
        [ValidateInput(false)]
        public string SaveGroupItemOrder(int stgGroupId, int[] groupItemOrders, string onMyOwn, string withSupport)
        {
            PostFormResponse response = new PostFormResponse();
            response.Update(_cotBusiness.UpdateStgGroup(stgGroupId, groupItemOrders, onMyOwn, withSupport));
            return JsonHelper.SerializeObject(response);
        }

        [CLIUrlAuthorize(Account = Authority.All, PageId = PagesModel.COT, Anonymity = Anonymous.Verified)]
        [HttpPost]
        public string DelStgGroupItem(int stgGroupId, int stgGroupItemId)
        {
            PostFormResponse response = new PostFormResponse();
            response.Update(_cotBusiness.DelStgGroupItem(stgGroupId, stgGroupItemId));
            return JsonHelper.SerializeObject(response);
        }
    }
}