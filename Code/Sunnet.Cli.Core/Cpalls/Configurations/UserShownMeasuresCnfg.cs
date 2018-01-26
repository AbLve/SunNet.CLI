using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Sunnet.Cli.Core.Cpalls.Entities;
using Sunnet.Framework.Core.Base;
using System.Data.Entity.ModelConfiguration.Configuration;
using System.Data.Entity.ModelConfiguration;

namespace Sunnet.Cli.Core.Cpalls.Configurations
{
    class UserShownMeasuresCnfg: EntityTypeConfiguration<UserShownMeasuresEntity>, IEntityMapper
    {
        public UserShownMeasuresCnfg()
        {
            ToTable("UserShownMeasures");
        }

        public void RegistTo(ConfigurationRegistrar configurations)
        {
            configurations.Add(this);
        }
    }
}