using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/**************************************************************************
 * Developer: 		JackZhang
 * Computer:		JACKZ
 * Domain:			JackZ
 * CreatedOn:		1/23 2015 15:34:55
 * Description:		Please input class summary
 * Version History:	Created,1/23 2015 15:34:55
 * 
 * 
 **************************************************************************/
using Sunnet.Cli.Core.Trs;

namespace Sunnet.Cli.Business.Trs.Models
{
    public class TrsItemBaseModel
    {
        public int Id { get; set; }
        public int ItemId { get; set; }

        public int AssessmentItemId { get; set; }

        public TRSCategoryEnum Category { get; set; }

        public int SubCategoryId { get; set; }

        /// <summary>
        /// item No
        /// </summary>
        public string Item { get; set; }

        public string Text { get; set; }


        public string TAPlanItem { get; set; }

        public string TAPlanItemType { get; set; }

        public string TAItemInstructions { get; set; }

        public string LinkingDocument { get; set; }

        /// <summary>
        /// 得分, 初始时为0, N/A为-1
        /// </summary>
        public int Score { get; set; }

        /// <summary>
        /// 选中的答案 TRSAnswer.ID ，初始时为 0 
        /// </summary>
        public int AnswerId { get; set; }

        public string AnswerText { get; set; }

        public int ClassId { get; set; }
    }
}
