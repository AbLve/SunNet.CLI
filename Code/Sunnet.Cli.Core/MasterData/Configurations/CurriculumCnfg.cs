using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Data.Entity.ModelConfiguration.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/**************************************************************************
 * Developer: 		Sam
 * Computer:		SAM-PC
 * Domain:			sam-pc
 * CreatedOn:		2014/8/27 12:13:15
 * Description:		Please input class summary
 * Version History:	Created,2014/8/27 12:13:15
 * 
 * 
 **************************************************************************/
using Sunnet.Cli.Core.MasterData.Entities;
using Sunnet.Framework.Core.Base;

namespace Sunnet.Cli.Core.MasterData.Configurations
{
    public class CurriculumCnfg : EntityTypeConfiguration<CurriculumEntity>, IEntityMapper
    {
        public CurriculumCnfg()
        {
            ToTable("Curriculums");
            HasMany(o => o.Classrooms).WithRequired(c => c.Curriculum).HasForeignKey(c => c.CurriculumId);
            HasMany(o => o.Classrooms).WithRequired(c => c.NeedCurriculum).HasForeignKey(c => c.NeedCurriculumId);
            HasMany(o => o.Classes).WithRequired(c => c.Curriculum).HasForeignKey(c => c.CurriculumId);
            HasMany(o => o.Classes).WithRequired(c => c.SupplementalCurriculum).HasForeignKey(c => c.SupplementalCurriculumId);
        }

        public void RegistTo(ConfigurationRegistrar configurations)
        {
            configurations.Add(this);
        }
    }
}
