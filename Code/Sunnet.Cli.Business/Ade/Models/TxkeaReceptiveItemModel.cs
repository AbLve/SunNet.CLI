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
using Sunnet.Cli.Core.Ade.Entities;
using Sunnet.Cli.Core.Ade;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Sunnet.Cli.Core.Ade.Enums;
using Sunnet.Framework.Mvc;
using Sunnet.Cli.Core.Common.Enums;
using Sunnet.Framework.Extensions;


namespace Sunnet.Cli.Business.Ade.Models
{
    public class TxkeaReceptiveItemModel : ItemModelBase<TxkeaReceptiveItemEntity>
    {
        public override ItemType Type
        {
            get
            {
                return ItemType.TxkeaReceptive;
            }
            set { }
        }

        [DisplayName("Choose background color fill or image")]
        [StringLength(200)]
        [EensureEmptyIfNull]
        public string BackgroundFill { get; set; }

        public BackgroundFillType BackgroundFillType { get; set; }

        //用于读取时存储背景图片值
        public string BackgroundImage { get; set; }

        //用于读取时存储引用Layout背景色
        public string LayoutBackgroundFill { get; set; }

        //用于读取时存储引用Layout背景图片值
        public string LayoutBackgroundImage { get; set; }

        //用于读取时存储引用Layout背景类型
        public BackgroundFillType LayoutBackgroundFillType { get; set; }

        [DisplayName("Instruction Audio")]
        [StringLength(200)]
        [EensureEmptyIfNull]
        public string InstructionAudio { get; set; }

        [DisplayName("Instruction Text")]
        [StringLength(1000)]
        [EensureEmptyIfNull]
        public string InstructionText { get; set; }

        public string InstructionTextNoHtml
        {
            get
            {
                if (!string.IsNullOrEmpty(InstructionText))
                    return InstructionText.ReplaceHtmlTag();
                else
                    return "";
            }
        }

        [DisplayName("Number of Image-Sound Pairs")]
        public int NumberOfImages { get; set; }

        [DisplayName("Selection Type")]
        public SelectionType SelectionType { get; set; }

        [DisplayName("Image Sequence")]
        public OrderType ImageSequence { get; set; }

        [DisplayName("Overall Timeout Delay")]
        [Range(100, 30000)]
        public int TimeoutValue { get; set; }

        [DisplayName("Overall Timeout")]
        public bool OverallTimeOut { get; set; }

        [DisplayName("Break Condition")]
        public BreakCondition BreakCondition { get; set; }

        public int StopConditionX { get; set; }

        public int StopConditionY { get; set; }

        public ScoringType Scoring { get; set; }

        [EensureEmptyIfNull]
        public string ItemLayout { get; set; }

        //从ItemLayout中抽离出只用于Cpalls布局的信息
        [EensureEmptyIfNull]
        public string CpallsItemLayout { get; set; }

        public decimal ScreenWidth { get; set; }

        public decimal ScreenHeight { get; set; }

        public int LayoutId { get; set; }

        [DisplayName("Grayed out Delay")]
        public bool GrayedOutDelay { get; set; }

        /// <summary>
        /// 共4步
        /// </summary>
        public int Step { get; set; }

        public TxkeaReceptiveItemModel()
            : base()
        {
            BackgroundFillType = BackgroundFillType.Color;
            SelectionType = SelectionType.SingleSelect;
            ImageSequence = OrderType.Sequenced;
            Scoring = ScoringType.AllorNone;
            OverallTimeOut = true;
            BreakCondition = BreakCondition.IncorrectResponse;
            CpallsItemLayout = string.Empty;
        }

        public TxkeaReceptiveItemModel(TxkeaReceptiveItemEntity entity)
            : base(entity)
        {
            this.BackgroundFill = string.Empty;
            this.BackgroundImage = string.Empty;
            if (entity.BackgroundFillType == BackgroundFillType.Color)
                this.BackgroundFill = entity.BackgroundFill;
            else
                this.BackgroundImage = entity.BackgroundFill;
            this.BackgroundFillType = entity.BackgroundFillType;

            this.LayoutBackgroundFill = string.Empty;
            this.LayoutBackgroundImage = string.Empty;
            this.LayoutBackgroundFillType = BackgroundFillType.Color;
            if (entity.Layout != null)
            {
                if (entity.Layout.BackgroundFillType == BackgroundFillType.Color)
                    this.LayoutBackgroundFill = entity.Layout.BackgroundFill;
                else
                    this.LayoutBackgroundImage = entity.Layout.BackgroundFill;

                this.LayoutBackgroundFillType = entity.Layout.BackgroundFillType;
            }

            this.InstructionAudio = entity.InstructionAudio;
            this.InstructionText = entity.InstructionText;
            this.NumberOfImages = entity.NumberOfImages;
            this.SelectionType = entity.SelectionType;
            this.ImageSequence = entity.ImageSequence;
            this.Timed = entity.Timed;
            this.OverallTimeOut = entity.OverallTimeOut;
            this.TimeoutValue = entity.TimeoutValue;
            this.BreakCondition = entity.BreakCondition;
            this.StopConditionX = entity.StopConditionX;
            this.StopConditionY = entity.StopConditionY;
            this.Scoring = entity.Scoring;
            this.ItemLayout = entity.ItemLayout;
            this.CpallsItemLayout = entity.CpallsItemLayout;
            this.ScreenWidth = entity.ScreenWidth;
            this.ScreenHeight = entity.ScreenHeight;
            this.LayoutId = entity.LayoutId;
            this.GrayedOutDelay = entity.GrayedOutDelay;
        }

