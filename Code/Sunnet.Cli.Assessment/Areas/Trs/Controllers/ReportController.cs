using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
/**************************************************************************
 * Developer: 		Joe
 * Computer:		JOE-PC
 * Domain:			Joe-pc
 * CreatedOn:		2015/1/15 12:15:10
 * Description:		Please input class summary
 * Version History:	Created,2015/1/15 12:15:10
 * 
 * 
 **************************************************************************/
using Sunnet.Cli.Assessment.Controllers;
using Sunnet.Cli.Business.Trs.Models;
using Sunnet.Cli.Business.Trs;
using Sunnet.Cli.UIBase;
using Sunnet.Cli.UIBase.Filters;
using Sunnet.Framework;
using Sunnet.Framework.PDF;
using Sunnet.Framework.Permission;
using Sunnet.Cli.UIBase.Models;
using Sunnet.Framework.Resources;
using Sunnet.Framework.Extensions;

namespace Sunnet.Cli.Assessment.Areas.Trs.Controllers
{
    public class ReportController : BaseController
    {
        private TrsBusiness _trsBusiness;
        public ReportController()
        {
            _trsBusiness = new TrsBusiness(AdeUnitWorkContext);
        }

        #region PDF

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

        private string GetExistedReport(int assessmentId, string filename)
        {
            filename = Path.Combine("Trs/" + assessmentId + "/", filename);
            return FileHelper.HasProtectedFile(filename);
        }

