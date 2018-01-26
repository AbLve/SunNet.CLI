using Sunnet.Cli.Core.MasterData.Entities;
using Sunnet.Framework.Core.Base;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Data.Entity.ModelConfiguration.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/**************************************************************************
 * Developer: 		Lee
 * Computer:		Lee-PC
 * Domain:			Lee-pc
 * CreatedOn:		2014/8/25 20:27:20
 * Description:		Create LanguageCnfg
 * Version History:	Created,2014/8/25 20:27:20
 * 
 * 
 **************************************************************************/
namespace Sunnet.Cli.Core.MasterData.Configurations
{
    public class LanguageCnfg : EntityTypeConfiguration<LanguageEntity>, IEntityMapper
    {
        public LanguageCnfg()
        {
            ToTable("Languages");
        }

        public void RegistTo(ConfigurationRegistrar configurations)
        {
            configurations.Add(this);
        }
    }
}
