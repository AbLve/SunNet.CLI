using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
/**************************************************************************
 * Developer: 		JackZhang
 * Computer:		JACKZ
 * Domain:			Jackz
 * CreatedOn:		2014/8/1 1:00:30
 * Description:		Please input class summary
 * Version History:	Created,2014/8/1 1:00:30
 * 
 * 
 **************************************************************************/
using StructureMap;
using StructureMap.Pipeline;
using Sunnet.Cli.Core.Ade;
using Sunnet.Cli.Core.Ade.Interfaces;
using Sunnet.Cli.Core.Cec;
using Sunnet.Cli.Core.Cec.Interfaces;
using Sunnet.Cli.Core.Classes;
using Sunnet.Cli.Core.Classes.Interfaces;
using Sunnet.Cli.Core.Classrooms;
using Sunnet.Cli.Core.Classrooms.Interfaces;
using Sunnet.Cli.Core.Communities;
using Sunnet.Cli.Core.Communities.Interfaces;
using Sunnet.Cli.Core.Cot;
using Sunnet.Cli.Core.Cot.Interfaces;
using Sunnet.Cli.Core.Cpalls;
using Sunnet.Cli.Core.Cpalls.Interfaces;
using Sunnet.Cli.Core.MasterData.Interfaces;
using Sunnet.Cli.Core.Observable;
using Sunnet.Cli.Core.Observable.Interfaces;
using Sunnet.Cli.Core.Reports.Interfaces;
using Sunnet.Cli.Core.Schools;
using Sunnet.Cli.Core.Schools.Interfaces;
using Sunnet.Cli.Core.Students;
using Sunnet.Cli.Core.Students.Interfaces;
using Sunnet.Cli.Core.TRSClasses;
using Sunnet.Cli.Core.TRSClasses.Interfaces;
using Sunnet.Cli.Core.Users;
using Sunnet.Cli.Core.Users.Interfaces;
using Sunnet.Framework.Core.Base;
using Sunnet.Framework.EmailSender;
using Sunnet.Framework.Encrypt;
using Sunnet.Framework.File;
using Sunnet.Framework.Log;
using Sunnet.Cli.Core.MasterData;
using Sunnet.Cli.Core.Permission;
using Sunnet.Cli.Core.Permission.Interfaces;
using Sunnet.Cli.Core.DataProcess;
using Sunnet.Cli.Core.DataProcess.Interfaces;
using Sunnet.Cli.Core.Log;
using Sunnet.Cli.Core.Log.Interfaces;
using Sunnet.Cli.Core.Vcw;
using Sunnet.Cli.Core.Vcw.Interfaces;
using Sunnet.Cli.Core.Reports;
using Sunnet.Cli.Core.Trs;
using Sunnet.Cli.Core.Trs.Interfaces;
using Sunnet.Cli.Core.StatusTracking;
using Sunnet.Cli.Core.StatusTracking.Interfaces;
using Sunnet.Cli.Core.BUP;
using Sunnet.Cli.Core.BUP.Interfaces;
using Sunnet.Cli.Core.CAC;
using Sunnet.Cli.Core.CAC.Interfaces;
using Sunnet.Cli.Core.Export;
using Sunnet.Cli.Core.Export.Interfaces;
using Sunnet.Cli.Core.Tsds;
using Sunnet.Cli.Core.Tsds.Interfaces;
using Sunnet.Cli.Core.Practices;
using Sunnet.Cli.Core.Practices.Interfaces;
using Sunnet.Cli.Core.UpdateClusters;
using Sunnet.Cli.Core.UpdateClusters.Interfaces;

