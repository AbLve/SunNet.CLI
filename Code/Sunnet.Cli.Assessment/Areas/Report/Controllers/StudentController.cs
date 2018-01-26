using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Common.CommandTrees.ExpressionBuilder;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using EvoPdf;
using Newtonsoft.Json;
using StructureMap;
using Sunnet.Cli.Assessment.Areas.Report.Models;
using Sunnet.Cli.Assessment.Controllers;
using Sunnet.Cli.Business;
using Sunnet.Cli.Business.Ade;
using Sunnet.Cli.Business.Classes;
using Sunnet.Cli.Business.Common;
using Sunnet.Cli.Business.Communities;
using Sunnet.Cli.Business.Cpalls;
using Sunnet.Cli.Business.Cpalls.Models;
using Sunnet.Cli.Business.Cpalls.Models.Report;
using Sunnet.Cli.Business.Reports;
using Sunnet.Cli.Business.Schools;
using Sunnet.Cli.Business.Students;
using Sunnet.Cli.Business.Students.Models;
using Sunnet.Cli.Core.Ade;
using Sunnet.Cli.Core.Ade.Entities;
using Sunnet.Cli.Core.Common.Enums;
using Sunnet.Cli.Core.Cpalls;
using Sunnet.Cli.Core.Reports.Entities;
using Sunnet.Cli.UIBase;
using Sunnet.Cli.UIBase.Filters;
using Sunnet.Cli.UIBase.Models;
using Sunnet.Framework;
using Sunnet.Framework.Core.Tool;
using Sunnet.Framework.Extensions;
using Sunnet.Framework.Log;
using Sunnet.Framework.PDF;
using Sunnet.Framework.Permission;
using Sunnet.Framework.Resources;
using Sunnet.Framework.Helpers;
using Sunnet.Cli.Business.Ade.Models;

namespace Sunnet.Cli.Assessment.Areas.Report.Controllers
{
    public class StudentController : BaseController
    {
        private readonly CpallsBusiness _cpallsBusiness;
        private readonly ClassBusiness _classBusiness;
        private readonly StudentBusiness _studentBusiness;
        private readonly SchoolBusiness _schoolBusiness;
        private readonly AdeBusiness _adeBusiness;
        private readonly ReportBusiness _reportBusiness;
        private ISunnetLog _logger;
        private readonly string _reportFirstColumnTitle = "Student";
        public StudentController()
        {
            _cpallsBusiness = new CpallsBusiness(AdeUnitWorkContext);
            _classBusiness = new ClassBusiness();
            _studentBusiness = new StudentBusiness();
            _schoolBusiness = new SchoolBusiness();
            _adeBusiness = new AdeBusiness(AdeUnitWorkContext);
            _reportBusiness = new ReportBusiness();
            _logger = ObjectFactory.GetInstance<ISunnetLog>();
        }

        #region Student reports

        [CLIUrlAuthorizeAttribute(Account = Authority.Index, PageId = PagesModel.CPALLS, Anonymity = Anonymous.Verified, Parameter = "assessmentId")]
        public ActionResult StudentReports(int assessmentId, StudentAssessmentLanguage language, int year, int classId, int id,
            string waves, string measures, string reportType, bool entireClass, bool printComments, string parentReportType, string customScores,
            DateTime? dobStartDate, DateTime? dobEndDate, bool export = true)
        {
            ///    Exception message: Cannot redirect after HTTP headers have been sent.
            ///    为了不让系统报错，所以增加了try catch
            try
            {
                Dictionary<Wave, IEnumerable<int>> waveMeasures = new Dictionary<Wave, IEnumerable<int>>();
                IList<int> selectWaves = JsonHelper.DeserializeObject<List<int>>(waves);
                IList<int> selectMeasures = JsonHelper.DeserializeObject<IList<int>>(measures);

                DateTime dobStart = dobStartDate ?? CommonAgent.MinDate;
                DateTime dobEnd = dobEndDate ?? DateTime.Now;
                if (!entireClass)
                {
                    dobStart = CommonAgent.MinDate;
                    dobEnd = DateTime.Now;
                }

                foreach (int selectWave in selectWaves)
                {
                    waveMeasures.Add((Wave)selectWave, selectMeasures);
                }

                if (reportType.ToLower() == "summary")
                {
                    StudentSummaryReport(assessmentId, language, year, classId, id, waveMeasures, entireClass, printComments, dobStart, dobEnd,
                       export);
                }
                else if (reportType.ToLower() == "percentilerank")
                {
                    StudentPercentileRankReport(assessmentId, language, year, classId, id, waveMeasures, entireClass, printComments, dobStart, dobEnd,
                       export);
                }
                else if (reportType.ToLower() == "parent")
                {
                    id = entireClass ? 0 : id;
                    List<int> scoreIds = JsonHelper.DeserializeObject<List<int>>(customScores);
                    StudentParentReport(assessmentId, language, year, classId, id, waveMeasures, scoreIds, parentReportType, dobStart, dobEnd, export);
                }
                return null;
            }
            catch (Exception ex)
            {
                return View("NoData");
            }

        }

        private ActionResult StudentSummaryReport(int assessmentId, StudentAssessmentLanguage language, int year, int classId, int id,
            Dictionary<Wave, IEnumerable<int>> waveMeasures,
            bool entireClass, bool printComments, DateTime dobStart, DateTime dobEnd, bool export = true)
        {

            var assessment = _adeBusiness.GetBaseAssessmentModel(assessmentId);
            if (assessment == null) return View("NoData");
            ViewBag.AssessmentName = assessment.Name;

            var legend = _adeBusiness.GetAssessmentLegend(assessmentId, LegendTypeEnum.StudentSummary);
            ViewBag.AssessmentLegend = legend;

            var class1 = _classBusiness.GetClassForCpalls(classId);

            ViewBag.Title = "Class Scores";
            ViewBag.District = class1.School.CommunitiesText;
            ViewBag.School = class1.School.Name;
            ViewBag.Class = class1.ClassName;
            ViewBag.Teacher = string.Join(", ", _classBusiness.GetTeachers(classId));
            ViewBag.Language = assessment.Language.ToDescription();
            ViewBag.ScoolYear = year.ToFullSchoolYearString();
            ViewBag.ReportFirstColumn = _reportFirstColumnTitle;

            var student = _studentBusiness.GetStudentModel(id, UserInfo);
            ViewBag.Student = student.FirstName + " " + student.LastName;

            id = entireClass ? 0 : id;
            Dictionary<object, List<ReportRowModel>> datas = _cpallsBusiness.GetStudentReport(assessmentId, language,
                printComments, UserInfo, year, class1.School.ID, classId, id, waveMeasures, dobStart, dobEnd);
            ViewBag.Datas = datas.ToDictionary(x => (int)x.Key, x => x.Value);
            ViewBag.Pdf = export;
            if (export)
            {
                GetPdf(GetViewHtml("Index"), "Student_Summary", PdfType.Assessment_Portrait);
            }
            return View();
        }

        private ActionResult StudentPercentileRankReport(int assessmentId, StudentAssessmentLanguage language, int year, int classId, int id,
            Dictionary<Wave, IEnumerable<int>> waveMeasures,
            bool entireClass, bool printComments, DateTime dobStart, DateTime dobEnd, bool export = true)
        {

            var assessment = _adeBusiness.GetBaseAssessmentModel(assessmentId);
            if (assessment == null) return View("NoData");
            ViewBag.AssessmentName = assessment.Name;

            var class1 = _classBusiness.GetClassForCpalls(classId);

            ViewBag.Title = "Class Scores";
            ViewBag.District = class1.School.CommunitiesText;
            ViewBag.School = class1.School.Name;
            ViewBag.Class = class1.ClassName;
            ViewBag.Teacher = string.Join(", ", _classBusiness.GetTeachers(classId));
            ViewBag.Language = assessment.Language.ToDescription();
            ViewBag.ScoolYear = year.ToFullSchoolYearString();
            ViewBag.ReportFirstColumn = _reportFirstColumnTitle;

            var student = _studentBusiness.GetStudentModel(id, UserInfo);
            ViewBag.Student = student.FirstName + " " + student.LastName;

            id = entireClass ? 0 : id;
            Dictionary<object, List<ReportRowModel>> datas = _cpallsBusiness.GetStudentPercentileRankReport(assessmentId, language,
                printComments, UserInfo, year, class1.School.ID, classId, id, waveMeasures, dobStart, dobEnd);
            ViewBag.Datas = datas.ToDictionary(x => (int)x.Key, x => x.Value);
            ViewBag.Pdf = export;
            if (export)
            {
                GetPdf(GetViewHtml("StudentPercentileRankPdf"), "Student_PercentileRank", PdfType.Assessment_Portrait);
            }
            return View();
        }

