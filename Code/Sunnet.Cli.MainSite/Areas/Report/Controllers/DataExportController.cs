using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using StructureMap;
using Sunnet.Cli.Business.Ade;
using Sunnet.Cli.Business.Ade.Models;
using Sunnet.Cli.Business.Common;
using Sunnet.Cli.Business.Communities;
using Sunnet.Cli.Business.Cpalls.Models;
using Sunnet.Cli.Business.Permission;
using Sunnet.Cli.Business.Reports;
using Sunnet.Cli.Business.Schools;
using Sunnet.Cli.Core;
using Sunnet.Cli.Core.Ade;
using Sunnet.Cli.Core.Communities.Entities;
using Sunnet.Cli.Core.Cpalls;
using Sunnet.Cli.Core.Reports;
using Sunnet.Cli.Core.Reports.Models;
using Sunnet.Cli.Core.Schools.Entities;
using Sunnet.Cli.Core.Users.Enums;
using Sunnet.Cli.MainSite.Controllers;
using Sunnet.Cli.UIBase;
using Sunnet.Cli.UIBase.Filters;
using Sunnet.Cli.UIBase.Models;
using Sunnet.Framework;
using Sunnet.Framework.Core.Tool;
using Sunnet.Framework.Encrypt;
using Sunnet.Framework.Helpers;
using Sunnet.Framework.Permission;
using Sunnet.Framework.Resources;
using Sunnet.Framework.Extensions;
using Sunnet.Cli.Business.Export.Models;
using Sunnet.Cli.Business.Users;
using Sunnet.Cli.Core.Common.Enums;
using Sunnet.Cli.Core.Reports.Entities;
using Sunnet.Cli.Core.Export.Enums;
using Sunnet.Framework.Log;
using Sunnet.Framework.SFTP;

namespace Sunnet.Cli.MainSite.Areas.Report.Controllers
{
    public class DataExportController : BaseController
    {
        readonly PermissionBusiness _permissionBusiness;
        readonly AdeBusiness _adeBusiness;
        readonly CommunityBusiness _communityBusiness;
        readonly SchoolBusiness _schoolBusiness;
        readonly ReportBusiness _reportBusiness;
        private readonly IEncrypt encrypter;

        public DataExportController()
        {
            _adeBusiness = new AdeBusiness(AdeUnitWorkContext);
            _permissionBusiness = new PermissionBusiness(UnitWorkContext);
            _communityBusiness = new CommunityBusiness(UnitWorkContext);
            _schoolBusiness = new SchoolBusiness(UnitWorkContext);
            _reportBusiness = new ReportBusiness(UnitWorkContext);


            encrypter = ObjectFactory.GetInstance<IEncrypt>();
        }

        [CLIUrlAuthorize(Account = Authority.Index, PageId = PagesModel.CIRCLEDataExport, Anonymity = Anonymous.Verified)]
        public ActionResult Index()
        {
            return View();
        }

        [CLIUrlAuthorize(Account = Authority.Index, PageId = PagesModel.CIRCLEDataExport, Anonymity = Anonymous.Verified)]
        public ActionResult Circle()
        {
            HttpContext.Response.Cache.SetNoStore();

            List<int> accountPageId = _permissionBusiness.CheckPage(UserInfo);
            List<int> pageIds = accountPageId.FindAll(r => r > SFConfig.AssessmentPageStartId);
            List<CpallsAssessmentModel> list = _adeBusiness.GetAssessmentCpallsList();
            List<CpallsAssessmentModel> accessList = list.FindAll(r => pageIds.Contains(r.ID + SFConfig.AssessmentPageStartId));

            List<CpallsAssessmentModel> available = new List<CpallsAssessmentModel>();
            foreach (CpallsAssessmentModel model in accessList)
            {
                if (model.Language == AssessmentLanguage.English)
                    available.Add(model);
                else if (accessList.Count(x => x.Name == model.Name && x.Language != model.Language) == 0)
                    available.Add(model);
            }
            ViewBag.List = available;
            return View();
        }

