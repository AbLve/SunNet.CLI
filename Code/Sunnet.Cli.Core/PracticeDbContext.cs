using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using Sunnet.Cli.Core.Ade.Configurations;
using Sunnet.Cli.Core.Practices.Configurations;

namespace Sunnet.Cli.Core
{
    public class PracticeDbContext : DbContext
    {
        public PracticeDbContext(): base("PracticeDbContext")
        {
            Database.SetInitializer<PracticeDbContext>(null);
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();


            #region Practice
            //Practice
            new DemoStudentCnfg().RegistTo(modelBuilder.Configurations);
            new PracticeStudentAssessmentCnfg().RegistTo(modelBuilder.Configurations);
            new PracticeStudentMeasureCnfg().RegistTo(modelBuilder.Configurations);
            new PracticeStudentItemCnfg().RegistTo(modelBuilder.Configurations);
            new CliAdeAssessmentCnfg().RegistTo(modelBuilder.Configurations);
            new CliAdeMeasureCnfg().RegistTo(modelBuilder.Configurations);
            new CliAdeItemBaseCnfg().RegistTo(modelBuilder.Configurations);
            new PracticeMeasureGroupCnfg().RegistTo(modelBuilder.Configurations);
            new PracticeStudentGroupCnfg().RegistTo(modelBuilder.Configurations);
            new PracticeGroupMyActivityCnfg().RegistTo(modelBuilder.Configurations);

            #endregion
        }
    }
}
