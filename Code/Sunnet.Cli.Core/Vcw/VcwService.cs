using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/**************************************************************************
 * Developer: 		Lee
 * Computer:		Lee-PC
 * Domain:			Lee-pc
 * CreatedOn:		2014/10/21
 * Description:		Create AuthorityEntity
 * Version History:	Created,2014/10/21
 * 
 * 
 **************************************************************************/
using Sunnet.Cli.Core.Vcw.Entities;
using Sunnet.Cli.Core.Vcw.Interfaces;
using Sunnet.Framework.Core.Base;
using Sunnet.Framework.EmailSender;
using Sunnet.Framework.Encrypt;
using Sunnet.Framework.File;
using Sunnet.Framework.Log;
using Sunnet.Framework.Core.Tool;
using Sunnet.Cli.Core.Vcw.Enums;

namespace Sunnet.Cli.Core.Vcw
{
    internal class VcwService : CoreServiceBase, IVcwContract
    {
        IAssignmentRpst AssignmentRpst;

        IAssignmentContentRpst AssignmentContentRpst;

        IAssignmentContextRpst AssignmentContextRpst;

        IAssignmentFileRpst AssignmentFileRpst;

        IAssignmentWatchFileRpst AssignmentWatchFileRpst;

        IAssignmentUploadTypeRpst AssignmentUploadTypeRpst;

        IAssignmentStrategyRpst AssignmentStrategyRpst;

        IAssignmentReportRpst AssignmentReportRpst;

        IVIPDocumentRpst VIPDocumentRpst;

        IFileRpst FileRpst;

        IFileSelectionRpst FileSelectionRpst;

        IFileSharedRpst FileSharedRpst;

        IFileContentRpst FileContentRpst;

        IFileStrategyRpst FileStrategyRpst;

        ITeacherRpst_V TeacherRpst;

        IUserRpst_V UserRpst;

        IUploadTypeRpst UploadTypeRpst;

        ISessionRpst SessionRpst;

        IWaveRpst WaveRpst;

        IContext_DataRpst Context_DataRpst;

        IAssignment_Content_DataRpst Assignment_Content_DataRpst;

        IVideo_Content_DataRpst Video_Content_DataRpst;

        IVideo_Language_DataRpst Video_Language_DataRpst;

        ICoachingStrategy_DataRpst CoachingStrategy_DataRpst;

        IVideo_SelectionList_DataRpst Video_SelectionList_DataRpst;


        public VcwService(ISunnetLog log,
            IFile fileHelper,
            IEmailSender emailSender,
            IEncrypt encrypt,
            IAssignmentRpst assignmentRpst,
            IAssignmentContentRpst assignmentContentRpst,
            IAssignmentContextRpst assignmentContextRpst,
            IAssignmentFileRpst assignmentFileRpst,
            IAssignmentWatchFileRpst assignmentWatchFileRpst,
            IAssignmentUploadTypeRpst assignmentUploadTypeRpst,
            IAssignmentStrategyRpst assignmentStrategyRpst,
            IAssignmentReportRpst assignmentReportRpst,
            IVIPDocumentRpst vipDocumentRpst,
            IFileRpst fileRpst,
            IFileSelectionRpst fileSelectionRpst,
            IFileSharedRpst fileSharedRpst,
            IFileContentRpst fileContentRpst,
            IFileStrategyRpst fileStrategyRpst,
            ITeacherRpst_V teacherRpst,
            IUserRpst_V userRpst,
            IUploadTypeRpst uploadTypeRpst,
            ISessionRpst sessionRpst,
            IWaveRpst waveRpst,
            IContext_DataRpst contextRpst,
            IAssignment_Content_DataRpst assignment_Content_DataRpst,
            IVideo_Content_DataRpst video_Content_DataRpst,
            IVideo_Language_DataRpst video_Language_DataRpst,
            ICoachingStrategy_DataRpst coachingStrategy_DataRpst,
            IVideo_SelectionList_DataRpst video_SelectionList_DataRpst,
            IUnitOfWork unit)
            : base(log, fileHelper, emailSender, encrypt)
        {
            AssignmentRpst = assignmentRpst;
            AssignmentContentRpst = assignmentContentRpst;
            AssignmentContextRpst = assignmentContextRpst;
            AssignmentFileRpst = assignmentFileRpst;
            AssignmentWatchFileRpst = assignmentWatchFileRpst;
            AssignmentUploadTypeRpst = assignmentUploadTypeRpst;
            AssignmentStrategyRpst = assignmentStrategyRpst;
            AssignmentReportRpst = assignmentReportRpst;
            VIPDocumentRpst = vipDocumentRpst;
            FileRpst = fileRpst;
            FileSelectionRpst = fileSelectionRpst;
            FileSharedRpst = fileSharedRpst;
            FileContentRpst = fileContentRpst;
            FileStrategyRpst = fileStrategyRpst;
            TeacherRpst = teacherRpst;
            UserRpst = userRpst;
            UploadTypeRpst = uploadTypeRpst;
            SessionRpst = sessionRpst;
            WaveRpst = waveRpst;
            Context_DataRpst = contextRpst;
            Assignment_Content_DataRpst = assignment_Content_DataRpst;
            Video_Content_DataRpst = video_Content_DataRpst;
            Video_Language_DataRpst = video_Language_DataRpst;
            CoachingStrategy_DataRpst = coachingStrategy_DataRpst;
            Video_SelectionList_DataRpst = video_SelectionList_DataRpst;
            UnitOfWork = unit;
        }