namespace Sunnet.Cli.Core
{
    public static class DomainFacade
    {
        #region CreateUserService
        public static IUserContract CreateUserService(EFUnitOfWorkContext unit = null)
        {
            if (unit == null)
                unit = new EFUnitOfWorkContext();
            IDictionary<string, object> dic = new Dictionary<string, object>();
            dic.Add("unit", unit);
            IUserContract mgr = new UserService(
                ObjectFactory.GetInstance<ISunnetLog>(),
                ObjectFactory.GetInstance<IFile>(),
                ObjectFactory.GetInstance<IEmailSender>(),
                ObjectFactory.GetInstance<IEncrypt>(),
                ObjectFactory.GetInstance<IUserBaseRpst>(new ExplicitArguments(dic)),
                ObjectFactory.GetInstance<ITeacherRpst>(new ExplicitArguments(dic)),
                ObjectFactory.GetInstance<IPrincipalRpst>(new ExplicitArguments(dic)),
                ObjectFactory.GetInstance<IDisCommunityRpst>(new ExplicitArguments(dic)),
                ObjectFactory.GetInstance<IParentRpst>(new ExplicitArguments(dic)),
                ObjectFactory.GetInstance<IParentStudentRelationRpst>(new ExplicitArguments(dic)),
                 ObjectFactory.GetInstance<IStateWideRpst>(new ExplicitArguments(dic)),
                 ObjectFactory.GetInstance<IAuditorRpst>(new ExplicitArguments(dic)),
                ObjectFactory.GetInstance<IRegistorLogRpst>(new ExplicitArguments(dic)),
                ObjectFactory.GetInstance<IApplicantRpst>(new ExplicitArguments(dic)),
                ObjectFactory.GetInstance<IApplicantContactRpst>(new ExplicitArguments(dic)),
                ObjectFactory.GetInstance<IApplicantEmailRpst>(new ExplicitArguments(dic)),
                ObjectFactory.GetInstance<ITeacherAgeGroupRpst>(new ExplicitArguments(dic)),
                ObjectFactory.GetInstance<ITeacherEquipmentRelationRpst>(new ExplicitArguments(dic)),
                ObjectFactory.GetInstance<ITeacherTransactionRpst>(new ExplicitArguments(dic)),
                ObjectFactory.GetInstance<IAffiliationRpst>(new ExplicitArguments(dic)),
                ObjectFactory.GetInstance<ICertificateRpst>(new ExplicitArguments(dic)),
                ObjectFactory.GetInstance<IPositionRpst>(new ExplicitArguments(dic)),
                ObjectFactory.GetInstance<IYearsInProjectRpst>(new ExplicitArguments(dic)),
                ObjectFactory.GetInstance<IProfessionalDevelopmentRpst>(new ExplicitArguments(dic)),
                ObjectFactory.GetInstance<IVideoCodingRpst>(new ExplicitArguments(dic)),
                ObjectFactory.GetInstance<ICoordCoachRpst>(new ExplicitArguments(dic)),
                ObjectFactory.GetInstance<ICoordCoachEquipmentRpst>(new ExplicitArguments(dic)),
                ObjectFactory.GetInstance<ITeacherRoleRpst>(new ExplicitArguments(dic)),
                ObjectFactory.GetInstance<IUserComSchRelationRpst>(new ExplicitArguments(dic)),
                ObjectFactory.GetInstance<IUserClassRelationRpst>(new ExplicitArguments(dic)),
                ObjectFactory.GetInstance<IIntManaCoachRelationRpst>(new ExplicitArguments(dic)),
                ObjectFactory.GetInstance<IPrincipalRoleRpst>(new ExplicitArguments(dic)),
                ObjectFactory.GetInstance<ICoordCoachRoleRpst>(new ExplicitArguments(dic)),
                unit);
            return mgr;
        }
        #endregion

        #region Ade Assessment
        public static IAdeContract CreateAdeService(AdeUnitOfWorkContext unit = null)
        {
            if (unit == null)
                unit = new AdeUnitOfWorkContext();
            IDictionary<string, object> dic = new Dictionary<string, object>();
            dic.Add("unit", unit);
            var mgr = new AdeService(
                ObjectFactory.GetInstance<ISunnetLog>(),
                ObjectFactory.GetInstance<IFile>(),
                ObjectFactory.GetInstance<IEmailSender>(),
                ObjectFactory.GetInstance<IEncrypt>(),

                ObjectFactory.GetInstance<IAdeLinkRpst>(new ExplicitArguments(dic)),
                ObjectFactory.GetInstance<IAnswerRpst>(new ExplicitArguments(dic)),
                ObjectFactory.GetInstance<IAssessmentRpst>(new ExplicitArguments(dic)),
                ObjectFactory.GetInstance<ICutOffScoreRpst>(new ExplicitArguments(dic)),
                ObjectFactory.GetInstance<IItemBaseRpst>(new ExplicitArguments(dic)),
                ObjectFactory.GetInstance<IMeasureRpst>(new ExplicitArguments(dic)),
                ObjectFactory.GetInstance<ITxkeaLayoutRpst>(new ExplicitArguments(dic)),
                ObjectFactory.GetInstance<ITxkeaBupTaskRpst>(new ExplicitArguments(dic)),
                ObjectFactory.GetInstance<ITxkeaBupLogRpst>(new ExplicitArguments(dic)),
                ObjectFactory.GetInstance<IWaveLogRpst>(new ExplicitArguments(dic)),
                ObjectFactory.GetInstance<IBenchmarkRpst>(new ExplicitArguments(dic)),
                ObjectFactory.GetInstance<IAssessmentReportRpst>(new ExplicitArguments(dic)),
                ObjectFactory.GetInstance<IPercentileRankLookupRpst>(new ExplicitArguments(dic)),
                ObjectFactory.GetInstance<IAssessmentLegendRpst>(new ExplicitArguments(dic)),


                ObjectFactory.GetInstance<IScoreRpst>(new ExplicitArguments(dic)),
                ObjectFactory.GetInstance<IScoreAgeOrWaveBandsRpst>(new ExplicitArguments(dic)),
                ObjectFactory.GetInstance<IScoreMeasureOrDefineCoefficientsRpst>(new ExplicitArguments(dic)),

                unit);
            return mgr;
        }
        public static IAdeDataContract CreateAdeDataService(AdeUnitOfWorkContext unit = null)
        {
            if (unit == null)
                unit = new AdeUnitOfWorkContext();
            IDictionary<string, object> dic = new Dictionary<string, object>();
            dic.Add("unit", unit);
            var mgr = new AdeDataService(
                ObjectFactory.GetInstance<ISunnetLog>(),
                ObjectFactory.GetInstance<IFile>(),
                ObjectFactory.GetInstance<IEmailSender>(),
                ObjectFactory.GetInstance<IEncrypt>(),

                ObjectFactory.GetInstance<IAdeLinkRpst>(new ExplicitArguments(dic)),
                ObjectFactory.GetInstance<IAnswerRpst>(new ExplicitArguments(dic)),
                ObjectFactory.GetInstance<IAssessmentRpst>(new ExplicitArguments(dic)),
                ObjectFactory.GetInstance<ICutOffScoreRpst>(new ExplicitArguments(dic)),
                ObjectFactory.GetInstance<IItemBaseRpst>(new ExplicitArguments(dic)),
                ObjectFactory.GetInstance<IMeasureRpst>(new ExplicitArguments(dic)),
                ObjectFactory.GetInstance<ITxkeaLayoutRpst>(new ExplicitArguments(dic)),
                ObjectFactory.GetInstance<ITxkeaBupTaskRpst>(new ExplicitArguments(dic)),
                ObjectFactory.GetInstance<ITxkeaBupLogRpst>(new ExplicitArguments(dic)),
                ObjectFactory.GetInstance<IBenchmarkRpst>(new ExplicitArguments(dic)),

                 ObjectFactory.GetInstance<IScoreRpst>(new ExplicitArguments(dic)),
                 ObjectFactory.GetInstance<IScoreAgeOrWaveBandsRpst>(new ExplicitArguments(dic)),
                 ObjectFactory.GetInstance<IScoreMeasureOrDefineCoefficientsRpst>(new ExplicitArguments(dic)),

                unit);
            return mgr;
        }
        public static ICpallsContract CreateCpallsService(AdeUnitOfWorkContext unit = null)
        {
            if (unit == null)
                unit = new AdeUnitOfWorkContext();
            IDictionary<string, object> dic = new Dictionary<string, object>();
            dic.Add("unit", unit);
            var mgr = new CpallsService(
                ObjectFactory.GetInstance<ISunnetLog>(),
                ObjectFactory.GetInstance<IFile>(),
                ObjectFactory.GetInstance<IEmailSender>(),
                ObjectFactory.GetInstance<IEncrypt>(),

                ObjectFactory.GetInstance<IStudentAssessmentRpst>(new ExplicitArguments(dic)),
                ObjectFactory.GetInstance<IStudentItemRpst>(new ExplicitArguments(dic)),
                ObjectFactory.GetInstance<IStudentMeasureRpst>(new ExplicitArguments(dic)),
                ObjectFactory.GetInstance<ICpallsSchoolRpst>(new ExplicitArguments(dic)),
                ObjectFactory.GetInstance<ICpallsSchoolMeasureRpst>(new ExplicitArguments(dic)),
                ObjectFactory.GetInstance<ICpallsClassRpst>(new ExplicitArguments(dic)),
                ObjectFactory.GetInstance<ICpallsClassMeasureRpst>(new ExplicitArguments(dic)),
                ObjectFactory.GetInstance<ICpallsStudentGroupRpst>(new ExplicitArguments(dic)),
                  ObjectFactory.GetInstance<IUserShownMeasuresRpst>(new ExplicitArguments(dic)),
                ObjectFactory.GetInstance<IMeasureClassGroupRpst>(new ExplicitArguments(dic)),
                  ObjectFactory.GetInstance<ICustomGroupMyActivityRpst>(new ExplicitArguments(dic)),
                unit);
            return mgr;
        }

