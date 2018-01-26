using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Data.Entity.ModelConfiguration.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sunnet.Cli.Core.Schools.Entities;
using Sunnet.Framework.Core.Base;

namespace Sunnet.Cli.Core.Schools.Configurations
{
    public class CommunitySchoolRelationsCnfg : EntityTypeConfiguration<CommunitySchoolRelationsEntity>, IEntityMapper
    {
        public CommunitySchoolRelationsCnfg()
        {
            ToTable("CommunitySchoolRelations");
             
        }
        public void RegistTo(ConfigurationRegistrar configurations)
        {
            configurations.Add(this);
        }
    }
}