        public IQueryable<AssignmentEntity> Assignments { get { return AssignmentRpst.Entities; } }

        public IQueryable<AssignmentContentEntity> AssignmentContents { get { return AssignmentContentRpst.Entities; } }

        public IQueryable<AssignmentContextEntity> AssignmentContexts { get { return AssignmentContextRpst.Entities; } }

        public IQueryable<AssignmentFileEntity> AssignmentFiles { get { return AssignmentFileRpst.Entities; } }

        public IQueryable<AssignmentWatchFileEntity> AssignmentWatchFiles { get { return AssignmentWatchFileRpst.Entities; } }

        public IQueryable<AssignmentUploadTypeEntity> AssignmentUploadTypes { get { return AssignmentUploadTypeRpst.Entities; } }

        public IQueryable<AssignmentStrategyEntity> AssignmentStrategies { get { return AssignmentStrategyRpst.Entities; } }

        public IQueryable<AssignmentReportEntity> AssignmentReports { get { return AssignmentReportRpst.Entities; } }

        public IQueryable<VIPDocumentEntity> VIPDocuments { get { return VIPDocumentRpst.Entities; } }

        public IQueryable<Vcw_FileEntity> Files { get { return FileRpst.Entities; } }

        public IQueryable<FileSelectionEntity> FileSelections { get { return FileSelectionRpst.Entities; } }

        public IQueryable<FileSharedEntity> FileShareds { get { return FileSharedRpst.Entities; } }

        public IQueryable<FileContentEntity> FileContents { get { return FileContentRpst.Entities; } }

        public IQueryable<FileStrategyEntity> FileStrategies { get { return FileStrategyRpst.Entities; } }

        public IQueryable<UploadTypeEntity> UploadTypes { get { return UploadTypeRpst.Entities; } }

        public IQueryable<SessionEntity> Sessions { get { return SessionRpst.Entities; } }

        public IQueryable<WaveEntity> Waves { get { return WaveRpst.Entities; } }

        public IQueryable<Context_DataEntity> Context_Datas { get { return Context_DataRpst.Entities; } }

        public IQueryable<Assignment_Content_DataEntity> Assignment_Content_Datas { get { return Assignment_Content_DataRpst.Entities; } }

        public IQueryable<Video_Content_DataEntity> Video_Content_Datas { get { return Video_Content_DataRpst.Entities; } }

        public IQueryable<Video_Language_DataEntity> Video_Language_Datas { get { return Video_Language_DataRpst.Entities; } }

        public IQueryable<CoachingStrategy_DataEntity> CoachingStrategy_Datas { get { return CoachingStrategy_DataRpst.Entities; } }

        public IQueryable<Video_SelectionList_DataEntity> Video_SelectionList_Datas { get { return Video_SelectionList_DataRpst.Entities; } }

