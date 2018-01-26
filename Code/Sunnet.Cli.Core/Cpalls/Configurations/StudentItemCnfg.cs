using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/**************************************************************************
 * Developer: 		JackZhang
 * Computer:		JACKZ
 * Domain:			Jackz
 * CreatedOn:		2014/9/4 4:22:23
 * Description:		Please input class summary
 * Version History:	Created,2014/9/4 4:22:23
 * 
 * 
 **************************************************************************/
using Sunnet.Cli.Core.Cpalls.Entities;
using Sunnet.Framework.Core.Base;
using System.Data.Entity.ModelConfiguration.Configuration;


namespace Sunnet.Cli.Core.Cpalls.Configurations
{
    internal class StudentItemCnfg : EntityTypeConfiguration<StudentItemEntity>, IEntityMapper
    {
        public StudentItemCnfg()
        {
            ToTable("StudentItems");
            HasRequired(x => x.Measure).WithMany(x => x.Items).HasForeignKey(x => x.SMId);

            HasRequired(x => x.Item).WithMany().HasForeignKey(x => x.ItemId);
            Property(x => x.Details).HasMaxLength(4000);
        }

        public void RegistTo(ConfigurationRegistrar configurations)
        {
            configurations.Add(this);
        }
    }
}