        public static ICotContract CreateCotContract(AdeUnitOfWorkContext unit = null)
        {
            if (unit == null)
                unit = new AdeUnitOfWorkContext();
            IDictionary<string, object> dic = new Dictionary<string, object>();
            dic.Add("unit", unit);
            var mgr = new CotService(
                ObjectFactory.GetInstance<ISunnetLog>(),
                ObjectFactory.GetInstance<IFile>(),
                ObjectFactory.GetInstance<IEmailSender>(),
                ObjectFactory.GetInstance<IEncrypt>(),

                ObjectFactory.GetInstance<ICotAssessmentRpst>(new ExplicitArguments(dic)),
                ObjectFactory.GetInstance<ICotAssessmentItemRpst>(new ExplicitArguments(dic)),
                ObjectFactory.GetInstance<ICotAssessmentWaveItemRpst>(new ExplicitArguments(dic)),
                ObjectFactory.GetInstance<ICotWaveRpst>(new ExplicitArguments(dic)),
                ObjectFactory.GetInstance<ICotStgReportRpst>(new ExplicitArguments(dic)),
                ObjectFactory.GetInstance<ICotStgGroupRpst>(new ExplicitArguments(dic)),
                ObjectFactory.GetInstance<ICotStgGroupItemRpst>(new ExplicitArguments(dic)),
                unit);
            return mgr;
        }

        #endregion

        #region Cec(CecHistory/CecResult)
        public static ICecContract CreateCecServer(AdeUnitOfWorkContext unit = null)
        {
            if (unit == null)
                unit = new AdeUnitOfWorkContext();
            IDictionary<string, object> dic = new Dictionary<string, object>();
            dic.Add("unit", unit);

            ICecContract cecContract = new CecService(
                ObjectFactory.GetInstance<ISunnetLog>(),
                ObjectFactory.GetInstance<IFile>(),
                ObjectFactory.GetInstance<IEmailSender>(),
                ObjectFactory.GetInstance<IEncrypt>(),

                ObjectFactory.GetInstance<ICecHistoryRpst>(new ExplicitArguments(dic)),
                ObjectFactory.GetInstance<ICecResultRpst>(new ExplicitArguments(dic)),

                unit
                );
            return cecContract;
        }
        #endregion



