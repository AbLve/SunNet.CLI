using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/**************************************************************************
 * Developer: 		JackZhang
 * Computer:		JACKZ
 * Domain:			Jackz
 * CreatedOn:		2014/12/17 9:43:03
 * Description:		Please input class summary
 * Version History:	Created,2014/12/17 9:43:03
 * 
 * 
 **************************************************************************/
using DocumentFormat.OpenXml.Office.CustomUI;
using LinqKit;
using Newtonsoft.Json;
using Sunnet.Cli.Business.Ade.Models;
using Sunnet.Cli.Core.Ade;
using Sunnet.Cli.Core.Ade.Entities;
using Sunnet.Framework;

namespace Sunnet.Cli.Business.Observable.Models
{
    public class ObservableReportModel
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Url {
            get
            {
                return (SFConfig.AssessmentDomain.EndsWith("\\") ? SFConfig.AssessmentDomain : SFConfig.AssessmentDomain+"\\") + "/Observable/Observable/AssessmentResults?ID=" + ID.ToString();
            }
        }
        public DateTime CreateOn { get; set; }
        public DateTime DOB { get; set; }
        public int assessmentId { get; set; }

        public string StudentAge {
            get
            {

                return Common.CommonAgent.GetAge(DOB,CreateOn); 
            }
        }
    }
}
