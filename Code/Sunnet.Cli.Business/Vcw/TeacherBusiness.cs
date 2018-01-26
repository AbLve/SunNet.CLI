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
using System.Linq.Expressions;
using Sunnet.Cli.Core.Vcw.Entities;
using LinqKit;
using Sunnet.Framework.Core.Extensions;
using Sunnet.Framework.Core.Tool;
using Sunnet.Cli.Core.Users.Entities;
using Sunnet.Cli.Core.Vcw.Enums;
using Sunnet.Cli.Business.Common;
using Sunnet.Cli.Core.Common.Enums;
using Sunnet.Framework.Extensions;

namespace Sunnet.Cli.Business.Vcw
{
    public partial class VcwBusiness
    {
        public Vcw_FileEntity GetFileEntity(int id)
        {
            return _server.GetFile(id);
        }

        public OperationResult UpdateFile(Vcw_FileEntity fileEntity)
        {
            return _server.UpdateFile(fileEntity);
        }


        /// <summary>
        /// 删除fileselection关系表
        /// </summary>
        /// <param name="FileSelections"></param>
        /// <returns></returns>
        public OperationResult DeleteFileSelection(List<FileSelectionEntity> FileSelections, bool isSave)
        {
            return _server.DeleteFileSelection(FileSelections, isSave);
        }

        /// <summary>
        /// 删除filecontent关系表
        /// </summary>
        /// <param name="filecontent"></param>
        /// <returns></returns>
        public OperationResult DeleteFileContent(List<FileContentEntity> FileContents, bool isSave)
        {
            return _server.DeleteFileContent(FileContents, isSave);
        }

        public List<FileListModel> GetSummaryList(Expression<Func<Vcw_FileEntity, bool>> fileContition,
            string sort, string order, int first, int count, out int total)
        {
            total = 0;
            var query = _server.Files.AsExpandable().Where(fileContition).Select(r => new FileListModel()
            {
                ID = r.ID,
                ContentOther = r.ContentOther,
                ContextId = r.ContextId,
                ContextOther = r.ContextOther,
                ContextText = r.Context == null ? "" : (r.Context.Status == EntityStatus.Active ? r.Context.Name : ""),
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
                ContentIds = r.FileContents.Select(c => c.ContentId),
                StrategyIds = r.FileStrategies.Select(c => c.StrategyId),
                AssignmentId = r.Assignment == null ? 0 : r.Assignment.ID,
                FeedbackText = r.FeedbackText,
                FeedbackFileName = r.FeedbackFileName,
                AssignmentFeedbackText = r.Assignment == null ? "" : r.Assignment.FeedbackText,
                AssignmentFeedbackFileName = r.Assignment == null ? "" : r.Assignment.FeedbackFileName
            });
            total = query.Count();
            List<FileListModel> list = query.OrderBy(sort, order).Skip(first).Take(count).ToList();
            return list;
        }

        public List<FileListModel> GetSummaryListWithDueDate(Expression<Func<Vcw_FileEntity, bool>> fileContition,
            string sort, string order, int first, int count, out int total)
        {
            total = 0;
            var query = _server.Files.AsExpandable().Where(fileContition).Select(r => new FileListModel()
            {
                ID = r.ID,
                ContentOther = r.ContentOther,
                ContextId = r.ContextId,
                ContextOther = r.ContextOther,
                ContextText = r.Context == null ? "" : (r.Context.Status == EntityStatus.Active ? r.Context.Name : ""),
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
                AssignmentDueDate = r.Assignment == null ? CommonAgent.MinDate : r.Assignment.DueDate,
                ContentIds = r.FileContents.Select(c => c.ContentId),
                StrategyIds = r.FileStrategies.Select(c => c.StrategyId),
                AssignmentId = r.Assignment == null ? 0 : r.Assignment.ID,
                FeedbackText = r.FeedbackText,
                FeedbackFileName = r.FeedbackFileName,
                AssignmentFeedbackText = r.Assignment == null ? "" : r.Assignment.FeedbackText,
                AssignmentFeedbackFileName = r.Assignment == null ? "" : r.Assignment.FeedbackFileName
            });
            total = query.Count();
            List<FileListModel> list = query.OrderBy(sort, order).Skip(first).Take(count).ToList();
            return list;
        }

