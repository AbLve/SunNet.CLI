using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using StructureMap;
using Sunnet.Cli.Business.Ade;
using Sunnet.Cli.Business.Common;
using Sunnet.Cli.Business.Cpalls;
using Sunnet.Cli.Business.Reports;
using Sunnet.Cli.Business.Users;
using Sunnet.Cli.Core;
using Sunnet.Cli.Core.Ade;
using Sunnet.Cli.Core.Cpalls;
using Sunnet.Cli.Core.Export.Enums;
using Sunnet.Cli.Core.Reports;
using Sunnet.Cli.Core.Reports.Entities;
using Sunnet.Cli.Core.Users.Entities;
using Sunnet.Framework;
using Sunnet.Framework.Core.Tool;
using Sunnet.Framework.EmailSender;
using Sunnet.Framework.Encrypt;
using Sunnet.Framework.Log;
using Sunnet.Framework.SFTP;

namespace ReportQueue
{
    internal delegate void QueueEventHandler();



    internal class QueueManager
    {
        private static EFUnitOfWorkContext RWContext
        {
            get
            {
                return new EFUnitOfWorkContext();
            }
        }

        private static EFReadonlyContext ReadContext
        {
            get
            {
                return new EFReadonlyContext();
            }
        }

        private static AdeReadonlyContext AdeReadContext
        {
            get
            {
                return new AdeReadonlyContext();
            }
        }

        private static AdeUnitOfWorkContext AdeRWContext
        {
            get
            {
                return new AdeUnitOfWorkContext();
            }
        }

        private ReportBusiness reportBusiness;
        private ReportBusiness readReportBusiness;
        private UserBusiness userBusiness;
        private IEncrypt encrypter;
        private IEmailSender emailSender;
        internal event QueueEventHandler BeforeProcessQueues;
        internal event QueueEventHandler AfterProcessQueues;

        internal QueueManager()
        {
            reportBusiness = new ReportBusiness(RWContext);
            readReportBusiness = new ReportBusiness(ReadContext);

            userBusiness = new UserBusiness(ReadContext);
            encrypter = ObjectFactory.GetInstance<IEncrypt>();
            emailSender = ObjectFactory.GetInstance<IEmailSender>();
        }

