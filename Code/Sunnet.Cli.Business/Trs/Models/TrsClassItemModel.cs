using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sunnet.Cli.Core.Trs.Enums;
/**************************************************************************
 * Developer: 		Joe
 * Computer:		Joe
 * Domain:			Joe
 * CreatedOn:		1/12 2015 16:47:17
 * Description:		Please input class summary
 * Version History:	Created,1/12 2015 16:47:17
 * 
 * 
 **************************************************************************/

namespace Sunnet.Cli.Business.Trs.Models
{
    public class TrsClassItemModel
    {
        public int ClassId { get; set; }

        public string ClassName { get; set; }

        public int AnswerId { get; set; }

        public int Score { get; set; }

        public string AnswerText { get; set; }

        public string Comments { get; set; }

        public int GroupSize { get; set; }

        public int CaregiversNo { get; set; }

        public ItemAgeGroup AgeGroup { get; set; }
    }
}
