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
    public class TRSEventLogFileCnfg : EntityTypeConfiguration<TRSEventLogFileEntity>, IEntityMapper
    {
        public TRSEventLogFileCnfg()
        {
            ToTable("TRSEventLogFiles");
            HasRequired(r => r.EventLog).WithMany(e => e.EventLogFiles).HasForeignKey(x => x.EventLogId);
        }

        public void RegistTo(ConfigurationRegistrar configurations)
        {
            configurations.Add(this);
        }
    }
}
