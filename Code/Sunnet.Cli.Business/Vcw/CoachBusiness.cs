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
using Sunnet.Framework.Core.Tool;
using Sunnet.Cli.Core.Vcw.Entities;
using Sunnet.Cli.Core.Vcw;
using Sunnet.Cli.Core;
using Sunnet.Cli.Core.Vcw.Enums;
using Sunnet.Cli.Business.Vcw.Models;
using Sunnet.Cli.Core.Users.Entities;
using Sunnet.Cli.Business.Users;
using Sunnet.Cli.Business.Communities;
using System.Linq.Expressions;
using LinqKit;
using Sunnet.Framework.Core.Extensions;
using Sunnet.Framework.Extensions;
using Sunnet.Cli.Business.Common;
using Sunnet.Cli.Core.Common.Enums;

namespace Sunnet.Cli.Business.Vcw
{
    public partial class VcwBusiness
    {
        private readonly IVcwContract _server;
        private readonly UserBusiness _userBusiness;
        private readonly CommunityBusiness _communityBusiness;
        private readonly VCW_MasterDataBusiness _vcwMasterDataBusiness;

        public VcwBusiness(VCWUnitOfWorkContext unit = null)
        {
            _server = DomainFacade.CreateVcwService(unit);
            _userBusiness = new UserBusiness();
            _communityBusiness = new CommunityBusiness();
            _vcwMasterDataBusiness = new VCW_MasterDataBusiness();
        }

        public List<FileListModel> GetAllFiles()
        {
            return _server.Files.Where(r => r.IsDelete == false)
                .Select(r => new FileListModel
                {
                    ID = r.AssignmentId.Value,
                    Status = r.Status
                }).ToList();
        }

        public List<AssignmentListModel> GetTeacherAssignmentsList(Expression<Func<AssignmentEntity, bool>> condition)
        {
            var Assignments = _server.Assignments.AsExpandable()
                    .Where(condition).Select(r => new AssignmentListModel()
                    {
                        ID = r.ID,
                        TeacherId = r.ReceiveUserId,
                        TeacherName = r.UserInfo.FirstName + " " + r.UserInfo.LastName,
                        WaveId = r.WaveId.Value,
                        DueDate = r.DueDate,
                        SessionId = r.SessionId.Value,
                        Status = r.Status,
                        FeedbackCalllDate = r.FeedbackCalllDate,
                        ContentOther = r.ContentOther,
                        ContextIds = r.AssignmentContexts.Select(a => a.ContextId),
                        ContentIds = r.AssignmentContents.Select(a => a.ContentId),
                        UploadTypeIds = r.AssignmentUploadTypes.Select(a => a.TypeId),
                        SendUserId = r.SendUserId,
                        SessionText = r.Session == null ? "" : (r.Session.Status == EntityStatus.Active ? r.Session.Name : "")
                    });
            var list = Assignments.OrderByDescending(r => r.ID).ToList();
            return list;
        }

        /// <summary>
        /// Coach查询已发送的TeacherAssignment列表
        /// </summary>
        /// <param name="user"></param>
        /// <param name="condition"></param>
        /// <param name="sort"></param>
        /// <param name="order"></param>
        /// <param name="first"></param>
        /// <param name="count"></param>
        /// <param name="total"></param>
        /// <returns></returns>
        public List<AssignmentListModel> GetTeacherAssignments(Expression<Func<AssignmentEntity, bool>> condition,
           string sort, string order, int first, int count, out int total)
        {
            var Assignments = _server.Assignments.AsExpandable().Where(condition)
                .Select(r => new AssignmentListModel()
                {
                    ID = r.ID,
                    TeacherId = r.ReceiveUserId,
                    TeacherName = r.UserInfo.FirstName + " " + r.UserInfo.LastName,
                    WaveId = r.WaveId.Value,
                    DueDate = r.DueDate,
                    SessionId = r.SessionId.Value,
                    Status = r.Status,
                    FeedbackCalllDate = r.FeedbackCalllDate,
                    ContextOther = r.ContextOther,
                    ContentOther = r.ContentOther,
                    ContextIds = r.AssignmentContexts.Select(a => a.ContextId),
                    ContentIds = r.AssignmentContents.Select(a => a.ContentId),
                    UploadTypeIds = r.AssignmentUploadTypes.Select(a => a.TypeId),
                    SendUserId = r.SendUserId,
                    WaveText = r.Wave == null ? "" : (r.Wave.Status == EntityStatus.Active ? r.Wave.Name : ""),
                    SessionText = r.Session == null ? "" : (r.Session.Status == EntityStatus.Active ? r.Session.Name : "")
                });

            total = Assignments.Count();
            List<AssignmentListModel> list = Assignments.OrderBy(sort, order).Skip(first).Take(count).ToList();

            return list.ToList();
        }

