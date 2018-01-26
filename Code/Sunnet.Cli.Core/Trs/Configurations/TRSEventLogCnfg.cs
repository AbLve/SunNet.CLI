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
    public class TRSEventLogCnfg : EntityTypeConfiguration<TRSEventLogEntity>, IEntityMapper
    {
        public TRSEventLogCnfg()
        {
            ToTable("TRSEventLogs");
        }

        public void RegistTo(ConfigurationRegistrar configurations)
        {
            configurations.Add(this);
        }
    }
}
