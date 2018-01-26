using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/**************************************************************************
 * Developer: 		Joe
 * Computer:		JOE-PC
 * Domain:			Joe
 * CreatedOn:		2015/10/23
 * Description:		
 * Version History:	Created,2015/10/23
 * 
 * 
 **************************************************************************/
using System.Data.Entity.ModelConfiguration;
using Sunnet.Framework.Core.Base;
using Sunnet.Cli.Core.Ade.Entities;
using System.Data.Entity.ModelConfiguration.Configuration;

namespace Sunnet.Cli.Core.Ade.Configurations.Items
{
    internal class TxkeaReceptiveItemCnfg : EntityTypeConfiguration<TxkeaReceptiveItemEntity>, IEntityMapper
    {
        public TxkeaReceptiveItemCnfg()
        {
            ToTable("TxkeaReceptiveItems");
            HasRequired(r => r.Layout).WithMany(x => x.TxkeaReceptiveItems).HasForeignKey(y => y.LayoutId);
        }
        public void RegistTo(ConfigurationRegistrar configurations)
        {
            configurations.Add(this);
        }
    }
}
