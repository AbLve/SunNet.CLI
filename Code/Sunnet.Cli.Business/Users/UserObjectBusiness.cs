using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using LinqKit;
using Sunnet.Cli.Business.Communities;
using Sunnet.Cli.Business.Cpalls.Models;
using Sunnet.Cli.Business.Permission.Models;
using Sunnet.Cli.Business.Schools.Models;
using Sunnet.Cli.Business.Users.Models;
using Sunnet.Cli.Core.Classes.Entites;
using Sunnet.Cli.Core.Common.Enums;
using Sunnet.Cli.Core.Communities.Entities;
using Sunnet.Cli.Core.Schools.Entities;
using Sunnet.Cli.Core.Users.Configurations;
using Sunnet.Cli.Core.Users.Entities;
using Sunnet.Cli.Core.Users.Enums;
using Sunnet.Framework.Core.Tool;
using Sunnet.Framework.Core.Extensions;
using Sunnet.Framework.Extensions;
using Sunnet.Framework.Helpers;
using Sunnet.Framework.Extensions;
using Sunnet.Cli.Business.Classes.Models;

namespace Sunnet.Cli.Business.Users
{
    public partial class UserBusiness
    {
        public OperationResult InsertUserCommunitySchoolRelations(int userId, int currentUserId, int communityId, int schoolId)
        {
            List<UserComSchRelationEntity> list = new List<UserComSchRelationEntity>();
            UserComSchRelationEntity entity = new UserComSchRelationEntity();
            entity.UserId = userId;
            entity.CommunityId = communityId;
            entity.SchoolId = schoolId;
            entity.CreatedOn = DateTime.Now;
            entity.UpdatedOn = DateTime.Now;
            entity.CreatedBy = currentUserId;
            entity.UpdatedBy = currentUserId;
            entity.Status = EntityStatus.Active;
            list.Add(entity);
            return userService.InsertUserCommunitySchoolRelations(list);
        }

        #region Assign and Unassign multi users to School
        public OperationResult RemoveUserSchoolRelations(int[] userIds, int schoolId)
        {
            var list = userService.UserComSchRelations.Where(
                 o => userIds.Contains(o.UserId) && o.SchoolId == schoolId && schoolId > 0 && o.CommunityId == 0).ToList();

            return userService.DelUserCommunitySchoolRelations(list);
        }
        public OperationResult InsertUserSchoolRelations(int[] userIds, int currentUserId, int schoolId)
        {
            List<UserComSchRelationEntity> list = new List<UserComSchRelationEntity>();
            foreach (int userId in userIds)
            {
                UserComSchRelationEntity entity = new UserComSchRelationEntity();
                entity.UserId = userId;
                entity.CommunityId = 0;
                entity.SchoolId = schoolId;
                entity.CreatedOn = DateTime.Now;
                entity.UpdatedOn = DateTime.Now;
                entity.CreatedBy = currentUserId;
                entity.UpdatedBy = currentUserId;
                entity.AccessType = AccessType.FullAccess;
                entity.Status = EntityStatus.Active;
                if (!(userService.UserComSchRelations.Any(o => o.UserId == userId && o.SchoolId == schoolId && schoolId > 0)))
                    list.Add(entity);
            }
            return userService.InsertUserCommunitySchoolRelations(list);
        }
        public OperationResult InsertUserSchoolRelations(int[] userIds, int currentUserId, int schoolId, AccessType type)
        {
            List<UserComSchRelationEntity> list = new List<UserComSchRelationEntity>();
            foreach (int userId in userIds)
            {
                UserComSchRelationEntity entity = new UserComSchRelationEntity();
                entity.UserId = userId;
                entity.CommunityId = 0;
                entity.SchoolId = schoolId;
                entity.CreatedOn = DateTime.Now;
                entity.UpdatedOn = DateTime.Now;
                entity.CreatedBy = currentUserId;
                entity.UpdatedBy = currentUserId;
                entity.AccessType = type;
                entity.Status = EntityStatus.Active;
                if (!(userService.UserComSchRelations.Any(o => o.UserId == userId && o.SchoolId == schoolId && schoolId > 0)))
                    list.Add(entity);
            }
            return userService.InsertUserCommunitySchoolRelations(list);
        }
        #endregion

        #region Assign multi community to user

