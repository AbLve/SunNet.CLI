using Sunnet.Cli.Core.Cot.Entities;
using Sunnet.Framework.Core.Base;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Data.Entity.ModelConfiguration.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sunnet.Cli.Core.Cot.Configurations
{
    public class CotStgGroupItemCnfg : EntityTypeConfiguration<CotStgGroupItemEntity>, IEntityMapper
    {
        public CotStgGroupItemCnfg()
        {
            HasRequired(x => x.CotStgGroup).WithMany(x => x.CotStgGroupItems).HasForeignKey(x => x.CotStgGroupId);
            HasRequired(x => x.Item).WithMany().HasForeignKey(x => x.ItemId);
            ToTable("CotStgGroupItems");
            Ignore(e => e.CreatedOn);
            Ignore(e => e.UpdatedOn);
        }
        public void RegistTo(ConfigurationRegistrar configurations)
        {
            configurations.Add(this);
        }
    }
}
