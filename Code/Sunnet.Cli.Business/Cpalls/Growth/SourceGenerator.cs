using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
/**************************************************************************
 * Developer: 		JackZhang
 * Computer:		JACKZ
 * Domain:			Jackz
 * CreatedOn:		2014/11/17 3:31:10
 * Description:		Please input class summary
 * Version History:	Created,2014/11/17 3:31:10
 * 
 * 
 **************************************************************************/
using LinqKit;
using Sunnet.Cli.Business.Ade;
using Sunnet.Cli.Business.Common;
using Sunnet.Cli.Business.Cpalls.Models;
using Sunnet.Cli.Business.Cpalls.Models.Report;
using Sunnet.Cli.Core.Ade.Entities;
using Sunnet.Cli.Core.Cpalls;
using Sunnet.Cli.Core.Cpalls.Models.Report;
using Sunnet.Framework.Extensions;

namespace Sunnet.Cli.Business.Cpalls.Growth
{
    #region Definition
    internal abstract class SourceGenerator<TSource, THeader, TRecord> : IReportGenerator
        where TSource : class
        where THeader : class
        where TRecord : class
    {
        private List<int> _allSelectedMeasureIds;
        private int _waveCount;
        protected TSource Source { get; private set; }
        protected List<TRecord> Records { get; private set; }
        protected List<THeader> Measures { get; private set; }
        protected Dictionary<Wave, IEnumerable<int>> Waves { get; private set; }

        protected List<int> AllSelectedMeasureIds
        {
            get
            {
                if (_allSelectedMeasureIds == null)
                {
                    _allSelectedMeasureIds = new List<int>();
                    Waves.ForEach(wave => _allSelectedMeasureIds.AddRange(wave.Value));
                    _allSelectedMeasureIds = _allSelectedMeasureIds.Distinct().ToList();
                }
                return _allSelectedMeasureIds;
            }
        }

        protected int WaveCount
        {
            get
            {
                if (_waveCount == 0)
                {
                    _waveCount = Waves.Count;
                }
                return _waveCount;
            }
        }

        protected SourceGenerator(TSource source,
            IEnumerable<THeader> measures,
            IEnumerable<TRecord> records, Dictionary<Wave, IEnumerable<int>> waves)
        {
            Source = source;
            Records = records.ToList();
            Measures = measures.ToList();

            Waves = new Dictionary<Wave, IEnumerable<int>>();
            waves.ForEach(wave =>
            {
                if (wave.Value != null && wave.Value.Any())
                    Waves.Add(wave.Key, wave.Value);
            });
        }

        /// <summary>
        /// 是否显示每个Measure的总分.
        /// </summary>
        public bool ShowMeasureTotalScore { get; set; }
        /// <summary>
        /// 是否为父级Measure生成Total列.
        /// </summary>
        public bool ShowTotalColumn { get; set; }

        #region Export member

        protected abstract void Report_Started();

        /// <summary>
        /// 开始写Headers.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="wave">The wave.</param>
        protected abstract void Header_Started();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="wave">The wave.</param>
        /// <param name="index">The index.</param>
        protected abstract void Wave_Added(Wave wave, int index);

        /// <summary>
        /// 新的Measure Headers.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="wave">The wave.</param>
        /// <param name="isParent">if set to <c>true</c> [is parent].</param>
        protected abstract void Measure_Added(THeader measure, int index, bool isParent = false, int children = 0);

        /// <summary>
        /// 表格头输出完毕.
        /// </summary>
        protected abstract void Header_Over();

        protected abstract void Row_Started();
        protected abstract void Record_Generating(List<TRecord> records, Wave wave, THeader measure, int index);

        protected abstract void Row_Ended();

        protected abstract void Report_Over();

        /// <summary>
        /// 设置每个单元格的样式（根据值计算）.
        /// </summary>
        /// <param name="cell">The cell.</param>
        /// <param name="source">The source.</param>
        /// <param name="records">The records.</param>
        /// <param name="wave">The wave.</param>
        /// <param name="measure">The measure.</param>
        /// <param name="index">The index.</param>
        /// <returns></returns>
        protected abstract void SetCellStyle(object cell, List<TRecord> records, Wave wave,
            THeader measure, int index, bool total = false);
        #endregion

        #region Host Members
        /// <summary>
        /// 报表标题：例如 Wave，School，Class，Student
        /// </summary>
        protected abstract string GetTitle();

        /// <summary>
        /// 获取显示名
        /// </summary>
        /// <param name="source">The source.</param>
        protected abstract string GetSourceName();

        protected abstract int GetSourceID();

        protected abstract string CalcValue(List<TRecord> records, Wave wave, ReportMeasureHeaderModel measure, int index, bool total = false);
        #endregion

        public abstract void Generate();
        public abstract string Filename { get; }
        public abstract Dictionary<object, List<ReportRowModel>> Reports { get; }
    }
    #endregion