        [HttpPost]
        [CLIUrlAuthorizeAttribute(Account = Authority.Index, PageId = PagesModel.CPALLS, Anonymity = Anonymous.Verified, Parameter = "assessmentId")]
        private string StudentParentReport(int assessmentId, StudentAssessmentLanguage language, int year, int classId, int studentId,
               Dictionary<Wave, IEnumerable<int>> waveMeasures, List<int> scoreIds, string parentReportType, DateTime dobStart, DateTime dobEnd, bool export = true)
        {

            if (parentReportType.ToLower() == "onlyreport")
            {
                return StudentParentOnlyReport(assessmentId, language, year, classId, studentId, waveMeasures, scoreIds, dobStart, dobEnd);
            }
            if (parentReportType.ToLower() == "pin")
            {
                return StudentParentPinPageReport(assessmentId, year, classId, studentId, dobStart, dobEnd);
            }

            OperationResult res = new OperationResult(OperationResultType.Success);
            var assessment = _adeBusiness.GetBaseAssessmentModel(assessmentId);
            if (assessment == null) return GetViewHtml("NoData");
            ViewBag.AssessmentName = assessment.Name;

            var parentLegend = _adeBusiness.GetAssessmentLegend(assessmentId, LegendTypeEnum.ParentReport);
            var class1 = _classBusiness.GetClassForCpalls(classId);
            ViewBag.Title = "Class Scores";
            ViewBag.District = class1.School.CommunitiesText;
            ViewBag.School = class1.School.Name;
            ViewBag.Class = class1.ClassName;
            ViewBag.Teacher = string.Join(", ", _classBusiness.GetTeachers(classId));
            ViewBag.Language = assessment.Language.ToDescription();
            ViewBag.ScoolYear = year.ToFullSchoolYearString();
            ViewBag.ReportFirstColumn = _reportFirstColumnTitle;
            var studentIds = new List<int>();

            bool selectWave1 = waveMeasures.ContainsKey(Wave.BOY);
            bool selectWave2 = waveMeasures.ContainsKey(Wave.MOY);
            bool selectWave3 = waveMeasures.ContainsKey(Wave.EOY);

            if (studentId == 0)
            {
                studentIds = _studentBusiness.GetStudentIdsByDOB(classId, dobStart, dobEnd);
            }
            else
            {
                studentIds.Add(studentId);
            }

            List<Wave> waves = new List<Wave>();
            if (selectWave1) waves.Add(Wave.BOY);
            if (selectWave2) waves.Add(Wave.MOY);
            if (selectWave3) waves.Add(Wave.EOY);
            List<ScoreReportModel> scoreReportModels = _adeBusiness.GetAllFinalResult(studentIds, assessmentId, waves, year.ToSchoolYearString(),
                scoreIds, DateTime.MinValue, DateTime.MaxValue);
            List<AdeLinkEntity> scoreLinks = _adeBusiness.GetLinks<ScoreEntity>(scoreIds.ToArray());

            var reports = new List<ParentReportEntity>();
            foreach (var id in studentIds)
            {
                var student = _studentBusiness.GetStudentModel(id, UserInfo);
                string waveNames = "";
                if (waveMeasures.Count > 0)
                {
                    foreach (Wave key in waveMeasures.Keys)
                    {
                        waveNames += key.ToString() + ",";
                    }
                }
                string displayName = "";
                string savePath = CreateReportName(id, waveNames.Trim(','), assessment.Name, out displayName);
                if (savePath == "")
                {
                    res.ResultType = OperationResultType.Error;
                    res.Message = "Saving report error.";
                }
                else
                {
                    try
                    {
                        string contentPage = "";
                        string contentStr = "";
                        string customScoreStr = "";
                        if (parentReportType != "pin")
                        {
                            Dictionary<object, List<ReportRowModel>> datas = _cpallsBusiness.GetStudentParentReport(assessmentId, language, false,
                            UserInfo, year, class1.School.ID, classId, id, waveMeasures);
                            var reportValues = datas.ToDictionary(x => (int)x.Key, x => x.Value);
                            contentPage = GetViewHtml("ParentReportTemplate").Replace("{MainSite}", SFConfig.MainSiteDomain);
                            contentPage = contentPage.Replace("{AssessmentName}", assessment.Name);
                            contentPage = contentPage.Replace("{StudentName}", student.FirstName + " " + student.LastName);
                            contentPage = contentPage.Replace("{AssessmentLanguage}", assessment.Language.ToString());
                            contentPage = contentPage.Replace("{ClassName}", class1.ClassName);
                            contentPage = contentPage.Replace("{TeacherName}", ViewBag.Teacher);
                            contentPage = contentPage.Replace("{SchoolName}", class1.School.Name);
                            contentPage = contentPage.Replace("{SchoolYear}", year.ToFullSchoolYearString());
                            contentPage = contentPage.Replace("{CommunityName}", class1.School.CommunitiesText);

                            foreach (var reportValue in reportValues)
                            {
                                var hasWave1 = false;
                                var hasWave2 = false;
                                var hasWave3 = false;
                                foreach (var rowModel in reportValue.Value)
                                {
                                    string strLink = "";
                                    var measureModel = _adeBusiness.GetMeasureModel(rowModel.MeasureId);
                                    if (measureModel != null && measureModel.Links != null && measureModel.Links.Count > 0)
                                    {
                                        var links = measureModel.Links.Where(l => l.LinkType == 1
                                            && l.Status == EntityStatus.Active
                                            && ((selectWave1 && l.StudentWave1) || (selectWave2 && l.StudentWave2) || (selectWave3 && l.StudentWave3))
                                            ).ToList();
                                        foreach (var lnk in links)
                                        {
                                            strLink += "<a href='" + lnk.Link + "' target='_blank'>" + lnk.DisplayText + "</a><br />";
                                        }
                                    }

                                    if (rowModel.Cells.Count < 3)
                                        continue;
                                    var titleCell = rowModel.Cells[0];
                                    var scoreCell = rowModel.Cells[2];
                                    if (rowModel.Cells[0].Text.ToString().ToLower() == "measure")
                                    {
                                        foreach (var cell in rowModel.Cells)
                                        {
                                            if (cell.Text.ToString().ToLower() == "wave 1") hasWave1 = true;
                                            else if (cell.Text.ToString().ToLower() == "wave 2") hasWave2 = true;
                                            else if (cell.Text.ToString().ToLower() == "wave 3") hasWave3 = true;
                                        }
                                    }
                                    else if (rowModel.Cells[0].Text.ToString().ToLower() == "overall measure")
                                    {
                                        var v1 = "";
                                        var v2 = "";
                                        var v3 = "";
                                        var A1 = "";
                                        var A2 = "";
                                        var A3 = "";
                                        if (hasWave1 && hasWave2 && hasWave3)
                                        {
                                            v1 = rowModel.Cells[2].Text.ToString(); A1 = rowModel.Cells[2].AlertText;
                                            v2 = rowModel.Cells[3].Text.ToString(); A2 = rowModel.Cells[3].AlertText;
                                            v3 = rowModel.Cells[4].Text.ToString(); A3 = rowModel.Cells[4].AlertText;
                                        }
                                        else if (hasWave1 && hasWave2)
                                        {
                                            v1 = rowModel.Cells[2].Text.ToString(); A1 = rowModel.Cells[2].AlertText;
                                            v2 = rowModel.Cells[3].Text.ToString(); A2 = rowModel.Cells[3].AlertText;
                                        }
                                        else if (hasWave1 && hasWave3)
                                        {
                                            v1 = rowModel.Cells[2].Text.ToString(); A1 = rowModel.Cells[2].AlertText;
                                            v3 = rowModel.Cells[3].Text.ToString(); A3 = rowModel.Cells[3].AlertText;
                                        }
                                        else if (hasWave2 && hasWave3)
                                        {
                                            v2 = rowModel.Cells[2].Text.ToString(); A2 = rowModel.Cells[2].AlertText;
                                            v3 = rowModel.Cells[3].Text.ToString(); A3 = rowModel.Cells[3].AlertText;
                                        }
                                        else if (hasWave1)
                                        {
                                            v1 = rowModel.Cells[2].Text.ToString(); A1 = rowModel.Cells[2].AlertText;
                                        }
                                        else if (hasWave2)
                                        {
                                            v2 = rowModel.Cells[2].Text.ToString(); A2 = rowModel.Cells[2].AlertText;
                                        }
                                        else if (hasWave3)
                                        {
                                            v3 = rowModel.Cells[2].Text.ToString(); A3 = rowModel.Cells[2].AlertText;
                                        }
                                        contentStr = contentStr.Replace("{wave1Total}", v1 == null ? "" : v1);
                                        contentStr = contentStr.Replace("{wave1Tota2}", v2 == null ? "" : v2);
                                        contentStr = contentStr.Replace("{wave1Tota3}", v3 == null ? "" : v3);
                                        contentStr = contentStr.Replace("{AlertTotal}", A1 == null ? "" : A1);
                                        contentStr = contentStr.Replace("{AlertTota2}", A2 == null ? "" : A2);
                                        contentStr = contentStr.Replace("{AlertTota3}", A3 == null ? "" : A3);
                                    }
                                    else if (titleCell.Rowspan == 1 && titleCell.Colspan == 2) //独立measure
                                    {
                                        contentStr += @"<tr><td><h5>" + titleCell.Text + @"</h5><p>" + titleCell.Description +
                                                     @"</p></td>";
                                        contentStr += SetWaveCellHtml(hasWave1, hasWave2, hasWave3, false, rowModel);


                                        contentStr += "<td>" + strLink + "</td></tr>";
                                    }
                                    else if (titleCell.Rowspan > 1) //父measure
                                    {
                                        scoreCell = rowModel.Cells[3];
                                        var parentTitleCell = rowModel.Cells[0]; //父节点名称
                                        titleCell = rowModel.Cells[1];
                                        string strParentLink = "";
                                        var parentMeasureModel = _adeBusiness.GetMeasureModel(rowModel.ParentMeasureId);
                                        if (parentMeasureModel != null && parentMeasureModel.Links != null && parentMeasureModel.Links.Count > 0)
                                        {
                                            foreach (var lnk in parentMeasureModel.Links)
                                            {
                                                if (lnk.LinkType == 1 && lnk.Status == EntityStatus.Active)
                                                    strParentLink += "<a href='" + lnk.Link + "' target='_blank'>" + lnk.DisplayText + "</a><br />";
                                            }
                                        }

                                        contentStr += @"<tr><td><h5>" + parentTitleCell.Text + @"</h5><p>" + parentTitleCell.Description + @"</p></td>";
                                        contentStr += SetWaveCellHtml(hasWave1, hasWave2, hasWave3, true, rowModel, true);
                                        contentStr += "<td>" + strParentLink + "</td></tr>";

                                        contentStr += @"<tr><td style=""padding-left:40px""><p>" + titleCell.Text + @"</p><p>" + titleCell.Description + @"</p></td>";
                                        contentStr += SetWaveCellHtml(hasWave1, hasWave2, hasWave3, true, rowModel);
                                        contentStr += "<td>" + strLink + "</td></tr>";



                                    }
                                    else if (titleCell.Rowspan == 1 && titleCell.Colspan == 1 && rowModel.Cells.Count >= 4)
                                    {
                                        scoreCell = rowModel.Cells[3];
                                        if (rowModel.Cells[1].Description == null)
                                        {
                                            titleCell = rowModel.Cells[0];
                                            contentStr += @"<tr><td style=""padding-left:40px""><p>" + titleCell.Text + @"</p><p>" + titleCell.Description + @"</p></td>";
                                            contentStr += SetWaveCellHtml(hasWave1, hasWave2, hasWave3, true, rowModel);
                                            contentStr += "<td>" + strLink + "</td></tr>";
                                        }
                                        else
                                        {
                                            titleCell = rowModel.Cells[1];
                                            contentStr += @"<tr><td ><p>" + titleCell.Text + @"</p><p>" + titleCell.Description + @"</p></td>";
                                            contentStr += SetWaveCellHtml(hasWave1, hasWave2, hasWave3, true, rowModel);
                                            contentStr += "<td>" + strLink + "</td></tr>";
                                        }
                                    }
                                    else if (titleCell.Colspan == 1) //子measure
                                    {
                                        contentStr += @"<tr><td style=""padding-left:40px""><p>" + titleCell.Text +
                                                      @"</p><p>" + titleCell.Description + @"</p></td>";
                                        contentStr += SetWaveCellHtml(hasWave1, hasWave2, hasWave3, false, rowModel);
                                        contentStr += "<td>" + strLink + "</td></tr>";
                                    }
                                }
                            }

                            StringBuilder scoreHeder = new StringBuilder();
                            scoreHeder.Append("<table cellspacing='0' cellpadding='0' width='100%'>");
                            scoreHeder.Append("<tr>");
                            scoreHeder.Append("<th width='59%' class='top'>");
                            scoreHeder.Append("<h3>Custom Scores Area</h3>");
                            scoreHeder.Append("<p style='color: red'>Pending (Guiding Text)</p>");
                            scoreHeder.Append("</th>");
                            scoreHeder.Append("<th colspan='4' height='30px'></th>");
                            scoreHeder.Append("</tr>");
                            foreach (int scoreId in scoreIds)
                            {
                                List<AdeLinkEntity> links = new List<AdeLinkEntity>();
                                if (selectWave3)
                                {
                                    links = scoreLinks.Where(x => x.HostId == scoreId && x.MeasureWave3).ToList();
                                }
                                else if (selectWave2)
                                {
                                    links = scoreLinks.Where(x => x.HostId == scoreId && x.MeasureWave2).ToList();
                                }
                                else
                                {
                                    links = scoreLinks.Where(x => x.HostId == scoreId && x.MeasureWave1).ToList();
                                }
                                string strScoreLink = "";
                                foreach (var lnk in links)
                                {
                                    strScoreLink += "<a href='" + lnk.Link + "' target='_blank'>" + lnk.DisplayText + "</a><br />";
                                }

                                var studentScoreModels = scoreReportModels.Where(x => x.StudentId == id && x.ScoreId == scoreId).ToList();
                                if (studentScoreModels.Any())
                                {
                                    customScoreStr += scoreHeder;
                                    customScoreStr += "<tr>";
                                    customScoreStr += "<th width='59%' style='font-weight:bold; text-align:left; padding: 5px; border-left:0;'>" + studentScoreModels.FirstOrDefault().ScoreDomain + "</th>";
                                    customScoreStr += "<th width='7%'>BOY</th>";
                                    customScoreStr += "<th width='7%'>MOY</th>";
                                    customScoreStr += "<th width='7%'>EOY</th>";
                                    customScoreStr += "<th width='20%' style='border-right:1px solid #ccc;'>Links</th>";
                                    customScoreStr += "</tr>";

                                    customScoreStr += "<tr>";
                                    customScoreStr += "<td style='word-break: break-all; word-wrap:break-all;'>" + studentScoreModels.FirstOrDefault().ScoreDescription + "</td>";
                                    string htmlW1 = "<td>*</td>";
                                    string htmlW2 = "<td>*</td>";
                                    string htmlW3 = "<td>*</td>";
                                    foreach (ScoreReportModel model in studentScoreModels)
                                    {
                                        string finalScore = model.FinalScore == null ? "*" : model.FinalScore.Value.ToString("f" + model.TargetRound);
                                        if (model.Wave == Wave.BOY)
                                        {
                                            htmlW1 = "<td>" + finalScore + "</br>" + model.LabelText + "</td>";
                                        }
                                        else if (model.Wave == Wave.MOY)
                                        {
                                            htmlW2 = "<td>" + finalScore + "</br>" + model.LabelText + "</td>";
                                        }
                                        else if (model.Wave == Wave.EOY)
                                        {
                                            htmlW3 = "<td>" + finalScore + "</br>" + model.LabelText + "</td>";
                                        }
                                    }

                                    customScoreStr += htmlW1;
                                    customScoreStr += htmlW2;
                                    customScoreStr += htmlW3;
                                    customScoreStr += "<td>" + strScoreLink + "</td>";
                                    customScoreStr += "</tr>";
                                    customScoreStr += "</table>";
                                    customScoreStr += "<div style='height:20px'></div>";
                                }
                            }
                        }

                        contentPage = contentPage.Replace("{ReportBody}", contentStr);
                        contentPage = contentPage.Replace("{CustomScoreReportBody}", customScoreStr);
                        string startPdfFilePath = "";
                        if (assessment.ParentReportCoverPath.Length > 0)
                            startPdfFilePath = SFConfig.UploadFile + assessment.ParentReportCoverPath;

                        if (parentLegend != null)
                        {
                            string legendBody =
                                string.Format(
                                    "<tr style='page-break-inside:avoid;'><td style='border: 0;'>{0}<img style='width: 100%;display:block;' src='{1}'/>{2}</td></tr> ",
                                    parentLegend.TextPosition == "Top" ? parentLegend.Text : ""
                                    , SFConfig.StaticDomain + "upload/" + parentLegend.ColorFilePath
                                    , parentLegend.TextPosition == "Bottom" ? parentLegend.Text : "");
                            contentPage = contentPage.Replace("{LegendBody}", legendBody);
                        }
                        else
                        {
                            contentPage = contentPage.Replace("{LegendBody}", "");
                        }
                        SavePdf(contentPage, savePath, assessment.Language, PdfType.Assessment_Portrait, startPdfFilePath);
                        ParentReportEntity item = new ParentReportEntity();
                        item.AssessmentId = assessmentId;
                        item.AssessmentName = assessment.Name;
                        item.SchoolId = class1.School.ID;
                        item.StudentId = id;
                        item.ReportName = displayName;
                        item.CreatedOn = DateTime.Now;
                        item.UpdatedOn = DateTime.Now;
                        item.Status = EntityStatus.Active;
                        item.CreatedBy = UserInfo.ID;
                        reports.Add(item);
                    }
                    catch (Exception ex)
                    {
                        res.Message = ex.ToString();
                        res.ResultType = OperationResultType.Error;
                    }

                }
            }

            if (parentReportType.ToLower() == "forparent" && res.ResultType == OperationResultType.Success)
            {
                var list = reports.Where(c => c.AssessmentName != null).ToList();
                res = _reportBusiness.InsertParentReport(list);
            }
            if (res.ResultType == OperationResultType.Success)
            {
                Document exportPdf = new Document();
                exportPdf.AutoCloseAppendedDocs = true;
                exportPdf.LicenseKey = SFConfig.EVOPDFKEY;
                try
                {
                    foreach (var report in reports)
                    {
                        if (!report.ReportName.Contains("PinReport_content") || parentReportType.ToLower() != "forparent")
                        {
                            string directory = Path.Combine(SFConfig.ProtectedFiles, string.Format("ParentReports/{0}/{1}", CommonAgent.SchoolYear, report.StudentId));
                            var filePath = Path.Combine(directory, string.Format("{0}.pdf", report.ReportName));
                            Document tmpDocument = new Document(filePath);
                            exportPdf.AppendDocument(tmpDocument);
                        }
                    }

                    byte[] outPdfBuffer = exportPdf.Save();
                    Response.AddHeader("Content-Type", "application/pdf");
                    Response.AddHeader("Content-Disposition", String.Format("attachment; filename=ParentReport.pdf; size={0}", outPdfBuffer.Length.ToString()));
                    Response.BinaryWrite(outPdfBuffer);
                    Response.End();

                }
                finally
                {
                    exportPdf.Close();
                }

            }
            var response = new PostFormResponse();
            response.Success = res.ResultType == OperationResultType.Success;
            response.Message = res.Message;
            return JsonConvert.SerializeObject(response);
        }

