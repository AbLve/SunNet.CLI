using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/**************************************************************************
 * Developer: 		JackZhang
 * Computer:		JACKZ
 * Domain:			JackZ
 * CreatedOn:		1/12 2015 16:01:43
 * Description:		Please input class summary
 * Version History:	Created,1/12 2015 16:01:43
 * 
 * 
 **************************************************************************/
using DocumentFormat.OpenXml.Office.CustomUI;
using DocumentFormat.OpenXml.Wordprocessing;
using LinqKit;
using Sunnet.Cli.Business.Classrooms;
using Sunnet.Cli.Business.Schools;
using Sunnet.Cli.Business.Trs.Models;
using Sunnet.Cli.Core;
using Sunnet.Cli.Core.Ade;
using Sunnet.Cli.Core.Trs;
using Sunnet.Cli.Core.Trs.Entities;
using Sunnet.Cli.Core.Users.Entities;
using Sunnet.Framework.Core.Tool;

namespace Sunnet.Cli.Business.Trs
{
    public partial class TrsBusiness
    {
        public TRSItemEntity GetItem(int id)
        {
            return _trsContract.GetItem(id);
        }

        public OperationResult UpdateItem(TRSItemEntity entity)
        {
            return _trsContract.UpdateItem(entity);
        }
    }
}
