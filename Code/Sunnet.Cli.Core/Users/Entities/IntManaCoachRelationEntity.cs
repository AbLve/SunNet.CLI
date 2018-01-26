using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/**************************************************************************
 * Developer: 		Joe
 * Computer:		Joe-PC
 * Domain:			Joe-pc
 * CreatedOn:		2015/7/13
 * Description:		Create AuthorityEntity
 * Version History:	Created,2015/7/13
 * 
 * 
 **************************************************************************/
using Sunnet.Framework.Core.Base;

namespace Sunnet.Cli.Core.Users.Entities
{
    public class IntManaCoachRelationEntity : EntityBase<int>
    {
        public int PMUserId { get; set; }

        public int CoordCoachUserId { get; set; }

        public virtual UserBaseEntity User { get; set; }
    }
}