        private string StudentParentOnlyReport(int assessmentId, StudentAssessmentLanguage language, int year, int classId, int studentId,
         Dictionary<Wave, IEnumerable<int>> waveMeasures, List<int> scoreIds, DateTime dobStart, DateTime dobEnd)
        {
            OperationResult res = new OperationResult(OperationResultType.Success);
            var assessment = _adeBusiness.GetBaseAssessmentModel(assessmentId);
            if (assessment == null) return GetViewHtml("NoData");
            ViewBag.AssessmentName = assessment.Name;
            var parentLegend = _adeBusiness.GetAssessmentLegend(assessmentId, LegendTypeEnum.ParentReport);
            var class1 = _classBusiness.GetClassForCpalls(classId);
            ViewBag.Title = "Class Scores";
            ViewBag.District = class1.School.CommunitiesText;
            ViewBag.School = class1.School.Name;
            ViewBag.Class = class1.ClassName;
            ViewBag.Teacher = string.Join(", ", _classBusiness.GetTeachers(classId));
            ViewBag.Language = assessment.Language.ToDescription();
            ViewBag.ScoolYear = year.ToFullSchoolYearString();
            ViewBag.ReportFirstColumn = _reportFirstColumnTitle;
            var studentIds = new List<int>();
            bool selectWave1 = waveMeasures.ContainsKey(Wave.BOY);
            bool selectWave2 = waveMeasures.ContainsKey(Wave.MOY);
            bool selectWave3 = waveMeasures.ContainsKey(Wave.EOY);

            if (studentId == 0)
            {
                studentIds = _studentBusiness.GetStudentIdsByDOB(classId, language, dobStart, dobEnd);
            }
            else
            {
                studentIds.Add(studentId);
            }

            List<Wave> waves = new List<Wave>();
            if (selectWave1) waves.Add(Wave.BOY);
            if (selectWave2) waves.Add(Wave.MOY);
            if (selectWave3) waves.Add(Wave.EOY);
            List<ScoreReportModel> scoreReportModels = _adeBusiness.GetAllFinalResult(studentIds, assessmentId, waves, year.ToSchoolYearString(),
                scoreIds, DateTime.MinValue, DateTime.MaxValue);
            List<AdeLinkEntity> scoreLinks = _adeBusiness.GetLinks<ScoreEntity>(scoreIds.ToArray());

            var reports = new List<ParentReportEntity>();
            foreach (var id in studentIds)
            {
                var student = _studentBusiness.GetStudentModel(id, UserInfo);
                string waveNames = "";

                if (waveMeasures.Count > 0)
                {
                    foreach (Wave key in waveMeasures.Keys)
                    {
                        waveNames += key.ToString() + ",";
                    }
                }
                string displayName = "";
                string savePath = CreateReportName(id, waveNames.Trim(','), assessment.Name, out displayName);
                if (savePath == "")
                {
                    res.ResultType = OperationResultType.Error;
                    res.Message = "Saving report error.";
                }
                else
                {
                    try
                    {
                        string contentPage = "";
                        string contentStr = "";
                        string customScoreStr = "";
                        Dictionary<object, List<ReportRowModel>> datas = _cpallsBusiness.GetStudentParentReport(assessmentId, language, false,
                           UserInfo, year, class1.School.ID, classId, id, waveMeasures);
                        var reportValues = datas.ToDictionary(x => (int)x.Key, x => x.Value);
                        contentPage = GetViewHtml("ParentReportTemplate").Replace("{MainSite}", SFConfig.MainSiteDomain);
                        contentPage = contentPage.Replace("{AssessmentName}", assessment.Name);
                        contentPage = contentPage.Replace("{StudentName}", student.FirstName + " " + student.LastName);
                        contentPage = contentPage.Replace("{AssessmentLanguage}", assessment.Language.ToString());
                        contentPage = contentPage.Replace("{ClassName}", class1.ClassName);
                        contentPage = contentPage.Replace("{TeacherName}", ViewBag.Teacher);
                        contentPage = contentPage.Replace("{SchoolName}", class1.School.Name);
                        contentPage = contentPage.Replace("{SchoolYear}", year.ToFullSchoolYearString());
                        contentPage = contentPage.Replace("{CommunityName}", class1.School.CommunitiesText);

                        foreach (var reportValue in reportValues)
                        {
                            var hasWave1 = false;
                            var hasWave2 = false;
                            var hasWave3 = false;
                            foreach (var rowModel in reportValue.Value)
                            {
                                string strLink = "";
                                var measureModel = _adeBusiness.GetMeasureModel(rowModel.MeasureId);
                                if (measureModel != null && measureModel.Links != null && measureModel.Links.Count > 0)
                                {
                                    var links = measureModel.Links.Where(l => l.LinkType == 1
                                        && l.Status == EntityStatus.Active
                                        && ((selectWave1 && l.StudentWave1) || (selectWave2 && l.StudentWave2) || (selectWave3 && l.StudentWave3))
                                        ).ToList();
                                    foreach (var lnk in links)
                                    {
                                        strLink += "<a href='" + lnk.Link + "' target='_blank'>" + lnk.DisplayText + "</a><br />";
                                    }
                                }

                                if (rowModel.Cells.Count < 3)
                                    continue;
                                var titleCell = rowModel.Cells[0];
                                var scoreCell = rowModel.Cells[2];
                                if (rowModel.Cells[0].Text.ToString().ToLower() == "measure")
                                {
                                    foreach (var cell in rowModel.Cells)
                                    {
                                        if (cell.Text.ToString().ToLower() == "wave 1") hasWave1 = true;
                                        else if (cell.Text.ToString().ToLower() == "wave 2") hasWave2 = true;
                                        else if (cell.Text.ToString().ToLower() == "wave 3") hasWave3 = true;
                                    }
                                }
                                else if (rowModel.Cells[0].Text.ToString().ToLower() == "overall measure")
                                //  else if (rowModel.Cells[0].IsParent)
                                {
                                    var v1 = "";
                                    var v2 = "";
                                    var v3 = "";
                                    var A1 = "";
                                    var A2 = "";
                                    var A3 = "";
                                    if (hasWave1 && hasWave2 && hasWave3)
                                    {
                                        v1 = rowModel.Cells[2].Text.ToString(); A1 = rowModel.Cells[2].AlertText;
                                        v2 = rowModel.Cells[3].Text.ToString(); A2 = rowModel.Cells[3].AlertText;
                                        v3 = rowModel.Cells[4].Text.ToString(); A3 = rowModel.Cells[4].AlertText;
                                    }
                                    else if (hasWave1 && hasWave2)
                                    {
                                        v1 = rowModel.Cells[2].Text.ToString(); A1 = rowModel.Cells[2].AlertText;
                                        v2 = rowModel.Cells[3].Text.ToString(); A2 = rowModel.Cells[3].AlertText;
                                    }
                                    else if (hasWave1 && hasWave3)
                                    {
                                        v1 = rowModel.Cells[2].Text.ToString(); A1 = rowModel.Cells[2].AlertText;
                                        v3 = rowModel.Cells[3].Text.ToString(); A3 = rowModel.Cells[3].AlertText;
                                    }
                                    else if (hasWave2 && hasWave3)
                                    {
                                        v2 = rowModel.Cells[2].Text.ToString(); A2 = rowModel.Cells[2].AlertText;
                                        v3 = rowModel.Cells[3].Text.ToString(); A3 = rowModel.Cells[3].AlertText;
                                    }
                                    else if (hasWave1)
                                    {
                                        v1 = rowModel.Cells[2].Text.ToString(); A1 = rowModel.Cells[2].AlertText;
                                    }
                                    else if (hasWave2)
                                    {
                                        v2 = rowModel.Cells[2].Text.ToString(); A2 = rowModel.Cells[2].AlertText;
                                    }
                                    else if (hasWave3)
                                    {
                                        v3 = rowModel.Cells[2].Text.ToString(); A3 = rowModel.Cells[2].AlertText;
                                    }
                                    contentStr = contentStr.Replace("{wave1Total}", v1 == null ? "" : v1);
                                    contentStr = contentStr.Replace("{wave1Tota2}", v2 == null ? "" : v2);
                                    contentStr = contentStr.Replace("{wave1Tota3}", v3 == null ? "" : v3);
                                    contentStr = contentStr.Replace("{AlertTotal}", A1 == null ? "" : A1);
                                    contentStr = contentStr.Replace("{AlertTota2}", A2 == null ? "" : A2);
                                    contentStr = contentStr.Replace("{AlertTota3}", A3 == null ? "" : A3);
                                }
                                else if (titleCell.Rowspan == 1 && titleCell.Colspan == 2) //独立measure
                                {
                                    contentStr += @"<tr><td><h5>" + titleCell.Text + @"</h5><p>" + titleCell.Description +
                                                 @"</p></td>";
                                    contentStr += SetWaveCellHtml(hasWave1, hasWave2, hasWave3, false, rowModel);


                                    contentStr += "<td>" + strLink + "</td></tr>";
                                }
                                else if (titleCell.Rowspan > 1) //父measure
                                {
                                    scoreCell = rowModel.Cells[3];
                                    var parentTitleCell = rowModel.Cells[0]; //父节点名称
                                    titleCell = rowModel.Cells[1];
                                    string strParentLink = "";
                                    var parentMeasureModel = _adeBusiness.GetMeasureModel(rowModel.ParentMeasureId);
                                    if (parentMeasureModel != null && parentMeasureModel.Links != null && parentMeasureModel.Links.Count > 0)
                                    {
                                        foreach (var lnk in parentMeasureModel.Links)
                                        {
                                            if (lnk.LinkType == 1 && lnk.Status == EntityStatus.Active)
                                                strParentLink += "<a href='" + lnk.Link + "' target='_blank'>" + lnk.DisplayText + "</a><br />";
                                        }
                                    }

                                    contentStr += @"<tr><td><h5>" + parentTitleCell.Text + @"</h5><p>" + parentTitleCell.Description + @"</p></td>";
                                    contentStr += SetWaveCellHtml(hasWave1, hasWave2, hasWave3, true, rowModel, true);
                                    contentStr += "<td>" + strParentLink + "</td></tr>";

                                    contentStr += @"<tr><td style=""padding-left:40px""><p>" + titleCell.Text + @"</p><p>" + titleCell.Description + @"</p></td>";
                                    contentStr += SetWaveCellHtml(hasWave1, hasWave2, hasWave3, true, rowModel);
                                    contentStr += "<td>" + strLink + "</td></tr>";



                                }
                                else if (titleCell.Rowspan == 1 && titleCell.Colspan == 1 && rowModel.Cells.Count >= 4)
                                {
                                    scoreCell = rowModel.Cells[3];
                                    if (rowModel.Cells[1].Description == null)
                                    {
                                        titleCell = rowModel.Cells[0];
                                        contentStr += @"<tr><td style=""padding-left:40px""><p>" + titleCell.Text + @"</p><p>" + titleCell.Description + @"</p></td>";
                                        contentStr += SetWaveCellHtml(hasWave1, hasWave2, hasWave3, true, rowModel);
                                        contentStr += "<td>" + strLink + "</td></tr>";
                                    }
                                    else
                                    {
                                        titleCell = rowModel.Cells[1];
                                        contentStr += @"<tr><td ><p>" + titleCell.Text + @"</p><p>" + titleCell.Description + @"</p></td>";
                                        contentStr += SetWaveCellHtml(hasWave1, hasWave2, hasWave3, true, rowModel);
                                        contentStr += "<td>" + strLink + "</td></tr>";
                                    }
                                }
                                else if (titleCell.Colspan == 1) //子measure
                                {
                                    contentStr += @"<tr><td style=""padding-left:40px""><p>" + titleCell.Text +
                                                  @"</p><p>" + titleCell.Description + @"</p></td>";
                                    contentStr += SetWaveCellHtml(hasWave1, hasWave2, hasWave3, false, rowModel);
                                    contentStr += "<td>" + strLink + "</td></tr>";
                                }
                            }
                        }

                        StringBuilder scoreHeder = new StringBuilder();
                        scoreHeder.Append("<table cellspacing='0' cellpadding='0' width='100%'>");
                        scoreHeder.Append("<tr>");
                        scoreHeder.Append("<th width='59%' class='top'>");
                        scoreHeder.Append("<h3>Custom Scores Area</h3>");
                        scoreHeder.Append("<p style='color: red'>Pending (Guiding Text)</p>");
                        scoreHeder.Append("</th>");
                        scoreHeder.Append("<th colspan='4' height='30px'></th>");
                        scoreHeder.Append("</tr>");
                        foreach (int scoreId in scoreIds)
                        {
                            List<AdeLinkEntity> links = new List<AdeLinkEntity>();
                            if (selectWave3)
                            {
                                links = scoreLinks.Where(x => x.HostId == scoreId && x.MeasureWave3).ToList();
                            }
                            else if (selectWave2)
                            {
                                links = scoreLinks.Where(x => x.HostId == scoreId && x.MeasureWave2).ToList();
                            }
                            else
                            {
                                links = scoreLinks.Where(x => x.HostId == scoreId && x.MeasureWave1).ToList();
                            }
                            string strScoreLink = "";
                            foreach (var lnk in links)
                            {
                                strScoreLink += "<a href='" + lnk.Link + "' target='_blank'>" + lnk.DisplayText + "</a><br />";
                            }

                            var studentScoreModels = scoreReportModels.Where(x => x.StudentId == id && x.ScoreId == scoreId).ToList();
                            if (studentScoreModels.Any())
                            {
                                customScoreStr += scoreHeder;
                                customScoreStr += "<tr>";
                                customScoreStr += "<th width='59%' style='font-weight:bold; text-align:left; padding: 5px; border-left:0;'>" + studentScoreModels.FirstOrDefault().ScoreDomain + "</th>";
                                customScoreStr += "<th width='7%'>BOY</th>";
                                customScoreStr += "<th width='7%'>MOY</th>";
                                customScoreStr += "<th width='7%'>EOY</th>";
                                customScoreStr += "<th width='20%' style='border-right:1px solid #ccc;'>Links</th>";
                                customScoreStr += "</tr>";

                                customScoreStr += "<tr>";
                                customScoreStr += "<td style='word-break: break-all; word-wrap:break-all;'>" + studentScoreModels.FirstOrDefault().ScoreDescription + "</td>";
                                string htmlW1 = "<td>*</td>";
                                string htmlW2 = "<td>*</td>";
                                string htmlW3 = "<td>*</td>";
                                foreach (ScoreReportModel model in studentScoreModels)
                                {
                                    string finalScore = model.FinalScore == null ? "*" : model.FinalScore.Value.ToString("f" + model.TargetRound);
                                    if (model.Wave == Wave.BOY)
                                    {
                                        htmlW1 = "<td>" + finalScore + "</br>" + model.LabelText + "</td>";
                                    }
                                    else if (model.Wave == Wave.MOY)
                                    {
                                        htmlW2 = "<td>" + finalScore + "</br>" + model.LabelText + "</td>";
                                    }
                                    else if (model.Wave == Wave.EOY)
                                    {
                                        htmlW3 = "<td>" + finalScore + "</br>" + model.LabelText + "</td>";
                                    }
                                }
                                customScoreStr += htmlW1;
                                customScoreStr += htmlW2;
                                customScoreStr += htmlW3;
                                customScoreStr += "<td>" + strScoreLink + "</td>";
                                customScoreStr += "</tr>";
                                customScoreStr += "</table>";
                                customScoreStr += "<div style='height:20px'></div>";
                            }
                        }

                        contentPage = contentPage.Replace("{ReportBody}", contentStr);
                        contentPage = contentPage.Replace("{CustomScoreReportBody}", customScoreStr);
                        string coverDisplayName = "";
                        if (assessment.ParentReportCoverPath.Length > 0)
                            coverDisplayName = SFConfig.UploadFile + assessment.ParentReportCoverPath.Substring(0, assessment.ParentReportCoverPath.LastIndexOf('.'));

                        var coverItem = new ParentReportEntity { ReportName = coverDisplayName, StudentId = id };
                        if (!string.IsNullOrEmpty(coverItem.ReportName))
                        {
                            reports.Add(coverItem);
                        }

                        var contentDisplayName = "PinReport_content_" + Guid.NewGuid().ToString().Replace("-", "");
                        var contentName = CreatePinReportTempName(id, contentDisplayName);
                        if (parentLegend != null)
                        {
                            string legendBody =
                                string.Format(
                                    "<tr style='page-break-inside:avoid;'><td style='border: 0;'>{0}<img style='width: 100%;display:block;' src='{1}'/>{2}</td></tr> ",
                                    parentLegend.TextPosition == "Top" ? parentLegend.Text : ""
                                    , SFConfig.StaticDomain + "upload/" + parentLegend.ColorFilePath
                                    , parentLegend.TextPosition == "Bottom" ? parentLegend.Text : "");
                            contentPage = contentPage.Replace("{LegendBody}", legendBody);
                        }
                        else
                        {
                            contentPage = contentPage.Replace("{LegendBody}", "");
                        }

                        SavePdf(contentPage, contentName, assessment.Language, PdfType.Assessment_Portrait, "", 612);
                        var contentItem = new ParentReportEntity { ReportName = contentDisplayName, StudentId = id };
                        reports.Add(contentItem);
                    }
                    catch (Exception ex)
                    {
                        _logger.Debug(ex);
                        res.Message = ex.ToString();
                        res.ResultType = OperationResultType.Error;
                    }

                }
            }

            if (res.ResultType == OperationResultType.Success)
            {
                Document exportPdf = new Document();
                exportPdf.AutoCloseAppendedDocs = true;
                exportPdf.LicenseKey = SFConfig.EVOPDFKEY;
                try
                {

                    foreach (var report in reports)
                    {
                        string directory = Path.Combine(SFConfig.ProtectedFiles, string.Format("ParentReports/{0}/{1}", CommonAgent.SchoolYear, report.StudentId));
                        var filePath = Path.Combine(directory, string.Format("{0}.pdf", report.ReportName));
                        Document tmpDocument = new Document(filePath);
                        exportPdf.AppendDocument(tmpDocument);
                    }

                    byte[] outPdfBuffer = exportPdf.Save();
                    Response.AddHeader("Content-Type", "application/pdf");
                    Response.AddHeader("Content-Disposition", String.Format("attachment; filename=ParentReport.pdf; size={0}", outPdfBuffer.Length.ToString()));
                    Response.BinaryWrite(outPdfBuffer);
                    Response.End();

                }
                catch (Exception ex)
                {
                    _logger.Debug(ex);
                }
                finally
                {
                    exportPdf.Close();
                }

            }
            var response = new PostFormResponse();
            response.Success = res.ResultType == OperationResultType.Success;
            response.Message = res.Message;

            return JsonConvert.SerializeObject(response);
        }