        private void ProcessSummaryWithAverge_Community(ReportQueueEntity queue)
        {
            var processor = new CpallsBusiness(AdeReadContext);
            var queryParams = JsonConvert.DeserializeObject<Dictionary<string, object>>(queue.QueryParams);
            var assessmentId = 0;
            int.TryParse(queryParams["assessmentId"].ToString(), out assessmentId);
            var language = 0;
            int.TryParse(queryParams["language"].ToString(), out language);
            int year;
            int.TryParse(queryParams["year"].ToString(), out year);
            var districtId = 0;
            int.TryParse(queryParams["districtId"].ToString(), out districtId);
            var wavesStr = queryParams["waves"].ToString();
            var waves = JsonConvert.DeserializeObject<Dictionary<Wave, IEnumerable<int>>>(wavesStr);
            DateTime startDate, endDate;
            if (!DateTime.TryParse(queryParams["startDate"].ToString(), out startDate))
                startDate = CommonAgent.MinDate;
            if (!DateTime.TryParse(queryParams["endDate"].ToString(), out endDate))
                endDate = DateTime.Now.AddDays(1).Date;
            var dobStartDate = queryParams.Keys.Contains("dobStartDate") ? DateTime.Parse(queryParams["dobStartDate"].ToString()) : CommonAgent.MinDate;
            var dobEndDate = queryParams.Keys.Contains("dobEndDate") ? DateTime.Parse(queryParams["dobEndDate"].ToString()) : DateTime.Now;

            var user = userBusiness.GetUser(queue.CreatedBy);
            var result = processor.GetSchoolSummaryReport(assessmentId, user, year, districtId, waves, startDate, endDate, dobStartDate, dobEndDate);
            queue.Result = JsonConvert.SerializeObject(result.ToDictionary(x => Convert.ToByte(x.Key), x => x.Value));
            queue.Status = ReportQueueStatus.Processed;
            var operation = reportBusiness.UpdateReport(queue);
            if (operation.ResultType == OperationResultType.Success)
            {
                SendEmail(user, queue);
                Config.Instance.Logger.Info("Process report {0} successful.", queue.ID);
            }
            else
            {
                Config.Instance.Logger.Debug("Process report {0} failed. message: {1}", queue.ID, operation.Message);
            }
        }
        private void ProcessCommunity_CustomScoreReport(ReportQueueEntity queue)
        {
            var processor = new CpallsBusiness(AdeReadContext);
            var queryParams = JsonConvert.DeserializeObject<Dictionary<string, object>>(queue.QueryParams);
            var assessmentId = 0;
            int.TryParse(queryParams["assessmentId"].ToString(), out assessmentId);
            var language = 0;
            int.TryParse(queryParams["language"].ToString(), out language);
            int year;
            int.TryParse(queryParams["year"].ToString(), out year);
            var districtId = 0;
            int.TryParse(queryParams["districtId"].ToString(), out districtId);
            int wave = 0;
            int.TryParse(queryParams["wave"].ToString(), out wave);
            //var waves = JsonConvert.DeserializeObject<Dictionary<Wave, IEnumerable<int>>>(wavesStr);
            var scoreIds =
                JsonConvert.DeserializeObject<List<int>>(queryParams["scoreIds"].ToString());
            DateTime startDate, endDate;
            if (!DateTime.TryParse(queryParams["startDate"].ToString(), out startDate))
                startDate = CommonAgent.MinDate;
            if (!DateTime.TryParse(queryParams["endDate"].ToString(), out endDate))
                endDate = DateTime.Now.AddDays(1).Date;
            var dobStartDate = queryParams.Keys.Contains("dobStartDate") ? DateTime.Parse(queryParams["dobStartDate"].ToString()) : CommonAgent.MinDate;
            var dobEndDate = queryParams.Keys.Contains("dobEndDate") ? DateTime.Parse(queryParams["dobEndDate"].ToString()) : DateTime.Now;

            var user = userBusiness.GetUser(queue.CreatedBy);
            var result = processor.GeCommunityScoreReportPdf(assessmentId, (StudentAssessmentLanguage)language, user, (Wave)wave, year, districtId, scoreIds, startDate, endDate, dobStartDate, dobEndDate);
            queue.Result = JsonConvert.SerializeObject(result);
            queue.Status = ReportQueueStatus.Processed;
            var operation = reportBusiness.UpdateReport(queue);
            if (operation.ResultType == OperationResultType.Success)
            {
                SendEmail(user, queue);
                Config.Instance.Logger.Info("Process report {0} successful.", queue.ID);
            }
            else
            {
                Config.Instance.Logger.Debug("Process report {0} failed. message: {1}", queue.ID, operation.Message);
            }
        }

        private void ProcessSummaryWithAvergePercentileRank_Community(ReportQueueEntity queue)
        {
            var processor = new CpallsBusiness(AdeReadContext);
            var queryParams = JsonConvert.DeserializeObject<Dictionary<string, object>>(queue.QueryParams);
            var assessmentId = 0;
            int.TryParse(queryParams["assessmentId"].ToString(), out assessmentId);
            var language = 0;
            int.TryParse(queryParams["language"].ToString(), out language);
            int year;
            int.TryParse(queryParams["year"].ToString(), out year);
            var districtId = 0;
            int.TryParse(queryParams["districtId"].ToString(), out districtId);
            var wavesStr = queryParams["waves"].ToString();
            var waves = JsonConvert.DeserializeObject<Dictionary<Wave, IEnumerable<int>>>(wavesStr);
            DateTime startDate, endDate;
            if (!DateTime.TryParse(queryParams["startDate"].ToString(), out startDate))
                startDate = CommonAgent.MinDate;
            if (!DateTime.TryParse(queryParams["endDate"].ToString(), out endDate))
                endDate = DateTime.Now.AddDays(1).Date;
            var dobStartDate = queryParams.Keys.Contains("dobStartDate") ? DateTime.Parse(queryParams["dobStartDate"].ToString()) : CommonAgent.MinDate;
            var dobEndDate = queryParams.Keys.Contains("dobEndDate") ? DateTime.Parse(queryParams["dobEndDate"].ToString()) : DateTime.Now;

            var user = userBusiness.GetUser(queue.CreatedBy);
            var result = processor.GetSchoolSummaryPercentileRankReport(assessmentId, user, year, districtId, waves, startDate, endDate, dobStartDate, dobEndDate);
            queue.Result = JsonConvert.SerializeObject(result.ToDictionary(x => Convert.ToByte(x.Key), x => x.Value));
            queue.Status = ReportQueueStatus.Processed;
            var operation = reportBusiness.UpdateReport(queue);
            if (operation.ResultType == OperationResultType.Success)
            {
                SendEmail(user, queue);
                Config.Instance.Logger.Info("Process report {0} successful.", queue.ID);
            }
            else
            {
                Config.Instance.Logger.Debug("Process report {0} failed. message: {1}", queue.ID, operation.Message);
            }
        }

