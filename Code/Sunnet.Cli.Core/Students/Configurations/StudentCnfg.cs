using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Data.Entity.ModelConfiguration.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/**************************************************************************
 * Developer: 		Damon
 * Computer:		Damon-PC
 * Domain:			Damon-pc
 * CreatedOn:		2014/8/25 09:33:23
 * Description:		Please input class summary
 * Version History:	Created,2014/8/25 09:33:23
 **************************************************************************/
using Sunnet.Cli.Core.Students.Entities;
using Sunnet.Framework.Core.Base;

namespace Sunnet.Cli.Core.Students.Configurations
{
    public class StudentCnfg : EntityTypeConfiguration<StudentEntity>, IEntityMapper
    {
        public StudentCnfg()
        {
            ToTable("Students");      
        }

        public void RegistTo(ConfigurationRegistrar configurations)
        {
            configurations.Add(this);
        }
    }
}
