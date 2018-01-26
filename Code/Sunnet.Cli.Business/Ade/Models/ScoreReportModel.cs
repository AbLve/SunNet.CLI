using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sunnet.Cli.Core.Cpalls;
using Sunnet.Cli.Core.Ade;

namespace Sunnet.Cli.Business.Ade.Models
{
    public class ScoreReportModel
    {
        public int AssessmentId { get; set; }
        public int ScoreId { get; set; }
        public string ScoreDomain { get; set; }
        public string ScoreDescription { get; set; }
        public int TargetRound { get; set; }
        public Wave Wave { get; set; }
        public int StudentId { get; set; }
        public string StudentName { get; set; }
        public List<ScoreMeasureModel> ScoreMeasures { get; set; }
        public decimal? FinalScore { get; set; }

        public int BenchmarkId { get; set; }

        public string LabelText { get; set; }

        public string Color { get; set; }

        public string BlackWhite { get; set; }
    }

    public class ScoreInitModel
    {
        public int ScoreId { get; set; }
        public string ScoreDomain { get; set; }
        public int TargetRound { get; set; }
        public List<ScoreMeasureModel> ScoreMeasures { get; set; }
        public decimal? FinalScore { get; set; }
        public List<ScoreBenchmarkModel> ScoreBenchmarks { get; set; }
    }

    public class ScoreMeasureModel
    {
        public int MeasureId { get; set; }
        public string MeasureName { get; set; }
        public decimal? Goal { get; set; }

        public Wave Wave { get; set; }
    }

    public class ScoreBenchmarkModel
    {
        public int BenchmarkId { get; set; }
        public string BenchmarkLabel { get; set; }
    }

    //为前台生成报表时，计算average时存储临时数据时使用
    public class BenchmarkHostModel
    {
        public int BenchmarkId { get; set; }
        //表示StudentID，ClassId，SchoolID
        public int HostId { get; set; }
        //这个字段可能是
        public decimal Scores { get; set; }
    }

    public class ScoreSelectModel
    {
        public int AssessmentId { get; set; }
        public int ScoreId { get; set; }
        public string ScoreName { get; set; }
        public string ScoreDomain { get; set; }
    }
}
