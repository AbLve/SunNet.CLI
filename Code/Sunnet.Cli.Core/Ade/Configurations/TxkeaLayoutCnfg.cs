using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/**************************************************************************
 * Developer: 		Joe
 * Computer:		JOE-PC
 * Domain:			Joe
 * CreatedOn:		2015/11/8
 * Description:		Add TxkeaReceptive Item
 * Version History:	Created,2015/11/8
 * 
 * 
 **************************************************************************/
using System.Data.Entity.ModelConfiguration;
using Sunnet.Cli.Core.Ade.Entities;
using Sunnet.Framework.Core.Base;
using System.Data.Entity.ModelConfiguration.Configuration;

namespace Sunnet.Cli.Core.Ade.Configurations
{
    internal class TxkeaLayoutCnfg : EntityTypeConfiguration<TxkeaLayoutEntity>, IEntityMapper
    {
        public TxkeaLayoutCnfg()
        {
            ToTable("TxkeaLayouts");
        }
        public void RegistTo(ConfigurationRegistrar configurations)
        {
            configurations.Add(this);
        }
    }
}
