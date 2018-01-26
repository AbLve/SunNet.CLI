using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sunnet.Cli.Core.Ade.Enums;
using System.ComponentModel;

namespace Sunnet.Cli.Core.Ade.Entities
{
    public class TxkeaExpressiveItemEntity : ItemBaseEntity
    {
        public string   BackgroundFill { get; set; }

        public BackgroundFillType BackgroundFillType { get; set; }

 


        [Description("Question Instruction Text")]
        public string InstructionText { get; set; }

        [Description("Question Instruction Audio")]
        public string InstructionAudio { get; set; }

        [Description("Question Instruction Audio Time Delay")]
        public int InstructionAudioTimeDelay { get; set; }
                
        [DisplayName("Number of Images")]
        public int Images { get; set; }

        /// <summary>
        /// 保存Items 的 Layout设计 json
        /// </summary>
        public string ItemLayout { get; set; }

        //从ItemLayout中抽离出只用于Cpalls布局的信息
        public string CpallsItemLayout { get; set; }

        /// <summary>
        /// Response background
        /// </summary>
        public string ResponseBackgroundFill { get; set; }

        /// <summary>
        /// Response background
        /// </summary>
        public BackgroundFillType ResponseBackgroundFillType { get; set; }

        /// <summary>
        /// 保存 ItemLayout 时的屏幕宽度
        /// </summary>
        public int ScreenWidth { get; set; }
        public int ScreenHeight { get; set; }
        /// <summary>
        /// 整个Item 完成，可以设置 active状态
        /// </summary>
        public bool IsCompleted { get; set; }

        public int Step { get; set; }

        /// <summary>
        /// 选中了哪个Layout 模板
        /// </summary>
        public int LayoutId { get; set; }

        /// <summary>
        /// Time out value
        /// </summary>
        [DisplayName("Time out value")]
        public int Timeoutvalue { get; set; }

        /// <summary>
        /// Response Type
        /// </summary>
        [DisplayName("Response Type")]
        public TxkeaExpressiveResponoseType ResponseType { get; set; }

        public virtual TxkeaLayoutEntity Layout { get; set; }        

        private ICollection<TxkeaExpressiveImageEntity> _imageList;

        public virtual ICollection<TxkeaExpressiveImageEntity> ImageList
        {
            get { return _imageList ?? (_imageList = new List<TxkeaExpressiveImageEntity>()); }
            set { _imageList = value; }
        }

        private ICollection<TxkeaExpressiveResponseEntity> _responses;
        public virtual ICollection<TxkeaExpressiveResponseEntity> Responses
        {
            get { return _responses ?? (_responses = new List<TxkeaExpressiveResponseEntity>()); }
            set { _responses = value; }
        }
    }
}