        private string StudentParentPinPageReport(int assessmentId, int year, int classId, int studentId, DateTime dobStart, DateTime dobEnd)
        {
            OperationResult res = new OperationResult(OperationResultType.Success);
            var assessment = _adeBusiness.GetBaseAssessmentModel(assessmentId);
            if (assessment == null) return GetViewHtml("NoData");
            ViewBag.AssessmentName = assessment.Name;

            var parentLegend = _adeBusiness.GetAssessmentLegend(assessmentId, LegendTypeEnum.ParentReport);
            var class1 = _classBusiness.GetClassForCpalls(classId);
            ViewBag.Title = "Class Scores";
            ViewBag.District = class1.School.CommunitiesText;
            ViewBag.School = class1.School.Name;
            ViewBag.Class = class1.ClassName;
            ViewBag.Teacher = string.Join(", ", _classBusiness.GetTeachers(classId));
            ViewBag.Language = assessment.Language.ToDescription();
            ViewBag.ScoolYear = year.ToFullSchoolYearString();
            ViewBag.ReportFirstColumn = _reportFirstColumnTitle;
            var studentIds = new List<int>();

            if (studentId == 0)
            {
                studentIds = _studentBusiness.GetStudentIdsByDOB(classId, dobStart, dobEnd);
            }
            else
            {
                studentIds.Add(studentId);
            }
            var reports = new List<ParentReportEntity>();
            string template = _studentBusiness.GenerateParentReportEmailBody();
            string body = "";
            foreach (var id in studentIds)
            {
                var student = _studentBusiness.GetStudentModel(id, UserInfo);
                try
                {

                    //var pinDisplayName = "PinReport_pin_" + Guid.NewGuid().ToString().Replace("-", "");
                    //var pinName = CreatePinReportTempName(id, pinDisplayName);
                    //var contentDisplayName = "PinReport_content_" + Guid.NewGuid().ToString().Replace("-", "");
                    //var contentName = CreatePinReportTempName(id, contentDisplayName);

                    body += template.Replace("[firstName]", student.FirstName)
              .Replace("[lastName]", student.LastName)
              .Replace("[parentCode]", student.ParentCode)
              .Replace("[staticdomain]", SFConfig.StaticDomain);

                    // var pinContent = _studentBusiness.GeneratePDFForParentReport(template,student);


                    //SavePdf(pinContent, pinName, assessment.Language, PdfType.ParentPinPage,612);
                    //var pinItem = new ParentReportEntity { ReportName = pinDisplayName, StudentId = id };
                    //reports.Add(pinItem);
                }
                catch (Exception ex)
                {
                    res.Message = ex.ToString();
                    res.ResultType = OperationResultType.Error;
                }


            }
            var pinDisplayName = "PinReport_pin_" + Guid.NewGuid().ToString().Replace("-", "");
            var pinName = CreatePinReportTempName(0, pinDisplayName);
            SavePdf(body, pinName, assessment.Language, PdfType.ParentPinPage, "", 612);
            if (res.ResultType == OperationResultType.Success)
            {
                Document exportPdf = new Document();
                exportPdf.AutoCloseAppendedDocs = true;
                exportPdf.LicenseKey = SFConfig.EVOPDFKEY;
                try
                {
                    string directory = Path.Combine(SFConfig.ProtectedFiles, string.Format("ParentReports/{0}/{1}", CommonAgent.SchoolYear, 0));
                    var filePath = Path.Combine(directory, pinName);
                    Document tmpDocument = new Document(pinName);
                    exportPdf.AppendDocument(tmpDocument);

                    byte[] outPdfBuffer = exportPdf.Save();
                    Response.AddHeader("Content-Type", "application/pdf");
                    Response.AddHeader("Content-Disposition", String.Format("attachment; filename=ParentPinPage.pdf; size={0}", outPdfBuffer.Length.ToString()));
                    Response.BinaryWrite(outPdfBuffer);
                    Response.End();
                }
                catch (Exception ex)
                {

                }

                finally
                {
                    exportPdf.Close();
                }

            }
            var response = new PostFormResponse();
            response.Success = res.ResultType == OperationResultType.Success;
            response.Message = res.Message;
            return JsonConvert.SerializeObject(response);
        }


        #endregion

        #region student summary

        [CLIUrlAuthorizeAttribute(Account = Authority.Index, PageId = PagesModel.CPALLS, Anonymity = Anonymous.Verified, Parameter = "assessmentId")]
        public ActionResult Waves(int assessmentId, int year, int classId, int id)
        {
            List<MeasureHeaderModel> measures;
            List<MeasureHeaderModel> parentMeasures;
            _cpallsBusiness.BuilderHeader(assessmentId, year, Wave.BOY, out measures, out parentMeasures, true);

            var groups = MeasureGroup.GetGroupJson(measures, parentMeasures);
            ViewBag.MeasureJson = JsonHelper.SerializeObject(groups);

            var assessment = _adeBusiness.GetAssessment(assessmentId);
            ViewBag.language = assessment.Language;

            ViewBag.TheOtherLanguage = assessment.Language == AssessmentLanguage.English
                ? AssessmentLanguage.Spanish
                : AssessmentLanguage.English;

            var student = _studentBusiness.GetStudentModel(id, UserInfo);
            ViewBag.Student = student.FirstName + " " + student.LastName;

            var theOtherLanguageAssessment = _adeBusiness.GetTheOtherLanguageAssessment(assessmentId);
            if (theOtherLanguageAssessment != null)
            {
                _cpallsBusiness.BuilderHeader(theOtherLanguageAssessment.ID, year, Wave.BOY, out measures, out parentMeasures, true);
                groups = MeasureGroup.GetGroupJson(measures, parentMeasures);
                ViewBag.MeasureJson2 = JsonHelper.SerializeObject(groups);
            }
            else
            {
                ViewBag.MeasureJson2 = "false";
            }
            return View();
        }

        //
        [CLIUrlAuthorizeAttribute(Account = Authority.Index, PageId = PagesModel.CPALLS, Anonymity = Anonymous.Verified, Parameter = "assessmentId")]
        public ActionResult StudentReportWaves(int assessmentId, int year, int classId, int id)
        {
            List<MeasureHeaderModel> measures;
            List<MeasureHeaderModel> parentMeasures;
            _cpallsBusiness.BuilderHeader(assessmentId, year, Wave.BOY, out measures, out parentMeasures, true);


            var groups = MeasureGroup.GetGroupJson(measures, parentMeasures);

            var meaGroup = groups["groups"] as List<MeasureGroup>;
            if (meaGroup != null)
            {
                meaGroup.ForEach(mg => mg.Measures.ForEach(mea => mea.Waves = "1,2,3"));
            }

            ViewBag.MeasureJson = JsonHelper.SerializeObject(groups);

            var assessment = _adeBusiness.GetAssessment(assessmentId);
            ViewBag.language = assessment.Language;

            ViewBag.TheOtherLanguage = assessment.Language == AssessmentLanguage.English
                ? AssessmentLanguage.Spanish
                : AssessmentLanguage.English;

            var student = _studentBusiness.GetStudentModel(id, UserInfo);
            ViewBag.Student = student.FirstName + " " + student.LastName;

            var theOtherLanguageAssessment = _adeBusiness.GetTheOtherLanguageAssessment(assessmentId);
            if (theOtherLanguageAssessment != null)
            {
                _cpallsBusiness.BuilderHeader(theOtherLanguageAssessment.ID, year, Wave.BOY, out measures, out parentMeasures, true);
                groups = MeasureGroup.GetGroupJson(measures, parentMeasures);
                ViewBag.MeasureJson2 = JsonHelper.SerializeObject(groups);
            }
            else
            {
                ViewBag.MeasureJson2 = "false";
            }

            ViewBag.AssessmentReports = _adeBusiness.GetAssessmentReports(assessmentId, ReportTypeEnum.Student);
            List<ScoreSelectModel> models = _adeBusiness.GetScoreSelectModels(assessmentId);
            ViewBag.JsonScores = JsonHelper.SerializeObject(models);
            return View();
        }


        [CLIUrlAuthorizeAttribute(Account = Authority.Index, PageId = PagesModel.CPALLS, Anonymity = Anonymous.Verified, Parameter = "assessmentId")]
        public ActionResult Index(int assessmentId, StudentAssessmentLanguage language, int year, int classId, int id,
            string waves, string measures, string reportType, bool entireClass, bool printComments, string parentReportType, bool export = true)
        {
            Dictionary<Wave, IEnumerable<int>> waveMeasures = new Dictionary<Wave, IEnumerable<int>>();
            IList<int> selectWaves = JsonHelper.DeserializeObject<List<int>>(waves);
            IList<int> selectMeasures = JsonHelper.DeserializeObject<IList<int>>(measures);
            foreach (int selectWave in selectWaves)
            {
                waveMeasures.Add((Wave)selectWave, selectMeasures);
            }

            var assessment = _adeBusiness.GetBaseAssessmentModel(assessmentId);
            if (assessment == null) return View("NoData");
            ViewBag.AssessmentName = assessment.Name;

            var legend = _adeBusiness.GetAssessmentLegend(assessmentId, LegendTypeEnum.StudentSummary);
            ViewBag.AssessmentLegend = legend;

            var class1 = _classBusiness.GetClassForCpalls(classId);

            ViewBag.Title = "Class Scores";
            ViewBag.District = class1.School.CommunitiesText;
            ViewBag.School = class1.School.Name;
            ViewBag.Class = class1.ClassName;
            ViewBag.Teacher = string.Join(", ", _classBusiness.GetTeachers(classId));
            ViewBag.Language = assessment.Language.ToDescription();
            ViewBag.ScoolYear = year.ToFullSchoolYearString();
            ViewBag.ReportFirstColumn = _reportFirstColumnTitle;

            var student = _studentBusiness.GetStudentModel(id, UserInfo);
            ViewBag.Student = student.FirstName + " " + student.LastName;

            id = entireClass ? 0 : id;
            Dictionary<object, List<ReportRowModel>> datas = _cpallsBusiness.GetStudentReport(assessmentId, language,
                printComments, UserInfo, year, class1.School.ID, classId, id, waveMeasures, null, null);
            ViewBag.Datas = datas.ToDictionary(x => (int)x.Key, x => x.Value);
            ViewBag.Pdf = export;
            //if (export)
            //{
            //    GetPdf(GetViewHtml("Index"), "Student_Summary", PdfType.Assessment_Portrait);
            //}

            return View();
        }



        #endregion

        #region Class Summary
        [CLIUrlAuthorize(Account = Authority.Index, PageId = PagesModel.CPALLS, Anonymity = Anonymous.Verified, Parameter = "assessmentId")]
        public ActionResult Summary(int assessmentId, int year, int classId,
            string className = "", bool export = true)
        {
            List<MeasureHeaderModel> measures;
            List<MeasureHeaderModel> parentMeasures;
            _cpallsBusiness.BuilderHeader(assessmentId, year, Wave.BOY, out measures, out parentMeasures, true);
            ViewBag.Measures = measures;
            ViewBag.Parents = parentMeasures;

            var groups = MeasureGroup.GetGroupJson(measures, parentMeasures);
            ViewBag.MeasureJson = JsonHelper.SerializeObject(groups);

            var assessment = _adeBusiness.GetAssessment(assessmentId);
            ViewBag.language = assessment.Language;

            ViewBag.TheOtherLanguage = assessment.Language == AssessmentLanguage.English
                ? AssessmentLanguage.Spanish
                : AssessmentLanguage.English;

            var theOtherLanguageAssessment = _adeBusiness.GetTheOtherLanguageAssessment(assessmentId);
            if (theOtherLanguageAssessment != null)
            {
                _cpallsBusiness.BuilderHeader(theOtherLanguageAssessment.ID, year, Wave.BOY, out measures, out parentMeasures, true);
                groups = MeasureGroup.GetGroupJson(measures, parentMeasures);
                ViewBag.MeasureJson2 = JsonHelper.SerializeObject(groups);
            }
            else
            {
                ViewBag.MeasureJson2 = "false";
            }
            return View();
        }

