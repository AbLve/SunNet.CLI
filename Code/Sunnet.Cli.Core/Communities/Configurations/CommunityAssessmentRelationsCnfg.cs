using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Data.Entity.ModelConfiguration.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sunnet.Cli.Core.Communities.Entities;
using Sunnet.Framework.Core.Base;

namespace Sunnet.Cli.Core.Communities.Configurations
{
    public class CommunityAssessmentRelationsCnfg : EntityTypeConfiguration<CommunityAssessmentRelationsEntity>, IEntityMapper
    {
        public CommunityAssessmentRelationsCnfg()
        {
            ToTable("CommunityAssessmentRelations");
            Ignore(o => o.AssessmentName);
        }
        public void RegistTo(ConfigurationRegistrar configurations)
        {
            configurations.Add(this);
        }
    }
}
