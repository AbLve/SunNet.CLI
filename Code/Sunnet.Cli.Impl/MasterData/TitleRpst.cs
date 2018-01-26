using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/**************************************************************************
 * Developer: 		Leason
 * Computer:		Leason-PC
 * Domain:			Leason-pc
 * CreatedOn:		2014/8/19 16:27:20
 * Description:		Create CommunitiesRspt
 * Version History:	Created,2014/8/19 16:27:20
 * 
 * 
 **************************************************************************/
using Sunnet.Cli.Core.MasterData.Entities;
using Sunnet.Cli.Core.MasterData.Interfaces;
using Sunnet.Framework.Core.Base;

namespace Sunnet.Cli.Impl.MasterData
{
    public class TitleRpst:EFRepositoryBase<TitleEntity, Int32>, ITitleRpst
    {
        public TitleRpst(IUnitOfWork unit)
        {
            UnitOfWork = unit;
        }
    }
}
