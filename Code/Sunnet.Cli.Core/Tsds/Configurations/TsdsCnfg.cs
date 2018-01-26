using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Data.Entity.ModelConfiguration.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sunnet.Cli.Core.Tsds.Entities;
using Sunnet.Framework.Core.Base;

namespace Sunnet.Cli.Core.Tsds.Configurations
{
    public class TsdsCnfg : EntityTypeConfiguration<TsdsEntity>, IEntityMapper
    {
        public TsdsCnfg()
        {
            ToTable("TSDS");
             
            HasRequired(c => c.DownloadUser).WithMany().HasForeignKey(o => o.DownloadBy);
            HasRequired(c => c.Community).WithMany().HasForeignKey(o => o.CommunityId);
        }

        public void RegistTo(ConfigurationRegistrar configurations)
        {
            configurations.Add(this);
        }
    }
}
