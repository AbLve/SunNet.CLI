using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/**************************************************************************
 * Developer: 		JackZhang
 * Computer:		JACKZ
 * Domain:			Jackz
 * CreatedOn:		2014/9/11 2:45:13
 * Description:		Please input class summary
 * Version History:	Created,2014/9/11 2:45:13
 * 
 * 
 **************************************************************************/
using Sunnet.Cli.Business.Common;
using Sunnet.Cli.Core.Ade;
using Sunnet.Cli.Core.Cpalls;

namespace Sunnet.Cli.Business.Cpalls.Models
{
    public class ExecCpallsAssessmentModel
    {
        private List<ExecCpallsMeasureModel> _measures;
        public int ExecId { get; set; }
        public int AssessmentId { get; set; }

        public AssessmentLanguage Language { get; set; }

        public OrderType OrderType { get; set; }

        public string Name { get; set; }

        public string SchoolYear { get; set; }

        public string SchoolName { get; set; }

        public string CommunityName { get; set; }

        public int SchoolId { get; set; }

        public Wave Wave { get; set; }

        public int StudentId { get; set; }

        public List<int> KeepSelectdMeasureIds { get; set; }

        public DateTime CreatedOn { get; set; }
        public bool InExecDateTemp
        {
            get
            {
                var startSchoolYear = CommonAgent.GetStartDateOfSchoolYear();
                var endSchoolYear = startSchoolYear.AddYears(1).AddDays(-1);
                bool inDate = startSchoolYear <= DateTime.Now && DateTime.Now <= endSchoolYear;
                return inDate;
            }
        }
        public bool InExecDate { get; set; }

        public string LegendUIFilePath { get; set; }

        public string LegendUIText { get; set; }

        public string LegendUITextPosition { get; set; }

        public List<ExecCpallsMeasureModel> Measures
        {
            get { return _measures ?? (_measures = new List<ExecCpallsMeasureModel>()); }
            set { _measures = value; }
        }

        public ExecCpallsStudentModel Student { get; set; }

        public ExecCpallsClassModel Class { get; set; }
    }
}
