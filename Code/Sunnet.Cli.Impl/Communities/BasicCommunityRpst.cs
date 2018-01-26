using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/**************************************************************************
 * Developer: 		Leason
 * Computer:		Leason-PC
 * Domain:			Leason-pc
 * CreatedOn:		2014/8/22 18:44:20
 * Description:		Create CommunitiesRspt
 * Version History:	Created,2014/8/22 18:44:20
 * 
 * 
 **************************************************************************/
using Sunnet.Cli.Core.Communities.Entities;
using Sunnet.Cli.Core.Communities.Interfaces;
using Sunnet.Framework.Core.Base;

namespace Sunnet.Cli.Impl.Communities
{
    public class BasicCommunityRpst : EFRepositoryBase<BasicCommunityEntity, Int32>, IBasicCommunityRpst
    {
        public BasicCommunityRpst(IUnitOfWork unit)
        {
            UnitOfWork = unit;
        }
    }
}
