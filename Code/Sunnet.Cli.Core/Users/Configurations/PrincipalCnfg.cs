using System.Data.Entity.ModelConfiguration.Configuration;
using Sunnet.Cli.Core.Users.Entities;
using Sunnet.Framework.Core.Base;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/**************************************************************************
 * Developer: 		Damon
 * Computer:		Damon-PC
 * Domain:			Damon-pc
 * CreatedOn:		2014/8/8 10:58:18
 * Description:		Please input class summary
 * Version History:	Created,2014/8/8 10:58:18
 * 
 * 
 **************************************************************************/

namespace Sunnet.Cli.Core.Users.Configurations
{
    public class PrincipalCnfg : EntityTypeConfiguration<PrincipalEntity>, IEntityMapper
    {
        public PrincipalCnfg()
        {
            ToTable("Principals");
            HasRequired(x => x.UserInfo).WithOptional(o => o.Principal).Map(conf => conf.MapKey("UserId"));
            
            Ignore(x => x.UpdatedOn);
            Ignore(x => x.CreatedOn);
        }
        public void RegistTo(ConfigurationRegistrar configuration)
        {
            configuration.Add(this);
        }
    }

}