        [CLIUrlAuthorize(Account = Authority.Index, PageId = PagesModel.CIRCLEDataExport, Anonymity = Anonymous.Verified)]
        public ActionResult SelectMeasures(int id)
        {
            List<SelectListItem> yearList = new SelectList(CommonAgent.GetYears(), "ID", "Name").ToList();
            yearList.ForEach(r => r.Selected = false);
            SelectListItem currentYear = yearList.Find(r => r.Value == CommonAgent.Year.ToString());
            if (currentYear != null)
                currentYear.Selected = true;
            ViewBag.YearOptions = yearList;

            var assessment = _adeBusiness.GetBaseAssessmentModelForReport(id);
            if (assessment == null)
            {
                throw new HttpException(404, "Assessment is required.");
            }
            var customScores = _adeBusiness.GetScoresByAssessmentId(id);
            if(assessment.Language == AssessmentLanguage.English)
            {
                ViewBag.CustomScores = customScores;
            }
            else if(assessment.Language == AssessmentLanguage.Spanish)
            {
                ViewBag.OtherAssessmentCustomScores = customScores;
            }
            var measures = _adeBusiness.GetAllMeasureReport(id).Where(x => x.ParentId == 1).OrderBy(x => x.Sort).ToList();
            List<MeasureReportModel> otherLanguageMeasures = null;
            if (assessment.TheOtherId > 0)
            {
                otherLanguageMeasures =
                    _adeBusiness.GetAllMeasureReport(assessment.TheOtherId).Where(x => x.ParentId == 1).ToList();
                var otherAssessmentCustomScores = _adeBusiness.GetScoresByAssessmentId(assessment.TheOtherId);
                if (assessment.Language == AssessmentLanguage.English)
                {
                    ViewBag.OtherAssessmentCustomScores = otherAssessmentCustomScores;
                }
                else if (assessment.Language == AssessmentLanguage.Spanish)
                {
                    ViewBag.CustomScores = otherAssessmentCustomScores;
                }
            }
            var englishMeasures = assessment.Language == AssessmentLanguage.English ? measures : otherLanguageMeasures;
            var spanishMeasures = assessment.Language == AssessmentLanguage.Spanish ? measures : otherLanguageMeasures;

            var result = new Dictionary<string, object>();

            var englishMaxSort = 0;

            if (englishMeasures != null)
            {
                englishMaxSort = englishMeasures.Max(x => x.Sort) + 1;
                result.Add("English", new
                {
                    length = englishMeasures.Count,
                    measures = englishMeasures
                });
            }
            else
            {
                result.Add("English", new
                {
                    length = 0,
                    measures = false
                });
            }

            if (spanishMeasures != null)
            {
                if (englishMeasures != null)
                {
                    spanishMeasures.ForEach(mea =>
                    {
                        var relatedEnglishMeasure = englishMeasures.FirstOrDefault(x => x.ID == mea.RelatedMeasureId);
                        if (relatedEnglishMeasure != null)
                            mea.Sort = relatedEnglishMeasure.Sort;
                        else
                            mea.Sort = englishMaxSort + mea.Sort;
                    });
                    spanishMeasures = spanishMeasures.OrderBy(x => x.Sort).ToList();
                }
                result.Add("Spanish", new
                {
                    length = spanishMeasures.Count,
                    measures = spanishMeasures
                });
            }
            else
            {
                result.Add("Spanish", new
                {
                    length = 0,
                    measures = false
                });
            }
            ViewBag.Measures = JsonHelper.SerializeObject(result);

            var assReportTemps = _reportBusiness.GetAssReportTempsSelectList(UserInfo, id);
            ViewBag.temps = assReportTemps.ToSelectList(ViewTextHelper.DefaultPleaseSelectText, "0");
            ViewBag.AssessmentId = id;
            ViewBag.ReceiveFileOptions = ReceiveFileBy.DownloadLink.ToSelectList();
            ViewBag.FileTypeOptions = ExportFileType.Comma.ToSelectList();
            return View(assessment);
        }

        private string title = "CIRCLE Data Export";

