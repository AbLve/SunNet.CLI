using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/**************************************************************************
 * Developer: 		Sam
 * Computer:		SAM-PC
 * Domain:			sam-pc
 * CreatedOn:		2014/8/13 12:01:49
 * Description:		Please input class summary
 * Version History:	Created,2014/8/13 12:01:49
 * 
 * 
 **************************************************************************/
using Sunnet.Cli.Core.Users.Entities;
using Sunnet.Framework.Core.Base;

namespace Sunnet.Cli.Core.Users.Interfaces
{
    public interface IApplicantRpst : IRepository<ApplicantEntity, Int32>
    {
    }
}
