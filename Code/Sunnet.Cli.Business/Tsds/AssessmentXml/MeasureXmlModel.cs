using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sunnet.Cli.Business.Tsds.AssessmentXml
{
    /*
     这里的Assessment对应CLI系统中Assessment的Measure
     */
    public class MeasureXmlModel
    {
        /// <summary>
        /// AID-Measure ID
        /// </summary>
        public string AssessmentID { get; set; }

        /// <summary>
        /// Measure Display Name
        /// </summary>
        public string AssessmentTitle { get; set; }

        public string AssessmentIdentificationSystem { get; set; }

        /// <summary>
        /// Measure ID
        /// </summary>
        public string AssessmentIdentificationID { get; set; }

        /// <summary>
        /// Parent Measure Display Name
        /// </summary>
        public string AssessmentCategory { get; set; }

        /// <summary>
        /// Parent Measure Display Name (Subject)
        /// </summary>
        public string AcademicSubject { get; set; }

        public string GradeLevelAssessed { get; set; }

        public string PerformanceDescription { get; set; }

        public string AssessmentReportingMethod { get; set; }

        /// <summary>
        /// Statewide, National standards
        /// </summary>
        public string ContentStandard { get; set; }

        /// <summary>
        /// Measure label
        /// </summary>
        public string Version { get; set; }

        /// <summary>
        /// latest updated date
        /// </summary>
        public string RevisionDate { get; set; }

        public string MaxRawScore { get; set; }

        public string ReportAssessmentType { get; set; }

        /// <summary>
        /// Wave value(s)
        /// </summary>
        public string AssessmentPeriodDescription { get; set; }

        public List<string> AssessmentItemReference { get; set; }
    }
}
