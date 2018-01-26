using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Cache;
using System.Text;
using System.Threading.Tasks;
using Sunnet.Cli.Core.Reports;
using Sunnet.Cli.Core.Reports.Entities;
using Sunnet.Cli.Core.Users.Entities;
using Sunnet.Framework.Core.Tool;
using Sunnet.Cli.Core.Common.Enums;
using Sunnet.Framework.Extensions;
using LinqKit;
using Sunnet.Cli.Business.Export.Models;
using Sunnet.Cli.Core.Users.Enums;
using System.Linq.Expressions;
using Sunnet.Cli.Core.Export.Enums;

namespace Sunnet.Cli.Business.Reports
{
    public partial class ReportBusiness
    {

        /// <summary>
        /// 提交报表请求
        /// </summary>
        /// <param name="title">标题(发送邮件时显示).</param>
        /// <param name="queryParams">参数(要执行方法的参数, JSON数组).</param>
        /// <param name="downloadUrl">下载链接({ID} 表示Report  ID 的占位符, 加密).</param>
        /// <param name="user">The user.</param>
        /// <returns></returns>
        public OperationResult SubmitReport(string title, ReportQueueType type, string queryParams, string downloadUrl, UserBaseEntity user)
        {
            var reportQueueEntity = _reportService.NewReportQueueEntity();
            reportQueueEntity.Title = title;
            reportQueueEntity.QueryParams = queryParams;
            reportQueueEntity.DownloadUrl = downloadUrl;
            reportQueueEntity.CreatedBy = user.ID;
            reportQueueEntity.CreatedOn = DateTime.Now;
            reportQueueEntity.Status = ReportQueueStatus.Saved;
            reportQueueEntity.Type = type;
            reportQueueEntity.UpdatedOn = DateTime.Now;

            return _reportService.InsertReportQueue(reportQueueEntity);
        }

        public List<ReportQueueEntity> GetReportQueues(IEnumerable<ReportQueueType> types)
        {
            //CIRCLE_Data_Export类型的报表按照Schedule执行，只执行今天的
            DateTime todayStart = DateTime.Now.Date;
            DateTime todayEnd = DateTime.Now.AddDays(1).Date.AddSeconds(-1);
            // 新增的记录或者上一次处理过程中出错了中断的记录
            return _reportService.ReportQueues.Where(x =>
                types.Contains(x.Type)
                && (x.Status == ReportQueueStatus.Saved
                || x.Status == ReportQueueStatus.Processing)
                && (x.Type == ReportQueueType.CIRCLE_Data_Export ?
            (x.ExcuteTime >= todayStart && x.ExcuteTime <= todayEnd) : 1 == 1)).ToList();
        }

        public ReportQueueEntity GetReportQueue(int id)
        {
            return _reportService.GetReportQueue(id);
        }

        public OperationResult UpdateReportStatus(int id, ReportQueueStatus status, string reportFile = "")
        {
            var report = GetReportQueue(id);
            if (report == null)
                return new OperationResult(OperationResultType.Error, "Report is not found.");
            report.Status = status;
            report.UpdatedOn = DateTime.Now;
            if (!string.IsNullOrEmpty(reportFile))
            {
                report.Report = reportFile;
            }
            return _reportService.UpdateReportQueue(report);
        }

        public OperationResult UpdateReport(ReportQueueEntity entity)
        {
            var report = GetReportQueue(entity.ID);
            if (report == null)
                return new OperationResult(OperationResultType.Error, "Report is not found.");
            report.Result = entity.Result;
            report.Status = entity.Status;
            report.Report = entity.Report;
            report.DownloadUrl = entity.DownloadUrl.Replace("{ID}", _encrypter.Encrypt(entity.ID.ToString()));
            entity.DownloadUrl = report.DownloadUrl;
            report.UpdatedOn = DateTime.Now;
            return _reportService.UpdateReportQueue(report);
        }

        #region Assessment Report Template
        /// <summary>
        /// Check same name template.Allow different Assessment or User have same name template.
        /// </summary>
        /// <param name="templateName"></param>
        /// <param name="assessmentId"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool SearchSameTemps(string templateName, int assessmentId, UserBaseEntity user, int id = -1)
        {
            var assessment = _adeService.GetAssessment(assessmentId);
            List<int> relateAssIds = _adeService.Assessments
                .Where(s => s.Name == assessment.Name).Select(s => s.ID).ToList();
            if (user.Role == Role.Super_admin)
                return _reportService.AssReportTemplates
                    .Count(t => t.ID != id && t.Name == templateName && relateAssIds.Contains(t.AssessmentId)) > 0;
            else
                return _reportService.AssReportTemplates
                    .Count(t => t.ID != id
                        && t.Name == templateName
                        && relateAssIds.Contains(t.AssessmentId)
                        && t.CreatedBy == user.ID) > 0;
        }

