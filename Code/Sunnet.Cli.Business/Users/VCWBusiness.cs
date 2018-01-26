using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using LinqKit;
using Sunnet.Cli.Business.Users.Models;
using Sunnet.Cli.Business.Users.Models.VCW;
using Sunnet.Cli.Business.Vcw.Models;
using Sunnet.Cli.Core.Common.Enums;
using Sunnet.Cli.Core.Users.Entities;
using Sunnet.Cli.Core.Users.Enums;
using Sunnet.Framework.Extensions;

namespace Sunnet.Cli.Business.Users
{
    public partial class UserBusiness
    {
        #region VCW专用

        /// <summary>
        /// 根据CoachId查找分配给该Coach的所有teacher
        /// </summary>
        /// <param name="coachId"></param>
        /// <returns></returns>
        public List<TeacherListModel> GetAssignedTeachersByCoach(int coachId)
        {
            return userService.Teachers
                .Where(a => a.CoachId == coachId && a.UserInfo.Status == EntityStatus.Active)
                .DistinctBy(o => o.UserInfo.ID).Select(o => new TeacherListModel
                {
                    TeacherId = o.ID,
                    TeacherUserId = o.UserInfo.ID,
                    TeacherName = o.UserInfo.FirstName + " " + o.UserInfo.LastName,
                    Schools = o.UserInfo.UserCommunitySchools
                    .Where(r => r.SchoolId > 0 && r.School.Status == SchoolStatus.Active)
                    .GroupBy(x => new { x.SchoolId, x.School.Name })
                    .Select(x => new SelectItemModel()
                    {
                        ID = x.Key.SchoolId,
                        Name = x.Key.Name
                    })
                }).OrderBy(o => o.TeacherName).ToList();
        }

        /// <summary>
        /// 根据CoachId查找分配给该Coach的所有teacher的ID集合
        /// </summary>
        /// <param name="coachId"></param>
        /// <returns></returns>
        public List<int> GetAssignedTeacherIdsByCoach(int coachId)
        {
            var list = userService.Teachers
                .Where(a => a.CoachId == coachId && a.UserInfo.Status == EntityStatus.Active)
                   .Select(o => o.UserInfo.ID).ToList();
            return list;
        }

        public List<int> GetTeacherIdsByExternalUser(UserBaseEntity userInfo)
        {
            return userService.Teachers.AsExpandable()
                           .Where(GetTeacherCondition(userInfo))
                           .Select(o => o.UserInfo.ID).ToList();
        }

        /// <summary>
        /// 根据PMid获取其所在Community内的Teacher
        /// </summary>
        /// <param name="pmId"></param>
        /// <returns></returns>
        public List<SelectItemModel> GetTeacherByPM(int pmId)
        {
            return userService.Teachers
                .Where(t => t.UserInfo.Status == EntityStatus.Active
                                                   && t.UserInfo.UserCommunitySchools.Any(
                                                       u => u.Community.UserCommunitySchools.Any(v => v.UserId == pmId)))
                .Select(o => new SelectItemModel
                {
                    ID = o.UserInfo.ID,
                    Name = o.UserInfo.FirstName + " " + o.UserInfo.LastName
                }).OrderBy(a => a.Name)
                .ToList();
        }

        public List<int> GetTeacherIdsByPM(int pmId)
        {
            return userService.Teachers
                .Where(t => t.UserInfo.Status == EntityStatus.Active
                       && t.UserInfo.UserCommunitySchools.Any(
                       u => u.Community.UserCommunitySchools.Any(v => v.UserId == pmId)))
                .Select(o => o.UserInfo.ID).ToList();
        }

        /// <summary>
        /// 根据PMid获取其所在Community内的Teacher
        /// </summary>
        /// <param name="coachId"></param>
        /// <returns></returns>
        public List<TeacherListModel> GetTeacherListByPm(int pmId)
        {
            return userService.Teachers.Where(t => t.UserInfo.Status == EntityStatus.Active
                                                   && t.UserInfo.UserCommunitySchools.Any(
                                                       u => u.Community.UserCommunitySchools.Any(v => v.UserId == pmId)))
                .Select(o => new TeacherListModel
                {
                    TeacherId = o.ID,
                    Schools = o.UserInfo.UserCommunitySchools
                    .Where(r => r.SchoolId > 0 && r.School.Status == SchoolStatus.Active)
                    .GroupBy(x => new { x.SchoolId, x.School.Name })
                    .Select(x => new SelectItemModel()
                    {
                        ID = x.Key.SchoolId,
                        Name = x.Key.Name
                    }),
                    TeacherUserId = o.UserInfo.ID,
                    TeacherName = o.UserInfo.FirstName + " " + o.UserInfo.LastName
                }).OrderBy(o => o.TeacherName).ToList();
        }

