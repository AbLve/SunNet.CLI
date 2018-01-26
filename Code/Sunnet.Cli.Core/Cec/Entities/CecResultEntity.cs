using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/**************************************************************************
 * Developer: 		Leason
 * Computer:		Leason-PC
 * Domain:			Leason
 * CreatedOn:		2014/12/1 11:20:00
 * Description:		CecResultEntity
 * Version History:	Created,2014/12/1 11:20:00
 * 
 * 
 **************************************************************************/
using Sunnet.Framework.Core.Base;
using Sunnet.Framework.Mvc;

namespace Sunnet.Cli.Core.Cec.Entities
{
    public class CecResultEntity:EntityBase<int>
    {
        [EensureEmptyIfNull]
        public int CecHistoryId { get; set; }
        
        public int ItemId { get; set; }

        public int AnswerId { get; set; }

        [EensureEmptyIfNull]
        public decimal Score { get; set; }
        
        [EensureEmptyIfNull]
        [DisplayName("Created By")]
        
        public int CreatedBy { get; set; }

        [EensureEmptyIfNull]
        [DisplayName("Updated By")]
        public int UpdatedBy { get; set; }

        public virtual CecHistoryEntity CecHistory { get; set; }
    }
}