        private void ProcessPercentage_Community(ReportQueueEntity queue)
        {
            var processor = new CpallsBusiness(AdeReadContext);
            var queryParams = JsonConvert.DeserializeObject<Dictionary<string, object>>(queue.QueryParams);
            var assessmentId = 0;
            int.TryParse(queryParams["assessmentId"].ToString(), out assessmentId);
            var language = 0;
            int.TryParse(queryParams["language"].ToString(), out language);
            var year = DateTime.Now.Year;
            int.TryParse(queryParams["year"].ToString(), out year);
            var districtId = 0;
            int.TryParse(queryParams["districtId"].ToString(), out districtId);
            var wavesStr = queryParams["waves"].ToString();
            var waves = JsonConvert.DeserializeObject<Dictionary<Wave, IEnumerable<int>>>(wavesStr);
            DateTime startDate, endDate;
            if (!DateTime.TryParse(queryParams["startDate"].ToString(), out startDate))
                startDate = CommonAgent.MinDate;
            if (!DateTime.TryParse(queryParams["endDate"].ToString(), out endDate))
                endDate = DateTime.Now.AddDays(1).Date;
            var dobStartDate = queryParams.Keys.Contains("dobStartDate") ? DateTime.Parse(queryParams["dobStartDate"].ToString()) : CommonAgent.MinDate;
            var dobEndDate = queryParams.Keys.Contains("dobEndDate") ? DateTime.Parse(queryParams["dobEndDate"].ToString()) : DateTime.Now;

            var user = userBusiness.GetUser(queue.CreatedBy);
            var result = processor.GetSchoolSatisfactoryReport(assessmentId, user, year, districtId, waves, startDate, endDate, dobStartDate, dobEndDate, false);
            var mergeLabelResult = processor.GetSchoolSatisfactoryReport(assessmentId, user, year, districtId, waves, startDate, endDate, dobStartDate, dobEndDate, true);
            queue.Result = JsonConvert.SerializeObject(result.ToDictionary(x => Convert.ToByte(x.Key), x => x.Value));
            queue.MergeLabelResult = JsonConvert.SerializeObject(mergeLabelResult.ToDictionary(x => Convert.ToByte(x.Key), x => x.Value));
            queue.Status = ReportQueueStatus.Processed;
            var operation = reportBusiness.UpdateReport(queue);
            if (operation.ResultType == OperationResultType.Success)
            {
                SendEmail(user, queue);
                Config.Instance.Logger.Info("Process report {0} successful.", queue.ID);
            }
            else
            {
                Config.Instance.Logger.Debug("Process report {0} failed. message: {1}", queue.ID, operation.Message);
            }
        }

