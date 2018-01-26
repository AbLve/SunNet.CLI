using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/**************************************************************************
 * Developer: 		JackZhang
 * Computer:		JACKZ
 * Domain:			JackZ
 * CreatedOn:		1/20 2015 15:43:42
 * Description:		Please input class summary
 * Version History:	Created,1/20 2015 15:43:42
 * 
 * 
 **************************************************************************/
using LinqKit;
using Newtonsoft.Json;
using Sunnet.Cli.Business.Common;
using Sunnet.Cli.Core.Trs;
using Sunnet.Framework.Extensions;

namespace Sunnet.Cli.Business.Trs.Models
{
    public abstract class TrsAssessmentModelBase
    {
       // private DateTime _approveDate;
        private string _taStatusString;

        public int Id { get; set; }

        public int SchoolId { get; set; }

        public TrsSchoolModel School { get; set; }

        public TRSStatusEnum Status { get; set; }

        /// <summary>
        /// 如果Status 是Completed, 但是该属性未赋值(0), 那么该条记录为添加Event Log产生的
        /// </summary>
        [DisplayName("Calculated Star")]
        public TRSStarEnum Star { get; set; }

        [DisplayName("Verified Star")]
        public TRSStarEnum VerifiedStar { get; set; }

        public TrsAssessmentType Type { get; set; }

        public EventLogType EventLogType { get; set; }

        [JsonIgnore]
        internal string TaStatusString
        {
            private get { return _taStatusString ?? (_taStatusString = string.Empty); }
            set { _taStatusString = value; }
        }

        [DisplayName("TA Status")]
        public IEnumerable<TrsTaStatus> TaStatuses
        {
            get
            {
                return TaStatusString.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries)
                    .Select(t => (TrsTaStatus)int.Parse(t));
            }
        }

        [DisplayName("Visit Date")]
        public DateTime VisitDate { get; set; }

        [DisplayName("Discuss Date")]
        public DateTime DiscussDate { get; set; }

        [DisplayName("Approval Date")]
        public DateTime ApproveDate
        {
            get; set;
            /*   David 12/14/2017
             * Ticket ID: 3188
 Title: TRS: Default approval date
 Description: In the TRS assessment tool both "Approval Date" and "Recertification Date" display with a default value over a year old. Approval date to be blank by default and recert date to be 3 years from whatever approval date is entered
             *get
             {
                 if (_approveDate <= CommonAgent.MinDate)
                 {
                     _approveDate = CommonAgent.TrsMinDate;
                     RecertificatedBy = _approveDate.AddYears(3);
                 }
                 return _approveDate;
             }
             set { _approveDate = value; }*/
        }

        [DisplayName("Recertification By")]
        public DateTime RecertificatedBy { get; set; }

        public DateTime UpdatedOn { get; set; }

        public DateTime CreatedOn { get; set; }

        [JsonIgnore]
        public IEnumerable<TrsItemModel> Items { get; set; }

        public List<TrsClassModel> Classes { get; set; }

        public Dictionary<TRSCategoryEnum, Dictionary<int, List<TrsItemModel>>> Categories { get; protected set; }

        public Dictionary<int, TrsSubcategoryModel> SubCategory { get; set; }

        /// <summary>
        /// 各Category的星级.
        /// </summary>
        public Dictionary<TRSCategoryEnum, TRSStarEnum> StarOfCategory { get; set; }

        /// <summary>
        /// 把Items分解到Categories和各个Class.
        /// </summary>
        internal abstract void Prepare(bool showNAItem = false);

    }
}
