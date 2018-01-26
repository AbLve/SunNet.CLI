using Sunnet.Cli.Core.Ade;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sunnet.Cli.Core.Cpalls.Models.Report
{
    public class CompletionMeasureModel
    {
        public CompletionMeasureModel()
        {
        }

        public CompletionMeasureModel(IDataReader reader)
        {
            Wave = (Wave)(byte.Parse(reader["Wave"].ToString()));

            MeasureId = (int)reader["MeasureId"];
            MeasureName = (string)reader["MeasureName"];
            RelatedMeasureId = (int)reader["RelatedMeasureId"];
            ParentId = (int)reader["ParentId"];

            Status = (CpallsStatus)(byte.Parse(reader["Status"].ToString()));
            Sort = (int)reader["Sort"];
            Language = (AssessmentLanguage)(byte.Parse(reader["Language"].ToString()));

            StudentId = (int)reader["StudentId"];

            Goal = decimal.Parse(reader["Goal"].ToString());
        }

        public Wave Wave { get; set; }

        public int MeasureId { get; set; }

        public string MeasureName { get; set; }

        public CpallsStatus Status { get; set; }

        public int Sort { get; set; }

        public int ParentId { get; set; }

        public List<CompletionMeasureModel> Children { get; set; }

        public int RelatedMeasureId { get; set; }

        public int StudentId { get; set; }

        public AssessmentLanguage Language { get; set; }

        public decimal Goal { get; set; }
    }
}
