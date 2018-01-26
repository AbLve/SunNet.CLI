using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/**************************************************************************
 * Developer: 		JackZhang
 * Computer:		JACKZ
 * Domain:			Jackz
 * CreatedOn:		2014/11/13 21:32:31
 * Description:		Please input class summary
 * Version History:	Created,2014/11/13 21:32:31
 * 
 * 
 **************************************************************************/
using StructureMap;
using Sunnet.Cli.Business.Ade;
using Sunnet.Cli.Business.Common;
using Sunnet.Cli.Business.Cpalls.Models;
using Sunnet.Cli.Business.Cpalls.Models.Report;
using Sunnet.Cli.Business.Practices.Models;
using Sunnet.Cli.Core.Ade;
using Sunnet.Cli.Core.Ade.Entities;
using Sunnet.Cli.Core.Cpalls;
using Sunnet.Cli.Core.Cpalls.Models.Report;
using Sunnet.Framework.Extensions;
using Sunnet.Framework.Log;
using Sunnet.Cli.Business.Ade.Models;

namespace Sunnet.Cli.Business.Cpalls.Growth
{
    #region Report Export Type - Pdf
    internal abstract class AverageBySourcePdfGenerator<TSource, TRecord> : AverageBySourceGenerator<TSource, TRecord>
        where TSource : class
        where TRecord : class
    {
        private Dictionary<object, List<ReportRowModel>> _result;
        private ISunnetLog _logger;

        private void Log(string format, params object[] args)
        {
            var s = string.Format(format, args);
            //_logger.Log(s);
        }

        protected AverageBySourcePdfGenerator(IEnumerable<TSource> sources, IEnumerable<ReportMeasureHeaderModel> measures, IEnumerable<TRecord> records, Dictionary<Wave, IEnumerable<int>> waves, IEnumerable<BenchmarkModel> benchmarks)
            : base(sources, measures, records, waves, benchmarks)
        {
            _logger = ObjectFactory.GetInstance<ISunnetLog>();
        }

        private ReportRowModel _sourceRow;
        private ReportRowModel _waveHeader;
        private ReportRowModel _dataRow;
        private ReportRowModel _totalRow;
        private int sourceId;
        protected override void Report_Started()
        {
            _sourceRow = new ReportRowModel();
            _waveHeader = new ReportRowModel();
            _dataRow = new ReportRowModel();
            Log("Report_Started");
        }

        protected sealed override void Source_Started(TSource source, int index)
        {
            Log("Source_Started, Name:{0}", GetSourceName(source));
            sourceId = GetSourceID(source);
            _sourceRow = new ReportRowModel();
            _sourceRow.Add(GetSourceName(source));
            _waveHeader = new ReportRowModel();
        }

        protected override void Header_Started()
        {
            _waveHeader.Add("Measure", 2, 1);
            if (ShowTotalScore)
                _waveHeader.Add(ReportText.TotalScore);
        }

        protected override void Wave_Added(Wave wave, int index)
        {
            _waveHeader.Add("Wave " + wave.ToDescription());
            if (PrintComment)
            {
                _waveHeader.Add("Comment");
            }
        }

        protected override void Header_Over()
        {
            AddRow(sourceId, _sourceRow);
            AddRow(sourceId, _waveHeader);
            Log("Header_Over");
        }
        protected override void Row_Started()
        {
            _dataRow = new ReportRowModel();
        }

        protected sealed override void Measure_Added(ReportMeasureHeaderModel measure, int index, bool isParent = false, int children = 0)
        {
            Log("Measure_Added, ID:{0}", measure.ID);
            _dataRow.MeasureId = measure.ID;
            if (isParent)
            {
                var parent = Measures.FirstOrDefault(c => c.ID == measure.ParentId);
                if (parent != null)
                {
                    _dataRow.Add(measure.ParentName, 1, children, true, parent.Description);
                    _dataRow.ParentMeasureId = parent.ID;
                }
                else
                {
                    var bus = new AdeBusiness();
                    var findParent = bus.GetMeasureModel(measure.ParentId);
                    _dataRow.Add(measure.ParentName, 1, children, true, findParent != null ? findParent.Description : "");
                    if (findParent != null)
                    {
                        _dataRow.ParentMeasureId = findParent.ID;
                    }
                }
                _dataRow.Add(measure.Name, measure.Description);
            }
            else
            {
                if (measure.ParentId > 1)
                    _dataRow.Add(measure.Name, 1, 1, measure.Description);
                else
                    _dataRow.Add(measure.Name, 2, 1, measure.Description);
            }
            if (ShowTotalScore)
            {
                if (measure.TotalScored)
                    _dataRow.Add(measure.TotalScore.ToPrecisionString(2));
                else
                    _dataRow.Add(ReportText.No_Record);
            }

            if (measure.IsLastOfChildren && ShowTotalForMeasure)
            {
                if (_totalRow == null)
                    _totalRow = new ReportRowModel();
                _totalRow.Add(ReportText.TotalColumnNameForSatisfactory);
                if (ShowTotalScore)
                {
                    var childMeasures = Measures.FindAll(x => x.ParentId == measure.ParentId && x.TotalScored);
                    if (childMeasures.Any())
                        _totalRow.Add(childMeasures.Sum(x => x.TotalScore).ToPrecisionString(2));
                    else
                        _totalRow.Add(ReportText.No_Record);
                }
            }
        }

        protected sealed override void Record_Generating(TSource source, List<TRecord> records, Wave wave, ReportMeasureHeaderModel measure, int index)
        {
            Log("Record_Generating, Name:{0},Measure:{1}", GetSourceName(source), measure.Name);
            var value = CalcValue(source, records, wave, measure, index);

            _dataRow.Add(value);
            SetCellStyle(_dataRow.Cells.Last(), source, records, wave, measure, index);
            if (PrintComment)
            {
                _dataRow.Add(GetComment(source, records, wave, measure, index));
            }

            if (measure.IsLastOfChildren && ShowTotalForMeasure)
            {
                value = CalcValue(source, records, wave, measure, index, true);
                _totalRow.Add(value);
                SetCellStyle(_totalRow.Cells.Last(), source, records, wave, measure, index, true);
                if (PrintComment)
                    _totalRow.Add(string.Empty);
            }
        }

        protected override void Row_Over()
        {
            AddRow(sourceId, _dataRow);
            _dataRow = null;
            AddRow(sourceId, _totalRow);
            _totalRow = null;
        }

        protected sealed override void Source_Over(TSource source, int index)
        {
            Log("Source_Over, Name:{0}", GetSourceName(source));
        }

        protected override void Report_Over()
        {
            Log("Report_Over");
        }

        private void AddRow(int sourceId, ReportRowModel row)
        {
            if (!Reports.ContainsKey(sourceId))
                Reports.Add(sourceId, new List<ReportRowModel>());
            if (row != null && row.Cells.Any())
                Reports[sourceId].Add(row);
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

    #region Report Host - Community

    internal class AverageBySourcePdfCommunityGenerator : AverageBySourcePdfGenerator<CpallsCommunityModel, StudentRecordModel>
    {
        public AverageBySourcePdfCommunityGenerator(IEnumerable<CpallsCommunityModel> sources, IEnumerable<ReportMeasureHeaderModel> measures, IEnumerable<StudentRecordModel> records, Dictionary<Wave, IEnumerable<int>> waves, IEnumerable<BenchmarkModel> benchmarks)
            : base(sources, measures, records, waves, benchmarks)
        {
        }

        protected override string GetTitle()
        {
            return "School";
        }

        protected override string GetSourceName(CpallsCommunityModel source)
        {
            return source.Name;
        }

        protected override int GetSourceID(CpallsCommunityModel source)
        {
            return source.ID;
        }

        private Dictionary<int, Dictionary<Wave, decimal>> _totalScores = new Dictionary<int, Dictionary<Wave, decimal>>();
        private int classId = -1;

        protected override string CalcValue(CpallsCommunityModel source, List<StudentRecordModel> records, Wave wave,
            ReportMeasureHeaderModel measure, int index, bool total)
        {
            if (classId != source.ID)
                _totalScores = new Dictionary<int, Dictionary<Wave, decimal>>();
            if (!_totalScores.ContainsKey(measure.ParentId))
            {
                var waves = new Dictionary<Wave, decimal>();
                waves.Add(Wave.BOY, -1);
                waves.Add(Wave.MOY, -1);
                waves.Add(Wave.EOY, -1);
                _totalScores.Add(measure.ParentId, waves);
            }
            classId = source.ID;
            if (!Waves.Keys.Contains(wave)) return ReportText.No_Choosed;
            if (total == false)
            {
                if (!Waves[wave].Contains(measure.ID)) return ReportText.No_Choosed;
                if (measure.TotalScored)
                {
                    var totalRecords = records.Where(x => x.Wave == wave && x.MeasureId == measure.ID).ToList();
                    if (totalRecords.Any())
                    {
                        var average = totalRecords.Sum(x => x.Goal) / totalRecords.Count();
                        _totalScores[measure.ParentId][wave] += average;
                        return average.ToPrecisionString(2);
                    }
                    return ReportText.No_Record;
                }
                else
                {
                    return ReportText.No_TotalScored;
                }
            }
            else
            {
                if (!Waves[wave].Contains(measure.ParentId)) return ReportText.No_Choosed;
                if (measure.ParentScored)
                {
                    if (measure.ParentId > 1 && _totalScores[measure.ParentId][wave] >= 0)
                        return (_totalScores[measure.ParentId][wave] + 1).ToPrecisionString(2);
                    else
                        return ReportText.No_Record;
                }
                else
                {
                    return ReportText.No_TotalScored;
                }
            }
        }
        protected override void SetCellStyle(object cell, CpallsCommunityModel source, List<StudentRecordModel> records, Wave wave,
            ReportMeasureHeaderModel measure, int index, bool total)
        {
            // school view: do not need to set styles
            //throw new NotImplementedException();
        }
    }

    #endregion

    #region Report Host - School
    internal class AverageBySourcePdfSchoolGenerator : AverageBySourcePdfGenerator<CpallsSchoolModel, StudentRecordModel>
    {
        public AverageBySourcePdfSchoolGenerator(IEnumerable<CpallsSchoolModel> sources, IEnumerable<ReportMeasureHeaderModel> measures, IEnumerable<StudentRecordModel> records, Dictionary<Wave, IEnumerable<int>> waves, IEnumerable<BenchmarkModel> benchmarks)
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

        protected override int GetSourceID(CpallsSchoolModel source)
        {
            return source.ID;
        }


        private Dictionary<int, Dictionary<Wave, decimal>> _totalScores = new Dictionary<int, Dictionary<Wave, decimal>>();
        private int classId = -1;

        protected override string CalcValue(CpallsSchoolModel source, List<StudentRecordModel> records, Wave wave,
            ReportMeasureHeaderModel measure, int index, bool total = false)
        {
            if (classId != source.ID)
                _totalScores = new Dictionary<int, Dictionary<Wave, decimal>>();
            if (!_totalScores.ContainsKey(measure.ParentId))
            {
                var waves = new Dictionary<Wave, decimal>();
                waves.Add(Wave.BOY, -1);
                waves.Add(Wave.MOY, -1);
                waves.Add(Wave.EOY, -1);
                _totalScores.Add(measure.ParentId, waves);
            }
            classId = source.ID;
            if (!Waves.Keys.Contains(wave)) return ReportText.No_Choosed;
            if (total == false)
            {
                if (!Waves[wave].Contains(measure.ID)) return ReportText.No_Choosed;
                if (measure.TotalScored)
                {
                    var totalRecords = records.Where(x => x.Wave == wave && x.MeasureId == measure.ID && x.SchoolId == source.ID).ToList();
                    if (totalRecords.Any())
                    {
                        var average = totalRecords.Sum(x => x.Goal) / totalRecords.Count();
                        _totalScores[measure.ParentId][wave] += average;
                        return average.ToPrecisionString(2);
                    }
                    return ReportText.No_Record;
                }
                else
                {
                    return ReportText.No_TotalScored;
                }
            }
            else
            {
                if (!Waves[wave].Contains(measure.ParentId)) return ReportText.No_Choosed;
                if (measure.ParentScored)
                {
                    if (measure.ParentId > 1 && _totalScores[measure.ParentId][wave] >= 0)
                        return (_totalScores[measure.ParentId][wave] + 1).ToPrecisionString(2);
                    else
                        return ReportText.No_Record;
                }
                else
                {
                    return ReportText.No_TotalScored;
                }
            }
        }

        protected override void SetCellStyle(object cell, CpallsSchoolModel source, List<StudentRecordModel> records, Wave wave,
            ReportMeasureHeaderModel measure, int index, bool total)
        {
            // school view: do not need to set styles
            //throw new NotImplementedException();
        }
    }
    #endregion

    #region Report Host - Class
    internal class AverageBySourcePdfClassGenerator : AverageBySourcePdfGenerator<CpallsClassModel, StudentRecordModel>
    {
        public AverageBySourcePdfClassGenerator(IEnumerable<CpallsClassModel> sources, IEnumerable<ReportMeasureHeaderModel> measures, IEnumerable<StudentRecordModel> records, Dictionary<Wave, IEnumerable<int>> waves, IEnumerable<BenchmarkModel> benchmarks)
            : base(sources, measures, records, waves, benchmarks)
        {
        }

        protected override string GetTitle()
        {
            return "School";
        }

        protected override string GetSourceName(CpallsClassModel source)
        {
            return source.Name;
        }

        protected override int GetSourceID(CpallsClassModel source)
        {
            return source.ID;
        }

        private Dictionary<int, Dictionary<Wave, decimal>> _totalScoresOfClass = new Dictionary<int, Dictionary<Wave, decimal>>();
        private int classId = -1;
        protected override string CalcValue(CpallsClassModel source, List<StudentRecordModel> records, Wave wave,
            ReportMeasureHeaderModel measure, int index, bool total)
        {
            if (classId != source.ID)
                _totalScoresOfClass = new Dictionary<int, Dictionary<Wave, decimal>>();
            if (!_totalScoresOfClass.ContainsKey(measure.ParentId))
            {
                var waves = new Dictionary<Wave, decimal>();
                waves.Add(Wave.BOY, -1);
                waves.Add(Wave.MOY, -1);
                waves.Add(Wave.EOY, -1);
                _totalScoresOfClass.Add(measure.ParentId, waves);
            }
            classId = source.ID;

            if (!Waves.Keys.Contains(wave)) return ReportText.No_Choosed;
            if (total == false)
            {
                if (!Waves[wave].Contains(measure.ID)) return ReportText.No_Choosed;
                if (measure.TotalScored)
                {
                    var totalRecords = records.Where(x => x.Wave == wave && x.MeasureId == measure.ID
                        && source.StudentIds.Contains(x.StudentId)).ToList();
                    if (totalRecords.Any())
                    {
                        var average = totalRecords.Sum(x => x.Goal) / totalRecords.Count();
                        _totalScoresOfClass[measure.ParentId][wave] += average;
                        return average.ToPrecisionString(2);
                    }
                    return ReportText.No_Record;
                }
                else
                {
                    return ReportText.No_TotalScored;
                }
            }
            else
            {
                if (!Waves[wave].Contains(measure.ParentId)) return ReportText.No_Choosed;
                if (measure.ParentScored)
                {
                    if (measure.ParentId > 1 && _totalScoresOfClass[measure.ParentId][wave] >= 0)
                        return (_totalScoresOfClass[measure.ParentId][wave] + 1).ToPrecisionString(2);
                    else
                        return ReportText.No_Record;
                }
                else
                {
                    return ReportText.No_TotalScored;
                }
            }
        }

        protected override void SetCellStyle(object cell, CpallsClassModel source, List<StudentRecordModel> records, Wave wave,
            ReportMeasureHeaderModel measure, int index, bool total)
        {
            // class view: do not need to set styles
            //throw new NotImplementedException();
        }
    }

    #endregion

    #region Report Host - Student

    internal class AverageBySourcePdfStudentGenerator : AverageBySourcePdfGenerator<CpallsStudentModel, StudentRecordModel>
    {
        private AdeBusiness _adeBusiness;
        private AdeBusiness AdeBusiness
        {
            get { return _adeBusiness ?? (_adeBusiness = new AdeBusiness()); }
            set { _adeBusiness = value; }
        }

        public AverageBySourcePdfStudentGenerator(IEnumerable<CpallsStudentModel> sources, IEnumerable<ReportMeasureHeaderModel> measures, IEnumerable<StudentRecordModel> records, Dictionary<Wave, IEnumerable<int>> waves, IEnumerable<BenchmarkModel> benchmarks)
            : base(sources, measures, records, waves, benchmarks)
        {
        }

        protected override void SetCellStyle(object cell, CpallsStudentModel source, List<StudentRecordModel> records,
            Wave wave, ReportMeasureHeaderModel measure, int index, bool total)
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
                //if (_adeBusiness == null)
                //{
                //    _adeBusiness = new AdeBusiness();
                //}
                //var findMeasure = _adeBusiness.GetMeasureModel(measure.ID);
                //cellModel.Description = findMeasure.Description;

                var recordsOfStu = records.FindAll(x => x.Wave == wave && x.MeasureId == measure.ID && source.ID == x.StudentId);
                var bentchmark = 0m;
                var lowerScore = 0m;
                var higherScore = 0m;
                BenchmarkEntity benchmark = new BenchmarkEntity();
                var goal = -1m;
                if (measure.TotalScored && recordsOfStu.Any() && total == false)
                {
                    //bentchmark = recordsOfStu.First().Benchmark;
                    goal = recordsOfStu.Any() ? recordsOfStu.First().Goal : 0m;
                    if (_adeBusiness == null)
                    {
                        _adeBusiness = new AdeBusiness();
                    }
                    //lowerScore = recordsOfStu.First().LowerScore;
                    //higherScore = recordsOfStu.First().HigherScore;
                    benchmark = _adeBusiness.GetBenchmark(recordsOfStu.First().BenchmarkId);
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
                            records.First(
                                x => x.Wave == wave && x.MeasureId == measure.ParentId && source.ID == x.StudentId);
                        benchmark = _adeBusiness.GetBenchmark(studentMeasure.BenchmarkId);
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
                    else if (benchmark != null)
                    {
                        cellModel.Background = benchmark.BlackWhite.ToString().ToLower();
                        cellModel.Color = benchmark.Color;
                    }
                    //if (!hasCutOffScore)
                    //    cellModel.Background = ReportTheme.Missing_Bentchmark_ClassName;
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

        protected override string GetTitle()
        {
            return "School";
        }

        protected override string GetSourceName(CpallsStudentModel source)
        {
            return source.FirstName + " " + source.LastName;
        }

        protected override int GetSourceID(CpallsStudentModel source)
        {
            return source.ID;
        }

        protected override string CalcValue(CpallsStudentModel source, List<StudentRecordModel> records, Wave wave,
            ReportMeasureHeaderModel measure, int index, bool total)
        {
            if (!Waves.Keys.Contains(wave) || !Waves[wave].Contains(measure.ID))
            {
                return ReportText.No_Choosed;
            }
            if (total == false)
            {
                var recordOfStu =
                    records.Find(x => x.Wave == wave && x.MeasureId == measure.ID && source.ID == x.StudentId);

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

        protected override string GetComment(CpallsStudentModel source, List<StudentRecordModel> records, Wave wave, ReportMeasureHeaderModel measure, int index,
            bool total = false)
        {
            var studentMeasure = records.FirstOrDefault(x => x.StudentId == source.ID && x.MeasureId == measure.ID && x.Wave == wave);
            if (studentMeasure != null)
                return studentMeasure.Comment;
            return string.Empty;
        }
    }

    #endregion


    #region parent Report
    internal class ParentReportPdfStudentGenerator : AverageBySourcePdfGenerator<CpallsStudentModel, StudentRecordModel>
    {
        private AdeBusiness _adeBusiness;
        private AdeBusiness AdeBusiness
        {
            get { return _adeBusiness ?? (_adeBusiness = new AdeBusiness()); }
            set { _adeBusiness = value; }
        }

        public ParentReportPdfStudentGenerator(IEnumerable<CpallsStudentModel> sources, IEnumerable<ReportMeasureHeaderModel> measures, IEnumerable<StudentRecordModel> records, Dictionary<Wave, IEnumerable<int>> waves, IEnumerable<BenchmarkModel> benchmarks)
            : base(sources, measures, records, waves, benchmarks)
        {
        }

        protected override void SetCellStyle(object cell, CpallsStudentModel source, List<StudentRecordModel> records,
            Wave wave, ReportMeasureHeaderModel measure, int index, bool total)
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
                //if (_adeBusiness == null)
                //{
                //    _adeBusiness = new AdeBusiness();
                //}
                //var findMeasure = _adeBusiness.GetMeasureModel(measure.ID);
                //cellModel.Description = findMeasure.Description;

                var recordsOfStu = records.FindAll(x => x.Wave == wave && x.MeasureId == measure.ID && source.ID == x.StudentId);
                int bentchmarkId = 0;
                var goal = -1m;
                if (measure.TotalScored && recordsOfStu.Any() && total == false)
                {
                    bentchmarkId = recordsOfStu.First().BenchmarkId;
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
                        bentchmarkId = records.First(x => x.Wave == wave && x.MeasureId == measure.ParentId && source.ID == x.StudentId).BenchmarkId;
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
                    if (!hasCutOffScore || bentchmarkId == 0)
                        cellModel.AlertText = @"";
                    else
                    {
                        var benchmarkModel = Benchmarks.First(b => b.ID == bentchmarkId);
                        if (benchmarkModel == null)
                            cellModel.AlertText = @"";
                        else
                        {
                            cellModel.AlertText = benchmarkModel.LabelText;
                        }
                    }
                }
            }
        }

        protected override string GetTitle()
        {
            return "School";
        }

        protected override string GetSourceName(CpallsStudentModel source)
        {
            return source.FirstName + " " + source.LastName;
        }

        protected override int GetSourceID(CpallsStudentModel source)
        {
            return source.ID;
        }

        protected override string CalcValue(CpallsStudentModel source, List<StudentRecordModel> records, Wave wave,
            ReportMeasureHeaderModel measure, int index, bool total)
        {
            if (!Waves.Keys.Contains(wave) || !Waves[wave].Contains(measure.ID))
            {
                return ReportText.No_Choosed;
            }
            if (total == false)
            {
                var recordOfStu =
                    records.Find(x => x.Wave == wave && x.MeasureId == measure.ID && source.ID == x.StudentId);

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

        protected override string GetComment(CpallsStudentModel source, List<StudentRecordModel> records, Wave wave, ReportMeasureHeaderModel measure, int index,
            bool total = false)
        {
            var studentMeasure = records.FirstOrDefault(x => x.StudentId == source.ID && x.MeasureId == measure.ID && x.Wave == wave);
            if (studentMeasure != null)
                return studentMeasure.Comment;
            return string.Empty;
        }
    }
    #endregion


    #region Report Host - Student PercentileRank

    internal class AverageBySourcePdfStudentPercentileRankGenerator : AverageBySourcePdfGenerator<CpallsStudentModel, StudentRecordModel>
    {
        private AdeBusiness _adeBusiness;
        private AdeBusiness AdeBusiness
        {
            get { return _adeBusiness ?? (_adeBusiness = new AdeBusiness()); }
            set { _adeBusiness = value; }
        }

        public AverageBySourcePdfStudentPercentileRankGenerator(IEnumerable<CpallsStudentModel> sources, IEnumerable<ReportMeasureHeaderModel> measures, IEnumerable<StudentRecordModel> records, Dictionary<Wave, IEnumerable<int>> waves, IEnumerable<BenchmarkModel> benchmarks)
            : base(sources, measures, records, waves, benchmarks)
        {
        }

        protected override void SetCellStyle(object cell, CpallsStudentModel source, List<StudentRecordModel> records,
            Wave wave, ReportMeasureHeaderModel measure, int index, bool total)
        {
            // Student Percentile Rank: do not need to set styles
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
                    goal = recordsOfStu.Any() ? recordsOfStu.First().Goal : 0m;
                    if (_adeBusiness == null)
                    {
                        _adeBusiness = new AdeBusiness();
                    }
                    benchmark = _adeBusiness.GetBenchmark(recordsOfStu.First().BenchmarkId);
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
                            records.First(
                                x => x.Wave == wave && x.MeasureId == measure.ParentId && source.ID == x.StudentId);
                        benchmark = _adeBusiness.GetBenchmark(studentMeasure.BenchmarkId);
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
                    else if (benchmark != null)
                    {
                        cellModel.Background = benchmark.BlackWhite.ToString().ToLower();
                        cellModel.Color = benchmark.Color;
                    }
                }
            }
        }

        protected override string GetTitle()
        {
            return "School";
        }

        protected override string GetSourceName(CpallsStudentModel source)
        {
            return source.FirstName + " " + source.LastName;
        }

        protected override int GetSourceID(CpallsStudentModel source)
        {
            return source.ID;
        }

        protected override string CalcValue(CpallsStudentModel source, List<StudentRecordModel> records, Wave wave,
            ReportMeasureHeaderModel measure, int index, bool total)
        {
            if (!Waves.Keys.Contains(wave) || !Waves[wave].Contains(measure.ID))
            {
                return ReportText.No_Choosed;
            }

            if (total == false)
            {
                var recordOfStu =
                    records.Find(x => x.Wave == wave && x.MeasureId == measure.ID && source.ID == x.StudentId);
                if (!measure.TotalScored)
                {
                    return ReportText.No_TotalScored;
                }
                else if (recordOfStu != null)
                {
                    //aaabbbccc PercentileRank int=>string
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
                    var recordsOfStu = records.Where(x => x.StudentId == source.ID && x.Wave == wave
                                                          && x.MeasureId == measure.ParentId).ToList();

                    if (recordsOfStu.Any())
                    {
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

        protected override string GetComment(CpallsStudentModel source, List<StudentRecordModel> records, Wave wave, ReportMeasureHeaderModel measure, int index,
            bool total = false)
        {
            var studentMeasure = records.FirstOrDefault(x => x.StudentId == source.ID && x.MeasureId == measure.ID && x.Wave == wave);
            if (studentMeasure != null)
                return studentMeasure.Comment;
            return string.Empty;
        }
    }

    #endregion

    #region Report Host - Practice Student

    internal class AverageBySourcePdfPracticeStudentGenerator : AverageBySourcePdfGenerator<CpallsStudentModel, StudentRecordModel>
    {
        private AdeBusiness _adeBusiness;
        private AdeBusiness AdeBusiness
        {
            get { return _adeBusiness ?? (_adeBusiness = new AdeBusiness()); }
            set { _adeBusiness = value; }
        }

        public AverageBySourcePdfPracticeStudentGenerator(IEnumerable<CpallsStudentModel> sources, IEnumerable<ReportMeasureHeaderModel> measures, IEnumerable<StudentRecordModel> records, Dictionary<Wave, IEnumerable<int>> waves, IEnumerable<BenchmarkModel> benchmarks)
            : base(sources, measures, records, waves, benchmarks)
        {
        }

        protected override void SetCellStyle(object cell, CpallsStudentModel source, List<StudentRecordModel> records,
            Wave wave, ReportMeasureHeaderModel measure, int index, bool total)
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
                var goal = -1m;
                if (measure.TotalScored && recordsOfStu.Any() && total == false)
                {
                    goal = recordsOfStu.Any() ? recordsOfStu.First().Goal : 0m;
                    if (_adeBusiness == null)
                    {
                        _adeBusiness = new AdeBusiness();
                    }
                    benchmark = _adeBusiness.GetBenchmark(recordsOfStu.First().BenchmarkId);
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
                            records.First(
                                x => x.Wave == wave && x.MeasureId == measure.ParentId && source.ID == x.StudentId);
                        benchmark = _adeBusiness.GetBenchmark(studentMeasure.BenchmarkId);
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
                    else if (benchmark != null)
                    {
                        cellModel.Background = benchmark.BlackWhite.ToString().ToLower();
                        cellModel.Color = benchmark.Color;
                    }
                }
            }
        }

        protected override string GetTitle()
        {
            return "School";
        }

        protected override string GetSourceName(CpallsStudentModel source)
        {
            return source.FirstName + " " + source.LastName;
        }

        protected override int GetSourceID(CpallsStudentModel source)
        {
            return source.ID;
        }

        protected override string CalcValue(CpallsStudentModel source, List<StudentRecordModel> records, Wave wave,
            ReportMeasureHeaderModel measure, int index, bool total)
        {
            if (!Waves.Keys.Contains(wave) || !Waves[wave].Contains(measure.ID))
            {
                return ReportText.No_Choosed;
            }
            if (total == false)
            {
                var recordOfStu =
                    records.Find(x => x.Wave == wave && x.MeasureId == measure.ID && source.ID == x.StudentId);

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

        protected override string GetComment(CpallsStudentModel source, List<StudentRecordModel> records, Wave wave, ReportMeasureHeaderModel measure, int index,
            bool total = false)
        {
            var studentMeasure = records.FirstOrDefault(x => x.StudentId == source.ID && x.MeasureId == measure.ID && x.Wave == wave);
            if (studentMeasure != null)
                return studentMeasure.Comment;
            return string.Empty;
        }
    }

    #endregion

    #region Report Host - Practice Class
    internal class AverageBySourcePdfPracticeClassGenerator : AverageBySourcePdfGenerator<PracticeAssessmentModel, StudentRecordModel>
    {
        public AverageBySourcePdfPracticeClassGenerator(IEnumerable<PracticeAssessmentModel> sources, IEnumerable<ReportMeasureHeaderModel> measures, IEnumerable<StudentRecordModel> records, Dictionary<Wave, IEnumerable<int>> waves, IEnumerable<BenchmarkModel> benchmarks)
            : base(sources, measures, records, waves, benchmarks)
        {
        }

        protected override string GetTitle()
        {
            return "School";
        }

        protected override string GetSourceName(PracticeAssessmentModel source)
        {
            return source.Name;
        }

        protected override int GetSourceID(PracticeAssessmentModel source)
        {
            return source.ID;
        }

        private Dictionary<int, Dictionary<Wave, decimal>> _totalScoresOfClass = new Dictionary<int, Dictionary<Wave, decimal>>();
        protected override string CalcValue(PracticeAssessmentModel source, List<StudentRecordModel> records, Wave wave,
            ReportMeasureHeaderModel measure, int index, bool total)
        {
            if (!_totalScoresOfClass.ContainsKey(measure.ParentId))
            {
                var waves = new Dictionary<Wave, decimal>();
                waves.Add(Wave.BOY, -1);
                waves.Add(Wave.MOY, -1);
                waves.Add(Wave.EOY, -1);
                _totalScoresOfClass.Add(measure.ParentId, waves);
            }
            //classId = source.ID;

            if (!Waves.Keys.Contains(wave)) return ReportText.No_Choosed;
            if (total == false)
            {
                if (!Waves[wave].Contains(measure.ID)) return ReportText.No_Choosed;
                if (measure.TotalScored)
                {
                    //var totalRecords = records.Where(x => x.Wave == wave && x.MeasureId == measure.ID
                    //    && source.StudentIds.Contains(x.StudentId)).ToList();
                    var totalRecords = records.Where(x => x.Wave == wave && x.MeasureId == measure.ID).ToList();
                    if (totalRecords.Any())
                    {
                        var average = totalRecords.Sum(x => x.Goal) / totalRecords.Count();
                        _totalScoresOfClass[measure.ParentId][wave] += average;
                        return average.ToPrecisionString(2);
                    }
                    return ReportText.No_Record;
                }
                else
                {
                    return ReportText.No_TotalScored;
                }
            }
            else
            {
                if (!Waves[wave].Contains(measure.ParentId)) return ReportText.No_Choosed;
                if (measure.ParentScored)
                {
                    if (measure.ParentId > 1 && _totalScoresOfClass[measure.ParentId][wave] >= 0)
                        return (_totalScoresOfClass[measure.ParentId][wave] + 1).ToPrecisionString(2);
                    else
                        return ReportText.No_Record;
                }
                else
                {
                    return ReportText.No_TotalScored;
                }
            }
        }

        protected override void SetCellStyle(object cell, PracticeAssessmentModel source, List<StudentRecordModel> records, Wave wave,
            ReportMeasureHeaderModel measure, int index, bool total)
        {
            // class view: do not need to set styles
            //throw new NotImplementedException();
        }
    }

    #endregion
}