        #region CreateCommunityService
        public static ICommunityContract CreateCommunityService(EFUnitOfWorkContext unit = null)
        {
            if (unit == null)
                unit = new EFUnitOfWorkContext();
            IDictionary<string, object> dic = new Dictionary<string, object>();
            dic.Add("unit", unit);

            ICommunityRpst communityRpst = ObjectFactory.GetInstance<ICommunityRpst>(new ExplicitArguments(dic));
            IBasicCommunityRpst basicCommunityRpst =
                ObjectFactory.GetInstance<IBasicCommunityRpst>(new ExplicitArguments(dic));

            ICommunityNotesRpst communityNotesRpst =
                ObjectFactory.GetInstance<ICommunityNotesRpst>(new ExplicitArguments(dic));

            ICommunityContract communityContract = new CommunityService(
                ObjectFactory.GetInstance<ISunnetLog>(),
                ObjectFactory.GetInstance<IFile>(),
                ObjectFactory.GetInstance<IEmailSender>(),
                ObjectFactory.GetInstance<IEncrypt>(),
                communityRpst,
                basicCommunityRpst,
                ObjectFactory.GetInstance<ICommunityRoleRpst>(new ExplicitArguments(dic)),
                communityNotesRpst,

                ObjectFactory.GetInstance<ICommunityAssessmentRelationsRpst>(new ExplicitArguments(dic)),

                unit);
            return communityContract;
        }
        #endregion

        #region CreateSchoolService
        public static ISchoolContract CreateSchoolService(EFUnitOfWorkContext unit = null)
        {
            if (unit == null)
                unit = new EFUnitOfWorkContext();
            IDictionary<string, object> dic = new Dictionary<string, object>();
            dic.Add("unit", unit);
            ISchoolContract mgr = new SchoolService(
                ObjectFactory.GetInstance<ISunnetLog>(),
                ObjectFactory.GetInstance<IFile>(),
                ObjectFactory.GetInstance<IEmailSender>(),
                ObjectFactory.GetInstance<IEncrypt>(),
                ObjectFactory.GetInstance<ISchoolRpst>(new ExplicitArguments(dic)),
                ObjectFactory.GetInstance<IBasicSchoolRpst>(new ExplicitArguments(dic)),
                ObjectFactory.GetInstance<ISchoolTypeRpst>(new ExplicitArguments(dic)),
                ObjectFactory.GetInstance<IIspRpst>(new ExplicitArguments(dic)),
                ObjectFactory.GetInstance<ITrsProviderRpst>(new ExplicitArguments(dic)),
                ObjectFactory.GetInstance<IParentAgencyRpst>(new ExplicitArguments(dic)),
                ObjectFactory.GetInstance<IPlaygroundRpst>(new ExplicitArguments(dic)),
                ObjectFactory.GetInstance<ICommunitySchoolRelationsRpst>(new ExplicitArguments(dic)),
                ObjectFactory.GetInstance<ISchoolStudentRelationRpst>(new ExplicitArguments(dic)),
                ObjectFactory.GetInstance<ISchoolRoleRpst>(new ExplicitArguments(dic)),
                unit
                );
            return mgr;
        }
        #endregion

        #region CreateClassroomService
        public static IClassroomContract CreateClassroomService(EFUnitOfWorkContext unit = null)
        {
            if (unit == null)
                unit = new EFUnitOfWorkContext();
            IDictionary<string, object> dic = new Dictionary<string, object>();
            dic.Add("unit", unit);

            IClassroomRpst classroomRpst = ObjectFactory.GetInstance<IClassroomRpst>(new ExplicitArguments(dic));
            IKitRpst kitRpst = ObjectFactory.GetInstance<IKitRpst>(new ExplicitArguments(dic));



            IClassroomContract classroomContract = new ClassroomService(
                ObjectFactory.GetInstance<ISunnetLog>(),
                ObjectFactory.GetInstance<IFile>(),
                ObjectFactory.GetInstance<IEmailSender>(),
                ObjectFactory.GetInstance<IEncrypt>(),
                classroomRpst,
                kitRpst,
                ObjectFactory.GetInstance<IClassroomRoleRpst>(new ExplicitArguments(dic)),
                unit);
            return classroomContract;
        }
        #endregion

        #region CreateClassService
        public static IClassContract CreateClassService(EFUnitOfWorkContext unit = null)
        {
            if (unit == null)
                unit = new EFUnitOfWorkContext();
            IDictionary<string, object> dic = new Dictionary<string, object>();
            dic.Add("unit", unit);

            IClassRpst classRpst = ObjectFactory.GetInstance<IClassRpst>(new ExplicitArguments(dic));
            IMonitoringToolRpst monitoringToolRpst =
                ObjectFactory.GetInstance<IMonitoringToolRpst>(new ExplicitArguments(dic));
            ICommunityRpst communityRpst = ObjectFactory.GetInstance<ICommunityRpst>(new ExplicitArguments(dic));
            ISchoolRpst schoolRpst = ObjectFactory.GetInstance<ISchoolRpst>(new ExplicitArguments(dic));
            IClassroomRpst classroomRpst = ObjectFactory.GetInstance<IClassroomRpst>(new ExplicitArguments(dic));
            IClassRoleRpst classRoleRpst = ObjectFactory.GetInstance<IClassRoleRpst>(new ExplicitArguments(dic));
            IClassroomClassRpst classroomClassRpst = ObjectFactory.GetInstance<IClassroomClassRpst>(new ExplicitArguments(dic));
            IClassLevelRpst classLevelRpst = ObjectFactory.GetInstance<IClassLevelRpst>(new ExplicitArguments(dic));

            IClassContract classContract = new ClassService(
               ObjectFactory.GetInstance<ISunnetLog>(),
               ObjectFactory.GetInstance<IFile>(),
               ObjectFactory.GetInstance<IEmailSender>(),
               ObjectFactory.GetInstance<IEncrypt>(),
               classRpst, monitoringToolRpst,
               communityRpst, schoolRpst, classroomRpst,
               classRoleRpst,
               classroomClassRpst,
               classLevelRpst,
               unit);
            return classContract;
        }
        #endregion