        public IQueryable<V_TeacherEntity> Teachers { get { return TeacherRpst.Entities; } }

        public IQueryable<V_UserEntity> Users { get { return UserRpst.Entities; } }

        public AssignmentEntity NewAssignmentEntity()
        {
            return AssignmentRpst.Create();
        }

        public Vcw_FileEntity NewFileEntity()
        {
            return FileRpst.Create();
        }

        public OperationResult AddAssignment(AssignmentEntity Assignment, bool isSave = true)
        {
            OperationResult result = new OperationResult(OperationResultType.Success);
            try
            {
                AssignmentRpst.Insert(Assignment, isSave);
            }
            catch (Exception ex)
            {
                result = ResultError(ex);
            }
            return result;
        }

        public OperationResult AddAssignment(List<AssignmentEntity> Assignments)
        {
            OperationResult result = new OperationResult(OperationResultType.Success);
            try
            {
                AssignmentRpst.Insert(Assignments);
            }
            catch (Exception ex)
            {
                result = ResultError(ex);
            }
            return result;
        }

        public OperationResult DeleteAssignment(AssignmentEntity Assignment, bool isSave = true)
        {
            OperationResult result = new OperationResult(OperationResultType.Success);
            try
            {
                Assignment.IsDelete = true;
                AssignmentRpst.Update(Assignment, isSave);
            }
            catch (Exception ex)
            {
                result = ResultError(ex);
            }
            return result;
        }

        public OperationResult DeleteAssignment(List<AssignmentEntity> Assignments)
        {
            OperationResult result = new OperationResult(OperationResultType.Success);
            try
            {
                foreach (AssignmentEntity item in Assignments)
                {
                    item.IsDelete = true;
                    AssignmentRpst.Update(item);
                }

            }
            catch (Exception ex)
            {
                result = ResultError(ex);
            }
            return result;
        }

        public AssignmentEntity GetAssignment(int id)
        {
            return AssignmentRpst.GetByKey(id);
        }

        public OperationResult UpdateAssignment(AssignmentEntity Assignment)
        {
            OperationResult result = new OperationResult(OperationResultType.Success);
            try
            {
                AssignmentRpst.Update(Assignment);
            }
            catch (Exception ex)
            {
                result = ResultError(ex);
            }
            return result;
        }


        public OperationResult AddAssignmentContent(List<AssignmentContentEntity> AssignmentContents)
        {
            OperationResult result = new OperationResult(OperationResultType.Success);
            try
            {
                AssignmentContentRpst.Insert(AssignmentContents);
            }
            catch (Exception ex)
            {
                result = ResultError(ex);
            }
            return result;
        }

        public OperationResult DeleteAssignmentContent(IEnumerable<AssignmentContentEntity> AssignmentContents, bool isSave = true)
        {
            OperationResult result = new OperationResult(OperationResultType.Success);
            try
            {
                AssignmentContentRpst.Delete(AssignmentContents, isSave);
            }
            catch (Exception ex)
            {
                result = ResultError(ex);
            }
            return result;
        }

        public OperationResult AddAssignmentContext(List<AssignmentContextEntity> AssignmentContexts)
        {
            OperationResult result = new OperationResult(OperationResultType.Success);
            try
            {
                AssignmentContextRpst.Insert(AssignmentContexts);
            }
            catch (Exception ex)
            {
                result = ResultError(ex);
            }
            return result;
        }

        public OperationResult DeleteAssignmentContext(IEnumerable<AssignmentContextEntity> AssignmentContexts, bool isSave = true)
        {
            OperationResult result = new OperationResult(OperationResultType.Success);
            try
            {
                AssignmentContextRpst.Delete(AssignmentContexts, isSave);
            }
            catch (Exception ex)
            {
                result = ResultError(ex);
            }
            return result;
        }


        public OperationResult AddAssignmentFile(List<AssignmentFileEntity> AssignmentFiles)
        {
            OperationResult result = new OperationResult(OperationResultType.Success);
            try
            {
                AssignmentFileRpst.Insert(AssignmentFiles);
            }
            catch (Exception ex)
            {
                result = ResultError(ex);
            }
            return result;
        }

