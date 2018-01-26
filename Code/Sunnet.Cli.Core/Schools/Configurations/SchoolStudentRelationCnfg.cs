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
    internal class SchoolStudentRelationCnfg:EntityTypeConfiguration<SchoolStudentRelationEntity>,IEntityMapper
    {
        public SchoolStudentRelationCnfg()
        {
            ToTable("SchoolStudentRelations");
            HasRequired(x => x.School).WithMany(x => x.StudentRelations).HasForeignKey(x => x.SchoolId);
            HasRequired(x => x.Student).WithMany(x => x.SchoolRelations).HasForeignKey(x => x.StudentId);
        }

        public void RegistTo(ConfigurationRegistrar configurations)
        {
            configurations.Add(this);
        }
    }
}
