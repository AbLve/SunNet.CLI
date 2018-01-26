using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Data.Entity.ModelConfiguration.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/**************************************************************************
 * Developer: 		Leason
 * Computer:		Leason-PC
 * Domain:			Leason-pc
 * CreatedOn:		2014/8/22 14:37:20
 * Description:		Create ClassCnfg
 * Version History:	Created,2014/8/22 14:37:20
 * 
 * 
 **************************************************************************/
using Sunnet.Cli.Core.Classes.Entites;
using Sunnet.Framework.Core.Base;

namespace Sunnet.Cli.Core.Classes.Configurations
{
    public class ClassCnfg : EntityTypeConfiguration<ClassEntity>, IEntityMapper
    {
        public ClassCnfg()
        {
            ToTable("Classes");

            //class与language，多对多
            HasMany(c => c.Languages).WithMany(l => l.Classeses).Map(o =>
            {
                o.MapLeftKey("ClassId");
                o.MapRightKey("LanguageId");
                o.ToTable("ClassLanguages");
            });

            HasMany(r => r.Students).WithMany(r => r.Classes).Map(
               m =>
               {
                   m.MapLeftKey("ClassId");
                   m.MapRightKey("StudentId");
                   m.ToTable("StudentClassRelations");
               }
           );
            HasRequired(r => r.LeadTeacher).WithMany().HasForeignKey(r => r.LeadTeacherId); 
        }

        public void RegistTo(ConfigurationRegistrar configurations)
        {
            configurations.Add(this);
        }
    }
}
