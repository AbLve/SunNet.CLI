using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Sunnet.Cli.Core.Trs.Entities;
using Sunnet.Framework.Core.Tool;
using LinqKit;
using Sunnet.Cli.Business.Trs.Models;
using Sunnet.Cli.Core.Schools.Entities;
using Sunnet.Cli.Core.Users.Entities;
using Sunnet.Cli.Core.Users.Enums;
using Sunnet.Cli.Core.Common.Enums;
using Sunnet.Cli.Core.Communities.Entities;
using Sunnet.Framework.Helpers;
using Sunnet.Framework;
using StructureMap;
using Sunnet.Framework.EmailSender;
using Sunnet.Cli.Core.Trs.Enums;
using Sunnet.Cli.Core.Trs;
using Sunnet.Cli.Business.Common;
using Sunnet.Cli.Core.Schools.Enums;

namespace Sunnet.Cli.Business.Trs
{
    public partial class TrsBusiness
    {
        public TRSEventLogEntity GetEventLogById(int id)
        {
            return _trsContract.GetEventLogById(id);
        }

        public List<TRSEventLogEntity> GetEventLogList(Expression<Func<TRSEventLogEntity, bool>> condition)
        {
            return _trsContract.TRSEventLogs.AsExpandable().Where(condition).OrderByDescending(x => x.CreatedOn).ToList();
        }

        public List<TRSEventLogModel> GetEventLogModelList(Expression<Func<TRSEventLogEntity, bool>> condition)
        {
            List<TRSEventLogModel> eventLogModels = _trsContract.TRSEventLogs.AsExpandable()
                .Where(condition).Select(TrsEventLogToModel).OrderByDescending(x => x.CreatedOn).ToList();
            return eventLogModels;

        }

        private static Expression<Func<TRSEventLogEntity, TRSEventLogModel>> TrsEventLogToModel
        {
            get
            {
                return r => new TRSEventLogModel
                {
                    ID = r.ID,
                    SchoolId = r.SchoolId,
                    DateCreated = r.DateCreated,
                    CreatedBy = r.CreatedBy,
                    EventType = r.EventType,
                    Comment = r.Comment,
                    ActionRequired = r.ActionRequired,
                    Notification = r.Notification,
                    Accreditation = r.Accreditation,
                    RelatedId = r.RelatedId,
                    CreatedOn = r.CreatedOn,
                    Documents = r.EventLogFiles.Where(f => f.IsDelete == false).Select(f => new TRSEventLogFileModel
                    {
                        ID = f.ID,
                        EventLogId = f.EventLogId,
                        FileName = f.FileName,
                        FilePath = f.FilePath,
                        FileServerPath = "",
                        IsDelete = f.IsDelete,
                        CreatedBy = f.CreatedBy,
                        IsAllowDel = false
                    })
                };
            }
        }

        public List<TRSEventLogEntity> GetEventLogListBySchooIds(List<int> schoolIds)
        {
            return _trsContract.TRSEventLogs.AsExpandable().Where(x => schoolIds.Contains(x.SchoolId)).ToList();
        }

        public OperationResult InsertEventLog(TRSEventLogEntity entity)
        {
            return _trsContract.InsertEventLog(entity);
        }

        public OperationResult UpdateEventLog(TRSEventLogEntity entity)
        {
            return _trsContract.UpdateEventLog(entity);
        }