    #region Report type - By Measure
    /// <summary>
    /// Measure为行显示单个实体(School/Class/Student)的报表
    /// </summary>
    /// <typeparam name="TSource">The type of the source.</typeparam>
    /// <typeparam name="TRecord">The type of the record.</typeparam>
    internal abstract class SourceByMeasureGenerator<TSource, TRecord> :
        SourceGenerator<TSource, ReportMeasureHeaderModel, TRecord>
        where TSource : class
        where TRecord : class
    {
        protected SourceByMeasureGenerator(TSource source, IEnumerable<ReportMeasureHeaderModel> measures, IEnumerable<TRecord> records, Dictionary<Wave, IEnumerable<int>> waves)
            : base(source, measures, records, waves)
        {
        }
        private List<ReportMeasureHeaderModel> GetHeaders(IEnumerable<ReportMeasureHeaderModel> headers, IEnumerable<int> selectedMeasureIds)
        {
            var wHeaders = headers.Where(x => (selectedMeasureIds == null || selectedMeasureIds.Contains(x.ID))).ToList();

            wHeaders.ForEach(x => x.IsLastOfChildren = false);

            var parentIds = wHeaders.Where(x => x.ParentId > 1).Select(x => x.ParentId).Distinct();
            parentIds.ForEach(delegate(int parentId)
            {
                var last = (wHeaders.LastOrDefault(child => child.ParentId == parentId));
                if (last != null)
                {
                    last.IsLastOfChildren = true;
                }
                var selectedCount = wHeaders.Count(x => x.ParentId == parentId) + 1;
                wHeaders.FindAll(x => x.ParentId == parentId).ForEach(p => p.Count = selectedCount);
            });
            return wHeaders;
        }

        public sealed override void Generate()
        {
            Report_Started();
            Header_Started();
            for (int w = 0; w < WaveCount; w++)
            {
                var wave = Waves.Keys.ToList()[w];
                Wave_Added(wave, w);
            }
            Header_Over();

            var selectedMeasureIds = AllSelectedMeasureIds;
            var selectedMeasures = GetHeaders(Measures, selectedMeasureIds);
            var measureCount = selectedMeasures.Count;
            var addedParentMeasure = new List<int>();
            for (int m = 0; m < measureCount; m++)
            {
                Row_Started();
                var measure = selectedMeasures[m];
                if (measure.ParentId > 1)
                {
                    var childMeasureCount = selectedMeasures.Count(x => selectedMeasureIds.Contains(x.ID) && x.ParentId == measure.ParentId);
                    if (ShowTotalColumn) childMeasureCount++;

                    if (!addedParentMeasure.Contains(measure.ParentId))
                    {
                        Measure_Added(measure, m, true, childMeasureCount);
                        addedParentMeasure.Add(measure.ParentId);
                    }
                    else
                    {
                        Measure_Added(measure, m);
                    }
                }
                else
                {
                    Measure_Added(measure, m);
                }

                for (int w = 0; w < WaveCount; w++)
                {
                    var wave = Waves.Keys.ToList()[w];
                    Record_Generating(Records, wave, measure, m);
                }
                Row_Ended();
            }

            Report_Over();
        }
    }
    #endregion

