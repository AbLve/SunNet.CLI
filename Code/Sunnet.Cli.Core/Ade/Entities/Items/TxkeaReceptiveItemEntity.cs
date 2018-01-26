using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/**************************************************************************
 * Developer: 		Joe
 * Computer:		JOE-PC
 * Domain:			Joe
 * CreatedOn:		2015/10/23
 * Description:		Add TxkeaReceptive Item
 * Version History:	Created,2015/10/23
 * 
 * 
 **************************************************************************/
using Sunnet.Cli.Core.Ade.Enums;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Sunnet.Cli.Core.Common.Enums;

namespace Sunnet.Cli.Core.Ade.Entities
{
    public class TxkeaReceptiveItemEntity : ItemBaseEntity
    {
        public TxkeaReceptiveItemEntity()
        {
            Type = ItemType.TxkeaReceptive;
            ScreenHeight = 0;
        }

        [StringLength(200)]
        public string BackgroundFill { get; set; }
 

        public BackgroundFillType BackgroundFillType { get; set; }

        public string InstructionAudio { get; set; }

        public string InstructionText { get; set; }

        public int NumberOfImages { get; set; }

        public SelectionType SelectionType { get; set; }

        public OrderType ImageSequence { get; set; }

        public bool OverallTimeOut { get; set; }

        public int TimeoutValue { get; set; }

        public BreakCondition BreakCondition { get; set; }

        public int StopConditionX { get; set; }

        public int StopConditionY { get; set; }

        public ScoringType Scoring { get; set; }

        public string ItemLayout { get; set; }

        //从ItemLayout中抽离出只用于Cpalls布局的信息
        public string CpallsItemLayout { get; set; }

        public decimal ScreenWidth { get; set; }

        public decimal ScreenHeight { get; set; }

        public int LayoutId { get; set; }

        public bool GrayedOutDelay { get; set; }

        public virtual TxkeaLayoutEntity Layout { get; set; }
    }
}