        [CLIUrlAuthorize(Account = Authority.Index, PageId = PagesModel.CPALLS, Anonymity = Anonymous.Verified, Parameter = "assessmentId")]
        public ActionResult SummaryWithAverge(int assessmentId, StudentAssessmentLanguage language, int year, int classId, string waves, DateTime? startDate, DateTime? endDate,
            DateTime? dobStartDate, DateTime? dobEndDate,
            int sortMeasureId = 0, string orderDirection = "ASC", string sortMeasureName = "", bool export = false)
        {
            var assessment = _adeBusiness.GetBaseAssessmentModel(assessmentId);
            if (assessment == null) return View("NoData");
            ViewBag.AssessmentName = assessment.Name;

            if (assessment.TheOtherId > 0 && (byte)assessment.TheOtherLang == (byte)language)
            {
                assessmentId = assessment.TheOtherId;
            }
            var legend = _adeBusiness.GetAssessmentLegend(assessmentId, LegendTypeEnum.ClassSummary);
            ViewBag.AssessmentLegend = legend;
            var class1 = _classBusiness.GetClassForCpalls(classId);

            ViewBag.Title = "Class Scores";
            ViewBag.District = class1.School.CommunitiesText;
            ViewBag.School = class1.School.Name;
            ViewBag.Class = class1.ClassName;
            ViewBag.Teacher = string.Join(", ", _classBusiness.GetTeachers(classId));
            ViewBag.Language = language.ToDescription();
            ViewBag.ScoolYear = year.ToFullSchoolYearString();
            ViewBag.ReportFirstColumn = _reportFirstColumnTitle;
            ViewBag.NoClassSummary = false;

            DateTime dobStart = dobStartDate ?? CommonAgent.MinDate;
            DateTime dobEnd = dobEndDate ?? DateTime.Now;
            var waveMeasures = JsonHelper.DeserializeObject<Dictionary<Wave, IEnumerable<int>>>(waves);
            Dictionary<object, List<ReportRowModel>> reports = _cpallsBusiness.GetStudentSummaryReport(assessmentId, language, UserInfo, year,
                class1.School.ID, classId, waveMeasures, startDate ?? StartDate, (endDate ?? EndDate).AddDays(1).Date, dobStart, dobEnd);

            if (sortMeasureId != 0 && sortMeasureName != "")
            {
                Dictionary<object, List<ReportRowModel>> sortReports = new Dictionary<object, List<ReportRowModel>>();
                List<ReportRowModel> needSortList = new List<ReportRowModel>();
                foreach (var report in reports)
                {
                    object key = report.Key;
                    if (!waveMeasures[(Wave)key].Contains(sortMeasureId))
                    {
                        sortReports.Add(key, report.Value);
                        continue;
                    }
                    int cellIndex = 0;
                    int parentMeasureIndex = report.Value[0].Cells.ToList().FindIndex(r => r.Text.ToString() == sortMeasureName.Trim());
                    if (parentMeasureIndex == -1)
                    {
                        //定义一个Dictionary变量，把所有的MeasureName依次存起来，有子Measure的父Measure存为Total
                        Dictionary<int, object> dicMeasureNameIndex = new Dictionary<int, object>();
                        int colSpan = 0;
                        int rowSpan = 0;
                        int dicIndex = 0;//记录dicMeasureNameIndex的Key的增长
                        int curSubMeasure = 0;//记录读取report.Value第二行的位置
                        for (int i = 0; i < report.Value[0].Cells.Count; i++)
                        {
                            colSpan = report.Value[0].Cells[i].Colspan;
                            rowSpan = report.Value[0].Cells[i].Rowspan;
                            if (colSpan > 1)
                            {
                                for (int j = 0; j < colSpan; j++)
                                {
                                    dicMeasureNameIndex.Add(dicIndex, report.Value[1].Cells[curSubMeasure].Text);
                                    curSubMeasure++;
                                    dicIndex++;
                                }
                            }
                            else
                            {
                                dicMeasureNameIndex.Add(dicIndex, report.Value[0].Cells[i].Text);
                                dicIndex++;
                            }
                        }
                        cellIndex = dicMeasureNameIndex.Where(r => r.Value.ToString() == sortMeasureName.Trim()).FirstOrDefault().Key;
                    }
                    else
                    {
                        for (int i = 1; i <= parentMeasureIndex; i++)
                        {
                            cellIndex = cellIndex + report.Value[0].Cells[i].Colspan;
                        }
                    }
                    sortReports.Add(key, report.Value.Take(3).ToList());
                    if (orderDirection.ToUpper() == "ASC")
                        needSortList = report.Value.Skip(3).OrderBy(r =>
                            r.Cells[cellIndex].Text.ToString().IsInt() ?
                            int.Parse(r.Cells[cellIndex].Text.ToString()) : -1).ThenBy(r => r.Cells[0].Text.ToString())
                            .ToList();
                    else
                        needSortList = report.Value.Skip(3).OrderByDescending(r =>
                            r.Cells[cellIndex].Text.ToString().IsInt() ?
                            int.Parse(r.Cells[cellIndex].Text.ToString()) : -1).ThenBy(r => r.Cells[0].Text.ToString())
                            .ToList();
                    sortReports[key].AddRange(needSortList);
                }
                reports = sortReports;
            }
            else
            {
                Dictionary<object, List<ReportRowModel>> sortByNameReports = new Dictionary<object, List<ReportRowModel>>();
                List<ReportRowModel> needSortByNameList = new List<ReportRowModel>();
                foreach (var report in reports)
                {
                    object key = report.Key;
                    sortByNameReports.Add(key, report.Value.Take(3).ToList());
                    if (orderDirection == "ASC")
                        needSortByNameList = report.Value.Skip(3).OrderBy(r => r.Cells[0].Text.ToString()).ToList();
                    else
                        needSortByNameList = report.Value.Skip(3).OrderByDescending(r => r.Cells[0].Text.ToString()).ToList();
                    sortByNameReports[key].AddRange(needSortByNameList);
                }
                reports = sortByNameReports;
            }

            ViewBag.Waves = reports.ToDictionary(x => (Wave)x.Key, x => x.Value);
            ViewBag.Pdf = export;
            var tmpList = reports.Where(r => r.Value.Count > 2).Select(r => r.Value).ToList();
            ViewBag.StudentCount = tmpList[0].Count - 3;
            if (export)
            {
                GetPdf(GetViewHtml("SummaryWithAverge"), "Summary_Report.pdf");
            }
            return View();
        }

        [CLIUrlAuthorize(Account = Authority.Index, PageId = PagesModel.CPALLS, Anonymity = Anonymous.Verified, Parameter = "assessmentId")]
        public ActionResult PercentageofSatisfactory(int assessmentId, StudentAssessmentLanguage language, int year, int classId,
            string waves, DateTime? startDate, DateTime? endDate, DateTime? dobStartDate, DateTime? dobEndDate, bool export = false)
        {
            var assessment = _adeBusiness.GetBaseAssessmentModel(assessmentId);
            if (assessment == null) return View("NoData");
            ViewBag.AssessmentName = assessment.Name;

            if (assessment.TheOtherId > 0 && (byte)assessment.TheOtherLang == (byte)language)
            {
                assessmentId = assessment.TheOtherId;
            }
            var class1 = _classBusiness.GetClassForCpalls(classId);

            ViewBag.Title = "Class Benchmark Report";
            ViewBag.District = class1.School.CommunitiesText;
            ViewBag.School = class1.School.Name;
            ViewBag.Class = class1.ClassName;
            ViewBag.Teacher = string.Join(", ", _classBusiness.GetTeachers(classId));
            ViewBag.Language = language.ToDescription();
            ViewBag.ScoolYear = year.ToFullSchoolYearString();
            ViewBag.ReportFirstColumn = _reportFirstColumnTitle;

            DateTime dobStart = dobStartDate ?? CommonAgent.MinDate;
            DateTime dobEnd = dobEndDate ?? DateTime.Now;
            var theOther = _adeBusiness.GetTheOtherLanguageAssessment(assessmentId);
            if (theOther != null && (byte)theOther.Language == (byte)language)
            {
                assessmentId = theOther.ID;
            }
            var benchmarks = _adeBusiness.GetIEnumBenchmarks(assessmentId);
            var waveMeasures = JsonHelper.DeserializeObject<Dictionary<Wave, IEnumerable<int>>>(waves);

            Dictionary<object, List<ReportRowModel>> reportsForChart = _cpallsBusiness.GetStudentSatisfactoryReport(
                assessmentId, language, UserInfo, year,
                class1.School.ID, classId, waveMeasures, startDate ?? StartDate, (endDate ?? EndDate).AddDays(1).Date, benchmarks, false, dobStart, dobEnd);

            Dictionary<object, List<ReportRowModel>> reportsForTable = _cpallsBusiness.GetStudentSatisfactoryReport(
                assessmentId, language, UserInfo, year,
                class1.School.ID, classId, waveMeasures, startDate ?? StartDate, (endDate ?? EndDate).AddDays(1).Date, benchmarks, true, dobStart, dobEnd);

            ViewBag.Waves = reportsForTable.ToDictionary(x => (Wave)x.Key, x => x.Value);
            var jdata = reportsForChart.ToDictionary(x => (byte)x.Key, x => x.Value);
            ViewBag.JData = JsonHelper.SerializeObject(jdata);
            return View();
        }

        [CLIUrlAuthorize(Account = Authority.Index, PageId = PagesModel.CPALLS, Anonymity = Anonymous.Verified, Parameter = "assessmentId")]
        public ActionResult PercentageofSatisfactory_Pdf(int assessmentId, StudentAssessmentLanguage language, int year, int classId, string waves,
              List<string> imgSources, DateTime? startDate, DateTime? endDate, DateTime? dobStartDate, DateTime? dobEndDate, bool export = false)
        {
            var assessment = _adeBusiness.GetBaseAssessmentModel(assessmentId);
            if (assessment == null) return View("NoData");
            ViewBag.AssessmentName = assessment.Name;

            var class1 = _classBusiness.GetClassForCpalls(classId);

            ViewBag.Title = "Class Benchmark Report";
            ViewBag.District = class1.School.CommunitiesText;
            ViewBag.School = class1.School.Name;
            ViewBag.Class = class1.ClassName;
            ViewBag.Teacher = string.Join(", ", _classBusiness.GetTeachers(classId));
            ViewBag.Language = language.ToDescription();
            ViewBag.ScoolYear = year.ToFullSchoolYearString();
            ViewBag.ReportFirstColumn = _reportFirstColumnTitle;


            DateTime dobStart = dobStartDate ?? CommonAgent.MinDate;
            DateTime dobEnd = dobEndDate ?? DateTime.Now;
            var theOther = _adeBusiness.GetTheOtherLanguageAssessment(assessmentId);
            if (theOther != null && (byte)theOther.Language == (byte)language)
            {
                assessmentId = theOther.ID;
            }
            var benchmarks = _adeBusiness.GetIEnumBenchmarks(assessmentId);
            var waveMeasures = JsonHelper.DeserializeObject<Dictionary<Wave, IEnumerable<int>>>(waves);

            Dictionary<object, List<ReportRowModel>> reportsForChart = _cpallsBusiness.GetStudentSatisfactoryReport(assessmentId, language, UserInfo, year,
                class1.School.ID, classId, waveMeasures, startDate ?? StartDate, (endDate ?? EndDate).AddDays(1).Date, benchmarks, false, dobStart, dobEnd);

            Dictionary<object, List<ReportRowModel>> reportsForTable = _cpallsBusiness.GetStudentSatisfactoryReport(assessmentId, language, UserInfo, year,
                class1.School.ID, classId, waveMeasures, startDate ?? StartDate, (endDate ?? EndDate).AddDays(1).Date, benchmarks, true, dobStart, dobEnd);

            var index = 0;
            foreach (var chart in reportsForChart)
            {
                ViewData["Image" + index] = imgSources[index++];
            }
            ViewBag.Waves = reportsForTable.ToDictionary(x => (Wave)x.Key, x => x.Value);
            ViewBag.Pdf = export;
            if (export)
            {
                GetPdf(GetViewHtml("PercentageofSatisfactory_Pdf"), "Benchmark_Report.pdf");
            }

            return View();
        }
        #endregion

        #region Custom Score Report

        [CLIUrlAuthorize(Account = Authority.Index, PageId = PagesModel.CPALLS, Anonymity = Anonymous.Verified,
            Parameter = "assessmentId")]
        public ActionResult CustomScoreReportView(int assessmentId, int year, int classId, string className = "")
        {
            var assessment = _adeBusiness.GetAssessment(assessmentId);
            ViewBag.language = assessment.Language;

            var theOtherLanguageAssessment = _adeBusiness.GetTheOtherLanguageAssessment(assessmentId);
            int theOtherAssessmentId = 0;
            ViewBag.AssessmentId = assessmentId;
            ViewBag.ClassId = classId;
            ViewBag.Year = year;
            if (theOtherLanguageAssessment != null)
            {
                theOtherAssessmentId = theOtherLanguageAssessment.ID;
                ViewBag.TheOtherLanguage = theOtherLanguageAssessment.Language;
            }
            if (assessment.Language == AssessmentLanguage.English)
            {
                ViewBag.CustomScoresEnglish = _adeBusiness.GetScoresByAssessmentId(assessmentId);
                ViewBag.CustomScoresSpanish = _adeBusiness.GetScoresByAssessmentId(theOtherAssessmentId);
            }
            else if (assessment.Language == AssessmentLanguage.Spanish)
            {
                ViewBag.CustomScoresSpanish = _adeBusiness.GetScoresByAssessmentId(assessmentId);
                ViewBag.CustomScoresEnglish = _adeBusiness.GetScoresByAssessmentId(theOtherAssessmentId);
            }
            return View();
        }

        [CLIUrlAuthorize(Account = Authority.Index, PageId = PagesModel.CPALLS, Anonymity = Anonymous.Verified, Parameter = "assessmentId")]
        public ActionResult CustomScorePdf(int assessmentId, StudentAssessmentLanguage language, int classId, bool allClasses,
            Wave wave, int scoreYear, List<int> scoreIds, DateTime? startDate, DateTime? endDate, DateTime? dobStartDate, DateTime? dobEndDate)
        {
            var assessment = _adeBusiness.GetBaseAssessmentModel(assessmentId);
            if (assessment == null) return View("NoData");
            ViewBag.AssessmentName = assessment.Name;

            if (assessment.TheOtherId > 0 && (byte)assessment.TheOtherLang == (byte)language)
            {
                assessmentId = assessment.TheOtherId;
            }
            var legend = _adeBusiness.GetAssessmentLegend(assessmentId, LegendTypeEnum.ClassSummary);
            ViewBag.AssessmentLegend = legend;
            var class1 = _classBusiness.GetClassForCpalls(classId);
            List<CustomScoreReportModel> customScoreReports = _cpallsBusiness.GetScoreReportPdf(assessmentId, language,
                UserInfo, wave, scoreYear, class1.School.ID, classId, allClasses, scoreIds, startDate ?? StartDate,
                (endDate ?? EndDate).AddDays(1).Date, dobStartDate ?? CommonAgent.MinDate, dobEndDate ?? DateTime.Now);
            ViewBag.CustomScoreReports = customScoreReports;
            List<ScoreInitModel> customScoreInits = customScoreReports.FirstOrDefault().ScoreInits;
            ViewBag.CustomScoreInits = customScoreInits;
            ViewBag.Language = language;
            GetPdf(GetViewHtml("CustomScorePdf"), "CustomScorePdf.pdf", PdfType.Assessment_Portrait);
            return View();
        }

