using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
/**************************************************************************
 * Developer: 		JackZhang
 * Computer:		JACKZ
 * Domain:			Jackz
 * CreatedOn:		2014/11/17 19:18:37
 * Description:		Please input class summary
 * Version History:	Created,2014/11/17 19:18:37
 * 
 * 
 **************************************************************************/
using LinqKit;
using Sunnet.Cli.Business.Cpalls.Models;
using Sunnet.Cli.Business.Cpalls.Models.Report;
using Sunnet.Cli.Core.Cpalls;
using Sunnet.Cli.Core.Cpalls.Models.Report;
using Sunnet.Framework.Extensions;
using Sunnet.Framework.Log;
using Sunnet.Cli.Core.Ade.Entities;
using Sunnet.Cli.Business.Ade.Models;
using Sunnet.Cli.Business.Practices.Models;

namespace Sunnet.Cli.Business.Cpalls
{
    #region Definition
    public enum SatisfactoryType
    {
        /// <summary>
        /// 包含总共的（Community/School/Class的总及格率）以及每个对象（School/Class/Student的及格率）
        /// 目前暂未实现
        /// </summary>
        ShowAll,

        /// <summary>
        /// 仅包含总共的（Community/School/Class的总及格率）
        /// </summary>
        Totaly,

        /// <summary>
        /// 仅包含每个对象（School/Class/Student的及格率）
        /// 目前暂未实现
        /// </summary>
        Separate
    }

