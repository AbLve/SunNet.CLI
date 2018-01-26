using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sunnet.Cli.Core.Ade.Entities;
using Sunnet.Framework.Core.Base;
using System.Data.Entity.ModelConfiguration.Configuration;


namespace Sunnet.Cli.Core.Ade.Configurations
{
    internal class PercentileRankLookupCnfg : EntityTypeConfiguration<PercentileRankLookupEntity>, IEntityMapper
    {
        public PercentileRankLookupCnfg()
        {
            ToTable("PercentileRankLookups");
            Ignore(e => e.CreatedOn);
            Ignore(e => e.UpdatedOn);
        }

        public void RegistTo(ConfigurationRegistrar configurations)
        {
            configurations.Add(this);
        }
    }
}
