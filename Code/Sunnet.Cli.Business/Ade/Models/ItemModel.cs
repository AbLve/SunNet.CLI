/**************************************************************************
 * Developer: 		JackZhang
 * Computer:		JACKZ
 * Domain:			Jackz
 * CreatedOn:		2014/8/28 5:48:48
 * Description:		Please input class summary
 * Version History:	Created,2014/8/28 5:48:48
 * 
 * 
 **************************************************************************/

using System.Linq;
using Sunnet.Cli.Business.Common;
using Sunnet.Cli.Business.Cpalls.Models;
using Sunnet.Cli.Core.Ade;
using Sunnet.Cli.Core.Ade.Entities;
using Sunnet.Cli.Core.Common.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Sunnet.Cli.Core.Cpalls;
using Sunnet.Framework.Mvc;

namespace Sunnet.Cli.Business.Ade.Models
{
    public abstract class ItemModel : Core.Ade.Models.ItemModel
    {
        private string _createdByName;

        private List<AnswerEntity> _answers;

        public MeasureModel Measure { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this item is locked by assessment.
        /// </summary>
        public bool Locked
        {
            get { return this.Measure != null && this.Measure.Locked; }
        }

        [DisplayName("Created By")]
        public string CreatedByName
        {
            get { return _createdByName ?? (_createdByName = ""); }
            set { _createdByName = value; }
        }

        public List<AnswerEntity> Answers
        {
            get { return _answers ?? (_answers = new List<AnswerEntity>()); }
            set { _answers = value; }
        }

        private List<BranchingScoreModel> _branchingScores;
        public List<BranchingScoreModel> BranchingScores
        {
            get { return _branchingScores ?? (_branchingScores = new List<BranchingScoreModel>()); }
            set { _branchingScores = value; }
        }

        /// <summary>
        /// 引用资源路径, Ade 编辑Item时使用
        /// </summary>
        public string BasePath { get; set; }


        protected ItemModel()
        {

        }

        protected ItemModel(ItemBaseEntity entity)
            : base(entity)
        {
            Answers = entity.Answers.ToList();
            BranchingScores = entity.BranchingScores.Where(r => r.IsDeleted == false).Select(BranchingScoresEntityToModelSelector).OrderBy(r => r.ID).ToList();
        }



        public abstract ExecCpallsItemModel ExecCpallsItemModel { get; }

        public abstract void UpdateEntity(ItemBaseEntity entity);

        protected static Func<BranchingScoreEntity, BranchingScoreModel> BranchingScoresEntityToModelSelector
        {
            get
            {
                return x => new BranchingScoreModel()
                {
                    ID = x.ID,
                    ItemId = x.ItemId,
                    From = x.From,
                    To = x.To,
                    IsDeleted = x.IsDeleted,
                    SkipItemId = x.SkipItemId
                };
            }
        }

        protected static Func<BranchingScoreModel, BranchingScoreEntity> BranchingScoresModelToEntity
        {
            get
            {
                return x => new BranchingScoreEntity()
                {
                    ID = x.ID,
                    ItemId = x.ItemId,
                    From = x.From,
                    To = x.To,
                    IsDeleted = x.IsDeleted,
                    SkipItemId = x.SkipItemId
                };
            }
        }

    }

    public abstract class ItemModelBase<T> : ItemModel where T : ItemBaseEntity
    {
        protected ItemModelBase()
            : base()
        {
            this.CreatedOn = DateTime.Now;
            this.UpdatedOn = DateTime.Now;
            Status = EntityStatus.Active;
            AnswerType = AnswerType.None;
        }

        protected ItemModelBase(T entity)
            : base(entity)
        {
            this.ID = entity.ID;
            this.AnswerType = entity.AnswerType;
            this.CreatedBy = entity.CreatedBy;
            this.CreatedOn = entity.CreatedOn;
            this.Status = entity.Status;
            this.IsDeleted = entity.IsDeleted;
            this.MeasureId = entity.MeasureId;
            this.Type = entity.Type;
            this.Label = entity.Label;
            this.Score = entity.Score;
            this.Scored = entity.Scored;
            this.Sort = entity.Sort;
            this.Status = entity.Status;
            this.Timed = entity.Timed;
            this.WaitTime = entity.WaitTime;
            this.UpdatedBy = entity.UpdatedBy;
            this.UpdatedOn = entity.UpdatedOn;
            this.RandomAnswer = entity.RandomAnswer;
            this.IsPractice = entity.IsPractice;
            this.ShowAtTestResume = entity.ShowAtTestResume;
            this.Description = entity.Description;
        }

