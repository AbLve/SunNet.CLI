using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/**************************************************************************
 * Developer: 		JackZhang
 * Computer:		JACKZ
 * Domain:			JackZ
 * CreatedOn:		2/7 2015 13:59:00
 * Description:		Please input class summary
 * Version History:	Created,2/7 2015 13:59:00
 * 
 * 
 **************************************************************************/
using Sunnet.Cli.Business.Common;
using Sunnet.Cli.Core.Users.Enums;

namespace Sunnet.Cli.Business.Cot.Cumulative
{
    public class CumulativeReportModel : ReportModel
    {

        [DisplayName("Years In Project")]
        public List<string> YearsInProject { get; set; }

        [DisplayName("Coaching Model")]
        public List<AssignmentType> CoachModels { get; set; }

        [DisplayName("eCircle Assignment")]
        public List<AssignmentType> ECircles { get; set; }

        public int Teachers { get; set; }

    }
}
