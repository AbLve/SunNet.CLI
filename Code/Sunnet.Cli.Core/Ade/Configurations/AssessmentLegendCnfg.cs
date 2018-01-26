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
    internal class AssessmentLegendCnfg : EntityTypeConfiguration<AssessmentLegendEntity>, IEntityMapper
    {
        public AssessmentLegendCnfg()
        {
            ToTable("AssessmentLegends");
            HasRequired(e => e.Assessment).WithMany(e => e.AssessmentLegends).HasForeignKey(e => e.AssessmentId);
            Ignore(e => e.CreatedOn);
            Ignore(e => e.UpdatedOn);
        }

        public void RegistTo(ConfigurationRegistrar configurations)
        {
            configurations.Add(this);
        }
    }
}
