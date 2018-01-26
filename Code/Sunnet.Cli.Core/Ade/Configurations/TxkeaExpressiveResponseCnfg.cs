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
    internal class TxkeaExpressiveResponseCnfg : EntityTypeConfiguration<TxkeaExpressiveResponseEntity>, IEntityMapper
    {
        public TxkeaExpressiveResponseCnfg()
        {
            ToTable("TxkeaExpressiveResponses");
            HasRequired(x => x.Item).WithMany(x => x.Responses).HasForeignKey(x => x.ItemId).WillCascadeOnDelete(true);

            Ignore(x => x.UpdatedOn);
            Ignore(x => x.CreatedOn);
        }
        public void RegistTo(ConfigurationRegistrar configurations)
        {
            configurations.Add(this);
        }
    }
}
