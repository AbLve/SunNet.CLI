using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Data.Entity.ModelConfiguration.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/**************************************************************************
 * Developer: 		Sam
 * Computer:		SAM-PC
 * Domain:			sam-pc
 * CreatedOn:		2014/8/19 8:55:27
 * Description:		Please input class summary
 * Version History:	Created,2014/8/19 8:55:27
 * 
 * 
 **************************************************************************/
using Sunnet.Cli.Core.Schools.Entities;
using Sunnet.Framework.Core.Base;

namespace Sunnet.Cli.Core.Schools.Configurations
{
    public class TrsProviderCnfg : EntityTypeConfiguration<TrsProviderEntity>, IEntityMapper
    {
        public TrsProviderCnfg()
        {
            ToTable("TrsProviders"); 
        }
        public void RegistTo(ConfigurationRegistrar configurations)
        {
            configurations.Add(this);
        }
    }
}
