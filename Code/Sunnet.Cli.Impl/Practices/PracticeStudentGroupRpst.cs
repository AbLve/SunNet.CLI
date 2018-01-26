using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/**************************************************************************
 * Developer: 		JackZhang
 * Computer:		JACKZ
 * Domain:			Jackz
 * CreatedOn:		2014/9/4 4:22:23
 * Description:		Please input class summary
 * Version History:	Created,2014/9/4 4:22:23
 * 
 * 
 **************************************************************************/
using Sunnet.Cli.Core;
using Sunnet.Cli.Core.Reports.Models;
using Sunnet.Framework.Core.Base;
using Sunnet.Framework.Extensions;
using Sunnet.Cli.Core.Cpalls.Models.Report;
using Sunnet.Cli.Core.Ade;
using Sunnet.Cli.Core.Cpalls;
using Sunnet.Cli.Core.Practices.Entities;
using Sunnet.Cli.Core.Practices.Interfaces;
using Sunnet.Cli.Core.Practices.Entites;

namespace Sunnet.Cli.Impl.Practices
{
    public class PracticeStudentGroupRpst : EFRepositoryBase<PracticeStudentGroupEntity, int>, IPracticeStudentGroupRpst
    {
        public PracticeStudentGroupRpst(IUnitOfWork unit)
        {
            this.UnitOfWork = unit;
        }
    
    }
}