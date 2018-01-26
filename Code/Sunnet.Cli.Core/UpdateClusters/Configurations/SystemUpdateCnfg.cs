using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Data.Entity.ModelConfiguration.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sunnet.Cli.Core.UpdateClusters.Entities;
using Sunnet.Framework.Core.Base;

namespace Sunnet.Cli.Core.UpdateClusters.Configurations
{
    public class SystemUpdateCnfg : EntityTypeConfiguration<SystemUpdateEntity>, IEntityMapper
    {
        public SystemUpdateCnfg()
        {
            ToTable("SystemUpdates");
        }

        public void RegistTo(ConfigurationRegistrar configurations)
        {
            configurations.Add(this);
        }
    }
}
