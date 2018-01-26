using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Sunnet.Cli.Business.Ade;
using Sunnet.Cli.UIBase.Filters;
using Sunnet.Framework.Permission;
using Sunnet.Cli.UIBase.Models;
using Sunnet.Framework.Helpers;
using Sunnet.Cli.Business.Communities;
using Sunnet.Cli.Core.Communities.Entities;
using Sunnet.Cli.Core.Schools.Entities;
using Sunnet.Cli.Business.Schools;
using Sunnet.Cli.Core.Users.Entities;
using Sunnet.Cli.Business.Users;
using Sunnet.Cli.Business.Cot.Models;
using Sunnet.Cli.Business.Cot;
using Sunnet.Cli.Business.Common;
using Sunnet.Framework.PDF;
using System.Text;
using System.Globalization;
using Sunnet.Cli.Vcw.Controllers;

namespace Sunnet.Cli.Vcw.Areas.STGReport.Controllers
{
    public class STGSendController : BaseController
    {
        UserBusiness _userBusiness;
        CotBusiness _cotBusiness;
        private readonly AdeBusiness _adeBusiness;
        public STGSendController()
        {
            _userBusiness = new UserBusiness();
            _cotBusiness = new CotBusiness();
            _adeBusiness = new AdeBusiness();
        }

        /// <summary>
        /// 返回Community搜索框的值
        /// </summary>
        /// <param name="exception"></param>
        /// <returns></returns>
        [CLIUrlAuthorizeAttribute(Account = Authority.All, PageId = PagesModel.VCW, Anonymity = Anonymous.Verified)]
        public string GetCommunity(int communityId = 0)
        {
            var expression = PredicateHelper.True<CommunityEntity>();
            var list = new CommunityBusiness().GetCommunitySelectList(null, expression, true);
            return JsonHelper.SerializeObject(list);
        }

        /// <summary>
        /// 返回school搜索框的值
        /// </summary>
        /// <param name="communityId"></param>
        /// <param name="schoolName"></param>
        /// <returns></returns>
        [CLIUrlAuthorizeAttribute(Account = Authority.All, PageId = PagesModel.VCW, Anonymity = Anonymous.Verified)]
        public string GetSchool(int communityId = 0)
        {
            var expression = PredicateHelper.True<SchoolEntity>();
            var schoolList = new SchoolBusiness().GetSchoolsSelectList(null, expression, true);
            return JsonHelper.SerializeObject(schoolList);
        }

        /// <summary>
        /// 返回teacher搜索框的值
        /// </summary>
        /// <param name="communityId"></param>
        /// <returns></returns>
        [CLIUrlAuthorizeAttribute(Account = Authority.All, PageId = PagesModel.VCW, Anonymity = Anonymous.Verified)]
        public string GetTeacher(int communityId = 0, int schoolId = 0)
        {
            var expression = PredicateHelper.True<TeacherEntity>();
            var teacherList = _userBusiness.GetTeacherSelectList(expression, true);
            return JsonHelper.SerializeObject(teacherList);
        }

        [CLIUrlAuthorizeAttribute(Account = Authority.All, PageId = PagesModel.VCW, Anonymity = Anonymous.Verified)]
        public string GetReports(int teacherId, string sort = "ID", string order = "DESC", int first = 0, int count = 10)
        {
            int total = 0;
            int year = CommonAgent.Year;
            List<CotStgReportModel> list = _cotBusiness.GetReports(year, teacherId, sort, order, first, count, out total);
            foreach (CotStgReportModel item in list)
            {
                string url = Url.Action("Pdf", "STGSend", new { Area = "STGReport", id = item.ID, type = "teacher" });
                item.AdditionalComments = "<a class='form-link2' target='_blank' title='Download pdf'  href='" + url + "'>"
                    + item.CreatedOn.ToString("MM/dd/yyyy HH:mm:ss tt") + "</a>";
                item.SpentTime = item.ID.ToString() + "|" + item.CreatedOn.ToString();
            }
            var result = new { total = total, data = list };
            return JsonHelper.SerializeObject(result);
        }

        [CLIUrlAuthorizeAttribute(Account = Authority.All, PageId = PagesModel.VCW, Anonymity = Anonymous.Verified)]
        public ActionResult Pdf(int id, string type, bool export = true)
        {
            CotAssessmentModel assessment = _cotBusiness.GetAssessmentModelForStg(id);
            if (assessment == null || assessment.Report == null)
            {
                return RedirectToAction("Index", "Dashboard", new { Area = "" });
            }

            var baseAssessment = _adeBusiness.GetBaseAssessmentModel(assessment.AssessmentId);
            if (baseAssessment == null) return new EmptyResult();
            ViewBag.AssessmentName = baseAssessment.Name;

            assessment.Measures.ToList().ForEach(mea =>
            {
                if (mea.Children != null && mea.Children.Any())
                {
                    mea.Children.ToList().ForEach(
                        child => child.Items = child.Items.Where(x => x.GoalMetDone == false));
                }
                else
                {
                    mea.Items = mea.Items.Where(x => x.GoalMetDone == false);
                }
            });

            ViewBag.Model = assessment;
            var isTeacher = type == "teacher";
            ViewBag.isTeacher = isTeacher;
            var filename = isTeacher ? "Teacher_STGReport" : "Mentor_STGReport";
            if (export)
            {
                GetPdf(GetViewHtml("Pdf"), filename +
                    (assessment.Report.GoalMetDate > CommonAgent.MinDate ? assessment.Report.GoalMetDate.ToString("_yyyy_MM_dd") : ""));
            }
            return View();
        }

        private void GetPdf(string html, string fileName)
        {
            PdfProvider pdfProvider = new PdfProvider();
            pdfProvider.IsPortrait = false;
            pdfProvider.GeneratePDF(html, fileName);
        }

        private string GetViewHtml(string viewName)
        {
            var resultHtml = string.Empty;
            ViewEngineResult result = ViewEngines.Engines.FindView(ControllerContext, viewName, null);
            if (null == result.View)
            {
                throw new InvalidOperationException(FormatErrorMessage(viewName, result.SearchedLocations));
            }
            try
            {
                ViewContext viewContext = new ViewContext(ControllerContext, result.View, this.ViewData, this.TempData, Response.Output);
                var textWriter = new System.IO.StringWriter();
                result.View.Render(viewContext, textWriter);
                resultHtml = textWriter.ToString();
            }
            finally
            {
                result.ViewEngine.ReleaseView(ControllerContext, result.View);
            }
            return resultHtml;
        }

        private string FormatErrorMessage(string viewName, IEnumerable<string> searchedLocations)
        {
            string format = "The view '{0}' or its master was not found or no view engine supports the searched locations. The following locations were searched:{1}";
            StringBuilder builder = new StringBuilder();
            foreach (string str in searchedLocations)
            {
                builder.AppendLine();
                builder.Append(str);
            }
            return string.Format(CultureInfo.CurrentCulture, format, viewName, builder);
        }

        [CLIUrlAuthorizeAttribute(Account = Authority.All, PageId = PagesModel.VCW, Anonymity = Anonymous.Verified)]
        public ActionResult STGReport()
        {
            return View();
        }

    }
}