using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/**************************************************************************
 * Developer: 		Joe
 * Computer:		Joe-PC
 * Domain:			Joe-pc
 * CreatedOn:		2015/7/13
 * Description:		Create AuthorityEntity
 * Version History:	Created,2015/7/13
 * 
 * 
 **************************************************************************/
using System.Data.Entity.ModelConfiguration;
using Sunnet.Framework.Core.Base;
using Sunnet.Cli.Core.Users.Entities;
using System.Data.Entity.ModelConfiguration.Configuration;

namespace Sunnet.Cli.Core.Users.Configurations
{
    public class IntManaCoachRelationCnfg : EntityTypeConfiguration<IntManaCoachRelationEntity>, IEntityMapper
    {
        public IntManaCoachRelationCnfg()
        {
            HasRequired(r => r.User).WithMany(x => x.IntManaCoachRelations).HasForeignKey(r => r.CoordCoachUserId);
            ToTable("IntManaCoordCoachRelations");
        }
        public void RegistTo(ConfigurationRegistrar configuration)
        {
            configuration.Add(this);
        }
    }
}
