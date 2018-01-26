using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Data.Entity.ModelConfiguration.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


using Sunnet.Cli.Core.Export.Entities;
using Sunnet.Framework.Core.Base;
namespace Sunnet.Cli.Core.Export.Configurations
{
    public class ReportTemplateCnfg : EntityTypeConfiguration<ReportTemplateEntity>, IEntityMapper
    {
        public ReportTemplateCnfg()
        {
            ToTable("ReportTemplates");
        }

        public void RegistTo(ConfigurationRegistrar configurations)
        {
            configurations.Add(this);
        }
    }
}
