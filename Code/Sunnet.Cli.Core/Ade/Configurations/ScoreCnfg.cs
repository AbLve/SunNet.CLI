using Sunnet.Cli.Core.Ade.Entities;
using Sunnet.Framework.Core.Base;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity.ModelConfiguration.Configuration;

namespace Sunnet.Cli.Core.Ade.Configurations
{
    internal class ScoreCnfg : EntityTypeConfiguration<ScoreEntity>, IEntityMapper
    {

        public ScoreCnfg()
        {
            ToTable("Scores");
        }
        public void RegistTo(ConfigurationRegistrar configurations)
        {
            configurations.Add(this);
        }


    }
}
