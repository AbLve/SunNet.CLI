using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/**************************************************************************
 * Developer: 		JackZhang
 * Computer:		JACKZ
 * Domain:			Jackz
 * CreatedOn:		2014/9/11 2:28:02
 * Description:		Please input class summary
 * Version History:	Created,2014/9/11 2:28:02
 * 
 * 
 **************************************************************************/
using Sunnet.Cli.Core.Ade;
using Sunnet.Cli.Core.Ade.Entities;
using Sunnet.Cli.Core.Cpalls;

namespace Sunnet.Cli.Business.Cpalls.Models
{
    public class ExecCpallsItemModel
    {

        public int ExecId { get; set; }

        public int MeasureId { get; set; }

        public int ItemId { get; set; }

        public AnswerType AnswerType { get; set; }

        public string Label { get; set; }

        public bool Timed { get; set; }

        //public int Timeout { get; set; }

        public int WaitTime { get; set; }

        public bool Scored { get; set; }

        public string Description { get; set; }

        public decimal Score { get; set; }

        public ItemType Type { get; set; }


        //public string TargetText { get; set; }
        //public int TargetTextTimeout { get; set; }
        //public string TargetAudio { get; set; }
        //public int TargetAudioTimeout { get; set; }

        //public string PromptPicture { get; set; }
        //public int PromptPictureTimeout { get; set; }

        //public string PromptText { get; set; }
        //public int PromptTextTimeout { get; set; }

        //public string PromptAudio { get; set; }
        //public int PromptAudioTimeout { get; set; }

        /// <summary>
        /// Direction
        /// </summary> 
        //public string DirectionText { get; set; }

        //public CecItemsDirection Direction { get; set; }

        //public bool IsMultiChoice { get; set; }

        public List<AnswerEntity> Answers { get; set; }

        private List<string> _links;
        private Dictionary<string, object> _props;

        /// <summary>
        /// items links.
        /// </summary>
        public List<string> Links
        {
            get { return _links ?? (_links = new List<string>()); }
            set { _links = value; }
        }
        public int PauseTime { get; set; }

        // Student item props
        public decimal? Goal { get; set; }

        public bool IsCorrect { get; set; }
        public List<int> SelectedAnswers { get; set; }
        public CpallsStatus Status { get; set; }
        // Student item props end

        //public int Response { get; set; }

        public bool RandomAnswer { get; set; }

        /// <summary>
        /// 除ItemBases 之外的属性
        /// </summary>
        /// Author : JackZhang
        /// Date   : 6/29/2015 16:22:46
        public Dictionary<string, object> Props
        {
            get { return _props ?? (_props = new Dictionary<string, object>()); }
            set { _props = value; }
        }

        /// <summary>
        /// Response 填写的详细信息以JSON序列化格式保存
        /// </summary>
        /// Author : JackZhang
        /// Date   : 7/6/2015 09:24:57
        public string Details { get; set; }

        /// <summary>
        /// 表示是执行过的 Item, 因为有Item 跳转逻辑，有些Item 是不会执行的
        /// 已执行的为 True ，跳过的 Item 的为 False
        /// </summary>
        public bool Executed { get; set; }

        /// <summary>
        /// 记录上一个Item 的index 顺序。主要为 Item 上的后退按钮服务
        /// </summary>        
        public int LastItemIndex { get; set; }

        //生成结果页面中显示的索引（Branching Skip时需要特殊处理）
        public int ResultIndex { get; set; }

        public bool IsPractice { get; set; }

        public bool ShowAtTestResume { get; set; }
    }
}
