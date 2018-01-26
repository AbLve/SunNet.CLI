using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
/**************************************************************************
 * Developer: 		JackZhang
 * Computer:		JACKZ
 * Domain:			Jackz
 * CreatedOn:		2014/11/8 1:11:33
 * Description:		Please input class summary
 * Version History:	Created,2014/11/8 1:11:33
 * 
 * 
 **************************************************************************/
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Drawing;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using LinqKit;
using Microsoft.Office.Interop.Excel;
using StructureMap;
using Sunnet.Cli.Business.Ade;
using Sunnet.Cli.Business.Common;
using Sunnet.Cli.Business.Cpalls.Models;
using Sunnet.Cli.Business.Cpalls.Models.Report;
using Sunnet.Cli.Core.Ade.Entities;
using Sunnet.Cli.Core.Cpalls;
using Sunnet.Cli.Core.Cpalls.Models.Report;
using Sunnet.Framework;
using Sunnet.Framework.Excel;
using Sunnet.Framework.Extensions;
using Sunnet.Framework.Log;
using Sunnet.Cli.Business.Ade.Models;

namespace Sunnet.Cli.Business.Cpalls.Growth
{
    #region Definition


    internal abstract class AverageGenerator<TSource, THeader, TRecord> : IReportGenerator
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
        protected List<BenchmarkModel> Benchmarks { get; private set; }

