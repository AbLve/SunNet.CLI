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
using Sunnet.Framework.Core.Tool;
using Sunnet.Cli.Core.Vcw.Enums;

namespace Sunnet.Cli.Core.Vcw
{
    public interface IVcwContract
    {
        #region

        IQueryable<AssignmentEntity> Assignments { get; }

        IQueryable<AssignmentContentEntity> AssignmentContents { get; }

        IQueryable<AssignmentContextEntity> AssignmentContexts { get; }

        IQueryable<AssignmentFileEntity> AssignmentFiles { get; }

        IQueryable<AssignmentWatchFileEntity> AssignmentWatchFiles { get; }

        IQueryable<AssignmentUploadTypeEntity> AssignmentUploadTypes { get; }

        IQueryable<VIPDocumentEntity> VIPDocuments { get; }

        IQueryable<Vcw_FileEntity> Files { get; }

        IQueryable<FileSelectionEntity> FileSelections { get; }

        IQueryable<FileSharedEntity> FileShareds { get; }

        IQueryable<FileContentEntity> FileContents { get; }

        IQueryable<FileStrategyEntity> FileStrategies { get; }

        IQueryable<UploadTypeEntity> UploadTypes { get; }

        IQueryable<SessionEntity> Sessions { get; }

        IQueryable<WaveEntity> Waves { get; }

        IQueryable<Context_DataEntity> Context_Datas { get; }

        IQueryable<Assignment_Content_DataEntity> Assignment_Content_Datas { get; }

        IQueryable<Video_Content_DataEntity> Video_Content_Datas { get; }

        IQueryable<Video_Language_DataEntity> Video_Language_Datas { get; }

        IQueryable<CoachingStrategy_DataEntity> CoachingStrategy_Datas { get; }

        IQueryable<Video_SelectionList_DataEntity> Video_SelectionList_Datas { get; }

        IQueryable<V_TeacherEntity> Teachers { get; }

        IQueryable<V_UserEntity> Users { get; }

        #endregion

        AssignmentEntity NewAssignmentEntity();
        Vcw_FileEntity NewFileEntity();


        OperationResult AddAssignment(AssignmentEntity Assignment, bool isSave = true);

        OperationResult AddAssignment(List<AssignmentEntity> Assignments);

        OperationResult DeleteAssignment(AssignmentEntity Assignment, bool isSave = true);

        OperationResult DeleteAssignment(List<AssignmentEntity> Assignments);

        AssignmentEntity GetAssignment(int id);

        OperationResult UpdateAssignment(AssignmentEntity Assignment);


        OperationResult AddAssignmentContent(List<AssignmentContentEntity> AssignmentContents);

        OperationResult DeleteAssignmentContent(IEnumerable<AssignmentContentEntity> AssignmentContents, bool isSave = true);


        OperationResult AddAssignmentContext(List<AssignmentContextEntity> AssignmentContexts);

        OperationResult DeleteAssignmentContext(IEnumerable<AssignmentContextEntity> AssignmentContexts, bool isSave = true);


        OperationResult AddAssignmentFile(List<AssignmentFileEntity> AssignmentFiles);

        OperationResult DeleteAssignmentFile(IEnumerable<AssignmentFileEntity> AssignmentFiles, bool isSave = true);

        OperationResult AddAssignmentWatchFile(List<AssignmentWatchFileEntity> AssignmentWatchFiles);

        OperationResult DeleteAssignmentWatchFile(IEnumerable<AssignmentWatchFileEntity> AssignmentWatchFiles, bool isSave = true);

        OperationResult AddVIPDocument(List<VIPDocumentEntity> VIPDocuments);

        OperationResult DeleteVIPDocument(IEnumerable<VIPDocumentEntity> VIPDocuments, bool isSave = true);


        OperationResult AddAssignmentUploadType(List<AssignmentUploadTypeEntity> AssignmentUploadTypes);

        OperationResult DeleteAssignmentUploadType(IEnumerable<AssignmentUploadTypeEntity> AssignmentUploadTypes, bool isSave = true);

