using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/**************************************************************************
 * Developer: 		JackZhang
 * Computer:		JACKZ
 * Domain:			Jackz
 * CreatedOn:		2014/9/4 4:22:23
 * Description:		Please input class summary
 * Version History:	Created,2014/9/4 4:22:23
 * 
 * 
 **************************************************************************/
using Sunnet.Cli.Core.Cpalls.Entities;
using Sunnet.Framework.Core.Base;


namespace Sunnet.Cli.Core.Cpalls.Configurations
{
    internal class StudentMeasureCnfg : EntityTypeConfiguration<StudentMeasureEntity>, IEntityMapper
    {
        public StudentMeasureCnfg()
        {
            ToTable("StudentMeasures");
            HasRequired(x => x.Assessment).WithMany(x => x.Measures).HasForeignKey(x => x.SAId);
            HasRequired(x => x.Measure).WithMany().HasForeignKey(x => x.MeasureId);
            Property(x => x.Comment).HasMaxLength(150);
        }


        public void RegistTo(System.Data.Entity.ModelConfiguration.Configuration.ConfigurationRegistrar configurations)
        {
            configurations.Add(this);
        }
    }
}
