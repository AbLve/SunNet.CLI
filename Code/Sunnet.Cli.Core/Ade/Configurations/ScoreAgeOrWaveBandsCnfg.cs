using Sunnet.Cli.Core.Ade.Entities;
using Sunnet.Framework.Core.Base;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Data.Entity.ModelConfiguration.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sunnet.Cli.Core.Ade.Configurations
{
    internal class ScoreAgeOrWaveBandsCnfg : EntityTypeConfiguration<ScoreAgeOrWaveBandsEntity>, IEntityMapper
    {


        public ScoreAgeOrWaveBandsCnfg()
        {
            ToTable("ScoreAgeOrWaveBands");
            //HasRequired(x => x.Template).WithMany(y => y.Items).HasForeignKey(x => x.TemplateId);


            //HasRequired(x => x.Score).WithMany(y => y.Items).HasForeignKey(x => x.ScoreId);

            HasRequired(x => x.Score).WithMany(y => y.ScoreAgeOrWaveBands).HasForeignKey(x => x.ScoreId);

            Ignore(e => e.CreatedOn);
            Ignore(e => e.UpdatedOn);
        }
        public void RegistTo(ConfigurationRegistrar configurations)
        {
            configurations.Add(this);
        }





    }


}
