using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Data.Entity.ModelConfiguration.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sunnet.Cli.Core.Ade.Entities;
using Sunnet.Framework.Core.Base;

namespace Sunnet.Cli.Core.Ade.Configurations
{
    internal class TypedResponseItemCnfg : EntityTypeConfiguration<TypedResponseItemEntity>, IEntityMapper
    {
        public TypedResponseItemCnfg()
        {
            ToTable("TypedResponseItems");
            Property(x => x.TargetText).HasMaxLength(1000);
            Property(x => x.TargetAudio).HasMaxLength(100);
        }

        public void RegistTo(ConfigurationRegistrar configurations)
        {
            configurations.Add(this);
        }
    }
}
