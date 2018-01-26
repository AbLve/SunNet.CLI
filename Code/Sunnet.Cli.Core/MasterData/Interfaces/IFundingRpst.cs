using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/**************************************************************************
 * Developer: 		Leason
 * Computer:		Leason-PC
 * Domain:			Leason-pc
 * CreatedOn:		2014/8/19 16:27:20
 * Description:		Create IFundingRpst
 * Version History:	Created,2014/8/19 16:27:20
 * 
 * 
 **************************************************************************/
using Sunnet.Cli.Core.MasterData.Entities;
using Sunnet.Framework.Core.Base;

namespace Sunnet.Cli.Core.MasterData.Interfaces
{
    public interface IFundingRpst : IRepository<FundingEntity, Int32>
    {

    }
}
