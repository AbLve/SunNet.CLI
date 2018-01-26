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
    public class ParentStudentRelationCnfg : EntityTypeConfiguration<ParentStudentRelationEntity>, IEntityMapper
    {
        public ParentStudentRelationCnfg()
        {
            ToTable("ParentStudentRelations");
            Ignore(x => x.UpdatedOn);
            Ignore(x => x.CreatedOn);
            HasRequired(e => e.Parent).WithMany(e => e.ParentStudents).HasForeignKey(e => e.ParentId);
            HasRequired(e => e.Student).WithMany(e => e.ParentStudents).HasForeignKey(e => e.StudentId);
        }
        public void RegistTo(ConfigurationRegistrar configurations)
        {
            ConfigurationRegistrar configurationRegistrar = configurations.Add(this);
        }
    }
}
