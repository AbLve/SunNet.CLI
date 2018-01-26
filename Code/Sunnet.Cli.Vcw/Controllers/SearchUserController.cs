using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
/**************************************************************************
 * Developer: 		Joe
 * Computer:		JOE-PC
 * Domain:			Joe-pc
 * CreatedOn:		2015/4/16 12:15:10
 * Description:		Please input class summary
 * Version History:	Created,2015/4/16 12:15:10
 * 
 * 
 **************************************************************************/
using Sunnet.Cli.UIBase.Filters;
using Sunnet.Framework.Permission;
using Sunnet.Cli.UIBase.Models;
using System.Linq.Expressions;
using Sunnet.Framework.Helpers;
using Sunnet.Cli.Core.Users.Entities;
using Sunnet.Cli.Business.Users.Models.VCW;
using Sunnet.Cli.Business.Users;
using Sunnet.Cli.Core.Users.Enums;
using Sunnet.Cli.Business.Vcw.Models;

namespace Sunnet.Cli.Vcw.Controllers
{
    public class SearchUserController : BaseController
    {
        UserBusiness _userBusiness;

        public SearchUserController()
        {
            _userBusiness = new UserBusiness();
        }


        [CLIUrlAuthorizeAttribute(Account = Authority.All, PageId = PagesModel.VCW, Anonymity = Anonymous.Verified)]
        public ActionResult TeacherList()
        {
            return View();
        }


        [CLIUrlAuthorizeAttribute(Account = Authority.All, PageId = PagesModel.VCW, Anonymity = Anonymous.Verified)]
        public string SearchTeacher(int community = 0, int school = 0, string firstName = "", string lastName = "",
             string sort = "LastName", string order = "Asc", int first = 0, int count = 10)
        {
            Expression<Func<TeacherEntity, bool>> expression = PredicateHelper.True<TeacherEntity>();

            if (community > 0)
                expression = expression.And(o => o.UserInfo.UserCommunitySchools.Any(p => p.CommunityId == community));
            if (school > 0)
                expression = expression.And(o => o.UserInfo.UserCommunitySchools.Any(p => p.SchoolId == school));
            if (firstName.Trim() != string.Empty)
                expression = expression.And(o => o.UserInfo.FirstName.Contains(firstName.Trim()));
            if (lastName.Trim() != string.Empty)
                expression = expression.And(o => o.UserInfo.LastName.Contains(lastName.Trim()));

            expression = expression.And(r => r.UserInfo.Status == Core.Common.Enums.EntityStatus.Active);

            switch (UserInfo.Role)
            {
                case Role.Super_admin:
                    expression = expression.And(r => true);
                    break;
                case Role.Coordinator:
                case Role.Intervention_manager:
                    List<int> teacherids = _userBusiness.GetTeacherIdsByPM(UserInfo.ID);
                    expression = expression.And(r => teacherids.Contains(r.UserInfo.ID));
                    break;
                case Role.Mentor_coach:
                    expression = expression.And(r => r.CoachId == UserInfo.ID);
                    break;
                default:
                    expression = expression.And(r => false);
                    break;
            }

            int total;
            List<TeacherSelectModel> list = _userBusiness.GetVCWTeacherSelectModel(expression, sort, order, first, count, out total);

            return JsonHelper.SerializeObject(new { total = total, data = list });
        }


        [CLIUrlAuthorizeAttribute(Account = Authority.All, PageId = PagesModel.VCW, Anonymity = Anonymous.Verified)]
        public ActionResult CoachList()
        {
            return View();
        }

        [CLIUrlAuthorizeAttribute(Account = Authority.All, PageId = PagesModel.VCW, Anonymity = Anonymous.Verified)]
        public string SearchCoach(int community = 0, string firstName = "", string lastName = "",
             string sort = "LastName", string order = "Asc", int first = 0, int count = 10)
        {
            Expression<Func<CoordCoachEntity, bool>> expression = PredicateHelper.True<CoordCoachEntity>();

            if (community > 0)
            {
                IQueryable<int> user_Ids = _userBusiness.GetAssignedCoordCoachByCommunity(community);
                if (user_Ids != null)
                {
                    expression = expression.And(r => user_Ids.Contains(r.User.ID));
                }
            }
            if (firstName.Trim() != string.Empty)
                expression = expression.And(o => o.User.FirstName.Contains(firstName.Trim()));
            if (lastName.Trim() != string.Empty)
                expression = expression.And(o => o.User.LastName.Contains(lastName.Trim()));

            expression = expression.And(r => r.User.Status == Core.Common.Enums.EntityStatus.Active);
            expression = expression.And(r => r.User.ID != UserInfo.ID);

            switch (UserInfo.Role)
            {
                case Role.Super_admin:
                case Role.Intervention_manager:
                    expression = expression.And(r => true);
                    break;
                case Role.Coordinator:
                case Role.Mentor_coach:
                    {
                        IQueryable<int> userIds = _userBusiness.GetCoachByCoachUserId(UserInfo.ID);
                        expression = expression.And(r => userIds.Contains(r.User.ID));
                    }
                    break;
                default:
                    expression = expression.And(r => false);
                    break;
            }

            int total;
            List<CoachListModel> list = _userBusiness.GetVCWCoachSelectModel(expression, sort, order, first, count, out total);

            return JsonHelper.SerializeObject(new { total = total, data = list });
        }
    }
}