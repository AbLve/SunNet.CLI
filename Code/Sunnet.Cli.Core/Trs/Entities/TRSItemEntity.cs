using Sunnet.Cli.Core.Trs.Enums;
using Sunnet.Framework.Core.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sunnet.Cli.Core.Trs.Entities
{
    public class TRSItemEntity : EntityBase<int>
    {
        public TRSCategoryEnum Category { get; set; }

        /// <summary>
        /// item No
        /// </summary>
        public string Item { get; set; }

        public string Text { get; set; }

        public string Description { get; set; }

        public string KeyBehavior { get; set; }

        public TRSFilterEnum Filter { get; set; }

        public string TAPlanItem { get; set; }

        public string TAPlanItemType { get; set; }

        public string TAItemInstructions { get; set; }

        public string LinkingDocument { get; set; }

        public bool IsRequired { get; set; }

        public bool ShowByInfants { get; set; }

        public bool ShowByToddlers { get; set; }

        public bool ShowByPreschool { get; set; }

        public bool ShowBySchoolAge { get; set; }

        public TRSItemType Type { get; set; }

        public int SubCategoryId { get; set; }

        public TRSFacilityEnum LCSA { get; set; }

        public TRSFacilityEnum LCCH { get; set; }

        public TRSFacilityEnum RCCH { get; set; }

        public TRSFacilityEnum LCAA { get; set; }

        /// <summary>
        /// 答案如何同步
        /// </summary>
        public SyncAnswerType SyncAnswer { get; set; }

        /// <summary>
        /// 排序号，升序排列
        /// </summary>
        public int Sort { get; set; }
        public virtual ICollection<TRSItemAnswerEntity> ItemAnswers { get; set; }

        public virtual TRSSubcategoryEntity SubCategory { get; set; }

        public bool IsDeleted { get; set; }

    }
}
