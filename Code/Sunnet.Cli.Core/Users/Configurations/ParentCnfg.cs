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
 * Developer: 		Sam
 * Computer:		SAM-PC
 * Domain:			sam-pc
 * CreatedOn:		2014/8/5 16:25:27
 * Description:		Please input class summary
 * Version History:	Created,2014/8/5 16:25:27
 * 
 * 
 **************************************************************************/
namespace Sunnet.Cli.Core.Users.Configurations
{
    public class ParentCnfg : EntityTypeConfiguration<ParentEntity>, IEntityMapper
    {
        public ParentCnfg()
        {
        
            ToTable("Parents");
           HasRequired(x => x.UserInfo).WithOptional(o => o.Parent).Map(conf => conf.MapKey("UserId")); 
            Ignore(x => x.UpdatedOn);
            Ignore(x => x.CreatedOn);
        }
        public void RegistTo(ConfigurationRegistrar configurations)
        {
            ConfigurationRegistrar configurationRegistrar = configurations.Add(this);
        }
    }
}