        [CLIUrlAuthorize(Account = Authority.Index, PageId = PagesModel.CPALLS, Anonymity = Anonymous.Verified, Parameter = "assessmentId")]
        public ActionResult CustomScoreBenchmark(int assessmentId, StudentAssessmentLanguage language, int classId, bool allClasses,
            Wave wave, int scoreYear, List<int> scoreIds, DateTime? startDate, DateTime? endDate, DateTime? dobStartDate, DateTime? dobEndDate)
        {
            var assessment = _adeBusiness.GetBaseAssessmentModel(assessmentId);
            if (assessment == null) return View("NoData");
            ViewBag.AssessmentName = assessment.Name;

            if (assessment.TheOtherId > 0 && (byte)assessment.TheOtherLang == (byte)language)
            {
                assessmentId = assessment.TheOtherId;
            }
            var legend = _adeBusiness.GetAssessmentLegend(assessmentId, LegendTypeEnum.ClassSummary);
            ViewBag.AssessmentLegend = legend;
            var class1 = _classBusiness.GetClassForCpalls(classId);
            List<CustomScoreReportModel> customScoreReports = _cpallsBusiness.GetScoreReportPdf(assessmentId, language,
                UserInfo, wave, scoreYear, class1.School.ID, classId, allClasses, scoreIds, startDate ?? StartDate,
                (endDate ?? EndDate).AddDays(1).Date, dobStartDate ?? CommonAgent.MinDate, dobEndDate ?? DateTime.Now);
            ViewBag.Language = language;
            List<ScoreInitModel> customScoreInits = customScoreReports.FirstOrDefault().ScoreInits;
            ViewBag.CustomScoreReports = customScoreReports;
            ViewBag.CustomScoreInits = customScoreInits;
            var benchmarks = _adeBusiness.GetIEnumBenchmarks(assessmentId);
            ViewBag.BenchmarkInits = benchmarks.ToList();
            Dictionary<object, List<ReportRowModel>> reportsForChart = new Dictionary<object, List<ReportRowModel>>();
            var count = 0;
            foreach (var customScoreReport in customScoreReports)
            {
                List<ReportRowModel> rrms = new List<ReportRowModel>();
                ReportRowModel rrm = new ReportRowModel();
                count++;
                List<ReportCellModel> rcms = new List<ReportCellModel>();
                ReportCellModel cellOne = new ReportCellModel();
                cellOne.Text = "Custom Score";
                cellOne.Rowspan = 1;
                cellOne.Colspan = 2;
                cellOne.Description = customScoreReport.Class;
                rcms.Add(cellOne);
                foreach (var scoreBenchmark in benchmarks)
                {
                    ReportCellModel cellOne1 = new ReportCellModel();
                    cellOne1.Text = scoreBenchmark.LabelText;
                    cellOne1.Rowspan = 1;
                    cellOne1.Colspan = 1;
                    cellOne1.Color = scoreBenchmark.Color;
                    rcms.Add(cellOne1);
                }
                rrm.Cells = rcms;
                rrms.Add(rrm);
                foreach (var customScoreInit in customScoreInits)
                {
                    List<ReportCellModel> rcms1 = new List<ReportCellModel>();
                    ReportRowModel rrm1 = new ReportRowModel();
                    ReportCellModel cellOther = new ReportCellModel();
                    cellOther.Text = customScoreInit.ScoreDomain;
                    cellOther.Rowspan = 1;
                    cellOther.Colspan = 2;
                    rcms1.Add(cellOther);
                    var currentScores = customScoreReport.ScoreReports.Where(e => e.ScoreId == customScoreInit.ScoreId && e.FinalScore != null && e.BenchmarkId > 0);
                    var allScoresStudentCount = currentScores.GroupBy(e => e.StudentId).Count();

                    foreach (var benchmarkInit in benchmarks)
                    {
                        var allScores = currentScores.Where(e => e.BenchmarkId == benchmarkInit.ID);
                        var studentCount = allScores.GroupBy(e => e.StudentId).Count();
                        decimal percentage = 0M;
                        if (allScoresStudentCount > 0)
                        {
                            percentage = (studentCount * 100.0M / allScoresStudentCount);
                        }
                        ReportCellModel cellOther1 = new ReportCellModel();
                        cellOther1.Text = percentage + "%";
                        cellOther1.Rowspan = 1;
                        cellOther1.Colspan = 1;
                        rcms1.Add(cellOther1);
                    }
                    rrm1.Cells = rcms1;
                    rrms.Add(rrm1);
                }
                reportsForChart.Add(count.ToString(), rrms);
            }

            var jdata = reportsForChart.ToDictionary(x => (string)x.Key, x => x.Value);
            ViewBag.JData = JsonHelper.SerializeObject(jdata);
            return View();
        }

        [CLIUrlAuthorize(Account = Authority.Index, PageId = PagesModel.CPALLS, Anonymity = Anonymous.Verified, Parameter = "assessmentId")]
        public ActionResult CustomScoreBenchmarkPdf(int assessmentId, StudentAssessmentLanguage language, int classId, bool allClasses,
            Wave wave, int scoreYear, string scoreIds, List<string> imgSources, DateTime? startDate, DateTime? endDate, DateTime? dobStartDate, DateTime? dobEndDate, bool export = false)
        {
            var assessment = _adeBusiness.GetBaseAssessmentModel(assessmentId);
            if (assessment == null) return View("NoData");
            ViewBag.AssessmentName = assessment.Name;

            var class1 = _classBusiness.GetClassForCpalls(classId);

            ViewBag.Title = "Class Benchmark Report";
            ViewBag.District = class1.School.CommunitiesText;
            ViewBag.School = class1.School.Name;
            ViewBag.Class = class1.ClassName;
            ViewBag.Teacher = string.Join(", ", _classBusiness.GetTeachers(classId));
            ViewBag.Language = language.ToDescription();
            ViewBag.ScoolYear = scoreYear.ToFullSchoolYearString();
            ViewBag.ReportFirstColumn = _reportFirstColumnTitle;


            DateTime dobStart = dobStartDate ?? CommonAgent.MinDate;
            DateTime dobEnd = dobEndDate ?? DateTime.Now;
            var theOther = _adeBusiness.GetTheOtherLanguageAssessment(assessmentId);
            if (theOther != null && (byte)theOther.Language == (byte)language)
            {
                assessmentId = theOther.ID;
            }
            List<string> scoreIdString = scoreIds.Split(',').ToList();
            List<int> scoreIdList = scoreIdString.ConvertAll<int>(e => e.ToInt32());
            List<CustomScoreReportModel> customScoreReports = _cpallsBusiness.GetScoreReportPdf(assessmentId, language,
                UserInfo, wave, scoreYear, class1.School.ID, classId, allClasses, scoreIdList, startDate ?? StartDate,
                (endDate ?? EndDate).AddDays(1).Date, dobStartDate ?? CommonAgent.MinDate, dobEndDate ?? DateTime.Now);
            ViewBag.Language = language;
            List<ScoreInitModel> customScoreInits = customScoreReports.FirstOrDefault().ScoreInits;
            ViewBag.CustomScoreReports = customScoreReports;
            ViewBag.CustomScoreInits = customScoreInits;
            var benchmarks = _adeBusiness.GetIEnumBenchmarks(assessmentId);
            ViewBag.BenchmarkInits = benchmarks.ToList();
            Dictionary<object, List<ReportRowModel>> reportsForChart = new Dictionary<object, List<ReportRowModel>>();
            var count = 0;
            foreach (var customScoreReport in customScoreReports)
            {
                List<ReportRowModel> rrms = new List<ReportRowModel>();
                ReportRowModel rrm = new ReportRowModel();
                count++;
                List<ReportCellModel> rcms = new List<ReportCellModel>();
                ReportCellModel cellOne = new ReportCellModel();
                cellOne.Text = "Custom Score";
                cellOne.Rowspan = 1;
                cellOne.Colspan = 2;
                cellOne.Description = customScoreReport.Class;
                rcms.Add(cellOne);
                foreach (var scoreBenchmark in benchmarks)
                {
                    ReportCellModel cellOne1 = new ReportCellModel();
                    cellOne1.Text = scoreBenchmark.LabelText;
                    cellOne1.Rowspan = 1;
                    cellOne1.Colspan = 1;
                    cellOne1.Color = scoreBenchmark.Color;
                    rcms.Add(cellOne1);
                }
                rrm.Cells = rcms;
                rrms.Add(rrm);
                foreach (var customScoreInit in customScoreInits)
                {
                    List<ReportCellModel> rcms1 = new List<ReportCellModel>();
                    ReportRowModel rrm1 = new ReportRowModel();
                    ReportCellModel cellOther = new ReportCellModel();
                    cellOther.Text = customScoreInit.ScoreDomain;
                    cellOther.Rowspan = 1;
                    cellOther.Colspan = 2;
                    rcms1.Add(cellOther);
                    var currentScores = customScoreReport.ScoreReports.Where(e => e.ScoreId == customScoreInit.ScoreId && e.FinalScore != null && e.BenchmarkId > 0);
                    var allScoresStudentCount = currentScores.GroupBy(e => e.StudentId).Count();

                    foreach (var benchmarkInit in benchmarks)
                    {
                        var allScores = currentScores.Where(e => e.BenchmarkId == benchmarkInit.ID);
                        var studentCount = allScores.GroupBy(e => e.StudentId).Count();
                        decimal percentage = 0M;
                        if (allScoresStudentCount > 0)
                        {
                            percentage = (studentCount * 100.0M / allScoresStudentCount);
                        }
                        ReportCellModel cellOther1 = new ReportCellModel();
                        cellOther1.Text = percentage + "%";
                        cellOther1.Rowspan = 1;
                        cellOther1.Colspan = 1;
                        rcms1.Add(cellOther1);
                    }
                    rrm1.Cells = rcms1;
                    rrms.Add(rrm1);
                }
                reportsForChart.Add(count.ToString(), rrms);
            }
            var index = 0;
            foreach (var chart in reportsForChart)
            {
                ViewData["Image" + index] = imgSources[index];
                index++;
            }
            ViewBag.Pdf = export;
            if (export)
            {
                GetPdf(GetViewHtml("CustomScoreBenchmarkPdf"), "Benchmark_Report.pdf", PdfType.Assessment_Portrait);
            }

            return View();
        }
        #endregion

        #region  Class Percentile Rank Report
        [CLIUrlAuthorize(Account = Authority.Index, PageId = PagesModel.CPALLS, Anonymity = Anonymous.Verified, Parameter = "assessmentId")]
        public ActionResult ClassPercentileRank(int assessmentId, int year, int classId,
            string className = "", bool export = true)
        {
            List<MeasureHeaderModel> measures;
            List<MeasureHeaderModel> parentMeasures;
            _cpallsBusiness.BuilderHeader(assessmentId, year, Wave.BOY, out measures, out parentMeasures, true);
            measures = measures.Where(e => e.PercentileRank).ToList();
            ViewBag.Measures = measures;
            ViewBag.Parents = parentMeasures;

            var groups = MeasureGroup.GetGroupJson(measures, parentMeasures);
            ViewBag.MeasureJson = JsonHelper.SerializeObject(groups);

            var assessment = _adeBusiness.GetAssessment(assessmentId);
            ViewBag.language = assessment.Language;

            ViewBag.TheOtherLanguage = assessment.Language == AssessmentLanguage.English
                ? AssessmentLanguage.Spanish
                : AssessmentLanguage.English;

            var theOtherLanguageAssessment = _adeBusiness.GetTheOtherLanguageAssessment(assessmentId);
            if (theOtherLanguageAssessment != null)
            {
                _cpallsBusiness.BuilderHeader(theOtherLanguageAssessment.ID, year, Wave.BOY, out measures, out parentMeasures, true);
                measures = measures.Where(e => e.PercentileRank).ToList();
                groups = MeasureGroup.GetGroupJson(measures, parentMeasures);
                ViewBag.MeasureJson2 = JsonHelper.SerializeObject(groups);
            }
            else
            {
                ViewBag.MeasureJson2 = "false";
            }
            return View();
        }

        [CLIUrlAuthorize(Account = Authority.Index, PageId = PagesModel.CPALLS, Anonymity = Anonymous.Verified, Parameter = "assessmentId")]
        public ActionResult SummaryPercentileRankAveragePdf(int assessmentId, StudentAssessmentLanguage language, int year, int classId,
            string waves, DateTime? startDate, DateTime? endDate, DateTime? dobStartDate, DateTime? dobEndDate,
            int sortMeasureId = 0, string orderDirection = "ASC", string sortMeasureName = "", bool export = false)
        {
            var assessment = _adeBusiness.GetBaseAssessmentModel(assessmentId);
            if (assessment == null) return View("NoData");
            ViewBag.AssessmentName = assessment.Name;

            if (assessment.TheOtherId > 0 && (byte)assessment.TheOtherLang == (byte)language)
            {
                assessmentId = assessment.TheOtherId;
            }
            var class1 = _classBusiness.GetClassForCpalls(classId);

            ViewBag.Title = "Percentile Rank - Class";
            ViewBag.District = class1.School.CommunitiesText;
            ViewBag.School = class1.School.Name;
            ViewBag.Class = class1.ClassName;
            ViewBag.Teacher = string.Join(", ", _classBusiness.GetTeachers(classId));
            ViewBag.Language = language.ToDescription();
            ViewBag.ScoolYear = year.ToFullSchoolYearString();
            ViewBag.ReportFirstColumn = _reportFirstColumnTitle;
            ViewBag.NoClassSummary = false;

            DateTime dobStart = dobStartDate ?? CommonAgent.MinDate;
            DateTime dobEnd = dobEndDate ?? DateTime.Now;
            var waveMeasures = JsonHelper.DeserializeObject<Dictionary<Wave, IEnumerable<int>>>(waves);
            Dictionary<object, List<ReportRowModel>> reports = _cpallsBusiness.GetClassPercentileRankReport(assessmentId, language, UserInfo, year, class1.School.ID, classId,
                waveMeasures, startDate ?? StartDate, (endDate ?? EndDate).AddDays(1).Date, dobStart, dobEnd);

            if (sortMeasureId != 0 && sortMeasureName != "")
            {
                Dictionary<object, List<ReportRowModel>> sortReports = new Dictionary<object, List<ReportRowModel>>();
                List<ReportRowModel> needSortList = new List<ReportRowModel>();
                foreach (var report in reports)
                {
                    object key = report.Key;
                    if (!waveMeasures[(Wave)key].Contains(sortMeasureId))
                    {
                        sortReports.Add(key, report.Value);
                        continue;
                    }
                    int cellIndex = 0;
                    int parentMeasureIndex = report.Value[0].Cells.ToList().FindIndex(r => r.Text.ToString() == sortMeasureName.Trim());
                    if (parentMeasureIndex == -1)
                    {
                        //定义一个Dictionary变量，把所有的MeasureName依次存起来，有子Measure的父Measure存为Total
                        Dictionary<int, object> dicMeasureNameIndex = new Dictionary<int, object>();
                        int colSpan = 0;
                        int rowSpan = 0;
                        int dicIndex = 0;//记录dicMeasureNameIndex的Key的增长
                        int curSubMeasure = 0;//记录读取report.Value第二行的位置
                        for (int i = 0; i < report.Value[0].Cells.Count; i++)
                        {
                            colSpan = report.Value[0].Cells[i].Colspan;
                            rowSpan = report.Value[0].Cells[i].Rowspan;
                            if (colSpan > 1)
                            {
                                for (int j = 0; j < colSpan; j++)
                                {
                                    dicMeasureNameIndex.Add(dicIndex, report.Value[1].Cells[curSubMeasure].Text);
                                    curSubMeasure++;
                                    dicIndex++;
                                }
                            }
                            else
                            {
                                dicMeasureNameIndex.Add(dicIndex, report.Value[0].Cells[i].Text);
                                dicIndex++;
                            }
                        }
                        cellIndex = dicMeasureNameIndex.Where(r => r.Value.ToString() == sortMeasureName.Trim()).FirstOrDefault().Key;
                    }
                    else
                    {
                        for (int i = 1; i <= parentMeasureIndex; i++)
                        {
                            cellIndex = cellIndex + report.Value[0].Cells[i].Colspan;
                        }
                    }
                    sortReports.Add(key, report.Value.Take(3).ToList());
                    if (orderDirection.ToUpper() == "ASC")
                        needSortList = report.Value.Skip(3).OrderBy(r =>
                            r.Cells[cellIndex].Text.ToString().IsInt() ?
                            int.Parse(r.Cells[cellIndex].Text.ToString()) : -1).ThenBy(r => r.Cells[0].Text.ToString())
                            .ToList();
                    else
                        needSortList = report.Value.Skip(3).OrderByDescending(r =>
                            r.Cells[cellIndex].Text.ToString().IsInt() ?
                            int.Parse(r.Cells[cellIndex].Text.ToString()) : -1).ThenBy(r => r.Cells[0].Text.ToString())
                            .ToList();
                    sortReports[key].AddRange(needSortList);
                }
                reports = sortReports;
            }
            else
            {
                Dictionary<object, List<ReportRowModel>> sortByNameReports = new Dictionary<object, List<ReportRowModel>>();
                List<ReportRowModel> needSortByNameList = new List<ReportRowModel>();
                foreach (var report in reports)
                {
                    object key = report.Key;
                    sortByNameReports.Add(key, report.Value.Take(3).ToList());
                    if (orderDirection == "ASC")
                        needSortByNameList = report.Value.Skip(3).OrderBy(r => r.Cells[0].Text.ToString()).ToList();
                    else
                        needSortByNameList = report.Value.Skip(3).OrderByDescending(r => r.Cells[0].Text.ToString()).ToList();
                    sortByNameReports[key].AddRange(needSortByNameList);
                }
                reports = sortByNameReports;
            }

            ViewBag.Waves = reports.ToDictionary(x => (Wave)x.Key, x => x.Value);
            ViewBag.Pdf = export;
            var tmpList = reports.Where(r => r.Value.Count > 2).Select(r => r.Value).ToList();
            ViewBag.StudentCount = tmpList[0].Count - 3;
            if (export)
            {
                GetPdf(GetViewHtml("SummaryPercentileRankAveragePdf"), "Class_PercentileRankReport.pdf");
            }
            return View();
        }
        #endregion

