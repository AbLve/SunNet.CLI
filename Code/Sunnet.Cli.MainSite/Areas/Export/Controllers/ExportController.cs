using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Text;
using Newtonsoft.Json;
using System.Web.Script.Serialization;

using Sunnet.Cli.MainSite.Controllers;
using Sunnet.Cli.Business.Export;
using Sunnet.Cli.Business.Export.Models;
using Sunnet.Cli.Core.Export.Entities;
using Sunnet.Framework.Core.Tool;
using Sunnet.Framework.Extensions;
using Sunnet.Cli.UIBase.Models;
using Sunnet.Cli.Core.Common.Enums;
using Sunnet.Cli.UIBase.Filters;
using Sunnet.Framework.Permission;
using Sunnet.Cli.Business.Common.Enum;
using Sunnet.Cli.Core.Reports;
using Sunnet.Framework.Encrypt;
using StructureMap;
using Sunnet.Framework.Resources;
using Sunnet.Cli.Core.Export.Enums;
using Sunnet.Cli.UIBase;
using Sunnet.Cli.Core.Communities.Entities;
using Sunnet.Cli.Business.Users;
using Sunnet.Cli.Business.Common;
using Sunnet.Framework.Log;
using Sunnet.Framework.SFTP;

namespace Sunnet.Cli.MainSite.Areas.Export.Controllers
{
    public class ExportController : BaseController
    {
        private readonly ExportBusiness _exportBussiness;
        private readonly IEncrypt _encrypt;
        public ExportController()
        {
            _exportBussiness = new ExportBusiness(UnitWorkContext);
            _encrypt = ObjectFactory.GetInstance<IEncrypt>();
        }

        #region view
        [CLIUrlAuthorizeAttribute(Account = Authority.Index, PageId = PagesModel.Export, Anonymity = Anonymous.Verified)]
        public ActionResult Index()
        {
            var reportTemps = _exportBussiness.GetReportTempsSelectList(UserInfo);
            ViewBag.temps = reportTemps.ToSelectList(ViewTextHelper.DefaultPleaseSelectText, "0");
            ViewBag.FileTypeOptions = ExportFileType.Comma.ToSelectList();
            ViewBag.ReceiveFileOptions = ReceiveFileBy.DownloadLink.ToSelectList();
            var exportModel = new ExportInfoModel();
            exportModel.StartDate = DateTime.Now;
            exportModel.StopDate = DateTime.Now;
            exportModel.Frequency = 1;
            exportModel.FrequencyUnit = FrequencyUnitType.Day;
            exportModel.FtpPort = 22;
            return View(exportModel);
        }

        [CLIUrlAuthorizeAttribute(Account = Authority.Index, PageId = PagesModel.Export, Anonymity = Anonymous.Verified)]
        public ViewResult ReportTempList()
        {
            return View();
        }

        [CLIUrlAuthorizeAttribute(Account = Authority.Index, PageId = PagesModel.Export, Anonymity = Anonymous.Verified)]
        public ActionResult FillTempName(string fields)
        {
            ViewBag.fields = fields;
            return View();
        }

        [CLIUrlAuthorizeAttribute(Account = Authority.Index, PageId = PagesModel.Export, Anonymity = Anonymous.Verified)]
        public ActionResult ExcuteSchedule()
        {
            var exportModel = new ExportInfoModel();
            return View(exportModel);
        }
        #endregion

        #region ExportInfo

