using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sunnet.Cli.Business.Cpalls.Models;
using Sunnet.Cli.Core.Cpalls;
using Sunnet.Cli.Core.Cpalls.Models.Report;
using Sunnet.Cli.Business.Ade.Models;
using Sunnet.Cli.Business.Practices.Models;

namespace Sunnet.Cli.Business.Cpalls
{
    #region Host - Community
    internal class SatisfactoryBySourcePdfCommunityGenerator :
        SatisfactoryBySourcePdfGenerator<CpallsCommunityModel, StudentRecordModel>
    {
        public SatisfactoryBySourcePdfCommunityGenerator(IEnumerable<CpallsCommunityModel> sources,
            IEnumerable<ReportMeasureHeaderModel> measures,
            IEnumerable<StudentRecordModel> records,
            Dictionary<Wave, IEnumerable<int>> waves,
            IEnumerable<BenchmarkModel> benchmarks)
            : base(sources, measures, records, waves, benchmarks)
        {
        }

        protected override string GetTitle()
        {
            return "Community";
        }

        protected override string GetSourceName(CpallsCommunityModel source)
        {
            return source.Name;
        }

        protected override int GetSourceID(CpallsCommunityModel source)
        {
            return source.ID;
        }

        protected override string CalcValue(CpallsCommunityModel source, List<StudentRecordModel> records, Wave wave, ReportMeasureHeaderModel measure, int index,
            bool total = false)
        {
            return SatifactoryByWaveCalculator.CalcValue(source, ShowTotalForMeasure, Measures, records, wave, measure, index,
                total);
        }

        protected override string CalcValue(List<StudentRecordModel> records, Wave wave, ReportMeasureHeaderModel measure, int index,
          List<BenchmarkModel> benchmarks, bool total = false)
        {
            return SatifactoryByWaveCalculator.CalcValue(ShowTotalForMeasure, Measures, records, wave, measure, index, benchmarks,
                   total);
        }

        protected override string CalcValue(List<StudentRecordModel> records, Wave wave, ReportMeasureHeaderModel measure, int index, bool total = false)
        {
            throw new NotImplementedException();
        }
    }

    #endregion

    #region Host - School

    internal class SatisfactoryBySourcePdfSchoolGenerator :
        SatisfactoryBySourcePdfGenerator<CpallsSchoolModel, StudentRecordModel>
    {
        public SatisfactoryBySourcePdfSchoolGenerator(IEnumerable<CpallsSchoolModel> sources,
            IEnumerable<ReportMeasureHeaderModel> measures,
            IEnumerable<StudentRecordModel> records,
            Dictionary<Wave, IEnumerable<int>> waves,
            IEnumerable<BenchmarkModel> benchmarks)
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

        protected override string CalcValue(CpallsSchoolModel source, List<StudentRecordModel> records, Wave wave, ReportMeasureHeaderModel measure, int index,
            bool total = false)
        {
            return SatifactoryByWaveCalculator.CalcValue(source, ShowTotalForMeasure, Measures, records, wave, measure, index,
                total);

        }

        protected override string CalcValue(List<StudentRecordModel> records, Wave wave, ReportMeasureHeaderModel measure, int index,
          List<BenchmarkModel> benchmarks, bool total = false)
        {
            return SatifactoryByWaveCalculator.CalcValue(ShowTotalForMeasure, Measures, records, wave, measure, index, benchmarks,
                total);
        }

        protected override string CalcValue(List<StudentRecordModel> records, Wave wave, ReportMeasureHeaderModel measure, int index, bool total = false)
        {
            throw new NotImplementedException();
        }
    }

    #endregion

    #region Host - Class

    internal class SatisfactoryBySourcePdfClassGenerator :
        SatisfactoryBySourcePdfGenerator<CpallsClassModel, StudentRecordModel>
    {
        public SatisfactoryBySourcePdfClassGenerator(IEnumerable<CpallsClassModel> sources,
            IEnumerable<ReportMeasureHeaderModel> measures,
            IEnumerable<StudentRecordModel> records,
            Dictionary<Wave, IEnumerable<int>> waves,
            IEnumerable<BenchmarkModel> benchmarks)
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

        protected override string CalcValue(CpallsClassModel source, List<StudentRecordModel> records, Wave wave, ReportMeasureHeaderModel measure, int index,
            bool total = false)
        {
            return SatifactoryByWaveCalculator.CalcValue(source, ShowTotalForMeasure, Measures, records, wave, measure, index,
                total);

        }

        protected override string CalcValue(List<StudentRecordModel> records, Wave wave, ReportMeasureHeaderModel measure, int index,
         List<BenchmarkModel> benchmarks, bool total = false)
        {
            //return SatifactoryByWaveCalculator.CalcValue(records, ShowTotalForMeasure, Measures, records, wave, measure, index,
            //    total);
            return SatifactoryByWaveCalculator.CalcValue(ShowTotalForMeasure, Measures, records, wave, measure, index, benchmarks,
                total);

        }

        protected override string CalcValue(List<StudentRecordModel> records, Wave wave, ReportMeasureHeaderModel measure, int index, bool total = false)
        {
            throw new NotImplementedException();
        }
    }

    #endregion
    #region Host - Practice benchmark Class

    internal class SatisfactoryBySourcePdfPracticeClassGenerator :
        SatisfactoryBySourcePdfGenerator<PracticeAssessmentModel, StudentRecordModel>
    {
        public SatisfactoryBySourcePdfPracticeClassGenerator(IEnumerable<PracticeAssessmentModel> sources,
            IEnumerable<ReportMeasureHeaderModel> measures,
            IEnumerable<StudentRecordModel> records,
            Dictionary<Wave, IEnumerable<int>> waves,
            IEnumerable<BenchmarkModel> benchmarks)
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

        protected override string CalcValue(PracticeAssessmentModel source, List<StudentRecordModel> records, Wave wave, ReportMeasureHeaderModel measure, int index,
            bool total = false)
        {
            return SatifactoryByWaveCalculator.CalcValue(source, ShowTotalForMeasure, Measures, records, wave, measure, index,
                total);
        }

        protected override string CalcValue(List<StudentRecordModel> records, Wave wave, ReportMeasureHeaderModel measure, int index,
         List<BenchmarkModel> benchmarks, bool total = false)
        {
            return SatifactoryByWaveCalculator.CalcValue(ShowTotalForMeasure, Measures, records, wave, measure, index, benchmarks,
                total);
        }

        protected override string CalcValue(List<StudentRecordModel> records, Wave wave, ReportMeasureHeaderModel measure, int index, bool total = false)
        {
            throw new NotImplementedException();
        }
    }

    #endregion
}
