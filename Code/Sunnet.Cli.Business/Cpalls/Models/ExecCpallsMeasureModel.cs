using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
/**************************************************************************
 * Developer: 		JackZhang
 * Computer:		JACKZ
 * Domain:			Jackz
 * CreatedOn:		2014/9/11 1:15:16
 * Description:		Please input class summary
 * Version History:	Created,2014/9/11 1:15:16
 * 
 * 
 **************************************************************************/
using Sunnet.Cli.Core.Ade;
using Sunnet.Cli.Core.Ade.Entities;
using Sunnet.Cli.Core.Cpalls;

namespace Sunnet.Cli.Business.Cpalls.Models
{
    public class ExecCpallsMeasureModel
    {
        private List<string> _links;
        private decimal _goal;
        public int ExecId { get; set; }
        public int MeasureId { get; set; }
        public OrderType OrderType { get; set; }
        public ItemShowType ShowType { get; set; }
        public string Name { get; set; }

        public int InnerTime { get; set; }
        public int Timeout { get; set; }

        public string StartPageHtml { get; set; }
        public string EndPageHtml { get; set; }

        public decimal Benchmark { get; set; }

        public int BenchmarkId { get; set; }

        public decimal LowerScore { get; set; }

        public decimal HigherScore { get; set; }

        public string BenchmarkText
        {
            get;
            set;
        }

        public string AgeGroup { get; set; }

        public decimal TotalScore { get; set; }
        public bool TotalScored { get; set; }

        public decimal Goal
        {
            get
            {
                if (_goal < 0)
                    return 0;
                return _goal;
            }
            set { _goal = value; }
        }

        public DateTime UpdatedOn { get; set; }

        /// <summary>
        /// Measure links.
        /// </summary>
        public List<string> Links
        {
            get { return _links ?? (_links = new List<string>()); }
            set { _links = value; }
        }

        public int PauseTime { get; set; }

        public CpallsStatus Status { get; set; }

        public bool Readonly
        {
            get { return Status == CpallsStatus.Finished || Status == CpallsStatus.Locked; }
        }

        public IEnumerable<ExecCpallsItemModel> Items { get; set; }

        public ExecCpallsParentMeasureModel Parent { get; set; }

        public int ParentSort { get; set; }

        public bool IsParent { get; set; }
        public int Children { get; set; }
        public int Sort { get; set; }

        public string ApplyToWave { get; set; }

        public bool StopButton { get; set; }

        public bool NextButton { get; set; }

        public bool PreviousButton { get; set; }

        public ExecCpallsMeasureModel Clone()
        {
            var newObj = new ExecCpallsMeasureModel();
            newObj.AgeGroup = this.AgeGroup;
            newObj.Benchmark = this.Benchmark;
            newObj.EndPageHtml = this.EndPageHtml;
            newObj.ExecId = this.ExecId;
            newObj.Goal = this.Goal;
            newObj.InnerTime = this.InnerTime;
            newObj.Links = this.Links;
            newObj.MeasureId = this.MeasureId;
            newObj.Name = this.Name;
            newObj.OrderType = this.OrderType;
            newObj.Parent = this.Parent;
            newObj.ParentSort = this.ParentSort;
            newObj.PauseTime = this.PauseTime;
            newObj.ShowType = this.ShowType;
            newObj.Sort = this.Sort;
            newObj.StartPageHtml = this.StartPageHtml;
            newObj.Status = this.Status;
            newObj.Timeout = this.Timeout;
            newObj.TotalScore = this.TotalScore;
            newObj.TotalScored = this.TotalScored;
            newObj.UpdatedOn = this.UpdatedOn;
            newObj.Comment = this.Comment;
            newObj.StopButton = this.StopButton;
            newObj.NextButton = this.NextButton;
            newObj.PreviousButton = this.PreviousButton;
            newObj.ShowLaunchPage = this.ShowLaunchPage;
            newObj.ShowFinalizePage = this.ShowFinalizePage;
            newObj.Items = this.Items.Select(x => new ExecCpallsItemModel()
            {
                Answers = x.Answers,
                AnswerType = x.AnswerType,
                ExecId = x.ExecId,
                Goal = x.Goal,
                IsCorrect = x.IsCorrect,
                ItemId = x.ItemId,
                Label = x.Label,
                Links = x.Links,
                MeasureId = x.MeasureId,
                PauseTime = x.PauseTime,
                Score = x.Score,
                Scored = x.Scored,
                Description = x.Description,
                SelectedAnswers = x.SelectedAnswers,
                Status = x.Status,
                Timed = x.Timed,
                WaitTime = x.WaitTime,
                Type = x.Type,
                RandomAnswer = x.RandomAnswer,
                Props = x.Props,
                Details = x.Details,
                IsPractice = x.IsPractice,
                ShowAtTestResume = x.ShowAtTestResume,
                ResultIndex = x.ResultIndex
            }).ToList();
            return newObj;
        }

        public string Comment { get; set; }

        public bool ShowLaunchPage { get; set; }
        public bool ShowFinalizePage { get; set; }

        public IEnumerable<ExecCpallsCutOffScoreModel> CutOffScores { get; set; }
    }
}