using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Data.Entity.ModelConfiguration.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sunnet.Cli.Core.Students.Entities;
using Sunnet.Framework.Core.Base;

namespace Sunnet.Cli.Core.Students.Configurations
{
    public class StudentRoleCnfg : EntityTypeConfiguration<StudentRoleEntity>, IEntityMapper
    {
        public StudentRoleCnfg()
        {
            ToTable("StudentRoles");      
        }

        public void RegistTo(ConfigurationRegistrar configurations)
        {
            configurations.Add(this);
        }
    }
}
