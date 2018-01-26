using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
/**************************************************************************
 * Developer: 		JackZhang
 * Computer:		JACKZ
 * Domain:			JackZ
 * CreatedOn:		6/9/2015 16:06:15
 * Description:		Please input class summary
 * Version History:	Created,6/9/2015 16:06:15
 *
 *
 **************************************************************************/
using Sunnet.Cli.Business.Cpalls.Models;
using Sunnet.Cli.Core.Ade;
using Sunnet.Cli.Core.Ade.Entities;
using Sunnet.Framework.Mvc;
using Extensions = LinqKit.Extensions;
using Newtonsoft.Json;


namespace Sunnet.Cli.Business.Ade.Models
{
    public class TypedResponseItemModel : ItemModelBase<TypedResponseItemEntity>
    {
        public override ItemType Type
        {
            get
            {
                return ItemType.TypedResponse;
            }
            set { }
        }

        public override AnswerType AnswerType
        {
            get
            {
                return AnswerType.TypedResponse;
            }
            set { }
        }

        [DisplayName("Target Text")]
        [StringLength(1000)]
        [EensureEmptyIfNull]
        public string TargetText { get; set; }

        [DisplayName("Target Text Time in")]
        public int TargetTextTimeout { get; set; }

        [DisplayName("Target Audio")]
        [StringLength(100)]
        [EensureEmptyIfNull]
        public string TargetAudio { get; set; }

        [DisplayName("Target Audio Time in")]
        public int TargetAudioTimeout { get; set; }

        public int? Timeout { get; set; }
        public TypedResponseItemModel()
            : base()
        {
        }

        public TypedResponseItemModel(TypedResponseItemEntity entity)
            : base(entity)
        {
            this.Timeout = entity.Timeout;
            this.TargetText = entity.TargetText;
            this.TargetTextTimeout = entity.TargetTextTimeout;
            this.TargetAudio = entity.TargetAudio;
            this.TargetAudioTimeout = entity.TargetAudioTimeout;

            this.Responses = entity.Responses.Where(x => x.IsDeleted == false).Select(EntityToModelSelector);
        }

        private static Func<TypedResponseEntity, TypedResopnseModel> EntityToModelSelector
        {
            get
            {
                return x => new TypedResopnseModel()
                {
                    Id = x.ID,
                    ItemId = x.ItemId,
                    Length = x.Length,
                    Picture = x.Picture,
                    PictureTimeIn = x.PictureTimeIn,
                    Required = x.Required,
                    Text = x.Text,
                    TextTimeIn = x.TextTimeIn,
                    Type = x.Type,
                    IsDeleted = x.IsDeleted,
                    Options = x.Options.Where(o => o.IsDeleted == false).Select(r => new TypedResponseOptionModel()
                    {
                        Id = r.ID,
                        From = r.From,
                        Keyword = r.Keyword,
                        ResponseId = r.ResponseId,
                        Score = r.Score,
                        To = r.To,
                        Type = x.Type,
                        IsDeleted = x.IsDeleted,
                    })
                };
            }
        }

        private static Func<TypedResopnseModel, TypedResponseEntity> ModelToEntity
        {
            get
            {
                return x => new TypedResponseEntity()
                {
                    ID = x.Id,
                    ItemId = x.ItemId,
                    Length = x.Length,
                    Picture = x.Picture,
                    PictureTimeIn = x.PictureTimeIn,
                    Required = x.Required,
                    Text = x.Text,
                    TextTimeIn = x.TextTimeIn,
                    Type = x.Type,
                    IsDeleted = x.IsDeleted,
                    Options = x.Options.Select(r => new TypedResponseOptionEntity()
                    {
                        ID = r.Id,
                        From = r.From,
                        Keyword = r.Keyword,
                        ResponseId = r.ResponseId,
                        Score = r.Score,
                        To = r.To,
                        Type = r.Type,
                        IsDeleted = r.IsDeleted,
                    }).ToList()
                };
            }
        }

        public override void UpdateEntity(TypedResponseItemEntity entity)
        {
            base.UpdateEntity(entity);

            entity.Timeout = this.Timeout ?? 0;
            entity.TargetText = this.TargetText;
            entity.TargetTextTimeout = this.TargetTextTimeout;
            entity.TargetAudio = this.TargetAudio;
            entity.TargetAudioTimeout = this.TargetAudioTimeout;

            var newResponses = this.Responses.Select(ModelToEntity).ToList();
            var existedEntities = entity.Responses.ToList();
            newResponses.ForEach(response =>
            {
                var existed = existedEntities.FirstOrDefault(x => x.ID == response.ID);
                if (existed != null)
                {
                    existed.Length = response.Length;
                    existed.Picture = response.Picture;
                    existed.PictureTimeIn = response.PictureTimeIn;
                    existed.Required = response.Required;
                    existed.Text = response.Text;
                    existed.TextTimeIn = response.TextTimeIn;
                    existed.Type = response.Type;
                    existed.IsDeleted = response.IsDeleted;

                    var newOptions = response.Options.ToList();
                    var existedOptions = existed.Options.ToList();

                    newOptions.ForEach(option =>
                    {
                        var existedOption = existedOptions.FirstOrDefault(x => x.ID == option.ID);
                        if (existedOption != null)
                        {
                            existedOption.From = option.From;
                            existedOption.Keyword = option.Keyword;
                            existedOption.Score = option.Score;
                            existedOption.To = option.To;
                            existedOption.Type = option.Type;
                            existedOption.IsDeleted = option.IsDeleted;
                        }
                        else
                        {
                            existed.Options.Add(option);
                        }
                    });
                }
                else
                {
                    entity.Responses.Add(response);
                }
            });
            if (this.Responses != null && !this.IsPractice)
                entity.Score = this.Responses.Sum(x => x.Score);
            else
                entity.Score = 0;
        }

        public IEnumerable<TypedResopnseModel> Responses { get; set; }

        public override ExecCpallsItemModel ExecCpallsItemModel
        {
            get
            {
                base.ExecCpallsItemModel.Props.Add("TargetText", TargetText);
                base.ExecCpallsItemModel.Props.Add("TargetTextTimeout", TargetTextTimeout);
                base.ExecCpallsItemModel.Props.Add("TargetAudio", TargetAudio);
                base.ExecCpallsItemModel.Props.Add("TargetAudioTimeout", TargetAudioTimeout);
                base.ExecCpallsItemModel.Props.Add("Timeout", Timeout ?? 0);
                base.ExecCpallsItemModel.Props.Add("Responses", Responses);
                
                return base.ExecCpallsItemModel;
            }
        }
    }
}