        private void GetPdf(string html, string localFilename, PdfType type = PdfType.General)
        {
            PdfProvider pdfProvider = new PdfProvider(type);
            pdfProvider.SavePdf(html, localFilename);
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

        [CLIUrlAuthorizeAttribute(Account = Authority.All, PageId = PagesModel.TRS, Anonymity = Anonymous.Verified)]
        public ActionResult AssessmentResults(int id, bool export = true, bool download = true)
        {
            var updatedOn = _trsBusiness.ReportAvailable(id);
            if (updatedOn <= 0)
            {
                return View("NotAvailable");
            }
            var targetView = "AssessmentResults";
            TrsResultReportModel resultModel = null;
            if (export)
            {
                string filename = "Results" + updatedOn + ".pdf";
                string outFilename = "Report - Assessment Results.pdf";
                var localFile = GetExistedReport(id, filename);
                if (string.IsNullOrEmpty(localFile))
                {
                    localFile = Path.Combine(SFConfig.ProtectedFiles, "Trs/" + id + "/", filename);
                    resultModel = _trsBusiness.GetReportAssement(id, UserInfo);
                    if (resultModel.Star == 0)
                    {
                        targetView = "AssessmentResultsFirst";
                    }
                    ViewBag.Model = resultModel;
                    ViewBag.AssessmentType = resultModel.Type == 0 ? resultModel.EventLogType.ToDescription() : resultModel.Type.ToDescription();
                    GetPdf(GetViewHtml(targetView), localFile);
                }
                if (download)
                {
                    FileHelper.ResponseFile(localFile, outFilename);
                    return new EmptyResult();
                }
            }
            else
            {
                resultModel = _trsBusiness.GetReportAssement(id, UserInfo);
                if (resultModel.Star == 0)
                {
                    targetView = "AssessmentResultsFirst";
                }
                ViewBag.Model = resultModel;
                ViewBag.AssessmentType = resultModel.Type == 0 ? resultModel.EventLogType.ToDescription() : resultModel.Type.ToDescription();
            }
            return View(targetView);
        }

        [CLIUrlAuthorizeAttribute(Account = Authority.All, PageId = PagesModel.TRS, Anonymity = Anonymous.Verified)]
        public ActionResult ReportWithComments(int id, bool export = true, bool download = true)
        {
            var updatedOn = _trsBusiness.ReportAvailable(id);
            if (updatedOn <= 0)
            {
                return View("NotAvailable");
            }
            var targetView = "ReportWithComments";
            TrsResultReportModel resultModel = null;
            if (export)
            {
                string filename = "Results_Comments" + updatedOn + ".pdf";
                string outFilename = "Report - Assessment-Comments Results.pdf";
                var localFile = GetExistedReport(id, filename);
                if (string.IsNullOrEmpty(localFile))
                {
                    localFile = Path.Combine(SFConfig.ProtectedFiles, "Trs/" + id + "/", filename);
                    resultModel = _trsBusiness.GetCommentReportAssement(id, UserInfo);
                    if (resultModel.Star == 0)
                    {
                        targetView = "AssessmentResultsFirst";
                    }
                    ViewBag.Model = resultModel;
                    ViewBag.AssessmentType = resultModel.Type == 0 ? resultModel.EventLogType.ToDescription() : resultModel.Type.ToDescription();
                    GetPdf(GetViewHtml(targetView), localFile);
                }
                if (download)
                {
                    FileHelper.ResponseFile(localFile, outFilename);
                    return new EmptyResult();
                }
            }
            else
            {
                resultModel = _trsBusiness.GetCommentReportAssement(id, UserInfo);
                if (resultModel.Star == 0)
                {
                    targetView = "AssessmentResultsFirst";
                }
                ViewBag.Model = resultModel;
                ViewBag.AssessmentType = resultModel.Type == 0 ? resultModel.EventLogType.ToDescription() : resultModel.Type.ToDescription();
            }
            return View(targetView);
        }


        public ActionResult Preview(int id, bool all = false)
        {
            ViewBag.AllFilled = all;
            ViewBag.Preview = true;
            var updatedOn = _trsBusiness.ReportAvailable(id);
            if (updatedOn > 0)
            {
                return View("NotAvailable");
            }

            TrsResultReportModel resultModel = _trsBusiness.GetReportAssement(id, UserInfo);
            var model = _trsBusiness.GetPreviewModel(id, UserInfo);
            resultModel.Star = model.Star;
            resultModel.VerifiedStar = model.VerifiedStar;
            resultModel.CategoryStars = model.StarOfCategory.Select(sc => new TrsCategoryStarModel()
            {
                Category = sc.Key,
                Star = sc.Value
            });
            ViewBag.Model = resultModel;
            ViewBag.StarNotAvailable = ResourceHelper.GetRM().GetInformation("Trs_CalcStar_NeedCompleted");
            return View("AssessmentResults");
        }

        [CLIUrlAuthorizeAttribute(Account = Authority.All, PageId = PagesModel.TRS, Anonymity = Anonymous.Verified)]
        public ActionResult Facility(int id, bool export = true, bool download = true)
        {
            var updatedOn = _trsBusiness.ReportAvailable(id);
            if (updatedOn <= 0)
            {
                return View("NotAvailable");
            }
            if (export)
            {
                string filename = "Facility" + updatedOn + ".pdf";
                string outFilename = "Report - Facility Item (TA).pdf";
                var localFile = GetExistedReport(id, filename);
                if (string.IsNullOrEmpty(localFile))
                {
                    localFile = Path.Combine(SFConfig.ProtectedFiles, "Trs/" + id + "/", filename);
                    var report = _trsBusiness.GetReportModel(id, UserInfo, true);
                    if (report == null)
                        return View("NotAvailable");
                    ViewBag.Model = report;
                    GetPdf(GetViewHtml("Facility"), localFile);
                }
                if (download)
                {
                    FileHelper.ResponseFile(localFile, outFilename);

                    return new EmptyResult();
                }
            }
            else
            {
                var report = _trsBusiness.GetReportModel(id, UserInfo, true);
                if (report == null)
                    return View("NotAvailable");
                ViewBag.Model = report;
            }
            return View();
        }

        [CLIUrlAuthorizeAttribute(Account = Authority.All, PageId = PagesModel.TRS, Anonymity = Anonymous.Verified)]
        public ActionResult Class(int id, int classId, bool export = true, bool download = true)
        {
            var updatedOn = _trsBusiness.ReportAvailable(id);
            if (updatedOn <= 0)
            {
                return View("NotAvailable");
            }
            if (export)
            {
                string filename = "Class" + classId + "_" + updatedOn + ".pdf";
                string outFilename = "Report - Classroom Item (TA).pdf";
                var localFile = GetExistedReport(id, filename);
                if (string.IsNullOrEmpty(localFile))
                {
                    localFile = Path.Combine(SFConfig.ProtectedFiles, "Trs/" + id + "/", filename);
                    var report = _trsBusiness.GetReportModel(id, UserInfo);

                    var list_Classes = report.Classes;
                    ViewBag.className = list_Classes.FirstOrDefault(x => x.Id == classId).Name;
                    foreach (var item in list_Classes)
                    {
                        if (id > 0)
                        {
                            var classModel = list_Classes.FirstOrDefault(r => r.Id == item.Id);
                            if (classModel != null)
                            {
                                item.ObservationLength = classModel.ObservationLength;
                            }
                        }
                        string newName = item.Name;
                        if (newName.Substring(0, 4) == "TRS-")
                        {
                            newName = newName.Substring(4);
                        }
                        if (item.Ages.Count() > 0)
                        {
                            newName += " - ";
                            foreach (var age in item.Ages)
                            {
                                Regex r = new Regex(@"\d");
                                int index = r.Match(age.TypeOfChildren).Index;
                                newName += age.TypeOfChildren.Substring(0, index) + "/";
                            }
                        }
                        item.Name = newName.TrimEnd('/');
                    }
                    ViewBag.Model = report;
                    ViewBag.ClassModel = report.Classes.FirstOrDefault(x => x.Id == classId);

                    GetPdf(GetViewHtml("Class"), localFile);
                }
                if (download)
                {
                    FileHelper.ResponseFile(localFile, outFilename);
                    return new EmptyResult();
                }
            }
            else
            {
                var report = _trsBusiness.GetReportModel(id, UserInfo);
                ViewBag.ClassModel = report.Classes.FirstOrDefault(x => x.Id == classId);
                ViewBag.Model = report;
            }
            return View();
        }

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);
            ViewBag.Pdf = false;
        }
    }
}