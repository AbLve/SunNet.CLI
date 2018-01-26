using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Sunnet.Cli.Core.Ade.Entities;
using Sunnet.Cli.Core.Ade.Interfaces;
using Sunnet.Framework.Core.Base;

/**************************************************************************
 * Developer: 		Jack
 * Computer:		Jackz
 * Domain:			Jackz
 * CreatedOn:		08/11/2014 03:52:57
 * Description:		AdeLinkEntity's IRepository
 * Version History:	Created,08/11/2014 03:52:57
 * 
 * 
 **************************************************************************/
namespace Sunnet.Cli.Impl.Ade
{
	/// <summary>
    /// AdeLinkEntity's Repository
    /// </summary>
	public class AdeLinkRpst:EFRepositoryBase<AdeLinkEntity, int>, IAdeLinkRpst
    {
        public AdeLinkRpst(IUnitOfWork unit)
        {
            this.UnitOfWork = unit;
        }
    }
}