        protected List<int> AllSelectedMeasureIds
        {
            get
            {
                if (_allSelectedMeasureIds == null)
                {
                    _allSelectedMeasureIds = new List<int>();
                    foreach (var wave in Waves)
                    {
                        _allSelectedMeasureIds.AddRange(wave.Value);
                    }

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

        protected int SourceCount
        {
            get
            {
                if (_sourceCount == 0)
                    _sourceCount = Sources.Count;
                return _sourceCount;
            }
        }

        protected AverageGenerator(IEnumerable<TSource> sources,
            IEnumerable<THeader> measures,
            IEnumerable<TRecord> records,
            Dictionary<Wave, IEnumerable<int>> waves,
            IEnumerable<BenchmarkModel> benchmarks)
        {
            Sources = sources.ToList();
            Records = records.ToList();
            Measures = measures.ToList();

            Waves = new Dictionary<Wave, IEnumerable<int>>();
            waves.ForEach(wave =>
            {
                if (wave.Value != null && wave.Value.Any())
                    Waves.Add(wave.Key, wave.Value);
            });
            Benchmarks = benchmarks.ToList();
        }

        /// <summary>
        /// 是否显示每个Measure的总分.
        /// </summary>
        public bool ShowTotalScore { get; set; }

        /// <summary>
        /// 是否为父级Measure生成Total列(行).
        /// </summary>
        public bool ShowTotalForMeasure { get; set; }

        /// <summary>
        /// 在每个Wave最后增加一行显示所有对象对每个Measure的平均分.
        /// </summary>
        public bool ShowAveragePerSource { get; set; }

        /// <summary>
        /// Student Report Could choose to print comment
        /// </summary>
        /// Author : JackZhang
        /// Date   : 7/10/2015 17:26:11
        public bool PrintComment { get; set; }

        #region Host Members
        /// <summary>
        /// 报表标题：例如 Wave，School，Class，Student
        /// </summary>
        protected abstract string GetTitle();

        /// <summary>
        /// 获取显示名
        /// </summary>
        /// <param name="source">The source.</param>
        protected virtual string GetSourceName(TSource source)
        {
            throw new NotImplementedException("Method is not Implemented");
        }

        protected virtual int GetSourceID(TSource source)
        {
            throw new NotImplementedException("Method is not Implemented");
        }

        protected virtual string CalcValue(TSource source, List<TRecord> records, Wave wave,
            ReportMeasureHeaderModel measure, int index, bool total = false)
        {
            throw new NotImplementedException("Method is not Implemented");
        }
        protected virtual string GetComment(TSource source, List<TRecord> records, Wave wave,
            ReportMeasureHeaderModel measure, int index, bool total = false)
        {
            throw new NotImplementedException("Method is not Implemented");
        }
        protected virtual string CalcValue(List<TRecord> records, Wave wave, ReportMeasureHeaderModel measure, int index,
            bool total = false)
        {
            throw new NotImplementedException("Method is not Implemented");
        }

        #endregion

        public abstract void Generate();
        public abstract string Filename { get; }
        public abstract Dictionary<object, List<ReportRowModel>> Reports { get; }
    }
    #endregion

    #region Report Type

    internal abstract class AverageByWaveGenerator<TSource, TRecord> : AverageGenerator<TSource, ReportMeasureHeaderModel, TRecord>
        where TSource : class
        where TRecord : class
    {
        protected AverageByWaveGenerator(IEnumerable<TSource> sources, IEnumerable<ReportMeasureHeaderModel> measures, IEnumerable<TRecord> records, Dictionary<Wave, IEnumerable<int>> waves, IEnumerable<BenchmarkModel> benchmarks)
            : base(sources, measures, records, waves, benchmarks)
        {

        }
        private List<ReportMeasureHeaderModel> GetHeadersOfWave(IEnumerable<ReportMeasureHeaderModel> headers, Wave wave, IEnumerable<int> selectedMeasureIds = null)
        {
            var wHeaders = headers.Where(x => (selectedMeasureIds == null || selectedMeasureIds.Contains(x.ID))).ToList();

            wHeaders.ForEach(x => x.IsLastOfChildren = false);

            var parentIds = wHeaders.Where(x => x.ParentId > 1).Select(x => x.ParentId).Distinct();
            parentIds.ForEach(delegate (int parentId)
            {
                var last = (wHeaders.LastOrDefault(child => child.ParentId == parentId));
                if (headers.Count(x => x.ParentId == parentId)
                    == wHeaders.Count(x => x.ParentId == parentId)
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
        /// 每个Wave开始时执行.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="wave">The wave.</param>
        protected abstract void Wave_Started(Wave wave, int index);

        /// <summary>
        /// 新的Measure Headers.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="wave">The wave.</param>
        /// <param name="isParent">if set to <c>true</c> [is parent].</param>
        protected abstract void Measure_Added(ReportMeasureHeaderModel measure, int index, bool isParent = false, int children = 0);

        /// <summary>
        /// 表格头输出完毕.
        /// </summary>
        protected abstract void Header_Over();

        protected abstract void Source_Started(TSource source, int index);

        protected abstract void Record_Generating(TSource source, List<TRecord> records, Wave wave, ReportMeasureHeaderModel measure, int index);

        protected abstract void Source_Over(TSource source, int index);

        /// <summary>
        /// 每个Wave结束时执行.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="wave">The wave.</param>
        protected abstract void Wave_Over(Wave wave, int index);

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
        protected abstract void SetCellStyle(object cell, TSource source, List<TRecord> records, Wave wave,
            ReportMeasureHeaderModel measure, int index, bool total = false);
        #endregion

        public sealed override void Generate()
        {
            Report_Started();
            for (int w = 0; w < WaveCount; w++)
            {
                var addedParentMeasure = new List<int>();
                var wave = Waves.Keys.ToList()[w];
                Wave_Started(wave, w);

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
                }
                Header_Over();

                for (int s = 0; s < SourceCount; s++)
                {
                    var source = Sources[s];
                    Source_Started(source, s);
                    for (int k = 0; k < measureCount; k++)
                    {
                        var measure = selectedMeasures[k];
                        Record_Generating(source, Records, wave, measure, k);
                    }
                    Source_Over(source, s);
                }
                Wave_Over(wave, 2);
            }
            Report_Over();
        }
    }

    internal abstract class AverageBySourceGenerator<TSource, TRecord> : AverageGenerator<TSource, ReportMeasureHeaderModel, TRecord>
        where TSource : class
        where TRecord : class
    {
        private ISunnetLog _logger;

        private void Log(string format, params object[] args)
        {
            var s = string.Format(format, args);
            //_logger.Info(s);
        }
        protected AverageBySourceGenerator(IEnumerable<TSource> sources, IEnumerable<ReportMeasureHeaderModel> measures, IEnumerable<TRecord> records, Dictionary<Wave, IEnumerable<int>> waves, IEnumerable<BenchmarkModel> benchmarks)
            : base(sources, measures, records, waves, benchmarks)
        {
            _logger = ObjectFactory.GetInstance<ISunnetLog>();
        }
        private List<ReportMeasureHeaderModel> GetHeadersOfWaves(IEnumerable<ReportMeasureHeaderModel> headers, IEnumerable<Wave> waves, IEnumerable<int> selectedMeasureIds = null)
        {
            var ws = waves.Select(x => ((int)x).ToString());
            var wHeaders = headers.Where(x => x.ApplyToWave.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries).ToList().Intersect(ws).Any()
                && (selectedMeasureIds == null || selectedMeasureIds.Contains(x.ID))).ToList();

            wHeaders.ForEach(x => x.IsLastOfChildren = false);

            var parentIds = wHeaders.Where(x => x.ParentId > 1).Select(x => x.ParentId).Distinct();
            parentIds.ForEach(delegate (int parentId)
            {
                var last = (wHeaders.LastOrDefault(child => child.ParentId == parentId));
                if (headers.Count(x => x.ParentId == parentId) == wHeaders.Count(x => x.ParentId == parentId) && last != null)
                {
                    last.IsLastOfChildren = true;
                }
            });
            return wHeaders;
        }
        #region Export member

        protected abstract void Report_Started();
        protected abstract void Source_Started(TSource source, int index);
        protected abstract void Header_Started();
        /// <summary>
        /// 
        /// </summary>
        /// <param name="wave">The wave.</param>
        /// <param name="index">The index.</param>
        protected abstract void Wave_Added(Wave wave, int index);
        /// <summary>
        /// 表格头输出完毕.
        /// </summary>
        protected abstract void Header_Over();

        protected abstract void Row_Started();
        /// <summary>
        /// 新的Measure Headers.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="wave">The wave.</param>
        /// <param name="isParent">if set to <c>true</c> [is parent].</param>
        protected abstract void Measure_Added(ReportMeasureHeaderModel measure, int index, bool isParent = false, int children = 0);


        protected abstract void Record_Generating(TSource source, List<TRecord> records, Wave wave, ReportMeasureHeaderModel measure, int index);
        protected abstract void Row_Over();

        protected abstract void Source_Over(TSource source, int index);

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
        protected abstract void SetCellStyle(object cell, TSource source, List<TRecord> records, Wave wave,
            ReportMeasureHeaderModel measure, int index, bool total = false);
        #endregion

        public sealed override void Generate()
        {
            Report_Started();

            var selectedMeasures = GetHeadersOfWaves(Measures, Waves.Keys, AllSelectedMeasureIds);
            var measureCount = selectedMeasures.Count;
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            Log("start:{0}", stopwatch.Elapsed.TotalMilliseconds);
            for (int s = 0; s < SourceCount; s++)
            {
                var source = Sources[s];
                Source_Started(source, s);

                var addedParentMeasure = new List<int>();

                // 开始写Header
                Header_Started();
                for (int w = 0; w < WaveCount; w++)
                {
                    var wave = Waves.Keys.ToList()[w];
                    Wave_Added(wave, w);
                }
                Header_Over();

                for (int m = 0; m < measureCount; m++)
                {
                    Row_Started();
                    var measure = selectedMeasures[m];
                    if (measure.ParentId > 1)
                    {
                        var childMeasureCount = selectedMeasures.Count(x => AllSelectedMeasureIds.Contains(x.ID) && x.ParentId == measure.ParentId);
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
                        Record_Generating(source, Records, wave, measure, m);
                        Log("Wave:{0} :{1}", wave, stopwatch.Elapsed.TotalMilliseconds);
                    }
                    Row_Over();
                }

                Source_Over(source, s);
                Log("Source_Over:{0} :{1}", GetSourceName(source), stopwatch.Elapsed.TotalMilliseconds);
            }
            Report_Over();
        }
    }

    #endregion

}