        [HttpPost]
        [ValidateAntiForgeryToken]
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        [CLIUrlAuthorizeAttribute(Account = Authority.Index, PageId = PagesModel.Export, Anonymity = Anonymous.Verified)]
        public string NewExportInfo(ExportInfoModel model, int[] chk)
        {
            #region 计算执行时间
            DateTime startDate = model.StartDate == null ? DateTime.Now : model.StartDate.Value;
            DateTime stopDate = model.StopDate == null ? DateTime.Now.AddDays(1) : model.StopDate.Value;
            int frequency = model.Frequency == null ? 1 : model.Frequency.Value;
            FrequencyUnitType unitType = model.FrequencyUnit;
            List<DateTime> dates = CalcExcuteDate(startDate, stopDate, frequency, unitType);
            #endregion

            #region 根据选择字段拼接m条sql，对应m个实体
            List<int> checkedFieldList = chk.ToList();
            List<int> objectFieldList = checkedFieldList.Where(r => r <= 500).ToList();
            List<int> userFieldList = checkedFieldList.Where(r => r > 500).ToList();

            List<ExportInfoEntity> exportInfoList = new List<ExportInfoEntity>();
            Random ra = new Random();
            int raNum = ra.Next(0, 1000);
            string groupName = DateTime.Now.ToString("yyyyMMddHHmmssfff") + raNum;

            IEncrypt encrypt = ObjectFactory.GetInstance<IEncrypt>();
            string ftpPassword = model.FtpPassword == null ? "" : encrypt.Encrypt(model.FtpPassword);

            int communityId = model.CommunityId;
            if (objectFieldList.Count > 0)
            {
                string exportSql = _exportBussiness.GetObjectExportSql(UserInfo, objectFieldList, communityId);
                exportInfoList.Add(_exportBussiness.ExportInfoModelToEntity
                    (exportSql, groupName, ftpPassword, model, UserInfo));
            }
            if (userFieldList.Count > 0)
            {
                List<string> userExportSqlList = _exportBussiness.GetUserExportSqlList(userFieldList, UserInfo, communityId);
                userExportSqlList.ForEach
                    (x => exportInfoList.Add(_exportBussiness.ExportInfoModelToEntity
                        (x, groupName, ftpPassword, model, UserInfo)));
            }
            #endregion

            #region 执行时间天数(n)*m=数据条数
            List<ExportInfoEntity> attachedDateEntities = new List<ExportInfoEntity>();
            foreach (DateTime dt in dates)
            {
                foreach (ExportInfoEntity entity in exportInfoList)
                {
                    ExportInfoEntity entityAttachedDate = new ExportInfoEntity();
                    entityAttachedDate.CreaterMail = entity.CreaterMail;
                    entityAttachedDate.CreaterFirstName = entity.CreaterFirstName;
                    entityAttachedDate.CreaterLastName = entity.CreaterLastName;
                    entityAttachedDate.ExecuteSQL = entity.ExecuteSQL;
                    entityAttachedDate.FileName = entity.FileName;
                    entityAttachedDate.DownloadUrl = entity.DownloadUrl;
                    entityAttachedDate.FileUrl = entity.FileUrl;
                    entityAttachedDate.Status = entity.Status;
                    entityAttachedDate.CreatedBy = entity.CreatedBy;
                    entityAttachedDate.GroupName = entity.GroupName;
                    entityAttachedDate.ExcuteTime = dt;
                    entityAttachedDate.FileType = entity.FileType;

                    entityAttachedDate.ReceiveFileBy = entity.ReceiveFileBy;
                    entityAttachedDate.FtpHostIp = entity.FtpHostIp;
                    entityAttachedDate.FtpPort = entity.FtpPort;
                    entityAttachedDate.FtpEnableSsl = entity.FtpEnableSsl;
                    entityAttachedDate.FtpFilePath = entity.FtpFilePath;
                    entityAttachedDate.FtpUserName = entity.FtpUserName;
                    entityAttachedDate.FtpPassword = entity.FtpPassword;
                    attachedDateEntities.Add(entityAttachedDate);
                }
            }
            #endregion
            var response = new PostFormResponse();
            OperationResult result = _exportBussiness.InsertExportInfoList(attachedDateEntities);
            response.Success = result.ResultType == OperationResultType.Success;
            if (response.Success)
            {
                string checkedFields = string.Join(",", chk.Select(i => i.ToString()).ToArray());
                if (CompareFieds(checkedFields))
                    response.Message = result.Message;
                else
                {
                    response.Data = checkedFields;
                    response.Message = "saveTemp";
                }
            }
            else
            {
                response.Message = result.Message;
            }
            return JsonConvert.SerializeObject(response);
        }

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
        #endregion

        #region ReportTemplate

