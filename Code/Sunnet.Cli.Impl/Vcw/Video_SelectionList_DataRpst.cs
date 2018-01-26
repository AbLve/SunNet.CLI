using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/**************************************************************************
 * Developer: 		Joe
 * Computer:		Joe-PC
 * Domain:			Joe-pc
 * CreatedOn:		2015/3/18
 * Description:		Create AuthorityEntity
 * Version History:	Created,2015/3/18
 * 
 * 
 **************************************************************************/
using Sunnet.Framework.Core.Base;
using Sunnet.Cli.Core.Vcw.Entities;
using Sunnet.Cli.Core.Vcw.Interfaces;

namespace Sunnet.Cli.Impl.Vcw
{
    public class Video_SelectionList_DataRpst : EFRepositoryBase<Video_SelectionList_DataEntity, Int32>, IVideo_SelectionList_DataRpst
    {
        public Video_SelectionList_DataRpst(IUnitOfWork unit)
        {
            this.UnitOfWork = unit;
        }
    }
}
