using Sunnet.Framework.Core.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/**************************************************************************
 * Developer: 		Lee
 * Computer:		Lee-PC
 * Domain:			Lee-pc
 * CreatedOn:		2014/8/25 20:27:20
 * Description:		Create ILanguageRpst
 * Version History:	Created,2014/8/25 20:27:20
 * 
 * 
 **************************************************************************/
using Sunnet.Cli.Core.MasterData.Entities;

namespace Sunnet.Cli.Core.MasterData.Interfaces
{
    public interface ILanguageRpst : IRepository<LanguageEntity, Int32>
    {
    }
}