    #region Export type
    internal abstract class SourceByMeasurePdfGenerator<TSource, TRecord> : SourceByMeasureGenerator<TSource, TRecord>
        where TSource : class
        where TRecord : class
    {

        protected SourceByMeasurePdfGenerator(TSource source, IEnumerable<ReportMeasureHeaderModel> measures, IEnumerable<TRecord> records, Dictionary<Wave, IEnumerable<int>> waves)
            : base(source, measures, records, waves)
        {
            sourceId = GetSourceID();
        }

        private int sourceId;
        private ReportRowModel _waveHeader;
        private ReportRowModel _dataRow;
        private ReportRowModel _totalRow;
        protected override void Report_Started()
        {
            _waveHeader = new ReportRowModel();
            _dataRow = new ReportRowModel();
        }

        protected override void Header_Started()
        {
            _waveHeader.Add("Measure", 2, 1);
            if (ShowMeasureTotalScore)
                _waveHeader.Add(ReportText.TotalScore);
        }

        protected override void Wave_Added(Wave wave, int index)
        {
            _waveHeader.Add("Wave " + wave.ToDescription());
        }

        protected override void Header_Over()
        {
            AddRow(_waveHeader);
        }

        protected override void Row_Started()
        {
            _dataRow = new ReportRowModel();
        }

        protected override void Measure_Added(ReportMeasureHeaderModel measure, int index, bool isParent = false, int children = 0)
        {
            if (isParent)
            {
                _dataRow.Add(measure.ParentName, 1, children, true);
                _dataRow.Add(measure.Name);
            }
            else
            {
                if (measure.ParentId > 1)
                    _dataRow.Add(measure.Name, 1, 1);
                else
                    _dataRow.Add(measure.Name, 2, 1);
            }
            if (ShowMeasureTotalScore)
            {
                if (measure.TotalScored)
                    _dataRow.Add(measure.TotalScore.ToPrecisionString(2));
                else
                    _dataRow.Add(ReportText.No_Record);
            }

            if (measure.IsLastOfChildren && ShowTotalColumn)
            {
                if (_totalRow == null)
                    _totalRow = new ReportRowModel();
                _totalRow.Add(ReportText.TotalColumnName);
                if (ShowMeasureTotalScore)
                {
                    var childMeasures = Measures.FindAll(x => x.ParentId == measure.ParentId && x.TotalScored);
                    if (childMeasures.Any())
                        _totalRow.Add(childMeasures.Sum(x => x.TotalScore).ToPrecisionString(2));
                    else
                        _totalRow.Add(ReportText.No_Record);
                }
            }
        }

        protected override void Record_Generating(List<TRecord> records, Wave wave, ReportMeasureHeaderModel measure, int index)
        {
            var value = CalcValue(records, wave, measure, index);
            _dataRow.Add(value);
            SetCellStyle(_dataRow.Cells.Last(), records, wave, measure, index);

            if (measure.IsLastOfChildren && ShowTotalColumn)
            {
                value = CalcValue(records, wave, measure, index, true);
                _totalRow.Add(value);
                SetCellStyle(_totalRow.Cells.Last(), records, wave, measure, index, true);
            }
        }

        protected override void Row_Ended()
        {
            AddRow(_dataRow);
            _dataRow = null;
            AddRow(_totalRow);
            _totalRow = null;
        }

        protected override void Report_Over()
        {

        }

        public override string Filename
        {
            get { throw new NotImplementedException(); }
        }

        private void AddRow(ReportRowModel row)
        {
            if (!Reports.ContainsKey(sourceId))
                Reports.Add(sourceId, new List<ReportRowModel>());
            if (row != null)
                Reports[sourceId].Add(row);
        }
        private Dictionary<object, List<ReportRowModel>> _result;
        public override Dictionary<object, List<ReportRowModel>> Reports
        {
            get { return _result ?? (_result = new Dictionary<object, List<ReportRowModel>>()); }
        }
    }
    #endregion