        [CLIUrlAuthorize(Account = Authority.All, PageId = PagesModel.Administrative, Anonymity = Anonymous.Logined)]
        public string Export_Queue(int assessmentId, int communityId, int year, List<Wave> waves,
            DateTime? startDate, DateTime? endDate,
            DateTime? dobStartDate, DateTime? dobEndDate,
            List<int> customScores, List<int> otherCustomScores,
            List<int> englishResults, List<int> englishItemLevel,
            List<int> spanishResults, List<int> spanishItemLevel,
            DateTime FromDate, DateTime ToDate, int Frequency, FrequencyUnitType FrequencyUnit,
            ReceiveFileBy ReceiveFileBy, string SFTPHostIp, int? SFTPPort, string SFTPFilePath,
            string SFTPUserName, string SFTPPassword, ExportFileType FileType, int schoolId = 0)
        {
            List<DateTime> dates = CalcExcuteDate(FromDate, ToDate, Frequency, FrequencyUnit);
            int _sftpPort = SFTPPort == null ? 22 : SFTPPort.Value;
            string _sftpFilePath = SFTPFilePath == "" ? "/" : SFTPFilePath;
            IEncrypt encrypt = ObjectFactory.GetInstance<IEncrypt>();
            string _sftpPassword = SFTPPassword == "" ? "" : encrypt.Encrypt(SFTPPassword);

            customScores = customScores ?? new List<int>();
            otherCustomScores = otherCustomScores ?? new List<int>();
            customScores.AddRange(otherCustomScores);
            englishResults = englishResults ?? new List<int>();
            englishItemLevel = englishItemLevel ?? new List<int>();
            spanishResults = spanishResults ?? new List<int>();
            spanishItemLevel = spanishItemLevel ?? new List<int>();

            var response = new PostFormResponse();

            var dicParams = new Dictionary<string, object>();
            dicParams.Add("assessmentId", assessmentId);
            dicParams.Add("communityId", communityId);
            dicParams.Add("year", year);
            dicParams.Add("schoolId", schoolId);
            dicParams.Add("waves", waves.Select(x => (int)x).ToArray());
            dicParams.Add("customScores", customScores);
            dicParams.Add("englishResults", englishResults);
            dicParams.Add("englishItemLevel", englishItemLevel);
            dicParams.Add("spanishResults", spanishResults);
            dicParams.Add("spanishItemLevel", spanishItemLevel);
            //dicParams.Add("startDate", startDate == null ? new DateTime(CommonAgent.Year, CommonAgent.YearSeparate, 1) : (DateTime)startDate);
            dicParams.Add("startDate", startDate == null ? CommonAgent.GetStartDateOfSchoolYear() : (DateTime)startDate);
            dicParams.Add("endDate", endDate == null ? DateTime.Now.AddDays(1).Date : ((DateTime)endDate).AddDays(1));
            dicParams.Add("dobStartDate", dobStartDate ?? CommonAgent.MinDate);
            dicParams.Add("dobEndDate", dobEndDate ?? DateTime.Now);
            var queryParams = JsonHelper.SerializeObject(dicParams);

            var assessment = _adeBusiness.GetAssessmentModel(assessmentId);
            title = assessment.Name+ " Results Export";
            response.Update(_reportBusiness.SubmitReportList(title, ReportQueueType.CIRCLE_Data_Export, queryParams,
                    Url.Action("Export_Report") + "/{ID}", UserInfo, dates,
                    ReceiveFileBy, SFTPHostIp, _sftpPort, _sftpFilePath, SFTPUserName, _sftpPassword, FileType));
            if (response.Success)
            {
                if (!IsExistTemplate(englishResults, englishItemLevel, spanishResults, spanishItemLevel))
                {
                    string ids = "{\"englishResults\":\"" + string.Join(",", englishResults) +
                        "\",\"englishItemLevel\":\"" + string.Join(",", englishItemLevel) +
                        "\",\"spanishResults\":\"" + string.Join(",", spanishResults) +
                        "\",\"spanishItemLevel\":\"" + string.Join(",", spanishItemLevel) +
                        "\"}";
                    response.Message = "SaveTemp";
                    response.Data = ids;
                }
                else
                    response.Message = ResourceHelper.GetRM().GetInformation("Report_Queue_Submitted");
            }
            return JsonHelper.SerializeObject(response);
        }