        #region CreateClassService
        public static ITRSClassContract CreateTRSClassService(EFUnitOfWorkContext unit = null)
        {
            if (unit == null)
                unit = new EFUnitOfWorkContext();
            IDictionary<string, object> dic = new Dictionary<string, object>();
            dic.Add("unit", unit);

            ITRSClassRpst trsClassRpst = ObjectFactory.GetInstance<ITRSClassRpst>(new ExplicitArguments(dic));
            ICHChildrenRpst cHChildrenRpst = ObjectFactory.GetInstance<ICHChildrenRpst>(new ExplicitArguments(dic));
            ICHChildrenResultRpst cHChildrenResultRpst = ObjectFactory.GetInstance<ICHChildrenResultRpst>(new ExplicitArguments(dic));

            ITRSClassContract trsClassContract = new TRSClassService(
               ObjectFactory.GetInstance<ISunnetLog>(),
               ObjectFactory.GetInstance<IFile>(),
               ObjectFactory.GetInstance<IEmailSender>(),
               ObjectFactory.GetInstance<IEncrypt>(),
               trsClassRpst,
               cHChildrenRpst, cHChildrenResultRpst,
               unit);
            return trsClassContract;
        }
        #endregion

        #region CreateStudentService
        public static IStudentContract CreateStudentService(EFUnitOfWorkContext unit = null)
        {
            if (unit == null)
                unit = new EFUnitOfWorkContext();
            IDictionary<string, object> dic = new Dictionary<string, object>();
            dic.Add("unit", unit);

            IStudentRpst studentRpst = ObjectFactory.GetInstance<IStudentRpst>(new ExplicitArguments(dic));
            IStudentRoleRpst studentRoleRpst = ObjectFactory.GetInstance<IStudentRoleRpst>(new ExplicitArguments(dic));
            IStudentDOBRpst studentDOBRpst = ObjectFactory.GetInstance<IStudentDOBRpst>(new ExplicitArguments(dic));

            IChildRpst childRpst = ObjectFactory.GetInstance<IChildRpst>(new ExplicitArguments(dic));
            //IV_ChildRpst v_ChildRpst = ObjectFactory.GetInstance<IV_ChildRpst>(new ExplicitArguments(dic));
            IParentChildRpst parentChildRpst = ObjectFactory.GetInstance<IParentChildRpst>(new ExplicitArguments(dic));

            IStudentContract contract = new StudentService(
                ObjectFactory.GetInstance<ISunnetLog>(),
                ObjectFactory.GetInstance<IFile>(),
                ObjectFactory.GetInstance<IEmailSender>(),
                ObjectFactory.GetInstance<IEncrypt>(),
                studentRpst,
                studentRoleRpst,
                studentDOBRpst,
                childRpst,
                //v_ChildRpst,
                parentChildRpst,
                unit);
            return contract;
        }
        #endregion

        public static IMasterDataContract CreateMasterDataServer(EFUnitOfWorkContext unit = null)
        {
            if (unit == null)
                unit = new EFUnitOfWorkContext();
            IDictionary<string, object> dic = new Dictionary<string, object>();
            dic.Add("unit", unit);

            IMasterDataContract mgr = new MasterDataServer(
                ObjectFactory.GetInstance<ISunnetLog>(),
                ObjectFactory.GetInstance<IFile>(),
                ObjectFactory.GetInstance<IEmailSender>(),
                ObjectFactory.GetInstance<IEncrypt>(),
                ObjectFactory.GetInstance<ILanguageRpst>(new ExplicitArguments(dic)),
                ObjectFactory.GetInstance<IStateRpst>(new ExplicitArguments(dic)),
                ObjectFactory.GetInstance<ICountryRpst>(new ExplicitArguments(dic)),
                ObjectFactory.GetInstance<IFundingRpst>(new ExplicitArguments(dic)),
                ObjectFactory.GetInstance<ICountyRpst>(new ExplicitArguments(dic)),
                ObjectFactory.GetInstance<ITitleRpst>(new ExplicitArguments(dic)),
                ObjectFactory.GetInstance<ICurriculumRpst>(new ExplicitArguments(dic)),

                unit
                );
            return mgr;
        }

        #region  CreatePermissionService
        public static IPermissionContract CreatePermissionService(EFUnitOfWorkContext unit = null)
        {
            if (unit == null)
                unit = new EFUnitOfWorkContext();
            IDictionary<string, object> dic = new Dictionary<string, object>();
            dic.Add("unit", unit);

            IPermissionContract permission = new PermissionService(
                ObjectFactory.GetInstance<ISunnetLog>(),
                ObjectFactory.GetInstance<IFile>(),
                ObjectFactory.GetInstance<IEmailSender>(),
                ObjectFactory.GetInstance<IEncrypt>(),
                ObjectFactory.GetInstance<IAuthorityRpst>(new ExplicitArguments(dic)),
                ObjectFactory.GetInstance<IPageRpst>(new ExplicitArguments(dic)),
                ObjectFactory.GetInstance<IPermissionRoleRpst>(new ExplicitArguments(dic)),
                ObjectFactory.GetInstance<IRolePageAuthorityRpst>(new ExplicitArguments(dic)),
                ObjectFactory.GetInstance<IAssignedPackageRpst>(new ExplicitArguments(dic)),
                ObjectFactory.GetInstance<IDisabledUserRoleRpst>(new ExplicitArguments(dic)),
                unit);

            return permission;
        }
        #endregion