        public OperationResult DeleteAssignmentFile(IEnumerable<AssignmentFileEntity> AssignmentFiles, bool isSave = true)
        {
            OperationResult result = new OperationResult(OperationResultType.Success);
            try
            {
                AssignmentFileRpst.Delete(AssignmentFiles, isSave);
            }
            catch (Exception ex)
            {
                result = ResultError(ex);
            }
            return result;
        }

        public OperationResult AddAssignmentWatchFile(List<AssignmentWatchFileEntity> AssignmentWatchFiles)
        {
            OperationResult result = new OperationResult(OperationResultType.Success);
            try
            {
                AssignmentWatchFileRpst.Insert(AssignmentWatchFiles);
            }
            catch (Exception ex)
            {
                result = ResultError(ex);
            }
            return result;
        }

        public OperationResult DeleteAssignmentWatchFile(IEnumerable<AssignmentWatchFileEntity> AssignmentWatchFiles, bool isSave = true)
        {
            OperationResult result = new OperationResult(OperationResultType.Success);
            try
            {
                AssignmentWatchFileRpst.Delete(AssignmentWatchFiles, isSave);
            }
            catch (Exception ex)
            {
                result = ResultError(ex);
            }
            return result;
        }


        public OperationResult AddAssignmentUploadType(List<AssignmentUploadTypeEntity> AssignmentUploadTypes)
        {
            OperationResult result = new OperationResult(OperationResultType.Success);
            try
            {
                AssignmentUploadTypeRpst.Insert(AssignmentUploadTypes);
            }
            catch (Exception ex)
            {
                result = ResultError(ex);
            }
            return result;
        }

        public OperationResult DeleteAssignmentUploadType(IEnumerable<AssignmentUploadTypeEntity> AssignmentUploadTypes, bool isSave = true)
        {
            OperationResult result = new OperationResult(OperationResultType.Success);
            try
            {
                AssignmentUploadTypeRpst.Delete(AssignmentUploadTypes, isSave);
            }
            catch (Exception ex)
            {
                result = ResultError(ex);
            }
            return result;
        }

        public OperationResult AddAssignmentStrategy(List<AssignmentStrategyEntity> AssignmentStrategies)
        {
            OperationResult result = new OperationResult(OperationResultType.Success);
            try
            {
                AssignmentStrategyRpst.Insert(AssignmentStrategies);
            }
            catch (Exception ex)
            {
                result = ResultError(ex);
            }
            return result;
        }

        public OperationResult DeleteAssignmentStrategy(IEnumerable<AssignmentStrategyEntity> AssignmentStrategies, bool isSave = true)
        {
            OperationResult result = new OperationResult(OperationResultType.Success);
            try
            {
                AssignmentStrategyRpst.Delete(AssignmentStrategies, isSave);
            }
            catch (Exception ex)
            {
                result = ResultError(ex);
            }
            return result;
        }

        public OperationResult AddAssignmentReport(List<AssignmentReportEntity> AssignmentReports)
        {
            OperationResult result = new OperationResult(OperationResultType.Success);
            try
            {
                AssignmentReportRpst.Insert(AssignmentReports);
            }
            catch (Exception ex)
            {
                result = ResultError(ex);
            }
            return result;
        }

        public OperationResult DeleteAssignmentReport(IEnumerable<AssignmentReportEntity> AssignmentReports, bool isSave = true)
        {
            OperationResult result = new OperationResult(OperationResultType.Success);
            try
            {
                AssignmentReportRpst.Delete(AssignmentReports, isSave);
            }
            catch (Exception ex)
            {
                result = ResultError(ex);
            }
            return result;
        }

        public OperationResult AddVIPDocument(List<VIPDocumentEntity> VIPDocuments)
        {
            OperationResult result = new OperationResult(OperationResultType.Success);
            try
            {
                VIPDocumentRpst.Insert(VIPDocuments);
            }
            catch (Exception ex)
            {
                result = ResultError(ex);
            }
            return result;
        }

        public OperationResult DeleteVIPDocument(IEnumerable<VIPDocumentEntity> VIPDocuments, bool isSave = true)
        {
            OperationResult result = new OperationResult(OperationResultType.Success);
            try
            {
                VIPDocumentRpst.Delete(VIPDocuments, isSave);
            }
            catch (Exception ex)
            {
                result = ResultError(ex);
            }
            return result;
        }



