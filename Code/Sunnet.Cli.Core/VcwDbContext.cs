using Sunnet.Cli.Core.Vcw.Configurations;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sunnet.Cli.Core
{
    public class VcwDbContext : DbContext
    {
        public VcwDbContext()
            : base("VcwDbContext")
        {
            Database.SetInitializer<MainDbContext>(null);
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();


            #region VCW
            new AssignmentCnfg().RegistTo(modelBuilder.Configurations);
            new AssignmentContentCnfg().RegistTo(modelBuilder.Configurations);
            new AssignmentContextCnfg().RegistTo(modelBuilder.Configurations);
            new AssignmentFileCnfg().RegistTo(modelBuilder.Configurations);
            new AssignmentWatchFileCnfg().RegistTo(modelBuilder.Configurations);
            new AssignmentUploadTypeCnfg().RegistTo(modelBuilder.Configurations);
            new AssignmentStrategyCnfg().RegistTo(modelBuilder.Configurations);
            new AssignmentReportCnfg().RegistTo(modelBuilder.Configurations);
            new VIPDocumentCnfg().RegistTo(modelBuilder.Configurations);
            new FileCnfg().RegistTo(modelBuilder.Configurations);
            new FileSelectionCnfg().RegistTo(modelBuilder.Configurations);
            new FileSharedCnfg().RegistTo(modelBuilder.Configurations);
            new FileContentCnfg().RegistTo(modelBuilder.Configurations);
            new FileStrategyCnfg().RegistTo(modelBuilder.Configurations);
            new UploadTypeCnfg().RegistTo(modelBuilder.Configurations);
            new SessionCnfg().RegistTo(modelBuilder.Configurations);
            new WaveCnfg().RegistTo(modelBuilder.Configurations);
            new Context_DataCnfg().RegistTo(modelBuilder.Configurations);
            new Assignment_Content_DataCnfg().RegistTo(modelBuilder.Configurations);
            new Video_Content_DataCnfg().RegistTo(modelBuilder.Configurations);
            new Video_Language_DataCnfg().RegistTo(modelBuilder.Configurations);
            new CoachingStrategy_DataCnfg().RegistTo(modelBuilder.Configurations);
            new Video_SelectionList_DataCnfg().RegistTo(modelBuilder.Configurations);
            new V_TeacherCnfg().RegistTo(modelBuilder.Configurations);
            new V_UserCnfg().RegistTo(modelBuilder.Configurations);
            #endregion
        }
    }
}
