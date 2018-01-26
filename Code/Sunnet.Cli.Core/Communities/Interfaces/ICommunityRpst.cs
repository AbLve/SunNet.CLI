using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/**************************************************************************
 * Developer: 		Leason
 * Computer:		Leason-PC
 * Domain:			Leason-pc
 * CreatedOn:		2014/8/18 16:27:20
 * Description:		Create CommunitiesRspt
 * Version History:	Created,2014/8/18 16:27:20
 * 
 * 
 **************************************************************************/
using Sunnet.Cli.Core.Common.Enums;
using Sunnet.Cli.Core.Communities.Entities;
using Sunnet.Framework.Core.Base;
using Sunnet.Cli.Core.Vcw.Entities;

namespace Sunnet.Cli.Core.Communities.Interfaces
{
    public interface ICommunityRpst : IRepository<CommunityEntity, Int32>
    {
        bool InactiveEntities(ModelName model, int entityId, EntityStatus status, string fundingYear);
    }
}