        private void Completion_Community(ReportQueueEntity queue)
        {
            var processor = new CpallsBusiness(AdeReadContext);
            var queryParams = JsonConvert.DeserializeObject<Dictionary<string, object>>(queue.QueryParams);
            var assessmentId = 0;
            int.TryParse(queryParams["assessmentId"].ToString(), out assessmentId);
            var language = 0;
            int.TryParse(queryParams["language"].ToString(), out language);
            var year = DateTime.Now.Year;
            int.TryParse(queryParams["year"].ToString(), out year);
            var communityId = 0;
            int.TryParse(queryParams["communityId"].ToString(), out communityId);

            var wavesStr = queryParams["waves"].ToString();
            var waves = JsonConvert.DeserializeObject<Dictionary<Wave, IEnumerable<int>>>(wavesStr);

            var startDate = queryParams.Keys.Contains("startDate") ? DateTime.Parse(queryParams["startDate"].ToString()) : StartDate;
            var endDate = queryParams.Keys.Contains("endDate") ? DateTime.Parse(queryParams["endDate"].ToString()).AddDays(1) : DateTime.Now.AddDays(1).Date;
            var dobStartDate = queryParams.Keys.Contains("dobStartDate") ? DateTime.Parse(queryParams["dobStartDate"].ToString()): CommonAgent.MinDate;
            var dobEndDate = queryParams.Keys.Contains("dobEndDate") ? DateTime.Parse(queryParams["dobEndDate"].ToString()) : DateTime.Now;

            var user = userBusiness.GetUser(queue.CreatedBy);

            var report = new ReportList();

            foreach (var v in waves)
            {
                if (v.Value.Any())
                {
                    report = processor.GetReport_Community(assessmentId, year,
                  v.Value.ToList(), v.Key, (StudentAssessmentLanguage)language, communityId, startDate, endDate, dobStartDate, dobEndDate);
                    break;
                }
            }

            queue.Result = JsonConvert.SerializeObject(report);
            queue.Status = ReportQueueStatus.Processed;
            var operation = reportBusiness.UpdateReport(queue);
            if (operation.ResultType == OperationResultType.Success)
            {
                SendEmail(user, queue);
                Config.Instance.Logger.Info("Process report {0} successful.", queue.ID);
            }
            else
            {
                Config.Instance.Logger.Debug("Process report {0} failed. message: {1}", queue.ID, operation.Message);
            }
        }

        private void ProcessCircleDataExport(ReportQueueEntity queue)
        {
            var user = userBusiness.GetUser(queue.CreatedBy);

            if (queue.ReceiveFileBy == ReceiveFileBy.SFTP)
            {
                bool connet = TestSFTPConnet(queue);
                if (!connet)
                {
                    SFTPFailEmail(user, queue);
                    queue.Status = ReportQueueStatus.SftpError;
                    reportBusiness.UpdateReport(queue);
                    Config.Instance.Logger.Info(string.Format("Connect to sftp failed. This time haven't execute the query. ID:{0}", queue.ID));
                    return;
                }
            }
            var queryParams = JsonConvert.DeserializeObject<Dictionary<string, object>>(queue.QueryParams);

            var assessmentId = int.Parse(queryParams["assessmentId"].ToString());
            var communityId = int.Parse(queryParams["communityId"].ToString());
            var year = int.Parse(queryParams["year"].ToString());
            var waves = JsonConvert.DeserializeObject<List<int>>(queryParams["waves"].ToString())
                .Select(x => (Wave)x).ToList();
            var scoreIds =
                JsonConvert.DeserializeObject<List<int>>(queryParams["customScores"].ToString());
            var englishResults =
                JsonConvert.DeserializeObject<List<int>>(queryParams["englishResults"].ToString());
            var englishItemLevel =
                JsonConvert.DeserializeObject<List<int>>(queryParams["englishItemLevel"].ToString());
            var spanishResults =
                JsonConvert.DeserializeObject<List<int>>(queryParams["spanishResults"].ToString());
            var spanishItemLevel =
                JsonConvert.DeserializeObject<List<int>>(queryParams["spanishItemLevel"].ToString());

            var schoolId = int.Parse(queryParams["schoolId"].ToString());

            var startDate = queryParams.Keys.Contains("startDate") ? DateTime.Parse(queryParams["startDate"].ToString()) : StartDate;
            var endDate = queryParams.Keys.Contains("endDate") ? DateTime.Parse(queryParams["endDate"].ToString()).AddDays(1).Date : DateTime.Now.AddDays(1).Date;

            var dobStartDate = queryParams.Keys.Contains("dobStartDate") ? DateTime.Parse(queryParams["dobStartDate"].ToString()) : CommonAgent.MinDate;
            var dobEndDate = queryParams.Keys.Contains("dobEndDate") ? DateTime.Parse(queryParams["dobEndDate"].ToString()) : DateTime.Now;

            var measures = englishResults.Union(spanishResults).Distinct().ToList();
            var measuresIncludeItems = englishItemLevel.Union(spanishItemLevel).Distinct().ToList();
            var students = readReportBusiness.GetCircleDataExport(communityId, year, schoolId,
                waves, measures, measuresIncludeItems, startDate, endDate, dobStartDate, dobEndDate, user);
            // queue.Result = JsonConvert.SerializeObject(students);
            queue.Result = string.Empty;
            queue.Status = ReportQueueStatus.Processed;
            queue.UpdatedOn = DateTime.Now;
            var exportCsv = true;
            string filename = "Community/" + communityId + "/CircleDataExport_" +
                              queue.UpdatedOn.ToString("yyyy_MM_dd_HH_mm_ss_fff") +
                              encrypter.Encrypt(queue.CreatedBy.ToString()).Substring(0, 4);
            filename += exportCsv ? ".zip" : ".xlsx";

            var localFile = Path.Combine(SFConfig.ProtectedFiles, filename);
            string directory = Path.Combine(SFConfig.ProtectedFiles, "Community", communityId.ToString());
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
            var _adeBusiness = new AdeBusiness(AdeReadContext);
            var scoreReportModels = _adeBusiness.GetStudentResultReportFinalResult(students.Select(e => e.ID).ToList(), waves, year.ToSchoolYearString(), scoreIds, startDate, endDate);
            var reportGeneratedSuccess =
                readReportBusiness.GenerateCircleDataExportCsv(localFile, students, measures, measuresIncludeItems, year.ToSchoolYearString(), waves, queue.FileType, scoreIds, scoreReportModels);
            if (reportGeneratedSuccess)
            {
                queue.Report = filename;
            }

            var operation = reportBusiness.UpdateReport(queue);
            if (operation.ResultType == OperationResultType.Success)
            {
                switch (queue.ReceiveFileBy)
                {
                    case ReceiveFileBy.DownloadLink:
                        SendEmail(user, queue);
                        break;
                    case ReceiveFileBy.SFTP:
                        PutToSFTP(user, queue);
                        break;
                }
                Config.Instance.Logger.Info("Process report {0} successful.", queue.ID);
            }
            else
            {
                Config.Instance.Logger.Debug("Process report {0} failed. message: {1}", queue.ID, operation.Message);
            }
        }

