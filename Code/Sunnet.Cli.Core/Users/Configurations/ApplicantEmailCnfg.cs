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
 * CreatedOn:		2014/8/13 11:59:23
 * Description:		Please input class summary
 * Version History:	Created,2014/8/13 11:44:23
 * 
 * 
 **************************************************************************/
using Sunnet.Cli.Core.Users.Entities;
using Sunnet.Framework.Core.Base;

namespace Sunnet.Cli.Core.Users.Configurations
{
    public class ApplicantEmailCnfg : EntityTypeConfiguration<ApplicantEmailEntity>, IEntityMapper
    {
        public ApplicantEmailCnfg()
        {
            ToTable("ApplicantEmails");
            HasRequired(x => x.Applicant).WithMany(o => o.ApplicantEmails);
        }

        public void RegistTo(ConfigurationRegistrar configurations)
        {
            configurations.Add(this);
        }
    }

}
