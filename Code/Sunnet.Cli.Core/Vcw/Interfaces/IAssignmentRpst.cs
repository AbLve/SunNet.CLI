using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/**************************************************************************
 * Developer: 		Joe
 * Computer:		Joe-PC
 * Domain:			Joe-pc
 * CreatedOn:		2014/10/21
 * Description:		Create AuthorityEntity
 * Version History:	Created,2014/10/21
 * 
 * 
 **************************************************************************/
using Sunnet.Framework.Core.Base;
using Sunnet.Cli.Core.Vcw.Entities;
using Sunnet.Cli.Core.Vcw.Enums;

namespace Sunnet.Cli.Core.Vcw.Interfaces
{
    public interface IAssignmentRpst : IRepository<AssignmentEntity, Int32>
    {
        void ChangeStatus(ChangeStatusEnum method, int assignmentId, FileStatus status);

        void ChangeStatus(List<int> assignmentIds);
    }
}
