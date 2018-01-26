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
 * CreatedOn:		2014/8/19 16:27:20
 * Description:		Create FundingRspt
 * Version History:	Created,2014/8/19 16:27:20
 * 
 * 
 **************************************************************************/
using Sunnet.Cli.Core.MasterData.Entities;
using Sunnet.Framework.Core.Base;

namespace Sunnet.Cli.Core.MasterData.Configurations
{
    public class FundingCnfg : EntityTypeConfiguration<FundingEntity>, IEntityMapper
    {
        public FundingCnfg()
        {
            ToTable("Fundings");

            HasMany(r => r.Schools).WithRequired(r => r.Funding).HasForeignKey(p => p.FundingId);
            HasMany(f => f.Communities).WithRequired(c => c.Funding).HasForeignKey(c => c.FundingId);

                HasMany(f => f.Classrooms).WithRequired(c => c.Funding).HasForeignKey(c => c.FundingId);
                HasMany(f => f.Classrooms).WithRequired(c => c.FcFunding).HasForeignKey(c => c.FcFundingId);
                HasMany(f => f.Classrooms).WithRequired(c => c.Part2Funding).HasForeignKey(c => c.Part2FundingId);
                HasMany(f => f.Classrooms).WithRequired(c => c.Part1Funding).HasForeignKey(c => c.Part1FundingId);
                HasMany(f => f.Classrooms).WithRequired(c => c.StartupKitFunding).HasForeignKey(c => c.StartupKitFundingId);
            
        }

        public void RegistTo(ConfigurationRegistrar configurations)
        {
            configurations.Add(this);
        }
    }
}