        public List<AssignmentListModel> GetVIPByTeacher(Expression<Func<AssignmentEntity, bool>> assignmentContition,
            string sort, string order, int first, int count, out int total)
        {
            total = 0;
            var query = _server.Assignments.AsExpandable().Where(assignmentContition).Select(r => new AssignmentListModel()
            {
                ID = r.ID,
                DueDate = r.DueDate,
                WaveId = r.WaveId.Value,
                ContextOther = r.ContextOther,
                ContentOther = r.ContentOther,
                ContextIds = r.AssignmentContexts.Select(a => a.ContextId),
                ContentIds = r.AssignmentContents.Select(a => a.ContentId),
                UploadTypeIds = r.AssignmentUploadTypes.Select(a => a.TypeId),
                Status = r.Status,
                SendUserId = r.SendUserId,
                WaveText = r.Wave == null ? "" : (r.Wave.Status == EntityStatus.Active ? r.Wave.Name : "")
            });
            total = query.Count();
            List<AssignmentListModel> list = query.OrderBy(sort, order).Skip(first).Take(count).ToList();
            return list;
        }

        public List<AssignmentListModel> GetTeacherAssignment(Expression<Func<AssignmentEntity, bool>> assignmentContition,
            string sort, string order, int first, int count, out int total)
        {
            total = 0;
            var query = _server.Assignments.AsExpandable().Where(assignmentContition).Select(r => new AssignmentListModel()
            {
                ID = r.ID,
                DueDate = r.DueDate,
                ContentOther = r.ContentOther,
                ContentIds = r.AssignmentContents.Select(a => a.ContentId),
                SessionId = r.SessionId.Value,
                FeedbackCalllDate = r.FeedbackCalllDate,
                UploadTypeIds = r.AssignmentUploadTypes.Select(a => a.TypeId),
                Status = r.Status,
                SendUserId = r.SendUserId,
                SessionText = r.Session == null ? "" : (r.Session.Status == EntityStatus.Active ? r.Session.Name : "")
            });
            total = query.Count();
            List<AssignmentListModel> list = query.OrderBy(sort, order).Skip(first).Take(count).ToList();
            return list;
        }

        /// <summary>
        /// 添加TeacherGeneral文件
        /// </summary>
        /// <param name="model"></param>
        /// <param name="Content"></param>
        /// <param name="UserInfo"></param>
        /// <returns></returns>
        public OperationResult InsertFileEntity(TeacherGeneralFileModel model, int[] Content, int[] Context, UserBaseEntity UserInfo)
        {
            Vcw_FileEntity fileEntity = _server.NewFileEntity();
            fileEntity.AssignmentId = 0;
            fileEntity.IdentifyFileName = model.IdentifyFileName;
            fileEntity.ContentOther = model.ContentOther ?? string.Empty;
            fileEntity.ContextId = Context == null ? 0 : Context[0];
            fileEntity.ContextOther = model.ContextOther ?? string.Empty;
            fileEntity.CreatedBy = UserInfo.ID;
            fileEntity.UpdatedBy = UserInfo.ID;
            fileEntity.DateRecorded = model.DateVieoRecorded.Value;
            fileEntity.IsDelete = false;
            fileEntity.Description = model.Description;
            fileEntity.Effectiveness = string.Empty;
            fileEntity.FeedbackFileName = string.Empty;
            fileEntity.FeedbackFilePath = string.Empty;
            fileEntity.FeedbackText = string.Empty;
            fileEntity.FileName = model.FileName;
            fileEntity.FilePath = model.FilePath;
            fileEntity.Objectives = string.Empty;
            fileEntity.OwnerId = UserInfo.ID;
            fileEntity.Status = FileStatus.Submitted;
            fileEntity.StrategyOther = string.Empty;
            fileEntity.TBRSDate = Common.CommonAgent.MinDate;
            fileEntity.UploadDate = DateTime.Now;
            fileEntity.UploadUserId = UserInfo.ID;
            fileEntity.UploadUserType = model.UploadUserType;
            fileEntity.VideoType = FileTypeEnum.TeacherGeneral;
            if (Content != null)
            {
                fileEntity.FileContents = new List<FileContentEntity>();
                foreach (int item in Content)
                {
                    FileContentEntity FileContent = new FileContentEntity();
                    FileContent.ContentId = item;
                    fileEntity.FileContents.Add(FileContent);
                }
            }
            return _server.AddFile(fileEntity);
        }

