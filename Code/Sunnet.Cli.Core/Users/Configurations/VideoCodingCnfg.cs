using Sunnet.Cli.Core.Users.Entities;
using Sunnet.Framework.Core.Base;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Data.Entity.ModelConfiguration.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sunnet.Cli.Core.Users.Configurations
{
    public class VideoCodingCnfg : EntityTypeConfiguration<VideoCodingEntity>, IEntityMapper
    {
        public VideoCodingCnfg()
        {
            ToTable("VideoCodings");
            HasRequired(x => x.User).WithOptional(o => o.VideoCoding).Map(conf => conf.MapKey("UserId"));
            Ignore(x => x.UpdatedOn);
            Ignore(x => x.CreatedOn);
        }

        public void RegistTo(ConfigurationRegistrar configurations)
        {
            configurations.Add(this);
        }
    }
}
