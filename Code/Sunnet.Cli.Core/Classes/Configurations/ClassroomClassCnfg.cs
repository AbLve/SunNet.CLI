using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sunnet.Framework.Core.Base;
using Sunnet.Cli.Core.Classes.Entites;
using System.Data.Entity.ModelConfiguration.Configuration;

namespace Sunnet.Cli.Core.Classes.Configurations
{
    public class ClassroomClassCnfg : EntityTypeConfiguration<ClassroomClassEntity>, IEntityMapper
    {
        public ClassroomClassCnfg()
        {
            ToTable("ClassroomClassRelations");
            HasRequired(r => r.Class).WithMany(c => c.ClassroomClasses).HasForeignKey(r => r.ClassId);
            HasRequired(r => r.Classroom).WithMany(c => c.ClassroomClasses).HasForeignKey(r => r.ClassroomId);
            
        }

        public void RegistTo(ConfigurationRegistrar configurations)
        {
            configurations.Add(this);
        }
    }
}
