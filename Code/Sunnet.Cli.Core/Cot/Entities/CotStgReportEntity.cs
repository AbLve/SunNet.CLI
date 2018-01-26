using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/**************************************************************************
 * Developer: 		JackZhang
 * Computer:		JACKZ
 * Domain:			Jackz
 * CreatedOn:		2014/12/16 9:16:02
 * Description:		Please input class summary
 * Version History:	Created,2014/12/16 9:16:02
 * 
 * 
 **************************************************************************/
using Sunnet.Cli.Core.Ade.Entities;
using Sunnet.Framework.Core.Base;
using Sunnet.Framework.Mvc;

namespace Sunnet.Cli.Core.Cot.Entities
{
    public class CotStgReportEntity : EntityBase<int>
    {
        public CotStgReportEntity()
        {
            Status = CotStgReportStatus.Initialised;
        }

        public int CotAssessmentId { get; set; }

        [DisplayName("Goal Set Date")]
        public DateTime GoalSetDate { get; set; }

        [DisplayName("Length of Visit (hours)")]
        [StringLength(10)]
        public string SpentTime { get; set; }

        [DisplayName("Goal Met Date")]
        public DateTime GoalMetDate { get; set; }

        [EensureEmptyIfNull]
        [StringLength(4000)]
        [DisplayName("On My Own")]
        public string OnMyOwn { get; set; }

        [EensureEmptyIfNull]
        [DisplayName("With Support")]
        [StringLength(4000)]
        public string WithSupport { get; set; }

        [EensureEmptyIfNull]
        [DisplayName("Additional Comments")]
        [StringLength(4000)]
        public string AdditionalComments { get; set; }


        [DisplayName("Created By")]
        public int CreatedBy { get; set; }

        [DisplayName("Updated By")]
        public int UpdatedBy { get; set; }

        public bool ShowFullText { get; set; }

        public virtual CotAssessmentEntity Assessment { get; set; }

        public CotStgReportStatus Status { get; set; }

        private ICollection<CotStgReportItemEntity> _items;
        public virtual ICollection<CotStgReportItemEntity> ReportItems
        {
            get { return _items ?? (_items = new Collection<CotStgReportItemEntity>()); }
            set { _items = value; }
        }

        private ICollection<CotStgGroupEntity> _cotStgGroups;

        public virtual ICollection<CotStgGroupEntity> CotStgGroups
        {
            get { return _cotStgGroups ?? (_cotStgGroups = new Collection<CotStgGroupEntity>()); }
            set { _cotStgGroups = value; }
        }
    }
}
