using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Data.Entity.ModelConfiguration.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sunnet.Cli.Core.Trs.Entities;
using Sunnet.Framework.Core.Base;

namespace Sunnet.Cli.Core.Trs.Configurations
{
    public class TRSNotificationCnfg : EntityTypeConfiguration<TRSNotificationEntity>, IEntityMapper
    {
        public TRSNotificationCnfg()
        {
            ToTable("TRSNotifications");
            Ignore(r => r.UpdatedOn);
            Ignore(r => r.CreatedOn);

            HasRequired(r => r.EventLog).WithMany(e => e.Notifications).HasForeignKey(x => x.EventLogId);
        }

        public void RegistTo(ConfigurationRegistrar configurations)
        {
            configurations.Add(this);
        }
    }
}
