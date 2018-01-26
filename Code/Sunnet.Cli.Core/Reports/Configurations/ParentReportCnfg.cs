using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Data.Entity.ModelConfiguration.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sunnet.Cli.Core.Reports.Entities;
using Sunnet.Framework.Core.Base;

namespace Sunnet.Cli.Core.Reports.Configurations
{
    public class ParentReportCnfg : EntityTypeConfiguration<ParentReportEntity>, IEntityMapper
    {
        public ParentReportCnfg()
        {
            ToTable("ParentReports");
        }

        public void RegistTo(ConfigurationRegistrar configurations)
        {
            configurations.Add(this);
        }
    }
}