        /// <summary>
        ///  查找所有teacher
        /// </summary>
        /// <returns></returns>
        public List<SelectItemModel> GetAllTeachers()
        {
            return userService.Teachers.Where(
                a => a.UserInfo.Status == EntityStatus.Active)
                .Select(b => new SelectItemModel
                {
                    ID = b.UserInfo.ID,
                    Name = b.UserInfo.FirstName + " " + b.UserInfo.LastName
                }).Distinct().OrderBy(a => a.Name)
                 .ToList();
        }

        /// <summary>
        /// 获取所有活动的老师
        /// </summary>
        /// <returns></returns>
        public List<TeacherSelectModel> GetVCWTeacherSelectModel(Expression<Func<TeacherEntity, bool>> expression,
            string sort, string order, int first, int count, out int total)
        {
            var query = userService.Teachers.AsExpandable().Where(expression)
                .Select(r => new TeacherSelectModel
                {
                    ID = r.UserInfo.ID,
                    TeacherId = r.ID,
                    FirstName = r.UserInfo.FirstName,
                    LastName = r.UserInfo.LastName
                }).OrderBy(o => o.FirstName);
            total = query.Count();
            return query.OrderBy(sort, order).Skip(first).Take(count).ToList();
        }

        /// <summary>
        /// 根据CoachId查找分配给该Coach的所有teacher
        /// </summary>
        /// <param name="coachId"></param>
        /// <returns></returns>
        public List<TeacherListModel> GetAssignedTeachersByCoach(int coachId, Expression<Func<TeacherEntity, bool>> condition,
            string sort, string order, int first, int count, out int total)
        {
            var query = userService.Teachers.AsExpandable()
                .Where(a => a.CoachId == coachId && a.UserInfo.Status == EntityStatus.Active)
                .Where(condition)
                .Select(o => new TeacherListModel
                {
                    TeacherId = o.ID,
                    TeacherUserId = o.UserInfo.ID,
                    TeacherName = o.UserInfo.FirstName + " " + o.UserInfo.LastName,
                    CommunityNames = o.UserInfo.UserCommunitySchools
                    .GroupBy(x => new { x.CommunityId, x.Community.Name }).Select(r => r.Key.Name),
                    SchoolNames = o.UserInfo.UserCommunitySchools
                    .GroupBy(x => new { x.SchoolId, x.School.Name }).Select(r => r.Key.Name)
                }).OrderBy(o => o.TeacherName);
            total = query.Count();
            var list = query.OrderBy(sort, order).Skip(first).Take(count);
            return list.ToList();
        }


        public List<TeacherListModel> GetTeachersByAdmin(Expression<Func<TeacherEntity, bool>> condition,
            string sort, string order, int first, int count, out int total)
        {
            var query = userService.Teachers.AsExpandable()
                .Where(a => a.UserInfo.Status == EntityStatus.Active)
                .Where(condition)
                .Select(o => new TeacherListModel
                {
                    TeacherId = o.ID,
                    TeacherUserId = o.UserInfo.ID,
                    TeacherName = o.UserInfo.FirstName + " " + o.UserInfo.LastName,
                    CommunityNames = o.UserInfo.UserCommunitySchools
                    .GroupBy(x => new { x.CommunityId, x.Community.Name }).Select(r => r.Key.Name),
                    SchoolNames = o.UserInfo.UserCommunitySchools
                    .GroupBy(x => new { x.SchoolId, x.School.Name }).Select(r => r.Key.Name)
                }).OrderBy(o => o.TeacherName);
            total = query.Count();
            var list = query.OrderBy(sort, order).Skip(first).Take(count);
            return list.ToList();
        }

