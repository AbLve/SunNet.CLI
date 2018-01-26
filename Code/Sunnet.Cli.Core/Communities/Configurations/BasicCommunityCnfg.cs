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
 * CreatedOn:		2014/8/22 17:33:00
 * Description:		Create BasicCommunityCnfg
 * Version History:	Created,2014/8/22 17:33:00
 * 
 * 
 **************************************************************************/
using Sunnet.Cli.Core.Communities.Entities;
using Sunnet.Framework.Core.Base;

namespace Sunnet.Cli.Core.Communities.Configurations
{
    public class BasicCommunityCnfg : EntityTypeConfiguration<BasicCommunityEntity>, IEntityMapper
    {
        public BasicCommunityCnfg()
        {
            ToTable("BasicCommunities");

            HasMany(o => o.Communities).WithRequired(c => c.BasicCommunity).HasForeignKey(c => c.BasicCommunityId);
        }
        public void RegistTo(ConfigurationRegistrar configurations)
        {
            configurations.Add(this);
        }
    }
}
