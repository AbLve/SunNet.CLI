using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/**************************************************************************
 * Developer: 		Joe
 * Computer:		Joe-PC
 * Domain:			Joe-pc
 * CreatedOn:		2015/3/18
 * Description:		Create AuthorityEntity
 * Version History:	Created,2015/3/18
 * 
 * 
 **************************************************************************/
using Sunnet.Framework.Core.Base;
using Sunnet.Cli.Core.Vcw.Entities;
using Sunnet.Cli.Core.Vcw.Interfaces;

namespace Sunnet.Cli.Impl.Vcw
{
    public class Assignment_Content_DataRpst : EFRepositoryBase<Assignment_Content_DataEntity, Int32>, IAssignment_Content_DataRpst
    {
        public Assignment_Content_DataRpst(IUnitOfWork unit)
        {
            this.UnitOfWork = unit;
        }
    }
}
