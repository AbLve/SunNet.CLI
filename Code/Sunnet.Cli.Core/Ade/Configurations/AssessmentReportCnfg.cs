using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Data.Entity.ModelConfiguration.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Sunnet.Cli.Core.Ade.Entities;
using Sunnet.Framework.Core.Base;
namespace Sunnet.Cli.Core.Ade.Configurations
{
    internal class AssessmentReportCnfg : EntityTypeConfiguration<AssessmentReportEntity>, IEntityMapper
    {
        public AssessmentReportCnfg()
        {
            ToTable("AssessmentReports");
            HasRequired(e => e.Assessment).WithMany(e => e.AssessmentReports).HasForeignKey(e => e.AssessmentId);
            Ignore(e => e.CreatedOn);
            Ignore(e => e.UpdatedOn);
        }

        public void RegistTo(ConfigurationRegistrar configurations)
        {
            configurations.Add(this);
        }
    }
}
