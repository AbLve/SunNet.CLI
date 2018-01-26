using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/**************************************************************************
 * Developer: 		Joe
 * Computer:		Joe-PC
 * Domain:			Joe-pc
 * CreatedOn:		2014/11/3
 * Description:		Create AuthorityEntity
 * Version History:	Created,2014/11/3
 * 
 * 
 **************************************************************************/
using System.Data.Entity.ModelConfiguration;
using Sunnet.Cli.Core.Vcw.Entities;
using Sunnet.Framework.Core.Base;
using System.Data.Entity.ModelConfiguration.Configuration;

namespace Sunnet.Cli.Core.Vcw.Configurations
{
    public class AssignmentWatchFileCnfg : EntityTypeConfiguration<AssignmentWatchFileEntity>, IEntityMapper
    {
        public AssignmentWatchFileCnfg()
        {
            ToTable("AssignmentWatchFiles");
            Ignore(r => r.UpdatedOn);
        }

        public void RegistTo(ConfigurationRegistrar configurations)
        {
            configurations.Add(this);
        }
    }
}
