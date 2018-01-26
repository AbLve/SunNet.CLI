using System.Data.Entity.ModelConfiguration;
using System.Data.Entity.ModelConfiguration.Configuration;
using Sunnet.Cli.Core.Practices.Entities;
using Sunnet.Framework.Core.Base;

/**************************************************************************
 * Developer: 		Jack
 * Computer:		Jackz
 * Domain:			Jackz
 * CreatedOn:		08/11/2014 03:39:16
 * Description:		MeasureEntity 's Configuration
 * Version History:	Created,08/11/2014 03:39:16
 * 
 * 
 **************************************************************************/
namespace Sunnet.Cli.Core.Practices.Configurations
{
    internal class CliAdeMeasureCnfg : EntityTypeConfiguration<CliAdeMeasureEntity>, IEntityMapper
    {
        public CliAdeMeasureCnfg()
        {
            ToTable("CLI_ADE_Measures");
            HasRequired(x => x.Parent).WithMany(x => x.SubMeasures).HasForeignKey(x => x.ParentId);
            HasRequired(x => x.Assessment).WithMany(y => y.Measures).HasForeignKey(x => x.AssessmentId);
        }

        public void RegistTo(ConfigurationRegistrar configurations)
        {
            configurations.Add(this);
        }
    }
}
