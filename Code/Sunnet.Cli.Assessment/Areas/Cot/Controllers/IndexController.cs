using System.Globalization;
using System.IO;
using System.Text;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Sunnet.Framework.Resources;
using WebGrease.Css.Extensions;

using Sunnet.Cli.Assessment.Areas.Cot.Models;
using Sunnet.Cli.Assessment.Controllers;
using Sunnet.Cli.Business.Ade;
using Sunnet.Cli.Business.Common;
using Sunnet.Cli.Business.Communities;
using Sunnet.Cli.Business.Cot;
using Sunnet.Cli.Business.Cpalls.Models;
using Sunnet.Cli.Business.Schools;
using Sunnet.Cli.Business.Users;
using Sunnet.Cli.Core.Ade;
using Sunnet.Cli.Core.Cot;
using Sunnet.Cli.Core.Cot.Entities;
using Sunnet.Cli.Core.Users.Entities;
using Sunnet.Cli.UIBase.Filters;
using Sunnet.Cli.UIBase.Models;
using Sunnet.Framework.Extensions;
using Sunnet.Framework.PDF;
using Sunnet.Framework.Permission;

namespace Sunnet.Cli.Assessment.Areas.Cot.Controllers
{
    public class IndexController : BaseController
    {
        private readonly AdeBusiness _adeBusiness;
        private readonly UserBusiness _userBusiness;
        private readonly CommunityBusiness _communityBusiness;
        private readonly SchoolBusiness _schoolBusiness;
        private readonly CotBusiness _cotBusiness;
        public IndexController()
        {
            _adeBusiness = new AdeBusiness(AdeUnitWorkContext);
            _userBusiness = new UserBusiness(UnitWorkContext);
            _communityBusiness = new CommunityBusiness(UnitWorkContext);
            _schoolBusiness = new SchoolBusiness(UnitWorkContext);
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

        private void GetPdf(string html, string fileName, PdfType type = PdfType.COT_Landscape)
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


        [CLIUrlAuthorize(Account = Authority.Index, PageId = PagesModel.COT, Anonymity = Anonymous.Verified, Parameter = "assessmentId")]
        public ActionResult Report(int assessmentId, int teacherId, int year)
        {
            var baseAssessment = _adeBusiness.GetBaseAssessmentModel(assessmentId);
            if (baseAssessment == null) return new EmptyResult();
            ViewBag.AssessmentName = baseAssessment.Name;

            if (!_adeBusiness.CanExecuteAssessment(assessmentId))
                return RedirectToAction("Index", "Dashboard", new { Area = "", showMessage = "assessment_unavaiable" });

            ViewBag.assessmentId = assessmentId;
            ViewBag.teacherId = teacherId;
            var status = _cotBusiness.GetTeacherStatus(assessmentId, year, teacherId);
            if (!(status.HasCotReport || status.HasOldData))
            {
                return RedirectToAction("Index", "Teacher",
                        new { Area = "Cot", id = teacherId, assessmentId = assessmentId, year = year });
            }
            TeacherEntity teacher = _userBusiness.GetTeacher(teacherId, UserInfo);
            ViewBag.teacher = string.Format("{0} {1}", teacher.UserInfo.FirstName, teacher.UserInfo.LastName);
            ViewBag.schoolYear = year.ToSchoolYearString();
            ViewBag.currentYear = CommonAgent.SchoolYear;

            var assessment = _cotBusiness.GetAssessment(assessmentId, year, teacherId);
            ViewBag.Json = JsonHelper.SerializeObject(assessment);
            List<CotWaveEntity> waves = _cotBusiness.GetWaves(assessmentId, year, teacherId);
            var completed = waves.OrderByDescending(x => x.Wave).LastOrDefault(x => x.Status > CotWaveStatus.Saved);
            return View(completed);
        }

        [CLIUrlAuthorize(Account = Authority.Index, PageId = PagesModel.COT, Anonymity = Anonymous.Verified, Parameter = "id")]
        public ActionResult Assessment(int id, int teacherId, int year)
        {
            var baseAssessment = _adeBusiness.GetBaseAssessmentModel(id);
            if (baseAssessment == null) return new EmptyResult();
            ViewBag.AssessmentName = baseAssessment.Name;

            if (!_adeBusiness.CanExecuteAssessment(id))
                return RedirectToAction("Index", "Dashboard", new { Area = "", showMessage = "assessment_unavaiable" });

            ViewBag.assessmentId = id;
            ViewBag.teacherId = teacherId;
            var status = _cotBusiness.GetTeacherStatus(id, year, teacherId);
            if (!status.HasWavesToDo)
            {
                return RedirectToAction("Index", "Teacher",
                       new { Area = "Cot", id = teacherId, assessmentId = id, year = year });
            }
            TeacherEntity teacher = _userBusiness.GetTeacher(teacherId, UserInfo);
            ViewBag.teacher = string.Format("{0} {1}", teacher.UserInfo.FirstName, teacher.UserInfo.LastName);
            ViewBag.SpentTimeOptions = CotHelper.SpentTimes;

            CotWave selectedWave = CotWave.EOY;

            List<CotWaveEntity> waves = _cotBusiness.GetWaves(id, year, teacherId);
            CotWaveEntity unfinalized = waves.FirstOrDefault(x => x.Status < CotWaveStatus.Finalized && x.Status > CotWaveStatus.OldData);
            if (unfinalized == null)
            {
                unfinalized = _cotBusiness.NewCotWaveEntity();
                unfinalized.Wave = 0;
            }
            var allWaves = new List<CotWave>() { CotWave.BOY, CotWave.MOY };
            var completed = waves.Where(x => x.Status > CotWaveStatus.Saved).Select(x => x.Wave);
            if (unfinalized.Wave == CotWave.BOY || unfinalized.Wave == CotWave.MOY)
            {
                selectedWave = unfinalized.Wave;
                ViewBag.WaveOptions = new List<SelectListItem>()
                        {
                            new SelectListItem()
                            {
                                Text = unfinalized.Wave.ToDescription(),
                                Value = ((int) unfinalized.Wave).ToString()
                            }
                        };
            }
            else
            {
                ViewBag.WaveOptions = allWaves.Except(completed).ToList().Select(x => new SelectItemModel()
                {
                    ID = (int)x,
                    Name = x.ToDescription(),
                    Selected = x == unfinalized.Wave
                }).ToSelectList(ViewTextHelper.DefaultPleaseSelectText, "");
            }

            var assessment = _cotBusiness.GetAssessmentForWave(id, year, teacherId, selectedWave);
            ViewBag.Json = JsonHelper.SerializeObject(assessment);

            if (unfinalized.Assessment == null)
                unfinalized.Assessment = new CotAssessmentEntity()
                {
                    ID = assessment.ID,
                    AssessmentId = id,
                    TeacherId = teacherId,
                    SchoolYear = year.ToSchoolYearString()
                };
            return View(unfinalized);
        }


        public ActionResult Pdf(int assessmentId, int teacherId, int year, bool export = true)
        {
            var baseAssessment = _adeBusiness.GetBaseAssessmentModel(assessmentId);
            if (baseAssessment == null) return new EmptyResult();
            ViewBag.AssessmentName = baseAssessment.Name;

            var status = _cotBusiness.GetTeacherStatus(assessmentId, year, teacherId);
            if (!(status.HasCotReport || status.HasOldData))
            {
                return RedirectToAction("Index", "Teacher",
                        new { Area = "Cot", id = teacherId, assessmentId = assessmentId, year = year });
            }
            TeacherEntity teacher = _userBusiness.GetTeacher(teacherId, UserInfo);
            ViewBag.teacher = string.Format("{0} {1}", teacher.UserInfo.FirstName, teacher.UserInfo.LastName);
            ViewBag.schoolYear = year.ToSchoolYearString();
            var assessment = _cotBusiness.GetAssessment(assessmentId, year, teacherId);
            ViewBag.model = assessment;
            CotStgReportEntity cotStgReport = _cotBusiness.GetLastReport(assessmentId, year, teacherId);
            if (cotStgReport != null)
            {
                var observer = _userBusiness.GetUserBaseModel(cotStgReport.CreatedBy);
                ViewBag.observerName = observer.FullName;
            }
            if (export)
            {
                GetPdf(GetViewHtml("Pdf"), "COT");
            }
            return View();
        }

        [HttpPost]
        [CLIUrlAuthorize(Account = Authority.All, PageId = PagesModel.COT, Anonymity = Anonymous.Verified)]
        public string SaveAssessment(CotWaveEntity waveEntity, string items, bool isFinalize = false)
        {
            waveEntity.CreatedBy = UserInfo.ID;
            waveEntity.CreatedOn = DateTime.Now;
            waveEntity.UpdatedBy = UserInfo.ID;
            waveEntity.UpdatedOn = DateTime.Now;
            if (waveEntity.Assessment != null)
            {
                waveEntity.Assessment.CreatedBy = UserInfo.ID;
                waveEntity.Assessment.CreatedOn = DateTime.Now;
                waveEntity.Assessment.UpdatedBy = UserInfo.ID;
                waveEntity.Assessment.UpdatedOn = DateTime.Now;
                waveEntity.Assessment.Status = CotAssessmentStatus.Initialised;
            }
            List<CotAssessmentItemEntity> edited = JsonHelper.DeserializeObject<List<CotAssessmentItemEntity>>(items);
            var response = new PostFormResponse();
            response.Update(_cotBusiness.SaveAssessment(waveEntity, edited, isFinalize));

            return JsonHelper.SerializeObject(response);
        }

        [HttpPost]
        [CLIUrlAuthorize(Account = Authority.All, PageId = PagesModel.COT, Anonymity = Anonymous.Verified)]
        public string SaveCotReport(CotWaveEntity waveEntity, string items, bool createStgReport = false)
        {
            waveEntity.CreatedBy = UserInfo.ID;
            waveEntity.CreatedOn = DateTime.Now;
            waveEntity.UpdatedBy = UserInfo.ID;
            waveEntity.UpdatedOn = DateTime.Now;
            if (waveEntity.Assessment != null)
            {
                waveEntity.Assessment.CreatedBy = UserInfo.ID;
                waveEntity.Assessment.CreatedOn = DateTime.Now;
                waveEntity.Assessment.UpdatedBy = UserInfo.ID;
                waveEntity.Assessment.UpdatedOn = DateTime.Now;
                waveEntity.Assessment.Status = CotAssessmentStatus.Initialised;
            }
            List<CotAssessmentItemEntity> edited = JsonHelper.DeserializeObject<List<CotAssessmentItemEntity>>(items);
            var response = new PostFormResponse();
            response.Update(_cotBusiness.SaveCotItems(waveEntity, edited, createStgReport));
            return JsonHelper.SerializeObject(response);
        }


    }
}