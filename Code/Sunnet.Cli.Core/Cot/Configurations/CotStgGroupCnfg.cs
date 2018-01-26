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
    public class CotStgGroupCnfg : EntityTypeConfiguration<CotStgGroupEntity>, IEntityMapper
    {
        public CotStgGroupCnfg()
        {
            HasRequired(x => x.CotStgReport).WithMany(x => x.CotStgGroups).HasForeignKey(x => x.CotStgReportId);
            ToTable("CotStgGroups");
            Ignore(e => e.CreatedOn);
            Ignore(e => e.UpdatedOn);
        }

        public void RegistTo(ConfigurationRegistrar configurations)
        {
            configurations.Add(this);
        }
    }
}