        /// <summary>
        /// Coach查询分配给自己的Assignments
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="sort"></param>
        /// <param name="order"></param>
        /// <param name="first"></param>
        /// <param name="count"></param>
        /// <param name="total"></param>
        /// <returns></returns>
        public List<AssignmentListModel> GetCoachAssignmentsByCoach(Expression<Func<AssignmentEntity, bool>> condition,
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
                    StrategyOther = r.StrategyOther,
                    SendUserId = r.SendUserId,
                    SessionText = r.Session == null ? "" : (r.Session.Status == EntityStatus.Active ? r.Session.Name : "")
                });

            total = Assignments.Count();
            List<AssignmentListModel> list = Assignments.OrderBy(sort, order).Skip(first).Take(count).ToList();
            return list;
        }

        /// <summary>
        /// Coach 查找 Teacher General时调用
        /// </summary>
        /// <param name="fileContition"></param>
        /// <param name="sort"></param>
        /// <param name="order"></param>
        /// <param name="first"></param>
        /// <param name="count"></param>
        /// <param name="total"></param>
        /// <returns></returns>
        public List<FileListModel> GetSummaryListByCoach(Expression<Func<Vcw_FileEntity, bool>> fileContition,
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
                TeacherId = r.OwnerId,
                TeacherName = r.UserInfo.FirstName + " " + r.UserInfo.LastName,
                IdentifyFileName = r.IdentifyFileName,
                FeedbackText = r.FeedbackText,
                FeedbackFileName = r.FeedbackFileName,
                AssignmentDueDate = r.Assignment == null ? CommonAgent.MinDate : r.Assignment.DueDate,
                ContentIds = r.FileContents.Select(c => c.ContentId)
            });
            total = query.Count();

            List<FileListModel> list = query.OrderBy(sort, order).Skip(first).Take(count).ToList();

            return list;
        }

        /// <summary>
        /// 查找LinkToAssignment功能选中的Assignments
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public List<LinkToAssignmentModel> GetLinkedAssignments(List<int> ids)
        {
            return _server.Assignments
                .Where(a => ids.Contains(a.ID))
                .Select(o => new LinkToAssignmentModel
                {
                    ID = o.ID,
                    ReceiveUserId = o.ReceiveUserId
                }).ToList();
        }

        /// <summary>
        /// 获取Coach发送给Teacher的Assignment信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public AssignmentModel GetTeacherAssignment(int id)
        {
            AssignmentModel Assignment = _server.Assignments.Where(a => a.ID == id && a.IsDelete == false)
                .Select(o => new AssignmentModel
                {
                    ID = o.ID,
                    Type = o.AssignmentType,
                    DueDate = o.DueDate,
                    CallDate = o.FeedbackCalllDate,
                    WaveId = o.WaveId.Value,
                    SessionId = o.SessionId.Value,
                    Status = o.Status,
                    Description = o.Description,
                    FeedbackText = o.FeedbackText,
                    FeedbackFileName = o.FeedbackFileName,
                    FeedbackFilePath = o.FeedbackFilePath,
                    ContextOther = o.ContextOther,
                    ContentOther = o.ContentOther,
                    ContextIds = o.AssignmentContexts.Select(a => a.ContextId),
                    ContentIds = o.AssignmentContents.Select(a => a.ContentId),
                    UploadTypeIds = o.AssignmentUploadTypes.Select(a => a.TypeId),
                    StrategyIds = o.AssignmentStrategies.Select(a => a.StrategyId),
                    Reports = o.AssignmentReports,
                    AssignmentFiles = o.AssignmentFiles,
                    Watch = o.Watch,
                    IsVisited = o.IsVisited,
                    ReceiveUserId = o.ReceiveUserId,
                    WaveText = o.Wave.Name == null ? "" : (o.Wave.Status == EntityStatus.Active ? o.Wave.Name : ""),
                    SessionText = o.Session == null ? "" : (o.Session.Status == EntityStatus.Active ? o.Session.Name : "")
                }).FirstOrDefault();

            if (Assignment != null)
            {
                List<SelectItemModel> UploadTypes = _vcwMasterDataBusiness.GetActiveUploadTypes();
                Assignment.UploadTypes = UploadTypes.Where(r => Assignment.UploadTypeIds.Contains(r.ID));
                IEnumerable<SelectItemModel> Contens = _vcwMasterDataBusiness.GetActiveAssignment_Content_Datas();
                Assignment.Contents = Contens.Where(r => Assignment.ContentIds.Contains(r.ID));
                IEnumerable<SelectItemModel> Contexs = _vcwMasterDataBusiness.GetActiveContext_Datas();
                Assignment.Contexts = Contexs.Where(r => Assignment.ContextIds.Contains(r.ID));
            }
            return Assignment;
        }

        public AssignmentModel GetCoachAssignment(int id)
        {
            AssignmentModel Assignment = _server.Assignments.Where(a => a.ID == id && a.IsDelete == false)
                .Select(o => new AssignmentModel
                {
                    ID = o.ID,
                    DueDate = o.DueDate,
                    CallDate = o.FeedbackCalllDate,
                    SessionId = o.SessionId.Value,
                    Status = o.Status,
                    Description = o.Description,
                    FeedbackText = o.FeedbackText,
                    FeedbackFileName = o.FeedbackFileName,
                    FeedbackFilePath = o.FeedbackFilePath,
                    StrategyIds = o.AssignmentStrategies.Select(a => a.StrategyId),
                    UploadTypeIds = o.AssignmentUploadTypes.Select(a => a.TypeId),
                    AssignmentFiles = o.AssignmentFiles,
                    Watch = o.Watch,
                    IsVisited = o.IsVisited,
                    ReceiveUserId = o.ReceiveUserId,
                    SessionText = o.Session == null ? "" : (o.Session.Status == EntityStatus.Active ? o.Session.Name : "")
                }).FirstOrDefault();

            if (Assignment != null)
            {
                List<SelectItemModel> UploadTypes = _vcwMasterDataBusiness.GetActiveUploadTypes();
                List<SelectItemModel> Strategies = _vcwMasterDataBusiness.GetActiveAssignmentCoachingStrategy_Datas();

                Assignment.UploadTypes = UploadTypes.Where(r => Assignment.UploadTypeIds.Contains(r.ID));
                Assignment.Strategies = Strategies.Where(r => Assignment.StrategyIds.Contains(r.ID));
            }
            return Assignment;
        }

        /// <summary>
        /// 添加或修改单个Assignment
        /// </summary>
        /// <param name="assignment"></param>
        /// <param name="contexts"></param>
        /// <param name="contents"></param>
        /// <param name="uploadTypes"></param>
        /// <param name="assignmentFiles"></param>
        /// <returns></returns>
        public OperationResult SendAssignment(AssignmentEntity assignment, int[] contexts, int[] contents, int[] uploadTypes,
            List<AssignmentFileEntity> assignmentFiles, string checkuploadedfiles, int[] strategies, string[] stgreports)
        {
            OperationResult result = new OperationResult(OperationResultType.Success);

            if (assignment.ID > 0)  //修改
            {
                //删除AssignmentContents
                _server.DeleteAssignmentContent(assignment.AssignmentContents.ToList(), false);

                //删除AssignmentContexts
                _server.DeleteAssignmentContext(assignment.AssignmentContexts.ToList(), false);

                //删除AssignmentFiles
                if (!string.IsNullOrEmpty(checkuploadedfiles))   //之前上传过的文件是否全部删除
                {
                    string[] filestr = checkuploadedfiles.Split('|');
                    List<AssignmentFileEntity> assignmentfiles = assignment.AssignmentFiles.ToList();
                    if (assignmentfiles.Count > 0)
                    {
                        foreach (string item in filestr)
                        {
                            AssignmentFileEntity file = assignmentfiles.Find(a => a.FilePath == item);
                            if (file != null)//在已上传的文件中查找是否包含该文件，若包含，则不删除
                            {
                                assignmentfiles.Remove(file);
                            }
                        }
                        _server.DeleteAssignmentFile(assignmentfiles, false);
                    }

                }
                else//如果之前上传过的文件已全部删除，则将其对应的文件子表全部删除
                {
                    _server.DeleteAssignmentFile(assignment.AssignmentFiles.ToList(), false);
                }


                //删除AssignmentUploadTypes
                _server.DeleteAssignmentUploadType(assignment.AssignmentUploadTypes.ToList(), false);

                //删除AssignmentStrategies
                _server.DeleteAssignmentStrategy(assignment.AssignmentStrategies.ToList(), false);

                _server.DeleteAssignmentReport(assignment.AssignmentReports.ToList(), false);

                AddAssignmentRelations(assignment, contexts, contents, uploadTypes, assignmentFiles, strategies, stgreports);

                result = _server.UpdateAssignment(assignment);

            }
            else   //添加
            {
                AddAssignmentRelations(assignment, contexts, contents, uploadTypes, assignmentFiles, strategies, stgreports);
                assignment.IsVisited = false;
                result = _server.AddAssignment(assignment);
            }
            return result;
        }

        /// <summary>
        /// 添加Assignment的子表数据
        /// </summary>
        /// <param name="assignment"></param>
        /// <param name="contexts"></param>
        /// <param name="contents"></param>
        /// <param name="uploadTypes"></param>
        /// <param name="assignmentFiles"></param>
        private static void AddAssignmentRelations(AssignmentEntity assignment, int[] contexts, int[] contents, int[] uploadTypes,
            List<AssignmentFileEntity> assignmentFiles, int[] strategies, string[] stgreports)
        {
            if (contexts != null)  //添加AssignmentContext
            {
                List<AssignmentContextEntity> contextEntities = new List<AssignmentContextEntity>();

                foreach (int item in contexts)
                {
                    AssignmentContextEntity AssignmentContext = new AssignmentContextEntity();
                    AssignmentContext.ContextId = item;
                    contextEntities.Add(AssignmentContext);
                }
                assignment.AssignmentContexts = contextEntities;
            }

            if (contents != null) //添加AssignmentContent
            {
                List<AssignmentContentEntity> contentEntities = new List<AssignmentContentEntity>();
                foreach (int item in contents)
                {
                    AssignmentContentEntity AssignmentContent = new AssignmentContentEntity();
                    AssignmentContent.ContentId = item;
                    contentEntities.Add(AssignmentContent);
                }
                assignment.AssignmentContents = contentEntities;
            }

            if (uploadTypes != null) //添加AssignmentUploadType
            {
                List<AssignmentUploadTypeEntity> assignmentUploadTypeEntities = new List<AssignmentUploadTypeEntity>();
                foreach (int item in uploadTypes)
                {
                    AssignmentUploadTypeEntity AssignmentUploadTypeContent = new AssignmentUploadTypeEntity();
                    AssignmentUploadTypeContent.TypeId = item;
                    assignmentUploadTypeEntities.Add(AssignmentUploadTypeContent);
                }
                assignment.AssignmentUploadTypes = assignmentUploadTypeEntities;
            }

            if (strategies != null) //添加AssignmentStrategy
            {
                List<AssignmentStrategyEntity> assignmentStrategies = new List<AssignmentStrategyEntity>();
                foreach (int item in strategies)
                {
                    AssignmentStrategyEntity AssignmentStrategy = new AssignmentStrategyEntity();
                    AssignmentStrategy.StrategyId = item;
                    assignmentStrategies.Add(AssignmentStrategy);
                }
                assignment.AssignmentStrategies = assignmentStrategies;
            }

            if (assignmentFiles != null) //添加AssignmentFile
            {
                if (assignment.AssignmentFiles != null)  //如果AssignmentFiles存在，则进行添加操作
                {
                    foreach (AssignmentFileEntity item in assignmentFiles)
                    {
                        assignment.AssignmentFiles.Add(item);
                    }
                }
                else
                {
                    List<AssignmentFileEntity> assignmentFileEntities = new List<AssignmentFileEntity>();

                    foreach (AssignmentFileEntity item in assignmentFiles)
                    {
                        assignmentFileEntities.Add(item);
                    }
                    assignment.AssignmentFiles = assignmentFileEntities;
                }
            }
            if (stgreports != null)
            {
                List<AssignmentReportEntity> assignmentReports = new List<AssignmentReportEntity>();
                foreach (string item in stgreports)
                {
                    string[] str_report = item.Split('|');
                    if (str_report.Length >= 2)
                    {
                        AssignmentReportEntity AssignmentReport = new AssignmentReportEntity();
                        AssignmentReport.ReportId = int.Parse(str_report[0]);
                        AssignmentReport.ReportCreatedOn = DateTime.Parse(str_report[1]);
                        assignmentReports.Add(AssignmentReport);
                    }
                }
                assignment.AssignmentReports = assignmentReports;
            }
        }

        /// <summary>
        /// 添加多个Assignment
        /// </summary>
        /// <param name="assignments"></param>
        /// <param name="contexts"></param>
        /// <param name="contents"></param>
        /// <param name="uploadTypes"></param>
        /// <param name="assignmentFiles"></param>
        /// <returns></returns>
        public OperationResult SendAssignment(AssignmentEntity assignment, List<int> teachers)
        {
            OperationResult result = new OperationResult(OperationResultType.Success);
            List<AssignmentEntity> list_assignment = new List<AssignmentEntity>();
            for (int i = 0; i < teachers.Count; i++)
            {
                AssignmentEntity assignment_new = _server.NewAssignmentEntity();
                assignment_new.UpdatedOn = assignment.UpdatedOn;
                assignment_new.CreatedOn = assignment.CreatedOn;
                assignment_new.Status = assignment.Status;
                assignment_new.AssignmentType = assignment.AssignmentType;
                assignment_new.SendUserId = assignment.SendUserId;
                assignment_new.ReceiveUserId = teachers[i];
                assignment_new.DueDate = assignment.DueDate;
                assignment_new.FeedbackCalllDate = assignment.FeedbackCalllDate;
                assignment_new.WaveId = assignment.WaveId;
                assignment_new.SessionId = assignment.SessionId;
                assignment_new.ContextOther = assignment.ContextOther;
                assignment_new.ContentOther = assignment.ContentOther;
                assignment_new.Description = assignment.Description;
                assignment_new.CreatedBy = assignment.CreatedBy;
                assignment_new.UpdatedBy = assignment.UpdatedBy;
                assignment_new.IsDelete = assignment.IsDelete;
                assignment_new.Watch = assignment.Watch;
                assignment_new.IsVisited = false;
                if (assignment.AssignmentFiles != null)
                {
                    assignment_new.AssignmentFiles = new List<AssignmentFileEntity>();
                    foreach (AssignmentFileEntity item in assignment.AssignmentFiles)
                    {
                        AssignmentFileEntity AssignmentFile = new AssignmentFileEntity();
                        AssignmentFile.FileName = item.FileName;
                        AssignmentFile.FilePath = item.FilePath;
                        assignment_new.AssignmentFiles.Add(AssignmentFile);
                    }
                }
                if (assignment.AssignmentContents != null)
                {
                    assignment_new.AssignmentContents = new List<AssignmentContentEntity>();
                    foreach (AssignmentContentEntity item in assignment.AssignmentContents)
                    {
                        AssignmentContentEntity AssignmentContent = new AssignmentContentEntity();
                        AssignmentContent.ContentId = item.ContentId;
                        assignment_new.AssignmentContents.Add(AssignmentContent);
                    }
                }
                if (assignment.AssignmentContexts != null)
                {
                    assignment_new.AssignmentContexts = new List<AssignmentContextEntity>();
                    foreach (AssignmentContextEntity item in assignment.AssignmentContexts)
                    {
                        AssignmentContextEntity AssignmentContext = new AssignmentContextEntity();
                        AssignmentContext.ContextId = item.ContextId;
                        assignment_new.AssignmentContexts.Add(AssignmentContext);
                    }
                }
                if (assignment.AssignmentUploadTypes != null)
                {
                    assignment_new.AssignmentUploadTypes = new List<AssignmentUploadTypeEntity>();
                    foreach (AssignmentUploadTypeEntity item in assignment.AssignmentUploadTypes)
                    {
                        AssignmentUploadTypeEntity AssignmentUploadType = new AssignmentUploadTypeEntity();
                        AssignmentUploadType.TypeId = item.TypeId;
                        assignment_new.AssignmentUploadTypes.Add(AssignmentUploadType);
                    }
                }
                if (assignment.AssignmentStrategies != null)
                {
                    assignment_new.AssignmentStrategies = new List<AssignmentStrategyEntity>();
                    foreach (AssignmentStrategyEntity item in assignment.AssignmentStrategies)
                    {
                        AssignmentStrategyEntity AssignmentStrategy = new AssignmentStrategyEntity();
                        AssignmentStrategy.StrategyId = item.StrategyId;
                        assignment_new.AssignmentStrategies.Add(AssignmentStrategy);
                    }
                }
                list_assignment.Add(assignment_new);
            }

            result = _server.AddAssignment(list_assignment);
            return result;
        }

        /// <summary>
        /// 删除多个Assignment
        /// </summary>
        /// <param name="assignments"></param>
        /// <returns></returns>
        public OperationResult DeleteAssignment(List<int> ids)
        {
            OperationResult result = new OperationResult(OperationResultType.Success);

            List<DeleteAssignmentModel> Assignments = _server.Assignments.Where(a => ids.Contains(a.ID))
                .Select(a => new DeleteAssignmentModel
                {
                    Assignment = a,
                    AssignmentContents = a.AssignmentContents,
                    AssignmentContexts = a.AssignmentContexts,
                    AssignmentFiles = a.AssignmentFiles,
                    AssignmentUploadTypes = a.AssignmentUploadTypes,
                    AssignmentStrategies = a.AssignmentStrategies,
                    AssignmentReports = a.AssignmentReports

                }).ToList();

            List<DeleteFileModel> Files = _server.Files.Where(a => ids.Contains(a.AssignmentId.Value))
                .Select(a => new DeleteFileModel
                {
                    File = a,
                    FileSelections = a.FileSelections,
                    FileShareds = a.FileShareds,
                    FileContents = a.FileContents,
                    FileStrategies = a.FileStrategies
                }).ToList();

            if (Assignments != null)
            {
                List<AssignmentEntity> Assignments_delete = new List<AssignmentEntity>();
                List<AssignmentContentEntity> AssignmentContents = new List<AssignmentContentEntity>();
                List<AssignmentContextEntity> AssignmentContexts = new List<AssignmentContextEntity>();
                List<AssignmentFileEntity> AssignmentFiles = new List<AssignmentFileEntity>();
                List<AssignmentWatchFileEntity> AssignmentWatchFiles = new List<AssignmentWatchFileEntity>();
                List<AssignmentUploadTypeEntity> AssignmentUploadTypes = new List<AssignmentUploadTypeEntity>();
                List<AssignmentStrategyEntity> AssignmentStrategies = new List<AssignmentStrategyEntity>();
                List<AssignmentReportEntity> AssignmentReports = new List<AssignmentReportEntity>();
                foreach (DeleteAssignmentModel item in Assignments)
                {
                    if (item.Assignment != null)
                        Assignments_delete.Add(item.Assignment);
                    if (item.AssignmentContents != null)
                        AssignmentContents.AddRange(item.AssignmentContents.ToList());
                    if (item.AssignmentContexts != null)
                        AssignmentContexts.AddRange(item.AssignmentContexts.ToList());
                    if (item.AssignmentFiles != null)
                        AssignmentFiles.AddRange(item.AssignmentFiles.ToList());
                    if (item.AssignmentUploadTypes != null)
                        AssignmentUploadTypes.AddRange(item.AssignmentUploadTypes.ToList());
                    if (item.AssignmentStrategies != null)
                        AssignmentStrategies.AddRange(item.AssignmentStrategies.ToList());
                    if (item.AssignmentReports != null)
                        AssignmentReports.AddRange(item.AssignmentReports.ToList());
                    if (item != null)
                        AssignmentUploadTypes.AddRange(item.AssignmentUploadTypes.ToList());
                }
                _server.DeleteAssignmentContent(AssignmentContents, false);
                _server.DeleteAssignmentContext(AssignmentContexts, false);
                _server.DeleteAssignmentFile(AssignmentFiles, false);
                _server.DeleteAssignmentWatchFile(AssignmentWatchFiles, false);
                _server.DeleteAssignmentUploadType(AssignmentUploadTypes, false);
                _server.DeleteAssignmentStrategy(AssignmentStrategies, false);
                _server.DeleteAssignmentReport(AssignmentReports, false);

                result = _server.DeleteAssignment(Assignments_delete);
            }

            if (Files != null)
            {
                List<Vcw_FileEntity> Files_delete = new List<Vcw_FileEntity>();
                List<FileSelectionEntity> FileSelections = new List<FileSelectionEntity>();
                List<FileSharedEntity> FileShareds = new List<FileSharedEntity>();
                List<FileContentEntity> FileContents = new List<FileContentEntity>();
                List<FileStrategyEntity> FileStrategies = new List<FileStrategyEntity>();
                foreach (DeleteFileModel item in Files)
                {
                    if (item.File != null)
                        Files_delete.Add(item.File);
                    if (item.FileSelections != null)
                        FileSelections.AddRange(item.FileSelections.ToList());
                    if (item.FileShareds != null)
                        FileShareds.AddRange(item.FileShareds.ToList());
                    if (item.FileContents != null)
                        FileContents.AddRange(item.FileContents.ToList());
                    if (item.FileStrategies != null)
                        FileStrategies.AddRange(item.FileStrategies.ToList());
                }
                _server.DeleteFileSelection(FileSelections, false);
                _server.DeleteFileShared(FileShareds, false);
                _server.DeleteFileContent(FileContents, false);
                _server.DeleteFileStrategy(FileStrategies, false);
                result = _server.DeleteFile(Files_delete);
            }
            return result;
        }

        /// <summary>
        /// 删除多个File
        /// </summary>
        /// <param name="ids"></param>
        /// <param name="isSummary">是否在Summary中删除文件</param>
        /// <returns></returns>
        public OperationResult DeleteFile(List<int> ids, bool isSummary = false)
        {
            OperationResult result = new OperationResult(OperationResultType.Success);

            List<DeleteFileModel> Files = _server.Files.Where(a => ids.Contains(a.ID))
                .Select(a => new DeleteFileModel
                {
                    File = a,
                    FileSelections = a.FileSelections,
                    FileShareds = a.FileShareds,
                    FileContents = a.FileContents,
                    FileStrategies = a.FileStrategies
                }).ToList();

            if (Files != null)
            {
                List<Vcw_FileEntity> Files_delete = new List<Vcw_FileEntity>();
                List<FileSelectionEntity> FileSelections = new List<FileSelectionEntity>();
                List<FileSharedEntity> FileShareds = new List<FileSharedEntity>();
                List<FileContentEntity> FileContents = new List<FileContentEntity>();
                List<FileStrategyEntity> FileStrategies = new List<FileStrategyEntity>();
                foreach (DeleteFileModel item in Files)
                {
                    if (item.File != null)
                        Files_delete.Add(item.File);
                    if (item.FileSelections != null)
                        FileSelections.AddRange(item.FileSelections.ToList());
                    if (item.FileShareds != null)
                        FileShareds.AddRange(item.FileShareds.ToList());
                    if (item.FileContents != null)
                        FileContents.AddRange(item.FileContents.ToList());
                    if (item.FileStrategies != null)
                        FileStrategies.AddRange(item.FileStrategies.ToList());
                }
                _server.DeleteFileSelection(FileSelections, false);
                _server.DeleteFileShared(FileShareds, false);
                _server.DeleteFileContent(FileContents, false);
                _server.DeleteFileStrategy(FileStrategies, false);
                result = _server.DeleteFile(Files_delete);

                if (isSummary)  //如果是Summary中删除文件，则要修改对应的Assignment的状态
                {
                    List<int> FileIds = new List<int>();
                    FileIds = Files.Select(r => r.File.AssignmentId.Value).Distinct().ToList();
                    foreach (int item in FileIds)
                    {
                        if (item > 0)
                        {
                            ChangeStatus(ChangeStatusEnum.DeleteFile, item);
                        }
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// Coach 角色 View并且 做回复操作时调用
        /// </summary>
        /// <param name="fileId"></param>
        /// <returns></returns>
        public TeacherAssignmentFileModel GetTeacherAssignmentFileModelByCoach(int fileId)
        {
            return _server.Files.Where(r => r.ID == fileId).Select(r => new TeacherAssignmentFileModel()
            {
                ID = r.ID,
                IdentifyFileName = r.IdentifyFileName,
                AssignmentID = r.AssignmentId.Value,
                DateRecorded = r.DateRecorded,
                Description = r.Description,
                LanguageId = r.LanguageId.Value,
                LanguageText = r.Language == null ? "" : (r.Language.Status == EntityStatus.Active ? r.Language.Name : ""),
                ContextId = r.ContextId.Value,
                ContextOther = r.ContextOther,
                ContextText = r.Context == null ? "" : (r.Context.Status == EntityStatus.Active ? r.Context.Name : ""),
                Status = r.Status,
                UploadUserId = r.UploadUserId,
                FileName = r.FileName,
                FilePath = r.FilePath,
                OwnerId = r.OwnerId,
                SelectionIds = r.FileSelections.Select(a => a.SelectionId)
            }).FirstOrDefault();
        }

        /// <summary>
        /// Coach 角色，View 并且 做回复操作时，调用
        /// </summary>
        /// <param name="fileId"></param>
        /// <returns></returns>
        public TeacherVIPFileModel GetTeacherVIPFileModelByCoach(int fileId)
        {
            return _server.Files.Where(r => r.ID == fileId).Select(r => new TeacherVIPFileModel()
            {
                ID = r.ID,
                IdentifyFileName = r.IdentifyFileName,
                AssignmentID = r.AssignmentId.Value,
                DateRecorded = r.DateRecorded,
                Description = r.Description,
                FileName = r.FileName,
                FilePath = r.FilePath,
                LanguageId = r.LanguageId.Value,
                LanguageText = r.Language == null ? "" : (r.Language.Status == EntityStatus.Active ? r.Language.Name : ""),
                OwnerId = r.OwnerId,
                UploadDate = r.UploadDate,
                VideoType = r.VideoType,
                Status = r.Status,
                TBRSDate = r.TBRSDate,
                UploadUserId = r.UploadUserId,
                SelectionIds = r.FileSelections.Select(a => a.SelectionId)
            }).FirstOrDefault();
        }

        /// <summary>
        /// 添加CoachFile
        /// </summary>
        /// <param name="model"></param>
        /// <param name="Content"></param>
        /// <param name="UserInfo"></param>
        /// <returns></returns>
        public OperationResult InsertFileEntity(CoachFileModel model, int[] language, int[] Content,
            int[] Context, int[] Strategy, UploadUserTypeEnum UploadUserType, UserBaseEntity UserInfo)
        {
            Vcw_FileEntity fileEntity = _server.NewFileEntity();
            //添加File信息表                    
            fileEntity.AssignmentId = model.AssignmentId;
            fileEntity.IdentifyFileName = model.IdentifyFileName;
            fileEntity.DateRecorded = model.DateRecorded.Value;
            fileEntity.StrategyOther = model.StrategyOther;
            fileEntity.ContentOther = model.ContentOther;
            fileEntity.ContextId = Context == null ? 0 : Context[0];
            fileEntity.ContextOther = model.ContextOther;
            fileEntity.Objectives = model.Objectives;
            fileEntity.Effectiveness = model.Effectiveness;
            fileEntity.OwnerId = model.OwnerId;
            fileEntity.UploadUserId = UserInfo.ID;
            fileEntity.UploadUserType = UploadUserType;
            fileEntity.VideoType = FileTypeEnum.CoachAssignment;
            fileEntity.FileName = model.FileName;
            fileEntity.FilePath = model.FilePath;
            fileEntity.LanguageId = language == null ? 0 : language[0];
            fileEntity.Status = FileStatus.Submitted;
            //----------------------------------------
            fileEntity.CreatedBy = UserInfo.ID;
            fileEntity.UpdatedBy = UserInfo.ID;
            fileEntity.UploadDate = DateTime.Now;
            fileEntity.UploadUserId = UserInfo.ID;
            fileEntity.IsDelete = false;
            fileEntity.TBRSDate = CommonAgent.MinDate;
            if (Content != null)
            {
                List<FileContentEntity> fileContents = new List<FileContentEntity>();
                foreach (int item in Content)
                {
                    FileContentEntity FileContent = new FileContentEntity();
                    FileContent.ContentId = item;
                    fileContents.Add(FileContent);
                }
                fileEntity.FileContents = fileContents;
            }
            if (Strategy != null)
            {
                List<FileStrategyEntity> fileStrategies = new List<FileStrategyEntity>();
                foreach (int item in Strategy)
                {
                    FileStrategyEntity FileStrategy = new FileStrategyEntity();
                    FileStrategy.StrategyId = item;
                    fileStrategies.Add(FileStrategy);
                }
                fileEntity.FileStrategies = fileStrategies;
            }
            OperationResult result = _server.AddFile(fileEntity);
            return result;
        }

        /// <summary>
        /// 添加CoachGeneral
        /// </summary>
        /// <param name="model"></param>
        /// <param name="Content"></param>
        /// <param name="UserInfo"></param>
        /// <returns></returns>
        public OperationResult InsertFileEntity(CoachFileModel model, int[] pm, int[] Coach,
            int[] Strategy, UploadUserTypeEnum UploadUserType, UserBaseEntity UserInfo)
        {
            Vcw_FileEntity entity = _server.NewFileEntity();
            //添加File信息表                                       
            entity.AssignmentId = 0;
            entity.IdentifyFileName = model.IdentifyFileName;
            entity.DateRecorded = model.DateRecorded.Value;
            entity.StrategyOther = model.StrategyOther;
            entity.Objectives = model.Objectives;
            entity.Effectiveness = model.Effectiveness;
            entity.OwnerId = UserInfo.ID;
            entity.UploadUserId = UserInfo.ID;
            entity.UploadUserType = UploadUserType;
            entity.VideoType = FileTypeEnum.CoachGeneral;
            entity.FileName = model.FileName;
            entity.FilePath = model.FilePath;
            entity.Status = FileStatus.Submitted;
            //----------------------------------------
            entity.CreatedBy = UserInfo.ID;
            entity.UpdatedBy = UserInfo.ID;
            entity.UploadDate = DateTime.Now;
            entity.UploadUserId = UserInfo.ID;
            entity.IsDelete = false;
            entity.TBRSDate = CommonAgent.MinDate;
            entity.FileShareds = new List<FileSharedEntity>();
            entity.FileStrategies = new List<FileStrategyEntity>();

            //添加FileShareds关系表
            if (pm != null)
            {
                if (pm.Length > 0)
                {
                    foreach (int item in pm)
                    {
                        FileSharedEntity FileShared = new FileSharedEntity();
                        FileShared.UserId = item;
                        FileShared.Type = SharedUserTypeEnum.PM;
                        entity.FileShareds.Add(FileShared);
                    }
                }
            }
            if (Coach != null)
            {
                if (Coach.Length > 0)
                {
                    foreach (int item in Coach)
                    {
                        FileSharedEntity FileShared = new FileSharedEntity();
                        FileShared.UserId = item;
                        FileShared.Type = SharedUserTypeEnum.Coach;
                        entity.FileShareds.Add(FileShared);
                    }
                }
            }

            if (Strategy != null)
            {
                if (Strategy.Length > 0)
                {
                    foreach (int item in Strategy)
                    {
                        FileStrategyEntity FileStrategy = new FileStrategyEntity();
                        FileStrategy.StrategyId = item;
                        FileStrategy.CreatedOn = DateTime.Now;
                        entity.FileStrategies.Add(FileStrategy);
                    }
                }
            }

            OperationResult result = _server.AddFile(entity);
            return result;
        }       

        /// <summary>
        /// 根据userid获取上传的VIPDocuments
        /// </summary>
        /// <param name="userid"></param>
        /// <returns></returns>
        public List<VIPDocumentEntity> GetVIPDocuments()
        {
            return _server.VIPDocuments.ToList();
        }

        public OperationResult DeleteVIPDocuments(List<VIPDocumentEntity> VIPDocuments)
        {
            return _server.DeleteVIPDocument(VIPDocuments);
        }

        public OperationResult DeleteFileShareds(List<FileSharedEntity> FileShareds, bool isSave)
        {
            return _server.DeleteFileShared(FileShareds, isSave);
        }

        public OperationResult DeleteFileStrategies(List<FileStrategyEntity> FileStrategies, bool isSave)
        {
            return _server.DeleteFileStrategy(FileStrategies, isSave);
        }

        public OperationResult AddVIPDocuments(List<VIPDocumentEntity> VIPDocuments)
        {
            return _server.AddVIPDocument(VIPDocuments);
        }

        public OperationResult UpdateAssignment(AssignmentEntity Assignment)
        {
            return _server.UpdateAssignment(Assignment);
        }

        /// <summary>
        /// 根据userid查找分享给该用户的File
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public List<int> GetSharedFilesByUser(int userId, SharedUserTypeEnum sharedUserType)
        {
            return _server.FileShareds
                .Where(a => a.UserId == userId && a.Type == sharedUserType)
                .Select(a => a.FileId)
                .ToList();
        }

        /// <summary>
        /// 根据userid查找分享给该用户的File
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public List<int> GetSharedFilesByUser(List<int> userIds, SharedUserTypeEnum sharedUserType)
        {
            return _server.FileShareds
                .Where(a => userIds.Contains(a.UserId) && a.Type == sharedUserType)
                .Select(a => a.FileId)
                .ToList();
        }
    }
}
