using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sunnet.Cli.Core.Ade;
using Sunnet.Cli.Core.Cpalls;

namespace Sunnet.Cli.Core.Reports.Models
{
    public class CircleDataExportStudentItemModel
    {
        public int AssessmentId { get; set; }

        public string SchoolYear { get; set; }

        public Wave Wave { get; set; }

        public int CDId { get; set; }

        public int SchoolId { get; set; }

        public int StudentId { get; set; }

        public int SMId { get; set; }

        public int MeasureId { get; set; }

        public string Description { get; set; }

        public ItemType Type { get; set; }

        public int Id { get; set; }

        public int ItemId { get; set; }

        public decimal? Goal { get; set; }

        public decimal Score { get; set; }

        public bool Scored { get; set; }

        public bool IsCorrect { get; set; }

        public int PauseTime { get; set; }

        public string SelectedAnswers { get; set; }
    }
}
