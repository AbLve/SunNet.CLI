using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/**************************************************************************
 * Developer: 		Joe
 * Computer:		Joe-PC
 * Domain:			Joe-pc
 * CreatedOn:		2015/3/3
 * Description:		Create AuthorityEntity
 * Version History:	Created,2015/3/3
 * 
 * 
 **************************************************************************/
using Sunnet.Framework.Core.Base;
using Sunnet.Cli.Core.Vcw.Entities;
using Sunnet.Cli.Core.Vcw.Interfaces;

namespace Sunnet.Cli.Impl.Vcw
{
    public class FileContentRpst : EFRepositoryBase<FileContentEntity, Int32>, IFileContentRpst
    {
        public FileContentRpst(IUnitOfWork unit)
        {
            this.UnitOfWork = unit;
        }
    }
}
