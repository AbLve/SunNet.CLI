using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/**************************************************************************
 * Developer: 		Joe
 * Computer:		Joe-PC
 * Domain:			Joe-pc
 * CreatedOn:		2015/7/22
 * Description:		Create CountyEntity
 * Version History:	Created,2015/7/22
 * 
 * 
 **************************************************************************/
using Sunnet.Framework.Core.Base;
using System.Data.Entity.ModelConfiguration;
using Sunnet.Cli.Core.Permission.Entities;
using System.Data.Entity.ModelConfiguration.Configuration;

namespace Sunnet.Cli.Core.Permission.Configurations
{
    public class DisabledUserRoleCnfg : EntityTypeConfiguration<DisabledUserRoleEntity>, IEntityMapper
    {
        public DisabledUserRoleCnfg()
        {
            ToTable("Permission_DisabledUserRoles");

            HasRequired(r => r.User).WithMany(x => x.DisabledUsrRoles).HasForeignKey(y => y.UserId);
        }

        public void RegistTo(ConfigurationRegistrar configurations)
        {
            configurations.Add(this);
        }
    }
}
