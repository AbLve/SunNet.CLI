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
    internal class TypedResponseOptionCnfg : EntityTypeConfiguration<TypedResponseOptionEntity>, IEntityMapper
    {
        public TypedResponseOptionCnfg()
        {
            ToTable("TypedResponseOptions");

            Ignore(x => x.CreatedOn);
            Ignore(x => x.UpdatedOn);
            HasRequired(x => x.Response).WithMany(x => x.Options).HasForeignKey(x => x.ResponseId).WillCascadeOnDelete(true);

            Property(x => x.Keyword).HasMaxLength(50);
        }

        public void RegistTo(ConfigurationRegistrar configurations)
        {
            configurations.Add(this);
        }
    }
}
