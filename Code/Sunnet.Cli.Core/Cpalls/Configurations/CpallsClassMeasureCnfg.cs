using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Sunnet.Cli.Core.Cpalls.Entities;
using Sunnet.Framework.Core.Base;
using System.Data.Entity.ModelConfiguration;
using System.Data.Entity.ModelConfiguration.Configuration;

namespace Sunnet.Cli.Core.Cpalls.Configurations
{
    class CpallsClassMeasureCnfg : EntityTypeConfiguration<CpallsClassMeasureEntity>, IEntityMapper
    {
        public CpallsClassMeasureCnfg()
        {
            ToTable("CpallsClassMeasures");
        }

        public void RegistTo(ConfigurationRegistrar configurations)
        {
            configurations.Add(this);
        }
    }
}