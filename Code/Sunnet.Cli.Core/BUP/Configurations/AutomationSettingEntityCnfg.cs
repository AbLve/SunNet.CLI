using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Sunnet.Cli.Core.BUP.Entities;
using Sunnet.Framework.Core.Base;
using System.Data.Entity.ModelConfiguration.Configuration;

namespace Sunnet.Cli.Core.BUP.Configurations
{
    public class AutomationSettingEntityCnfg : EntityTypeConfiguration<AutomationSettingEntity>, IEntityMapper
    {
        public AutomationSettingEntityCnfg()
        {
            ToTable("BUP_AutomationSettings");
            HasRequired(r => r.Community).WithMany(x => x.AutomationSettings).HasForeignKey(y => y.CommunityId);
        }
        public void RegistTo(ConfigurationRegistrar configurations)
        {
            configurations.Add(this);
        }
    }
}
