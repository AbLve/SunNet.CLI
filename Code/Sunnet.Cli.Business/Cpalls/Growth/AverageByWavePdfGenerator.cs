using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
/**************************************************************************
 * Developer: 		JackZhang
 * Computer:		JACKZ
 * Domain:			Jackz
 * CreatedOn:		2014/11/13 2:14:59
 * Description:		Please input class summary
 * Version History:	Created,2014/11/13 2:14:59
 * 
 * 
 **************************************************************************/
using LinqKit;
using StructureMap;
using Sunnet.Cli.Business.Ade;
using Sunnet.Cli.Business.Common;
using Sunnet.Cli.Business.Cpalls.Models;
using Sunnet.Cli.Business.Cpalls.Models.Report;
using Sunnet.Cli.Core.Ade.Entities;
using Sunnet.Cli.Core.Cpalls;
using Sunnet.Cli.Core.Cpalls.Models.Report;
using Sunnet.Framework.Extensions;
using Sunnet.Framework.Log;
using Sunnet.Cli.Business.Ade.Models;

namespace Sunnet.Cli.Business.Cpalls.Growth
{
    #region Report Export Type - Pdf
    internal abstract class AverageByWavePdfGenerator<TSource, TRecord> : AverageByWaveGenerator<TSource, TRecord>
        where TSource : class
        where TRecord : class
    {
        private Dictionary<object, List<ReportRowModel>> _result;
        private ISunnetLog _logger;

        protected void Log(string format, params object[] args)
        {
            var s = string.Format(format, args);
            //_logger.Info(s);
        }

        protected AverageByWavePdfGenerator(IEnumerable<TSource> sources, IEnumerable<ReportMeasureHeaderModel> measures, IEnumerable<TRecord> records, Dictionary<Wave, IEnumerable<int>> waves, IEnumerable<BenchmarkModel> benchmarks)
            : base(sources, measures, records, waves, benchmarks)
        {
            _logger = ObjectFactory.GetInstance<ISunnetLog>();
        }


        private ReportRowModel _parentRow;
        private ReportRowModel _measureRow;
        private ReportRowModel _totalScoreRow;
        private ReportRowModel _dataRow;
        private Dictionary<int, List<decimal>> _averageScoresOfSources;

        private Wave _currentWave;

        protected override void Report_Started()
        {
        }

        protected sealed override void Wave_Started(Wave wave, int index)
        {
            Log("Wave_Started, index:{0},Wave:{1}", index, wave);
            _averageScoresOfSources = new Dictionary<int, List<decimal>>();

            _parentRow = new ReportRowModel();
            _parentRow.Add("Wave " + wave.ToDescription(), 1, 2);

            _measureRow = new ReportRowModel();
            _totalScoreRow = new ReportRowModel();
            _totalScoreRow.Add(ReportText.TotalScore);
            _currentWave = wave;
            _dataRow = new ReportRowModel();
        }

        protected sealed override void Measure_Added(ReportMeasureHeaderModel measure, int index, bool isParent = false, int children = 0)
        {
            Log("Measure_Added, ID:{0}", measure.ID);
            if (isParent)
            {
                _parentRow.Add(measure.ParentName, children, 1, true);
                _measureRow.Add(measure.Name);

            }
            else
            {
                if (measure.ParentId > 1)
                    _measureRow.Add(measure.Name, 1, 1);
                else
                    _parentRow.Add(measure.Name, 1, 2);

            }
            if (measure.TotalScored)
                _totalScoreRow.Add(measure.TotalScore.ToPrecisionString(2));
            else
                _totalScoreRow.Add(ReportText.No_Record);

            if (measure.IsLastOfChildren)
            {
                var parentTotal = Measures.Where(x => x.ParentId == measure.ParentId).Sum(x => x.TotalScore);
                _totalScoreRow.Add(parentTotal.ToPrecisionString(2));

                if (ShowTotalForMeasure)
                {
                    _measureRow.Add(ReportText.TotalColumnName, 1, 1);
                }
            }
        }

        protected override void Header_Over()
        {
            if (_parentRow != null)
            {
                AddRow(_currentWave, _parentRow);
                _parentRow = null;
            }

            if (_measureRow != null)
            {
                AddRow(_currentWave, _measureRow);
                _measureRow = null;
            }

            if (ShowTotalScore)
            {
                if (_totalScoreRow != null)
                {
                    AddRow(_currentWave, _totalScoreRow);
                    _totalScoreRow = null;
                }
            }
        }

        protected sealed override void Source_Started(TSource source, int index)
        {
            Log("Source_Started, Name:{0}", GetSourceName(source));
            if (_dataRow == null)
                _dataRow = new ReportRowModel();
            _dataRow.Add(GetSourceName(source));
        }
        protected sealed override void Record_Generating(TSource source, List<TRecord> records, Wave wave, ReportMeasureHeaderModel measure, int index)
        {
            Log("Record_Generating, Name:{0},Measure:{1}", GetSourceName(source), measure.Name);
            var value = CalcValue(source, records, wave, measure, index);
            _dataRow.Add(value);
            RecordAverageScore(measure.ID, value);
            SetCellStyle(_dataRow.Cells.Last(), source, records, wave, measure, index);

            if (ShowTotalForMeasure)
            {
                if (measure.IsLastOfChildren)
                {
                    value = CalcValue(source, records, wave, measure, index, true);
                    Log("Total:{0}", value);
                    RecordAverageScore(measure.ParentId, value);
                    _dataRow.Add(value);
                    SetCellStyle(_dataRow.Cells.Last(), source, records, wave, measure, index, true);
                }
            }
        }
        protected sealed override void Source_Over(TSource source, int index)
        {
            Log("Source_Over, Name:{0}", GetSourceName(source));
            if (_parentRow != null)
            {
                AddRow(_currentWave, _parentRow);
                _parentRow = null;
            }

            if (_measureRow != null)
            {
                AddRow(_currentWave, _measureRow);
                _measureRow = null;
            }
            if (_dataRow != null)
            {
                AddRow(_currentWave, _dataRow);
                _dataRow = null;
            }

        }

        protected sealed override void Wave_Over(Wave wave, int index)
        {
            Log("Wave_Over, index:{0},Wave:{1}", index, wave);
            if (ShowAveragePerSource)
            {
                var averageRow = new ReportRowModel();
                averageRow.Add(ReportText.AveragePerSource);
                averageRow.SetBGColorofLastCell(ReportTheme.AveragePerSource_ClassName);
                var waveMeasures = Waves[wave];
                Measures.Where(x => waveMeasures.Contains(x.ID)).ForEach(mea =>
                {
                    if (_averageScoresOfSources.ContainsKey(mea.ID) && _averageScoresOfSources[mea.ID].Any())
                    {
                        averageRow.Add((_averageScoresOfSources[mea.ID].Sum() / _averageScoresOfSources[mea.ID].Count).ToPrecisionString(2));
                    }
                    else
                    {
                        averageRow.Add(ReportText.No_Record);
                    }
                    if (mea.IsLastOfChildren)
                    {
                        if (_averageScoresOfSources.ContainsKey(mea.ParentId) && _averageScoresOfSources[mea.ParentId].Any())
                        {
                            averageRow.Add((_averageScoresOfSources[mea.ParentId].Sum() / _averageScoresOfSources[mea.ParentId].Count).ToPrecisionString(2));
                        }
                        else
                        {
                            averageRow.Add(ReportText.No_Record);
                        }
                    }
                });

                AddRow(_currentWave, averageRow);
            }
        }
        protected override void Report_Over()
        {
        }

        private void AddRow(Wave wave, ReportRowModel row)
        {
            if (!Reports.ContainsKey(wave))
                Reports.Add(wave, new List<ReportRowModel>());
            Reports[wave].Add(row);
        }

        private void RecordAverageScore(int measureId, string average)
        {
            decimal score;
            if (decimal.TryParse(average, out score) || average == ReportText.PercentileRankNA)
            {
                RecordAverageScore(measureId, score);
            }
        }

        private void RecordAverageScore(int measureId, decimal average)
        {
            if (!_averageScoresOfSources.ContainsKey(measureId))
            {
                _averageScoresOfSources.Add(measureId, new List<decimal>());
            }
            _averageScoresOfSources[measureId].Add(average);
        }

        public override string Filename
        {
            get { throw new NotImplementedException(); }
        }
        public override Dictionary<object, List<ReportRowModel>> Reports
        {
            get { return _result ?? (_result = new Dictionary<object, List<ReportRowModel>>()); }
        }
    }

