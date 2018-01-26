using Sunnet.Framework.Core.Base;
using System;
using System.Collections.Generic;

namespace Sunnet.Cli.Core.Tsds.Entities
{
    public class TsdsMapEntity : EntityBase<int>
    {
        public string AssessmentIdentificationCode { get; set; }
        public string AssessmentTitle { get; set; }
        public string AcademicSubject { get; set; }
        public string AssessmentLabel { get; set; }
        public int AssessmentId { get; set; }
        public string MeasureLabel { get; set; }
        public string MeasureName { get; set; }

        public Nullable<int> MeasureId { get; set; }
        public Nullable<int> Wave { get; set; }
        public string Score { get; set; }

    }
}
