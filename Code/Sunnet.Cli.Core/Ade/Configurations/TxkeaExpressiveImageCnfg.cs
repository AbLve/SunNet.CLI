using Sunnet.Cli.Core.Ade.Entities;
using Sunnet.Framework.Core.Base;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Data.Entity.ModelConfiguration.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sunnet.Cli.Core.Ade.Configurations
{
    internal class TxkeaExpressiveImageCnfg : EntityTypeConfiguration<TxkeaExpressiveImageEntity>, IEntityMapper
    {
        public TxkeaExpressiveImageCnfg()
        {
            ToTable("TxkeaExpressiveImages");

            Ignore(x => x.UpdatedOn);
            Ignore(x => x.CreatedOn);

            HasRequired(x => x.Item).WithMany(x => x.ImageList).HasForeignKey(x => x.ItemId).WillCascadeOnDelete(true);
        }

        public void RegistTo(ConfigurationRegistrar configurations)
        {
            configurations.Add(this);
        }
    }
}
