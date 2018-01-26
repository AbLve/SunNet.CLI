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
 * CreatedOn:		2014/10/7 4:34:44
 * Description:		Please input class summary
 * Version History:	Created,2014/10/7 4:34:44
 * 
 * 
 **************************************************************************/
using Sunnet.Cli.Core.Cpalls.Entities;
using Sunnet.Framework.Core.Base;

namespace Sunnet.Cli.Core.Cpalls.Configurations
{
    internal class CpallsStudentGroupCnfg : EntityTypeConfiguration<CpallsStudentGroupEntity>, IEntityMapper
    {
        public CpallsStudentGroupCnfg()
        {
            ToTable("CpallsStudentGroups");
        }

        public void RegistTo(System.Data.Entity.ModelConfiguration.Configuration.ConfigurationRegistrar configurations)
        {
            configurations.Add(this);
        }
    }
}
