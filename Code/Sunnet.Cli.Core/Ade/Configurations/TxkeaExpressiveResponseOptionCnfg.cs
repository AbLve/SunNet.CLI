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


namespace Sunnet.Cli.Core.Ade.Configurations
{
    internal class TxkeaExpressiveResponseOptionCnfg : EntityTypeConfiguration<TxkeaExpressiveResponseOptionEntity>, IEntityMapper
    {
        public TxkeaExpressiveResponseOptionCnfg()
        {
            ToTable("TxkeaExpressiveResponseOptions");
            HasRequired(x => x.Response).WithMany(x => x.Options).HasForeignKey(x => x.ResponseId).WillCascadeOnDelete(true);

            Ignore(x => x.UpdatedOn);
            Ignore(x => x.CreatedOn);
        }
        public void RegistTo(ConfigurationRegistrar configurations)
        {
            configurations.Add(this);
        }
    }
}
