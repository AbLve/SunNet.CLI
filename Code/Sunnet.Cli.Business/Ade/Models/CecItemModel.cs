using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/**************************************************************************
 * Developer: 		JackZhang
 * Computer:		JACKZ
 * Domain:			Jackz
 * CreatedOn:		2014/8/28 20:19:15
 * Description:		Please input class summary
 * Version History:	Created,2014/8/28 20:19:15
 * 
 * 
 **************************************************************************/
using Sunnet.Cli.Business.Cpalls.Models;
using Sunnet.Cli.Core.Ade;
using Sunnet.Cli.Core.Ade.Entities;

namespace Sunnet.Cli.Business.Ade.Models
{
    /// <summary>
    /// 因为有一个CEC的Model(本来不应在当前命名空间), 因此暂时命名为Base, 另一个类移走之后重命名.
    /// </summary>
    /// Author : JackZhang
    /// Date   : 6/9/2015 15:00:08
    public class CecItemModel : ItemModelBase<CecItemEntity>, IMultiChoiceProperty
    {
        public override ItemType Type
        {
            get
            {
                return ItemType.Cec;
            }
            set { }
        }

        public override AnswerType AnswerType
        {
            get
            {
                return AnswerType.Cec;
            }
            set { }
        }

        public CecItemModel()
            : base()
        {
        }

        public CecItemModel(CecItemEntity entity)
            : base(entity)
        {

            this.Description = entity.Description;
            this.TargetText = entity.TargetText;
            this.IsMultiChoice = entity.IsMultiChoice;
            this.Direction = entity.Direction;
            this.ResponseCount = entity.ResponseCount;
            this.IsRequired = entity.IsRequired;
        }

        [StringLength(1000)]
        [Required]
        [DisplayName("Target Text")]
        public string TargetText { get; set; }

        [DisplayName("Choice type")]
        [Required(ErrorMessage = "Choose one, please.")]
        public bool IsMultiChoice { get; set; }

        /// <summary>
        /// CEC | Checklist.
        /// </summary> 
        [DisplayName("Response #")]
        public int ResponseCount { get; set; }

        [Required(ErrorMessage = "Choose one, please.")]
        public CecItemsDirection Direction { get; set; }

        [DisplayName("Required")]
        public bool IsRequired { get; set; }

        public override void UpdateEntity(CecItemEntity entity)
        {
            base.UpdateEntity(entity);

            entity.TargetText = this.TargetText;
            entity.IsMultiChoice = this.IsMultiChoice;
            entity.Direction = this.Direction;
            entity.ResponseCount = this.ResponseCount;
            entity.Scored = true;
            entity.Timed = false;
            entity.IsRequired = this.IsRequired;
            if (this.Answers != null)
                entity.Score = this.IsMultiChoice ?
                    this.Answers.Sum(a => a.Score) :
                    this.Answers.Max(a => a.Score);
        }

        public override ExecCpallsItemModel ExecCpallsItemModel
        {
            get
            {
                return null;
                //throw new NotImplementedException("CEC Item does not implement this property");
            }
        }
    }
}
