using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Data.Entity.ModelConfiguration.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/**************************************************************************
 * Developer: 		JackZhang
 * Computer:		JACKZ
 * Domain:			JackZ
 * CreatedOn:		2/13 2015 11:37:23
 * Description:		Please input class summary
 * Version History:	Created,2/13 2015 11:37:23
 * 
 * 
 **************************************************************************/
using Sunnet.Cli.Core.Users.Entities;
using Sunnet.Framework.Core.Base;

namespace Sunnet.Cli.Core.Users.Configurations
{
    public class UserClassRelationCnfg : EntityTypeConfiguration<UserClassRelationEntity>, IEntityMapper
    {
        public UserClassRelationCnfg()
        {
            ToTable("UserClassRelations");
            HasRequired(x => x.User).WithMany(x => x.UserClasses).HasForeignKey(x => x.UserId);
            HasRequired(x => x.Class).WithMany(x => x.UserClasses).HasForeignKey(x => x.ClassId);
        }

        public void RegistTo(ConfigurationRegistrar configurations)
        {
            configurations.Add(this);
        }
    }
}
