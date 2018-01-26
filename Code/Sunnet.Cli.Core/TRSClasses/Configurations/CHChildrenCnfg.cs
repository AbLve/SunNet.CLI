using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Data.Entity.ModelConfiguration.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sunnet.Cli.Core.TRSClasses.Entites;
using Sunnet.Framework.Core.Base;

namespace Sunnet.Cli.Core.TRSClasses.Configurations
{
    public class CHChildrenCnfg:EntityTypeConfiguration<CHChildrenEntity>,IEntityMapper
    {
        public CHChildrenCnfg()
        {
            ToTable("CHChildrens"); 
        }

        public void RegistTo(ConfigurationRegistrar configurations)
        {
            configurations.Add(this);
        }
    }
}