    #region Host(Student)
    internal class SourceByMeasurePdfStudentGenerator :
        SourceByMeasurePdfGenerator<CpallsStudentModel, StudentRecordModel>
    {
        private AdeBusiness _adeBusiness;
        private AdeBusiness AdeBusiness
        {
            get { return _adeBusiness ?? (_adeBusiness = new AdeBusiness()); }
            set { _adeBusiness = value; }
        }

        private List<CutOffScoreEntity> _cutoffScores;
        public SourceByMeasurePdfStudentGenerator(CpallsStudentModel source, IEnumerable<ReportMeasureHeaderModel> measures, IEnumerable<StudentRecordModel> records, Dictionary<Wave, IEnumerable<int>> waves)
            : base(source, measures, records, waves)
        {
            _cutoffScores = AdeBusiness.GetCutOffScores<MeasureEntity>(AllSelectedMeasureIds);
        }

        protected override void SetCellStyle(object cell, List<StudentRecordModel> records, Wave wave, ReportMeasureHeaderModel measure, int index, bool total = false)
        {
            if (!Waves.Keys.Contains(wave) || !Waves[wave].Contains(measure.ID))
            {
                return;
            }
            var cellModel = cell as ReportCellModel;
            bool lightColor = measure.LightColor;
            bool hasCutOffScore = measure.HasCutOffScores;
            if (cellModel != null)
            {
                var recordsOfStu = records.FindAll(x => x.Wave == wave && x.MeasureId == measure.ID && Source.ID == x.StudentId);
                var bentchmark = 0m;
                var goal = -1m;
                if (measure.TotalScored && recordsOfStu.Any() && total == false)
                {
                    bentchmark =
                        CpallsBusiness.CalculateBenchmark(
                            _cutoffScores.FindAll(x => x.HostId == measure.ID && x.Wave == wave),
                            Source.BirthDate, Source.SchoolYear.ToSchoolYearString());
                    goal = recordsOfStu.Any() ? recordsOfStu.First().Goal : 0m;
                    if (wave == Wave.BOY)
                        hasCutOffScore = measure.BOYHasCutOffScores;
                    else if (wave == Wave.MOY)
                        hasCutOffScore = measure.MOYHasCutOffScores;
                    else if (wave == Wave.EOY)
                        hasCutOffScore = measure.EOYHasCutOffScores;
                }
                else if (measure.ParentScored && total == true)
                {
                    recordsOfStu = records.FindAll(x => x.StudentId == Source.ID && x.Wave == wave
                                                          &&
                                                          Measures.Any(m => m.ParentId == measure.ParentId && m.TotalScored && m.ID == x.MeasureId));
                    if (recordsOfStu.Any())
                    {
                        bentchmark =
                                CpallsBusiness.CalculateBenchmark(
                                    _cutoffScores.FindAll(x => x.HostId == measure.ParentId && x.Wave == wave),
                                    Source.BirthDate, Source.SchoolYear.ToSchoolYearString());
                        goal = recordsOfStu.Sum(x => x.Goal);
                        if (_adeBusiness == null)
                        {
                            _adeBusiness = new AdeBusiness();
                        }
                        var currentMeasure = _adeBusiness.GetMeasureModel(measure.ID);
                        lightColor = currentMeasure.Parent.LightColor;
                        if (wave == Wave.BOY)
                            hasCutOffScore = currentMeasure.Parent.BOYHasCutOffScores;
                        else if (wave == Wave.MOY)
                            hasCutOffScore = currentMeasure.Parent.MOYHasCutOffScores;
                        else if (wave == Wave.EOY)
                            hasCutOffScore = currentMeasure.Parent.EOYHasCutOffScores;
                    }
                }
                if (goal >= 0)
                {
                    if (!hasCutOffScore)
                        cellModel.Background = ReportTheme.Missing_Bentchmark_ClassName;
                    else if (bentchmark < 0)
                        cellModel.Background = lightColor ? ReportTheme.TE3_Light_ClassName : ReportTheme.TE3_ClassName;
                    else
                    {
                        if (goal < bentchmark)
                        {
                            if (Source.Age >= 4)
                                cellModel.Background = lightColor ? ReportTheme.GE4_Light_ClassName : ReportTheme.GE4_ClassName;
                            else
                                cellModel.Background = lightColor ? ReportTheme.TE4_Light_ClassName : ReportTheme.TE4_ClassName;
                        }
                        else
                            cellModel.Background = lightColor ? ReportTheme.Passed_Light_ClassName : ReportTheme.Passed_ClassName;
                    }
                }
            }
        }

        protected override string GetTitle()
        {
            return "Student";
        }

        protected override string GetSourceName()
        {
            return Source.FirstName + " " + Source.LastName;
        }

        protected override int GetSourceID()
        {
            return Source.ID;
        }

        protected override string CalcValue(List<StudentRecordModel> records, Wave wave, ReportMeasureHeaderModel measure, int index, bool total = false)
        {
            if (total == false)
            {
                if (!Waves.Keys.Contains(wave) || !Waves[wave].Contains(measure.ID))
                {
                    return ReportText.No_Choosed;
                }
                var recordOfStu = records.Find(x => x.Wave == wave && x.MeasureId == measure.ID && Source.ID == x.StudentId);

                if (measure.TotalScored && recordOfStu != null)
                {
                    return recordOfStu.Goal.ToPrecisionString(2);
                }
                else if (!measure.TotalScored)
                {
                    return ReportText.No_TotalScored;
                }
                else
                {
                    return ReportText.No_Record;
                }
            }
            else
            {
                if (measure.ParentScored)
                {
                    var recordsOfStu = records.Where(x => x.StudentId == Source.ID && x.Wave == wave
                                                          &&
                                                          Measures.Where(m => m.ParentId == measure.ParentId && m.TotalScored)
                                                              .Select(cm => cm.ID)
                                                              .Contains(x.MeasureId)).ToList();
                    if (recordsOfStu.Any())
                    {
                        var totalScore = recordsOfStu.Sum(x => x.Goal);
                        return totalScore.ToPrecisionString(2);
                    }
                    else
                    {
                        return ReportText.No_Record;
                    }
                }
                else
                {
                    return ReportText.No_TotalScored;
                }
            }
        }
    }
    #endregion

}
