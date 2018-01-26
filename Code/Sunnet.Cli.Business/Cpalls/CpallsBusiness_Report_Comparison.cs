using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
/**************************************************************************
 * Developer: 		Leason
 * Computer:		Leason-PC
 * Domain:			Leason-pc
 * CreatedOn:		2014/10/27 10:50:00
 * Description:		Create CpallsBusiness
 * Version History:	Created,2014/10/27 10:50:00
 * 
 * 
 **************************************************************************/
using Sunnet.Cli.Core.Cpalls;
using Sunnet.Cli.Core.Cpalls.Entities;
using Sunnet.Framework.Extensions;

namespace Sunnet.Cli.Business.Cpalls
{
    public partial class CpallsBusiness
    {

        /// <summary>
        /// Community下已完成Measure
        /// </summary>
        /// <param name="assessmentId"></param>
        /// <param name="schoolYear"></param>
        /// <param name="communityId"></param>
        /// <param name="schoolId"></param>
        /// <returns></returns>
        private List<MeasureModel> GetComparison(int assessmentId, string[] schoolYear, int communityId = 0, int schoolId = 0)
        {
            throw new NotImplementedException(""); 
        }

        #region School Comparison Pdf
        public Dictionary<Wave, List<ReportModel>> GetReport_School(int assessmentId, string[] schoolYear,
            Dictionary<Wave, IEnumerable<int>> waves, int communityId = 0)
        {
            throw new NotImplementedException("");
        }

        public Dictionary<Wave, List<ReportModel>> GetReport_Class(int assessmentId, string[] schoolYear,
            Dictionary<Wave, IEnumerable<int>> waves, int schoolId)
        {
            throw new NotImplementedException("");
        }

        private List<ReportModel> PrintReport(Wave wave, List<ReportModel> title, List<MeasureModel> completionMeasure, string[] schoolYear)
        {
            throw new NotImplementedException("");
        }
        #endregion
    }
}