using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Data.Entity.ModelConfiguration.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/**************************************************************************
 * Developer: 		JackZhang
 * Computer:		JACKZ
 * Domain:			Jackz
 * CreatedOn:		2014/12/16 9:42:13
 * Description:		Please input class summary
 * Version History:	Created,2014/12/16 9:42:13
 * 
 * 
 **************************************************************************/
using Sunnet.Cli.Core.Cot.Entities;
using Sunnet.Framework.Core.Base;

namespace Sunnet.Cli.Core.Cot.Configurations
{
    internal class CotWaveCnfg : EntityTypeConfiguration<CotWaveEntity>, IEntityMapper
    {
        public CotWaveCnfg()
        {
            HasRequired(x => x.Assessment).WithMany(x => x.Waves).HasForeignKey(x => x.CotAssessmentId);
            ToTable("CotWaves");
        }
        public void RegistTo(ConfigurationRegistrar configurations)
        {
            configurations.Add(this);
        }
    }
}
