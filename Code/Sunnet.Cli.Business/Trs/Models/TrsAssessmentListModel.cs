using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/**************************************************************************
 * Developer: 		JackZhang
 * Computer:		JACKZ
 * Domain:			JackZ
 * CreatedOn:		1/16 2015 15:22:38
 * Description:		列表页显示的模型
 * Version History:	Created,1/16 2015 15:22:38
 * 
 * 
 **************************************************************************/
using Sunnet.Cli.Business.Common;
using Sunnet.Cli.Core.Trs;
using Sunnet.Cli.Core.Users.Entities;
using Sunnet.Cli.Core.Users.Enums;

namespace Sunnet.Cli.Business.Trs.Models
{
    public class TrsAssessmentListModel
    {

        public int Id { get; set; }

        public int SchoolId { get; set; }

        public TrsSchoolModel School { get; set; }

        public TRSStatusEnum Status { get; set; }

        /// <summary>
        /// 如果Status 是Completed, 但是该属性未赋值(0), 那么该条记录为School页面修改Verified Star时产生
        /// </summary>
        [DisplayName("Calculated Star Designation")]
        public TRSStarEnum Star { get; set; }

        [DisplayName("Verified Star")]
        public TRSStarEnum VerifiedStar { get; set; }

        public TrsAssessmentType Type { get; set; }

        public DateTime CreatedOn { get; set; }

        public DateTime UpdatedOn { get; set; }

        public IEnumerable<int> ClassIds { get; set; }

        public List<TrsClassModel> Classes { get; set; }

        private DateTime _approveDate;
        [DisplayName("Approval Date")]
        public DateTime ApproveDate
        {
            get
            {
                if (_approveDate < CommonAgent.MinDate)
                    _approveDate = CommonAgent.MinDate;
                return _approveDate;
            }
            set { _approveDate = value; }
        }

        public void UpdateAction(UserBaseEntity user)
        {
            if (this.School == null)
                return;

            bool isPrincipal = false;
            bool isAssessor = false;
            bool isClassAssessor = false;
            bool isClassMentor = false;


            if (user.Role == Role.Principal || user.Role == Role.Principal_Delegate
                || user.Role == Role.TRS_Specialist || user.Role == Role.TRS_Specialist_Delegate)
            {
                //代理同父级操作权限相同
                isPrincipal = this.School.Principals.Any(x => x.UserId == user.ID)
                    || this.School.Principals.Any(x => x.UserId == user.Principal.ParentId && user.Principal.ParentId > 0);
                isAssessor = this.School.Assessor.UserId > 0
                    && (this.School.Assessor.UserId == user.ID || this.School.Assessor.UserId == user.Principal.ParentId);
                isClassAssessor = this.Classes.Any(r => r.TrsAssessorId == user.ID)
                    || this.Classes.Any(r => r.TrsAssessorId == user.Principal.ParentId && user.Principal.ParentId > 0);
                isClassMentor = this.Classes.Any(r => r.TrsMentorId == user.ID)
                    || this.Classes.Any(r => r.TrsMentorId == user.Principal.ParentId && user.Principal.ParentId > 0);
            }

            var isCommunity = user.CommunityUser != null &&
                              user.UserCommunitySchools.Any(x => School.CommunityIds.Contains(x.CommunityId));
            if (isPrincipal || isCommunity)
                Action = Status == TRSStatusEnum.Completed ? "view" : "";

            var isAdmin = user.Role == Role.Super_admin;
            if (isClassMentor)
                Action = Status == TRSStatusEnum.Completed ? "" : "viewAssessment";
            if (isClassAssessor)
                Action = Status == TRSStatusEnum.Completed ? "" : "classedit";
            if (isAdmin || isAssessor)
                Action = Status == TRSStatusEnum.Completed ? "invalidate" : "edit";

        }

        /// <summary>
        /// Assessment可用操作：
        /// view: 已完成，可以下载报表
        /// invalidate: 已完成，可以下载报表，可以取消Finalize
        /// edit: 未完成，继续完成Assessement
        /// </summary>
        public string Action { get; set; }

        public EventLogType EventLogType { get; set; }

    }
}
