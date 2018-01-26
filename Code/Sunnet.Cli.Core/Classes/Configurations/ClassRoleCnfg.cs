using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Data.Entity.ModelConfiguration.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/**************************************************************************
 * Developer: 		Leason
 * Computer:		Leason-PC
 * Domain:			Leason-pc
 * CreatedOn:		2014/9/10 16:21:20
 * Description:		Create ClassRoleCnfg
 * Version History:	Created,2014/9/10 16:21:20
 * 
 * 
 **************************************************************************/
using Sunnet.Cli.Core.Classes.Entites;
using Sunnet.Framework.Core.Base;

namespace Sunnet.Cli.Core.Classes.Configurations
{
    public class ClassRoleCnfg : EntityTypeConfiguration<ClassRoleEntity>, IEntityMapper
    {
        public ClassRoleCnfg() 
        {
            ToTable("ClassRoles");
        }

        public void RegistTo(ConfigurationRegistrar configurations)
        {
            configurations.Add(this);
        }
    }
}
