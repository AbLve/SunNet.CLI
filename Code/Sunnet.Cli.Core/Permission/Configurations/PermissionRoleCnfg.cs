using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/**************************************************************************
 * Developer: 		Joe
 * Computer:		Joe-PC
 * Domain:			Joe-pc
 * CreatedOn:		2014/9/2 8:55:27
 * Description:		Please input class summary
 * Version History:	Created,2014/9/2 8:55:27
 * 
 * 
 **************************************************************************/
using Sunnet.Framework.Core.Base;
using System.Data.Entity.ModelConfiguration;
using Sunnet.Cli.Core.Permission.Entities;
using System.Data.Entity.ModelConfiguration.Configuration;

namespace Sunnet.Cli.Core.Permission.Configurations
{
    public class PermissionRoleCnfg : EntityTypeConfiguration<PermissionRoleEntity>, IEntityMapper
    {
        public PermissionRoleCnfg()
        {
            ToTable("Permission_Roles");

            HasMany(a => a.Users).WithMany(a => a.PermissionRoles)
                   .Map(a =>
                   {
                       a.MapLeftKey("RoleId");
                       a.MapRightKey("UserId");
                       a.ToTable("Permission_UserRole");
                   }
                   );
        }

        public void RegistTo(ConfigurationRegistrar configurations)
        {
            configurations.Add(this);
        }
    }
}
