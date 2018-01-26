using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/**************************************************************************
 * Developer: 		JackZhang
 * Computer:		JACKZ
 * Domain:			JackZ
 * CreatedOn:		1/12 2015 16:47:17
 * Description:		Please input class summary
 * Version History:	Created,1/12 2015 16:47:17
 * 
 * 
 **************************************************************************/
using Sunnet.Cli.Core.Trs;
using Sunnet.Cli.Core.Trs.Entities;
using Sunnet.Cli.Core.Trs.Enums;

namespace Sunnet.Cli.Business.Trs.Models
{
    public class TrsItemModel : TrsItemBaseModel
    {
        public string Description { get; set; }

        public string KeyBehavior { get; set; }

        /// <summary>
        /// 显示条件类型，已作废
        /// </summary>
        public TRSFilterEnum Filter { get; set; }

        public bool IsRequired { get; set; }

        public bool ShowByInfants { get; set; }

        public bool ShowByToddlers { get; set; }

        public bool ShowByPreschool { get; set; }

        public bool ShowBySchoolAge { get; set; }

        public TRSFacilityEnum LCSA { get; set; }

        public TRSFacilityEnum LCCH { get; set; }

        public TRSFacilityEnum RCCH { get; set; }

        public TRSFacilityEnum LCAA { get; set; }

        public int SubCategorySort { get; set; }

        /// <summary>
        /// 答案如何同步
        /// </summary>
        public SyncAnswerType SyncAnswer { get; set; }

        public int Sort { get; set; }

        public IEnumerable<TrsAnswerModel> Answers { get; set; }

        /// <summary>
        /// 是否已做过题目
        /// </summary>
        public bool Filled { get; set; }

        public TRSItemType Type { get; set; }

        public string Comments { get; set; }

        public bool IfCanAccess { get; set; }

        public int AgeGroup { get; set; }

        public int GroupSize { get; set; }

        public int CaregiversNo { get; set; } 
    }
}