        #region DataProcess
        public static IDataProcessContract CreateDataProcessService(EFUnitOfWorkContext unit = null)
        {
            if (unit == null)
                unit = new EFUnitOfWorkContext();
            IDictionary<string, object> dic = new Dictionary<string, object>();
            dic.Add("unit", unit);

            IDataProcessContract permission = new DataProcessService(
                ObjectFactory.GetInstance<ISunnetLog>(),
                ObjectFactory.GetInstance<IFile>(),
                ObjectFactory.GetInstance<IEmailSender>(),
                ObjectFactory.GetInstance<IEncrypt>(),
                ObjectFactory.GetInstance<IDataGroupRpst>(new ExplicitArguments(dic)),
                ObjectFactory.GetInstance<IDataCommunityRpst>(new ExplicitArguments(dic)),
                ObjectFactory.GetInstance<IDataSchoolRpst>(new ExplicitArguments(dic)),
                ObjectFactory.GetInstance<IDataStudentRpst>(new ExplicitArguments(dic)),
                unit);

            return permission;
        }
        #endregion

        #region Reports
        public static IReportContract CreateReportService(EFUnitOfWorkContext unit = null)
        {
            if (unit == null)
                unit = new EFUnitOfWorkContext();
            IDictionary<string, object> dic = new Dictionary<string, object>();
            dic.Add("unit", unit);

            IReportContract report = new ReportService(
                ObjectFactory.GetInstance<ISunnetLog>(),
                ObjectFactory.GetInstance<IFile>(),
                ObjectFactory.GetInstance<IEmailSender>(),
                ObjectFactory.GetInstance<IEncrypt>(),
                ObjectFactory.GetInstance<IStudentRpst>(new ExplicitArguments(dic)),
                ObjectFactory.GetInstance<ICoordCoachRpst>(new ExplicitArguments(dic)),
                ObjectFactory.GetInstance<ISchoolViewRpst>(new ExplicitArguments(dic)),
                ObjectFactory.GetInstance<ISchoolRpst>(new ExplicitArguments(dic)),
                ObjectFactory.GetInstance<ISchoolTypeRpst>(new ExplicitArguments(dic)),
                ObjectFactory.GetInstance<ITeacherRpst>(new ExplicitArguments(dic)),
                ObjectFactory.GetInstance<IReportQueueRpst>(new ExplicitArguments(dic)),
                ObjectFactory.GetInstance<IAssessmentReportTemplateRpst>(new ExplicitArguments(dic)),
                ObjectFactory.GetInstance<IParentReportRpst>(new ExplicitArguments(dic)),
                unit);

            return report;
        }
        #endregion

        public static IOperationLogContract CreateOperationLogService(EFUnitOfWorkContext unit = null)
        {
            if (unit == null)
                unit = new EFUnitOfWorkContext();
            IDictionary<string, object> dic = new Dictionary<string, object>();
            dic.Add("unit", unit);

            IOperationLogContract mgr = new OperationLogService(
                ObjectFactory.GetInstance<ISunnetLog>(),
                ObjectFactory.GetInstance<IFile>(),
                ObjectFactory.GetInstance<IEmailSender>(),
                ObjectFactory.GetInstance<IEncrypt>(),
                ObjectFactory.GetInstance<IOperationLogRpst>(new ExplicitArguments(dic)),
                unit
                );
            return mgr;
        }

        #region StatusTracking

        public static IStatusTrackingContract CreateStatusTrackingService(EFUnitOfWorkContext unit = null)
        {
            if (unit == null)
                unit = new EFUnitOfWorkContext();
            IDictionary<string, object> dic = new Dictionary<string, object>();
            dic.Add("unit", unit);

            IStatusTrackingContract mgr = new StatusTrackingService(
                ObjectFactory.GetInstance<ISunnetLog>(),
                ObjectFactory.GetInstance<IFile>(),
                ObjectFactory.GetInstance<IEmailSender>(),
                ObjectFactory.GetInstance<IEncrypt>(),
                ObjectFactory.GetInstance<IStatusTrackingRpst>(new ExplicitArguments(dic)),
                unit
                );
            return mgr;
        }

        #endregion


