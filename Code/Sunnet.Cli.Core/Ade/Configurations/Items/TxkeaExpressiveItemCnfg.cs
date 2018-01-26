using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/**************************************************************************
 * Developer: 		Joe
 * Computer:		JOE-PC
 * Domain:			Joe
 * CreatedOn:		2015/10/28
 * Description:		
 * Version History:	Created,2015/10/28
 * 
 * 
 **************************************************************************/
using System.Data.Entity.ModelConfiguration;
using Sunnet.Framework.Core.Base;
using Sunnet.Cli.Core.Ade.Entities;
using System.Data.Entity.ModelConfiguration.Configuration;

namespace Sunnet.Cli.Core.Ade.Configurations.Items
{
    internal class TxkeaExpressiveItemCnfg : EntityTypeConfiguration<TxkeaExpressiveItemEntity>, IEntityMapper
    {
        public TxkeaExpressiveItemCnfg()
        {
            ToTable("TxkeaExpressiveItems");
            Ignore(r => r.Step);
            HasRequired(r => r.Layout).WithMany(x => x.TxkeaExpressiveItems).HasForeignKey(y => y.LayoutId);
        }
        public void RegistTo(ConfigurationRegistrar configurations)
        {
            configurations.Add(this);
        }
    }
}
