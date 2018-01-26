using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Data.Entity.ModelConfiguration.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/**************************************************************************
 * Developer: 		JackZhang
 * Computer:		JACKZ
 * Domain:			Jackz
 * CreatedOn:		2014/12/16 9:38:09
 * Description:		Please input class summary
 * Version History:	Created,2014/12/16 9:38:09
 * 
 * 
 **************************************************************************/
using Sunnet.Cli.Core.Cot.Entities;
using Sunnet.Cli.Core.Observable.Entities;
using Sunnet.Framework.Core.Base;

namespace Sunnet.Cli.Core.Observable.Configurations
{
    internal class ObservableAssessmentCnfg : EntityTypeConfiguration<ObservableAssessmentEntity>, IEntityMapper
    {
        public ObservableAssessmentCnfg()
        {
            ToTable("ObservableAssessments");
            HasRequired(o => o.Assessment).WithMany().HasForeignKey(o => o.AssessmentId);
            HasRequired(o => o.Student).WithMany().HasForeignKey(o => o.StudentId);
        }
        public void RegistTo(ConfigurationRegistrar configurations)
        {
            configurations.Add(this);
        }
    }

    internal class ObservableAssessmentItemCnfg : EntityTypeConfiguration<ObservableAssessmentItemEntity>, IEntityMapper
    {
        public ObservableAssessmentItemCnfg()
        {
            ToTable("ObservableAssessmentItems");
            HasRequired(o => o.ObservableAssessment).WithMany().HasForeignKey(o => o.ObservableAssessmentId);
        }
        public void RegistTo(ConfigurationRegistrar configurations)
        {
            configurations.Add(this);
        }
    }

    internal class ObservableItemsHistoryCnfg : EntityTypeConfiguration<ObservableItemsHistoryEntity>, IEntityMapper
    {
        public ObservableItemsHistoryCnfg()
        {
            ToTable("ObservableItemHistories");
            HasRequired(o => o.ObservableAssessment).WithMany().HasForeignKey(o => o.ObservableAssessmentId);
        }
        public void RegistTo(ConfigurationRegistrar configurations)
        {
            configurations.Add(this);
        }
    }
}
