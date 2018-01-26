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
    public class Sy_UserBaseCnfg : EntityTypeConfiguration<Sy_UserBaseEntity>, IEntityMapper
    {
        public Sy_UserBaseCnfg()
        {
            ToTable("Cli_Engage__Users");

            //HasRequired(e => e.Role).WithMany(e => e.users);
        }
        public void RegistTo(ConfigurationRegistrar configurations)
        {
            configurations.Add(this);
        }
    }
}
