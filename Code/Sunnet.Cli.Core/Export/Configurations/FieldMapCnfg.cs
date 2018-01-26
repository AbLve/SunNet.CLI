using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


using Sunnet.Framework.Core.Base;
using Sunnet.Cli.Core.Export.Entities;
using System.Data.Entity.ModelConfiguration.Configuration;

namespace Sunnet.Cli.Core.Export.Configurations
{
    public class FieldMapCnfg : EntityTypeConfiguration<FieldMapEntity>, IEntityMapper
    {
        public FieldMapCnfg()
        {
            ToTable("FieldMap");
        }

        public void RegistTo(ConfigurationRegistrar configurations)
        {
            configurations.Add(this);
        }
    }
}
