using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Sunnet.Cli.Core.Ade;
using Sunnet.Cli.Core.Ade.Entities;
using Sunnet.Cli.Core.Common.Enums;
using Sunnet.Cli.Core.Practices.Entities;
using Sunnet.Framework.Core.Base;
/**************************************************************************
 * Developer: 		JackZhang
 * Computer:		JACKZ
 * Domain:			Jackz
 * CreatedOn:		2014/8/11 0:56:14
 * Description:		Please input class summary
 * Version History:	Created,2014/8/11 0:56:14
 * 
 * 
 **************************************************************************/
using Sunnet.Framework.Mvc;

namespace Sunnet.Cli.Core.Practices.Entities
{

    public class CliAdeAssessmentEntity : EntityBase<int>, ICutOffScoreProperties
    {
        public AssessmentType Type { get; set; }

        [StringLength(100)]
        [Required]
        [DisplayName("Display Name")]
        public string Name { get; set; }

        /// <summary>
        /// Unique Label
        /// </summary>
        [StringLength(100)]
        [Required]
        [DisplayName("Assessment Label")]
        public string Label { get; set; }

        [DisplayName("Administration Order")]
        public OrderType OrderType { get; set; }

        [DisplayName("Total Scored")]
        public bool TotalScored { get; set; }

        public EntityStatus Status { get; set; }

        public AssessmentLanguage Language { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="AssessmentEntity"/> is locked.
        /// </summary>
        public bool Locked { get; set; }

        public bool IsDeleted { get; set; }

        [DisplayName("Created By")]
        public int CreatedBy { get; set; }

        [DisplayName("Updated By")]
        public int UpdatedBy { get; set; }

        [EensureEmptyIfNull]
        [StringLength(150)]
        [DisplayName("Description")]
        public string Description { get; set; }

        [DisplayName("Parent Report Cover Sheet")]
        public string ParentReportCoverPath { get; set; }

        [DisplayName("Parent Report Cover Sheet")]
        public string ParentReportCoverName { get; set; }

        [DisplayName("Display Percentile Rank Toggle")]
        public bool DisplayPercentileRank { get; set; }

        public virtual ICollection<CliAdeMeasureEntity> Measures { get; set; }

        //public virtual ICollection<AssessmentReportEntity> AssessmentReports { get; set; }

        //public virtual ICollection<AssessmentLegendEntity> AssessmentLegends { get; set; }
    }
}