        private void ProcessSchool_CustomScoreReport(ReportQueueEntity queue)
        {
            var processor = new CpallsBusiness(AdeReadContext);
            var queryParams = JsonConvert.DeserializeObject<Dictionary<string, object>>(queue.QueryParams);
            var assessmentId = 0;
            int.TryParse(queryParams["assessmentId"].ToString(), out assessmentId);
            var language = 0;
            int.TryParse(queryParams["language"].ToString(), out language);
            int year;
            int.TryParse(queryParams["year"].ToString(), out year);
            var schoolId = 0;
            int.TryParse(queryParams["schoolId"].ToString(), out schoolId);
            int wave = 0;
            int.TryParse(queryParams["wave"].ToString(), out wave);
            //var waves = JsonConvert.DeserializeObject<Dictionary<Wave, IEnumerable<int>>>(wavesStr);
            var scoreIds =
                JsonConvert.DeserializeObject<List<int>>(queryParams["scoreIds"].ToString());
            DateTime startDate, endDate;
            if (!DateTime.TryParse(queryParams["startDate"].ToString(), out startDate))
                startDate = CommonAgent.MinDate;
            if (!DateTime.TryParse(queryParams["endDate"].ToString(), out endDate))
                endDate = DateTime.Now.AddDays(1).Date;
            var dobStartDate = queryParams.Keys.Contains("dobStartDate") ? DateTime.Parse(queryParams["dobStartDate"].ToString()) : CommonAgent.MinDate;
            var dobEndDate = queryParams.Keys.Contains("dobEndDate") ? DateTime.Parse(queryParams["dobEndDate"].ToString()) : DateTime.Now;

            var user = userBusiness.GetUser(queue.CreatedBy);
            var result = processor.GetSchoolScoreReportPdf(assessmentId, (StudentAssessmentLanguage)language, user, (Wave)wave, year, schoolId, scoreIds, startDate, endDate, dobStartDate, dobEndDate);
            queue.Result = JsonConvert.SerializeObject(result);
            queue.Status = ReportQueueStatus.Processed;
            var operation = reportBusiness.UpdateReport(queue);
            if (operation.ResultType == OperationResultType.Success)
            {
                SendEmail(user, queue);
                Config.Instance.Logger.Info("Process report {0} successful.", queue.ID);
            }
            else
            {
                Config.Instance.Logger.Debug("Process report {0} failed. message: {1}", queue.ID, operation.Message);
            }
        }