        public List<TeacherListModel> GetTeachersByExternalUser(Expression<Func<TeacherEntity, bool>> condition, UserBaseEntity userInfo,
            string sort, string order, int first, int count, out int total)
        {
            var query = userService.Teachers.AsExpandable()
                .Where(a => a.UserInfo.Status == EntityStatus.Active)
                .Where(condition)
                .Where(GetTeacherCondition(userInfo))
                .Select(o => new TeacherListModel
                {
                    TeacherId = o.ID,
                    TeacherUserId = o.UserInfo.ID,
                    TeacherName = o.UserInfo.FirstName + " " + o.UserInfo.LastName,
                    CommunityNames = o.UserInfo.UserCommunitySchools
                    .GroupBy(x => new { x.CommunityId, x.Community.Name }).Select(r => r.Key.Name),
                    SchoolNames = o.UserInfo.UserCommunitySchools
                    .GroupBy(x => new { x.SchoolId, x.School.Name }).Select(r => r.Key.Name)
                }).OrderBy(o => o.TeacherName);
            total = query.Count();
            var list = query.OrderBy(sort, order).Skip(first).Take(count);
            return list.ToList();
        }


        /// <summary>
        /// 查找所有的Coach
        /// </summary>
        /// <returns></returns>
        public List<CoachListModel> GetVCWCoachSelectModel(Expression<Func<CoordCoachEntity, bool>> expression,
            string sort, string order, int first, int count, out int total)
        {
            var query = userService.CoordCoachs.AsExpandable().Where(expression)
                .Select(r => new CoachListModel
                {
                    UserId = r.User.ID,
                    FirstName = r.User.FirstName,
                    LastName = r.User.LastName
                });
            total = query.Count();
            return query.OrderBy(sort, order).Skip(first).Take(count).ToList();
        }

        /// <summary>
        /// 根据Coach的UserId查找 该Coach所分配的Community 下的Coach
        /// </summary>
        /// <returns></returns>
        public IQueryable<int> GetCoachByCoachUserId(int coachUserId)
        {
            //获取Coach分配的Community集合
            IQueryable<int> AssignedCommunityIds = userService.UserComSchRelations
                .Where(u => u.CommunityId > 0 && u.Community.Status == EntityStatus.Active && u.UserId == coachUserId)
                .Select(u => u.CommunityId).Distinct();
            //根据Community集合获取分配这些Community的User集合
            IQueryable<int> UserIds = userService.UserComSchRelations
                .Where(u => AssignedCommunityIds.Contains(u.CommunityId))
                .Select(u => u.UserId).Distinct();
            return UserIds;
        }


        /// <summary>
        /// 根据PMid获取其下的Coach
        /// </summary>
        /// <param name="pmId"></param>
        /// <returns></returns>
        public List<SelectItemModel> GetCoachByPM(int pmId)
        {
            return userService.IntManaCoachRelations
                 .Where(r => (r.User.Role == Role.Coordinator || r.User.Role == Role.Mentor_coach)
                     && r.User.CoordCoach != null && r.User.Status == EntityStatus.Active
                     && r.PMUserId == pmId)
                     .DistinctBy(r => r.CoordCoachUserId).Select(
                     r => new SelectItemModel()
                     {
                         ID = r.CoordCoachUserId,
                         Name = r.User.FirstName + " " + r.User.LastName
                     }).OrderBy(r => r.Name).ToList();
        }

        /// <summary>
        /// 查找所有的Coach
        /// </summary>
        /// <returns></returns>
        public List<SelectItemModel> GetCoach()
        {
            return userService.CoordCoachs.Where(
                a => a.User.Status == EntityStatus.Active)
                .Select(r => new SelectItemModel
                {
                    ID = r.User.ID,
                    Name = r.User.FirstName + " " + r.User.LastName
                }).OrderBy(a => a.Name).ToList();
        }


        public List<SelectItemModel> GetAssignedCommunities(int userId)
        {
            var query = userService.UserComSchRelations
                .Where(o => o.UserId == userId && o.CommunityId > 0 && o.Community.Status == EntityStatus.Active)
                .DistinctBy(o => o.CommunityId).Select(o => new SelectItemModel()
                {
                    ID = o.CommunityId,
                    Name = o.Community.Name
                }).OrderBy(a => a.Name);
            return query.ToList();
        }

        public IList<UserCommunityRelationModel> GetAssignedUsersByCommunity(int communityId)
        {
            var query = userService.UserComSchRelations.AsExpandable()
                .Where(o => o.CommunityId == communityId && o.Community.Status == EntityStatus.Active)
                .Select(o => new UserCommunityRelationModel()
                {
                    ID = o.ID,
                    UserId = o.UserId
                });
            return query.ToList();
        }

