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
using Sunnet.Cli.Core.Practices.Entites;

namespace Sunnet.Cli.Core.Practices.Configurations
{
    internal class DemoStudentCnfg : EntityTypeConfiguration<DemoStudentEntity>, IEntityMapper
    {
        public DemoStudentCnfg()
        {
            ToTable("PracticeDemoStudents");
        }

        public void RegistTo(ConfigurationRegistrar configurations)
        {
            configurations.Add(this);
        }
    }
}
