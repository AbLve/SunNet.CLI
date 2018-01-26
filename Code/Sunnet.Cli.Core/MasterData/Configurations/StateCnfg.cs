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
 * Description:		Create StateCnfg
 * Version History:	Created,2014/8/19 16:27:20
 * 
 * 
 **************************************************************************/
using Sunnet.Cli.Core.MasterData.Entities;
using Sunnet.Framework.Core.Base;

namespace Sunnet.Cli.Core.MasterData.Configurations
{
    public class StateCnfg : EntityTypeConfiguration<StateEntity>, IEntityMapper
    {
        public StateCnfg()
        {
            ToTable("States");

            HasMany(o => o.Communities).WithRequired(c => c.State).HasForeignKey(c => c.StateId);
            HasMany(o => o.BasicSchools).WithRequired(c => c.State).HasForeignKey(c => c.StateId);
        }

        public void RegistTo(ConfigurationRegistrar configurations)
        {
            configurations.Add(this);
        }
    }
}
