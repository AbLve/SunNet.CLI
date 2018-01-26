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
    internal class ReportQueueCnfg : EntityTypeConfiguration<ReportQueueEntity>, IEntityMapper
    {
        public ReportQueueCnfg()
        {
            ToTable("ReportQueue");
            Property(x => x.QueryParams).HasMaxLength(4000);
            Property(x => x.DownloadUrl).HasMaxLength(200);
            Property(x => x.Report).HasMaxLength(200);
            Property(x => x.Title).HasMaxLength(50);
        }

        public void RegistTo(ConfigurationRegistrar configurations)
        {
            configurations.Add(this);
        }


    }
}
