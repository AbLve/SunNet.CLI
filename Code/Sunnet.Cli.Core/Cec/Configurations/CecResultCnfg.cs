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
 * Domain:			Leason
 * CreatedOn:		2014/12/1 11:20:00
 * Description:		CecResultCnfg
 * Version History:	Created,2014/12/1 11:20:00
 * 
 * 
 **************************************************************************/
using Sunnet.Cli.Core.Cec.Entities;
using Sunnet.Framework.Core.Base;

namespace Sunnet.Cli.Core.Cec.Configurations
{
    public class CecResultCnfg: EntityTypeConfiguration<CecResultEntity>, IEntityMapper
    {
        public CecResultCnfg()
        {
            ToTable("CecResults");
        }

        public void RegistTo(ConfigurationRegistrar configurations)
        {
            configurations.Add(this);
        }
    }
}
