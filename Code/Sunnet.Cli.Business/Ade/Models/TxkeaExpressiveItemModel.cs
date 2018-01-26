using Sunnet.Cli.Business.Cpalls.Models;
using Sunnet.Cli.Core.Ade;
using Sunnet.Cli.Core.Ade.Entities;
using Sunnet.Cli.Core.Ade.Enums;
using Sunnet.Framework.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sunnet.Framework.Extensions;

namespace Sunnet.Cli.Business.Ade.Models
{
    public class TxkeaExpressiveItemModel : ItemModelBase<TxkeaExpressiveItemEntity>
    {
        public override ItemType Type
        {
            get
            {
                return ItemType.TxkeaExpressive;
            }
            set { }
        }

        public override AnswerType AnswerType
        {
            get
            {
                return AnswerType.TxkeaTypedResponse;
            }
            set { }
        }

        [EensureEmptyIfNull]
        public string BackgroundFill { get; set; }

        [EensureEmptyIfNull]
        public string BackgroundImage { get; set; }

        public BackgroundFillType BackgroundFillType { get; set; }


        [EensureEmptyIfNull]
        public string LayoutBackgroundFill { get; set; }

        [EensureEmptyIfNull]
        public string LayoutBackgroundImage { get; set; }

        public BackgroundFillType LayoutBackgroundFillType { get; set; }

        [DisplayName("Question Instruction Text")]
        [StringLength(1000)]
        [EensureEmptyIfNull]
        public string InstructionText { get; set; }

        public string InstructionTextNoHtml
        {
            get
            {
                if (string.IsNullOrEmpty(InstructionText)) return string.Empty;
                else
                    return InstructionText.ReplaceHtmlTag();
            }
        }

        [DisplayName("Question Instruction Audio")]
        [StringLength(200)]
        [EensureEmptyIfNull]
        public string InstructionAudio { get; set; }

        [Range(0, 30000)]
        [DisplayName("Question Instruction Audio Time Delay")]
        public int InstructionAudioTimeDelay { get; set; }

        [DisplayName("Number of Image-Sound Pairs")]
        public int Images { get; set; }

        /// <summary>
        /// 保存Items 的 Layout设计 json
        /// </summary>
        [EensureEmptyIfNull]
        public string ItemLayout { get; set; }

        //从ItemLayout中抽离出只用于Cpalls布局的信息
        [EensureEmptyIfNull]
        public string CpallsItemLayout { get; set; }

        /// <summary>
        /// Response background
        /// </summary>
        [EensureEmptyIfNull]
        public string ResponseBackgroundFill { get; set; }

        [EensureEmptyIfNull]
        public string ResponseBackgroundImage { get; set; }

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

        /// <summary>
        /// 选中了哪个Layout 模板
        /// </summary>
        public int LayoutId { get; set; }


        /// <summary>
        /// 共4步
        /// </summary>
        public int Step { get; set; }


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

        public TxkeaExpressiveItemModel()
            : base()
        {
            this.ResponseType = TxkeaExpressiveResponoseType.Detailed;
        }