        public OperationResult AddFile(Vcw_FileEntity File)
        {
            OperationResult result = new OperationResult(OperationResultType.Success);
            try
            {
                FileRpst.Insert(File);
            }
            catch (Exception ex)
            {
                result = ResultError(ex);
            }
            return result;
        }

        public OperationResult AddFile(List<Vcw_FileEntity> Files)
        {
            OperationResult result = new OperationResult(OperationResultType.Success);
            try
            {
                FileRpst.Insert(Files);
            }
            catch (Exception ex)
            {
                result = ResultError(ex);
            }
            return result;
        }


        public OperationResult DeleteFile(Vcw_FileEntity File, bool isSave = true)
        {
            OperationResult result = new OperationResult(OperationResultType.Success);
            try
            {
                File.IsDelete = true;
                FileRpst.Update(File, isSave);
            }
            catch (Exception ex)
            {
                result = ResultError(ex);
            }
            return result;
        }

        public OperationResult DeleteFile(IEnumerable<Vcw_FileEntity> Files)
        {
            OperationResult result = new OperationResult(OperationResultType.Success);
            try
            {
                foreach (Vcw_FileEntity item in Files)
                {
                    item.IsDelete = true;
                    FileRpst.Update(item);
                }
            }
            catch (Exception ex)
            {
                result = ResultError(ex);
            }
            return result;
        }


        public Vcw_FileEntity GetFile(int id)
        {
            return FileRpst.GetByKey(id);
        }

        public OperationResult UpdateFile(Vcw_FileEntity File)
        {
            OperationResult result = new OperationResult(OperationResultType.Success);
            try
            {
                FileRpst.Update(File);
            }
            catch (Exception ex)
            {
                result = ResultError(ex);
            }
            return result;
        }


        public OperationResult AddFileSelection(List<FileSelectionEntity> FileSelections)
        {
            OperationResult result = new OperationResult(OperationResultType.Success);
            try
            {
                FileSelectionRpst.Insert(FileSelections);
            }
            catch (Exception ex)
            {
                result = ResultError(ex);
            }
            return result;
        }

        public OperationResult DeleteFileSelection(IEnumerable<FileSelectionEntity> FileSelections, bool isSave = true)
        {
            OperationResult result = new OperationResult(OperationResultType.Success);
            try
            {
                FileSelectionRpst.Delete(FileSelections, isSave);
            }
            catch (Exception ex)
            {
                result = ResultError(ex);
            }
            return result;
        }


        public OperationResult AddFileShared(List<FileSharedEntity> FileShareds)
        {
            OperationResult result = new OperationResult(OperationResultType.Success);
            try
            {
                FileSharedRpst.Insert(FileShareds);
            }
            catch (Exception ex)
            {
                result = ResultError(ex);
            }
            return result;
        }

        public OperationResult DeleteFileShared(IEnumerable<FileSharedEntity> FileShareds, bool isSave = true)
        {
            OperationResult result = new OperationResult(OperationResultType.Success);
            try
            {
                FileSharedRpst.Delete(FileShareds, isSave);
            }
            catch (Exception ex)
            {
                result = ResultError(ex);
            }
            return result;
        }

        public FileSharedEntity GetFileSharedEntity(int id)
        {
            return FileSharedRpst.GetByKey(id);
        }

        public void ChangeStatus(ChangeStatusEnum method, int assignmentId, FileStatus status)
        {
            AssignmentRpst.ChangeStatus(method, assignmentId, status);
        }

        public void ChangeStatus(List<int> assignmentIds)
        {
            AssignmentRpst.ChangeStatus(assignmentIds);
        }

        public OperationResult AddFileContent(List<FileContentEntity> FileContents)
        {
            OperationResult result = new OperationResult(OperationResultType.Success);
            try
            {
                FileContentRpst.Insert(FileContents);
            }
            catch (Exception ex)
            {
                result = ResultError(ex);
            }
            return result;
        }

