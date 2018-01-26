using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/**************************************************************************
 * Developer: 		Joe
 * Computer:		JOE-PC
 * Domain:			Joe-pc
 * CreatedOn:		2014/10/21 12:15:10
 * Description:		Please input class summary
 * Version History:	Created,2014/10/21 12:15:10
 * 
 * 
 **************************************************************************/
using Sunnet.Framework.Core.Base;
using Sunnet.Cli.Core.Vcw.Entities;
using Sunnet.Cli.Core.Vcw.Interfaces;

namespace Sunnet.Cli.Impl.Vcw
{
    public class FileRpst : EFRepositoryBase<Vcw_FileEntity, Int32>, IFileRpst
    {
        public FileRpst(IUnitOfWork unit)
        {
            this.UnitOfWork = unit;
        }
    }
}
