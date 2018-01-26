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
    public class TeacherTransactionCnfg : EntityTypeConfiguration<TeacherTransactionEntity>, IEntityMapper
    {
        public TeacherTransactionCnfg()
        {
            ToTable("TeacherTransactions");
            HasRequired(e => e.Teacher).WithMany(e => e.TeacherTransactions).HasForeignKey(e => e.TeacherId);
        }

        public void RegistTo(ConfigurationRegistrar configurations)
        {
            configurations.Add(this);
        }
    }
}