        /// <summary>
        /// 添加TeacherGeneral文件
        /// </summary>
        /// <param name="model"></param>
        /// <param name="Content"></param>
        /// <param name="UserInfo"></param>
        /// <returns></returns>
        public OperationResult InsertFileEntity(TeacherGeneralFileModel model, int[] Content, int[] Context, int[] teacher_select, UserBaseEntity UserInfo)
        {
            List<Vcw_FileEntity> entities = new List<Vcw_FileEntity>();
            foreach (int item in teacher_select)
            {
                Vcw_FileEntity fileEntity = _server.NewFileEntity();
                fileEntity.AssignmentId = 0;
                fileEntity.IdentifyFileName = model.IdentifyFileName;
                fileEntity.ContentOther = model.ContentOther ?? string.Empty;
                fileEntity.ContextId = Context == null ? 0 : Context[0];
                fileEntity.ContextOther = model.ContextOther ?? string.Empty;
                fileEntity.CreatedBy = UserInfo.ID;
                fileEntity.UpdatedBy = UserInfo.ID;
                fileEntity.DateRecorded = model.DateVieoRecorded.Value;
                fileEntity.IsDelete = false;
                fileEntity.Description = model.Description;
                fileEntity.Effectiveness = string.Empty;
                fileEntity.FeedbackFileName = string.Empty;
                fileEntity.FeedbackFilePath = string.Empty;
                fileEntity.FeedbackText = string.Empty;
                fileEntity.FileName = model.FileName;
                fileEntity.FilePath = model.FilePath;
                fileEntity.Objectives = string.Empty;
                fileEntity.OwnerId = item;
                fileEntity.Status = FileStatus.Submitted;
                fileEntity.StrategyOther = string.Empty;
                fileEntity.TBRSDate = Common.CommonAgent.MinDate;
                fileEntity.UploadDate = DateTime.Now;
                fileEntity.UploadUserId = UserInfo.ID;
                fileEntity.UploadUserType = model.UploadUserType;
                fileEntity.VideoType = FileTypeEnum.TeacherGeneral;
                if (Content != null)
                {
                    List<FileContentEntity> fileContents = new List<FileContentEntity>();
                    foreach (int item_content in Content)
                    {
                        FileContentEntity FileContent = new FileContentEntity();
                        FileContent.ContentId = item_content;
                        fileContents.Add(FileContent);
                    }
                    fileEntity.FileContents = fileContents;
                }
                entities.Add(fileEntity);
            }
            return _server.AddFile(entities);
        }

        public TeacherGeneralFileModel GetTeacherGeneralFileModel(int id)
        {
            TeacherGeneralFileModel File = _server.Files.AsExpandable().Where(r => r.ID == id).Select(
                r => new TeacherGeneralFileModel()
                {
                    ID = r.ID,
                    IdentifyFileName = r.IdentifyFileName,
                    ContentOther = r.ContentOther,
                    ContextId = r.ContextId,
                    ContextText = r.Context == null ? "" : (r.Context.Status == EntityStatus.Active ? r.Context.Name : ""),
                    ContextOther = r.ContextOther,
                    DateVieoRecorded = r.DateRecorded,
                    Description = r.Description,
                    FileName = r.FileName,
                    FilePath = r.FilePath,
                    FeedbackFileName = r.FeedbackFileName,
                    FeedbackFilePath = r.FeedbackFilePath,
                    FeedbackText = r.FeedbackText,
                    UploadDate = r.UploadDate,
                    UploadUserId = r.UploadUserId,
                    OwnerId = r.OwnerId,
                    ContentIds = r.FileContents.Select(c => c.ContentId)
                }
                ).FirstOrDefault();
            if (File != null)
            {
                IEnumerable<SelectItemModel> Contents = _vcwMasterDataBusiness.GetActiveVideo_Content_Datas();
                File.Contents = Contents.Where(r => File.ContentIds.Contains(r.ID));
            }
            return File;
        }



