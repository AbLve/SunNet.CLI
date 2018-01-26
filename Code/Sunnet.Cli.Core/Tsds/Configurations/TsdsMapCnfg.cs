using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Data.Entity.ModelConfiguration.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sunnet.Cli.Core.Tsds.Entities;
using Sunnet.Framework.Core.Base;

namespace Sunnet.Cli.Core.Tsds.Configurations
{
    public class TsdsMapCnfg : EntityTypeConfiguration<TsdsMapEntity>, IEntityMapper
    {
        public TsdsMapCnfg()
        {
            ToTable("TsdsMap");
            Ignore(t => t.CreatedOn);
            Ignore(t => t.UpdatedOn);
        }

        public void RegistTo(ConfigurationRegistrar configurations)
        {
            configurations.Add(this);
        }
    }
}