    #endregion

    #region report host - Community(schools)
    internal class AverageByWavePdfCommunityGenerator : AverageByWavePdfGenerator<CpallsSchoolModel, SchoolRecordModel>
    {
        public AverageByWavePdfCommunityGenerator(IEnumerable<CpallsSchoolModel> sources, IEnumerable<ReportMeasureHeaderModel> measures
            , IEnumerable<SchoolRecordModel> records, Dictionary<Wave, IEnumerable<int>> waves, IEnumerable<BenchmarkModel> benchmarks)
            : base(sources, measures, records, waves, benchmarks)
        {

        }

        protected override string GetTitle()
        {
            return "School";
        }

        protected override string GetSourceName(CpallsSchoolModel source)
        {
            return source.Name;
        }

        protected override string CalcValue(CpallsSchoolModel source, List<SchoolRecordModel> records, Wave wave,
            ReportMeasureHeaderModel measure, int index, bool total = false)
        {
            if (total == false)
            {
                if (!Waves.Keys.Contains(wave) || !Waves[wave].Contains(measure.ID))
                {
                    return ReportText.No_Choosed;
                }
                if (measure.TotalScored)
                {
                    var recordOfSchool = records.FirstOrDefault(x => x.Wave == wave && x.SchoolId == source.ID && x.MeasureId == measure.ID);
                    if (recordOfSchool != null)
                        return recordOfSchool.AverageForCalc.ToPrecisionString(2);
                    return ReportText.No_Record;
                }
                else
                {
                    return ReportText.No_TotalScored;
                }
            }
            else
            {
                if (!Waves.Keys.Contains(wave) || !Waves[wave].Contains(measure.ParentId))
                {
                    return ReportText.No_Choosed;
                }
                if (measure.ParentScored)
                {
                    var recordsOfSchool = records.Where(x => x.SchoolId == source.ID && x.Wave == wave &&
                                                                         Measures.Any(m => m.ParentId == measure.ParentId
                                                                             && m.TotalScored
                                                                             && m.ID == x.MeasureId)
                                                                             && Waves[wave].Contains(x.MeasureId)
                                                                             ).ToList();
                    if (recordsOfSchool.Any())
                        return recordsOfSchool.Sum(m => m.AverageForCalc).ToPrecisionString(2);
                    return ReportText.No_Record;
                }
                else
                {
                    return ReportText.No_TotalScored;
                }
            }
        }

