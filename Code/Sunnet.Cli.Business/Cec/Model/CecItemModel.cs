using System.Collections.Generic;
using Sunnet.Cli.Core.Ade.Entities;

/**************************************************************************
 * Developer: 		Leason
 * Computer:		Leason-PC
 * Domain:			Leason
 * CreatedOn:		2014/11/28 14:53:10
 * Description:		For Cec Item
 * Version History:	Created,2014/11/28 14:53:10
 * 
 * 
 **************************************************************************/

namespace Sunnet.Cli.Business.Cec.Model
{
    public class CecItemModel
    {
        public CecItemModel()
        {
            Answer = new List<CecAnswerModel>();
        }

        public string Area { get; set; }

        public string Description { get; set; }

        public int ItemId { get; set; }

        public int Sort { get; set; }

        public int MeasureId { get; set; }
        
        public string MeasureName { get; set; }

        public string Wave { get; set; }

        public decimal Score { get; set; }

        public bool IsRequired { get; set; }

        public IEnumerable<CecAnswerModel> Answer { get; set; }

        public List<AdeLinkEntity>  Links { get; set; }
    }

   
}
