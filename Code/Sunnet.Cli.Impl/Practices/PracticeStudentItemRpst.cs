using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/**************************************************************************
 * Developer: 		JackZhang
 * Computer:		JACKZ
 * Domain:			Jackz
 * CreatedOn:		2014/9/4 4:22:15
 * Description:		Please input class summary
 * Version History:	Created,2014/9/4 4:22:15
 * 
 * 
 **************************************************************************/
using Sunnet.Cli.Core;
using Sunnet.Cli.Core.Ade;
using Sunnet.Cli.Core.Reports.Models;
using Sunnet.Framework.Core.Base;
using Sunnet.Cli.Core.Practices.Entities;
using Sunnet.Cli.Core.Practices.Interfaces;

namespace Sunnet.Cli.Impl.Practices
{
    public class PracticeStudentItemRpst : EFRepositoryBase<PracticeStudentItemEntity, int>, IPracticeStudentItemRpst
    {
        public PracticeStudentItemRpst(IUnitOfWork unit)
        {
            this.UnitOfWork = unit;
        }

    }
}