        public OperationResult InsertUserCommunityRelations(int userId, int currentUserId, int[] comIds)
        {
            List<UserComSchRelationEntity> list = new List<UserComSchRelationEntity>();
            var user = GetUser(userId);
            foreach (int comId in comIds)
            {
                UserComSchRelationEntity entity = new UserComSchRelationEntity();
                entity.UserId = userId;
                entity.CommunityId = comId;
                entity.SchoolId = 0;
                entity.CreatedOn = DateTime.Now;
                entity.UpdatedOn = DateTime.Now;
                entity.CreatedBy = currentUserId;
                entity.UpdatedBy = currentUserId;
                entity.Status = EntityStatus.Active;
                list.Add(entity);
            }
            return userService.InsertUserCommunitySchoolRelations(list);
        }


        public OperationResult DeleteUserCommunityRelations(int userId, int[] comIds)
        {
            IList<UserComSchRelationEntity> list = userService.UserComSchRelations.
                Where(o => o.UserId == userId && comIds.Contains(o.CommunityId)).ToList();

            OperationResult result = new OperationResult(OperationResultType.Success);

            //如果Teacher去掉与某Community的关系，需要同步删除其下分配的Class
            var user = GetUser(userId);
            if (user.Role == Role.Teacher)
            {
                List<int> deleteSchoolIds = new List<int>();
                deleteSchoolIds =
                    user.UserCommunitySchools.Where(e => comIds.Contains(e.CommunityId) && e.SchoolId > 0)
                        .Select(o => o.SchoolId).ToList();
                if (deleteSchoolIds.Count > 0)
                {
                    while (user.TeacherInfo.Classes.Where(e => deleteSchoolIds.Contains(e.SchoolId)).ToList().Count > 0)
                    {
                        user.TeacherInfo.Classes.Remove(
                            user.TeacherInfo.Classes.Where(e => deleteSchoolIds.Contains(e.SchoolId)).ToList().First());
                    }
                    UpdateUser(user);
                }
            }
            else if (user.Role == Role.Statewide || user.Role == Role.Community ||
                     user.Role == Role.District_Community_Specialist)
            {
                List<int> schoolIds = schoolBusiness.GetSchoolIds(comIds.ToList());
                userService.DelUserClassRelations(
                    user.UserClasses.Where(o => schoolIds.Contains(o.Class.SchoolId) && o.Class.IsDeleted == false).ToList());
            }
            result = userService.DelUserCommunitySchoolRelations(list);
            return result;
        }

        public List<int> GetAssignedCommunityIds(int userId)
        {
            return
                userService.UserComSchRelations.Where(
                    o => o.UserId == userId && o.Community.Status == EntityStatus.Active)
                    .Select(e => e.CommunityId)
                    .ToList();
        }

        public List<int> GetAssignedCommunityIdsForPrincipal(int userId)
        {
            var schoolComs = userService.UserComSchRelations.Where(o => o.UserId == userId)
                .Select(s => s.School.CommunitySchoolRelations.Select(c => c.CommunityId))
                .ToList();
            var coms = new List<int>();
            schoolComs.ForEach(coms.AddRange);
            return coms.Distinct().ToList();
        }

        public IList<UserCommunityRelationModel> GetAssignedCommunities(
            Expression<Func<UserComSchRelationEntity, bool>> condition,
            string sort, string order, int first, int count, out int total)
        {
            var query =
                userService.UserComSchRelations.AsExpandable()
                    .Where(condition)
                    .Select(o => new UserCommunityRelationModel()
                    {
                        ID = o.ID,
                        CommunityId = o.CommunityId,
                        CommunityName = o.Community.BasicCommunity.Name
                    });
            total = query.Count();
            var list = query.OrderBy(sort, order).Skip(first).Take(count);
            return list.ToList();
        }

        /// <summary>
        /// Gets the communities from UserCommunityRelations.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <returns></returns>
        public List<int> GetCommunities(int userId)
        {
            return
                userService.UserComSchRelations.Where(x => x.UserId == userId).Select(x => x.CommunityId).ToList();
        }

        public List<int> GetUserIdsByCommunityIds(List<int> communityIds)
        {
            return
                userService.UserComSchRelations.Where(x => communityIds.Contains(x.CommunityId))
                    .Select(x => x.UserId)
                    .ToList();
        }
        #endregion