        #region vcw
        public static IVcwContract CreateVcwService(VCWUnitOfWorkContext unit = null)
        {
            if (unit == null)
                unit = new VCWUnitOfWorkContext();
            IDictionary<string, object> dic = new Dictionary<string, object>();
            dic.Add("unit", unit);

            IVcwContract vcw = new VcwService(
                ObjectFactory.GetInstance<ISunnetLog>(),
                ObjectFactory.GetInstance<IFile>(),
                ObjectFactory.GetInstance<IEmailSender>(),
                ObjectFactory.GetInstance<IEncrypt>(),
                ObjectFactory.GetInstance<IAssignmentRpst>(new ExplicitArguments(dic)),
                ObjectFactory.GetInstance<IAssignmentContentRpst>(new ExplicitArguments(dic)),
                ObjectFactory.GetInstance<IAssignmentContextRpst>(new ExplicitArguments(dic)),
                ObjectFactory.GetInstance<IAssignmentFileRpst>(new ExplicitArguments(dic)),
                ObjectFactory.GetInstance<IAssignmentWatchFileRpst>(new ExplicitArguments(dic)),
                ObjectFactory.GetInstance<IAssignmentUploadTypeRpst>(new ExplicitArguments(dic)),
                ObjectFactory.GetInstance<IAssignmentStrategyRpst>(new ExplicitArguments(dic)),
                ObjectFactory.GetInstance<IAssignmentReportRpst>(new ExplicitArguments(dic)),
                ObjectFactory.GetInstance<IVIPDocumentRpst>(new ExplicitArguments(dic)),
                ObjectFactory.GetInstance<IFileRpst>(new ExplicitArguments(dic)),
                ObjectFactory.GetInstance<IFileSelectionRpst>(new ExplicitArguments(dic)),
                ObjectFactory.GetInstance<IFileSharedRpst>(new ExplicitArguments(dic)),
                ObjectFactory.GetInstance<IFileContentRpst>(new ExplicitArguments(dic)),
                ObjectFactory.GetInstance<IFileStrategyRpst>(new ExplicitArguments(dic)),
                ObjectFactory.GetInstance<ITeacherRpst_V>(new ExplicitArguments(dic)),
                ObjectFactory.GetInstance<IUserRpst_V>(new ExplicitArguments(dic)),
                ObjectFactory.GetInstance<IUploadTypeRpst>(new ExplicitArguments(dic)),
                ObjectFactory.GetInstance<ISessionRpst>(new ExplicitArguments(dic)),
                ObjectFactory.GetInstance<IWaveRpst>(new ExplicitArguments(dic)),
                ObjectFactory.GetInstance<IContext_DataRpst>(new ExplicitArguments(dic)),
                ObjectFactory.GetInstance<IAssignment_Content_DataRpst>(new ExplicitArguments(dic)),
                ObjectFactory.GetInstance<IVideo_Content_DataRpst>(new ExplicitArguments(dic)),
                ObjectFactory.GetInstance<IVideo_Language_DataRpst>(new ExplicitArguments(dic)),
                ObjectFactory.GetInstance<ICoachingStrategy_DataRpst>(new ExplicitArguments(dic)),
                ObjectFactory.GetInstance<IVideo_SelectionList_DataRpst>(new ExplicitArguments(dic)),
                unit);
            return vcw;
        }
        #endregion

        #region
        public static ITRSContract CreateTRSService(AdeUnitOfWorkContext unit = null)
        {
            if (unit == null)
                unit = new AdeUnitOfWorkContext();
            IDictionary<string, object> dic = new Dictionary<string, object>();
            dic.Add("unit", unit);

            ITRSContract mgr = new TRSService(
                ObjectFactory.GetInstance<ISunnetLog>(),
                ObjectFactory.GetInstance<IFile>(),
                ObjectFactory.GetInstance<IEmailSender>(),
                ObjectFactory.GetInstance<IEncrypt>(),
                ObjectFactory.GetInstance<ITRSAnswerRpst>(new ExplicitArguments(dic)),
                ObjectFactory.GetInstance<ITRSAssessmentItemRpst>(new ExplicitArguments(dic)),
                ObjectFactory.GetInstance<ITrsStarRpst>(new ExplicitArguments(dic)),
                ObjectFactory.GetInstance<ITRSAssessmentRpst>(new ExplicitArguments(dic)),
                ObjectFactory.GetInstance<ITRSItemRpst>(new ExplicitArguments(dic)),
                ObjectFactory.GetInstance<ITRSItemAnswerRpst>(new ExplicitArguments(dic)),
                ObjectFactory.GetInstance<ITRSSubcategoryRpst>(new ExplicitArguments(dic)),
                ObjectFactory.GetInstance<ITRSAssessmentClassRpst>(new ExplicitArguments(dic)),
                ObjectFactory.GetInstance<ITRSEventLogRpst>(new ExplicitArguments(dic)),
                ObjectFactory.GetInstance<ITRSNotificationRpst>(new ExplicitArguments(dic)),
                ObjectFactory.GetInstance<ITRSEventLogFileRpst>(new ExplicitArguments(dic)),
                unit
                );
            return mgr;
        }
        #endregion

        public static IBUPContract CreateBUPService(EFUnitOfWorkContext unit = null)
        {
            if (unit == null)
                unit = new EFUnitOfWorkContext();
            IDictionary<string, object> dic = new Dictionary<string, object>();
            dic.Add("unit", unit);

            IBUPContract mgr = new BUPService(
                 ObjectFactory.GetInstance<ISunnetLog>(),
                ObjectFactory.GetInstance<IFile>(),
                ObjectFactory.GetInstance<IEmailSender>(),
                ObjectFactory.GetInstance<IEncrypt>(),
                ObjectFactory.GetInstance<IBUPTaskRpst>(new ExplicitArguments(dic)),
                ObjectFactory.GetInstance<IAutomationSettingRpst>(new ExplicitArguments(dic)),
                unit
                );
            return mgr;
        }

        #region Exprot

        public static IExportContract CreateExportService(EFUnitOfWorkContext unit = null)
        {
            if (unit == null)
                unit = new EFUnitOfWorkContext();
            IDictionary<string, object> dic = new Dictionary<string, object>();
            dic.Add("unit", unit);

            IExportContract mgr = new ExportService(
                ObjectFactory.GetInstance<ISunnetLog>(),
                ObjectFactory.GetInstance<IFile>(),
                ObjectFactory.GetInstance<IEmailSender>(),
                ObjectFactory.GetInstance<IEncrypt>(),
                ObjectFactory.GetInstance<IReportTemplateRpst>(new ExplicitArguments(dic)),
                ObjectFactory.GetInstance<IFieldMapRpst>(new ExplicitArguments(dic)),
                ObjectFactory.GetInstance<IExportInfoRpst>(new ExplicitArguments(dic)),
                unit);
            return mgr;
        }

