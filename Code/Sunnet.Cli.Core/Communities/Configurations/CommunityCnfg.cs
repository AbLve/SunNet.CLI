using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Data.Entity.ModelConfiguration.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/**************************************************************************
 * Developer: 		Leason
 * Computer:		Leason-PC
 * Domain:			Leason-pc
 * CreatedOn:		2014/8/18 16:27:20
 * Description:		Create CommunitiesRspt
 * Version History:	Created,2014/8/18 16:27:20
 * 
 * 
 **************************************************************************/
using Sunnet.Cli.Core.Communities.Entities;
using Sunnet.Framework.Core.Base;

namespace Sunnet.Cli.Core.Communities.Configurations
{
    public class CommunityCnfg : EntityTypeConfiguration<CommunityEntity>, IEntityMapper
    {
        public CommunityCnfg() 
        {
            ToTable("Communities");

            //将实体类之间的关系设置，分散到相应的外键表里
            //HasRequired(c => c.Funding).WithMany(f => f.Communities).HasForeignKey(x=>x.FundingId);
            //HasRequired(c => c.County).WithMany(c => c.Communities).HasForeignKey(x => x.CountyId);
            //HasRequired(c => c.State).WithMany(s => s.Communities).HasForeignKey(x => x.StateId);
            //HasRequired(c => c.PrimaryTitle).WithMany(t=>t.Communities).HasForeignKey(x => x.PrimaryTitleId);
            //HasRequired(c => c.SecondaryTitle).WithMany(t => t.Communities).HasForeignKey(x => x.SecondaryTitleId);

            //HasMany(c => c.Classes).WithRequired(c => c.Community).HasForeignKey(c => c.CommunityId);
            //HasMany(c => c.Students).WithRequired(c => c.Community).HasForeignKey(c => c.CommunityId);
            HasMany(c => c.UserCommunitySchools).WithRequired(e => e.Community).HasForeignKey(m => m.CommunityId);
            HasMany(c => c.CommunitySchoolRelations).WithRequired(e => e.Community).HasForeignKey(m => m.CommunityId);
            HasMany(c => c.CommunityAssessmentRelations).WithRequired(e => e.Community).HasForeignKey(m => m.CommunityId);

        }

        public void RegistTo(ConfigurationRegistrar configurations)
        {
            configurations.Add(this);
        }
    }
}
