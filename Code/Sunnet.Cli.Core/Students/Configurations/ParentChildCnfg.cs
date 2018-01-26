using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Data.Entity.ModelConfiguration.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sunnet.Cli.Core.Students.Entities;
using Sunnet.Framework.Core.Base;

namespace Sunnet.Cli.Core.Students.Configurations
{
    public class ParentChildCnfg : EntityTypeConfiguration<ParentChildEntity>, IEntityMapper
    {
        public ParentChildCnfg()
        {
            ToTable("ParentChildRelations");
            Ignore(x => x.UpdatedOn);
            Ignore(x => x.CreatedOn);
            HasRequired(e => e.Parent).WithMany(e => e.ParentChilds).HasForeignKey(e => e.ParentId);
            //HasRequired(e => e.V_Child).WithMany(e => e.ParentChilds).HasForeignKey(e => e.ChildId);
            HasRequired(e => e.Child).WithMany(e => e.ParentChilds).HasForeignKey(e => e.ChildId);
        }

        public void RegistTo(ConfigurationRegistrar configurations)
        {
            configurations.Add(this);
        }
    }
}
