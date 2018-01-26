using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/**************************************************************************
 * Developer: 		JackZhang
 * Computer:		JACKZ
 * Domain:			Jackz
 * CreatedOn:		2014/8/28 20:22:25
 * Description:		Please input class summary
 * Version History:	Created,2014/8/28 20:22:25
 * 
 * 
 **************************************************************************/
using Sunnet.Cli.Business.Cpalls.Models;
using Sunnet.Cli.Core.Ade;
using Sunnet.Cli.Core.Ade.Entities;
using Sunnet.Framework.Mvc;

namespace Sunnet.Cli.Business.Ade.Models
{
    public class DirectionItemModel : ItemModelBase<DirectionItemEntity>
    {
        public override ItemType Type
        {
            get
            {
                return ItemType.Direction;
            }
            set { }
        }

        public override AnswerType AnswerType
        {
            get
            {
                return AnswerType.None;
            }
            set { }
        }

        [EensureEmptyIfNull]
        [DisplayName("Direction Text")]
        public string DirectionText { get; set; }

        public DirectionItemModel()
            : base()
        {

        }

        public DirectionItemModel(DirectionItemEntity entity)
            : base(entity)
        {
            this.DirectionText = entity.FullDescription;
        }

        public override void UpdateEntity(DirectionItemEntity entity)
        {
            base.UpdateEntity(entity);

            entity.FullDescription = this.DirectionText;
            entity.Scored = false;
            entity.Timed = false;
        }

        public override ExecCpallsItemModel ExecCpallsItemModel
        {
            get
            {
                base.ExecCpallsItemModel.Props.Add("DirectionText", DirectionText);

                return base.ExecCpallsItemModel;
            }
        }
    }
}
