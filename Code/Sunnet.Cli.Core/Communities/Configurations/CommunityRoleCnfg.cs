using System.Data.Entity.ModelConfiguration;
using System.Data.Entity.ModelConfiguration.Configuration;
/**************************************************************************
 * Developer: 		Leason
 * Computer:		Leason-PC
 * Domain:			Leason-pc
 * CreatedOn:		2014/8/18 16:27:20
 * Description:		Create CommunitiesRspt
 * Version History:	Created,2014/8/18 16:27:20
 * 
 * 
 **************************************************************************/
using Sunnet.Cli.Core.Communities.Entities;
using Sunnet.Framework.Core.Base;

namespace Sunnet.Cli.Core.Communities.Configurations
{
    public class CommunityRoleCnfg : EntityTypeConfiguration<CommunityRoleEntity>, IEntityMapper
    {
        public CommunityRoleCnfg() 
        {
            ToTable("CommunityRoles");
        }

        public void RegistTo(ConfigurationRegistrar configurations)
        {
            configurations.Add(this);
        }
    }
}