    internal abstract class SatisfactoryGenerator<TSource, THeader, TRecord> : IReportGenerator
        where TSource : class
        where THeader : class
        where TRecord : class
    {
        private List<int> _allSelectedMeasureIds;
        private int _waveCount;
        private int _sourceCount;

        protected List<TSource> Sources { get; private set; }

        protected List<TRecord> Records { get; private set; }

        protected List<THeader> Measures { get; private set; }

        protected Dictionary<Wave, IEnumerable<int>> Waves { get; private set; }

        protected SatisfactoryType Type { get; private set; }

        protected List<BenchmarkModel> Benchmarks { get; private set; }

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

        protected int SourceCount
        {
            get
            {
                if (_sourceCount == 0)
                    _sourceCount = Sources == null ? 0 : Sources.Count;
                return _sourceCount;
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

        protected SatisfactoryGenerator(
            SatisfactoryType type,
            IEnumerable<TSource> sources,
            IEnumerable<THeader> measures,
            IEnumerable<TRecord> records,
            Dictionary<Wave, IEnumerable<int>> waves,
            IEnumerable<BenchmarkModel> benchmarks)
        {
            Type = type;
            Sources = sources == null ? new List<TSource>() : sources.ToList();
            Records = records.ToList();
            Measures = measures.ToList();

            Waves = new Dictionary<Wave, IEnumerable<int>>();
            waves.ForEach(wave =>
            {
                if (wave.Value != null && wave.Value.Any())
                    Waves.Add(wave.Key, wave.Value);
            });
            Benchmarks = new List<BenchmarkModel>();
            if (benchmarks != null)
                Benchmarks = benchmarks.ToList();
        }

        /// <summary>
        /// 是否为父级Measure生成Total列(行).
        /// </summary>
        public bool ShowTotalForMeasure { get; set; }

        /// <summary>
        /// 是否把Label相同的数据合并
        /// </summary>
        public bool MergeSameLabel { get; set; }

        /// <summary>
        /// true为% Meeting Benchmarks Report，false为Average Scores Report
        /// </summary>
        public bool MeetingBenchmark { get; set; }

        #region Host Members
        /// <summary>
        /// 报表标题：例如 Wave，School，Class，Student
        /// </summary>
        protected abstract string GetTitle();

        /// <summary>
        /// 获取显示名
        /// </summary>
        /// <param name="source">The source.</param>
        protected abstract string GetSourceName(TSource source);

        protected abstract int GetSourceID(TSource source);

        protected abstract string CalcValue(TSource source, List<TRecord> records, Wave wave, THeader measure, int index, bool total = false);

        protected abstract string CalcValue(List<TRecord> records, Wave wave, THeader measure, int index,
            bool total = false);

        protected abstract string CalcValue(List<TRecord> records, Wave wave, THeader measure, int index,
           List<BenchmarkModel> benchmarks, bool total = false);
        #endregion

        public abstract void Generate();

        public abstract string Filename { get; }
        public abstract Dictionary<object, List<ReportRowModel>> Reports { get; }
    }
    #endregion

    #region Report Type
    internal abstract class SatisfactoryByWaveGenerator<TSource, TRecord> :
        SatisfactoryGenerator<TSource, ReportMeasureHeaderModel, TRecord>
        where TSource : class
        where TRecord : class
    {
        #region Export member

        protected abstract void Report_Started();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="wave">The wave.</param>
        /// <param name="index">The index.</param>
        protected abstract void Wave_Started(Wave wave, int index);
        /// <summary>
        /// 开始写Headers.
        /// </summary>
        protected abstract void Generate_Header();

        /// <summary>
        /// 新的Measure Headers.
        /// </summary>
        /// <param name="measure">The measure.</param>
        /// <param name="index">The index.</param>
        /// <param name="isParent">if set to <c>true</c> [is parent].</param>
        /// <param name="children">The children.</param>
        protected abstract void Measure_Added(ReportMeasureHeaderModel measure, int index, bool isParent = false, int children = 0);

        protected abstract void Record_Generating(TSource source, List<TRecord> records, Wave wave, ReportMeasureHeaderModel measure,
            int index);
        protected abstract void Record_Generating(List<TRecord> records, Wave wave, ReportMeasureHeaderModel measure,
            int index);
        protected abstract void Measure_Over(ReportMeasureHeaderModel measure, int index, bool isParent = false, int children = 0);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="wave">The wave.</param>
        /// <param name="index">The index.</param>
        protected abstract void Wave_Over(Wave wave, int index);

        protected abstract void Report_Over();

        #endregion

        protected SatisfactoryByWaveGenerator(SatisfactoryType type, IEnumerable<TSource> sources,
            IEnumerable<ReportMeasureHeaderModel> measures,
            IEnumerable<TRecord> records, Dictionary<Wave,
            IEnumerable<int>> waves,
            IEnumerable<BenchmarkModel> benchmarks)
            : base(type, sources, measures, records, waves, benchmarks)
        {
        }
        private List<ReportMeasureHeaderModel> GetHeadersOfWave(IEnumerable<ReportMeasureHeaderModel> headers, Wave wave, IEnumerable<int> selectedMeasureIds)
        {
            var wHeaders = headers.Where(x => x.ApplyToWave.Contains(((int)wave).ToString())
                && (selectedMeasureIds == null || selectedMeasureIds.Contains(x.ID))).ToList();

            wHeaders.ForEach(x => x.IsLastOfChildren = false);

            var parentIds = wHeaders.Where(x => x.ParentId > 1).Select(x => x.ParentId).Distinct();
            parentIds.ForEach(delegate(int parentId)
            {
                var last = (wHeaders.LastOrDefault(child => child.ParentId == parentId));
                if (headers.Count(x => x.ParentId == parentId && x.ApplyToWave.Contains(((int)wave).ToString()))
                    == wHeaders.Count(x => x.ParentId == parentId)
                    && last != null)
                {
                    last.IsLastOfChildren = true;
                }
            });
            return wHeaders;
        }
        public sealed override void Generate()
        {
            Report_Started();
            for (int w = 0; w < WaveCount; w++)
            {
                var addedParentMeasure = new List<int>();
                var wave = Waves.Keys.ToList()[w];
                Wave_Started(wave, w);
                Generate_Header();

                var selectedMeasureIds = Waves[wave];
                var selectedMeasures = GetHeadersOfWave(Measures, wave, selectedMeasureIds);
                var measureCount = selectedMeasures.Count;

                for (int m = 0; m < measureCount; m++)
                {
                    var measure = selectedMeasures[m];
                    if (measure.ParentId > 1)
                    {
                        var childMeasureCount = selectedMeasures.Count(x => selectedMeasureIds.Contains(x.ID) && x.ParentId == measure.ParentId);
                        if (ShowTotalForMeasure && selectedMeasures.Any(x => x.ParentId == measure.ParentId && x.IsLastOfChildren)) childMeasureCount++;

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
                    if (Type == SatisfactoryType.Separate || Type == SatisfactoryType.ShowAll)
                    {
                        for (int i = 0; i < SourceCount; i++)
                        {
                            var source = Sources[i];
                            Record_Generating(source, Records, wave, measure, m);
                        }
                    }
                    if (Type == SatisfactoryType.Totaly || Type == SatisfactoryType.ShowAll)
                    {
                        Record_Generating(Records, wave, measure, m);
                    }
                    Measure_Over(measure, m);
                }
                Wave_Over(wave, w);
            }
            Report_Over();
        }
    }

    internal abstract class SatisfactoryBySourceGenerator<TSource, TRecord> :
       SatisfactoryGenerator<TSource, ReportMeasureHeaderModel, TRecord>
        where TSource : class
        where TRecord : class
    {

        protected SatisfactoryBySourceGenerator(IEnumerable<TSource> sources,
            IEnumerable<ReportMeasureHeaderModel> measures,
            IEnumerable<TRecord> records, Dictionary<Wave,
            IEnumerable<int>> waves,
            IEnumerable<BenchmarkModel> benchmarks)
            : base(SatisfactoryType.Totaly, sources, measures, records, waves, benchmarks)
        {
        }

        private List<ReportMeasureHeaderModel> GetHeadersOfWave(IEnumerable<ReportMeasureHeaderModel> headers, IEnumerable<int> selectedMeasureIds)
        {
            var wHeaders = headers.Where(x => (selectedMeasureIds == null || selectedMeasureIds.Contains(x.ID))).ToList();
            wHeaders.ForEach(x => x.IsLastOfChildren = false);

            var parentIds = wHeaders.Where(x => x.ParentId > 1).Select(x => x.ParentId).Distinct();
            parentIds.ForEach(delegate(int parentId)
            {
                var last = (wHeaders.LastOrDefault(child => child.ParentId == parentId));
                if (headers.Count(x => x.ParentId == parentId) == wHeaders.Count(x => x.ParentId == parentId)
                    && last != null)
                {
                    last.IsLastOfChildren = true;
                }
            });
            return wHeaders;
        }

        #region Export member

        protected abstract void Report_Started();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="wave">The wave.</param>
        /// <param name="index">The index.</param>
        protected abstract void Source_Started(TSource source, int index);
        /// <summary>
        /// 开始写Headers.
        /// </summary>
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
        /// <param name="measure">The measure.</param>
        /// <param name="index">The index.</param>
        /// <param name="isParent">if set to <c>true</c> [is parent].</param>
        /// <param name="children">The children.</param>
        protected abstract void Measure_Added(ReportMeasureHeaderModel measure, int index, bool isParent = false, int children = 0);

        protected abstract void Record_Generating(TSource source, List<TRecord> records, Wave wave,
            ReportMeasureHeaderModel measure,
            int index);

        protected abstract void Measure_Over(ReportMeasureHeaderModel measure, int index, bool isParent = false, int children = 0);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="wave">The wave.</param>
        /// <param name="index">The index.</param>
        protected abstract void Source_Over(TSource source, int index);

        protected abstract void Report_Over();

        #endregion

        private List<int> _allMeasureIds;
        private List<int> AllMeasureIds
        {
            get
            {
                if (_allMeasureIds == null)
                {
                    _allMeasureIds = new List<int>();
                    Waves.ForEach(w => _allMeasureIds.AddRange(w.Value.ToList()));
                }
                return _allMeasureIds.Distinct().ToList();
            }
        }
        public sealed override void Generate()
        {
            Report_Started();
            var selectedMeasureIds = AllMeasureIds;
            var selectedMeasures = GetHeadersOfWave(Measures, selectedMeasureIds);
            var measureCount = selectedMeasures.Count;
            for (int i = 0; i < SourceCount; i++)
            {
                Source_Started(Sources[i], i);
                var addedParentMeasure = new List<int>();

                Header_Started();
                //for (int w = 0; w < WaveCount; w++)
                //{
                //    Wave_Added(Waves.Keys.ToList()[w], w);
                //}
                Wave_Added(Waves.Keys.ToList()[0], 0);
                for (int m = 0; m < measureCount; m++)
                {
                    var measure = selectedMeasures[m];
                    if (measure.ParentId > 1)
                    {
                        var childMeasureCount = selectedMeasures.Count(x => selectedMeasureIds.Contains(x.ID) && x.ParentId == measure.ParentId);
                        if (ShowTotalForMeasure && selectedMeasures.Any(x => x.ParentId == measure.ParentId && x.IsLastOfChildren)) childMeasureCount++;

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
                        var source = Sources[i];
                        Record_Generating(source, Records, wave, measure, m);
                    }
                    Measure_Over(measure, m);
                }
                Source_Over(Sources[i], i);
            }
            Report_Over();
        }
    }
    #endregion

    #region Report Export Type - Pdf

    internal abstract class SatisfactoryByWavePdfGenerator<TSource, TRecord> :
        SatisfactoryByWaveGenerator<TSource, TRecord>
        where TSource : class
        where TRecord : class
    {
        protected SatisfactoryByWavePdfGenerator(SatisfactoryType type, IEnumerable<TSource> sources, IEnumerable<ReportMeasureHeaderModel> measures, IEnumerable<TRecord> records,
            Dictionary<Wave, IEnumerable<int>> waves, IEnumerable<BenchmarkModel> benchmarks)
            : base(type, sources, measures, records, waves, benchmarks)
        {

        }

        private ReportRowModel _headerRow;
        private ReportRowModel _measureRow;
        private ReportRowModel _totalMeasureRow;

        private Wave _currentWave;
        protected sealed override void Report_Started()
        {

        }

        protected sealed override void Wave_Started(Wave wave, int index)
        {
            _currentWave = wave;

            _headerRow = new ReportRowModel();

        }

        protected sealed override void Generate_Header()
        {
            _headerRow.Add("Measure", 2, 1);

        }

        protected sealed override void Measure_Added(ReportMeasureHeaderModel measure, int index, bool isParent = false, int children = 0)
        {
            _measureRow = new ReportRowModel();

            if (isParent)
            {
                _measureRow.Add(measure.ParentName, 1, children, true);
                _measureRow.Add(measure.Name);
            }
            else
            {
                if (measure.ParentId > 1)
                    _measureRow.Add(measure.Name, 1, 1);
                else
                    _measureRow.Add(measure.Name, 2, 1);
            }

            if (ShowTotalForMeasure && measure.IsLastOfChildren)
            {
                if (_totalMeasureRow == null)
                    _totalMeasureRow = new ReportRowModel();

                _totalMeasureRow.Add(ReportText.TotalColumnNameForSatisfactory, 1, 1);
            }
        }

        protected sealed override void Record_Generating(TSource source, List<TRecord> records, Wave wave, ReportMeasureHeaderModel measure, int index)
        {
            throw new NotImplementedException("Not show per source's Satisfactory report currently.");
        }

        protected sealed override void Record_Generating(List<TRecord> records, Wave wave, ReportMeasureHeaderModel measure,
            int index)
        {
            if (MergeSameLabel)
            {
                var mergeBenchmarks = Benchmarks.Select(x => x.LabelText).Distinct();
                foreach (string labelText in mergeBenchmarks)
                {
                    List<BenchmarkModel> benchmarks = Benchmarks.Where(b => b.LabelText == labelText).ToList();
                    var value = CalcValue(records, wave, measure, index, benchmarks);
                    _measureRow.Add(value);

                    if (ShowTotalForMeasure && measure.IsLastOfChildren)
                    {
                        value = CalcValue(records, wave, measure, index, benchmarks, true);
                        _totalMeasureRow.Add(value);
                    }
                }
            }
            else
            {
                foreach (BenchmarkModel benchmark in Benchmarks)
                {
                    var benchmarks = new List<BenchmarkModel>() { benchmark };
                    var value = CalcValue(records, wave, measure, index, benchmarks);
                    _measureRow.Add(value);

                    if (ShowTotalForMeasure && measure.IsLastOfChildren)
                    {
                        value = CalcValue(records, wave, measure, index, benchmarks, true);
                        _totalMeasureRow.Add(value);
                    }
                }
            }
        }

        protected override void Measure_Over(ReportMeasureHeaderModel measure, int index, bool isParent = false, int children = 0)
        {
            if (_headerRow != null)
            {
                //_headerRow.Add(ReportText.SatisfactoryColumnName);
                if (MergeSameLabel)
                {
                    var mergeBenchmarks = Benchmarks.Select(x => x.LabelText).Distinct();
                    mergeBenchmarks.ForEach(x =>
                        {
                            _headerRow.Add(x);
                        });
                }
                else
                {
                    Benchmarks.ForEach(x =>
                    {
                        _headerRow.Add(x.LabelText, "", x.Color);
                    });
                }

                AddRow(_currentWave, _headerRow);
                _headerRow = null;
            }
            AddRow(_currentWave, _measureRow);
            if (_totalMeasureRow != null && ShowTotalForMeasure)
            {
                AddRow(_currentWave, _totalMeasureRow);
                _totalMeasureRow = null;
            }
        }

        protected sealed override void Wave_Over(Wave wave, int index)
        {

        }

        protected sealed override void Report_Over()
        {

        }

        public sealed override string Filename
        {
            get { throw new NotImplementedException(); }
        }

        private void AddRow(Wave wave, ReportRowModel row)
        {
            if (!Reports.ContainsKey(wave))
                Reports.Add(wave, new List<ReportRowModel>());
            if (row != null)
                Reports[wave].Add(row);
        }

        private Dictionary<object, List<ReportRowModel>> _result;

        public sealed override Dictionary<object, List<ReportRowModel>> Reports
        {
            get { return _result ?? (_result = new Dictionary<object, List<ReportRowModel>>()); }
        }
    }

    internal abstract class SatisfactoryBySourcePdfGenerator<TSource, TRecord> :
        SatisfactoryBySourceGenerator<TSource, TRecord>
        where TSource : class
        where TRecord : class
    {
        private ISunnetLog _logger;

        private void Log(string format, params object[] args)
        {
            var s = string.Format(format, args);
            //_logger.Log(s);
        }

        protected SatisfactoryBySourcePdfGenerator(IEnumerable<TSource> sources,
            IEnumerable<ReportMeasureHeaderModel> measures,
            IEnumerable<TRecord> records,
            Dictionary<Wave, IEnumerable<int>> waves,
            IEnumerable<BenchmarkModel> benchmarks)
            : base(sources, measures, records, waves, benchmarks)
        {

        }

        private ReportRowModel _sourceRow;
        private ReportRowModel _waveHeader;
        private ReportRowModel _dataRow;
        private ReportRowModel _totalRow;
        private int sourceId;
        protected sealed override void Report_Started()
        {

        }

        protected override void Source_Started(TSource source, int index)
        {
            Log("Source_Started, Name:{0}", GetSourceName(source));
            sourceId = GetSourceID(source);
            _sourceRow = new ReportRowModel();
            var waves = "";
            foreach (var wave in Waves)
            {
                waves = waves + ((int)wave.Key).ToString() + ",";
            }
            waves = waves.TrimEnd(',');
            _sourceRow.Add(GetSourceName(source), waves, "");
            _waveHeader = new ReportRowModel();
        }

        protected sealed override void Header_Started()
        {
            Log("Header_Started");
            _waveHeader.Add("Measure", 2, 1);

            AddRow(sourceId, _sourceRow);
            AddRow(sourceId, _waveHeader);
        }

        protected override void Wave_Added(Wave wave, int index)
        {
            Benchmarks.ForEach(x =>
            {
                _waveHeader.Add(x.LabelText, x.ID.ToString(), x.Color);
            });
            //Benchmarks.OrderBy(e => e.LabelText).ForEach(x =>
            //{
            //    _waveHeader.Add(x.LabelText, x.ID.ToString(), x.Color);
            //});
        }

        protected sealed override void Measure_Added(ReportMeasureHeaderModel measure, int index, bool isParent = false, int children = 0)
        {
            _dataRow = new ReportRowModel();
            Log("Measure_Added, ID:{0}", measure.ID);
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
            if (measure.IsLastOfChildren && ShowTotalForMeasure)
            {
                if (_totalRow == null)
                    _totalRow = new ReportRowModel();
                _totalRow.Add(ReportText.TotalColumnNameForSatisfactory);
            }
        }

        protected override void Record_Generating(TSource source, List<TRecord> records, Wave wave, ReportMeasureHeaderModel measure, int index)
        {
            Log("Record_Generating, Name:{0},Measure:{1}", GetSourceName(source), measure.Name);

            foreach (BenchmarkModel benchmark in Benchmarks)
            {
                var benchmarks = new List<BenchmarkModel>() { benchmark };
                var value = CalcValue(records, wave, measure, index, benchmarks);
                _dataRow.Add(value, benchmark.ID.ToString(), ((int)wave).ToString());

                if (ShowTotalForMeasure && measure.IsLastOfChildren)
                {
                    value = CalcValue(records, wave, measure, index, benchmarks, true);
                    _totalRow.Add(value, benchmark.ID.ToString(), ((int)wave).ToString());
                }
            }
        }

        protected override void Measure_Over(ReportMeasureHeaderModel measure, int index, bool isParent = false, int children = 0)
        {
            AddRow(sourceId, _dataRow);
            _dataRow = null;
            AddRow(sourceId, _totalRow);
            _totalRow = null;
        }

        protected override void Source_Over(TSource source, int index)
        {
            _sourceRow = null;
            _waveHeader = null;
            _dataRow = null;
            _totalRow = null;
        }

        protected sealed override void Report_Over()
        {

        }

        public sealed override string Filename
        {
            get { throw new NotImplementedException(); }
        }

        private void AddRow(int sourceId, ReportRowModel row)
        {
            if (!Reports.ContainsKey(sourceId))
                Reports.Add(sourceId, new List<ReportRowModel>());
            if (row != null && row.Cells.Any())
                Reports[sourceId].Add(row);
        }

        private Dictionary<object, List<ReportRowModel>> _result;
        public sealed override Dictionary<object, List<ReportRowModel>> Reports
        {
            get { return _result ?? (_result = new Dictionary<object, List<ReportRowModel>>()); }
        }
    }

    #endregion

    #region Calculator

    internal static class SatifactoryByWaveCalculator
    {
        internal static string CalcValue(CpallsCommunityModel source, bool ShowTotalForMeasure,
            IEnumerable<ReportMeasureHeaderModel> Measures, List<StudentRecordModel> records,
            Wave wave, ReportMeasureHeaderModel measure, int index, bool total = false)
        {
            return CalcValue(ShowTotalForMeasure, Measures, records, wave, measure, index, total);
        }

        internal static string CalcValue(CpallsSchoolModel source, bool ShowTotalForMeasure,
            IEnumerable<ReportMeasureHeaderModel> Measures, List<StudentRecordModel> records,
            Wave wave, ReportMeasureHeaderModel measure, int index, bool total = false)
        {
            var schoolRecords = records.Where(x => x.SchoolId == source.ID).ToList();
            return CalcValue(ShowTotalForMeasure, Measures, schoolRecords, wave, measure, index, total);
        }

        internal static string CalcValue(CpallsClassModel source, bool ShowTotalForMeasure,
            IEnumerable<ReportMeasureHeaderModel> Measures, List<StudentRecordModel> records,
            Wave wave, ReportMeasureHeaderModel measure, int index, bool total = false)
        {
            var classRecords = records.Where(x => source.StudentIds.Contains(x.StudentId)).ToList();
            return CalcValue(ShowTotalForMeasure, Measures, classRecords, wave, measure, index, total);
        }
        //因为是Practice的growth，所以此处显示所有的records，不需要source的过滤
        internal static string CalcValue(PracticeAssessmentModel source, bool ShowTotalForMeasure,
            IEnumerable<ReportMeasureHeaderModel> Measures, List<StudentRecordModel> records,
            Wave wave, ReportMeasureHeaderModel measure, int index, bool total = false)
        {
            //var classRecords = records.Where(x => source.StudentIds.Contains(x.StudentId)).ToList();
            return CalcValue(ShowTotalForMeasure, Measures, records, wave, measure, index, total);
        }

        internal static string CalcValue(bool ShowTotalForMeasure, IEnumerable<ReportMeasureHeaderModel> Measures, List<SchoolRecordModel> records, Wave wave, ReportMeasureHeaderModel measure, int index, bool total = false)
        {
            var measureId = measure.ID;
            if (total == false)
            {
                if (measure.TotalScored)
                {
                    var measureRecords = records.FindAll(x => x.MeasureId == measureId && x.Wave == wave);
                    if (measureRecords.Any())
                    {
                        var studentsCount = measureRecords.Sum(x => x.Count);
                        var satisfied = measureRecords.Sum(x => x.Satisfied) + 0m;
                        if (studentsCount > 0)
                            return (satisfied / studentsCount).ToPercentage(0);
                        return ReportText.No_Benchmark;
                    }
                    else
                    {
                        return ReportText.No_Benchmark;
                    }
                }
                else
                {
                    return ReportText.No_TotalScored;
                }
            }
            else if (ShowTotalForMeasure && measure.IsLastOfChildren)
            {
                if (measure.ParentScored)
                {
                    measureId = measure.ParentId;
                    var measureRecords = records.FindAll(x => x.MeasureId == measureId && x.Wave == wave);
                    if (measureRecords.Any())
                    {
                        var studentsCount = measureRecords.Sum(x => x.Count);
                        var satisfied = measureRecords.Sum(x => x.Satisfied) + 0m;
                        if (studentsCount > 0)
                            return (satisfied / studentsCount).ToPercentage(0);
                        return ReportText.No_Benchmark;
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
            return ReportText.No_Record;
        }

        internal static string CalcValue(bool ShowTotalForMeasure, IEnumerable<ReportMeasureHeaderModel> Measures, List<StudentRecordModel> records, Wave wave, ReportMeasureHeaderModel measure, int index, bool total = false)
        {
            var measureId = measure.ID;
            if (total == false)
            {
                if (measure.TotalScored)
                {
                    var recordOfSatisfied =
                        records.Count(
                            x => x.MeasureId == measureId && x.Wave == wave && x.LowerScore >= 0 && x.HigherScore >= 0
                                && x.Goal >= x.LowerScore && x.Goal <= x.HigherScore);
                    var recordOfStus = records.Count(x => x.MeasureId == measureId && x.Wave == wave && x.Goal >= 0) + 0m;
                    if (recordOfStus > 0)
                        return (recordOfSatisfied / recordOfStus).ToPercentage(0);
                    return ReportText.No_Benchmark;
                }
                else
                    return ReportText.No_TotalScored;
            }
            else if (ShowTotalForMeasure && measure.IsLastOfChildren)
            {
                if (measure.ParentScored)
                {
                    measureId = measure.ParentId;
                    var recordOfSatisfied =
                        records.Count(
                            x => x.MeasureId == measureId && x.Wave == wave && x.LowerScore >= 0 && x.HigherScore >= 0
                                && x.Goal >= x.LowerScore && x.Goal <= x.HigherScore
                            &&
                                  records.Any(
                                      child =>
                                          child.ParentId == measureId && child.StudentId == x.StudentId &&
                                          child.Wave == x.Wave
                                          && Measures.Any(z => z.ID == child.MeasureId && z.TotalScored)));
                    var recordOfStus = records.Count(
                        parent => parent.MeasureId == measureId && parent.Wave == wave && parent.Goal >= 0
                                  &&
                                  records.Any(
                                      child =>
                                          child.ParentId == measureId && child.StudentId == parent.StudentId &&
                                          child.Wave == parent.Wave
                                          && Measures.Any(z => z.ID == child.MeasureId && z.TotalScored))) + 0m;
                    if (recordOfStus > 0)
                        return (recordOfSatisfied / recordOfStus).ToPercentage(0);
                    return ReportText.No_Benchmark;
                }
                else
                    return ReportText.No_TotalScored;
            }
            return ReportText.No_Record;
        }

        internal static string CalcValue(bool ShowTotalForMeasure, IEnumerable<ReportMeasureHeaderModel> Measures,
            List<SchoolRecordModel> records, Wave wave, ReportMeasureHeaderModel measure,
            int index, List<BenchmarkModel> benchmarks, bool total = false)
        {
            var measureId = measure.ID;
            var benchmarkIds = benchmarks.Select(b => b.ID).ToList();
            if (total == false)
            {
                if (measure.TotalScored)
                {
                    var measureRecords = records.FindAll(x => x.MeasureId == measureId && x.Wave == wave);
                    if (measureRecords.Any())
                    {
                        var studentsCount = measureRecords.Sum(x => x.Count);
                        var satisfied = measureRecords.Where(x => benchmarkIds.Contains(x.BenchmarkId)).Sum(x => x.Satisfied) + 0m;
                        if (studentsCount > 0)
                            return (satisfied / studentsCount).ToPercentage(0);
                        return ReportText.No_Benchmark;
                    }
                    else
                    {
                        return ReportText.No_Benchmark;
                    }
                }
                else
                {
                    return ReportText.No_TotalScored;
                }
            }
            else if (ShowTotalForMeasure && measure.IsLastOfChildren)
            {
                if (measure.ParentScored)
                {
                    measureId = measure.ParentId;
                    var measureRecords = records.FindAll(x => x.MeasureId == measureId && x.Wave == wave);
                    if (measureRecords.Any())
                    {
                        var studentsCount = measureRecords.Sum(x => x.Count);
                        var satisfied = measureRecords.Where(x => benchmarkIds.Contains(x.BenchmarkId)).Sum(x => x.Satisfied) + 0m;
                        if (studentsCount > 0)
                            return (satisfied / studentsCount).ToPercentage(0);
                        return ReportText.No_Benchmark;
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
            return ReportText.No_Record;
        }

        internal static string CalcValue(bool ShowTotalForMeasure, IEnumerable<ReportMeasureHeaderModel> Measures,
            List<StudentRecordModel> records, Wave wave, ReportMeasureHeaderModel measure, int index,
            List<BenchmarkModel> benchmarks, bool total = false)
        {
            var measureId = measure.ID;
            var benchmarkIds = benchmarks.Select(b => b.ID).ToList();
            if (total == false)
            {
                if (measure.TotalScored)
                {
                    var recordOfSatisfied =
                        records.Count(
                            x => x.MeasureId == measureId && x.Wave == wave && benchmarkIds.Contains(x.BenchmarkId) && x.Goal >= 0
                                && x.LowerScore >= 0 && x.HigherScore >= 0 && x.Goal >= x.LowerScore && x.Goal <= x.HigherScore);
                    var recordOfStus = records.Count(x => x.MeasureId == measureId && x.Wave == wave && x.Goal >= 0) + 0m;
                    if (recordOfStus > 0)
                        return (recordOfSatisfied / recordOfStus).ToPercentage(0);
                    return ReportText.No_Benchmark;
                }
                else
                    return ReportText.No_TotalScored;
            }
            else if (ShowTotalForMeasure && measure.IsLastOfChildren)
            {
                if (measure.ParentScored)
                {
                    measureId = measure.ParentId;
                    var recordOfSatisfied =
                        records.Count(
                            x => x.MeasureId == measureId && x.Wave == wave && benchmarkIds.Contains(x.BenchmarkId) && x.Goal >= 0
                                && x.LowerScore >= 0 && x.HigherScore >= 0 && x.Goal >= x.LowerScore && x.Goal <= x.HigherScore
                                 &&
                                  records.Any(
                                      child =>
                                          child.ParentId == measureId && child.StudentId == x.StudentId &&
                                          child.Wave == x.Wave && child.Goal >= 0
                                          && Measures.Any(z => z.ID == child.MeasureId && z.TotalScored))
                                          );
                    var recordOfStus = records.Count(
                        parent => parent.MeasureId == measureId && parent.Wave == wave && parent.Goal >= 0
                                  &&
                                  records.Any(
                                      child =>
                                          child.ParentId == measureId && child.StudentId == parent.StudentId &&
                                          child.Wave == parent.Wave && child.Goal >= 0
                                          && Measures.Any(z => z.ID == child.MeasureId && z.TotalScored))
                                          ) + 0m;
                    if (recordOfStus > 0)
                        return (recordOfSatisfied / recordOfStus).ToPercentage(0);
                    return ReportText.No_Benchmark;
                }
                else
                    return ReportText.No_TotalScored;
            }
            return ReportText.No_Record;
        }

    }

    #endregion
}