        [CLIUrlAuthorizeAttribute(Account = Authority.Index, PageId = PagesModel.Export, Anonymity = Anonymous.Verified)]
        public string SearchTemp(string keyWord = "")
        {
            List<ReportTemplateWithUserModel> list = null;
            list = _exportBussiness.GetReportTempsSelectListOther(keyWord, UserInfo);
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

        [CLIUrlAuthorizeAttribute(Account = Authority.Index, PageId = PagesModel.Export, Anonymity = Anonymous.Verified)]
        public string UpdateTemp(int tempId, string tempName, sbyte tempStatus)
        {
            var response = new PostFormResponse();
            OperationResult result = new OperationResult(OperationResultType.Warning);
            if (_exportBussiness.SearchSameTemps(tempName, tempId))
            {
                response.Success = false;
                response.Message = GetInformation("ReportTempNameExists");
                return JsonConvert.SerializeObject(response);
            }
            else
            {
                ReportTemplateEntity entity = _exportBussiness.GetReportTempsById(tempId);
                entity.Name = tempName;
                entity.Status = (EntityStatus)tempStatus;
                result = _exportBussiness.UpdateReportTemp(entity);
                response.Success = result.ResultType == OperationResultType.Success;
                response.Message = result.Message;
                return JsonConvert.SerializeObject(response);
            }
        }

        [CLIUrlAuthorizeAttribute(Account = Authority.Index, PageId = PagesModel.Export, Anonymity = Anonymous.Verified)]
        public string DeleteTemp(int id = -1)
        {
            var response = new PostFormResponse();
            OperationResult result = _exportBussiness.DeleteReportTemp(id);
            response.Success = result.ResultType == OperationResultType.Success;
            response.Message = result.Message;
            return JsonHelper.SerializeObject(response);
        }

        [CLIUrlAuthorizeAttribute(Account = Authority.Index, PageId = PagesModel.Export, Anonymity = Anonymous.Verified)]
        public string NewReportTemp(string TepmName, string fields)
        {
            var response = new PostFormResponse();
            if (_exportBussiness.SearchSameTemps(TepmName))
            {
                response.Success = false;
                response.Message = GetInformation("ReportTempNameExists");
                return JsonConvert.SerializeObject(response);
            }
            ReportTemplateEntity reportTemp = new ReportTemplateEntity();
            reportTemp.Name = TepmName;

            reportTemp.Fields = fields;

            reportTemp.CreatedBy = UserInfo.ID;
            reportTemp.UpdatedBy = UserInfo.ID;
            reportTemp.Status = EntityStatus.Active;

            OperationResult result = _exportBussiness.InsertReportTemp(reportTemp);
            response.Success = result.ResultType == OperationResultType.Success;
            response.Message = result.Message;
            return JsonConvert.SerializeObject(response);
        }

        [CLIUrlAuthorizeAttribute(Account = Authority.Index, PageId = PagesModel.Export, Anonymity = Anonymous.Verified)]
        public string BindTemp(int tempId = -1)
        {
            if (tempId == -1)
                return "";
            ReportTemplateEntity tempEntity = _exportBussiness.GetReportTempsById(tempId);
            string bindFields = tempEntity.Fields;
            return bindFields;
        }

        /// <summary>
        /// 验证模板是否已存在
        /// </summary>
        /// <param name="checkedFields"></param>
        /// <returns></returns>
        private bool CompareFieds(string checkedFields)
        {
            string[] checkedFieldArr = checkedFields.Split(',');
            var reportTemps = _exportBussiness.GetReportTemps().Select(r => r.Fields).ToList();
            foreach (string fields in reportTemps)
            {
                string[] tempFieldArr = fields.Split(',');
                if (tempFieldArr.Except(checkedFieldArr).ToList().Count == 0
                    && checkedFieldArr.Except(tempFieldArr).ToList().Count == 0)
                {
                    return true;
                }
            }
            return false;
        }

        #endregion

        #region FieldMap
        [CLIUrlAuthorizeAttribute(Account = Authority.Index, PageId = PagesModel.Export, Anonymity = Anonymous.Verified)]
        public string GetAllFields()
        {
            var allFiels = _exportBussiness.GetAllFilds(UserInfo);
            return JsonHelper.SerializeObject(allFiels);
        }

        #endregion

        #region download
        [CLIUrlAuthorizeAttribute(Account = Authority.Index, PageId = PagesModel.Export, Anonymity = Anonymous.Verified)]
        public ActionResult DowloadFile(string id)
        {
            int exportId = int.Parse(_encrypt.Decrypt(id));
            ExportInfoEntity exportInfo = _exportBussiness.GetExportInfosById(exportId);
            if (exportInfo == null)
                throw new HttpException(404, ResourceHelper.GetRM().GetInformation("Report_Queue_Report_404"));
            ViewBag.Creater = true;
            if (UserInfo.ID != exportInfo.CreatedBy)
                ViewBag.Creater = false;
            ViewBag.CanDownload = (exportInfo.Status == ExportInfoStatus.Sent) && ViewBag.Creater;
            return View();
        }

        [CLIUrlAuthorizeAttribute(Account = Authority.Index, PageId = PagesModel.Export, Anonymity = Anonymous.Verified)]
        public ActionResult File_Export(string id, bool export = true)
        {
            int exportId = int.Parse(_encrypt.Decrypt(id));
            ExportInfoEntity exportInfo = _exportBussiness.GetExportInfosById(exportId);
            if (exportInfo == null)
                throw new HttpException(404, ResourceHelper.GetRM().GetInformation("Report_Queue_Report_404"));
            if (UserInfo.ID != exportInfo.CreatedBy)
                return RedirectToAction("DowloadFile", "Export", new { id = id });
            string fileName = exportInfo.FileUrl;
            string outPutFilename = exportInfo.UpdatedOn.ToString("yyyyMMdd_HHmmss") + ".zip";
            var localFile = FileHelper.HasProtectedFile(fileName);
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
        #endregion
    }
}