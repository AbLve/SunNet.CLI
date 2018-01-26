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
 * CreatedOn:		2014/8/28 20:22:59
 * Description:		Please input class summary
 * Version History:	Created,2014/8/28 20:22:59
 * 
 * 
 **************************************************************************/
using Sunnet.Cli.Business.Cpalls.Models;
using Sunnet.Cli.Core.Ade;
using Sunnet.Cli.Core.Ade.Entities;
using Sunnet.Framework.Mvc;

namespace Sunnet.Cli.Business.Ade.Models
{
    public class ObservableChoiceModel : ItemModelBase<ObservableChoiceEntity>, IMultiChoiceProperty
    {

        public override ItemType Type
        {
            get
            {
                return ItemType.ObservableChoice;
            }
            set { }
        }

        public override AnswerType AnswerType
        {
            get
            {
                return AnswerType.YesNo;
            }
            set { }
        }
        [DisplayName("Target Text")]
        [StringLength(1000)]
        [EensureEmptyIfNull]
        public string TargetText { get; set; }

        [DisplayName("Choice type")]
        [Required(ErrorMessage = "Choose one, please.")]
        public bool IsMultiChoice { get; set; }

        public bool IsShown { get; set; }

        [DisplayName("Required")]
        public bool IsRequired { get; set; }

        public override void UpdateEntity(ObservableChoiceEntity entity)
        {
            base.UpdateEntity(entity);
            entity.TargetText = this.TargetText;
            entity.IsMultiChoice = this.IsMultiChoice;
            entity.IsShown = this.IsShown;
            entity.IsRequired = this.IsRequired;
        }
        public ObservableChoiceModel()
            : base()
        {

        }
        public ObservableChoiceModel(ObservableChoiceEntity entity)
            : base(entity)
        {
            this.TargetText = entity.TargetText;
            this.IsMultiChoice = entity.IsMultiChoice;
            this.IsShown = entity.IsShown;
            this.IsRequired = entity.IsRequired;
        }
     
    }
}