        public override void UpdateEntity(TxkeaReceptiveItemEntity entity)
        {
            base.UpdateEntity(entity);

            entity.BackgroundFill = this.BackgroundFill;
            entity.BackgroundFillType = this.BackgroundFillType;
            entity.InstructionAudio = this.InstructionAudio;
            entity.InstructionText = this.InstructionText;
            entity.NumberOfImages = this.NumberOfImages;
            entity.SelectionType = this.SelectionType;
            entity.ImageSequence = this.ImageSequence;
            entity.Timed = this.Timed;
            entity.OverallTimeOut = this.OverallTimeOut;
            entity.TimeoutValue = this.TimeoutValue;
            entity.BreakCondition = this.BreakCondition;
            entity.StopConditionX = this.StopConditionX;
            entity.StopConditionY = this.StopConditionY;
            entity.Scoring = this.Scoring;
            entity.ItemLayout = this.ItemLayout;
            entity.CpallsItemLayout = this.CpallsItemLayout;
            entity.ScreenWidth = this.ScreenWidth;
            entity.ScreenHeight = this.ScreenHeight;
            entity.LayoutId = this.LayoutId;
            entity.GrayedOutDelay = this.GrayedOutDelay;

            if (this.Answers != null && !this.IsPractice)
                entity.Score = this.Answers.Sum(a => a.Score);
            else
                entity.Score = 0;
        }

        public override void UpdateModel(TxkeaReceptiveItemEntity entity)
        {
            base.UpdateModel(entity);
            this.BackgroundFill = string.Empty;
            this.BackgroundImage = string.Empty;
            if (entity.BackgroundFillType == BackgroundFillType.Color)
                this.BackgroundFill = entity.BackgroundFill;
            else
                this.BackgroundImage = entity.BackgroundFill;
            this.BackgroundFillType = entity.BackgroundFillType;

            this.LayoutBackgroundFill = string.Empty;
            this.LayoutBackgroundImage = string.Empty;
            this.LayoutBackgroundFillType = BackgroundFillType.Color;
            if (entity.Layout != null)
            {
                if (entity.Layout.BackgroundFillType == BackgroundFillType.Color)
                    this.LayoutBackgroundFill = entity.Layout.BackgroundFill;
                else
                    this.LayoutBackgroundImage = entity.Layout.BackgroundFill;

                this.LayoutBackgroundFillType = entity.Layout.BackgroundFillType;
            }

            this.InstructionText = entity.InstructionText;
            this.NumberOfImages = entity.NumberOfImages;
            this.SelectionType = entity.SelectionType;
            this.ImageSequence = entity.ImageSequence;
            this.OverallTimeOut = entity.OverallTimeOut;
            this.TimeoutValue = entity.TimeoutValue;
            this.BreakCondition = entity.BreakCondition;
            this.StopConditionX = entity.StopConditionX;
            this.StopConditionY = entity.StopConditionY;
            this.ItemLayout = entity.ItemLayout;
            this.CpallsItemLayout = entity.CpallsItemLayout;
            this.ScreenWidth = entity.ScreenWidth;
            this.ScreenHeight = entity.ScreenHeight;
            this.LayoutId = entity.LayoutId;
            // Layout 模板 背景色或图片 begin
            this.LayoutBackgroundFill = string.Empty;
            this.LayoutBackgroundImage = string.Empty;
            this.LayoutBackgroundFillType = BackgroundFillType.Color;
            if (entity.Layout != null)
            {
                if (entity.Layout.BackgroundFillType == BackgroundFillType.Color)
                    this.LayoutBackgroundFill = entity.Layout.BackgroundFill;
                else
                    this.LayoutBackgroundImage = entity.Layout.BackgroundFill;

                this.LayoutBackgroundFillType = entity.Layout.BackgroundFillType;
            }
            // Layout 模板 背景色或图片 end

            this.Answers = entity.Answers.ToList();
            this.GrayedOutDelay = entity.GrayedOutDelay;
            this.BranchingScores = entity.BranchingScores.Where(r => r.IsDeleted == false)
                .Select(BranchingScoresEntityToModelSelector).ToList();
        }

        public override Cpalls.Models.ExecCpallsItemModel ExecCpallsItemModel
        {
            get
            {
                base.ExecCpallsItemModel.Props.Add("CpallsItemLayout", CpallsItemLayout);
                base.ExecCpallsItemModel.Props.Add("ScreenWidth", ScreenWidth);
                base.ExecCpallsItemModel.Props.Add("ScreenHeight", ScreenHeight);
                base.ExecCpallsItemModel.Props.Add("InstructionText", InstructionText);
                base.ExecCpallsItemModel.Props.Add("InstructionAudio", InstructionAudio);
                base.ExecCpallsItemModel.Props.Add("OverallTimeOut", OverallTimeOut);
                base.ExecCpallsItemModel.Props.Add("TimeoutValue", TimeoutValue);
                base.ExecCpallsItemModel.Props.Add("BreakCondition", BreakCondition);
                base.ExecCpallsItemModel.Props.Add("StopConditionX", StopConditionX);
                base.ExecCpallsItemModel.Props.Add("StopConditionY", StopConditionY);
                base.ExecCpallsItemModel.Props.Add("GrayedOutDelay", GrayedOutDelay);
                base.ExecCpallsItemModel.Props.Add("SelectionType", SelectionType);
                base.ExecCpallsItemModel.Props.Add("Scoring", Scoring);
                base.ExecCpallsItemModel.Props.Add("ImageSequence", ImageSequence);
                base.ExecCpallsItemModel.Props.Add("BackgroundImage", BackgroundImage);
                base.ExecCpallsItemModel.Props.Add("LayoutBackgroundImage", LayoutBackgroundImage);
                if (BranchingScores != null)
                    base.ExecCpallsItemModel.Props.Add("BranchingScores", this.BranchingScores);
                return base.ExecCpallsItemModel;
            }
        }
    }
}
