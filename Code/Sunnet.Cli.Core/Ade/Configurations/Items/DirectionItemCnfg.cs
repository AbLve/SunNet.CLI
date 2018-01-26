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
 * Domain:			Jackz
 * CreatedOn:		2014/8/28 1:21:55
 * Description:		Please input class summary
 * Version History:	Created,2014/8/28 1:21:55
 * 
 * 
 **************************************************************************/
using Sunnet.Cli.Core.Ade.Entities;
using Sunnet.Framework.Core.Base;

namespace Sunnet.Cli.Core.Ade.Configurations
{

    internal class DirectionItemCnfg : EntityTypeConfiguration<DirectionItemEntity>, IEntityMapper
    {
        public DirectionItemCnfg()
        {
            ToTable("DirectionItems");
        }
        public void RegistTo(ConfigurationRegistrar configurations)
        {
            configurations.Add(this);
        }
    }
}
