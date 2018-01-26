using Sunnet.Cli.Core.Ade.Entities;
using Sunnet.Framework.Core.Base;
using System.Data.Entity.ModelConfiguration;
using System.Data.Entity.ModelConfiguration.Configuration;
using Sunnet.Cli.Core.Practices.Entities;

/**************************************************************************
 * Developer: 		Jack
 * Computer:		Jackz
 * Domain:			Jackz
 * CreatedOn:		08/11/2014 03:39:16
 * Description:		ItemBaseEntity 's Configuration
 * Version History:	Created,08/11/2014 03:39:16
 * 
 * 
 **************************************************************************/
namespace Sunnet.Cli.Core.Practices.Configurations
{
	internal class CliAdeItemBaseCnfg : EntityTypeConfiguration<CliAdeItemBaseEntity>, IEntityMapper
    {
	    public CliAdeItemBaseCnfg()
	    {
            ToTable("CLI_ADE_ItemBases");
	        //HasRequired(x => x.Template).WithMany(y => y.Items).HasForeignKey(x => x.TemplateId);
	        HasRequired(x => x.Measure).WithMany(y => y.Items).HasForeignKey(x => x.MeasureId);
	    }
		public void RegistTo(ConfigurationRegistrar configurations)
	    {
	        configurations.Add(this);
	    }
    }
}
