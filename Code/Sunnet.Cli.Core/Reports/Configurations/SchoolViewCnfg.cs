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
 * CreatedOn:		2/3 2015 17:20:14
 * Description:		Please input class summary
 * Version History:	Created,2/3 2015 17:20:14
 * 
 * 
 **************************************************************************/
using Sunnet.Cli.Core.Reports.Entities;
using Sunnet.Framework.Core.Base;

namespace Sunnet.Cli.Core.Reports.Configurations
{
    public class SchoolViewCnfg : EntityTypeConfiguration<SchoolViewEntity>, IEntityMapper
    {
        public SchoolViewCnfg()
        {
            ToTable("V_Schools");
        }
        public void RegistTo(ConfigurationRegistrar configurations)
        {
            configurations.Add(this);
        }
    }
}
