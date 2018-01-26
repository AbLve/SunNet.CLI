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
 * CreatedOn:		2014/12/16 8:57:14
 * Description:		Please input class summary
 * Version History:	Created,2014/12/16 8:57:14
 * 
 * 
 **************************************************************************/
using Sunnet.Framework.Core.Base;

namespace Sunnet.Cli.Core.Cot.Entities
{
    public class CotAssessmentEntity : EntityBase<int>
    {
        private ICollection<CotWaveEntity> _waves;
        private ICollection<CotStgReportEntity> _reports;
        private ICollection<CotAssessmentItemEntity> _items;
        private ICollection<CotAssessmentWaveItemEntity> _waveItems;

        /// <summary>
        /// Teacher表的ID
        /// </summary>
        [DisplayName("Teacher")]
        public int TeacherId { get; set; }

        public int AssessmentId { get; set; }

        [DisplayName("School Year")]
        [StringLength(10)]
        public string SchoolYear { get; set; }

        public CotAssessmentStatus Status { get; set; }

        [DisplayName("Created By")]
        public int CreatedBy { get; set; }

        [DisplayName("Updated By")]
        public int UpdatedBy { get; set; }


        public virtual ICollection<CotAssessmentItemEntity> Items
        {
            get { return _items ?? (_items = new Collection<CotAssessmentItemEntity>()); }
            set { _items = value; }
        }

        public virtual ICollection<CotAssessmentWaveItemEntity> WaveItems
        {
            get { return _waveItems ?? (_waveItems = new Collection<CotAssessmentWaveItemEntity>()); }
            set { _waveItems = value; }
        }

        public virtual ICollection<CotStgReportEntity> Reports
        {
            get { return _reports ?? (_reports = new Collection<CotStgReportEntity>()); }
            set { _reports = value; }
        }

        public virtual ICollection<CotWaveEntity> Waves
        {
            get { return _waves ?? (_waves = new Collection<CotWaveEntity>()); }
            set { _waves = value; }
        }
    }
}
