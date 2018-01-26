using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/**************************************************************************
 * Developer: 		JackZhang
 * Computer:		JACKZ
 * Domain:			JackZ
 * CreatedOn:		2/13 2015 11:26:18
 * Description:		Please input class summary
 * Version History:	Created,2/13 2015 11:26:18
 * 
 * 
 **************************************************************************/
using Sunnet.Cli.Core.Common.Enums;
using Sunnet.Cli.Core.Communities.Entities;
using Sunnet.Cli.Core.Schools.Entities;
using Sunnet.Framework.Core.Base;

namespace Sunnet.Cli.Core.Users.Entities
{
    public class UserComSchRelationEntity : EntityBase<int>
    {
        public UserComSchRelationEntity()
        {
            CommunityId = 0;
            SchoolId = 0;
            UpdatedOn = DateTime.Now;
            CreatedOn = DateTime.Now;
            AccessType = AccessType.ReadOnly;
        }
        public int UserId { get; set; }

        public int CommunityId { get; set; }

        public int SchoolId { get; set; }

        public EntityStatus Status { get; set; }

        public int CreatedBy { get; set; }

        public int UpdatedBy { get; set; }

        public AccessType AccessType { get; set; }

        public virtual UserBaseEntity User { get; set; }

        public virtual CommunityEntity Community { get; set; }

        public virtual SchoolEntity School { get; set; }
    }
}
