using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sunnet.Cli.Core.Ade;
using Sunnet.Cli.Core.Cpalls;
using Sunnet.Cli.Core.Reports.Models;
using Sunnet.Framework.Core.Base;
using Sunnet.Cli.Core.Cpalls.Models.Report;
using Sunnet.Cli.Core.Practices.Entities;

/**************************************************************************
 * Developer: 		JackZhang
 * Computer:		JACKZ
 * Domain:			Jackz
 * CreatedOn:		2014/9/4 4:22:23
 * Description:		Please input class summary
 * Version History:	Created,2014/9/4 4:22:23
 * 
 * 
 **************************************************************************/


namespace Sunnet.Cli.Core.Practices.Interfaces
{
    public interface IPracticeStudentMeasureRpst : IRepository<PracticeStudentMeasureEntity, int>
    {
        int InitMeasures(int userId, int assessmentId, string schoolYear, int studentId,
          DateTime studentBirthday, Wave wave, IEnumerable<int> measureIds);

        void RecalculateParentGoal(int saId, int parentMeasureId = 0);
        void RefreshClassroom(int assessmentId, Wave wave, int userId);
        void CleanClassroom(int assessmentId);

        #region Practice Report
        List<ReportMeasureHeaderModel> GetReportMeasureHeaders(int assessmentId);


        List<StudentRecordModel> GetReportStudentRecords(int assessmentId, string schoolYear, DateTime startDate,
            DateTime endDate, IEnumerable<int> studentIds, int userId);
        List<WaveFinishedOnModel> GetWaveFinishedDate(int assessmentId,int userId);

        #endregion

    }
}