        #endregion


        #region Observable
        public static IObservableContract CreateObservableAssessmentService(AdeUnitOfWorkContext unit = null)
        {
            if (unit == null)
                unit = new AdeUnitOfWorkContext();
            IDictionary<string, object> dic = new Dictionary<string, object>();
            dic.Add("unit", unit);
            var mgr = new ObservableService(
                ObjectFactory.GetInstance<ISunnetLog>(),
                ObjectFactory.GetInstance<IFile>(),
                ObjectFactory.GetInstance<IEmailSender>(),
                ObjectFactory.GetInstance<IEncrypt>(),
                ObjectFactory.GetInstance<IObservableAssessmentRpst>(new ExplicitArguments(dic)),
                ObjectFactory.GetInstance<IObservableAssessmentItemRpst>(new ExplicitArguments(dic)),
                ObjectFactory.GetInstance<IObservableItemsHistoryRpst>(new ExplicitArguments(dic)),
                unit);
            return mgr;
        }
        #endregion

        #region Tsds
        public static ITsdsContract CreateTsdsService(AdeUnitOfWorkContext unit = null)
        {
            if (unit == null)
                unit = new AdeUnitOfWorkContext();
            IDictionary<string, object> dic = new Dictionary<string, object>();
            dic.Add("unit", unit);
            var mgr = new TsdsService(
                ObjectFactory.GetInstance<ISunnetLog>(),
                ObjectFactory.GetInstance<IFile>(),
                ObjectFactory.GetInstance<IEmailSender>(),
                ObjectFactory.GetInstance<IEncrypt>(),
                ObjectFactory.GetInstance<ITsdsAssessmentFileRpst>(new ExplicitArguments(dic)),
                ObjectFactory.GetInstance<ITsdsRpst>(new ExplicitArguments(dic)),
                ObjectFactory.GetInstance<ITsdsMapRpst>(new ExplicitArguments(dic)),
                unit);
            return mgr;
        }
        #endregion


        #region Update Cluster Front End
        public static IUpdateClusterContract CreateUpdateClusterService(EFUnitOfWorkContext unit = null)
        {
            if (unit == null)
                unit = new EFUnitOfWorkContext();
            IDictionary<string, object> dic = new Dictionary<string, object>();
            dic.Add("unit", unit);
            var mgr = new UpdateClusterService(
                ObjectFactory.GetInstance<ISunnetLog>(),
                ObjectFactory.GetInstance<IFile>(),
                ObjectFactory.GetInstance<IEmailSender>(),
                ObjectFactory.GetInstance<IEncrypt>(),
                ObjectFactory.GetInstance<ISystemUpdateRpst>(new ExplicitArguments(dic)),
                ObjectFactory.GetInstance<IMessageCenterRpst>(new ExplicitArguments(dic)),
                ObjectFactory.GetInstance<INewFeaturedRpst>(new ExplicitArguments(dic)),
                unit);
            return mgr;
        }

        #endregion

        #region Practice
        public static IPracticeContract CreatePracticeService(PracticeUnitOfWorkContext unit = null)
        {
            if (unit == null)
                unit = new PracticeUnitOfWorkContext();
            IDictionary<string, object> dic = new Dictionary<string, object>();
            dic.Add("unit", unit);
            var mgr = new PracticeService(
                ObjectFactory.GetInstance<ISunnetLog>(),
                ObjectFactory.GetInstance<IFile>(),
                ObjectFactory.GetInstance<IEmailSender>(),
                ObjectFactory.GetInstance<IEncrypt>(),
                ObjectFactory.GetInstance<IPracticeStudentAssessmentRpst>(new ExplicitArguments(dic)),
                ObjectFactory.GetInstance<IPracticeStudentItemRpst>(new ExplicitArguments(dic)),
                ObjectFactory.GetInstance<IPracticeStudentMeasureRpst>(new ExplicitArguments(dic)),
                ObjectFactory.GetInstance<IDemoStudentRpst>(new ExplicitArguments(dic)),
                ObjectFactory.GetInstance<IPracticeMeasureGroupRpst>(new ExplicitArguments(dic)),
                      ObjectFactory.GetInstance<IPracticeStudentGroupRpst>(new ExplicitArguments(dic)),
                      ObjectFactory.GetInstance<IPracticeGroupMyActivityRpst>(new ExplicitArguments(dic)),
                unit);
            return mgr;
        }
        #endregion


        public static ICACContract CreateCACService(EFUnitOfWorkContext unit = null)
        {
            if (unit == null)
                unit = new EFUnitOfWorkContext();
            IDictionary<string, object> dic = new Dictionary<string, object>();
            dic.Add("unit", unit);

            ICACContract mgr = new CACService(
                 ObjectFactory.GetInstance<ISunnetLog>(),
                ObjectFactory.GetInstance<IFile>(),
                ObjectFactory.GetInstance<IEmailSender>(),
                ObjectFactory.GetInstance<IEncrypt>(),
                ObjectFactory.GetInstance<IMyActivityRpst>(new ExplicitArguments(dic)),
                      ObjectFactory.GetInstance<IActivityHistoryRpst>(new ExplicitArguments(dic)),
                unit
                );
            return mgr;
        }
    }
}