        #region Assign multi school to user
        public OperationResult InsertUserSchoolRelations(int userId, int currentUserId, int[] schoolIds)
        {
            List<UserComSchRelationEntity> list = new List<UserComSchRelationEntity>();
            foreach (int schoolId in schoolIds)
            {
                UserComSchRelationEntity entity = new UserComSchRelationEntity();
                entity.UserId = userId;
                entity.CommunityId = 0;
                entity.SchoolId = schoolId;
                entity.CreatedOn = DateTime.Now;
                entity.UpdatedOn = DateTime.Now;
                entity.CreatedBy = currentUserId;
                entity.UpdatedBy = currentUserId;
                entity.Status = EntityStatus.Active;
                list.Add(entity);
            }
            return userService.InsertUserCommunitySchoolRelations(list);
        }

        public OperationResult DeleteUserSchoolRelations(int userId, int[] schoolIds)
        {
            IList<UserComSchRelationEntity> list =
                userService.UserComSchRelations.Where(o => o.UserId == userId && schoolIds.Contains(o.SchoolId))
                    .ToList();
            var user = GetUser(userId);
            if (user.Role == Role.School_Specialist)
            {
                userService.DelUserClassRelations(
                    user.UserClasses.Where(o => schoolIds.Contains(o.Class.SchoolId) && o.Class.IsDeleted == false).ToList());
            }
            return userService.DelUserCommunitySchoolRelations(list);
        }

        public OperationResult DeleteUserSchoolRelations(int userId, List<int> communityids, List<int> schoolIds)
        {
            IList<UserComSchRelationEntity> list =
                userService.UserComSchRelations.Where(o =>
                    o.UserId == userId
                    && communityids.Contains(o.CommunityId)
                    && schoolIds.Contains(o.SchoolId))
                    .ToList();
            return userService.DelUserCommunitySchoolRelations(list);
        }

        public List<AssignSchoolModel> GetAssignedSchools(Expression<Func<UserComSchRelationEntity, bool>> condition,
          string sort, string order, int first, int count, out int total)
        {
            var query = userService.UserComSchRelations.AsExpandable().Where(condition).Select(o => new AssignSchoolModel()
            {
                SchoolId = o.SchoolId,
                SchoolName = o.School.Name,
                CommunityNames = o.School.CommunitySchoolRelations.Select(p => p.Community.Name)
            });
            total = query.Count();
            var list = query.OrderBy(sort, order).Skip(first).Take(count);
            return list.ToList();
        }

        public List<AssignSchoolModel> GetTeacherAssignedSchools(Expression<Func<UserComSchRelationEntity, bool>> condition,
          string sort, string order, int first, int count, out int total)
        {
            var query = userService.UserComSchRelations.AsExpandable().Where(condition).Select(o => new AssignSchoolModel()
            {
                SchoolId = o.SchoolId,
                SchoolName = o.School.Name,
                CommunityId = o.CommunityId,
                CommunityName = o.Community.Name
            });
            total = query.Count();
            var list = query.OrderBy(sort, order).Skip(first).Take(count);
            return list.ToList();
        }

        public List<AssignSchoolModel> GetAssignedTrsSpecialistSchools(Expression<Func<UserComSchRelationEntity, bool>> condition,
          string sort, string order, int first, int count, out int total)
        {
            var query = userService.UserComSchRelations.AsExpandable().Where(condition).Select(o => new AssignSchoolModel()
            {
                SchoolId = o.SchoolId,
                SchoolName = o.School.Name,
                CommunityNames = o.School.CommunitySchoolRelations.Select(p => p.Community.Name)
            });
            total = query.Count();
            var list = query.OrderBy(sort, order).Skip(first).Take(count);
            return list.ToList();
        }

        public List<SelectItemModel> GetSchoolSpecialist(int[] schoolIds)
        {
            return userService.UserComSchRelations.Where(
                x => schoolIds.Contains(x.SchoolId)
                     && x.User.Role == Role.TRS_Specialist
                     && x.User.IsDeleted == false
                     && x.User.Status == EntityStatus.Active)
                .Select(x => new SelectItemModel()
                {
                    ID = x.UserId,
                    Name = x.User.FirstName + " " + x.User.LastName
                }).DistinctBy(x => x.ID).OrderBy(x => x.Name).ToList();
        }
        #endregion

