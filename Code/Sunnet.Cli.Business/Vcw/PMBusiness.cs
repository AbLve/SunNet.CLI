using System;
using System.Collections.Generic;
using System.Linq;
/**************************************************************************
 * Developer: 		Joe
 * Computer:		JOE-PC
 * Domain:			Joe-pc
 * CreatedOn:		2014/10/22 12:15:10
 * Description:		Please input class summary
 * Version History:	Created,2014/10/22 12:15:10
 * 
 * 
 **************************************************************************/
using Sunnet.Cli.Business.Vcw.Models;
using Sunnet.Cli.Core.Vcw.Enums;
using Sunnet.Cli.Core.Vcw.Entities;
using LinqKit;
using Sunnet.Framework.Core.Extensions;
using System.Linq.Expressions;
using Sunnet.Framework.Extensions;
using Sunnet.Framework.Core.Tool;
using Sunnet.Cli.Core.Common.Enums;
using Sunnet.Cli.Business.Common;

namespace Sunnet.Cli.Business.Vcw
{
    public partial class VcwBusiness
    {
        /// <summary>
        /// PM查询分配给Coach的Assignments
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="sort"></param>
        /// <param name="order"></param>
        /// <param name="first"></param>
        /// <param name="count"></param>
        /// <param name="total"></param>
        /// <returns></returns>
        public List<AssignmentListModel> GetCoachAssignments(Expression<Func<AssignmentEntity, bool>> condition,
           string sort, string order, int first, int count, out int total)
        {
            var Assignments = _server.Assignments.AsExpandable()
                .Where(condition).Select(r => new AssignmentListModel()
                {
                    ID = r.ID,
                    SessionId = r.SessionId.Value,
                    Status = r.Status,
                    DueDate = r.DueDate,
                    FeedbackCalllDate = r.FeedbackCalllDate,
                    UploadTypeIds = r.AssignmentUploadTypes.Select(a => a.TypeId),
                    StrategyIds = r.AssignmentStrategies.Select(a => a.StrategyId),
                    CoachId = r.ReceiveUserId,
                    CoachName = r.UserInfo.FirstName + " " + r.UserInfo.LastName,
                    CommunityName = "",
                    SendUserId = r.SendUserId,
                    SessionText = r.Session == null ? "" : (r.Session.Status == EntityStatus.Active ? r.Session.Name : "")
                });

            total = Assignments.Count();
            var list = Assignments.OrderBy(sort, order).Skip(first).Take(count);
            return list.ToList();
        }

        /// <summary>
        /// PM 查找CoachGeneral时调用
        /// </summary>
        /// <param name="fileContition"></param>
        /// <param name="sort"></param>
        /// <param name="order"></param>
        /// <param name="first"></param>
        /// <param name="count"></param>
        /// <param name="total"></param>
        /// <returns></returns>
        public List<FileListModel> GetPMCoachGeneral(Expression<Func<Vcw_FileEntity, bool>> fileContition,
            string sort, string order, int first, int count, out int total)
        {
            total = 0;
            var query = _server.Files.AsExpandable().Where(fileContition).Select(r => new FileListModel()
            {
                ID = r.ID,
                IdentifyFileName = r.IdentifyFileName,
                UploadDate = r.UploadDate,
                DateRecorded = r.DateRecorded,
                FileName = r.FileName,
                FilePath = r.FilePath,
                Status = r.Status,
                StrategyOther = r.StrategyOther,
                CoachId = r.OwnerId,
                FeedbackText = r.FeedbackText,
                FeedbackFileName = r.FeedbackFileName,
                CoachName = r.UserInfo.FirstName + " " + r.UserInfo.LastName,
                StrategyIds = r.FileStrategies.Select(c => c.StrategyId)
            });
            total = query.Count();
            List<FileListModel> list = query.OrderBy(sort, order).Skip(first).Take(count).ToList();
            return list;
        }

        /// <summary>
        /// PM 角色 Summary 菜单的  Coach Files
        /// </summary>
        /// <param name="fileContition"></param>
        /// <param name="sort"></param>
        /// <param name="order"></param>
        /// <param name="first"></param>
        /// <param name="count"></param>
        /// <param name="total"></param>
        /// <returns></returns>
        public List<FileListModel> GetSummaryCoachFileList(Expression<Func<Vcw_FileEntity, bool>> fileContition,
            string sort, string order, int first, int count, out int total)
        {
            total = 0;
            var query = _server.Files.AsExpandable().Where(fileContition).Select(r => new FileListModel()
            {
                ID = r.ID,
                UploadDate = r.UploadDate,
                DateRecorded = r.DateRecorded,
                StrategyOther = r.StrategyOther,
                ContentOther = r.ContentOther,
                ContextId = r.ContextId,
                ContextOther = r.ContextOther,
                ContextText = r.Context == null ? "" : (r.Context.Status == EntityStatus.Active ? r.Context.Name : ""),
                FileName = r.FileName,
                FilePath = r.FilePath,
                Status = r.Status,
                VideoType = r.VideoType,
                LanguageId = r.LanguageId.Value,
                LanguageText = r.Language == null ? "" : (r.Language.Status == EntityStatus.Active ? r.Language.Name : ""),
                CoachId = r.OwnerId,
                CoachName = r.UserInfo.FirstName + " " + r.UserInfo.LastName,
                IdentifyFileName = r.IdentifyFileName,
                ContentIds = r.FileContents.Select(c => c.ContentId),
                StrategyIds = r.FileStrategies.Select(c => c.StrategyId)
            });
            total = query.Count();
            List<FileListModel> list = query.OrderBy(sort, order).Skip(first).Take(count).ToList();
            return list;
        }

