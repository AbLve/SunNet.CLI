using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/**************************************************************************
 * Developer: 		JackZhang
 * Computer:		JACKZ
 * Domain:			JackZ
 * CreatedOn:		2/3 2015 17:26:24
 * Description:		Please input class summary
 * Version History:	Created,2/3 2015 17:26:24
 * 
 * 
 **************************************************************************/
using Sunnet.Cli.Core.Reports.Entities;
using Sunnet.Cli.Core.Reports.Interfaces;
using Sunnet.Framework.Core.Base;

namespace Sunnet.Cli.Impl.Reports
{
    public class SchoolViewRpst : EFRepositoryBase<SchoolViewEntity, Int32>, ISchoolViewRpst
    {
        public SchoolViewRpst(IUnitOfWork unit)
        {
            this.UnitOfWork = unit;
        }

    }
}
