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
    public class UserComSchRelationCnfg : EntityTypeConfiguration<UserComSchRelationEntity>, IEntityMapper
    {
        public UserComSchRelationCnfg()
        {
            ToTable("UserComSchRelations");
            HasRequired(x => x.User).WithMany(x => x.UserCommunitySchools).HasForeignKey(x => x.UserId);
        }

        public void RegistTo(ConfigurationRegistrar configurations)
        {
            configurations.Add(this);
        }
    }
}
