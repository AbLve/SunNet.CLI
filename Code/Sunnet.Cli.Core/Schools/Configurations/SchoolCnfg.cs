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
 * CreatedOn:		2014/8/19 8:55:27
 * Description:		Please input class summary
 * Version History:	Created,2014/8/19 8:55:27
 * 
 * 
 **************************************************************************/
using Sunnet.Cli.Core.Schools.Entities;
using Sunnet.Framework.Core.Base;

namespace Sunnet.Cli.Core.Schools.Configurations
{
    public class SchoolCnfg : EntityTypeConfiguration<SchoolEntity>, IEntityMapper
    {
        public SchoolCnfg()
        {
            ToTable("Schools");
            //HasRequired(o => o.Funding).WithMany().HasForeignKey(o => o.FundingId);
            HasOptional(o => o.State).WithMany().HasForeignKey(o => o.StateId);

            HasRequired(o => o.MailState).WithMany().HasForeignKey(o => o.MailingStateId);
            HasRequired(o => o.County).WithMany().HasForeignKey(o => o.CountyId);
            HasRequired(o => o.MailCounty).WithMany().HasForeignKey(o => o.MailingCountyId);
            HasMany(o => o.Classes).WithRequired(c => c.School).HasForeignKey(c => c.SchoolId);
            HasMany(o => o.TRSClasses).WithRequired(c => c.School).HasForeignKey(c => c.SchoolId);

           

            HasOptional(a => a.Assessor).WithMany().HasForeignKey(s => s.TrsAssessorId);

           // HasRequired(x => x.Community).WithMany(x => x.Schools).HasForeignKey(x => x.CommunityId);

            HasMany(c => c.CommunitySchoolRelations).WithRequired(e => e.School).HasForeignKey(m => m.SchoolId);
            HasMany(c => c.UserCommunitySchools).WithRequired(e => e.School).HasForeignKey(m => m.SchoolId);

        }
        public void RegistTo(ConfigurationRegistrar configurations)
        {
            configurations.Add(this);
        }
    }
}