        protected override void SetCellStyle(object cell, CpallsSchoolModel source, List<SchoolRecordModel> records, Wave wave,
            ReportMeasureHeaderModel measure, int index, bool total = false)
        {
            // school view: do not need to set styles
            //throw new NotImplementedException();
        }

        protected override int GetSourceID(CpallsSchoolModel source)
        {
            return source.ID;
        }
    }

    #endregion

    #region report host - Community(schools) Percentile Rank
    internal class AverageByWavePdfCommunityPercentileRankGenerator : AverageByWavePdfGenerator<CpallsSchoolModel, SchoolPercentileRankModel>
    {
        public AverageByWavePdfCommunityPercentileRankGenerator(IEnumerable<CpallsSchoolModel> sources, IEnumerable<ReportMeasureHeaderModel> measures
            , IEnumerable<SchoolPercentileRankModel> records, Dictionary<Wave, IEnumerable<int>> waves, IEnumerable<BenchmarkModel> benchmarks)
            : base(sources, measures, records, waves, benchmarks)
        {

        }

        protected override string GetTitle()
        {
            return "School";
        }

        protected override string GetSourceName(CpallsSchoolModel source)
        {
            return source.Name;
        }

        protected override string CalcValue(CpallsSchoolModel source, List<SchoolPercentileRankModel> records, Wave wave,
            ReportMeasureHeaderModel measure, int index, bool total = false)
        {
            if (total == false)
            {
                if (!Waves.Keys.Contains(wave) || !Waves[wave].Contains(measure.ID))
                {
                    return ReportText.No_Choosed;
                }
                if (measure.TotalScored)
                {
                    var recordOfSchools = records.FindAll(x => x.Wave == wave && x.SchoolId == source.ID && x.MeasureId == measure.ID);
                    if (recordOfSchools.Any())
                    {
                        //aaabbbccc PercentileRank int=>string
                        int count = 0;
                        List<decimal> percentileRanks = new List<decimal>();
                        foreach (var recordOfSchool in recordOfSchools)
                        {
                            if (recordOfSchool.PercentileRank != ReportText.PercentileRankNA && recordOfSchool.PercentileRank != ReportText.No_Record)
                            {
                                int percentileRank = 0;
                                string percentieRankString = new Regex("<|>").Replace(recordOfSchool.PercentileRank, "");
                                int.TryParse(percentieRankString, out percentileRank);
                                percentileRanks.Add(percentileRank);
                                count++;
                            }
                        }
                        return percentileRanks.Any()
                            ? (percentileRanks.Sum() / count).ToString("0.##")
                            : ReportText.No_Record;
                    }
                    else
                        return ReportText.No_Record;
                }
                else
                {
                    return ReportText.No_TotalScored;
                }
            }
            else
            {
                if (!Waves.Keys.Contains(wave) || !Waves[wave].Contains(measure.ParentId))
                {
                    return ReportText.No_Choosed;
                }
                //aaabbbccc PercentileRank int=>string
                var recordOfSchools = records.FindAll(x => x.Wave == wave && x.SchoolId == source.ID && x.MeasureId == measure.ParentId);
                if (measure.ParentScored)
                {
                    if (recordOfSchools.Any())
                    {
                        int count = 0;
                        List<decimal> percentileRanks = new List<decimal>();
                        foreach (var recordOfSchool in recordOfSchools)
                        {
                            if (recordOfSchool.PercentileRank != ReportText.PercentileRankNA && recordOfSchool.PercentileRank != ReportText.No_Record)
                            {
                                int percentileRank = 0;
                                string percentieRankString = new Regex("<|>").Replace(recordOfSchool.PercentileRank, "");
                                int.TryParse(percentieRankString, out percentileRank);
                                percentileRanks.Add(percentileRank);
                                count++;
                            }
                        }
                        return percentileRanks.Any()
                            ? (percentileRanks.Sum() / count).ToString("0.##")
                            : ReportText.No_Record;
                    }
                    else
                        return ReportText.No_Record;
                }
                else
                {
                    return ReportText.No_TotalScored;
                }
            }
        }

