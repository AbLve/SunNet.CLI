using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sunnet.Cli.Core.Trs.Enums;
using Sunnet.Cli.Core.Trs;
/**************************************************************************
 * Developer: 		Joe
 * Computer:		Joe
 * Domain:			Joe
 * CreatedOn:		1/12 2015 16:47:17
 * Description:		Please input class summary
 * Version History:	Created,1/12 2015 16:47:17
 * 
 * 
 **************************************************************************/

namespace Sunnet.Cli.Business.Trs.Models
{
    public class TrsItemReportModel : TrsItemBaseModel
    {
        public TRSItemType ItemType { get; set; }

        public TRSItemType SubcategoryType { get; set; }

        public string Description { get; set; }

        public int SubCategorySort { get; set; }

        public int Sort { get; set; }

        public TRSItemType Type { get; set; }

        public List<TrsClassItemModel> ClassItems { get; set; }

        public TRSFacilityEnum LCSA { get; set; }

        public TRSFacilityEnum LCCH { get; set; }

        public TRSFacilityEnum RCCH { get; set; }

        public TRSFacilityEnum LCAA { get; set; }

        public bool ShowByInfants { get; set; }

        public bool ShowByToddlers { get; set; }

        public bool ShowByPreschool { get; set; }

        public bool ShowBySchoolAge { get; set; }

        /// <summary>
        /// 答案如何同步
        /// </summary>
        public SyncAnswerType SyncAnswer { get; set; }

        public string Description_Format
        {
            get
            {
                if (this.Category == TRSCategoryEnum.Category2)
                {
                    return Description + "<br />" + Text;
                }
                else
                {
                    return Description;
                }
            }
        }

        public string Comments { get; set; }

        public ItemAgeGroup AgeGroup { get; set; }

        public int GroupSize { get; set; }

        public int CaregiversNo { get; set; } 
    }
}
