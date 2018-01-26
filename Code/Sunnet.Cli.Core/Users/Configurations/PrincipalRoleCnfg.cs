using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Data.Entity.ModelConfiguration.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sunnet.Cli.Core.Users.Entities;
using Sunnet.Framework.Core.Base;

namespace Sunnet.Cli.Core.Users.Configurations
{
    public class PrincipalRoleCnfg : EntityTypeConfiguration<PrincipalRoleEntity>, IEntityMapper
    {
        public PrincipalRoleCnfg()
        {
            ToTable("PrincipalRoles");
        }
        public void RegistTo(ConfigurationRegistrar configuration)
        {
            configuration.Add(this);
        }
    }
}
