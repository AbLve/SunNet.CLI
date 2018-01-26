using System.Data.Entity.ModelConfiguration;
using System.Data.Entity.ModelConfiguration.Configuration;
using Sunnet.Cli.Core.Practices.Entites;
using Sunnet.Framework.Core.Base;

/**************************************************************************
 * Developer: 		Jack
 * Computer:		Jackz
 * Domain:			Jackz
 * CreatedOn:		08/11/2014 03:39:16
 * Description:		AssessmentEntity 's Configuration
 * Version History:	Created,08/11/2014 03:39:16
 * 
 * 
 **************************************************************************/
namespace Sunnet.Cli.Core.Practices.Configurations
{
    internal class PracticeMeasureGroupCnfg : EntityTypeConfiguration<PracticeMeasureGroupEntity>, IEntityMapper
    {
        public PracticeMeasureGroupCnfg()
        {
            ToTable("PracticeMeasureGroups");
        }
        public void RegistTo(ConfigurationRegistrar configurations)
        {
            configurations.Add(this);
        }
    }
}
