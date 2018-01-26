using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Data.Entity.ModelConfiguration.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/**************************************************************************
 * Developer: 		Sam
 * Computer:		SAM-PC
 * Domain:			sam-pc
 * CreatedOn:		2014/8/13 12:03:17
 * Description:		Please input class summary
 * Version History:	Created,2014/8/13 12:03:17
 * 
 * 
 **************************************************************************/
using Sunnet.Cli.Core.Users.Entities;
using Sunnet.Framework.Core.Base;

namespace Sunnet.Cli.Core.Users.Configurations
{
    public class ApplicantCnfg : EntityTypeConfiguration<ApplicantEntity>, IEntityMapper
    {
        public ApplicantCnfg()
        {
            ToTable("Applicants");
        }

        public void RegistTo(ConfigurationRegistrar configurations)
        {
            configurations.Add(this);
        }
    }

}
