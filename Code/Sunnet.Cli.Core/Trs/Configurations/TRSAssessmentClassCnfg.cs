using Sunnet.Cli.Core.Trs.Entities;
using Sunnet.Framework.Core.Base;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Data.Entity.ModelConfiguration.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sunnet.Cli.Core.Trs.Configurations
{
    public class TRSAssessmentClassCnfg : EntityTypeConfiguration<TRSAssessmentClassEntity>, IEntityMapper
    {
        public TRSAssessmentClassCnfg()
        {
            ToTable("TRSAssessmentClasses");
            Ignore(r => r.UpdatedOn);
            Ignore(r => r.CreatedOn);
            HasRequired(r => r.Assessment).WithMany(x => x.AssessmentClasses).HasForeignKey(r => r.AssessmentId);
        }

        public void RegistTo(ConfigurationRegistrar configurations)
        {
            configurations.Add(this);
        }
    }
}
