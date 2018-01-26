using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/**************************************************************************
 * Developer: 		Sam
 * Computer:		SAM-PC
 * Domain:			sam-pc
 * CreatedOn:		2014/8/27 12:16:26
 * Description:		Please input class summary
 * Version History:	Created,2014/8/27 12:16:26
 * 
 * 
 **************************************************************************/
using Sunnet.Cli.Core.MasterData.Entities;
using Sunnet.Framework.Core.Base;

namespace Sunnet.Cli.Core.MasterData.Interfaces
{
    public interface ICurriculumRpst : IRepository<CurriculumEntity, Int32>
    {
    }
}
