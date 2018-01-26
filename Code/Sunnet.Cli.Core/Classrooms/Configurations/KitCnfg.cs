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
 * CreatedOn:		2014/8/23 14:23:20
 * Description:		Create KitCnfg
 * Version History:	Created,2014/8/23 14:23:20
 * 
 * 
 **************************************************************************/
using Sunnet.Cli.Core.Classrooms.Entites;
using Sunnet.Framework.Core.Base;

namespace Sunnet.Cli.Core.Classrooms.Configurations
{
    public class KitCnfg:EntityTypeConfiguration<KitEntity>,IEntityMapper
    {
        public KitCnfg()
        {
            ToTable("Kits");

            HasMany(o => o.Classrooms).WithRequired(c => c.Kit).HasForeignKey(c => c.KitId);
            HasMany(o => o.Classrooms).WithRequired(c => c.FcNeedKit).HasForeignKey(c => c.FcNeedKitId);
            HasMany(o => o.Classrooms).WithRequired(c => c.Part1Kit).HasForeignKey(c => c.Part1KitId);
            HasMany(o => o.Classrooms).WithRequired(c => c.Part1NeedKit).HasForeignKey(c => c.Part1NeedKitId);

            HasMany(o => o.Classrooms).WithRequired(c => c.Part2Kit).HasForeignKey(c => c.Part2KitId);
            HasMany(o => o.Classrooms).WithRequired(c => c.Part2NeedKit).HasForeignKey(c => c.Part2NeedKitId);
            HasMany(o => o.Classrooms).WithRequired(c => c.StartupKit).HasForeignKey(c => c.StartupKitId);
            HasMany(o => o.Classrooms).WithRequired(c => c.StartupNeedKit).HasForeignKey(c => c.StartupNeedKitId);
        }
        public void RegistTo(ConfigurationRegistrar configurations)
        {
            configurations.Add(this);
        }
    }
}