        public OperationResult InsertTemplate(UserBaseEntity user, string name, string ids, int assessmentId)
        {
            AssessmentReportTemplateEntity entity = new AssessmentReportTemplateEntity();
            entity.AssessmentId = assessmentId;
            entity.Name = name;
            entity.Status = EntityStatus.Active;
            entity.CreatedBy = user.ID;
            entity.UpdatedBy = user.ID;
            entity.Ids = ids;
            OperationResult result = _reportService.InsertAssReportTemplate(entity);
            result.AppendData = entity.ID + "," + entity.Name;
            return result;
        }

        public List<string> GetAllTemplate(UserBaseEntity user)
        {
            if (user.Role == Role.Super_admin)
                return _reportService.AssReportTemplates.Select(t => t.Ids).ToList();
            else
                return _reportService.AssReportTemplates
                    .Where(t => t.CreatedBy == user.ID).Select(t => t.Ids).ToList();
        }

        public IEnumerable<SelectItemModel> GetAssReportTempsSelectList(UserBaseEntity user, int assessmentId)
        {
            var assessment = _adeService.GetAssessment(assessmentId);
            List<int> relateAssId = _adeService.Assessments
                .Where(s => s.Name == assessment.Name).Select(s => s.ID).ToList();
            return _reportService.AssReportTemplates
                .Where(t => t.Status == EntityStatus.Active)
                .Where(t => t.CreatedBy == user.ID)
                .Where(t => relateAssId.Contains(t.AssessmentId))
                .Select(t => new SelectItemModel()
                {
                    ID = t.ID,
                    Name = t.Name
                });
        }

        public AssessmentReportTemplateEntity GetAssReportTempById(int id)
        {
            return _reportService.GetAssReportTemplate(id);
        }

        public List<ReportTemplateWithUserModel> GetReportTempsSelectListOther
            (int assessmentId, string name, UserBaseEntity user)
        {
            var assessment = _adeService.GetAssessment(assessmentId);
            List<int> relateAssId = _adeService.Assessments
                .Where(s => s.Name == assessment.Name).Select(s => s.ID).ToList();
            return _reportService.AssReportTemplates.AsExpandable()
                .Where(r => r.Name.Contains(name))
                .Where(GetRoleCondition(user))
                .Where(t => relateAssId.Contains(t.AssessmentId))
                .Select(r => new ReportTemplateWithUserModel()
                {
                    ID = r.ID,
                    Name = r.Name,
                    Status = r.Status,
                    CreatedBy = r.CreatedBy,
                    CreatedOn = r.CreatedOn
                }).ToList();
        }
        private Expression<Func<AssessmentReportTemplateEntity, bool>> GetRoleCondition(UserBaseEntity userInfo)
        {
            Expression<Func<AssessmentReportTemplateEntity, bool>> condition = o => true;
            if (userInfo.Role != Role.Super_admin) //管理员可看所有的，其他角色只能看自己创建的
                condition = PredicateBuilder.And(condition, r => r.CreatedBy == userInfo.ID);
            return condition;
        }

        public OperationResult UpdateAssReportTemp(AssessmentReportTemplateEntity entity)
        {
            return _reportService.UpdateAssReportTemplate(entity);
        }

        public OperationResult DeleteAssReportTemp(int id)
        {
            return _reportService.DeleteAssReportTemplate(id);
        }

        public OperationResult SubmitReportList
            (string title, ReportQueueType type, string queryParams, string downloadUrl, UserBaseEntity user, List<DateTime> dates,
            ReceiveFileBy ReceiveFileBy, string SFTPHostIp, int SFTPPort, string SFTPFilePath,
            string SFTPUserName, string SFTPPassword, ExportFileType FileType)
        {

            List<ReportQueueEntity> entityList = new List<ReportQueueEntity>();
            foreach (DateTime date in dates)
            {
                var reportQueueEntity = _reportService.NewReportQueueEntity();
                reportQueueEntity.Title = title;
                reportQueueEntity.QueryParams = queryParams;
                reportQueueEntity.DownloadUrl = downloadUrl;
                reportQueueEntity.CreatedBy = user.ID;
                reportQueueEntity.CreatedOn = DateTime.Now;
                reportQueueEntity.Status = ReportQueueStatus.Saved;
                reportQueueEntity.Type = type;
                reportQueueEntity.UpdatedOn = DateTime.Now;
                reportQueueEntity.ExcuteTime = date;
                reportQueueEntity.ReceiveFileBy = ReceiveFileBy;
                reportQueueEntity.SFTPHostIp = SFTPHostIp;
                reportQueueEntity.SFTPPort = SFTPPort;
                reportQueueEntity.SFTPFilePath = SFTPFilePath;
                reportQueueEntity.SFTPUserName = SFTPUserName;
                reportQueueEntity.SFTPPassword = SFTPPassword;
                reportQueueEntity.FileType = FileType;
                entityList.Add(reportQueueEntity);
            }

            return _reportService.InsertReportQueueList(entityList);
        }
        #endregion
    }
}