        public TxkeaExpressiveItemModel(TxkeaExpressiveItemEntity entity)
            : base(entity)
        {
            this.BackgroundFill = string.Empty;
            this.BackgroundImage = string.Empty;
            if (entity.BackgroundFillType == Core.Ade.Enums.BackgroundFillType.Color)
                this.BackgroundFill = entity.BackgroundFill;
            else
                this.BackgroundImage = entity.BackgroundFill;

            this.BackgroundFillType = entity.BackgroundFillType;

            this.LayoutBackgroundFill = string.Empty;
            this.LayoutBackgroundImage = string.Empty;
            this.LayoutBackgroundFillType = Core.Ade.Enums.BackgroundFillType.Color;
            if (entity.Layout != null)
            {
                if (entity.Layout.BackgroundFillType == Core.Ade.Enums.BackgroundFillType.Color)
                    this.LayoutBackgroundFill = entity.Layout.BackgroundFill;
                else
                    this.LayoutBackgroundImage = entity.Layout.BackgroundFill;

                this.LayoutBackgroundFillType = entity.Layout.BackgroundFillType;
            }

            this.InstructionText = entity.InstructionText;
            this.InstructionAudio = entity.InstructionAudio;
            this.InstructionAudioTimeDelay = entity.InstructionAudioTimeDelay;
            this.Images = entity.Images;
            this.ItemLayout = entity.ItemLayout;
            this.CpallsItemLayout = entity.CpallsItemLayout;

            this.ResponseBackgroundFill = string.Empty;
            this.ResponseBackgroundImage = string.Empty;
            if (entity.ResponseBackgroundFillType == Core.Ade.Enums.BackgroundFillType.Color)
                this.ResponseBackgroundFill = entity.ResponseBackgroundFill;
            else
                this.ResponseBackgroundImage = entity.ResponseBackgroundFill;

            this.ResponseBackgroundFillType = entity.ResponseBackgroundFillType;

            this.ScreenWidth = entity.ScreenWidth;
            this.ScreenHeight = entity.ScreenHeight;
            this.IsCompleted = entity.IsCompleted;
            this.LayoutId = entity.LayoutId;
            this.Timeoutvalue = entity.Timeoutvalue;
            this.ResponseType = entity.ResponseType;
            if ((byte)this.ResponseType < (byte)TxkeaExpressiveResponoseType.Simple)
                this.ResponseType = TxkeaExpressiveResponoseType.Detailed;


            this.ImageList = entity.ImageList.Where(r => r.IsDeleted == false).Select(EntityToModelSelector).ToList();

            if (this.ImageList.Count < Images)
            {
                while (this.ImageList.Count < Images)
                    this.ImageList.Add(new TxkeaExpressiveImageModel());
            }
            else if (this.ImageList.Count > Images)
            {
                for (int i = Images; i < this.ImageList.Count; i++)
                {
                    this.ImageList[i].IsDeleted = true;
                }
            }

            this.Responses = entity.Responses.Where(r => r.IsDeleted == false).Select(ResponseEntityToModelSelector).ToList();
            if (this.Responses.Count == 0)
                this.Responses.Add(new TxkeaExpressiveResponseModel(false));
        }

        private static Func<TxkeaExpressiveImageEntity, TxkeaExpressiveImageModel> EntityToModelSelector
        {
            get
            {
                return x => new TxkeaExpressiveImageModel()
                {
                    ID = x.ID,
                    ItemId = x.ItemId,
                    TargetImage = x.TargetImage,
                    ImageTimeDelay = x.ImageTimeDelay,
                    TargetAudio = x.TargetAudio,
                    AudioTimeDelay = x.AudioTimeDelay,
                    SameasImageDelay = x.SameasImageDelay
                };
            }
        }

        private static Func<TxkeaExpressiveImageModel, TxkeaExpressiveImageEntity> ModelToEntity
        {
            get
            {
                return x => new TxkeaExpressiveImageEntity()
                {
                    ID = x.ID,
                    ItemId = x.ItemId,
                    TargetImage = x.TargetImage,
                    ImageTimeDelay = x.ImageTimeDelay,
                    TargetAudio = x.TargetAudio,
                    AudioTimeDelay = x.AudioTimeDelay,
                    SameasImageDelay = x.SameasImageDelay,
                    IsDeleted = x.IsDeleted
                };
            }
        }

        private static Func<TxkeaExpressiveResponseEntity, TxkeaExpressiveResponseModel> ResponseEntityToModelSelector
        {
            get
            {
                return x => new TxkeaExpressiveResponseModel()
                {
                    ID = x.ID,
                    ItemId = x.ItemId,
                    Text = x.Text,
                    Mandatory = x.Mandatory,
                    Type = x.Type,
                    Buttons = x.Buttons,
                    IsDeleted = x.IsDeleted,
                    Options = x.Options.Where(o => o.IsDeleted == false).Select(r => new TxkeaExpressiveResponseOptionModel()
                    {
                        ID = r.ID,
                        ResponseId = r.ResponseId,
                        IsCorrect = r.IsCorrect,
                        Lable = r.Lable,
                        AddTextbox = r.AddTextbox,
                        Score = r.Score,
                        IsDeleted = r.IsDeleted
                    }).ToList()
                };
            }
        }

        private static Func<TxkeaExpressiveResponseModel, TxkeaExpressiveResponseEntity> ResponseModelToEntity
        {
            get
            {
                return x => new TxkeaExpressiveResponseEntity()
                {
                    ID = x.ID,
                    ItemId = x.ItemId,
                    Text = x.Text,
                    Mandatory = x.Mandatory,
                    Type = x.Type,
                    Buttons = x.Buttons,
                    IsDeleted = x.IsDeleted,
                    Options = x.Options.Where(r => r.ID > 0 || (r.ID == 0 && r.IsDeleted == false)).Select(r => new TxkeaExpressiveResponseOptionEntity()
                    {
                        ID = r.ID,
                        ResponseId = r.ResponseId,
                        // IsCorrect = r.IsCorrect,
                        Lable = r.Lable == null ? string.Empty : r.Lable,
                        AddTextbox = r.AddTextbox,
                        Score = r.Score,
                        IsDeleted = r.IsDeleted
                    }).ToList()
                };
            }
        }


