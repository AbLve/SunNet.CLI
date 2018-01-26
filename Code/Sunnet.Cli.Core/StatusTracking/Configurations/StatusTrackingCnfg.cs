using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/**************************************************************************
 * Developer: 		Joe
 * Computer:		Joe-PC
 * Domain:			Joe-pc
 * CreatedOn:		2015/06/03
 * Description:		Create AuthorityEntity
 * Version History:	Created,2015/06/03
 * 
 * 
 **************************************************************************/
using System.Data.Entity.ModelConfiguration;
using Sunnet.Cli.Core.StatusTracking.Entities;
using Sunnet.Framework.Core.Base;
using System.Data.Entity.ModelConfiguration.Configuration;

namespace Sunnet.Cli.Core.StatusTracking.Configurations
{
    public class StatusTrackingCnfg : EntityTypeConfiguration<StatusTrackingEntity>, IEntityMapper
    {
        public StatusTrackingCnfg()
        {
            ToTable("StatusTrackings");
            HasOptional(r => r.UserInfo_Approver).WithMany(o => o.StatusTrackings_Approvers).HasForeignKey(t => t.ApproverId);
            HasRequired(r => r.UserInfo_Requestor).WithMany(o => o.StatusTrackings_Requestors).HasForeignKey(t => t.RequestorId);
            HasRequired(r => r.UserInfo_SupposedApprover).WithMany(o => o.StatusTracking_SupposedApprover).HasForeignKey(t => t.SupposedApproverId);
            HasOptional(r => r.SchoolInfo).WithMany(o => o.StatusTrackings).HasForeignKey(t => t.SchoolId);
        }
        public void RegistTo(ConfigurationRegistrar configurations)
        {
            configurations.Add(this);
        }
    }
}
