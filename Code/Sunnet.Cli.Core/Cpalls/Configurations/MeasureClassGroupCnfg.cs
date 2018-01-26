using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sunnet.Cli.Core.Cpalls.Entities;
using Sunnet.Framework.Core.Base;

namespace Sunnet.Cli.Core.Cpalls.Configurations
{
    internal class MeasureClassGroupCnfg : EntityTypeConfiguration<MeasureClassGroupEntity>, IEntityMapper
    {
        public MeasureClassGroupCnfg()
        {
            ToTable("MeasureClassGroups");
        }

        public void RegistTo(System.Data.Entity.ModelConfiguration.Configuration.ConfigurationRegistrar configurations)
        {
            configurations.Add(this);
        }
    }
}
