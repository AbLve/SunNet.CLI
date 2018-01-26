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
 * CreatedOn:		2014/9/16 14:06:20
 * Description:		Create TeacherRoleCnfg
 * Version History:	Created,2014/9/16 14:06:20
 * 
 * 
 **************************************************************************/
using Sunnet.Cli.Core.Users.Entities;
using Sunnet.Framework.Core.Base;

namespace Sunnet.Cli.Core.Users.Configurations
{
    public class TeacherRoleCnfg : EntityTypeConfiguration<TeacherRoleEntity>, IEntityMapper
    {
        public TeacherRoleCnfg()
        {
            ToTable("TeacherRoles");
        }

        public void RegistTo(ConfigurationRegistrar configurations)
        {
            configurations.Add(this);
        }
    }
}
