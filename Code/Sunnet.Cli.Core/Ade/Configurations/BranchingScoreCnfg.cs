using Sunnet.Cli.Core.Ade.Entities;
using Sunnet.Framework.Core.Base;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Data.Entity.ModelConfiguration.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sunnet.Cli.Core.Ade.Configurations
{
    internal class BranchingScoreCnfg : EntityTypeConfiguration<BranchingScoreEntity>, IEntityMapper
    {
        public BranchingScoreCnfg()
        {
            ToTable("BranchingScores");

            HasRequired(r => r.ItemBase).WithMany(r => r.BranchingScores).HasForeignKey(r=>r.ItemId);

        }
        public void RegistTo(ConfigurationRegistrar configurations)
        {
            configurations.Add(this);
        }
    }
}
