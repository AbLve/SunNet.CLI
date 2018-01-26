using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;
/**************************************************************************
 * Developer: 		JackZhang
 * Computer:		JACKZ
 * Domain:			JackZ
 * CreatedOn:		2014/12/20 9:47:46
 * Description:		Please input class summary
 * Version History:	Created,2014/12/20 9:47:46
 * 
 * 
 **************************************************************************/
using DocumentFormat.OpenXml.Office2010.ExcelAc;
using Sunnet.Cli.Business.Common;
using Sunnet.Cli.Core.Cot;
using Sunnet.Cli.Core.Cot.Entities;
using Sunnet.Framework.Mvc;

namespace Sunnet.Cli.Business.Cot.Models
{
    public class CotStgReportModel
    {
        public CotStgReportModel()
        {
            Status = CotStgReportStatus.Initialised;
            SpentTime = string.Empty;
            GoalMetDate = CommonAgent.MinDate;
            GoalSetDate = CommonAgent.MinDate;
            CreatedOn = CommonAgent.MinDate;
            UpdatedOn = CommonAgent.MinDate;
            ShowFullText = true;
        }
        public CotStgReportModel(CotStgReportEntity entity)
        {
            this.ID = entity.ID;
            this.AdditionalComments = entity.AdditionalComments;
            this.CotAssessmentId = entity.CotAssessmentId;
            this.CotItems = entity.ReportItems.Select(x => x.ItemId).ToList();
            this.CreatedBy = entity.CreatedBy;
            this.CreatedOn = entity.CreatedOn;
            this.GoalMetDate = entity.GoalMetDate;
            this.GoalSetDate = entity.GoalSetDate;
            this.OnMyOwn = entity.OnMyOwn;
            this.SpentTime = entity.SpentTime;
            this.Status = entity.Status;
            this.UpdatedBy = entity.UpdatedBy;
            this.UpdatedOn = entity.UpdatedOn;
            this.WithSupport = entity.WithSupport;
            this.ShowFullText = entity.ShowFullText;
        }
        private IEnumerable<int> _cotItems;
        public int ID { get; set; }
        public int CotAssessmentId { get; set; }

        [DisplayName("Goal Set Date")]
        public DateTime GoalSetDate { get; set; }
        [Required]
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

        public DateTime CreatedOn { get; set; }
        public DateTime UpdatedOn { get; set; }
        public CotStgReportStatus Status { get; set; }

        public bool ShowFullText { get; set; }

        public IEnumerable<int> CotItems
        {
            get { return _cotItems ?? (_cotItems = new List<int>()); }
            set { _cotItems = value; }
        }

        public List<CotStgGroupModel> Groups { get; set; }
    }
}
