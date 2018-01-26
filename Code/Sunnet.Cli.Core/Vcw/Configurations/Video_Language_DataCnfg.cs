using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/**************************************************************************
 * Developer: 		Joe
 * Computer:		Joe-PC
 * Domain:			Joe-pc
 * CreatedOn:		2015/3/18
 * Description:		Create AuthorityEntity
 * Version History:	Created,2015/3/18
 * 
 * 
 **************************************************************************/
using System.Data.Entity.ModelConfiguration;
using Sunnet.Cli.Core.Vcw.Entities;
using Sunnet.Framework.Core.Base;
using System.Data.Entity.ModelConfiguration.Configuration;

namespace Sunnet.Cli.Core.Vcw.Configurations
{
    public class Video_Language_DataCnfg : EntityTypeConfiguration<Video_Language_DataEntity>, IEntityMapper
    {
        public Video_Language_DataCnfg()
        {
            ToTable("Video_Language_Datas");
            HasMany(r => r.Files).WithOptional(r => r.Language).HasForeignKey(r => r.LanguageId);
        }

        public void RegistTo(ConfigurationRegistrar configurations)
        {
            configurations.Add(this);
        }
    }
}
