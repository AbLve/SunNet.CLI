using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/**************************************************************************
 * Developer: 		Leason
 * Computer:		Leason-PC
 * Domain:			Leason-pc
 * CreatedOn:		2014/8/22 17:33:00
 * Description:		Create BasicCommunityCnfg
 * Version History:	Created,2014/8/22 17:33:00
 * 
 * 
 **************************************************************************/
using Sunnet.Cli.Core.Communities.Entities;
using Sunnet.Framework.Core.Base;

namespace Sunnet.Cli.Core.Communities.Interfaces
{
    public interface IBasicCommunityRpst : IRepository<BasicCommunityEntity, Int32>
    {

    }
}
