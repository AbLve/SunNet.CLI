using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Sunnet.Framework.Core.Base;
using Sunnet.Cli.Core.DataProcess.Entities;
using System.Data.Entity.ModelConfiguration.Configuration;

namespace Sunnet.Cli.Core.DataProcess.Configurations
{
    public class DataSchoolCnfg : EntityTypeConfiguration<DataSchoolEntity>, IEntityMapper
    {
        public DataSchoolCnfg()
        {
            ToTable("DataSchools");
            Ignore(r => r.CreatedOn);
            Ignore(r => r.UpdatedOn);
        }

        public void RegistTo(ConfigurationRegistrar configurations)
        {
            configurations.Add(this);
        }
    }
}