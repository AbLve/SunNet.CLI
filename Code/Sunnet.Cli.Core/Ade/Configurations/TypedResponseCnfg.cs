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
    internal class TypedResponseCnfg : EntityTypeConfiguration<TypedResponseEntity>, IEntityMapper
    {
        public TypedResponseCnfg()
        {
            ToTable("TypedResonses");

            HasRequired(x => x.Item).WithMany(x => x.Responses).HasForeignKey(x => x.ItemId).WillCascadeOnDelete(true);
            
            Ignore(x => x.UpdatedOn);
            Ignore(x => x.CreatedOn);

            Property(x => x.Text).HasMaxLength(1000);
            Property(x => x.Picture).HasMaxLength(100);
        }
        public void RegistTo(ConfigurationRegistrar configurations)
        {
            configurations.Add(this);
        }
    }
}
