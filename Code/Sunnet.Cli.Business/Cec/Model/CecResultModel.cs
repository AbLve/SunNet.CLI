using System;
using System.Collections.Generic;
using Sunnet.Cli.Core.Ade.Entities;

/**************************************************************************
 * Developer: 		Leason
 * Computer:		Leason-PC
 * Domain:			Leason
 * CreatedOn:		2014/12/2 12:19:10
 * Description:		CecResultModel
 * Version History:	Created,2014/12/2 12:19:10
 * 
 * 
 **************************************************************************/
namespace Sunnet.Cli.Business.Cec.Model
{
    public class CecResultModel
    {
        public int ID { get; set; }

        public int CecHistoryId { get; set; }

        public DateTime AssessmentDate { get; set; }

        public int ItemId { get; set; }

        public int AnswerId { get; set; }

        public decimal Score { get; set; }

        public List<AdeLinkEntity> Links { get; set; }

        public DateTime CreatedOn { get; set; }

        public DateTime UpdatedOn { get; set; }
    }
}
