using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Data.Entity.ModelConfiguration.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sunnet.Cli.Core.CAC.Entities;
using Sunnet.Cli.Core.Classes.Entites;
using Sunnet.Framework.Core.Base;

namespace Sunnet.Cli.Core.CAC.Configurations
{
    public class MyActivityCnfg : EntityTypeConfiguration<MyActivityEntity>, IEntityMapper
    {
        public MyActivityCnfg()
        {
            ToTable("MyActivities"); 
        }

        public void RegistTo(ConfigurationRegistrar configurations)
        {
            configurations.Add(this);
        }
    }
    public class ActivityHistoryCnfg : EntityTypeConfiguration<ActivityHistoryEntity>, IEntityMapper
    {
        public ActivityHistoryCnfg()
        {
            ToTable("ActivityHistory");
        }

        public void RegistTo(ConfigurationRegistrar configurations)
        {
            configurations.Add(this);
        }
    }
}
