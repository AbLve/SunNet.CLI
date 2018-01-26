using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity.ModelConfiguration;
using System.Data.Entity.ModelConfiguration.Configuration;


using Sunnet.Cli.Core.Communities.Entities;
using Sunnet.Framework.Core.Base;

namespace Sunnet.Cli.Core.Communities.Configurations
{
    public class CommunityNotesCnfg : EntityTypeConfiguration<CommunityNotesEntity>, IEntityMapper
    {
        public CommunityNotesCnfg()
        {
            ToTable("CommunityNotes");
        }

        public void RegistTo(ConfigurationRegistrar configurations)
        {
            configurations.Add(this);
        }
    }
}