        #region Growth
        [CLIUrlAuthorize(Account = Authority.Index, PageId = PagesModel.CPALLS, Anonymity = Anonymous.Verified, Parameter = "assessmentId")]
        public ActionResult Growth(int assessmentId, int year, int classId, int schoolId, string class1)
        {
            ViewBag.Title = "Class Growth Report";
            ViewBag.Source = class1;
            ViewBag.AllSourceType = "Classes";

            List<MeasureHeaderModel> measures;
            List<MeasureHeaderModel> parentMeasures;
            _cpallsBusiness.BuilderHeader(assessmentId, year, Wave.BOY, out measures, out parentMeasures, true);

            var assessment = _adeBusiness.GetAssessment(assessmentId);
            ViewBag.language = assessment.Language;

            var groups = MeasureGroup.GetGroupJson(measures, parentMeasures);
            // growth report, 需要比较三个Wave的Measure 成绩
            var meaGroup = groups["groups"] as List<MeasureGroup>;
            if (meaGroup != null)
            {
                meaGroup.ForEach(mg => mg.Measures.ForEach(mea => mea.Waves = "1,2,3"));
            }
            ViewBag.MeasureJson = JsonHelper.SerializeObject(groups);

            ViewBag.TheOtherLanguage = assessment.Language == AssessmentLanguage.English
                ? AssessmentLanguage.Spanish
                : AssessmentLanguage.English;

            var theOtherLanguageAssessment = _adeBusiness.GetTheOtherLanguageAssessment(assessmentId);
            if (theOtherLanguageAssessment != null)
            {
                _cpallsBusiness.BuilderHeader(theOtherLanguageAssessment.ID, year, Wave.BOY, out measures, out parentMeasures, true);
                groups = MeasureGroup.GetGroupJson(measures, parentMeasures);
                // growth report, 需要比较三个Wave的Measure 成绩
                meaGroup = groups["groups"] as List<MeasureGroup>;
                if (meaGroup != null)
                {
                    meaGroup.ForEach(mg => mg.Measures.ForEach(mea => mea.Waves = "1,2,3"));
                }
                ViewBag.MeasureJson2 = JsonHelper.SerializeObject(groups);
            }
            else
            {
                ViewBag.MeasureJson2 = "false";
            }
            return View();
        }

        private string Key_Growth
        {
            get { return "CPALLS_Growth"; }
        }
        private string Key_GrowthBenchmark
        {
            get { return "CPALLS_Growth_BENCHMARK"; }
        }

        [CLIUrlAuthorize(Account = Authority.Index, PageId = PagesModel.CPALLS, Anonymity = Anonymous.Verified, Parameter = "assessmentId")]
        public ActionResult Growth_Pdf(int assessmentId, StudentAssessmentLanguage language, int year, int schoolId, int classId,
            GrowthReportType type, bool all, string waves, string measures, DateTime? startDate, DateTime? endDate, DateTime? dobStartDate, DateTime? dobEndDate)
        {
            var assessment = _adeBusiness.GetBaseAssessmentModel(assessmentId);
            if (assessment == null) return View("NoData");
            if (assessment.TheOtherId > 0 && (byte)assessment.TheOtherLang == (byte)language)
                assessmentId = assessment.TheOtherId;
            ViewBag.AssessmentName = assessment.Name;
            ViewBag.assessmentId = assessmentId;

            DateTime dobStart = dobStartDate ?? CommonAgent.MinDate;
            DateTime dobEnd = dobEndDate ?? DateTime.Now;
            var ws = JsonHelper.DeserializeObject<List<int>>(waves).Select(x => (Wave)x).ToList();
            var measureIds = JsonHelper.DeserializeObject<List<int>>(measures);
            var benchmarks = _adeBusiness.GetIEnumBenchmarks(assessmentId);
            var waveMeasures = ws.ToDictionary(x => x, x => measureIds);
            var reports = _cpallsBusiness.GetClassGrowthPdf(assessmentId, language, UserInfo, year, type, waveMeasures,
                classId, schoolId, all, startDate ?? StartDate, (endDate ?? EndDate).AddDays(1).Date, benchmarks, dobStart, dobEnd);
            if (reports == null || !reports.Any())
            {
                return View("NoData");
            }
            SetCache(type == GrowthReportType.Average ? Key_Growth : Key_GrowthBenchmark, reports);

            ViewBag.Models = reports;
            ViewBag.JData = JsonHelper.SerializeObject(reports);
            return View("Growth_Pdf_" + type.ToString());
        }

        [CLIUrlAuthorize(Account = Authority.Index, PageId = PagesModel.CPALLS, Anonymity = Anonymous.Verified, Parameter = "assessmentId")]
        public ActionResult Growth_Pdf_Download(int assessmentId, StudentAssessmentLanguage language, GrowthReportType type,
            int year, int schoolId, int classId, bool all, string waves, string measures, List<string> imgSources,
            DateTime? startDate, DateTime? endDate, DateTime? dobStartDate, DateTime? dobEndDate, bool export)
        {
            var assessment = _adeBusiness.GetBaseAssessmentModel(assessmentId);
            if (assessment == null) return View("NoData");
            ViewBag.AssessmentName = assessment.Name;

            DateTime dobStart = dobStartDate ?? CommonAgent.MinDate;
            DateTime dobEnd = dobEndDate ?? DateTime.Now;
            var ws = JsonHelper.DeserializeObject<List<int>>(waves).Select(x => (Wave)x).ToList();
            var measureIds = JsonHelper.DeserializeObject<List<int>>(measures);
            var benchmarks = _adeBusiness.GetIEnumBenchmarks(assessmentId);
            var waveMeasures = ws.ToDictionary(x => x, x => measureIds);

            var reports = GetFromCache<List<GrowthReportModel>>(type == GrowthReportType.Average ? Key_Growth : Key_GrowthBenchmark);
            reports = reports ??
                      _cpallsBusiness.GetClassGrowthPdf(assessmentId, language, UserInfo, year, type, waveMeasures,
                          classId, schoolId, all, startDate ?? StartDate, (endDate ?? EndDate).AddDays(1).Date, benchmarks, dobStart, dobEnd);
            if (reports == null || !reports.Any())
            {
                return View("NoData");
            }
            var index = 0;
            foreach (var img in imgSources)
            {
                ViewData["Image" + (index++).ToString()] = img;
            }
            ViewBag.Models = reports;
            if (export)
            {
                GetPdf(GetViewHtml("Growth_Pdf_Download"), reports.First().Title.Replace(" ", "_"));
            }
            return View();
        }


        #endregion

        #region Completion Report Pdf
        [CLIUrlAuthorize(Account = Authority.Index, PageId = PagesModel.CPALLS, Anonymity = Anonymous.Verified, Parameter = "assessmentId")]
        public ActionResult Completion(int assessmentId, int year, int schoolId, int classId)
        {
            List<MeasureHeaderModel> measures = new List<MeasureHeaderModel>();
            List<MeasureHeaderModel> parentMeasures = new List<MeasureHeaderModel>();
            List<MeasureHeaderModel> measuresOther = new List<MeasureHeaderModel>();
            List<MeasureHeaderModel> parentMeasuresOther = new List<MeasureHeaderModel>();
            List<MeasureHeaderModel> measuresSum = new List<MeasureHeaderModel>();
            List<MeasureHeaderModel> parentMeasuresSum = new List<MeasureHeaderModel>();
            _cpallsBusiness.BuilderHeader(assessmentId, year, Wave.BOY, out measures, out parentMeasures, true);


            var assessment = _adeBusiness.GetAssessment(assessmentId);
            ViewBag.language = assessment.Language;
            var groups = MeasureGroup.GetGroupJson(measures, parentMeasures);
            ViewBag.MeasureJson = JsonHelper.SerializeObject(groups);

            ViewBag.TheOtherLanguage = assessment.Language == AssessmentLanguage.English
                ? StudentAssessmentLanguage.Spanish
                : StudentAssessmentLanguage.English;

            bool otherAssessmentStatusIsActive = false;
            var theOtherLanguageAssessment = _adeBusiness.GetTheOtherLanguageAssessment(assessmentId);
            if (theOtherLanguageAssessment != null)
            {
                _cpallsBusiness.BuilderHeader(theOtherLanguageAssessment.ID, year, Wave.BOY, out measuresOther, out parentMeasuresOther, true);
                groups = MeasureGroup.GetGroupJson(measuresOther, parentMeasuresOther);
                ViewBag.MeasureJson2 = JsonHelper.SerializeObject(groups);
                otherAssessmentStatusIsActive = theOtherLanguageAssessment.Status == EntityStatus.Active;
            }
            else
            {
                ViewBag.MeasureJson2 = "false";
            }

            ViewBag.BilingualLanguage = StudentAssessmentLanguage.Bilingual;
            if (assessment.Language == AssessmentLanguage.English)
            {
                measuresSum.AddRange(measures);
                parentMeasuresSum.AddRange(parentMeasures);
                if (otherAssessmentStatusIsActive)
                {
                    foreach (var item in measuresOther)
                    {
                        if (measures.All(o => o.MeasureId != item.RelatedMeasureId))
                        {
                            //Assessment为西班牙，改变无关联项的Measure的Name
                            measuresSum.Add(GetCloneModel(item, false));
                        }
                        //将ParentId!=1同时又没有关联项的Measure的父Measure，添加到parentMeasuresSum里
                        if (item.ParentId != 1 && parentMeasuresSum.All(o => o.MeasureId != item.ParentId))
                        {
                            parentMeasuresSum.Add(parentMeasuresOther.FirstOrDefault(o => o.MeasureId == item.ParentId));
                        }
                    }

                    //Assessment为英语，改变Measure的Name
                    foreach (var item in measures)
                    {
                        if (measuresOther.All(o => o.RelatedMeasureId != item.MeasureId))
                        {
                            int index = measures.IndexOf(item);
                            measuresSum.Remove(item);
                            measuresSum.Insert(index, GetCloneModel(item, true));
                        }
                        else
                        {
                            var spanishFirst = measuresOther.FirstOrDefault(o => o.RelatedMeasureId == item.MeasureId);
                            int index = measures.IndexOf(item);
                            measuresSum.Remove(item);
                            measuresSum.Insert(index, GetCloneModel(item, true, true, spanishFirst.Name));
                        }
                    }
                }
            }
            else
            {
                if (otherAssessmentStatusIsActive)
                {
                    measuresSum.AddRange(measuresOther);
                    parentMeasuresSum.AddRange(parentMeasuresOther);
                    foreach (var item in measures)
                    {
                        if (measuresOther.All(o => o.MeasureId != item.RelatedMeasureId))
                        {
                            //改Assessment为西班牙，改变无关联项的Measure的Name
                            measuresSum.Add(GetCloneModel(item, false));
                        }
                        if (item.ParentId != 1 && parentMeasuresSum.All(o => o.MeasureId != item.ParentId))
                        {
                            parentMeasuresSum.Add(parentMeasures.FirstOrDefault(o => o.MeasureId == item.ParentId));
                        }
                    }
                    //Assessment为英语，改变Measure的Name
                    foreach (var item in measuresOther)
                    {
                        if (measures.All(o => o.RelatedMeasureId != item.MeasureId))
                        {
                            int index = measuresOther.IndexOf(item);
                            measuresSum.Remove(item);
                            measuresSum.Insert(index, GetCloneModel(item, true));
                        }
                        else
                        {
                            var spanishFirst = measures.FirstOrDefault(o => o.RelatedMeasureId == item.MeasureId);
                            int index = measuresOther.IndexOf(item);
                            measuresSum.Remove(item);
                            measuresSum.Insert(index, GetCloneModel(item, true, true, spanishFirst.Name));
                        }
                    }
                }
                else
                {
                    measuresSum.AddRange(measures);
                    parentMeasuresSum.AddRange(parentMeasures);
                }
            }


            List<int> parentIds = measuresSum.Where(r => r.MeasureId != r.ParentId).GroupBy(r => r.ParentId).Select(r => r.Key).ToList();
            List<int> removeIds = new List<int>();
            foreach (MeasureHeaderModel item in parentMeasuresSum)
            {
                if (!(parentIds.Contains(item.MeasureId) || (item.ParentId == 1 && item.Subs == 0)))
                    removeIds.Add(item.MeasureId);
            }

            parentMeasuresSum.RemoveAll(r => removeIds.Contains(r.MeasureId));
            measuresSum.RemoveAll(r => r.MeasureId == r.ParentId && removeIds.Contains(r.MeasureId));

            groups = MeasureGroup.GetGroupJson(measuresSum, parentMeasuresSum);
            ViewBag.MeasureJson3 = JsonHelper.SerializeObject(groups);
            ViewBag.className = _classBusiness.GetClass(classId).Name;
            return View();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="measure"></param>
        /// <param name="isEnglish">measure所属的Assessment是否是英语</param>
        /// <param name="isCombine">是否需要将2个Name合并</param>
        /// <param name="name"></param>
        /// <returns></returns>
        private MeasureHeaderModel GetCloneModel(MeasureHeaderModel measure, bool isEnglish, bool isCombine = false, string name = "")
        {
            MeasureHeaderModel otherMeasure = new MeasureHeaderModel();
            otherMeasure.ID = measure.ID;
            otherMeasure.MeasureId = measure.MeasureId;
            otherMeasure.Name = measure.Name;
            if (!isCombine)
            {
                if (isEnglish)
                {
                    otherMeasure.TheOtherLanguageName = "Only in English";
                }
                else
                {
                    otherMeasure.TheOtherLanguageName = "Only in Spanish";
                }
            }
            else
            {
                otherMeasure.TheOtherLanguageName = name;
            }
            otherMeasure.ParentId = measure.ParentId;
            otherMeasure.TotalScored = measure.TotalScored;
            otherMeasure.TotalScore = measure.TotalScore;
            otherMeasure.ParentMeasureName = measure.ParentMeasureName;
            otherMeasure.Subs = measure.Subs;
            otherMeasure.Sort = measure.Sort;
            otherMeasure.ApplyToWave = measure.ApplyToWave;
            otherMeasure.RelatedMeasureId = measure.RelatedMeasureId;
            otherMeasure.Links = measure.Links;
            otherMeasure.IsFirstOfParent = measure.IsFirstOfParent;
            otherMeasure.IsLastOfParent = measure.IsLastOfParent;
            return otherMeasure;
        }

        [CLIUrlAuthorize(Account = Authority.Index, PageId = PagesModel.CPALLS, Anonymity = Anonymous.Verified, Parameter = "assessmentId")]
        public ActionResult CompletionPdf(int assessmentId, int year, int schoolId, int classId, string waves,
            StudentAssessmentLanguage language, DateTime? startDate, DateTime? endDate, DateTime? dobStartDate, DateTime? dobEndDate)
        {
            var assessment = _adeBusiness.GetBaseAssessmentModel(assessmentId);
            if (assessment == null) return View("NoData");
            ViewBag.AssessmentName = assessment.Name;
            var waveMeasures = JsonHelper.DeserializeObject<Dictionary<Wave, IEnumerable<int>>>(waves);
            string teacherName = string.Empty;
            var classModel = _classBusiness.GetClassForCpalls(classId);
            DateTime dobStart = dobStartDate ?? CommonAgent.MinDate;
            DateTime dobEnd = dobEndDate ?? DateTime.Now;

            ReportList report = new ReportList();

            foreach (var v in waveMeasures)
            {
                if (v.Value.Any())
                {
                    report = _cpallsBusiness.GetReport_Class(assessmentId, year.ToSchoolYearString(), classId,
                  v.Value.ToList(), v.Key, language, startDate ?? StartDate, (endDate ?? EndDate).AddDays(1).Date,
                  dobStart, dobEnd, ref teacherName);
                    break;
                }
            }

            ViewBag.communityName = classModel.School.CommunitiesText;
            ViewBag.schoolName = classModel.School.Name;
            ViewBag.className = classModel.ClassName;
            ViewBag.language = language;
            ViewBag.year = year;
            ViewBag.teacherName = teacherName;
            ViewBag.Title = string.Format("Class {0} Completion Report", language);
            if (language == StudentAssessmentLanguage.Bilingual)
            {
                ViewBag.Title = "Class Combined Completion Report";
                ViewBag.language = "Combined";
            }

            ViewBag.report = report;
            ViewBag.json = JsonHelper.SerializeObject(report.ModelList);
            ViewBag.num = GetNum(report);

            return View();
        }

        [CLIUrlAuthorize(Account = Authority.Index, PageId = PagesModel.CPALLS, Anonymity = Anonymous.Verified, Parameter = "assessmentId")]
        public ActionResult CompletionPdf_Export(int assessmentId, int year, int schoolId, int classId, string waves,
            StudentAssessmentLanguage language, string imgSource, string imgSourcePercent, DateTime? startDate, DateTime? endDate,
            DateTime? dobStartDate, DateTime? dobEndDate, bool export = true)
        {
            var assessment = _adeBusiness.GetBaseAssessmentModel(assessmentId);
            if (assessment == null) return View("NoData");
            ViewBag.AssessmentName = assessment.Name;
            var waveMeasures = JsonHelper.DeserializeObject<Dictionary<Wave, IEnumerable<int>>>(waves);
            string teacherName = string.Empty;
            var classModel = _classBusiness.GetClassForCpalls(classId);
            DateTime dobStart = dobStartDate ?? CommonAgent.MinDate;
            DateTime dobEnd = dobEndDate ?? DateTime.Now;

            ReportList report = new ReportList();

            foreach (var v in waveMeasures)
            {
                if (v.Value.Any())
                {
                    report = _cpallsBusiness.GetReport_Class(assessmentId, year.ToSchoolYearString(), classId,
                  v.Value.ToList(), v.Key, language, startDate ?? StartDate, (endDate ?? EndDate).AddDays(1).Date,
                  dobStart, dobEnd, ref teacherName);
                    break;
                }
            }

            ViewBag.communityName = classModel.School.CommunitiesText;
            ViewBag.schoolName = classModel.School.Name;
            ViewBag.className = classModel.ClassName;
            ViewBag.language = language;
            ViewBag.year = year;
            ViewBag.teacherName = teacherName;
            ViewBag.Title = string.Format("Class {0} Completion Report", language);
            if (language == StudentAssessmentLanguage.Bilingual)
            {
                ViewBag.Title = "Class Combined Completion Report";
                ViewBag.language = "Combined";
            }

            ViewBag.report = report;
            ViewBag.num = GetNum(report);
            ViewBag.imgSource = imgSource;
            ViewBag.imgSourcePercent = imgSourcePercent;
            if (export)
            {
                GetPdf(GetViewHtml("CompletionPdf_Export"), "Class_Combined_Completion.pdf");
            }
            return View();
        }


        private int GetNum(ReportList report)
        {
            if (report == null)
                return 0;

            int numTmp = 0;

            foreach (var wave in report.ModelList.Keys)
            {
                foreach (var item in report.ModelList[wave])
                {
                    if (item.Children == null)
                        ++numTmp;
                    else
                        numTmp += item.Children.Count;
                }
            }
            return numTmp;
        }
        #endregion

        private DateTime EndDate
        {
            get
            {
                return DateTime.Now.Date;
            }
        }

        protected override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            base.OnActionExecuted(filterContext);

            ViewBag.StartDate = StartDate;
            ViewBag.EndDate = EndDate;
        }