        protected override void SetCellStyle(object cell, CpallsSchoolModel source, List<SchoolPercentileRankModel> records, Wave wave,
            ReportMeasureHeaderModel measure, int index, bool total = false)
        {
            // school view: do not need to set styles
            //throw new NotImplementedException();
        }

        protected override int GetSourceID(CpallsSchoolModel source)
        {
            return source.ID;
        }
    }

    #endregion

    #region report host - school(classes)
    internal class AverageByWavePdfSchoolGenerator : AverageByWavePdfGenerator<CpallsClassModel, StudentRecordModel>
    {
        public AverageByWavePdfSchoolGenerator(IEnumerable<CpallsClassModel> sources, IEnumerable<ReportMeasureHeaderModel> measures, IEnumerable<StudentRecordModel> records, Dictionary<Wave, IEnumerable<int>> waves, IEnumerable<BenchmarkModel> benchmarks)
            : base(sources, measures, records, waves, benchmarks)
        {
        }

        protected override string GetTitle()
        {
            return "Class";
        }

        protected override string GetSourceName(CpallsClassModel source)
        {
            return source.Name;
        }

        private Dictionary<int, decimal> _totalScoresOfClass = new Dictionary<int, decimal>();
        private int classId = -1;
        protected override string CalcValue(CpallsClassModel source, List<StudentRecordModel> records, Wave wave, ReportMeasureHeaderModel measure, int index, bool total)
        {
            if (classId != source.ID)
                _totalScoresOfClass = new Dictionary<int, decimal>();
            if (!_totalScoresOfClass.ContainsKey(measure.ParentId))
                _totalScoresOfClass.Add(measure.ParentId, -1);
            classId = source.ID;

            if (total == false)
            {
                if (!Waves.Keys.Contains(wave) || !Waves[wave].Contains(measure.ID))
                {
                    return ReportText.No_Choosed;
                }
                if (measure.TotalScored)
                {
                    var recordOfClass = records.FindAll(x => x.Wave == wave && x.MeasureId == measure.ID && source.StudentIds.Contains(x.StudentId));
                    if (recordOfClass.Count > 0)
                    {
                        var average = recordOfClass.Sum(x => x.Goal) / recordOfClass.Count;
                        _totalScoresOfClass[measure.ParentId] += average;
                        return average.ToPrecisionString(2);
                    }
                    else
                        return ReportText.No_Record;
                }
                else
                    return ReportText.No_TotalScored;
            }
            else
            {
                if (measure.ParentScored)
                {
                    if (measure.ParentId > 1 && _totalScoresOfClass[measure.ParentId] >= 0)
                        return (_totalScoresOfClass[measure.ParentId] + 1).ToPrecisionString(2);
                    else
                        return ReportText.No_Record;
                }
                else
                {
                    return ReportText.No_TotalScored;
                }
            }
        }

