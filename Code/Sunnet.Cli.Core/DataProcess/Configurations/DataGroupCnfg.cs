using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Sunnet.Cli.Core.DataProcess.Entities;
using Sunnet.Framework.Core.Base;
using System.Data.Entity.ModelConfiguration.Configuration;

namespace Sunnet.Cli.Core.DataProcess.Configurations
{
    public class DataGroupCnfg : EntityTypeConfiguration<DataGroupEntity>, IEntityMapper
    {
        public DataGroupCnfg()
        {
            ToTable("DataGroups");
        }

        public void RegistTo(ConfigurationRegistrar configurations)
        {
            configurations.Add(this);
        }
    }
}
