using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/**************************************************************************
 * Developer: 		Sam
 * Computer:		SAM-PC
 * Domain:			sam-pc
 * CreatedOn:		2014/8/13 12:12:46
 * Description:		Please input class summary
 * Version History:	Created,2014/8/13 12:12:46
 * 
 * 
 **************************************************************************/
using Sunnet.Cli.Core.Users.Entities;
using Sunnet.Cli.Core.Users.Interfaces;
using Sunnet.Framework.Core.Base;

namespace Sunnet.Cli.Impl.Users
{
    public class ApplicantRpst : EFRepositoryBase<ApplicantEntity, Int32>, IApplicantRpst
    {
        public ApplicantRpst(IUnitOfWork unit)
        {
            this.UnitOfWork = unit;
        }
    }
}