        #region Assign multi school to teacher
        /// <param name="userId"></param>
        /// <param name="currentUserId"></param>
        /// <param name="communitySchoolIds">communityId,schoolId</param>
        /// <returns></returns>
        public OperationResult InsertTeacherSchoolRelations(int userId, int currentUserId, string[] communitySchoolIds)
        {
            List<UserComSchRelationEntity> list = new List<UserComSchRelationEntity>();
            for (int i = 0; i < communitySchoolIds.Length; i++)
            {
                UserComSchRelationEntity entity = new UserComSchRelationEntity();
                entity.UserId = userId;
                if (communitySchoolIds[i].Split(',').Length > 0)
                {
                    entity.CommunityId = Convert.ToInt32(communitySchoolIds[i].Split(',')[0]);
                    entity.SchoolId = Convert.ToInt32(communitySchoolIds[i].Split(',')[1]);
                }
                entity.CreatedOn = DateTime.Now;
                entity.UpdatedOn = DateTime.Now;
                entity.CreatedBy = currentUserId;
                entity.UpdatedBy = currentUserId;
                entity.Status = EntityStatus.Active;
                list.Add(entity);
            }
            List<int> insertCommunityIds = list.Select(e => e.CommunityId).ToList();
            List<int> existCommunityIds =
                userService.UserComSchRelations.Where(
                    e => e.UserId == userId && insertCommunityIds.Contains(e.CommunityId) && e.SchoolId == 0)
                    .Select(x => x.CommunityId)
                    .ToList();
            DeleteUserCommunityRelations(userId, existCommunityIds.ToArray());

            return userService.InsertUserCommunitySchoolRelations(list);
        }

        /// <param name="userId"></param>
        /// <param name="communitySchoolIds">communityId,schoolId</param>
        /// <returns></returns>
        public OperationResult DeleteTeacherSchoolRelations(int userId, string[] communitySchoolIds)
        {
            List<int> communityIds = new List<int>();
            List<int> schoolIds = new List<int>();
            if (communitySchoolIds.Length > 0)
            {
                for (int i = 0; i < communitySchoolIds.Length; i++)
                {
                    if (communitySchoolIds[i].Split(',').Length > 0)
                    {
                        communityIds.Add(Convert.ToInt32(communitySchoolIds[i].Split(',')[0]));
                        schoolIds.Add(Convert.ToInt32(communitySchoolIds[i].Split(',')[1]));
                    }
                }
            }
            IList<UserComSchRelationEntity> list = userService.UserComSchRelations.
                Where(o => o.UserId == userId && communityIds.Contains(o.CommunityId) && schoolIds.Contains(o.SchoolId))
                .ToList();
            OperationResult result = new OperationResult(OperationResultType.Success);
            result = userService.DelUserCommunitySchoolRelations(list);

            //如果用户分配的多个相同名称的School关系被全部删除了，则需要删除Teacher所属School下分配的相关Class
            var user = GetUser(userId);
            if (user.Role == Role.Teacher)
            {
                List<int> deleteSchoolIds = new List<int>();
                foreach (var entity in list)
                {
                    if (!(user.UserCommunitySchools.Any(o => entity.SchoolId == o.SchoolId)))
                        deleteSchoolIds.Add(entity.SchoolId);
                }
                if (deleteSchoolIds.Count > 0)
                {
                    while (user.TeacherInfo.Classes.Where(e => deleteSchoolIds.Contains(e.SchoolId)).ToList().Count > 0)
                    {
                        user.TeacherInfo.Classes.Remove(
                            user.TeacherInfo.Classes.Where(e => deleteSchoolIds.Contains(e.SchoolId)).ToList().First());
                    }
                    UpdateUser(user);
                }
            }
            return result;
        }

        #endregion

        #region User Class Relations
        public OperationResult InsertUserClassRelationsMoreClass(int userId, int currentUserId, int[] classIds)
        {
            List<UserClassRelationEntity> list = new List<UserClassRelationEntity>();
            foreach (int classId in classIds)
            {
                UserClassRelationEntity entity = new UserClassRelationEntity();
                entity.UserId = userId;
                entity.ClassId = classId;
                entity.CreatedOn = DateTime.Now;
                entity.UpdatedOn = DateTime.Now;
                entity.CreatedBy = currentUserId;
                entity.UpdatedBy = currentUserId;
                entity.Status = EntityStatus.Active;
                list.Add(entity);
            }
            return userService.InsertUserClassRelations(list);
        }

