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
    public class ObservableEntryModel : ItemModelBase<ObservableEntryEntity> 
    {

        public override ItemType Type
        {
            get
            {
                return ItemType.ObservableResponse;
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
        public bool IsShown { get; set; }
        public IEnumerable<TypedResopnseModel> Responses { get; set; }

        public override void UpdateEntity(ObservableEntryEntity entity)
        {
            base.UpdateEntity(entity);
            entity.TargetText = this.TargetText;
            entity.IsShown = this.IsShown;
        }
        public ObservableEntryModel()
            : base()
        {
        }
        public ObservableEntryModel(ObservableEntryEntity entity)
            : base(entity)
        {
            this.TargetText = entity.TargetText;
            this.IsShown = entity.IsShown;
        }
     
    }
}
