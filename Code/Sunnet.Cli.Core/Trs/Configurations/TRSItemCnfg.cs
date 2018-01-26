using System.Data.Entity.Migrations.Model;
using Sunnet.Cli.Core.Trs.Entities;
using Sunnet.Framework.Core.Base;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Data.Entity.ModelConfiguration.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sunnet.Cli.Core.Trs.Configurations
{
    public class TRSItemCnfg : EntityTypeConfiguration<TRSItemEntity>, IEntityMapper
    {
        public TRSItemCnfg()
        {
            ToTable("TRSItems");
            Ignore(r => r.UpdatedOn);
            Ignore(r => r.CreatedOn);
            HasRequired(a => a.SubCategory).WithMany(b => b.Items).HasForeignKey(a => a.SubCategoryId);
        }

        public void RegistTo(ConfigurationRegistrar configurations)
        {
            configurations.Add(this);
        }
    }
}
