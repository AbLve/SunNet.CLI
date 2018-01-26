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
    public  class TRSItemAnswerCnfg: EntityTypeConfiguration<TRSItemAnswerEntity>, IEntityMapper
    {
        public TRSItemAnswerCnfg()
        {
            ToTable("TRSItemAnswers");
            Ignore(r => r.UpdatedOn);
            Ignore(r => r.CreatedOn);

            HasRequired(r => r.Answer).WithMany(r => r.ItemAnswers).HasForeignKey(x=>x.AnswerId);
            HasRequired(r => r.TRSItem).WithMany(r => r.ItemAnswers).HasForeignKey(x=>x.ItemId);
        }

        public void RegistTo(ConfigurationRegistrar configurations)
        {
            configurations.Add(this);
        }
    }
}