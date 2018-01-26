using Sunnet.Cli.Core.Users.Entities;
using Sunnet.Framework.Core.Base;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Data.Entity.ModelConfiguration.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sunnet.Cli.Core.Users.Configurations
{
    public class YearsInProjectCnfg : EntityTypeConfiguration<YearsInProjectEntity>, IEntityMapper
    {
        public YearsInProjectCnfg()
        {
            ToTable("YearsInProjects");
        }

        public void RegistTo(ConfigurationRegistrar configurations)
        {
            configurations.Add(this);
        }
    }
}