        [CLIUrlAuthorize(Account = Authority.Index, PageId = PagesModel.CIRCLEDataExport, Anonymity = Anonymous.Verified)]
        public ActionResult Export_Report(string id)
        {
            var reportId = 0;
            if (!int.TryParse(id, out reportId))
            {
                int.TryParse(encrypter.Decrypt(id), out reportId);
            }
            var reportEntity = _reportBusiness.GetReportQueue(reportId);

            if (reportEntity == null)
                throw new HttpException(404, ResourceHelper.GetRM().GetInformation("Report_Queue_Report_404"));
            if (reportEntity.Status < ReportQueueStatus.Processed)
                throw new HttpException(404, ResourceHelper.GetRM().GetInformation("Report_Queue_Report_Not_Ready"));
            if (reportEntity.CreatedBy != UserInfo.ID)
                return new RedirectResult("/error/nonauthorized");
            if (reportEntity.UpdatedOn.AddDays(SFConfig.ReportExpire) < DateTime.Now.AddDays(1))
                return new RedirectResult(string.Format("{0}/expire.html", DomainHelper.SsoSiteDomain));

            ViewBag.Title = reportEntity.Title;
            ViewBag.CanDownload = reportEntity.Status >= ReportQueueStatus.Processed;
            return View();
        }

        [CLIUrlAuthorize(Account = Authority.Index, PageId = PagesModel.CIRCLEDataExport, Anonymity = Anonymous.Verified)]
        public ActionResult Export_Dl(string id, bool export = true)
        {
            var reportId = 0;
            if (!int.TryParse(id, out reportId))
            {
                int.TryParse(encrypter.Decrypt(id), out reportId);
            }
            var reportEntity = _reportBusiness.GetReportQueue(reportId);
            if (reportEntity == null)
                throw new HttpException(404, ResourceHelper.GetRM().GetInformation("Report_Queue_Report_404"));
            if (reportEntity.Status < ReportQueueStatus.Processed)
                throw new HttpException(404, ResourceHelper.GetRM().GetInformation("Report_Queue_Report_Not_Ready"));
            var queryParams = JsonConvert.DeserializeObject<Dictionary<string, object>>(reportEntity.QueryParams);
            var communityId = int.Parse(queryParams["communityId"].ToString());

            ViewBag.Pdf = export;
            string filename = string.IsNullOrEmpty(reportEntity.Report)
                ? "Community/" + communityId + "/CircleDataExport_" +
                  reportEntity.CreatedOn.ToString("yyyy_MM_dd_HH_mm_ss_fff") +
                  encrypter.Encrypt(reportEntity.CreatedBy.ToString()).Substring(0, 4) + ".xlsx"
                : reportEntity.Report;

            string outPutFilename = reportEntity.Title
                + reportEntity.UpdatedOn.ToString("_yyyyMMdd_HHmmss")
                + Path.GetExtension(reportEntity.Report);
            var localFile = FileHelper.HasProtectedFile(filename);
            if (string.IsNullOrEmpty(localFile))
            {
                return Content("The report is damaged.");
            }
            if (export)
            {
                FileHelper.ResponseFile(localFile, outPutFilename);
            }
            return new EmptyResult();
        }

        [CLIUrlAuthorize(Account = Authority.All, PageId = PagesModel.Administrative, Anonymity = Anonymous.Logined)]
        public string GetCommunitySelectListForSearch(string keyword, int communityId = -1)
        {
            var expression = PredicateHelper.True<CommunityEntity>();

            if (communityId > 0)
                expression = expression.And(o => o.ID == communityId);
            var list = _communityBusiness.GetCommunitySelectList(UserInfo, expression, false);
            return JsonHelper.SerializeObject(list);
        }

        [CLIUrlAuthorize(Account = Authority.All, PageId = PagesModel.Administrative, Anonymity = Anonymous.Logined)]
        public string GetSchoolSelectList(string keyword, int communityId = 0, string schoolName = "", bool isActive = true)
        {
            var expression = PredicateHelper.True<SchoolEntity>();
            if (communityId > 0)
                expression = expression.And(s => s.CommunitySchoolRelations.Any(csr => csr.CommunityId == communityId));
            if (schoolName != null && schoolName.Trim() != string.Empty)
                expression = expression.And(o => o.BasicSchool.Name.Contains(schoolName));
            var schoolList = _schoolBusiness.GetSchoolsSelectList(UserInfo, expression, isActive);
            return JsonHelper.SerializeObject(schoolList);
        }

        #region Assessment Report Template
        [CLIUrlAuthorize(Account = Authority.Index, PageId = PagesModel.CIRCLEDataExport, Anonymity = Anonymous.Verified)]
        public ActionResult FillTempName(string ids, string assessmentId)
        {
            ViewBag.ids = ids;
            ViewBag.assessmentId = assessmentId;
            return View();
        }

