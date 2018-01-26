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
    public class TeacherEquipmentRelationCnfg : EntityTypeConfiguration<TeacherEquipmentRelationEntity>, IEntityMapper
    {
        public TeacherEquipmentRelationCnfg()
        {
            ToTable("TeacherEquipmentRelations");
            HasRequired(m => m.Teacher).WithMany(n => n.TeacherEquipmentRelations);
        }

        public void RegistTo(ConfigurationRegistrar configurations)
        {
            configurations.Add(this);
        }
    }
}