        OperationResult AddAssignmentStrategy(List<AssignmentStrategyEntity> AssignmentStrategies);

        OperationResult DeleteAssignmentStrategy(IEnumerable<AssignmentStrategyEntity> AssignmentStrategies, bool isSave = true);

        OperationResult AddAssignmentReport(List<AssignmentReportEntity> AssignmentReports);

        OperationResult DeleteAssignmentReport(IEnumerable<AssignmentReportEntity> AssignmentReports, bool isSave = true);


        OperationResult AddFile(Vcw_FileEntity File);

        OperationResult AddFile(List<Vcw_FileEntity> Files);

        OperationResult DeleteFile(Vcw_FileEntity File, bool isSave = true);

        OperationResult DeleteFile(IEnumerable<Vcw_FileEntity> Files);


        Vcw_FileEntity GetFile(int id);

        OperationResult UpdateFile(Vcw_FileEntity File);


        OperationResult AddFileSelection(List<FileSelectionEntity> FileSelections);

        OperationResult DeleteFileSelection(IEnumerable<FileSelectionEntity> FileSelections, bool isSave = true);


        OperationResult AddFileShared(List<FileSharedEntity> FileShareds);

        OperationResult DeleteFileShared(IEnumerable<FileSharedEntity> FileShareds, bool isSave = true);

        FileSharedEntity GetFileSharedEntity(int id);

        void ChangeStatus(ChangeStatusEnum method, int assignmentId, FileStatus status);

        void ChangeStatus(List<int> assignmentIds);

        OperationResult AddFileContent(List<FileContentEntity> FileContents);

        OperationResult DeleteFileContent(IEnumerable<FileContentEntity> FileContents, bool isSave = true);

        UploadTypeEntity GetUploadType(int id);

        OperationResult AddUploadType(UploadTypeEntity UplaodType);

        OperationResult UpdateUploadType(UploadTypeEntity UplaodType);

        SessionEntity GetSession(int id);

        OperationResult AddSession(SessionEntity Session);

        OperationResult UpdateSession(SessionEntity Session);

        WaveEntity GetWave(int id);

        OperationResult AddWave(WaveEntity Wave);

        OperationResult UpdateWave(WaveEntity Wave);

        Context_DataEntity GetContext(int id);

        OperationResult AddContext(Context_DataEntity Context);

        OperationResult UpdateContext(Context_DataEntity Context);

        Assignment_Content_DataEntity GetAssignmentContentData(int id);

        OperationResult AddAssignmentContentData(Assignment_Content_DataEntity Assignment_Content_Data);

        OperationResult UpdateAssignmentContentData(Assignment_Content_DataEntity Assignment_Content_Data);

        Video_Content_DataEntity GetVideoContentData(int id);

        OperationResult AddVideoContentData(Video_Content_DataEntity Video_Content_Data);

        OperationResult UpdateVideoContentData(Video_Content_DataEntity Video_Content_Data);

        Video_Language_DataEntity GetVideoLanguageData(int id);

        OperationResult AddVideoLanguageData(Video_Language_DataEntity Video_Language_Data);

        OperationResult UpdateVideoLanguageData(Video_Language_DataEntity Video_Language_Data);

        CoachingStrategy_DataEntity GetCoachingStrategyData(int id);

        OperationResult AddCoachingStrategyData(CoachingStrategy_DataEntity CoachingStrategy_Data);

        OperationResult UpdateCoachingStrategyData(CoachingStrategy_DataEntity CoachingStrategy_Data);

        Video_SelectionList_DataEntity GetVideoSelectionListData(int id);

        OperationResult AddVideoSelectionListData(Video_SelectionList_DataEntity Video_SelectionList_Data);

        OperationResult UpdateVideoSelectionListData(Video_SelectionList_DataEntity VideoSelectionList_Data);

        OperationResult AddFileStrategy(List<FileStrategyEntity> FileStrategies);

        OperationResult DeleteFileStrategy(IEnumerable<FileStrategyEntity> FileStrategies, bool isSave = true);
    }
}
