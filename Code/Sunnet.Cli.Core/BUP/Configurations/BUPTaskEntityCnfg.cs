using Sunnet.Cli.Core.BUP.Entities;
using Sunnet.Framework.Core.Base;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Data.Entity.ModelConfiguration.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sunnet.Cli.Core.BUP.Configurations
{
   public  class BUPTaskEntityCnfg: EntityTypeConfiguration<BUPTaskEntity>, IEntityMapper
    {
       public BUPTaskEntityCnfg()
        {
            ToTable("BUP_Tasks");

        }
        public void RegistTo(ConfigurationRegistrar configurations)
        {
            configurations.Add(this);
        }
    }
}