        public void Copy(TxkeaExpressiveItemEntity entity, TxkeaExpressiveItemModel copyModel)
        {
            base.UpdateEntity(entity);

            entity.IsCompleted = true;
            entity.Status = Core.Common.Enums.EntityStatus.Inactive;

            if (this.BackgroundImage.Trim() != string.Empty)
            {
                entity.BackgroundFill = this.BackgroundImage;
                entity.BackgroundFillType = Core.Ade.Enums.BackgroundFillType.Image;
            }
            else
            {
                entity.BackgroundFill = this.BackgroundFill;
                entity.BackgroundFillType = Core.Ade.Enums.BackgroundFillType.Color;
            }

            entity.InstructionText = this.InstructionText;
            entity.InstructionAudio = this.InstructionAudio;
            entity.InstructionAudioTimeDelay = this.InstructionAudioTimeDelay;
            entity.Images = this.Images;

            ///-----------------------------
            var tmpImageList = copyModel.ImageList;
            if (entity.Images != tmpImageList.Count)
            {
                entity.LayoutId = 0;
            }
            else
                entity.LayoutId = copyModel.LayoutId;

            if (entity.Images < tmpImageList.Count())
            {
                for (var i = entity.Images; i < tmpImageList.Count; i++)
                {
                    tmpImageList[i].IsDeleted = true;
                }
            }

            entity.ItemLayout = copyModel.ItemLayout;
            entity.CpallsItemLayout = copyModel.CpallsItemLayout;
            entity.ResponseBackgroundFill = copyModel.ResponseBackgroundImage;
            entity.ResponseBackgroundFillType = copyModel.ResponseBackgroundFillType;
            entity.ScreenWidth = copyModel.ScreenWidth;
            entity.ScreenHeight = copyModel.ScreenHeight;
            entity.Timeoutvalue = this.Timeoutvalue;
            entity.ResponseType = this.ResponseType;

            ////Edit by Sam ==>  Find the Score and Scored field In DB is null
            if (this.IsPractice)
                entity.Score = 0;
            else if (entity.ResponseType == TxkeaExpressiveResponoseType.Simple)
                entity.Score = 1;
            else if (copyModel.ResponseType == TxkeaExpressiveResponoseType.Detailed)
            {
                entity.Score = copyModel.Score;   
            }
            //////////---------------------------------
            List<TxkeaExpressiveImageEntity> imageList = copyModel.ImageList.Select(ModelToEntity).ToList();
            List<TxkeaExpressiveImageEntity> existedEntities = new List<TxkeaExpressiveImageEntity>();

            foreach (TxkeaExpressiveImageEntity imageEntity in imageList)
            {
                var existed = existedEntities.SingleOrDefault(r => r.ID == imageEntity.ID);
                if (existed != null)
                {
                    existed.TargetImage = imageEntity.TargetImage;
                    existed.ImageTimeDelay = imageEntity.ImageTimeDelay;
                    existed.TargetAudio = imageEntity.TargetAudio;
                    existed.AudioTimeDelay = imageEntity.AudioTimeDelay;
                    existed.SameasImageDelay = imageEntity.SameasImageDelay;
                    existed.IsDeleted = imageEntity.IsDeleted;
                }
                else
                {
                    imageEntity.ID = 0;
                    imageEntity.ItemId = 0;
                    entity.ImageList.Add(imageEntity);
                }
            }

            ///////////////////////////////////////
            List<TxkeaExpressiveResponseEntity> responses = copyModel.Responses.Select(ResponseModelToEntity).ToList();

            foreach (TxkeaExpressiveResponseEntity tmpResonse in responses)
            {
                tmpResonse.ID = 0;
                tmpResonse.ItemId = 0;
                var newOptionList = tmpResonse.Options.Where(r => r.ID > 0 || (r.ID == 0 && r.IsDeleted == false)).ToList();

                foreach (var option in newOptionList)
                {
                    option.ID = 0;
                    option.ResponseId = 0;
                    tmpResonse.Options.Add(option);
                }

                entity.Responses.Add(tmpResonse);
            }
        }


