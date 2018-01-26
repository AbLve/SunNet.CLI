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
 * CreatedOn:		2014/12/16 9:40:11
 * Description:		Please input class summary
 * Version History:	Created,2014/12/16 9:40:11
 * 
 * 
 **************************************************************************/
using Sunnet.Cli.Core.Cot.Entities;
using Sunnet.Framework.Core.Base;

namespace Sunnet.Cli.Core.Cot.Configurations
{
    internal class CotAssessmentItemCnfg: EntityTypeConfiguration<CotAssessmentItemEntity>, IEntityMapper
    {
        public CotAssessmentItemCnfg()
        {
            HasRequired(x => x.Assessment).WithMany(x => x.Items).HasForeignKey(x=>x.CotAssessmentId);
            HasRequired(x => x.Item).WithMany().HasForeignKey(x => x.ItemId);
            ToTable("COTAssessmentItems");
        }
        public void RegistTo(ConfigurationRegistrar configurations)
        {
            configurations.Add(this);
        }
    }
}
