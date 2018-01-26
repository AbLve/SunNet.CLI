using Sunnet.Cli.Core.Log.Entities;
using Sunnet.Framework.Core.Base;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Data.Entity.ModelConfiguration.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sunnet.Cli.Core.Log.Configurations
{
    public class OperationLogCnfg : EntityTypeConfiguration<OperationLogEntity>, IEntityMapper
    {
        public OperationLogCnfg()
        {
            ToTable("OperationLogs");
            Ignore(r => r.UpdatedOn);
        }

        public void RegistTo(ConfigurationRegistrar configurations)
        {
            configurations.Add(this);
        }
    }
}
