using System.Collections.ObjectModel;
using System.ComponentModel;
using Sunnet.Framework.Core.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sunnet.Cli.Core.Trs.Entities
{
    public class TRSAssessmentEntity : EntityBase<int>
    {
        public TRSAssessmentEntity()
        {
            TAStatus = string.Empty;
        }
        private ICollection<TRSAssessmentItemEntity> _assessmentItems;
        private ICollection<TrsStarEntity> _classroomStars;
        private ICollection<TRSAssessmentClassEntity> _assessmentClasses;
        public int SchoolId { get; set; }

        public TRSStatusEnum Status { get; set; }

        public TRSStarEnum Star { get; set; }

        public TRSStarEnum VerifiedStar { get; set; }

        public TrsAssessmentType Type { get; set; }

        [DisplayName("Visit Date")]
        public DateTime VisitDate { get; set; }

        [DisplayName("Discuss Date")]
        public DateTime DiscussDate { get; set; }

        [DisplayName("Approval Date")]
        public DateTime ApproveDate { get; set; }

        [DisplayName("Recertification By")]
        public DateTime RecertificatedBy { get; set; }

        public string TAStatus { get; set; }
        public int CreatedBy { get; set; }

        public int UpdatedBy { get; set; }

        public virtual ICollection<TrsStarEntity> Stars
        {
            get { return _classroomStars ?? (_classroomStars = new Collection<TrsStarEntity>()); }
            set { _classroomStars = value; }
        }

        public virtual ICollection<TRSAssessmentItemEntity> AssessmentItems
        {
            get { return _assessmentItems ?? (_assessmentItems = new Collection<TRSAssessmentItemEntity>()); }
            set { _assessmentItems = value; }
        }

        public virtual ICollection<TRSAssessmentClassEntity> AssessmentClasses
        {
            get { return _assessmentClasses ?? (_assessmentClasses = new Collection<TRSAssessmentClassEntity>()); }
            set { _assessmentClasses = value; }
        }



        public bool IsDeleted { get; set; }

        public EventLogType EventLogType { get; set; }
    }
}