        protected override int GetSourceID(CpallsClassModel source)
        {
            return source.ID;
        }

        protected override void SetCellStyle(object cell, CpallsClassModel source, List<StudentRecordModel> records, Wave wave, ReportMeasureHeaderModel measure,
            int index, bool total)
        {
            // class view: do not need to set styles
            //throw new NotImplementedException();
        }
    }

    #endregion

    #region report host - school(classes) percentile rank
    internal class AverageByWavePdfClassPercentileRankAverageGenerator : AverageByWavePdfGenerator<CpallsClassModel, StudentRecordModel>
    {
        public AverageByWavePdfClassPercentileRankAverageGenerator(IEnumerable<CpallsClassModel> sources, IEnumerable<ReportMeasureHeaderModel> measures, IEnumerable<StudentRecordModel> records, Dictionary<Wave, IEnumerable<int>> waves, IEnumerable<BenchmarkModel> benchmarks)
            : base(sources, measures, records, waves, benchmarks)
        {
        }

        protected override string GetTitle()
        {
            return "Class";
        }

        protected override string GetSourceName(CpallsClassModel source)
        {
            return source.Name;
        }

        private Dictionary<int, decimal> _totalScoresOfClass = new Dictionary<int, decimal>();
        private int classId = -1;
        protected override string CalcValue(CpallsClassModel source, List<StudentRecordModel> records, Wave wave, ReportMeasureHeaderModel measure, int index, bool total)
        {
            if (classId != source.ID)
                _totalScoresOfClass = new Dictionary<int, decimal>();
            if (!_totalScoresOfClass.ContainsKey(measure.ParentId))
                _totalScoresOfClass.Add(measure.ParentId, -1);
            classId = source.ID;

            if (total == false)
            {
                if (!Waves.Keys.Contains(wave) || !Waves[wave].Contains(measure.ID))
                {
                    return ReportText.No_Choosed;
                }
                if (measure.TotalScored)
                {
                    var recordOfClass = records.FindAll(x => x.Wave == wave && x.MeasureId == measure.ID && source.StudentIds.Contains(x.StudentId));
                    if (recordOfClass.Any())
                    {
                        //aaabbbccc PercentileRank int=>string
                        int count = 0;
                        List<decimal> percentileRanks = new List<decimal>();
                        foreach (var studentRecordModel in recordOfClass)
                        {
                            if (studentRecordModel.PercentileRank != ReportText.PercentileRankNA && studentRecordModel.PercentileRank != ReportText.No_Record)
                            {
                                int percentileRank = 0;
                                string percentieRankString = new Regex("<|>").Replace(studentRecordModel.PercentileRank, "");
                                int.TryParse(percentieRankString, out percentileRank);
                                percentileRanks.Add(percentileRank);
                                count++;
                            }
                        }
                        return percentileRanks.Any()
                            ? (percentileRanks.Sum() / count).ToString("0.##")
                            : ReportText.No_Record;
                    }
                    else
                        return ReportText.No_Record;
                }
                else
                    return ReportText.No_TotalScored;
            }
            else
            {
                var recordOfClass = records.FindAll(x => x.Wave == wave
                                                         && source.StudentIds.Contains(x.StudentId) &&
                                                         x.MeasureId == measure.ParentId);
                if (measure.ParentScored)
                {
                    if (recordOfClass.Any())
                    {
                        int count = 0;
                        List<decimal> percentileRanks = new List<decimal>();
                        foreach (var studentRecordModel in recordOfClass)
                        {
                            if (studentRecordModel.PercentileRank != ReportText.PercentileRankNA && studentRecordModel.PercentileRank != ReportText.No_Record)
                            {
                                int percentileRank = 0;
                                string percentieRankString = new Regex("<|>").Replace(studentRecordModel.PercentileRank, "");
                                int.TryParse(percentieRankString, out percentileRank);
                                percentileRanks.Add(percentileRank);
                                count++;
                            }
                        }
                        return percentileRanks.Any()
                            ? (percentileRanks.Sum() / count).ToString("0.##")
                            : ReportText.No_Record;
                    }
                    else
                        return ReportText.No_Record;
                }
                else
                {
                    return ReportText.No_TotalScored;
                }
            }
        }

