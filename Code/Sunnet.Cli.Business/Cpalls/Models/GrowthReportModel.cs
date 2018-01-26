using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/**************************************************************************
 * Developer: 		JackZhang
 * Computer:		JACKZ
 * Domain:			JackZ
 * CreatedOn:		2/10 2015 17:02:24
 * Description:		Please input class summary
 * Version History:	Created,2/10 2015 17:02:24
 * 
 * 
 **************************************************************************/
using Sunnet.Cli.Business.Common;
using Sunnet.Cli.Business.Cpalls.Models.Report;
using Sunnet.Cli.Core.Ade;

namespace Sunnet.Cli.Business.Cpalls.Models
{
    public class GrowthReportModel
    {
        public string Title { get; set; }

        [DisplayName("Community/District")]
        public string Community { get; set; }

        public string School { get; set; }

        public int Schools { get; set; }

        public string Class { get; set; }

        public int Classes { get; set; }

        public string Teacher { get; set; }

        [DisplayName("Wave 1")]
        public DateTime Wave1 { get; set; }

        [DisplayName("Wave 2")]
        public DateTime Wave2 { get; set; }

        [DisplayName("Wave 3")]
        public DateTime Wave3 { get; set; }

        public int Year { get; set; }

        [DisplayName("School Year")]
        public string SchoolYear
        {
            get { return Year.ToFullSchoolYearString(); }
        }

        public GrowthReportType Type { get; set; }

        [DisplayName("Assessment Language")]
        public StudentAssessmentLanguage Language { get; set; }

        public List<ReportRowModel> Report { get; set; }
    }
}