        private void SendEmail(UserBaseEntity user, ReportQueueEntity queue)
        {
            if (queue.Status == ReportQueueStatus.Processed)
            {
                var emailTemplate = Helper.GetReportTemplete();
                var subject = emailTemplate.Subject.Replace("{Title}", queue.Title);
                string body = string.Empty;
                if (queue.Type == ReportQueueType.CIRCLE_Data_Export)
                {
                    body = emailTemplate.Body.Replace("{Title}", queue.Title).Replace("{FirstName}", user.FirstName)
                    .Replace("{Link}",
                        string.Format("{0}{1}", Config.Instance.MainSiteDomain,
                            queue.DownloadUrl).Replace("//Report", "/Report"));
                }
                else
                {
                    body = emailTemplate.Body.Replace("{Title}", queue.Title).Replace("{FirstName}", user.FirstName)
                    .Replace("{Link}",
                        string.Format("{0}{1}", Config.Instance.AssessmentDomain,
                            queue.DownloadUrl).Replace("//Report", "/Report"));
                }

                var localFile = Path.Combine(SFConfig.ProtectedFiles, queue.Report);
                emailSender.SendMail(user.PrimaryEmailAddress, subject, body);
                queue.Status = ReportQueueStatus.Sent;
                reportBusiness.UpdateReport(queue);
            }
        }

        #region SFTP
        private bool TestSFTPConnet(ReportQueueEntity queue)
        {
            ISunnetLog LoggerHelper = ObjectFactory.GetInstance<ISunnetLog>();
            IEncrypt encrypt = ObjectFactory.GetInstance<IEncrypt>();
            SFTPHelper sfptHelper = new SFTPHelper
                (queue.SFTPHostIp, queue.SFTPPort, queue.SFTPUserName, encrypt.Decrypt(queue.SFTPPassword), LoggerHelper);
            bool bconn = sfptHelper.Connect();
            //连接失败
            if (bconn)
                sfptHelper.Disconnect();
            return bconn;
        }

        private void SFTPFailEmail(UserBaseEntity user, ReportQueueEntity queue)
        {
            var emailTemplate = Helper.GetReportTemplete("SFTPConnctFail.xml");
            var subject = emailTemplate.Subject.Replace("{Title}", queue.Title);
            string body = string.Empty;

            body = emailTemplate.Body.Replace("{Title}", queue.Title).Replace("{FirstName}", user.FirstName)
            .Replace("{SFTPHostIp}", queue.SFTPHostIp).Replace("{SFTPPort}", queue.SFTPPort.ToString())
            .Replace("{SFTPUserName}", queue.SFTPUserName);
            emailSender.SendMail(user.PrimaryEmailAddress, subject, body);
        }
        private void PutToSFTP(UserBaseEntity user, ReportQueueEntity queue)
        {
            ISunnetLog LoggerHelper = ObjectFactory.GetInstance<ISunnetLog>();

            IEncrypt encrypt = ObjectFactory.GetInstance<IEncrypt>();
            SFTPHelper sfptHelper = new SFTPHelper
                (queue.SFTPHostIp, queue.SFTPPort, queue.SFTPUserName, encrypt.Decrypt(queue.SFTPPassword), LoggerHelper);
            try
            {
                string protectedFilePath = SFConfig.ProtectedFiles;
                string remoteDir = queue.SFTPFilePath;
                bool bconn = sfptHelper.Connect();
                //连接失败
                if (!bconn)
                {
                    SFTPFailEmail(user, queue);
                    queue.Status = ReportQueueStatus.SftpError;
                    reportBusiness.UpdateReport(queue);
                    Config.Instance.Logger.Info(string.Format("-->Connect to sftp failed.This time already execute the query. ID:{0}", queue.ID));
                    return;
                }
                if (!sfptHelper.DirExist(remoteDir))
                    sfptHelper.MakeDir(remoteDir);
                var reportArr = queue.Report.Split('/');
                string fileName = reportArr[reportArr.Length - 1];
                string remoteFile = remoteDir.TrimEnd('/') + "/" + fileName;
                string localFile = protectedFilePath + "/" + queue.Report;
                if (bconn)
                {
                    bool putFile = sfptHelper.Put(localFile, remoteFile);
                    if (putFile)
                    {
                        Config.Instance.Logger.Info("-->Put files to sftp success");
                        queue.Status = ReportQueueStatus.Sent;
                        reportBusiness.UpdateReport(queue);
                        //发邮件通知
                        var emailTemplate = Helper.GetReportTemplete("UploadToSFTPSuccess.xml");
                        var subject = emailTemplate.Subject.Replace("{Title}", queue.Title);
                        string body = string.Empty;
                        body = emailTemplate.Body.Replace("{Title}", queue.Title).Replace("{FirstName}", user.FirstName)
                        .Replace("{SFTPHostIp}", queue.SFTPHostIp);
                        emailSender.SendMail(user.PrimaryEmailAddress, subject, body);
                    }
                    else
                    {
                        Config.Instance.Logger.Info(string.Format("-->Put file to sftp failed. ID:{0}", queue.ID));
                        queue.Status = ReportQueueStatus.SftpError;
                        reportBusiness.UpdateReport(queue);
                    }
                    sfptHelper.Disconnect();
                }
            }
            catch (Exception ex)
            {
                sfptHelper.Disconnect();
                queue.Status = ReportQueueStatus.SftpError;
                reportBusiness.UpdateReport(queue);
                LoggerHelper.Debug(ex);
            }
        }