        public OperationResult UpdateSchoolAccreditation(int schoolId,DateTime approveDate, TrsAccreditation accreditation, EventLogType eventLogType, TRSStarEnum verifiedStar = TRSStarEnum.Four)
        {
            OperationResult result = new OperationResult(OperationResultType.Success);
            if (accreditation != 0)
            {
                SchoolEntity schoolEntity = SchoolBusiness.GetSchool(schoolId);
                schoolEntity.NAEYC = false;
                schoolEntity.CANASA = false;
                schoolEntity.NECPA = false;
                schoolEntity.NACECCE = false;
                schoolEntity.NAFCC = false;
                schoolEntity.ACSI = false;
                schoolEntity.USMilitary = false;
                schoolEntity.QELS = false;
                switch (accreditation)
                {
                    case TrsAccreditation.NAEYC:
                        schoolEntity.NAEYC = true;
                        break;
                    case TrsAccreditation.COA:
                        schoolEntity.CANASA = true;
                        break;
                    case TrsAccreditation.NECPA:
                        schoolEntity.NECPA = true;
                        break;
                    case TrsAccreditation.NAC:
                        schoolEntity.NACECCE = true;
                        break;
                    case TrsAccreditation.NAFCC:
                        schoolEntity.NAFCC = true;
                        break;
                    case TrsAccreditation.ACSI:
                        schoolEntity.ACSI = true;
                        break;
                    case TrsAccreditation.USMilitary:
                        schoolEntity.USMilitary = true;
                        break;
                    case TrsAccreditation.QELS:
                        schoolEntity.QELS = true;
                        break;
                }
                schoolEntity.VSDesignation = verifiedStar;
                schoolEntity.TrsLastStatusChange = DateTime.Now;
                schoolEntity.RecertificatedBy = CommonAgent.GetTrsReceritificationDate();
                switch (eventLogType)
                {
                    case EventLogType.Auto_Assign:
                        schoolEntity.StarAssessmentType = StarAssessmentType.Auto_Assign;
                        break;
                    case EventLogType.Star_Level_Change:
                        schoolEntity.StarAssessmentType = StarAssessmentType.Star_Level_Change;
                        break;
                }
                //Ticket 2608 Chat 03/03/2017
                //schoolEntity.StarStatus = 0;
                //schoolEntity.StarDate = CommonAgent.MinDate;
                result = SchoolBusiness.UpdateSchool(schoolEntity);
                //插入一个空的TrsAssessment记录
                if (result.ResultType == OperationResultType.Success)
                {
                    InsertRecordOfVerifiedStarChanged(schoolId, verifiedStar, approveDate, CommonAgent.GetTrsReceritificationDate(), eventLogType);
                }
            }
            return result;
        }

        public OperationResult UpdateSchoolVerifiedStar(int schoolId, EventLogType eventLogType,DateTime approvalDate, TRSStarEnum verifiedStar = TRSStarEnum.Four)
        {
            OperationResult result = new OperationResult(OperationResultType.Success);
            SchoolEntity schoolEntity = SchoolBusiness.GetSchool(schoolId);
            schoolEntity.VSDesignation = verifiedStar;
            schoolEntity.TrsLastStatusChange = DateTime.Now;
            schoolEntity.RecertificatedBy = CommonAgent.GetTrsReceritificationDate();
            switch (eventLogType)
            {
                case EventLogType.Auto_Assign:
                    schoolEntity.StarAssessmentType = StarAssessmentType.Auto_Assign;
                    break;
                case EventLogType.Star_Level_Change:
                    schoolEntity.StarAssessmentType = StarAssessmentType.Star_Level_Change;
                    break;
            }
            //Ticket 2608 Chat 03/03/2017
            //schoolEntity.StarStatus = 0;
            //schoolEntity.StarDate = CommonAgent.MinDate;
            result = SchoolBusiness.UpdateSchool(schoolEntity);
            //插入一个空的TrsAssessment记录
            if (result.ResultType == OperationResultType.Success)
            {
                InsertRecordOfVerifiedStarChanged(schoolId, verifiedStar, approvalDate, CommonAgent.GetTrsReceritificationDate(), eventLogType);
            }
            return result;
        }

        public int GetSchoolAccreditation(SchoolEntity schoolEntity)
        {
            int accreditation = 0;
            if (schoolEntity != null)
            {
                if (schoolEntity.NAEYC)
                    accreditation = (int)TrsAccreditation.NAEYC;
                else if (schoolEntity.NECPA)
                    accreditation = (int)TrsAccreditation.NECPA;
                else if (schoolEntity.NAFCC)
                    accreditation = (int)TrsAccreditation.NAFCC;
                else if (schoolEntity.USMilitary)
                    accreditation = (int)TrsAccreditation.USMilitary;
                else if (schoolEntity.CANASA)
                    accreditation = (int)TrsAccreditation.COA;
                else if (schoolEntity.NACECCE)
                    accreditation = (int)TrsAccreditation.NAC;
                else if (schoolEntity.ACSI)
                    accreditation = (int)TrsAccreditation.ACSI;
                else if (schoolEntity.QELS)
                    accreditation = (int)TrsAccreditation.QELS;
            }
            return accreditation;
        }

