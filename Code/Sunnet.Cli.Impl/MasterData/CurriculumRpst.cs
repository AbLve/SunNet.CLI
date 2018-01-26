using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/**************************************************************************
 * Developer: 		Sam
 * Computer:		SAM-PC
 * Domain:			sam-pc
 * CreatedOn:		2014/8/27 12:27:42
 * Description:		Please input class summary
 * Version History:	Created,2014/8/27 12:27:42
 * 
 * 
 **************************************************************************/
using Sunnet.Cli.Core.MasterData.Entities;
using Sunnet.Cli.Core.MasterData.Interfaces;
using Sunnet.Framework.Core.Base;

namespace Sunnet.Cli.Impl.MasterData
{
   public class CurriculumRpst : EFRepositoryBase<CurriculumEntity, Int32>, ICurriculumRpst
    {
       public CurriculumRpst(IUnitOfWork unit)
       {
           UnitOfWork = unit;
       }
    }
}
