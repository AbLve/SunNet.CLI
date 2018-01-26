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

namespace Sunnet.Cli.Core.Observable.Entities
{
    public class ObservableAssessmentItemEntity : EntityBase<int>
    {
        public int ItemId { get; set; }
        public string Response { get; set; }
        public int ObservableAssessmentId { get; set; }
        [DisplayName("Created By")]
        public int CreatedBy { get; set; }
        [DisplayName("Updated By")]
        public int UpdatedBy { get; set; }

        public virtual ObservableAssessmentEntity ObservableAssessment { get; set; }

    }
}
