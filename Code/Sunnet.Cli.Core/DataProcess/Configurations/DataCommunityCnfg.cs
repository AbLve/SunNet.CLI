using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity.ModelConfiguration.Configuration;

using Sunnet.Framework.Core.Base;
using Sunnet.Cli.Core.DataProcess.Entities;

namespace Sunnet.Cli.Core.DataProcess.Configurations
{
    public class DataCommunityCnfg : EntityTypeConfiguration<DataCommunityEntity>, IEntityMapper
    {
        public DataCommunityCnfg()
        {
            ToTable("DataCommunities");
            Ignore(r => r.CreatedOn);
            Ignore(r => r.UpdatedOn);
        }

        public void RegistTo(ConfigurationRegistrar configurations)
        {
            configurations.Add(this);
        }
    }
}