        /// <summary>
        /// 获取SharedFiles列表
        /// </summary>
        /// <param name="fileContition"></param>
        /// <param name="sort"></param>
        /// <param name="order"></param>
        /// <param name="first"></param>
        /// <param name="count"></param>
        /// <param name="total"></param>
        /// <returns></returns>
        public List<FileListModel> GetSharedFiles(Expression<Func<Vcw_FileEntity, bool>> fileContition,
            string sort, string order, int first, int count, out int total)
        {
            total = 0;
            var query = _server.Files.AsExpandable().Where(fileContition).Select(r => new FileListModel()
            {
                ID = r.ID,
                ContentOther = r.ContentOther,
                ContextId = r.ContextId,
                ContextOther = r.ContextOther,
                DateRecorded = r.DateRecorded,
                FileName = r.FileName,
                FilePath = r.FilePath,
                Status = r.Status,
                UpdatedOn = r.UpdatedOn,
                UploadDate = r.UploadDate,
                VideoType = r.VideoType,
                LanguageId = r.LanguageId.Value,
                LanguageText = r.Language == null ? "" : (r.Language.Status == EntityStatus.Active ? r.Language.Name : ""),
                StrategyOther = r.StrategyOther,
                IdentifyFileName = r.IdentifyFileName,
                CoachId = r.OwnerId,
                FeedbackText = r.FeedbackText,
                FeedbackFileName = r.FeedbackFileName,
                CoachName = r.UserInfo.FirstName + " " + r.UserInfo.LastName,
                ContentIds = r.FileContents.Select(c => c.ContentId),
                StrategyIds = r.FileStrategies.Select(c => c.StrategyId)
            });
            total = query.Count();
            List<FileListModel> list = query.OrderBy(sort, order).Skip(first).Take(count).ToList();
            return list;
        }

        public CoachFileModel GetCoachGeneralFileModel(int id)
        {
            return _server.Files.Where(r => r.ID == id && r.IsDelete == false)
                .Select(r => new CoachFileModel()
                {
                    ID = r.ID,
                    IdentifyFileName = r.IdentifyFileName,
                    UploadDate = r.UploadDate,
                    DateRecorded = r.DateRecorded,
                    ContextId = r.ContextId.Value,
                    ContextOther = r.ContextOther,
                    ContentOther = r.ContentOther,
                    StrategyOther = r.StrategyOther,
                    Objectives = r.Objectives,
                    Effectiveness = r.Effectiveness,
                    LanguageId = r.LanguageId.Value,
                    LanguageText = r.Language == null ? "" : (r.Language.Status == EntityStatus.Active ? r.Language.Name : ""),
                    FileName = r.FileName,
                    FilePath = r.FilePath,
                    FeedbackText = r.FeedbackText,
                    FeedbackFileName = r.FeedbackFileName,
                    FeedbackFilePath = r.FeedbackFilePath,
                    FileShareds = r.FileShareds,
                    UploadUserId = r.UploadUserId,
                    AssignmentId = r.AssignmentId.Value,
                    OwnerId = r.OwnerId,
                    ContentIds = r.FileContents.Select(c => c.ContentId),
                    StrategyIds = r.FileStrategies.Select(c => c.StrategyId)
                }).FirstOrDefault();
        }

        /// <summary>
        /// PM 角色 View并且 做回复操作时调用
        /// </summary>
        /// <param name="fileId"></param>
        /// <returns></returns>
        public CoachFileModel GetCoachFileModelByPM(int fileId)
        {
            CoachFileModel File = _server.Files.Where(r => r.ID == fileId).Select(r => new CoachFileModel()
            {
                ID = r.ID,
                OwnerId = r.OwnerId,
                IdentifyFileName = r.IdentifyFileName,
                DateRecorded = r.DateRecorded,
                LanguageId = r.LanguageId.Value,
                LanguageText = r.Language == null ? "" : (r.Language.Status == EntityStatus.Active ? r.Language.Name : ""),
                ContextId = r.ContextId,
                ContextOther = r.ContextOther,
                ContextText = r.Context == null ? "" : (r.Context.Status == EntityStatus.Active ? r.Context.Name : ""),
                ContentOther = r.ContentOther,
                StrategyOther = r.StrategyOther,
                UploadUserId = r.UploadUserId,
                FileName = r.FileName,
                FilePath = r.FilePath,
                Objectives = r.Objectives,
                Effectiveness = r.Effectiveness,
                Status = r.Status,
                AssignmentId = r.AssignmentId.Value,
                SelectionIds = r.FileSelections.Select(a => a.SelectionId),
                ContentIds = r.FileContents.Select(c => c.ContentId),
                StrategyIds = r.FileStrategies.Select(c => c.StrategyId)
            }).FirstOrDefault();
            if (File != null)
            {
                IEnumerable<SelectItemModel> Contents = _vcwMasterDataBusiness.GetActiveVideo_Content_Datas();
                File.Contents = Contents.Where(r => File.ContentIds.Contains(r.ID));
            }
            return File;
        }

