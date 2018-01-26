using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/**************************************************************************
 * Developer: 		Joe
 * Computer:		Joe-PC
 * Domain:			Joe-pc
 * CreatedOn:		2015/3/12
 * Description:		Create AuthorityEntity
 * Version History:	Created,2015/3/12
 * 
 * 
 **************************************************************************/
using Sunnet.Framework.Core.Base;
using Sunnet.Cli.Core.Vcw.Entities;
using Sunnet.Cli.Core.Vcw.Interfaces;

namespace Sunnet.Cli.Impl.Vcw
{
    public class AssignmentReportRpst : EFRepositoryBase<AssignmentReportEntity, Int32>, IAssignmentReportRpst
    {
        public AssignmentReportRpst(IUnitOfWork unit)
        {
            this.UnitOfWork = unit;
        }
    }
}
