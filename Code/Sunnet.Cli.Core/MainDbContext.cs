using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StructureMap.Query;
using Sunnet.Cli.Core.Classes.Configurations;
using Sunnet.Cli.Core.Classrooms.Configurations;
using Sunnet.Cli.Core.Communities.Configurations;
using Sunnet.Cli.Core.MasterData.Configurations;
using Sunnet.Cli.Core.Schools.Configurations;
using Sunnet.Cli.Core.Students.Configurations;
using Sunnet.Cli.Core.TRSClasses.Configurations;
using Sunnet.Cli.Core.Users.Configurations;
using Sunnet.Cli.Core.Permission.Configurations;
using Sunnet.Cli.Core.DataProcess.Configurations;
using Sunnet.Cli.Core.Log.Configurations;
using Sunnet.Cli.Core.Reports.Configurations;
using Sunnet.Cli.Core.StatusTracking.Configurations;
using Sunnet.Cli.Core.BUP.Configurations;
using Sunnet.Cli.Core.CAC.Configurations;
using Sunnet.Cli.Core.Export.Configurations;
using Sunnet.Cli.Core.UpdateClusters.Configurations;


/**************************************************************************
 * Developer: 		JackZhang
 * Computer:		JACKZ
 * Domain:			Jackz
 * CreatedOn:		2014/8/1 0:56:11
 * Description:		Please input class summary
 * Version History:	Created,2014/8/1 0:56:11
 * 
 * 
 **************************************************************************/

namespace Sunnet.Cli.Core
{
    internal class MainDbContext : DbContext
    {
        public MainDbContext(string nameOrConnectionString)
            : base(nameOrConnectionString)
        {
            Database.SetInitializer<MainDbContext>(null);
        }

