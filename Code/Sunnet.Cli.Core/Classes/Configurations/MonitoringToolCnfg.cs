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
 * CreatedOn:		2014/8/27 16:16:16
 * Description:		Create MonitoringToolCnfg
 * Version History:	Created,2014/8/27 16:16:16
 * 
 * 
 **************************************************************************/
using Sunnet.Cli.Core.Classes.Entites;
using Sunnet.Framework.Core.Base;

namespace Sunnet.Cli.Core.Classes.Configurations
{
    public class MonitoringToolCnfg : EntityTypeConfiguration<MonitoringToolEntity>, IEntityMapper
    {
        public MonitoringToolCnfg()
        {
            ToTable("MonitoringTools");

            HasMany(o => o.Classes).WithRequired(c => c.MonitoringTool).HasForeignKey(c => c.MonitoringToolId);
        }

        public void RegistTo(ConfigurationRegistrar configurations)
        {
            configurations.Add(this);
        }
    }
}