        public OperationResult DeleteFileContent(IEnumerable<FileContentEntity> FileContents, bool isSave = true)
        {
            OperationResult result = new OperationResult(OperationResultType.Success);
            try
            {
                FileContentRpst.Delete(FileContents, isSave);
            }
            catch (Exception ex)
            {
                result = ResultError(ex);
            }
            return result;
        }

        public OperationResult AddFileStrategy(List<FileStrategyEntity> FileStrategies)
        {
            OperationResult result = new OperationResult(OperationResultType.Success);
            try
            {
                FileStrategyRpst.Insert(FileStrategies);
            }
            catch (Exception ex)
            {
                result = ResultError(ex);
            }
            return result;
        }

        public OperationResult DeleteFileStrategy(IEnumerable<FileStrategyEntity> FileStrategies, bool isSave = true)
        {
            OperationResult result = new OperationResult(OperationResultType.Success);
            try
            {
                FileStrategyRpst.Delete(FileStrategies, isSave);
            }
            catch (Exception ex)
            {
                result = ResultError(ex);
            }
            return result;
        }

        public UploadTypeEntity GetUploadType(int id)
        {
            return UploadTypeRpst.GetByKey(id);
        }

        public OperationResult AddUploadType(UploadTypeEntity UploadType)
        {
            OperationResult result = new OperationResult(OperationResultType.Success);
            try
            {
                UploadTypeRpst.Insert(UploadType);
            }
            catch (Exception ex)
            {
                result = ResultError(ex);
            }
            return result;
        }

        public OperationResult UpdateUploadType(UploadTypeEntity UploadType)
        {
            OperationResult result = new OperationResult(OperationResultType.Success);
            try
            {
                UploadTypeRpst.Update(UploadType);
            }
            catch (Exception ex)
            {
                result = ResultError(ex);
            }
            return result;
        }

        public SessionEntity GetSession(int id)
        {
            return SessionRpst.GetByKey(id);
        }

        public OperationResult AddSession(SessionEntity Session)
        {
            OperationResult result = new OperationResult(OperationResultType.Success);
            try
            {
                SessionRpst.Insert(Session);
            }
            catch (Exception ex)
            {
                result = ResultError(ex);
            }
            return result;
        }

        public OperationResult UpdateSession(SessionEntity Session)
        {
            OperationResult result = new OperationResult(OperationResultType.Success);
            try
            {
                SessionRpst.Update(Session);
            }
            catch (Exception ex)
            {
                result = ResultError(ex);
            }
            return result;
        }

        public WaveEntity GetWave(int id)
        {
            return WaveRpst.GetByKey(id);
        }

        public OperationResult AddWave(WaveEntity Wave)
        {
            OperationResult result = new OperationResult(OperationResultType.Success);
            try
            {
                WaveRpst.Insert(Wave);
            }
            catch (Exception ex)
            {
                result = ResultError(ex);
            }
            return result;
        }

        public OperationResult UpdateWave(WaveEntity Wave)
        {
            OperationResult result = new OperationResult(OperationResultType.Success);
            try
            {
                WaveRpst.Update(Wave);
            }
            catch (Exception ex)
            {
                result = ResultError(ex);
            }
            return result;
        }

        public Context_DataEntity GetContext(int id)
        {
            return Context_DataRpst.GetByKey(id);
        }

        public OperationResult AddContext(Context_DataEntity Context)
        {
            OperationResult result = new OperationResult(OperationResultType.Success);
            try
            {
                Context_DataRpst.Insert(Context);
            }
            catch (Exception ex)
            {
                result = ResultError(ex);
            }
            return result;
        }

        public OperationResult UpdateContext(Context_DataEntity Context)
        {
            OperationResult result = new OperationResult(OperationResultType.Success);
            try
            {
                Context_DataRpst.Update(Context);
            }
            catch (Exception ex)
            {
                result = ResultError(ex);
            }
            return result;
        }

        public Assignment_Content_DataEntity GetAssignmentContentData(int id)
        {
            return Assignment_Content_DataRpst.GetByKey(id);
        }