        protected override int GetSourceID(CpallsClassModel source)
        {
            return source.ID;
        }

        protected override void SetCellStyle(object cell, CpallsClassModel source, List<StudentRecordModel> records, Wave wave, ReportMeasureHeaderModel measure,
            int index, bool total)
        {
            // class view: do not need to set styles
            //throw new NotImplementedException();
        }
    }

    #endregion

    #region report host - class(Students)
    internal class AverageByWavePdfClassGenerator : AverageByWavePdfGenerator<CpallsStudentModel, StudentRecordModel>
    {
        private AdeBusiness _adeBusiness;
        private AdeBusiness AdeBusiness
        {
            get { return _adeBusiness ?? (_adeBusiness = new AdeBusiness()); }
            set { _adeBusiness = value; }
        }

        public AverageByWavePdfClassGenerator(IEnumerable<CpallsStudentModel> sources, IEnumerable<ReportMeasureHeaderModel> measures, IEnumerable<StudentRecordModel> records, Dictionary<Wave, IEnumerable<int>> waves, IEnumerable<BenchmarkModel> benchmarks)
            : base(sources, measures, records, waves, benchmarks)
        {
        }

        protected override string GetTitle()
        {
            return "Class";
        }

        protected override string GetSourceName(CpallsStudentModel source)
        {
            return source.FirstName + " " + source.LastName;
        }

