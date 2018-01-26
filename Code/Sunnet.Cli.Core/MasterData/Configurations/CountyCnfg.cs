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
 * Domain:			Leason-pc
 * CreatedOn:		2014/8/19 16:27:20
 * Description:		Create CountyCnfg
 * Version History:	Created,2014/8/19 16:27:20
 * 
 * 
 **************************************************************************/
using Sunnet.Cli.Core.MasterData.Entities;
using Sunnet.Framework.Core.Base;

namespace Sunnet.Cli.Core.MasterData.Configurations
{
    public class CountyCnfg : EntityTypeConfiguration<CountyEntity>, IEntityMapper
    {
        public CountyCnfg()
        {
            ToTable("Counties");
            HasMany(o => o.Communities).WithRequired(c => c.County).HasForeignKey(c => c.CountyId);
            HasMany(o => o.Schools).WithRequired(c => c.County).HasForeignKey(c => c.CountyId);
        }

        public void RegistTo(ConfigurationRegistrar configurations)
        {
            configurations.Add(this);
        }
    }
}