        public OperationResult AddAssignmentContentData(Assignment_Content_DataEntity Assignment_Content_Data)
        {
            OperationResult result = new OperationResult(OperationResultType.Success);
            try
            {
                Assignment_Content_DataRpst.Insert(Assignment_Content_Data);
            }
            catch (Exception ex)
            {
                result = ResultError(ex);
            }
            return result;
        }

        public OperationResult UpdateAssignmentContentData(Assignment_Content_DataEntity Assignment_Content_Data)
        {
            OperationResult result = new OperationResult(OperationResultType.Success);
            try
            {
                Assignment_Content_DataRpst.Update(Assignment_Content_Data);
            }
            catch (Exception ex)
            {
                result = ResultError(ex);
            }
            return result;
        }

        public Video_Content_DataEntity GetVideoContentData(int id)
        {
            return Video_Content_DataRpst.GetByKey(id);
        }

        public OperationResult AddVideoContentData(Video_Content_DataEntity Video_Content_Data)
        {
            OperationResult result = new OperationResult(OperationResultType.Success);
            try
            {
                Video_Content_DataRpst.Insert(Video_Content_Data);
            }
            catch (Exception ex)
            {
                result = ResultError(ex);
            }
            return result;
        }

        public OperationResult UpdateVideoContentData(Video_Content_DataEntity Video_Content_Data)
        {
            OperationResult result = new OperationResult(OperationResultType.Success);
            try
            {
                Video_Content_DataRpst.Update(Video_Content_Data);
            }
            catch (Exception ex)
            {
                result = ResultError(ex);
            }
            return result;
        }

        public Video_Language_DataEntity GetVideoLanguageData(int id)
        {
            return Video_Language_DataRpst.GetByKey(id);
        }

        public OperationResult AddVideoLanguageData(Video_Language_DataEntity Video_Language_Data)
        {
            OperationResult result = new OperationResult(OperationResultType.Success);
            try
            {
                Video_Language_DataRpst.Insert(Video_Language_Data);
            }
            catch (Exception ex)
            {
                result = ResultError(ex);
            }
            return result;
        }

        public OperationResult UpdateVideoLanguageData(Video_Language_DataEntity Video_Language_Data)
        {
            OperationResult result = new OperationResult(OperationResultType.Success);
            try
            {
                Video_Language_DataRpst.Update(Video_Language_Data);
            }
            catch (Exception ex)
            {
                result = ResultError(ex);
            }
            return result;
        }

        public CoachingStrategy_DataEntity GetCoachingStrategyData(int id)
        {
            return CoachingStrategy_DataRpst.GetByKey(id);
        }

        public OperationResult AddCoachingStrategyData(CoachingStrategy_DataEntity CoachingStrategy_Data)
        {
            OperationResult result = new OperationResult(OperationResultType.Success);
            try
            {
                CoachingStrategy_DataRpst.Insert(CoachingStrategy_Data);
            }
            catch (Exception ex)
            {
                result = ResultError(ex);
            }
            return result;
        }

        public OperationResult UpdateCoachingStrategyData(CoachingStrategy_DataEntity CoachingStrategy_Data)
        {
            OperationResult result = new OperationResult(OperationResultType.Success);
            try
            {
                CoachingStrategy_DataRpst.Update(CoachingStrategy_Data);
            }
            catch (Exception ex)
            {
                result = ResultError(ex);
            }
            return result;
        }

        public Video_SelectionList_DataEntity GetVideoSelectionListData(int id)
        {
            return Video_SelectionList_DataRpst.GetByKey(id);
        }

        public OperationResult AddVideoSelectionListData(Video_SelectionList_DataEntity Video_SelectionList_Data)
        {
            OperationResult result = new OperationResult(OperationResultType.Success);
            try
            {
                Video_SelectionList_DataRpst.Insert(Video_SelectionList_Data);
            }
            catch (Exception ex)
            {
                result = ResultError(ex);
            }
            return result;
        }

        public OperationResult UpdateVideoSelectionListData(Video_SelectionList_DataEntity Video_SelectionList_Data)
        {
            OperationResult result = new OperationResult(OperationResultType.Success);
            try
            {
                Video_SelectionList_DataRpst.Update(Video_SelectionList_Data);
            }
            catch (Exception ex)
            {
                result = ResultError(ex);
            }
            return result;
        }
    }
}