        [CLIUrlAuthorize(Account = Authority.Index, PageId = PagesModel.CIRCLEDataExport, Anonymity = Anonymous.Verified)]
        public string NewTemp(string TepmName, string ids, int assessmentId)
        {
            var response = new PostFormResponse();
            if (_reportBusiness.SearchSameTemps(TepmName, assessmentId, UserInfo))
            {
                response.Success = false;
                response.Message = GetInformation("ReportTempNameExists");
                return JsonConvert.SerializeObject(response);
            }
            OperationResult result = _reportBusiness.InsertTemplate(UserInfo, TepmName, ids, assessmentId);
            response.Success = result.ResultType == OperationResultType.Success;
            response.Message = result.Message;
            response.Data = result.AppendData.ToString().Split(',');
            return JsonHelper.SerializeObject(response);
        }

        [CLIUrlAuthorize(Account = Authority.Index, PageId = PagesModel.CIRCLEDataExport, Anonymity = Anonymous.Verified)]
        private bool IsExistTemplate
            (List<int> englishResults, List<int> englishItemLevel, List<int> spanishResults, List<int> spanishItemLevel)
        {
            List<string> englishResultIds = englishResults.Select(s => s.ToString()).ToList();
            List<string> englishItemLevelIds = englishItemLevel.Select(s => s.ToString()).ToList();
            List<string> spanishResultIds = spanishResults.Select(s => s.ToString()).ToList();
            List<string> spanishItemLevelIds = spanishItemLevel.Select(s => s.ToString()).ToList();
            List<string> IdsJosnList = _reportBusiness.GetAllTemplate(UserInfo);
            ReportTemplateIdsModel idsModel = new ReportTemplateIdsModel();
            foreach (string ids in IdsJosnList)
            {
                try
                {
                    idsModel = JsonHelper.DeserializeObject<ReportTemplateIdsModel>(ids);
                }
                catch
                {
                    continue;
                }
                if (idsModel == null)
                    continue;
                List<string> englishResultModel = new List<string>();
                List<string> englishItemLevelModel = new List<string>();
                List<string> spanishResultModel = new List<string>();
                List<string> spanishItemLevelModel = new List<string>();
                if (idsModel.englishResults != "")
                    englishResultModel = idsModel.englishResults.Split(',').ToList();
                if (idsModel.englishItemLevel != "")
                    englishItemLevelModel = idsModel.englishItemLevel.Split(',').ToList();
                if (idsModel.spanishResults != "")
                    spanishResultModel = idsModel.spanishResults.Split(',').ToList();
                if (idsModel.spanishItemLevel != "")
                    spanishItemLevelModel = idsModel.spanishItemLevel.Split(',').ToList();
                if (englishResultIds.Except(englishResultModel).ToList().Count == 0
                    && englishResultModel.Except(englishResultIds).ToList().Count == 0
                    && englishItemLevelIds.Except(englishItemLevelModel).ToList().Count == 0
                    && englishItemLevelModel.Except(englishItemLevelIds).ToList().Count == 0
                    && spanishResultIds.Except(spanishResultModel).ToList().Count == 0
                    && spanishResultModel.Except(spanishResultIds).ToList().Count == 0
                    && spanishItemLevelIds.Except(spanishItemLevelModel).ToList().Count == 0
                    && spanishItemLevelModel.Except(spanishItemLevelIds).ToList().Count == 0)
                {
                    return true;
                }
            }
            return false;
        }

        [CLIUrlAuthorize(Account = Authority.Index, PageId = PagesModel.CIRCLEDataExport, Anonymity = Anonymous.Verified)]
        public string BindTemp(int tempId = -1)
        {
            if (tempId == -1)
                return null;
            var AsstempEntity = _reportBusiness.GetAssReportTempById(tempId);
            return AsstempEntity.Ids;

        }

        [CLIUrlAuthorize(Account = Authority.Index, PageId = PagesModel.CIRCLEDataExport, Anonymity = Anonymous.Verified)]
        public ActionResult AssReportTempList(int id)
        {
            ViewBag.assessmentId = id;
            return View();
        }