        /// <summary>
        /// 添加Teacher VIP File
        /// </summary>
        /// <param name="model"></param>
        /// <param name="UserInfo"></param>
        /// <returns></returns>
        public OperationResult InsertFileEntity(TeacherVIPFileModel model, UserBaseEntity UserInfo)
        {
            Vcw_FileEntity fileEntity = _server.NewFileEntity();
            fileEntity.IdentifyFileName = model.IdentifyFileName;
            fileEntity.AssignmentId = model.AssignmentID;
            fileEntity.DateRecorded = model.DateRecorded.Value;
            fileEntity.LanguageId = model.LanguageId == null ? 0 : model.LanguageId;
            fileEntity.Description = model.Description;
            fileEntity.OwnerId = model.OwnerId;
            fileEntity.UploadUserId = UserInfo.ID;
            fileEntity.UploadUserType = model.UploadUserType;
            fileEntity.VideoType = FileTypeEnum.TeacherVIP;
            fileEntity.FileName = model.FileName;
            fileEntity.FilePath = model.FilePath;
            fileEntity.Status = model.Status;
            //----------------------------------------
            fileEntity.CreatedBy = UserInfo.ID;
            fileEntity.UpdatedBy = UserInfo.ID;
            fileEntity.UploadDate = DateTime.Now;
            fileEntity.IsDelete = false;

            fileEntity.Effectiveness = string.Empty;
            fileEntity.FeedbackFileName = string.Empty;
            fileEntity.FeedbackFilePath = string.Empty;
            fileEntity.FeedbackText = string.Empty;
            fileEntity.Objectives = string.Empty;

            fileEntity.StrategyOther = string.Empty;
            fileEntity.TBRSDate = Common.CommonAgent.MinDate;
            fileEntity.ContentOther = string.Empty;
            fileEntity.ContextId = 0;
            fileEntity.ContextOther = string.Empty;

            OperationResult result = _server.AddFile(fileEntity);
            model.ID = fileEntity.ID;
            return result;
        }

        /// <summary>
        /// 添加Teacher Assignment File
        /// </summary>
        /// <param name="model"></param>
        /// <param name="UserInfo"></param>
        /// <returns></returns>
        public OperationResult InsertFileEntity(TeacherAssignmentFileModel model, UserBaseEntity UserInfo)
        {
            Vcw_FileEntity fileEntity = _server.NewFileEntity();
            fileEntity.IdentifyFileName = model.IdentifyFileName;
            fileEntity.AssignmentId = model.AssignmentID;
            fileEntity.DateRecorded = model.DateRecorded.Value;
            fileEntity.LanguageId = model.LanguageId == null ? 0 : model.LanguageId;
            fileEntity.Description = model.Description;
            fileEntity.OwnerId = model.OwnerId;
            fileEntity.UploadUserId = UserInfo.ID;
            fileEntity.UploadUserType = model.UploadUserType;
            fileEntity.VideoType = FileTypeEnum.TeacherAssignment;
            fileEntity.FileName = model.FileName;
            fileEntity.FilePath = model.FilePath;
            fileEntity.Status = model.Status;
            fileEntity.ContextId = model.ContextId == null ? 0 : model.ContextId;
            fileEntity.ContextOther = model.ContextOther ?? string.Empty;
            //----------------------------------------
            fileEntity.CreatedBy = UserInfo.ID;
            fileEntity.UpdatedBy = UserInfo.ID;
            fileEntity.UploadDate = DateTime.Now;
            fileEntity.IsDelete = false;

            fileEntity.Effectiveness = string.Empty;
            fileEntity.FeedbackFileName = string.Empty;
            fileEntity.FeedbackFilePath = string.Empty;
            fileEntity.FeedbackText = string.Empty;
            fileEntity.Objectives = string.Empty;

            fileEntity.StrategyOther = string.Empty;
            fileEntity.TBRSDate = Common.CommonAgent.MinDate;
            fileEntity.ContentOther = string.Empty;


            OperationResult result = _server.AddFile(fileEntity);
            model.ID = fileEntity.ID;
            return result;
        }

        public OperationResult InsertFileEntity(Vcw_FileEntity fileEntity)
        {
            return _server.AddFile(fileEntity);
        }

        public OperationResult InsertFileEntity(List<Vcw_FileEntity> fileEntities)
        {
            return _server.AddFile(fileEntities);
        }