        public List<NotificationUserModel> GetNotificationUsers(int schoolId)
        {
            SchoolEntity schoolEntity = SchoolBusiness.GetSchool(schoolId);

            List<int> userIdList = new List<int>();
            //Community User
            List<CommunityEntity> communityList = schoolEntity.CommunitySchoolRelations.Select(r => r.Community).ToList();
            foreach (var community in communityList)
            {
                List<int> communityUserIds = community.UserCommunitySchools
                    .Where(r => r.User.Role == Role.Community).Select(r => r.UserId).ToList();
                userIdList.AddRange(communityUserIds);
            }

            //School Assessor
            if (schoolEntity.TrsAssessorId != null)
                userIdList.Add(schoolEntity.TrsAssessorId.Value);

            //TRS Class Mentor / Assessor
            if (schoolEntity.TRSClasses.Any())
            {
                schoolEntity.TRSClasses.Where(x => x.TrsMentorId != null)
                    .ForEach(x => userIdList.Add(x.TrsMentorId.Value));

                schoolEntity.TRSClasses.ForEach(x => userIdList.Add(x.TrsAssessorId));
            }
            List<UserBaseEntity> userBaseList = UserBusiness.GetUsers(userIdList);

            List<NotificationUserModel> users = userBaseList
                .Where(u => u.Status == EntityStatus.Active && u.IsDeleted == false)
                .Select(u => new NotificationUserModel()
                {
                    UserId = u.ID,
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    Role = u.Role
                }).ToList();
            return users;
        }

        public OperationResult InsertNotifications(int eventLogId, int schoolId, int[] userIds)
        {
            OperationResult result = new OperationResult(OperationResultType.Success);
            TRSEventLogEntity entity = GetEventLogById(eventLogId);
            List<TRSNotificationEntity> currentEntities = entity.Notifications.ToList();
            List<TRSNotificationEntity> needDelEntities = currentEntities.Where(x => !userIds.Contains(x.UserId)).ToList();
            if (needDelEntities.Any())
                _trsContract.DeleteNotifications(needDelEntities);

            List<int> existUserIds = currentEntities.Where(x => userIds.Contains(x.UserId)).Select(x => x.UserId).ToList();
            List<int> userIdList = userIds.ToList();
            if (existUserIds.Any())
                existUserIds.ForEach(x => userIdList.Remove(x));

            if (userIdList.Any())
            {
                List<TRSNotificationEntity> needInsertEntities = new List<TRSNotificationEntity>();

                foreach (int userId in userIdList)
                {
                    TRSNotificationEntity notification = new TRSNotificationEntity();
                    notification.UserId = userId;
                    notification.EventLogId = eventLogId;
                    needInsertEntities.Add(notification);
                }
                result = _trsContract.InsertNotifications(needInsertEntities);
            }
            if (result.ResultType == OperationResultType.Success)
            {
                SendNotifyEmail(schoolId, userIds);
            }
            return result;
        }

        public delegate void SendEmailHandler(int schoolId, int[] SelectUserId);
        public void SendNotifyEmail(int schoolId, int[] SelectUserId)
        {
            SendEmailHandler sendDelegate = AsyncSendEmail;
            sendDelegate.BeginInvoke(schoolId, SelectUserId, null, null);
        }
        private void AsyncSendEmail(int schoolId, int[] SelectUserId)
        {
            SchoolEntity school = SchoolBusiness.GetSchool(schoolId);
            string schoolName = school.Name;
            string mainSite = SFConfig.MainSiteDomain;
            EmailTemplete template = new EmailTemplete();
            template = XmlHelper.GetEmailTemp("TrsNotification_Template.xml");
            string subject = template.Subject;

            List<UserBaseEntity> userList = UserBusiness.GetUsers(SelectUserId.ToList());
            var emailSender = ObjectFactory.GetInstance<IEmailSender>();
            foreach (UserBaseEntity user in userList)
            {
                string emailBody = template.Body
                    .Replace("{UserName}", user.FirstName + " " + user.LastName)
                    .Replace("{SchoolName}", schoolName)
                    .Replace("{MainSiteDomain}", mainSite);
                emailSender.SendMail(user.PrimaryEmailAddress, subject, emailBody);
            }
        }

        public OperationResult InsertEventLogFiles(List<TRSEventLogFileEntity> eventLogFiles)
        {
            OperationResult result = new OperationResult(OperationResultType.Success);
            result = _trsContract.InsertEventLogFiles(eventLogFiles);
            return result;
        }

        public OperationResult DeleteEventlogFile(int eventLogFileId, UserBaseEntity userInfo)
        {
            OperationResult result = new OperationResult(OperationResultType.Success);
            TRSEventLogFileEntity entity = _trsContract.GetEventLogFileById(eventLogFileId);
            if (entity != null)
            {
                if (entity.CreatedBy == userInfo.ID)
                {
                    entity.IsDelete = true;
                    entity.UpdatedOn = DateTime.Now;
                    return _trsContract.UpdateEventLogFile(entity);
                }
                else
                {
                    result.ResultType = OperationResultType.PurviewLack;
                    result.Message = "No access permissions !";
                }
            }
            else
            {
                result.ResultType = OperationResultType.Error;
            }
            return result;
        }
    }
}