        public OperationResult InsertUserClassRelationsMoreUser(int[] userIds, int currentUserId, int classId)
        {
            List<UserClassRelationEntity> list = new List<UserClassRelationEntity>();
            foreach (int userId in userIds)
            {
                UserClassRelationEntity entity = new UserClassRelationEntity();
                entity.UserId = userId;
                entity.ClassId = classId;
                entity.CreatedOn = DateTime.Now;
                entity.UpdatedOn = DateTime.Now;
                entity.CreatedBy = currentUserId;
                entity.UpdatedBy = currentUserId;
                entity.Status = EntityStatus.Active;
                list.Add(entity);
            }
            return userService.InsertUserClassRelations(list);
        }

        public List<AssignClassToSpecialistModel> GetAssignedClasses(Expression<Func<UserClassRelationEntity, bool>> condition,
          string sort, string order, int first, int count, out int total)
        {
            var query = userService.UserClassRelations.AsExpandable().Where(condition).Select(o => new AssignClassToSpecialistModel()
            {
                ID = o.ClassId,
                ClassName = o.Class.Name,
                SchoolName = o.Class.School.Name,
                CommunityNameList = o.Class.School.CommunitySchoolRelations.Select(p => p.Community.Name)
            });
            total = query.Count();
            var list = query.OrderBy(sort, order).Skip(first).Take(count);
            return list.ToList();
        }

        public OperationResult DeleteUserClassRelationsMoreClass(int[] classIds, int userId)
        {
            IList<UserClassRelationEntity> list =
                userService.UserClassRelations.Where(o => o.UserId == userId && classIds.Contains(o.ClassId))
                    .ToList();
            return userService.DelUserClassRelations(list);
        }

        public OperationResult DeleteUserClassRelationsMoreUser(int classId, int[] userIds)
        {
            List<UserClassRelationEntity> list = userService.UserClassRelations
                .Where(r => r.ClassId == classId && userIds.Contains(r.UserId)).ToList();
            return userService.DelUserClassRelations(list);
        }
        #endregion

        #region Check User Existed Status

        public UserExistedStatus CheckStatewideExistedStatus(string firstname, string lastname,
            string email, Role role, out Role existedUserRole)
        {
            existedUserRole = Role.Super_admin;

            List<UserBaseEntity> listExistUser =
                userService.BaseUsers.Where(
                    x => x.Status == EntityStatus.Active
                         && x.IsDeleted == false
                         && x.FirstName == firstname
                         && x.LastName == lastname
                         && x.PrimaryEmailAddress == email).ToList();
            if (!listExistUser.Any())
                return UserExistedStatus.NotExisted;

            if (listExistUser.Any(e => e.Role == role))
                existedUserRole = role;
            else
                existedUserRole = listExistUser.FirstOrDefault().Role;

            return UserExistedStatus.UserExisted;
        }

        public UserExistedStatus CheckCommunityUserExistedStatus(int userId, string firstname, string lastname,
            string email, Role role, int communityId, out int existedUserId, out Role existedUserRole)
        {
            existedUserId = 0;
            existedUserRole = Role.Super_admin;

            List<UserBaseEntity> listExistUser =
                userService.BaseUsers.Where(
                    x => x.Status == EntityStatus.Active
                         && x.IsDeleted == false
                         && x.FirstName == firstname
                         && x.LastName == lastname
                         && x.PrimaryEmailAddress == email && x.ID != userId).ToList();
            if (!listExistUser.Any())
                return UserExistedStatus.NotExisted;

            if (listExistUser.Any(e => e.Role == role))
            {
                existedUserId = listExistUser.Where(e => e.Role == role).FirstOrDefault().ID;
                existedUserRole = role;
                List<int> userIds = listExistUser.Where(e => e.Role == role).Select(e => e.ID).ToList<int>();

                var existedInCommunity =
                    userService.UserComSchRelations.Any(
                        e => userIds.Contains(e.UserId) && e.CommunityId == communityId);
                if (existedInCommunity)
                    return UserExistedStatus.ExistedInCommunity;
            }
            else
                existedUserRole = listExistUser.FirstOrDefault().Role;

            return UserExistedStatus.UserExisted;
        }