        [CLIUrlAuthorize(Account = Authority.Index, PageId = PagesModel.CIRCLEDataExport, Anonymity = Anonymous.Verified)]
        public string SearchAssTemp(int assessmentId, string keyWord = "")
        {
            List<ReportTemplateWithUserModel> list = null;
            list = _reportBusiness.GetReportTempsSelectListOther(assessmentId, keyWord, UserInfo);
            list = list ?? new List<ReportTemplateWithUserModel>();
            if (list.Count() > 0)
            {
                List<int> userIds = list.Select(r => r.CreatedBy).ToList();
                if (userIds != null && userIds.Count > 0)
                {
                    List<UsernameModel> list_Names = new UserBusiness().GetUsernames(userIds);
                    if (list_Names.Count > 0)
                    {
                        foreach (ReportTemplateWithUserModel item in list)
                        {
                            UsernameModel namemodel = list_Names.Find(r => r.ID == item.CreatedBy);
                            item.CreatedUserName = namemodel == null ? "" : namemodel.Firstname + " " + namemodel.Lastname;
                        }
                    }
                }
            }
            var result = new { total = list.Count(), data = list };
            return JsonHelper.SerializeObject(result);
        }

        [CLIUrlAuthorize(Account = Authority.Index, PageId = PagesModel.CIRCLEDataExport, Anonymity = Anonymous.Verified)]
        public string UpdateAssTemp(int tempId, string tempName, sbyte tempStatus, int assessmentId)
        {
            var response = new PostFormResponse();
            OperationResult result = new OperationResult(OperationResultType.Warning);
            if (_reportBusiness.SearchSameTemps(tempName, assessmentId, UserInfo, tempId))
            {
                response.Success = false;
                response.Message = GetInformation("ReportTempNameExists");
                return JsonConvert.SerializeObject(response);
            }
            else
            {
                AssessmentReportTemplateEntity entity = _reportBusiness.GetAssReportTempById(tempId);
                entity.Name = tempName;
                entity.Status = (EntityStatus)tempStatus;
                result = _reportBusiness.UpdateAssReportTemp(entity);
                response.Success = result.ResultType == OperationResultType.Success;
                response.Message = result.Message;
                return JsonConvert.SerializeObject(response);
            }
        }

        [CLIUrlAuthorize(Account = Authority.Index, PageId = PagesModel.CIRCLEDataExport, Anonymity = Anonymous.Verified)]
        public string DeleteAssTemp(int id = -1)
        {
            var response = new PostFormResponse();
            OperationResult result = _reportBusiness.DeleteAssReportTemp(id);
            response.Success = result.ResultType == OperationResultType.Success;
            response.Message = result.Message;
            return JsonHelper.SerializeObject(response);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [CLIUrlAuthorize(Account = Authority.Index, PageId = PagesModel.CIRCLEDataExport, Anonymity = Anonymous.Verified)]
        private List<DateTime> CalcExcuteDate
            (DateTime startDate, DateTime stopDate, int frequency, FrequencyUnitType unitType)
        {
            List<DateTime> dates = new List<DateTime>();
            switch (unitType)
            {
                case FrequencyUnitType.Day:
                    while (startDate <= stopDate)
                    {
                        dates.Add(startDate);
                        startDate = startDate.AddDays(frequency);
                    }
                    break;
                case FrequencyUnitType.Week:
                    while (startDate <= stopDate)
                    {
                        dates.Add(startDate);
                        startDate = startDate.AddDays(7 * frequency);
                    }
                    break;
                case FrequencyUnitType.Month:
                    while (startDate <= stopDate)
                    {
                        dates.Add(startDate);
                        startDate = startDate.AddMonths(frequency);
                    }
                    break;
            }
            return dates;
        }
        #endregion
        [CLIUrlAuthorize(Account = Authority.Index, PageId = PagesModel.CIRCLEDataExport, Anonymity = Anonymous.Verified)]
        public string ValidateSFTP(string hostIP, int hostPort, string userName, string password)
        {
            ISunnetLog LoggerHelper = ObjectFactory.GetInstance<ISunnetLog>();
            SFTPHelper sfptHelper = new SFTPHelper
                (hostIP, hostPort, userName, password, LoggerHelper);
            if (sfptHelper.Connect())
            {
                sfptHelper.Disconnect();
                return "success";
            }
            else
                return "fail";
        }

    }
}