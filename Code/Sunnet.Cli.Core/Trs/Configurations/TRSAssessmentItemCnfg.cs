using Sunnet.Cli.Core.Trs.Entities;
using Sunnet.Framework.Core.Base;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Data.Entity.ModelConfiguration.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sunnet.Cli.Core.Trs.Configurations
{
    public class TRSAssessmentItemCnfg : EntityTypeConfiguration<TRSAssessmentItemEntity>, IEntityMapper
    {
        public TRSAssessmentItemCnfg()
        {
            ToTable("TRSAssessmentItems");
            HasRequired(r => r.Assessment).WithMany(r => r.AssessmentItems).HasForeignKey(x=>x.TRSAssessmentId);
            HasRequired(r => r.Item).WithMany().HasForeignKey(r => r.ItemId);
            HasOptional(i => i.Answer).WithMany().HasForeignKey(i => i.AnswerId);
        }

        public void RegistTo(ConfigurationRegistrar configurations)
        {
            configurations.Add(this);
        }
    }
}