        /// <summary>
        /// 删除FileShareds
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="fileId"></param>
        /// <returns></returns>
        public OperationResult DeleteFileSharedRelation(List<int> fileIds, SharedUserTypeEnum type, int userId = 0)
        {
            IQueryable<FileSharedEntity> Entities;
            if (userId == 0)
            {
                Entities = _server.FileShareds
                .Where(f => fileIds.Contains(f.FileId) && f.Type == type);
            }
            else
            {
                Entities = _server.FileShareds
                .Where(f => f.UserId == userId && fileIds.Contains(f.FileId) && f.Type == type);
            }
            return _server.DeleteFileShared(Entities);
        }

        public int GetAssignmentReceive(int id)
        {
            return _server.Assignments
                .Where(a => a.ID == id)
                .Select(a => a.ReceiveUserId)
                .FirstOrDefault();
        }

        public AssignmentEntity GetAssignment(int id)
        {
            return _server.GetAssignment(id);
        }

        /// <summary>
        /// 转换CoachAssignment基本信息
        /// </summary>
        /// <param name="assignments"></param>
        /// <returns></returns>
        public List<AssignmentListModel> FormatTeacherAssignments(List<AssignmentListModel> assignments)
        {
            List<int> teacherids = assignments.Select(r => r.TeacherId).Distinct().ToList();
            List<TeacherListModel> list_teacher = _userBusiness.GetTeacherInfoByUserIds(teacherids);

            List<int> senderIds = assignments.Select(s => s.SendUserId).Distinct().ToList();
            List<UsernameModel> userNames = _userBusiness.GetUsernames(senderIds);

            List<SelectItemModel> UploadTypes = _vcwMasterDataBusiness.GetActiveUploadTypes();
            IEnumerable<SelectItemModel> Contents = _vcwMasterDataBusiness.GetActiveAssignment_Content_Datas();
            IEnumerable<SelectItemModel> Contexts = _vcwMasterDataBusiness.GetActiveContext_Datas();

            foreach (AssignmentListModel item in assignments)
            {
                TeacherListModel teacher = list_teacher.Where(r => r.TeacherUserId == item.TeacherId).FirstOrDefault();
                if (teacher != null)
                {
                    item.SchoolName = teacher.SchoolName;
                    item.CommunityName = teacher.CommunityName;
                }
                UsernameModel username = userNames.Find(r => r.ID == item.SendUserId);
                if (username != null)
                {
                    item.SendUserName = username.Firstname + " " + username.Lastname;
                }
                item.UploadTypes = UploadTypes.Where(r => item.UploadTypeIds.Contains(r.ID));
                item.Contents = Contents.Where(r => item.ContentIds.Contains(r.ID));
                item.Contexts = Contexts.Where(r => item.ContextIds.Contains(r.ID));
            }
            return assignments;
        }

        public List<FileListModel> FormatCoachSummary(List<FileListModel> list)
        {
            IEnumerable<SelectItemModel> Contents = _vcwMasterDataBusiness.GetActiveVideo_Content_Datas();
            List<SelectItemModel> Strategies = _vcwMasterDataBusiness.GetActiveCoachingStrategy_Datas();
            foreach (FileListModel item in list)
            {
                item.Strategies = Strategies.Where(r => item.StrategyIds.Contains(r.ID));
                item.Contents = Contents.Where(r => item.ContentIds.Contains(r.ID));
            }
            return list;
        }

        public List<FileListModel> FormatTeacherGenerals(List<FileListModel> list)
        {
            List<int> teacherids = list.Select(r => r.TeacherId).Distinct().ToList();
            List<TeacherListModel> list_teacher = _userBusiness.GetTeacherInfoByUserIds(teacherids);
            IEnumerable<SelectItemModel> Contents = _vcwMasterDataBusiness.GetActiveVideo_Content_Datas();

            foreach (FileListModel item in list)
            {
                TeacherListModel teacher = list_teacher.Where(r => r.TeacherUserId == item.TeacherId).FirstOrDefault();
                if (teacher != null)
                {
                    item.SchoolName = teacher.SchoolName;
                    item.CommunityName = teacher.CommunityName;
                }
                item.Contents = Contents.Where(r => item.ContentIds.Contains(r.ID));
            }
            return list;
        }
    }
}
