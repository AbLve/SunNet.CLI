using Sunnet.Cli.Core.MasterData.Entities;
using Sunnet.Framework.Core.Base;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Data.Entity.ModelConfiguration.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sunnet.Cli.Core.MasterData.Configurations
{
    public class CountryCnfg : EntityTypeConfiguration<CountryEntity>, IEntityMapper
    {
        public CountryCnfg()
        {
            ToTable("Countries");
        }

        public void RegistTo(ConfigurationRegistrar configurations)
        {
            configurations.Add(this);
        }
    }
}
