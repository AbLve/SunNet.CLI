using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/**************************************************************************
 * Developer: 		Joe
 * Computer:		JOE-PC
 * Domain:			Joe
 * CreatedOn:		2015/12/22
 * Description:		
 * Version History:	Created,2015/12/22
 * 
 * 
 **************************************************************************/
using Sunnet.Framework.Core.Base;


namespace Sunnet.Cli.Core.Ade.Entities
{
    public class TxkeaBupLogEntity : EntityBase<int>
    {
        public int TaskId { get; set; }

        public int RowNumber { get; set; }

        public string ItemName { get; set; }

        public string Remark { get; set; }

        public virtual TxkeaBupTaskEntity Task { get; set; }
    }
}