        public IQueryable<int> GetAssignedCoordCoachByCommunity(int communityId)
        {
            var query = userService.UserComSchRelations.AsExpandable()
                .Where(o => o.CommunityId == communityId && o.Community.Status == EntityStatus.Active
                && (o.User.Role == Role.Coordinator || o.User.Role == Role.Mentor_coach))
                .Select(o => o.UserId);
            return query;
        }

        /// <summary>
        /// Teacher筛选列表
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="isActive"></param>
        /// <returns></returns>
        public IEnumerable<SelectItemModel> GetTeacherSelectList(Expression<Func<TeacherEntity, bool>> expression, bool isActive = false)
        {
            return userService.Teachers.AsExpandable()
                .Where(expression).Where(o => o.UserInfo.Status == EntityStatus.Active || isActive == false)
                .Select(o => new SelectItemModel()
                {
                    ID = o.UserInfo.ID,
                    Name = o.UserInfo.FirstName + " " + o.UserInfo.LastName
                }).OrderBy(o => o.Name);
        }

        /// <summary>
        ///外部用户获取Teachers 
        /// </summary>
        /// <param name="userInfo"></param>
        /// <returns></returns>
        public IEnumerable<SelectItemModel> GetTeacherSelectList(UserBaseEntity userInfo)
        {
            return userService.Teachers.AsExpandable()
                .Where(GetTeacherCondition(userInfo))
                .Select(o => new SelectItemModel()
                {
                    ID = o.UserInfo.ID,
                    Name = o.UserInfo.FirstName + " " + o.UserInfo.LastName
                }).OrderBy(o => o.Name);
        }

        public List<int> GetAllPM()
        {
            return userService.BaseUsers
                .Where(u => u.Role == Role.Intervention_manager && u.Status == EntityStatus.Active)
                .Select(u => u.ID).ToList();
        }

        public TeacherListModel GetTeacherInfoByUserId(int userId)
        {
            var model = userService.BaseUsers
                .Where(r => r.ID == userId && r.Status == EntityStatus.Active)
                .Select(r => new TeacherListModel()
                {
                    TeacherUserId = r.ID,
                    CoachId = r.TeacherInfo.CoachId,
                    CommunityNames = r.UserCommunitySchools
                    .Where(x => x.CommunityId > 0 && x.Community.Status == EntityStatus.Active)
                    .GroupBy(x => new { x.CommunityId, x.Community.Name }).Select(x => x.Key.Name),
                    SchoolNames = r.UserCommunitySchools
                    .Where(x => x.SchoolId > 0 && x.School.Status == SchoolStatus.Active)
                   .GroupBy(x => new { x.SchoolId, x.School.Name }).Select(x => x.Key.Name),
                    TeacherName = r.FirstName + " " + r.LastName
                }).FirstOrDefault();

            if (model != null && model.CoachId > 0)
            {
                UserBaseModel userModel = GetUserBaseModel(model.CoachId);
                model.CoachName = userModel == null ? "" : userModel.FirstName + " " + userModel.LastName;
            }
            return model;
        }

        public List<TeacherListModel> GetTeacherInfoByUserIds(List<int> userIds)
        {
            return userService.BaseUsers
                .Where(r => userIds.Contains(r.ID) && r.Status == EntityStatus.Active)
                .Select(r => new TeacherListModel()
                {
                    TeacherUserId = r.ID,
                    CommunityNames = r.UserCommunitySchools
                    .Where(x => x.CommunityId > 0 && x.Community.Status == EntityStatus.Active)
                    .GroupBy(x => new { x.CommunityId, x.Community.Name }).Select(x => x.Key.Name),
                    SchoolNames = r.UserCommunitySchools
                    .Where(x => x.SchoolId > 0 && x.School.Status == SchoolStatus.Active)
                   .GroupBy(x => new { x.SchoolId, x.School.Name }).Select(x => x.Key.Name)
                }).ToList();
        }

        public int GetTeacherId(int userId)
        {
            return userService.Teachers.Where(t => t.UserInfo.ID == userId)
                .Select(r => r.ID).FirstOrDefault();
        }