        #endregion

        internal void Start()
        {
            var types = Config.Instance.ProcessTypes.Select(t => (ReportQueueType)t).ToList();
            //Config.Instance.Logger.Info("Searching...");
            var queues = reportBusiness.GetReportQueues(types);
            if (queues != null && queues.Any())
            {
                if (BeforeProcessQueues != null) BeforeProcessQueues();
                Config.Instance.Logger.Info("Found {0} reports", queues.Count);
                foreach (var queue in queues)
                {
                    Config.Instance.Logger.Info("Start process queue: {0}", queue.ID);
                    reportBusiness.UpdateReportStatus(queue.ID, ReportQueueStatus.Processing);
                    try
                    {
                        switch (queue.Type)
                        {
                            case ReportQueueType.SummaryWithAverge_Community:
                                ProcessSummaryWithAverge_Community(queue);
                                break;
                            case ReportQueueType.SummaryWithAvergePercentileRank_Community:
                                ProcessSummaryWithAvergePercentileRank_Community(queue);
                                break;
                            case ReportQueueType.Percentage_Community:
                                ProcessPercentage_Community(queue);
                                break;
                            case ReportQueueType.Community_Completion_Report:
                                Completion_Community(queue);
                                break;
                            case ReportQueueType.CIRCLE_Data_Export:
                                ProcessCircleDataExport(queue);
                                break;
                            case ReportQueueType.Community_CustomScoreReport:
                                ProcessCommunity_CustomScoreReport(queue);
                                break;
                            case ReportQueueType.School_CustomScoreReport:
                                ProcessSchool_CustomScoreReport(queue);
                                break;
                            default:
                                break;
                        }
                    }
                    catch (Exception e)
                    {
                        reportBusiness.UpdateReportStatus(queue.ID, ReportQueueStatus.ProcessError);
                        Config.Instance.Logger.Debug("Exception throw during process report {0}. message: {1}, stacktrace: {2}", queue.ID, e.Message, e.StackTrace);

                        emailSender.SendMail("david@sunnet.us", "CLI Report Queue error", "Exception throw during process report " + queue.ID + ". message: " + e.Message + ", stacktrace: " + e.StackTrace);
                    }
                    Config.Instance.Logger.Info("Process queue: {0} over.", queue.ID);
                }

                if (AfterProcessQueues != null) AfterProcessQueues();
            }
        }

        private DateTime StartDate
        {
            get
            {
                 return new DateTime(CommonAgent.Year, 8, 1);
               // return new DateTime(CommonAgent.Year, CommonAgent.YearSeparate, 1);
            }
        }

    }
}
