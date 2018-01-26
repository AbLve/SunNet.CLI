using Sunnet.Cli.Core.Ade.Entities;
using Sunnet.Framework.Core.Base;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Data.Entity.ModelConfiguration.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sunnet.Cli.Core.Ade.Configurations
{
    internal class BenchmarkCnfg : EntityTypeConfiguration<BenchmarkEntity>, IEntityMapper
    {
        public BenchmarkCnfg()
        {
            ToTable("Benchmarks");
        }
        public void RegistTo(ConfigurationRegistrar configurations)
        {
            configurations.Add(this);
        }
    }
}
