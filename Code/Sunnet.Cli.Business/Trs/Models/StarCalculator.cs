using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/**************************************************************************
 * Developer: 		JackZhang
 * Computer:		JACKZ
 * Domain:			JackZ
 * CreatedOn:		1/20 2015 9:38:31
 * Description:		Please input class summary
 * Version History:	Created,1/20 2015 9:38:31
 * 
 * 
 **************************************************************************/
using Sunnet.Cli.Core.Trs;

namespace Sunnet.Cli.Business.Trs.Models
{
    internal static class StarCalculator
    {
        /// <summary>
        /// 根据平均分计算星级.
        /// </summary>
        /// <param name="average">The average.</param>
        /// <returns></returns>
        internal static TRSStarEnum Get(decimal average)
        {
            if (average < 1.8m)
                return TRSStarEnum.Two;
            if (1.8m <= average && average < 2.4m)
                return TRSStarEnum.Three;
            if (2.4m <= average)
                return TRSStarEnum.Four;
            return TRSStarEnum.One;
        }

        /// <summary>
        /// 计算一组数据的中位数
        /// </summary>
        /// <param name="scores">The scores.</param>
        /// <returns></returns>
        internal static decimal Median(List<int> scores)
        {
            if (scores == null || !scores.Any())
                return 0m;
            if (scores.Count == 1)
                return scores.First();
            var scoresList = scores.OrderBy(x => x).ToList();
            var count = scoresList.Count;
            if (count % 2 == 0)
            {
                var index = count / 2 - 1;
                return (scoresList[index] + scoresList[index + 1] + 0m) / 2;
            }
            else
            {
                return scoresList[count / 2] + 0m;
            }
        }
    }
}
