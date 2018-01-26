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
using Sunnet.Framework.Core.Base;
using System.Data.Entity.ModelConfiguration.Configuration;
using Sunnet.Cli.Core.Practices.Entities;

namespace Sunnet.Cli.Core.Practices.Configurations
{
    internal class PracticeStudentItemCnfg : EntityTypeConfiguration<PracticeStudentItemEntity>, IEntityMapper
    {
        public PracticeStudentItemCnfg()
        {
            ToTable("PracticeStudentItems");
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
