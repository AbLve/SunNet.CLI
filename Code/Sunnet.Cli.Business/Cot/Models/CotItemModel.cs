using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/**************************************************************************
 * Developer: 		JackZhang
 * Computer:		JACKZ
 * Domain:			Jackz
 * CreatedOn:		2014/12/17 10:02:54
 * Description:		Please input class summary
 * Version History:	Created,2014/12/17 10:02:54
 * 
 * 
 **************************************************************************/
using Newtonsoft.Json;
using Sunnet.Cli.Business.Ade.Models;
using Sunnet.Cli.Business.Common;
using Sunnet.Cli.Core.Ade;
using Sunnet.Cli.Core.Ade.Entities;
using Sunnet.Cli.Core.Cpalls;

namespace Sunnet.Cli.Business.Cot.Models
{
    public class CotItemModel
    {
        public CotItemModel()
        {
        }

        public int ID { get; set; }

        public int CotAssessmentId { get; set; }

        public int ItemId { get; set; }

        public CotLevel Level { get; set; }

        public string ShortTargetText { get; set; }

        public string FullTargetText { get; set; }

        public string PrekindergartenGuidelines { get; set; }

        public string CircleManual { get; set; }

        public string MentoringGuide { get; set; }

        public string CotItemId { get; set; }

        public DateTime GoalSetDate { get; set; }

        public DateTime GoalMetDate { get; set; }

        /// <summary>
        /// 此属性标识 Wave1 是否Observe了该Item(日期大于系统最小日期CommonAgent.MinDate)
        /// </summary>
        public DateTime BoyObsDate { get; set; }

        /// <summary>
        /// 此属性标识 Wave2 是否Observe了该Item(日期大于系统最小日期CommonAgent.MinDate)
        /// </summary>
        public DateTime MoyObsDate { get; set; }

        /// <summary>
        /// STG Report是否处于等待状态：已完成GoalSetDate，未完成GoalMetDate，处于等待状态将会在COT Report页面上选中Item.
        /// </summary>
        public bool WaitingGoalMet { get; set; }

        public bool NeedSupport { get; set; }

        public DateTime CotUpdatedOn { get; set; }

        public int CreatedBy { get; set; }

        public int UpdatedBy { get; set; }

        public DateTime CreatedOn { get; set; }

        public DateTime UpdatedOn { get; set; }

        /// <summary>
        /// 是否启用GoalMetDate 设置：设置完GoalSetDate并且填写完表单,尚未设置GoalMetDate
        /// </summary>
        public bool GoalMetAble { get; set; }

        /// <summary>
        /// Item 是否已经完成：GoalMetDate已完成
        /// </summary>
        public bool GoalMetDone
        {
            get { return GoalMetDate > CommonAgent.MinDate; }
        }

        public List<AdeLinkModel> Links { get; set; }


        /// <summary>
        /// STG Report页面自定义排序.
        /// </summary>
        public int Sort { get; set; }
    }
}


