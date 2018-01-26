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
    public class AuditorCnfg : EntityTypeConfiguration<AuditorEntity>, IEntityMapper
    {
        public AuditorCnfg()
        {
            ToTable("Auditors");
            HasRequired(x => x.UserInfo).WithOptional(o => o.Auditor).Map(conf => conf.MapKey("UserId"));
            Ignore(x => x.UpdatedOn);
            Ignore(x => x.CreatedOn);
        }
        public void RegistTo(ConfigurationRegistrar configurations)
        {
            configurations.Add(this);
        }
    }
}