        /// <summary>
        /// Teacher 角色 View时调用
        /// </summary>
        /// <param name="fileId"></param>
        /// <returns></returns>
        public TeacherVIPFileModel GetTeacherVIPFileModel(int fileId)
        {
            return _server.Files.Where(r => r.ID == fileId).Select(r => new TeacherVIPFileModel()
            {
                ID = r.ID,
                AssignmentID = r.AssignmentId.Value,
                DateRecorded = r.DateRecorded,
                Description = r.Description,
                FileName = r.FileName,
                FilePath = r.FilePath,
                LanguageId = r.LanguageId.Value,
                LanguageText = r.Language == null ? "" : (r.Language.Status == EntityStatus.Active ? r.Language.Name : ""),
                OwnerId = r.OwnerId,
                UploadDate = r.UploadDate,
                IdentifyFileName = r.IdentifyFileName
            }).FirstOrDefault();
        }

        /// <summary>
        /// Teacher 角色 View时调用
        /// </summary>
        /// <param name="fileId"></param>
        /// <returns></returns>
        public TeacherAssignmentFileModel GetTeacherAssignmentFileModel(int fileId)
        {
            return _server.Files.Where(r => r.ID == fileId).Select(r => new TeacherAssignmentFileModel()
            {
                ID = r.ID,
                AssignmentID = r.AssignmentId.Value,
                DateRecorded = r.DateRecorded,
                Description = r.Description,
                FileName = r.FileName,
                FilePath = r.FilePath,
                LanguageId = r.LanguageId.Value,
                LanguageText = r.Language == null ? "" : (r.Language.Status == EntityStatus.Active ? r.Language.Name : ""),
                OwnerId = r.OwnerId,
                UploadDate = r.UploadDate,
                ContextId = r.ContextId.Value,
                ContextText = r.Context == null ? "" : (r.Context.Status == EntityStatus.Active ? r.Context.Name : ""),
                ContextOther = r.ContextOther,
                IdentifyFileName = r.IdentifyFileName
            }).FirstOrDefault();
        }

        /// <summary>
        /// 更新Assignment的状态
        /// </summary>
        /// <param name="method"></param>
        /// <param name="assignmentId"></param>
        /// <param name="status"></param>
        public void ChangeStatus(ChangeStatusEnum method, int assignmentId, FileStatus status = FileStatus.Submitted)
        {
            _server.ChangeStatus(method, assignmentId, status);
        }

        public int GetTeacherVIPCount(int userId)
        {
            return _server.Assignments
                .Where(r => r.ReceiveUserId == userId && r.IsDelete == false &&
                    r.IsVisited == false && r.AssignmentType == AssignmentTypeEnum.TeacherVIP).Count();
        }

        public int GetTeacherAssignmentCount(int userId)
        {
            return _server.Assignments
                .Where(r => r.ReceiveUserId == userId && r.IsDelete == false &&
                    r.IsVisited == false && r.AssignmentType == AssignmentTypeEnum.TeacherAssignment).Count();
        }

        public int GetCoachAssignmentCount(int userId)
        {
            return _server.Assignments
                .Where(r => r.ReceiveUserId == userId && r.IsDelete == false &&
                    r.IsVisited == false && r.AssignmentType == AssignmentTypeEnum.CoachAssignment).Count();
        }


        public string GetLanguageText(int? LanguageId)
        {
            if (LanguageId == null)
            {
                return "";
            }
            else
            {
                if (LanguageId.Value > 0)
                {
                    Video_Language_DataEntity Language = _vcwMasterDataBusiness.GetVideo_Language_Data(LanguageId.Value);
                    return Language == null ? "" : Language.Name;
                }
                else
                {
                    return "";
                }
            }
        }

        public FeedbackModel GetFileFeedbackModel(int id)
        {
            FeedbackModel File = _server.Files.AsExpandable().Where(r => r.ID == id).Select(
                r => new FeedbackModel()
                {
                    FeedbackFileName = r.FeedbackFileName,
                    FeedbackFilePath = r.FeedbackFilePath,
                    FeedbackText = r.FeedbackText,
                }
                ).FirstOrDefault();
            return File;
        }

        public FeedbackModel GetAssignmentFeedbackModel(int id)
        {
            FeedbackModel File = _server.Assignments.AsExpandable().Where(r => r.ID == id).Select(
                r => new FeedbackModel()
                {
                    FeedbackFileName = r.FeedbackFileName,
                    FeedbackFilePath = r.FeedbackFilePath,
                    FeedbackText = r.FeedbackText,
                }
                ).FirstOrDefault();
            return File;
        }
    }
}
