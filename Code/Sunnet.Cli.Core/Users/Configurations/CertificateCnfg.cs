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
    public class CertificateCnfg : EntityTypeConfiguration<CertificateEntity>, IEntityMapper
    {
        public CertificateCnfg()
        {
            ToTable("Certificates");
            HasMany(r => r.Users).WithMany(r => r.Certificates).Map(
                m => { m.MapLeftKey("CertificateId"); m.MapRightKey("UserId"); m.ToTable("UserCertificateRelations"); }
                );
        }

        public void RegistTo(ConfigurationRegistrar configurations)
        {
            configurations.Add(this);
        }
    }
}