        public override void UpdateEntity(ItemBaseEntity entity)
        {
            var specific = (T)entity;

            UpdateEntity(specific);
        }

        public virtual void UpdateEntity(T entity)
        {
            if (this.ID < 1)
            {
                entity.AnswerType = this.AnswerType;
                entity.CreatedBy = this.CreatedBy;
                entity.CreatedOn = this.CreatedOn;
                entity.Status = EntityStatus.Active;
                entity.IsDeleted = false;
                entity.MeasureId = this.MeasureId;
                entity.Type = this.Type;
            }

            entity.Label = this.Label;
            entity.Score = this.Score;
            entity.Scored = this.Scored;
            entity.Status = this.Status;
            entity.Timed = this.Timed;
            entity.WaitTime = this.WaitTime;
            entity.UpdatedBy = this.UpdatedBy;
            entity.UpdatedOn = this.UpdatedOn;
            entity.RandomAnswer = this.RandomAnswer;
            entity.IsPractice = this.IsPractice;
            entity.ShowAtTestResume = this.ShowAtTestResume;
            entity.Description = string.IsNullOrEmpty(this.Description) ? "" : this.Description;

            List<BranchingScoreEntity> existList = entity.BranchingScores.Where(r => r.IsDeleted == false).ToList();
            List<BranchingScoreEntity> branchingScoreList = this.BranchingScores.Select(BranchingScoresModelToEntity).ToList();
            foreach (BranchingScoreEntity item in branchingScoreList)
            {
                BranchingScoreEntity tmp = existList.Find(r => r.ID == item.ID);
                if (tmp != null)
                {
                    tmp.From = item.From;
                    tmp.To = item.To;
                    tmp.SkipItemId = item.SkipItemId;
                    tmp.IsDeleted = item.IsDeleted;
                }
                else
                    entity.BranchingScores.Add(item);
            }
        }

        public virtual void UpdateModel(T entity)
        {
            this.ID = entity.ID;
            this.AnswerType = entity.AnswerType;
            this.CreatedBy = entity.CreatedBy;
            this.CreatedOn = entity.CreatedOn;
            this.Status = entity.Status;
            this.IsDeleted = entity.IsDeleted;
            this.MeasureId = entity.MeasureId;
            this.Type = entity.Type;
            this.Label = entity.Label;
            this.Score = entity.Score;
            this.Scored = entity.Scored;
            this.Sort = entity.Sort;
            this.Status = entity.Status;
            this.Timed = entity.Timed;
            this.WaitTime = entity.WaitTime;
            this.UpdatedBy = entity.UpdatedBy;
            this.UpdatedOn = entity.UpdatedOn;
            this.RandomAnswer = entity.RandomAnswer;
            this.IsPractice = entity.IsPractice;
            this.ShowAtTestResume = entity.ShowAtTestResume;
            this.Description = entity.Description;
        }

        private ExecCpallsItemModel _execCpallsItemmodel;
        public override ExecCpallsItemModel ExecCpallsItemModel
        {
            get
            {
                return _execCpallsItemmodel ?? (_execCpallsItemmodel = new ExecCpallsItemModel()
                {
                    ItemId = ID,
                    MeasureId = MeasureId,
                    AnswerType = AnswerType,
                    Label = Label,
                    Timed = Timed,
                    WaitTime = WaitTime,
                    Scored = Scored,
                    Score = Score,
                    Type = Type,
                    Answers = Answers,
                    Status = CpallsStatus.Initialised,
                    RandomAnswer = RandomAnswer,
                    IsPractice = IsPractice,
                    ShowAtTestResume = ShowAtTestResume,
                    Description = Description
                });
            }
        }
    }
}
