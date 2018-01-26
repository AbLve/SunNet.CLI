using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/**************************************************************************
 * Developer: 		Sam
 * Computer:		SAM-PC
 * Domain:			sam-pc
 * CreatedOn:		2016/8/9 19:54:40
 * Description:		Please input class summary
 * Version History:	Created,2016/8/9 19:54:40
 * 
 * 
 **************************************************************************/
using Sunnet.Framework;

namespace Sunnet.Cli.Business.Reports.Model
{

    public class ParentReportModel
    {
        public int ID { get; set; }
        public string ReportName { get; set; }
        public string Url
        {
            get
            {
                return (SFConfig.AssessmentDomain.EndsWith("\\") ? SFConfig.AssessmentDomain : SFConfig.AssessmentDomain + "\\") + "/report/student/DownloadReport?id=" + ID.ToString();
            }
        }
        public DateTime CreatedOn { get; set; }
        public DateTime DOB { get; set; }
        public int assessmentId { get; set; }
        public string StudentAge
        {
            get
            {

                return Common.CommonAgent.GetAge(DOB, CreatedOn);
            }
        }
    }
}
