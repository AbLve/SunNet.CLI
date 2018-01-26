using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Sunnet.Cli.Core.Vcw.Entities;
using Sunnet.Framework.Core.Base;
using System.Data.Entity.ModelConfiguration.Configuration;

namespace Sunnet.Cli.Core.Vcw.Configurations
{
    public class V_TeacherCnfg : EntityTypeConfiguration<V_TeacherEntity>, IEntityMapper
    {
        public V_TeacherCnfg()
        {
            ToTable("dbo.Cli_Engage__Teachers");
            HasRequired(x => x.UserInfo).WithOptional(o => o.TeacherInfo).Map(conf => conf.MapKey("UserId"));
            Ignore(x => x.UpdatedOn);
            Ignore(x => x.CreatedOn);
        }

        public void RegistTo(ConfigurationRegistrar configurations)
        {
            configurations.Add(this);
        }
    }
}
