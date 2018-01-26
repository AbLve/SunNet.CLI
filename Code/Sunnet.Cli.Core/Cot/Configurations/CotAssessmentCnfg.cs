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
using Sunnet.Framework.Core.Base;

namespace Sunnet.Cli.Core.Cot.Configurations
{
    internal class CotAssessmentCnfg : EntityTypeConfiguration<CotAssessmentEntity>, IEntityMapper
    {
        public CotAssessmentCnfg()
        {
            ToTable("COTAssessments");
        }
        public void RegistTo(ConfigurationRegistrar configurations)
        {
            configurations.Add(this);
        }
    }
}