        public UserExistedStatus CheckPrincipalUserExistedStatus(int userId, string firstname, string lastname,
            string email, Role role, int schoolId, out int existedUserId, out Role existedUserRole)
        {
            existedUserId = 0;
            existedUserRole = Role.Super_admin;

            List<UserBaseEntity> listExistUser =
                userService.BaseUsers.Where(
                    x => x.Status == EntityStatus.Active
                         && x.IsDeleted == false
                         && x.FirstName == firstname
                         && x.LastName == lastname
                         && x.PrimaryEmailAddress == email && x.ID != userId).ToList();
            if (!listExistUser.Any())
                return UserExistedStatus.NotExisted;

            if (listExistUser.Any(e => e.Role == role))
            {
                existedUserId = listExistUser.Where(e => e.Role == role).FirstOrDefault().ID;
                existedUserRole = role;
                List<int> userIds = listExistUser.Where(e => e.Role == role).Select(e => e.ID).ToList<int>();
                var existedInSchool =
                    userService.UserComSchRelations.Any(e => userIds.Contains(e.UserId) && e.SchoolId == schoolId);
                if (existedInSchool)
                    return UserExistedStatus.ExistedInSchool;
            }
            else
                existedUserRole = listExistUser.FirstOrDefault().Role;

            return UserExistedStatus.UserExisted;
        }

        public UserExistedStatus CheckTeacherUserExistedStatus(int userId, string firstname, string lastname,
            string email, Role role, int communityId, int schoolId, out int existedUserId, out Role existedUserRole)
        {
            existedUserId = 0;
            existedUserRole = Role.Super_admin;

            List<UserBaseEntity> listExistUser =
                userService.BaseUsers.Where(
                    x => x.Status == EntityStatus.Active
                         && x.IsDeleted == false
                         && x.FirstName == firstname
                         && x.LastName == lastname
                         && x.PrimaryEmailAddress == email && (x.ID != userId)).ToList();
            if (!listExistUser.Any())
                return UserExistedStatus.NotExisted;

            if (listExistUser.Any(e => e.Role == role))
            {
                existedUserId = listExistUser.Where(e => e.Role == role).FirstOrDefault().ID;
                existedUserRole = role;

                List<int> userIds = listExistUser.Where(e => e.Role == role).Select(e => e.ID).ToList<int>();
                if (schoolId > 0)
                {
                    var existedInSchool =
                        userService.UserComSchRelations.Any(
                            e => userIds.Contains(e.UserId) && e.CommunityId == communityId && e.SchoolId == schoolId);
                    if (existedInSchool)
                        return UserExistedStatus.ExistedInSchool;
                }
                else
                {
                    var existedInCommunity =
                        userService.UserComSchRelations.Any(
                            e => userIds.Contains(e.UserId) && e.CommunityId == communityId);
                    if (existedInCommunity)
                    {
                        return UserExistedStatus.ExistedInCommunity;
                    }
                }
            }
            else
                existedUserRole = listExistUser.FirstOrDefault().Role;

            return UserExistedStatus.UserExisted;
        }

        public bool CheckUserExistedStatus(int userId, string firstname, string lastname,
            string email, Role role, out OperationResult result)
        {
            result = new OperationResult(OperationResultType.Success);
            List<UserBaseEntity> listExistUser =
                userService.BaseUsers.Where(
                    x => x.Status == EntityStatus.Active
                         && x.IsDeleted == false
                         && x.FirstName == firstname
                         && x.LastName == lastname
                         && x.PrimaryEmailAddress == email && x.ID != userId).ToList();
            if (!listExistUser.Any())
                return false;
            else
            {
                if (listExistUser.Any(e => e.Role == role))
                {
                    result.ResultType = OperationResultType.Success;
                    result.Message = "A " + role.ToDescription().ToLower() + " with the same first name, last name, " +
                                     "and primary email already exists in CLI Engage.";
                    result.AppendData = "waiting";
                }
                else
                {
                    result.ResultType = OperationResultType.Success;
                    result.Message = "A " + listExistUser.FirstOrDefault().Role.ToDescription().ToLower() +
                                     " with the same first name, last name, " +
                                     "and primary email already exists in CLI Engage.  Do you want to continue?";
                    result.AppendData = new
                    {
                        type = "continue"
                    };
                }
                return true;
            }
        }

        public List<UserComSchRelationEntity> GetUserRelationsByUserId(int userId)
        {
            var list = userService.UserComSchRelations.Where(o => o.UserId == userId).ToList();
            return list;
        }

        #endregion
    }
}
