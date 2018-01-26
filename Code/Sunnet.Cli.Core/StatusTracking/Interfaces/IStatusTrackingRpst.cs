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
using Sunnet.Cli.Core.Schools.Entities;
using Sunnet.Framework.Core.Base;
using Sunnet.Cli.Core.StatusTracking.Entities;
namespace Sunnet.Cli.Core.StatusTracking.Interfaces
{
    public interface IStatusTrackingRpst : IRepository<StatusTrackingEntity, Int32>
    {

    }
}
