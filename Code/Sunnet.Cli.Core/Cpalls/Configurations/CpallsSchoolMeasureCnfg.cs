using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Sunnet.Cli.Core.Cpalls.Entities;
using Sunnet.Framework.Core.Base;
using System.Data.Entity.ModelConfiguration;
using System.Data.Entity.ModelConfiguration.Configuration;

namespace Sunnet.Cli.Core.Cpalls.Configurations
{
    public class CpallsSchoolMeasureCnfg : EntityTypeConfiguration<CpallsSchoolMeasureEntity>, IEntityMapper
    {
        public CpallsSchoolMeasureCnfg()
        {
            ToTable("CPallsSchoolMeasures");

            HasRequired(r => r.CpallsSchool).WithMany(r => r.CpallsSchoolMeasures).HasForeignKey(r => r.CpallsSchoolId);
            HasRequired(r => r.Measure).WithMany().HasForeignKey(r => r.MeasureId);
        }

        public void RegistTo(ConfigurationRegistrar configurations)
        {
            configurations.Add(this);
        }
    }
}