        public MainDbContext()
            : base("MainDbContext")
        {
            Database.SetInitializer<MainDbContext>(null);
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();

            #region User
            new ApplicantCnfg().RegistTo(modelBuilder.Configurations);
            new ParentCnfg().RegistTo(modelBuilder.Configurations);
            new ParentStudentRelationCnfg().RegistTo(modelBuilder.Configurations);
            new UserBaseCnfg().RegistTo(modelBuilder.Configurations);
            new ApplicantEmailCnfg().RegistTo(modelBuilder.Configurations);
            new ApplicantContactCnfg().RegistTo(modelBuilder.Configurations);
            new TeacherCnfg().RegistTo(modelBuilder.Configurations);
            new PrincipalCnfg().RegistTo(modelBuilder.Configurations);
            new DisCommunityCnfg().RegistTo(modelBuilder.Configurations);
            new StateWideCnfg().RegistTo(modelBuilder.Configurations);
            new AuditorCnfg().RegistTo(modelBuilder.Configurations);
            new TeacherAgeGroupCnfg().RegistTo(modelBuilder.Configurations);
            new TeacherEquipmentRelationCnfg().RegistTo(modelBuilder.Configurations);
            new TeacherTransactionCnfg().RegistTo(modelBuilder.Configurations);
            new VideoCodingCnfg().RegistTo(modelBuilder.Configurations);
            new CoordCoachCnfg().RegistTo(modelBuilder.Configurations);
            new CoordCoachEquipmentCnfg().RegistTo(modelBuilder.Configurations);
            new TeacherRoleCnfg().RegistTo(modelBuilder.Configurations);
            new UserComSchRelationCnfg().RegistTo(modelBuilder.Configurations);
            new UserClassRelationCnfg().RegistTo(modelBuilder.Configurations);
            new IntManaCoachRelationCnfg().RegistTo(modelBuilder.Configurations);
            new PrincipalRoleCnfg().RegistTo(modelBuilder.Configurations);
            new CoordCoachRoleCnfg().RegistTo(modelBuilder.Configurations);
            #endregion

            new LanguageCnfg().RegistTo(modelBuilder.Configurations);
            new CertificateCnfg().RegistTo(modelBuilder.Configurations);
            new PositionCnfg().RegistTo(modelBuilder.Configurations);
            new ProfessionalDevelopmentCnfg().RegistTo(modelBuilder.Configurations);
            new YearsInProjectCnfg().RegistTo(modelBuilder.Configurations);
            new AffiliationCnfg().RegistTo(modelBuilder.Configurations);

            #region School
            new SchoolCnfg().RegistTo(modelBuilder.Configurations);
            new BasicSchoolCnfg().RegistTo(modelBuilder.Configurations);
            new SchoolTypeCnfg().RegistTo(modelBuilder.Configurations);
            new IspCnfg().RegistTo(modelBuilder.Configurations);
            new TrsProviderCnfg().RegistTo(modelBuilder.Configurations);
            new ParentAgencyCnfg().RegistTo(modelBuilder.Configurations);
            new PlaygroundCnfg().RegistTo(modelBuilder.Configurations);
            new CommunitySchoolRelationsCnfg().RegistTo(modelBuilder.Configurations);
            new SchoolStudentRelationCnfg().RegistTo(modelBuilder.Configurations);
            new SchoolRoleCnfg().RegistTo(modelBuilder.Configurations);
            #endregion

            #region Community
            new CommunityCnfg().RegistTo(modelBuilder.Configurations);
            new CommunityRoleCnfg().RegistTo(modelBuilder.Configurations);
            new CommunityNotesCnfg().RegistTo(modelBuilder.Configurations);
            #endregion

            new FundingCnfg().RegistTo(modelBuilder.Configurations);
            new CountyCnfg().RegistTo(modelBuilder.Configurations);
            new CountryCnfg().RegistTo(modelBuilder.Configurations);
            new StateCnfg().RegistTo(modelBuilder.Configurations);
            new TitleCnfg().RegistTo(modelBuilder.Configurations);
            new BasicCommunityCnfg().RegistTo(modelBuilder.Configurations);

            new ClassLevelCnfg().RegistTo(modelBuilder.Configurations);
            new ClassCnfg().RegistTo(modelBuilder.Configurations);
            new ClassRoleCnfg().RegistTo(modelBuilder.Configurations);
            new MonitoringToolCnfg().RegistTo(modelBuilder.Configurations);
            new ClassroomClassCnfg().RegistTo(modelBuilder.Configurations);

            new ClassroomCnfg().RegistTo(modelBuilder.Configurations);
            new ClassroomRoleCnfg().RegistTo(modelBuilder.Configurations);
            new KitCnfg().RegistTo(modelBuilder.Configurations);
            new CurriculumCnfg().RegistTo(modelBuilder.Configurations);

            new CHChildrenCnfg().RegistTo(modelBuilder.Configurations);
            new CHChildrenResultCnfg().RegistTo(modelBuilder.Configurations);

            new TRSClassCnfg().RegistTo(modelBuilder.Configurations);

            new CommunityAssessmentRelationsCnfg().RegistTo(modelBuilder.Configurations);


            #region Student
            new StudentCnfg().RegistTo(modelBuilder.Configurations);
            new StudentRoleCnfg().RegistTo(modelBuilder.Configurations);
            new StudentDOBCnfg().RegistTo(modelBuilder.Configurations);
            new ChildCnfg().RegistTo(modelBuilder.Configurations);
            new ParentChildCnfg().RegistTo(modelBuilder.Configurations);
            //new V_ChildCnfg().RegistTo(modelBuilder.Configurations);
            #endregion

            #region Permission
            new AuthorityCnfg().RegistTo(modelBuilder.Configurations);
            new PageCnfg().RegistTo(modelBuilder.Configurations);
            new PermissionRoleCnfg().RegistTo(modelBuilder.Configurations);
            new RolePageAuthorityCnfg().RegistTo(modelBuilder.Configurations);
            new AssignedPackageCnfg().RegistTo(modelBuilder.Configurations);
            new DisabledUserRoleCnfg().RegistTo(modelBuilder.Configurations);
            #endregion

            #region DataProcess
            new DataCommunityCnfg().RegistTo(modelBuilder.Configurations);
            new DataGroupCnfg().RegistTo(modelBuilder.Configurations);
            new DataSchoolCnfg().RegistTo(modelBuilder.Configurations);
            new DataStudentCnfg().RegistTo(modelBuilder.Configurations);
            #endregion

            #region Reports
            new SchoolViewCnfg().RegistTo(modelBuilder.Configurations);
            new ReportQueueCnfg().RegistTo(modelBuilder.Configurations);
            new AssessmentReportTemplateCnfg().RegistTo(modelBuilder.Configurations);
            new ParentReportCnfg().RegistTo(modelBuilder.Configurations);

            #endregion

            new OperationLogCnfg().RegistTo(modelBuilder.Configurations);

            #region
            new BUPTaskEntityCnfg().RegistTo(modelBuilder.Configurations);
            new AutomationSettingEntityCnfg().RegistTo(modelBuilder.Configurations);
            #endregion

            #region StatusTracking
            new StatusTrackingCnfg().RegistTo(modelBuilder.Configurations);
            #endregion

            #region Export
            new ReportTemplateCnfg().RegistTo(modelBuilder.Configurations);
            new FieldMapCnfg().RegistTo(modelBuilder.Configurations);
            new ExportInfoCnfg().RegistTo(modelBuilder.Configurations);
            #endregion

            #region
            new MyActivityCnfg().RegistTo(modelBuilder.Configurations);
            new ActivityHistoryCnfg().RegistTo(modelBuilder.Configurations);
            #endregion

            #region Update Cluster Front End
            new SystemUpdateCnfg().RegistTo(modelBuilder.Configurations);
            new MessageCenterCnfg().RegistTo(modelBuilder.Configurations);
            new NewFeaturedCnfg().RegistTo(modelBuilder.Configurations);
            #endregion
        }
    }
}
