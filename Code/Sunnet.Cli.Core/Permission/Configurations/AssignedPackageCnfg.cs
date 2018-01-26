using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/**************************************************************************
 * Developer: 		Joe
 * Computer:		Joe-PC
 * Domain:			Joe-pc
 * CreatedOn:		2014/9/5
 * Description:		Create CountyEntity
 * Version History:	Created,2014/9/5
 * 
 * 
 **************************************************************************/
using Sunnet.Framework.Core.Base;
using System.Data.Entity.ModelConfiguration;
using Sunnet.Cli.Core.Permission.Entities;
using System.Data.Entity.ModelConfiguration.Configuration;

namespace Sunnet.Cli.Core.Permission.Configurations
{
    public class AssignedPackageCnfg : EntityTypeConfiguration<AssignedPackageEntity>, IEntityMapper
    {
        public AssignedPackageCnfg()
        {
            ToTable("Permission_AssignedPackages");

            HasRequired(a => a.Package).WithMany(a => a.DistrictsAndSchools)
                .HasForeignKey(a => a.PackageId);

            HasRequired(r => r.Community).WithMany(r => r.AssignedPackages).HasForeignKey(r => r.ScopeId);

            HasRequired(r => r.School).WithMany(r => r.AssignedPackages).HasForeignKey(r => r.ScopeId);
        }

        public void RegistTo(ConfigurationRegistrar configurations)
        {
            configurations.Add(this);
        }
    }
}
