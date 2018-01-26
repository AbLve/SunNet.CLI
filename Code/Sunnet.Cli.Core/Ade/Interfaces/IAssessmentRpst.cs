using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Sunnet.Cli.Core.Ade.Entities;
using Sunnet.Framework.Core.Base;

/**************************************************************************
 * Developer: 		Jack
 * Computer:		Jackz
 * Domain:			Jackz
 * CreatedOn:		08/11/2014 03:49:39
 * Description:		AssessmentEntity's IRepository
 * Version History:	Created,08/11/2014 03:49:39
 * 
 * 
 **************************************************************************/
namespace Sunnet.Cli.Core.Ade.Interfaces
{
	/// <summary>
    /// AssessmentEntity's IRepository
    /// </summary>
	public interface IAssessmentRpst : IRepository<AssessmentEntity, int>
	{
	    int Unlock(int id);
	}
}
