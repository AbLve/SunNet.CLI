using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sunnet.Cli.Business.Cpalls.Models;
using Sunnet.Cli.Business.Cpalls.Models.Report;
using Sunnet.Cli.Core.Cpalls;
using Sunnet.Cli.Core.Cpalls.Models.Report;
using Sunnet.Framework.Extensions;
using Sunnet.Cli.Business.Ade.Models;

namespace Sunnet.Cli.Business.Cpalls
{
    #region Host - Community
    internal class SatisfactoryByWavePdfCommunityGenerator :
        SatisfactoryByWavePdfGenerator<CpallsSchoolModel, SchoolRecordModel>
    {
        public SatisfactoryByWavePdfCommunityGenerator(SatisfactoryType type,
            IEnumerable<CpallsSchoolModel> sources,
            IEnumerable<ReportMeasureHeaderModel> measures,
            IEnumerable<SchoolRecordModel> records,
            Dictionary<Wave, IEnumerable<int>> waves,
            IEnumerable<BenchmarkModel> benchmarks)
            : base(type, sources, measures, records, waves, benchmarks)
        {
        }

        protected override string GetSourceName(CpallsSchoolModel source)
        {
            throw new NotImplementedException();
        }

        protected override int GetSourceID(CpallsSchoolModel source)
        {
            throw new NotImplementedException();
        }

        protected override string GetTitle()
        {
            return "Community";
        }

        protected override string CalcValue(CpallsSchoolModel source, List<SchoolRecordModel> records, Wave wave, ReportMeasureHeaderModel measure, int index,
            bool total = false)
        {
            throw new NotImplementedException();
        }

        protected override string CalcValue(List<SchoolRecordModel> records, Wave wave, ReportMeasureHeaderModel measure, int index, bool total = false)
        {
            return SatifactoryByWaveCalculator.CalcValue(ShowTotalForMeasure, Measures, records, wave, measure, index,
                total);
        }

        protected override string CalcValue(List<SchoolRecordModel> records, Wave wave, ReportMeasureHeaderModel measure, int index,
          List<BenchmarkModel> benchmarks, bool total = false)
        {
            return SatifactoryByWaveCalculator.CalcValue(ShowTotalForMeasure, Measures, records, wave, measure, index, benchmarks,
                total);

        }
    }

    #endregion

    #region Host - School

    internal class SatisfactoryByWavePdfSchoolGenerator :
        SatisfactoryByWavePdfGenerator<CpallsClassModel, StudentRecordModel>
    {
        public SatisfactoryByWavePdfSchoolGenerator(SatisfactoryType type,
            IEnumerable<CpallsClassModel> sources,
            IEnumerable<ReportMeasureHeaderModel> measures,
            IEnumerable<StudentRecordModel> records,
            Dictionary<Wave, IEnumerable<int>> waves,
            IEnumerable<BenchmarkModel> benchmarks)
            : base(type, sources, measures, records, waves, benchmarks)
        {
        }

        protected override string GetSourceName(CpallsClassModel source)
        {
            throw new NotImplementedException();
        }

        protected override int GetSourceID(CpallsClassModel source)
        {
            throw new NotImplementedException();
        }

        protected override string GetTitle()
        {
            return "School";
        }

        protected override string CalcValue(CpallsClassModel source, List<StudentRecordModel> records, Wave wave, ReportMeasureHeaderModel measure, int index,
            bool total = false)
        {
            throw new NotImplementedException();
        }

        protected override string CalcValue(List<StudentRecordModel> records, Wave wave, ReportMeasureHeaderModel measure, int index, bool total = false)
        {
            return SatifactoryByWaveCalculator.CalcValue(ShowTotalForMeasure, Measures, records, wave, measure, index,
                total);
        }

        protected override string CalcValue(List<StudentRecordModel> records, Wave wave, ReportMeasureHeaderModel measure, int index,
          List<BenchmarkModel> benchmarks, bool total = false)
        {
            return SatifactoryByWaveCalculator.CalcValue(ShowTotalForMeasure, Measures, records, wave, measure, index, benchmarks,
                total);

        }
    }

    #endregion

    #region Host - Class

    internal class SatisfactoryByWavePdfClassGenerator :
        SatisfactoryByWavePdfGenerator<CpallsStudentModel, StudentRecordModel>
    {
        public SatisfactoryByWavePdfClassGenerator(SatisfactoryType type,
            IEnumerable<CpallsStudentModel> sources,
            IEnumerable<ReportMeasureHeaderModel> measures,
            IEnumerable<StudentRecordModel> records,
            Dictionary<Wave, IEnumerable<int>> waves,
            IEnumerable<BenchmarkModel> benchmarks)
            : base(type, sources, measures, records, waves, benchmarks)
        {
        }

        protected override string GetSourceName(CpallsStudentModel source)
        {
            throw new NotImplementedException();
        }

        protected override int GetSourceID(CpallsStudentModel source)
        {
            throw new NotImplementedException();
        }

        protected override string GetTitle()
        {
            return "School";
        }

        protected override string CalcValue(CpallsStudentModel source, List<StudentRecordModel> records, Wave wave, ReportMeasureHeaderModel measure, int index,
            bool total = false)
        {
            throw new NotImplementedException();
        }

        protected override string CalcValue(List<StudentRecordModel> records, Wave wave, ReportMeasureHeaderModel measure, int index, bool total = false)
        {
            return SatifactoryByWaveCalculator.CalcValue(ShowTotalForMeasure, Measures, records, wave, measure, index,
                total);
        }

        protected override string CalcValue(List<StudentRecordModel> records, Wave wave, ReportMeasureHeaderModel measure, int index,
          List<BenchmarkModel> benchmarks, bool total = false)
        {
            return SatifactoryByWaveCalculator.CalcValue(ShowTotalForMeasure, Measures, records, wave, measure, index, benchmarks,
                total);

        }
    }

    #endregion
}