        public override void UpdateEntity(TxkeaExpressiveItemEntity entity)
        {
            switch (this.Step)
            {
                case 1:
                    base.UpdateEntity(entity);

                    if (entity.Images != this.Images)
                        entity.IsCompleted = false;

                    if (entity.IsCompleted == false)
                        entity.Status = Core.Common.Enums.EntityStatus.Inactive;
                    else
                        entity.Status = this.Status;

                    if (this.BackgroundImage.Trim() != string.Empty)
                    {
                        entity.BackgroundFill = this.BackgroundImage;
                        entity.BackgroundFillType = Core.Ade.Enums.BackgroundFillType.Image;
                    }
                    else
                    {
                        entity.BackgroundFill = this.BackgroundFill;
                        entity.BackgroundFillType = Core.Ade.Enums.BackgroundFillType.Color;
                    }

                    entity.InstructionText = this.InstructionText;
                    entity.InstructionAudio = this.InstructionAudio;
                    entity.InstructionAudioTimeDelay = this.InstructionAudioTimeDelay;
                    entity.Images = this.Images;
                    var tmpImageList = entity.ImageList.Where(r => r.IsDeleted == false).OrderBy(r => r.ID).ToList();
                    if (entity.Images != tmpImageList.Count)
                    {
                        entity.LayoutId = 0;
                    }

                    if (entity.Images < tmpImageList.Count())
                    {
                        for (var i = entity.Images; i < tmpImageList.Count; i++)
                        {
                            tmpImageList[i].IsDeleted = true;
                        }
                    }
                    entity.ItemLayout = this.ItemLayout;
                    entity.CpallsItemLayout = this.CpallsItemLayout;

                    if (this.ResponseBackgroundImage.Trim() != string.Empty)
                    {
                        entity.ResponseBackgroundFill = this.ResponseBackgroundImage;
                        entity.ResponseBackgroundFillType = Core.Ade.Enums.BackgroundFillType.Image;
                    }
                    else
                    {
                        entity.ResponseBackgroundFill = this.ResponseBackgroundFill;
                        entity.ResponseBackgroundFillType = Core.Ade.Enums.BackgroundFillType.Color;
                    }

                    entity.ScreenWidth = this.ScreenWidth;
                    entity.ScreenHeight = this.ScreenHeight;
                    entity.Timeoutvalue = this.Timeoutvalue;
                    entity.ResponseType = this.ResponseType;

                    if (this.IsPractice)
                        entity.Score = 0;
                    else if (entity.ResponseType == TxkeaExpressiveResponoseType.Simple)
                        entity.Score = 1;
                     
                    break;
                case 2:
                    var imageList = this.ImageList.Select(ModelToEntity).ToList();
                    var existedEntities = entity.ImageList.ToList();
                    foreach (TxkeaExpressiveImageEntity imageEntity in imageList)
                    {
                        var existed = existedEntities.SingleOrDefault(r => r.ID == imageEntity.ID);
                        if (existed != null)
                        {
                            existed.TargetImage = imageEntity.TargetImage;
                            existed.ImageTimeDelay = imageEntity.ImageTimeDelay;
                            existed.TargetAudio = imageEntity.TargetAudio;
                            existed.AudioTimeDelay = imageEntity.AudioTimeDelay;
                            existed.SameasImageDelay = imageEntity.SameasImageDelay;
                            existed.IsDeleted = imageEntity.IsDeleted;
                        }
                        else
                            entity.ImageList.Add(imageEntity);
                    }
                    break;

                case 3: //
                    entity.ScreenWidth = this.ScreenWidth;
                    entity.ScreenHeight = this.ScreenHeight;
                    entity.ItemLayout = this.ItemLayout;
                    entity.CpallsItemLayout = this.CpallsItemLayout;
                    entity.LayoutId = this.LayoutId;
                    //resposne 为简单类型时，没有第四步
                    if (this.ResponseType == TxkeaExpressiveResponoseType.Simple)
                    {
                        entity.IsCompleted = true;
                        entity.Status = this.Status;
                    }
                    break;
                case 4:
                    if (this.ResponseBackgroundImage.Trim() != string.Empty)
                    {
                        entity.ResponseBackgroundFill = this.ResponseBackgroundImage;
                        entity.ResponseBackgroundFillType = Core.Ade.Enums.BackgroundFillType.Image;
                    }
                    else
                    {
                        entity.ResponseBackgroundFill = this.ResponseBackgroundFill;
                        entity.ResponseBackgroundFillType = Core.Ade.Enums.BackgroundFillType.Color;
                    }

                    entity.IsCompleted = true;
                    entity.Status = this.Status;

                    var responses = this.Responses.Select(ResponseModelToEntity).ToList();
                    var existedResponseList = entity.Responses.Where(r => r.IsDeleted == false).ToList();
                    foreach (TxkeaExpressiveResponseEntity tmpResonse in responses)
                    {
                        var existedResponse = existedResponseList.SingleOrDefault(r => r.ID == tmpResonse.ID);
                        if (existedResponse != null)
                        {
                            existedResponse.Text = tmpResonse.Text;
                            existedResponse.Mandatory = tmpResonse.Mandatory;
                            existedResponse.Type = tmpResonse.Type;
                            existedResponse.Buttons = tmpResonse.Buttons;
                            existedResponse.IsDeleted = tmpResonse.IsDeleted;

                            var newOptionList = tmpResonse.Options.Where(r => r.ID > 0 || (r.ID == 0 && r.IsDeleted == false)).ToList();
                            var existedOptionList = existedResponse.Options;

                            newOptionList.ForEach(option =>
                            {
                                var existedOption = existedOptionList.FirstOrDefault(x => x.ID == option.ID && x.ID>0);
                                if (existedOption != null)
                                {
                                    existedOption.IsCorrect = option.IsCorrect;
                                    existedOption.Lable = option.Lable;
                                    existedOption.AddTextbox = option.AddTextbox;
                                    existedOption.Score = option.Score;
                                    existedOption.IsDeleted = option.IsDeleted;
                                }
                                else
                                {
                                    existedResponse.Options.Add(option);
                                }
                            });
                        }
                        else
                            entity.Responses.Add(tmpResonse);
                    }

                    if (entity.IsPractice)
                        entity.Score = 0;
                    else
                    {
                        if (entity.ResponseType == TxkeaExpressiveResponoseType.Simple)
                            entity.Score = 1;
                        else
                        {
                            if (this.Responses != null && this.Responses.Count() > 0)
                            {
                                switch (this.Responses[0].Type)
                                {
                                    case TypedResponseType.Text:
                                        entity.Score = 0;
                                        break;
                                    case TypedResponseType.Radionbox:
                                        entity.Score = this.Responses[0].Options.Where(r => r.IsDeleted == false).Sum(r => r.Score);
                                        break;
                                    case TypedResponseType.Checkbox:
                                        entity.Score = this.Responses[0].Options.Where(r => r.IsDeleted == false).Sum(r => r.Score);
                                        break;
                                }
                            }
                            else
                                entity.Score = 0;
                        }
                    }
                    break;
            }
        }

