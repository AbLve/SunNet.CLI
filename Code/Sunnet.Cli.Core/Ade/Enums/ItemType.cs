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
 * CreatedOn:		2014/8/12 3:34:10
 * Description:		Please input class summary
 * Version History:	Created,2014/8/12 3:34:10
 * 
 * 
 **************************************************************************/
namespace Sunnet.Cli.Core.Ade
{
    // 更新时注意更新 
    // Sunnet.Cli.Static\Content\Scripts_cliProject\Module_AssPub.js
    // Sunnet.Cli.Static\Content\Scripts_cliProject\Module_Ade.js
    // AdeService.cs
    public enum ItemType : byte
    {
        Direction = 9,
        [Description("CEC")]
        Cec = 1,
        [Description("COT")]
        Cot = 2,
        [Description("Multiple Choice")]
        MultipleChoices = 3,
        [Description("Phonological Awareness")]
        Pa = 4,
        Quadrant = 5,
        [Description("Rapid Expressive")]
        RapidExpressive = 6,
        [Description("Receptive without prompt")]
        Receptive = 7,
        [Description("Receptive with prompt")]
        ReceptivePrompt = 8,
        Checklist = 10,
        [Description("Typed Response")]
        TypedResponse = 11,
        [Description("TX-KEA Receptive")]
        TxkeaReceptive = 12,
        [Description("TX-KEA Expressive")]
        TxkeaExpressive = 13,
        [Description("Multiple/Single Choice")]
        ObservableChoice = 14,
        [Description("Text Entry")]
        ObservableResponse = 15

    }

    public static class ItemTypeHelper
    {
        public static Dictionary<AssessmentType, List<ItemType>> Types
        {
            get
            {
                var list1 = new List<ItemType>()
                {
                    ItemType.Direction,
                    ItemType.MultipleChoices,
                    ItemType.Pa,
                    ItemType.Quadrant,
                    ItemType.RapidExpressive,
                    ItemType.Receptive,
                    ItemType.ReceptivePrompt,
                    ItemType.Checklist,
                    ItemType.TypedResponse,
                    ItemType.TxkeaReceptive,
                    ItemType.TxkeaExpressive
                };
                var dics = new Dictionary<AssessmentType, List<ItemType>>();
                dics.Add(AssessmentType.Cpalls, list1);
                dics.Add(AssessmentType.Cec, new List<ItemType>() { ItemType.Cec });
                dics.Add(AssessmentType.Cot, new List<ItemType>() { ItemType.Cot });

                dics.Add(AssessmentType.UpdateObservables,new List<ItemType>()
                {
                    ItemType.ObservableChoice,ItemType.ObservableResponse
                });
                return dics;
            }
        }

        public static List<ItemType> GetCpallsTypes(ItemShowType type)
        {
            var list = Types[AssessmentType.Cpalls];
            if (type == ItemShowType.Sequenced)
            {
                return list.Where(x => x != ItemType.Checklist).ToList();
            }
            return list.Where(x => x == ItemType.Checklist).ToList();
        }
    }

}