        //查找Coach信息
        public CoachesListModel GetCoachInfoById(int userId)
        {
            return userService.BaseUsers
                .Where(r => r.ID == userId && r.Status == EntityStatus.Active)
                .Select(r => new CoachesListModel()
                {
                    CoachUserId = r.ID,
                    CoachName = r.FirstName + " " + r.LastName,
                    CommunityNames = r.UserCommunitySchools
                    .Where(x => x.CommunityId > 0 && x.Community.Status == EntityStatus.Active)
                    .GroupBy(x => new { x.CommunityId, x.Community.Name })
                    .Select(x => x.Key.Name)
                }).FirstOrDefault();
        }


        //获取用户ID 和 姓名
        public List<CoachListModel> GetFileSharedCoaches(List<int> userIds)
        {
            return userService.BaseUsers.Where(r => userIds.Contains(r.ID)).Select(r =>
                new CoachListModel
                {
                    UserId = r.ID,
                    FirstName = r.FirstName,
                    LastName = r.LastName
                }).ToList();
        }

        /// <summary>
        /// 根据coach查找分配到该用户所分配的Community下的PM集合
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public List<SelectItemModel> GetPMByCoach(int userId)
        {
            IQueryable<int> AssignedUserIds = GetCoachByCoachUserId(userId);
            List<SelectItemModel> PMs = userService.BaseUsers.Where(r => AssignedUserIds.Contains(r.ID)
                && r.Status == EntityStatus.Active && r.Role == Role.Intervention_manager)
                .Select(r => new SelectItemModel
                {
                    ID = r.ID,
                    Name = r.FirstName + " " + r.LastName
                }).ToList();
            return PMs;
        }


        /// <summary>
        /// 根据Community查找属于该Community的teacher
        /// </summary>
        /// <param name="communityId"></param>
        /// <returns></returns>
        public List<int> GetTeacheridsByCommunity(int communityId)
        {
            return userService.UserComSchRelations
                .Where(r => r.User.Role == Role.Teacher && r.CommunityId > 0
                    && r.CommunityId == communityId && r.User.Status == EntityStatus.Active)
                .Select(r => r.UserId).ToList();
        }


        /// <summary>
        /// 根据School查找属于该School的teacher
        /// </summary>
        /// <param name="schoolId"></param>
        /// <returns></returns>
        public List<int> GetTeacheridsBySchool(int schoolId)
        {
            return userService.UserComSchRelations
                .Where(r => r.User.Role == Role.Teacher && r.SchoolId > 0
                    && r.SchoolId == schoolId && r.User.Status == EntityStatus.Active)
                .Select(r => r.UserId).ToList();
        }


        public List<CoachesListModel> GetSendCoachByPM(Expression<Func<CoordCoachEntity, bool>> condition, int pmUserId,
            string sort, string order, int first, int count, out int total)
        {
            IQueryable<int> CoachIds = userService.IntManaCoachRelations
                    .Where(r => r.PMUserId == pmUserId && r.User.Status == EntityStatus.Active)
                    .Select(r => r.CoordCoachUserId);
            var query = userService.CoordCoachs.AsExpandable().Where(condition)
                .Where(r => r.User.Status == EntityStatus.Active && CoachIds.Contains(r.User.ID))
                .Select(r => new CoachesListModel()
                {
                    CoachUserId = r.User.ID,
                    CoachName = r.User.FirstName + " " + r.User.LastName,
                    CommunityNames = r.User.UserCommunitySchools
                    .Where(x => x.CommunityId > 0 && x.Community.Status == EntityStatus.Active)
                    .GroupBy(x => new { x.CommunityId, x.Community.Name })
                    .Select(x => x.Key.Name)
                });
            total = query.Count();
            return query.OrderBy(sort, order).Skip(first).Take(count).ToList();
        }

        public List<CoachesListModel> GetSendCoachByAdmin(Expression<Func<CoordCoachEntity, bool>> condition,
            string sort, string order, int first, int count, out int total)
        {
            var query = userService.CoordCoachs.AsExpandable().Where(condition)
                .Where(r => r.User.Status == EntityStatus.Active)
                .Select(r => new CoachesListModel()
                {
                    CoachUserId = r.User.ID,
                    CoachName = r.User.FirstName + " " + r.User.LastName,
                    CommunityNames = r.User.UserCommunitySchools
                    .Where(x => x.CommunityId > 0 && x.Community.Status == EntityStatus.Active)
                    .GroupBy(x => new { x.CommunityId, x.Community.Name })
                    .Select(x => x.Key.Name)
                });
            total = query.Count();
            return query.OrderBy(sort, order).Skip(first).Take(count).ToList();
        }

        #endregion
    }
}
