using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/**************************************************************************
 * Developer: 		Joe
 * Computer:		Joe-PC
 * Domain:			Joe-pc
 * CreatedOn:		2015/4/20
 * Description:		Create AuthorityEntity
 * Version History:	Created,2015/4/20
 * 
 * 
 **************************************************************************/
using System.Data.Entity.ModelConfiguration;
using Sunnet.Cli.Core.Vcw.Entities;
using Sunnet.Framework.Core.Base;
using System.Data.Entity.ModelConfiguration.Configuration;

namespace Sunnet.Cli.Core.Vcw.Configurations
{
    public class FileStrategyCnfg : EntityTypeConfiguration<FileStrategyEntity>, IEntityMapper
    {
        public FileStrategyCnfg()
        {
            ToTable("FileStrategies");
            Ignore(r => r.UpdatedOn);
            HasRequired(a => a.File).WithMany(b => b.FileStrategies).HasForeignKey(r => r.FileId);
        }

        public void RegistTo(ConfigurationRegistrar configurations)
        {
            configurations.Add(this);
        }
    }
}
