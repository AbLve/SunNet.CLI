using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/**************************************************************************
 * Developer: 		JackZhang
 * Computer:		JACKZ
 * Domain:			JackZ
 * CreatedOn:		6/9/2015 10:52:25
 * Description:		Please input class summary
 * Version History:	Created,6/9/2015 10:52:25
 *
 *
 **************************************************************************/
using Sunnet.Cli.Core.Ade;
using Sunnet.Cli.Core.Ade.Entities;
using Sunnet.Cli.Core.Ade.Entities;

namespace Sunnet.Cli.Business.Ade.Models
{
    public sealed class ItemFactory
    {
        public static ItemModel GetItemModel(ItemType type)
        {
            ItemModel model;
            switch (type)
            {
                case ItemType.Cec:
                    model = new CecItemModel();
                    break;
                case ItemType.Checklist:
                    model = new ChecklistItemModel();
                    break;
                case ItemType.Cot:
                    model = new CotItemModel();
                    break;
                case ItemType.Direction:
                    model = new DirectionItemModel();
                    break;
                case ItemType.MultipleChoices:
                    model = new MultipleItemModel();
                    break;
                case ItemType.Pa:
                    model = new PaItemModel();
                    break;
                case ItemType.Quadrant:
                    model = new QuadrantItemModel();
                    break;
                case ItemType.RapidExpressive:
                    model = new RapidExpressiveItemModel();
                    break;
                case ItemType.Receptive:
                    model = new ReceptiveItemModel();
                    break;
                case ItemType.ReceptivePrompt:
                    model = new ReceptivePromptItemModel();
                    break;
                case ItemType.TypedResponse:
                    model = new TypedResponseItemModel();
                    break;
                case ItemType.ObservableChoice:
                    model = new ObservableChoiceModel();
                    break;
                case ItemType.ObservableResponse:
                    model = new ObservableEntryModel();
                    break;
                case ItemType.TxkeaReceptive:
                    model = new TxkeaReceptiveItemModel();
                    break;
                case ItemType.TxkeaExpressive:
                    model = new TxkeaExpressiveItemModel();
                    break;
                default:
                    return null;
            }
            model.Type = type;
            return model;
        }

        public static ItemModel GetItemModel(ItemBaseEntity entity)
        {
            try
            {
            switch (entity.Type)
            {
                case ItemType.Cec:
                    return new CecItemModel((CecItemEntity)entity);
                case ItemType.Checklist:
                    return new ChecklistItemModel((ChecklistItemEntity)entity);
                case ItemType.Cot:
                    return new CotItemModel((CotItemEntity)entity);
                case ItemType.Direction:
                    return new DirectionItemModel((DirectionItemEntity)entity);
                case ItemType.MultipleChoices:
                    return new MultipleItemModel((MultipleChoicesItemEntity)entity);
                case ItemType.Pa:
                    return new PaItemModel((PaItemEntity)entity);
                case ItemType.Quadrant:
                    return new QuadrantItemModel((QuadrantItemEntity)entity);
                case ItemType.RapidExpressive:
                    return new RapidExpressiveItemModel((RapidExpressiveItemEntity)entity);
                case ItemType.Receptive:
                    return new ReceptiveItemModel((ReceptiveItemEntity)entity);
                case ItemType.ReceptivePrompt:
                    return new ReceptivePromptItemModel((ReceptivePromptItemEntity)entity);
                case ItemType.TypedResponse:
                    return new TypedResponseItemModel((TypedResponseItemEntity)entity);

                    case ItemType.ObservableChoice:
                        return new ObservableChoiceModel((ObservableChoiceEntity)entity);
                    case ItemType.ObservableResponse:
                        return new ObservableEntryModel((ObservableEntryEntity)entity);
                case ItemType.TxkeaReceptive:
                    return new TxkeaReceptiveItemModel((TxkeaReceptiveItemEntity)entity);
                case ItemType.TxkeaExpressive:
                    return new TxkeaExpressiveItemModel((TxkeaExpressiveItemEntity)entity);
                default:
                    return null;
            }
            }
            catch (Exception exception)
            {
                
                throw;
            }
          
        }

    }
}
