using Sunnet.Cli.Business.Ade.Models;
using Sunnet.Cli.Core.Cec.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sunnet.Cli.Business.Cec.Model
{
    public class CECOfflineTeacherModel : CecSchoolTeacherModel
    {
        public CECOfflineTeacherModel() { }

        public CECOfflineTeacherModel(CecSchoolTeacherModel teacher)
        {
            ID = teacher.ID;
            FirstName = teacher.FirstName;
            LastName = teacher.LastName;
            TeacherID = teacher.TeacherID;
            CoachId = teacher.CoachId;
            CoachFirstName = teacher.CoachFirstName;
            CoachLastName = teacher.CoachLastName;
            YearsInProjectId = teacher.YearsInProjectId;
            YearsInProject = teacher.YearsInProject;
            SchoolYear = teacher.SchoolYear;
            BOY = teacher.BOY;
            MOY = teacher.MOY;
            EOY = teacher.EOY;
            //Schools = teacher.Schools;
        }

        /// <summary>
        /// 0：无，1：有数据；2：更新失败；3： 更新成功 ; 4:上传中 对应 CECOfflineStatus
        /// </summary>
        public int ChangeStauts { get; set; }

        /// <summary>
        /// 0表示未完成，1表示完成，2表示离线完成  对应 CECStatus
        /// </summary>
        public int BOYStatus { get; set; }
        public List<CecResultModel> BOYHistory { get; set; }

        /// <summary>
        /// 0表示未完成，1表示完成，2表示离线完成 对应 CECStatus
        /// </summary>
        public int MOYStatus { get; set; }
        public List<CecResultModel> MOYHistory { get; set; }

        /// <summary>
        /// 0表示未完成，1表示完成，2表示离线完成  对应 CECStatus
        /// </summary>
        public int EOYStatus { get; set; }
        public List<CecResultModel> EOYHistory { get; set; }

    }

    public enum CECOfflineStatus
    {
        /// <summary>
        /// 无状态
        /// </summary>
        None = 0,
        
        /// <summary>
        /// 已完成
        /// </summary>
        Complete = 1,

        /// <summary>
        /// 同步失败
        /// </summary>
        Fail = 2,

        /// <summary>
        /// 同步成功
        /// </summary>
        Succeed = 3,

        /// <summary>
        /// 同步中
        /// </summary>
        Synchronizing = 4
    }

    public enum CECStatus
    {
        /// <summary>
        /// 无状态 
        /// </summary>
        None = 0,

        /// <summary>
        /// 完成
        /// </summary>
        Complete= 1,

        /// <summary>
        /// 离线完成
        /// </summary>
        OfflineComplete= 2,
    }
}