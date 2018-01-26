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
 * CreatedOn:		2014/8/22 14:37:20
 * Description:		Create ClassroomCnfg
 * Version History:	Created,2014/8/22 14:37:20
 * 
 * 
 **************************************************************************/
using Sunnet.Cli.Core.Classrooms.Entites;
using Sunnet.Framework.Core.Base;

namespace Sunnet.Cli.Core.Classrooms.Configurations
{
    public class ClassroomRoleCnfg : EntityTypeConfiguration<ClassroomRoleEntity>, IEntityMapper
    {
        public ClassroomRoleCnfg()
        {
            ToTable("ClassroomRoles");
        }
        public void RegistTo(ConfigurationRegistrar configurations)
        {
            configurations.Add(this);
        }
    }
}
