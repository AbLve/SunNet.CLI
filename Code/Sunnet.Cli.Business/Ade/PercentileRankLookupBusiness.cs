using System;
using System.Linq;
using Sunnet.Cli.Core.Ade.Entities;

namespace Sunnet.Cli.Business.Ade
{
    public partial class AdeBusiness
    {
        public bool PercentileRankLookup(string measureLabel)
        {
            return _adeContract.PercentileRanks.Any(e => e.MeasureLabel == measureLabel);
        }

        public string PercentileRankLookup(int measureId, DateTime birthday, decimal rawScore,DateTime stuMeasureUpdated)
        {
            int rawScoreInt = (int)rawScore;
            string percentileRank = "-";
            MeasureEntity measure = _adeContract.GetMeasure(measureId);
            if (measure.PercentileRank)
            {
                TimeSpan ts = stuMeasureUpdated.Subtract(birthday);
                int subjectAge = ts.Days;
                var percentileRankLookup =
                    _adeContract.PercentileRanks.FirstOrDefault(e => subjectAge >= e.AgeMin && subjectAge <= e.AgeMax
                                                                     && rawScoreInt == e.RawScore
                                                                     && e.MeasureLabel == measure.Label);
                if (percentileRankLookup != null)
                {
                    percentileRank = percentileRankLookup.PercentileRank;
                }
                else
                {
                    percentileRank = "N/A";
                }

                //logger.Info("PercentileRankLookup(measureId=" + measureId + ", birthday=" + birthday + ", rawScore=" +
                //            rawScore + ")=" + percentileRank
                //            + " | subjectAge=" + subjectAge + "days, studentMeasure.UpdatedOn=" + stuMeasureUpdated);
                    //David 03/21/2017, please remove this after pass the testing
            }
            return percentileRank;
        }
    }
}