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
using Sunnet.Cli.Core.Ade.Entities;
using Sunnet.Cli.Core.Common.Enums;
using Sunnet.Cli.Core.Students.Entities;
using Sunnet.Framework.Core.Base;

namespace Sunnet.Cli.Core.Observable.Entities
{
    public class ObservableAssessmentEntity : EntityBase<int>
    {
        /// <summary>
        /// Student表的ID
        /// </summary>
        [DisplayName("Student")]
        public int StudentId { get; set; }

        public int ChildId { get; set; }

        public int AssessmentId { get; set; }

        public EntityStatus Status { get; set; }

        [DisplayName("Created By")]
        public int CreatedBy { get; set; }

        [DisplayName("Updated By")]
        public int UpdatedBy { get; set; }

        public string ReportUrl { get; set; }
        public string ReportName { get; set; }
        public virtual AssessmentEntity Assessment { get; set; }
        public virtual StudentEntity Student { get; set; }

    }
}
