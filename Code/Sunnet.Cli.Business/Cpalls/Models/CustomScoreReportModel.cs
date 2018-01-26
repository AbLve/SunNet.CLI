using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sunnet.Cli.Business.Ade.Models;
using Sunnet.Cli.Business.Common;
using Sunnet.Cli.Business.Cpalls.Models.Report;
using Sunnet.Cli.Core.Ade;
using Sunnet.Cli.Core.Cpalls;

namespace Sunnet.Cli.Business.Cpalls.Models
{
    public class CustomScoreReportModel
    {
        [DisplayName("Community/District")]
        public string Community { get; set; }

        public string School { get; set; }

        public int SchoolId { get; set; }

        public string Teacher { get; set; }

        public string Class { get; set; }

        public int ClassId { get; set; }

        public Wave Wave { get; set; }

        public int Year { get; set; }

        [DisplayName("School Year")]
        public string SchoolYear
        {
            get { return Year.ToFullSchoolYearString(); }
        }

        [DisplayName("Assessment Language")]
        public StudentAssessmentLanguage Language { get; set; }

        public List<ScoreReportModel> ScoreReports { get; set; }

        public List<ScoreInitModel> ScoreInits { get; set; }
    }
}