        public List<TxkeaExpressiveImageModel> ImageList { get; set; }

        List<AnswerEntity> ToAnserEntity(List<TxkeaExpressiveImageModel> list)
        {
            List<AnswerEntity> newList = new List<AnswerEntity>();
            foreach (TxkeaExpressiveImageModel image in list)
            {
                AnswerEntity entity = new AnswerEntity();
                entity.ID = image.ID;
                entity.Picture = image.TargetImage;
                entity.PictureTime = image.ImageTimeDelay;
                entity.Audio = image.TargetAudio;
                entity.AudioTime = image.AudioTimeDelay;
                newList.Add(entity);
            }
            return newList;
        }

        public List<TxkeaExpressiveResponseModel> Responses { get; set; }

        public override ExecCpallsItemModel ExecCpallsItemModel
        {
            get
            {
                if (Responses != null)
                    base.ExecCpallsItemModel.Props.Add("Responses", Responses.Where(r => r.IsDeleted == false));
                base.ExecCpallsItemModel.Props.Add("CpallsItemLayout", CpallsItemLayout);
                base.ExecCpallsItemModel.Props.Add("ScreenWidth", ScreenWidth);
                base.ExecCpallsItemModel.Props.Add("ScreenHeight", ScreenHeight);
                base.ExecCpallsItemModel.Props.Add("InstructionText", InstructionText);
                base.ExecCpallsItemModel.Props.Add("InstructionAudio", InstructionAudio);
                base.ExecCpallsItemModel.Props.Add("InstructionAudioTimeDelay", InstructionAudioTimeDelay);
                base.ExecCpallsItemModel.Props.Add("OverallTimeOut", Timed);
                base.ExecCpallsItemModel.Props.Add("TimeoutValue", Timeoutvalue);
                base.ExecCpallsItemModel.Props.Add("ResponseBackground", this.ResponseBackgroundFill);
                base.ExecCpallsItemModel.Props.Add("ResponseBackgroundImage", this.ResponseBackgroundImage);
                base.ExecCpallsItemModel.Props.Add("ResponseType", this.ResponseType);
                base.ExecCpallsItemModel.Props.Add("BackgroundImage", this.BackgroundImage);
                base.ExecCpallsItemModel.Props.Add("LayoutBackgroundImage", this.LayoutBackgroundImage);
                if (BranchingScores != null)
                    base.ExecCpallsItemModel.Props.Add("BranchingScores", this.BranchingScores);
                if (ImageList != null)
                    base.ExecCpallsItemModel.Props.Add("ImageList", ToAnserEntity(ImageList));
                return base.ExecCpallsItemModel;
            }
        }

