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
 * Domain:			JackZ
 * CreatedOn:		1/27 2015 14:55:10
 * Description:		Please input class summary
 * Version History:	Created,1/27 2015 14:55:10
 * 
 * 
 **************************************************************************/
using Sunnet.Cli.Core.Cot.Entities;
using Sunnet.Framework.Core.Base;

namespace Sunnet.Cli.Core.Cot.Configurations
{
    internal class CotStgReportItemCnfg : EntityTypeConfiguration<CotStgReportItemEntity>, IEntityMapper
    {
        public CotStgReportItemCnfg()
        {
            ToTable("CotStgReportItems");
            Ignore(x => x.CreatedOn);
            Ignore(x => x.UpdatedOn);
            HasRequired(x => x.Report).WithMany(x => x.ReportItems).HasForeignKey(x => x.CotStgReportId);
            HasRequired(x => x.Item).WithMany().HasForeignKey(x => x.ItemId);
        }
        public void RegistTo(ConfigurationRegistrar configurations)
        {
            configurations.Add(this);
        }
    }
}
