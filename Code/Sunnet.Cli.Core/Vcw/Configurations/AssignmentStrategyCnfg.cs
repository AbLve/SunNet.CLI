using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/**************************************************************************
 * Developer: 		Joe
 * Computer:		Joe-PC
 * Domain:			Joe-pc
 * CreatedOn:		2015/3/6
 * Description:		Create AuthorityEntity
 * Version History:	Created,2015/3/6
 * 
 * 
 **************************************************************************/
using System.Data.Entity.ModelConfiguration;
using Sunnet.Cli.Core.Vcw.Entities;
using Sunnet.Framework.Core.Base;
using System.Data.Entity.ModelConfiguration.Configuration;

namespace Sunnet.Cli.Core.Vcw.Configurations
{
    public class AssignmentStrategyCnfg : EntityTypeConfiguration<AssignmentStrategyEntity>, IEntityMapper
    {
        public AssignmentStrategyCnfg()
        {
            ToTable("AssignmentStrategies");
            Ignore(r => r.UpdatedOn);
            HasRequired(a => a.Assignment).WithMany(b => b.AssignmentStrategies);
        }

        public void RegistTo(ConfigurationRegistrar configurations)
        {
            configurations.Add(this);
        }
    }
}
