using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Data.Entity.ModelConfiguration.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Sunnet.Cli.Core.Ade.Entities;
using Sunnet.Framework.Core.Base;

/**************************************************************************
 * Developer: 		Jack
 * Computer:		Jackz
 * Domain:			Jackz
 * CreatedOn:		08/11/2014 03:39:16
 * Description:		CutOffScoreEntity 's Configuration
 * Version History:	Created,08/11/2014 03:39:16
 * 
 * 
 **************************************************************************/
namespace Sunnet.Cli.Core.Ade.Configurations
{
    internal class CutOffScoreCnfg : EntityTypeConfiguration<CutOffScoreEntity>, IEntityMapper
    {
        public CutOffScoreCnfg()
        {
            ToTable("CutOffScores");
            HasRequired(x => x.Benchmark).WithMany().HasForeignKey(x => x.BenchmarkId);
        }
        public void RegistTo(ConfigurationRegistrar configurations)
        {
            configurations.Add(this);
        }
    }
}