        protected override string CalcValue(CpallsStudentModel source, List<StudentRecordModel> records, Wave wave, ReportMeasureHeaderModel measure, int index, bool total)
        {
            if (total == false)
            {
                if (!Waves.Keys.Contains(wave) || !Waves[wave].Contains(measure.ID))
                {
                    return ReportText.No_Choosed;
                }
                var recordOfStu = records.Find(x => x.Wave == wave && x.MeasureId == measure.ID && source.ID == x.StudentId);

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
                    var recordsOfStu = records.Where(x => x.StudentId == source.ID && x.Wave == wave
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

        protected override int GetSourceID(CpallsStudentModel source)
        {
            return source.ID;
        }

        protected override void SetCellStyle(object cell, CpallsStudentModel source, List<StudentRecordModel> records,
            Wave wave, ReportMeasureHeaderModel measure,
            int index, bool total)
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
                var recordsOfStu = records.FindAll(x => x.Wave == wave && x.MeasureId == measure.ID && source.ID == x.StudentId);
                BenchmarkEntity benchmark = new BenchmarkEntity();
                var bentchmark = 0m;
                var goal = -1m;
                if (measure.TotalScored && recordsOfStu.Any() && total == false)
                {
                    //bentchmark = recordsOfStu.First().Benchmark;
                    if (_adeBusiness == null)
                    {
                        _adeBusiness = new AdeBusiness();
                    }
                    benchmark = _adeBusiness.GetBenchmark(recordsOfStu.First().BenchmarkId);
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
                    recordsOfStu = records.FindAll(x => x.StudentId == source.ID && x.Wave == wave
                                                          &&
                                                          Measures.Any(m => m.ParentId == measure.ParentId && m.TotalScored && m.ID == x.MeasureId));
                    if (recordsOfStu.Any())
                    {
                        //bentchmark = records.First(x => x.Wave == wave && x.MeasureId == measure.ParentId && source.ID == x.StudentId).Benchmark;
                        var studentMeasure =
                            records.FirstOrDefault(
                                x => x.Wave == wave && x.MeasureId == measure.ParentId && source.ID == x.StudentId);
                        goal = recordsOfStu.Sum(x => x.Goal);
                        if (_adeBusiness == null)
                        {
                            _adeBusiness = new AdeBusiness();
                        }
                        benchmark = _adeBusiness.GetBenchmark(studentMeasure == null ? 0 : studentMeasure.BenchmarkId);
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
                    else if (benchmark != null)
                    {
                        cellModel.Background = benchmark.BlackWhite.ToString().ToLower();
                        cellModel.Color = benchmark.Color;
                    }
                    //else if (bentchmark < 0)
                    //    cellModel.Background = lightColor ? ReportTheme.TE3_Light_ClassName : ReportTheme.TE3_ClassName;
                    //else
                    //{
                    //    if (goal < bentchmark)
                    //    {
                    //        if (source.Age >= 4)
                    //            cellModel.Background = lightColor ? ReportTheme.GE4_Light_ClassName : ReportTheme.GE4_ClassName;
                    //        else
                    //            cellModel.Background = lightColor ? ReportTheme.TE4_Light_ClassName : ReportTheme.TE4_ClassName;
                    //    }
                    //    else
                    //        cellModel.Background = lightColor ? ReportTheme.Passed_Light_ClassName : ReportTheme.Passed_ClassName;
                    //}
                }
            }
        }
    }

    #endregion

    #region report host - class(Students) Percentile Rank
    internal class AverageByWavePdfClassPercentileRankGenerator : AverageByWavePdfGenerator<CpallsStudentModel, StudentRecordModel>
    {
        private AdeBusiness _adeBusiness;
        private AdeBusiness AdeBusiness
        {
            get { return _adeBusiness ?? (_adeBusiness = new AdeBusiness()); }
            set { _adeBusiness = value; }
        }

        public AverageByWavePdfClassPercentileRankGenerator(IEnumerable<CpallsStudentModel> sources, IEnumerable<ReportMeasureHeaderModel> measures, IEnumerable<StudentRecordModel> records, Dictionary<Wave, IEnumerable<int>> waves, IEnumerable<BenchmarkModel> benchmarks)
            : base(sources, measures, records, waves, benchmarks)
        {
        }

        protected override string GetTitle()
        {
            return "Class";
        }

        protected override string GetSourceName(CpallsStudentModel source)
        {
            return source.FirstName + " " + source.LastName;
        }

        protected override string CalcValue(CpallsStudentModel source, List<StudentRecordModel> records, Wave wave, ReportMeasureHeaderModel measure, int index, bool total)
        {
            if (total == false)
            {
                var recordOfStu = records.Find(x => x.Wave == wave && x.MeasureId == measure.ID && source.ID == x.StudentId);
                if (!measure.TotalScored)
                {
                    return ReportText.No_TotalScored;
                }
                else if (recordOfStu != null)
                {
                    //aaabbbccc PercentileRank int=>string
                    //return recordOfStu.PercentileRank == 0
                    //    ? ReportText.PercentileRankNA
                    //    : recordOfStu.PercentileRank.ToString();
                    return recordOfStu.PercentileRank;
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
                    //aaabbbccc PercentileRank int=>string
                    //var recordsOfStu = records.Where(x => x.StudentId == source.ID && x.Wave == wave
                    //                                      &&
                    //                                      Measures.Where(m => m.ParentId == measure.ParentId && m.TotalScored)
                    //                                          .Select(cm => cm.ID)
                    //                                          .Contains(x.MeasureId)).ToList();
                    var recordsOfStu = records.Where(x => x.StudentId == source.ID && x.Wave == wave
                                                          &&
                                                          x.MeasureId == measure.ParentId).ToList();
                    if (recordsOfStu.Any())
                    {
                        //aaabbbccc PercentileRank int=>string
                        //var totalScore = recordsOfStu.Sum(x => x.PercentileRank);
                        //return totalScore.ToString();
                        return recordsOfStu.FirstOrDefault().PercentileRank;
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

        protected override int GetSourceID(CpallsStudentModel source)
        {
            return source.ID;
        }

        protected override void SetCellStyle(object cell, CpallsStudentModel source, List<StudentRecordModel> records,
            Wave wave, ReportMeasureHeaderModel measure,
            int index, bool total)
        {
            // Class Percentile Rank: do not need to set styles

            if (!Waves.Keys.Contains(wave) || !Waves[wave].Contains(measure.ID))
            {
                return;
            }
            var cellModel = cell as ReportCellModel;
            bool lightColor = measure.LightColor;
            bool hasCutOffScore = measure.HasCutOffScores;
            if (cellModel != null)
            {
                var recordsOfStu = records.FindAll(x => x.Wave == wave && x.MeasureId == measure.ID && source.ID == x.StudentId);
                BenchmarkEntity benchmark = new BenchmarkEntity();
                var goal = -1m;
                if (measure.TotalScored && recordsOfStu.Any() && total == false)
                {
                    if (_adeBusiness == null)
                    {
                        _adeBusiness = new AdeBusiness();
                    }
                    benchmark = _adeBusiness.GetBenchmark(recordsOfStu.First().BenchmarkId);
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
                    recordsOfStu = records.FindAll(x => x.StudentId == source.ID && x.Wave == wave
                                                          &&
                                                          Measures.Any(m => m.ParentId == measure.ParentId && m.TotalScored && m.ID == x.MeasureId));
                    if (recordsOfStu.Any())
                    {
                        var studentMeasure =
                            records.FirstOrDefault(
                                x => x.Wave == wave && x.MeasureId == measure.ParentId && source.ID == x.StudentId);
                        goal = recordsOfStu.Sum(x => x.Goal);
                        if (_adeBusiness == null)
                        {
                            _adeBusiness = new AdeBusiness();
                        }
                        benchmark = _adeBusiness.GetBenchmark(studentMeasure == null ? 0 : studentMeasure.BenchmarkId);
                        var currentMeasure = _adeBusiness.GetMeasureModel(measure.ID);
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
                    else if (benchmark != null)
                    {
                        cellModel.Background = benchmark.BlackWhite.ToString().ToLower();
                        cellModel.Color = benchmark.Color;
                    }
                }
            }
        }
    }

    #endregion
}
