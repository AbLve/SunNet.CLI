using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sunnet.Cli.Core.Practices.Entites;
using Sunnet.Framework.Core.Base;

namespace Sunnet.Cli.Core.Practices.Configurations
{
    internal class PracticeGroupMyActivityCnfg : EntityTypeConfiguration<PracticeGroupMyActivityEntity>, IEntityMapper
    {
        public PracticeGroupMyActivityCnfg()
        {
            ToTable("PracticeGroupMyActivities");
            HasRequired(r => r.Group).WithMany(r => r.Activities).HasForeignKey(r => r.GroupId);
        }

        public void RegistTo(System.Data.Entity.ModelConfiguration.Configuration.ConfigurationRegistrar configurations)
        {
            configurations.Add(this);
        }
    }
}