        #region Parent Report Private Method

        public ActionResult ParentReportTemplate()
        {
            return View();
        }
        private string SetWaveCellHtml(bool hasWave1, bool hasWave2, bool hasWave3, bool isParentRow, ReportRowModel row, bool showTotal = false)
        {
            var contentStr = "";
            if (hasWave1 && hasWave2 && hasWave3)
            {
                if (isParentRow && row.Cells.Count == 6)
                {
                    contentStr += @"<td><p>" + (showTotal ? "{wave1Total}" : row.Cells[3].Text.ToString()) + @"</p>" + (showTotal ? "{AlertTotal}" : row.Cells[3].AlertText) + @"</td>
                                    <td><p>" + (showTotal ? "{wave1Tota2}" : row.Cells[4].Text.ToString()) + @"</p>" + (showTotal ? "{AlertTota2}" : row.Cells[4].AlertText) + @"</td>
                                    <td><p>" + (showTotal ? "{wave1Tota3}" : row.Cells[5].Text.ToString()) + @"</p>" + (showTotal ? "{AlertTota3}" : row.Cells[5].AlertText) + @"</td>";
                }
                else if (row.Cells.Count == 5)
                {
                    contentStr += @"<td><p>" + (showTotal ? "{wave1Total}" : row.Cells[2].Text.ToString()) + @"</p>" + (showTotal ? "{AlertTotal}" : row.Cells[2].AlertText) + @"</td>
                                    <td><p>" + (showTotal ? "{wave1Tota2}" : row.Cells[3].Text.ToString()) + @"</p>" + (showTotal ? "{AlertTota2}" : row.Cells[3].AlertText) + @"</td>
                                    <td><p>" + (showTotal ? "{wave1Tota3}" : row.Cells[4].Text.ToString()) + @"</p>" + (showTotal ? "{AlertTota3}" : row.Cells[4].AlertText) + @"</td>";
                }
            }
            else if (hasWave1 && hasWave2)
            {
                if (isParentRow && row.Cells.Count == 5)
                {
                    contentStr += @"<td><p>" + (showTotal ? "{wave1Total}" : row.Cells[3].Text.ToString()) + @"</p>" + (showTotal ? "{AlertTotal}" : row.Cells[3].AlertText) + @"</td>
                                    <td><p>" + (showTotal ? "{wave1Tota2}" : row.Cells[4].Text.ToString()) + @"</p>" + (showTotal ? "{AlertTota2}" : row.Cells[4].AlertText) + @"</td>
                                    <td></td>";
                }
                else if (row.Cells.Count == 4)
                {
                    contentStr += @"<td><p>" + (showTotal ? "{wave1Total}" : row.Cells[2].Text.ToString()) + @"</p>" + (showTotal ? "{AlertTotal}" : row.Cells[2].AlertText) + @"</td>
                                    <td><p>" + (showTotal ? "{wave1Tota2}" : row.Cells[3].Text.ToString()) + @"</p>" + (showTotal ? "{AlertTota2}" : row.Cells[3].AlertText) + @"</td>
                                    <td></td>";
                }
            }
            else if (hasWave2 && hasWave3)
            {
                if (isParentRow && row.Cells.Count == 5)
                {
                    contentStr += @"<td></td>
                                    <td><p>" + (showTotal ? "{wave1Tota2}" : row.Cells[3].Text.ToString()) + @"</p>" + (showTotal ? "{AlertTota2}" : row.Cells[3].AlertText) + @"</td>
                                    <td><p>" + (showTotal ? "{wave1Tota3}" : row.Cells[4].Text.ToString()) + @"</p>" + (showTotal ? "{AlertTota3}" : row.Cells[4].AlertText) + @"</td>";
                }
                else if (row.Cells.Count == 4)
                {
                    contentStr += @"<td></td>
                                    <td><p>" + (showTotal ? "{wave1Tota2}" : row.Cells[2].Text.ToString()) + @"</p>" + (showTotal ? "{AlertTota2}" : row.Cells[2].AlertText) + @"</td>
                                    <td><p>" + (showTotal ? "{wave1Tota3}" : row.Cells[3].Text.ToString()) + @"</p>" + (showTotal ? "{AlertTota3}" : row.Cells[3].AlertText) + @"</td>";
                }
            }
            else if (hasWave1 && hasWave3)
            {
                if (isParentRow && row.Cells.Count == 5)
                {
                    contentStr += @"<td><p>" + (showTotal ? "{wave1Total}" : row.Cells[3].Text.ToString()) + @"</p>" + (showTotal ? "{AlertTotal}" : row.Cells[3].AlertText) + @"</td>
                                    <td></td>
                                    <td><p>" + (showTotal ? "{wave1Tota3}" : row.Cells[4].Text.ToString()) + @"</p>" + (showTotal ? "{AlertTota3}" : row.Cells[4].AlertText) + @"</td>";
                }
                else if (row.Cells.Count == 4)
                {
                    contentStr += @"<td><p>" + (showTotal ? "{wave1Total}" : row.Cells[2].Text.ToString()) + @"</p>" + (showTotal ? "{AlertTotal}" : row.Cells[2].AlertText) + @"</td>
                                    <td></td>
                                    <td><p>" + (showTotal ? "{wave1Tota3}" : row.Cells[3].Text.ToString()) + @"</p>" + (showTotal ? "{AlertTota3}" : row.Cells[3].AlertText) + @"</td>";
                }
            }
            else if (hasWave1)
            {
                if (isParentRow && row.Cells.Count == 4)
                {
                    contentStr += @"<td><p>" + (showTotal ? "{wave1Total}" : row.Cells[3].Text.ToString()) + @"</p>" + (showTotal ? "{AlertTotal}" : row.Cells[3].AlertText) + @"</td><td></td><td></td>";
                }
                else if (row.Cells.Count == 3)
                {
                    contentStr += @"<td><p>" + (showTotal ? "{wave1Total}" : row.Cells[2].Text.ToString()) + @"</p>" + (showTotal ? "{AlertTotal}" : row.Cells[2].AlertText) + @"</td><td></td> <td></td>";
                }
            }
            else if (hasWave2)
            {
                if (isParentRow && row.Cells.Count == 4)
                {
                    contentStr += @"<td></td><td><p>" + (showTotal ? "{wave1Tota2}" : row.Cells[3].Text.ToString()) + @"</p>" + (showTotal ? "{AlertTota2}" : row.Cells[3].AlertText) + @"</td><td></td>";
                }
                else if (row.Cells.Count == 3)
                {
                    contentStr += @"<td></td><td><p>" + (showTotal ? "{wave1Tota2}" : row.Cells[2].Text.ToString()) + @"</p>" + (showTotal ? "{AlertTota2}" : row.Cells[2].AlertText) + @"</td><td></td>";
                }
            }
            else if (hasWave3)
            {
                if (isParentRow && row.Cells.Count == 4)
                {
                    contentStr += @"<td></td><td></td><td><p>" + (showTotal ? "{wave1Tota3}" : row.Cells[3].Text.ToString()) + @"</p>" + (showTotal ? "{AlertTota3}" : row.Cells[3].AlertText) + @"</td>";
                }
                else if (row.Cells.Count == 3)
                {
                    contentStr += @"<td></td><td></td><td><p>" + (showTotal ? "{wave1Tota3}" : row.Cells[2].Text.ToString()) + @"</p>" + (showTotal ? "{AlertTota3}" : row.Cells[2].AlertText) + @"</td>";
                }
            }

            return contentStr;
        }

        private string CreateReportName(int studentId, string wave, string assessmentName, out string displayName)
        {
            string directory = Path.Combine(SFConfig.ProtectedFiles, string.Format("ParentReports/{0}/{1}", CommonAgent.SchoolYear, studentId));
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
            var reportName = _reportBusiness.GetParentReportName(studentId, wave, assessmentName);
            displayName = reportName;
            return Path.Combine(directory, string.Format("{0}.pdf", reportName));
        }

        //为 Parent PIN Page 准备的临时PDF文件
        private string CreatePinReportTempName(int studentId, string displayName)
        {
            string directory = Path.Combine(SFConfig.ProtectedFiles, string.Format("ParentReports/{0}/{1}", CommonAgent.SchoolYear, studentId));
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
            return Path.Combine(directory, string.Format("{0}.pdf", displayName));
        }

        public ActionResult DownloadReport(int id)
        {
            var findEntity = _reportBusiness.GetParentReport(id);
            var assessment = _adeBusiness.GetAssessment(findEntity.AssessmentId);
            if (findEntity != null && assessment != null)
            {
                if (UserInfo.Parent.ParentStudents.Any(c => c.StudentId == findEntity.StudentId))
                {
                    string directory = Path.Combine(SFConfig.ProtectedFiles, string.Format("ParentReports/{0}/{1}", CommonAgent.SchoolYear, findEntity.StudentId));
                    var filePath = Path.Combine(directory, string.Format("{0}.pdf", findEntity.ReportName));

                    FileHelper.ResponseFile(filePath, findEntity.ReportName + ".pdf");
                }
            }
            return new EmptyResult();
        }

        #endregion

        #region Report Public Method
        private DateTime StartDate
        {
            get
            {
                return CommonAgent.GetStartDateOfSchoolYear();
                //return new DateTime(CommonAgent.Year, CommonAgent.YearSeparate, 1);
            }
        }
        private void SetCache(string key, object value)
        {
            key = string.Format("_{0}_{1}_{2}_", UserInfo.ID, ControllerContext.Controller, key);
            CacheHelper.Add(key, value, 10 * 60);
        }

        private T GetFromCache<T>(string key) where T : class
        {
            key = string.Format("_{0}_{1}_{2}_", UserInfo.ID, ControllerContext.Controller, key);
            var value = CacheHelper.Get<T>(key);
            return value;
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

        private void SavePdf(string html, string fileName, AssessmentLanguage language, PdfType type = PdfType.Assessment_Landscape, string startPdfPath = "", int pdfPageWidth = 0)
        {
            PdfProvider pdfProvider = new PdfProvider(type, "", startPdfPath, "", "#reportHtml", pdfPageWidth);

            pdfProvider.SavePdf(html, fileName);
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