        public override void UpdateModel(TxkeaExpressiveItemEntity entity)
        {
            base.UpdateModel(entity);
            // Layout 背景色或图片 begin
            this.BackgroundFill = string.Empty;
            this.BackgroundImage = string.Empty;
            if (entity.BackgroundFillType == Core.Ade.Enums.BackgroundFillType.Color)
                this.BackgroundFill = entity.BackgroundFill;
            else
                this.BackgroundImage = entity.BackgroundFill;

            this.BackgroundFillType = entity.BackgroundFillType;
            // Layout 背景色或图片 end

            // Layout 模板 背景色或图片 begin
            this.LayoutBackgroundFill = string.Empty;
            this.LayoutBackgroundImage = string.Empty;
            this.LayoutBackgroundFillType = Core.Ade.Enums.BackgroundFillType.Color;
            if (entity.Layout != null)
            {
                if (entity.Layout.BackgroundFillType == Core.Ade.Enums.BackgroundFillType.Color)
                    this.LayoutBackgroundFill = entity.Layout.BackgroundFill;
                else
                    this.LayoutBackgroundImage = entity.Layout.BackgroundFill;

                this.LayoutBackgroundFillType = entity.Layout.BackgroundFillType;
            }
            // Layout 模板 背景色或图片 end

            // Response 背景色或图片 begin
            this.ResponseBackgroundFill = string.Empty;
            this.ResponseBackgroundImage = string.Empty;
            if (entity.ResponseBackgroundFillType == Core.Ade.Enums.BackgroundFillType.Color)
                this.ResponseBackgroundFill = entity.ResponseBackgroundFill;
            else
                this.ResponseBackgroundImage = entity.ResponseBackgroundFill;

            this.ResponseBackgroundFillType = entity.ResponseBackgroundFillType;
            // Response 背景色或图片 end

            this.InstructionText = entity.InstructionText;
            this.InstructionAudio = entity.InstructionAudio;
            this.InstructionAudioTimeDelay = entity.InstructionAudioTimeDelay;
            this.Images = entity.Images;
            this.ItemLayout = entity.ItemLayout;
            this.CpallsItemLayout = entity.CpallsItemLayout;
            this.ScreenWidth = entity.ScreenWidth;
            this.ScreenHeight = entity.ScreenHeight;
            this.IsCompleted = entity.IsCompleted;
            this.LayoutId = entity.LayoutId;
            this.Timeoutvalue = entity.Timeoutvalue;
            this.ResponseType = entity.ResponseType;

            this.ImageList = entity.ImageList.Where(r => r.IsDeleted == false).Select(EntityToModelSelector).ToList();
            if (this.ImageList.Count < Images)
            {
                while (this.ImageList.Count < Images)
                    this.ImageList.Add(new TxkeaExpressiveImageModel());
            }
            else if (this.ImageList.Count > Images)
            {
                for (int i = Images; i < this.ImageList.Count; i++)
                {
                    this.ImageList[i].IsDeleted = true;
                }
            }


            this.Responses = entity.Responses.Select(ResponseEntityToModelSelector).ToList();
            if (this.Responses.Count == 0)
                this.Responses.Add(new TxkeaExpressiveResponseModel(false));

            this.BranchingScores = entity.BranchingScores.Where(r => r.IsDeleted == false)
                .Select(BranchingScoresEntityToModelSelector).ToList();
        }
    }
}
