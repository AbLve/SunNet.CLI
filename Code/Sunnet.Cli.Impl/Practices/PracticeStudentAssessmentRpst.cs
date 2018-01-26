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
 * CreatedOn:		2014/9/4 4:21:56
 * Description:		Please input class summary
 * Version History:	Created,2014/9/4 4:21:56
 * 
 * 
 **************************************************************************/
using Sunnet.Framework.Core.Base;
using Sunnet.Cli.Core;
using Sunnet.Cli.Core.Practices.Entities;
using Sunnet.Cli.Core.Practices.Interfaces;

namespace Sunnet.Cli.Impl.Practices
{
    public class PracticeStudentAssessmentRpst : EFRepositoryBase<PracticeStudentAssessmentEntity, int>, IPracticeStudentAssessmentRpst
    {
        public PracticeStudentAssessmentRpst(IUnitOfWork unit)
        {
            this.UnitOfWork = unit;
        } 
    }
}


