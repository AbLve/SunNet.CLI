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
    public class TrsStarCnfg: EntityTypeConfiguration<TrsStarEntity>, IEntityMapper
    {
        public TrsStarCnfg()
        {
            ToTable("TrsStars");
            Ignore(r => r.UpdatedOn);
            Ignore(r => r.CreatedOn);
            HasRequired(r => r.Assessment).WithMany(r => r.Stars).HasForeignKey(x=>x.AssessmentId);
        }

        public void RegistTo(ConfigurationRegistrar configurations)
        {
            configurations.Add(this);
        }
    }
}