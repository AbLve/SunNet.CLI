using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
/**************************************************************************
 * Developer: 		Sam
 * Computer:		SAM-PC
 * Domain:			sam-pc
 * CreatedOn:		2014/8/6 9:14:18
 * Description:		Please input class summary
 * Version History:	Created,2014/8/6 9:14:18
 * 
 * 
 **************************************************************************/
using DocumentFormat.OpenXml.Office2010.ExcelAc;
using DocumentFormat.OpenXml.Spreadsheet;
using Sunnet.Cli.Business.Communities.Models;
using Sunnet.Cli.Business.Cot.Cumulative;
using Sunnet.Cli.Core.Ade;
using Sunnet.Cli.Core.Schools.Entities;
using Sunnet.Cli.Core.Users;
using Sunnet.Cli.Core;
using Sunnet.Cli.Core.Users.Entities;
using Sunnet.Framework.Core.Tool;
using Sunnet.Framework.Resources;
using Sunnet.Framework.Helpers;
using Sunnet.Cli.Business.Users.Models;
using Sunnet.Cli.Core.Common.Enums;
using Sunnet.Cli.Business.Common;
using Sunnet.Framework.Core.Extensions;
using Sunnet.Cli.Core.Classes.Entites;
using Sunnet.Cli.Business.Classes;
using Sunnet.Cli.Core.Users.Enums;
using Sunnet.Cli.Business.Schools;
using Sunnet.Cli.Business.Schools.Models;
using Sunnet.Framework;
using LinqKit;
using Sunnet.Cli.Business.Students;
using Sunnet.Cli.Business.Permission;
using Sunnet.Cli.Core.Permission.Entities;
using Sunnet.Cli.Core.Communities.Entities;
using System.Reflection;
using Sunnet.Cli.Business.Vcw.Models;
using Sunnet.Cli.Business.Communities;
using Sunnet.Cli.Core.Vcw.Entities;
using Sunnet.Framework.Extensions;
using Sunnet.Cli.Business.Users.Models.VCW;
using Sunnet.Cli.Business.Cec.Model;

namespace Sunnet.Cli.Business.Users
{
    public partial class UserBusiness
    {
        private readonly IUserContract userService;
        private readonly SchoolBusiness schoolBusiness;
        private readonly ClassBusiness classBusiness;
        private readonly StudentBusiness studentBusiness;
        private readonly PermissionBusiness permissionBusiness;
        private readonly CommunityBusiness communityBusiness;


        public UserBusiness(EFUnitOfWorkContext unit = null)
        {
            userService = DomainFacade.CreateUserService(unit);
            schoolBusiness = new SchoolBusiness(unit);
            classBusiness = new ClassBusiness(unit);
            studentBusiness = new StudentBusiness(unit);
            permissionBusiness = new PermissionBusiness(unit);
            communityBusiness = new CommunityBusiness(unit);
        }

        #region Get Methods
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id">User Id</param>
        /// <returns></returns>
        public UserBaseEntity GetUser(int id)
        {
            return userService.GetUser(id);
        }

        public List<UserBaseModel> GetUsers(UserBaseEntity user, Expression<Func<UserBaseEntity, bool>> condition,
            string sort, string order, int first, int count, out int total)
        {

            var query = userService.BaseUsers.AsExpandable().Where(condition);
            total = query.Count();
            var list = query.Select(r => new UserBaseModel()
            {
                UserId = r.ID,
                FirstName = r.FirstName,
                LastName = r.LastName,
                UserRole = r.Role,
                CommunityNames = r.UserCommunitySchools.Where(o => o.Community != null).Select(o => o.Community.Name)
            }).OrderBy(sort, order).AsQueryable().Skip(first).Take(count);
            return list.ToList();
        }

        public UserBaseModel GetUserBaseModel(int id)
        {
            return userService.BaseUsers.Where(x => x.ID == id).Select(x => new UserBaseModel()
            {
                FirstName = x.FirstName,
                LastName = x.LastName,
                UserId = x.ID
            }).FirstOrDefault();
        }

        public UserBaseEntity UserLogin(string googleId)
        {
            UserBaseEntity user = userService.BaseUsers.Where(e => e.GoogleId == googleId
                && e.Status == EntityStatus.Active
                && e.IsDeleted == false).FirstOrDefault();
            return user;
        }

        /// <summary>
        /// 修改 状态为 Inactive
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public OperationResult ChangeStatusInactive(UserBaseEntity user)
        {
            user.Status = EntityStatus.Inactive;
            user.StatusDate = DateTime.Now;
            //当用户修改状态时，这条数据需要同步到LMS系统
            user.IsSyncLms = true;
            return userService.UpdateUser(user);
        }

        /// <summary>
        /// QA 专用
        /// </summary>
        /// <param name="googleId"></param>
        /// <returns></returns>
        public UserBaseEntity UserLoginQA(string googleId)
        {
            UserBaseEntity user = userService.BaseUsers.Where(e => e.GoogleId == googleId).FirstOrDefault();
            return user;
        }

        public CoordCoachEntity GetCoordCoach(int id)
        {
            return userService.GetCoordCoach(id);
        }

        public UserBaseModel GetCoordCoachByUserId(int userId)
        {
            return userService.CoordCoachs.Where(r => r.User.ID == userId)
                .Select(r => new UserBaseModel() { UserId = r.User.ID, FirstName = r.User.FirstName, LastName = r.User.LastName })
            .FirstOrDefault();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="teacherId">Teacher.Id</param>
        /// <param name="userInfo"></param>
        /// <returns></returns>
        public TeacherEntity GetTeacher(int teacherId, UserBaseEntity userInfo)
        {
            return userService.Teachers.AsExpandable()
                .Where(a => a.ID == teacherId)
                .Where(GetTeacherCondition(userInfo))
                .FirstOrDefault();
        }

        public TeacherModel GetTeacherModel(int teacherId)
        {
            var model = userService.Teachers.AsExpandable()
               .Where(a => a.ID == teacherId).Select(x => new TeacherModel()
               {
                   CommunityNames = x.UserInfo.UserCommunitySchools.Select(e => e.Community.Name),
                   SchoolNames = x.UserInfo.UserCommunitySchools.Where(s => s.SchoolId > 0).Select(s => s.School.Name),
                   Communities = x.UserInfo.UserCommunitySchools.Select(e => new CommunityModel()
                   {
                       ID = e.Community.ID,
                       CommunityName = e.Community.Name
                   }),
                   Schools = x.UserInfo.UserCommunitySchools.Where(s => s.SchoolId > 0).Select(s => new BasicSchoolModel()
                   {
                       ID = s.School.ID,
                       Name = s.School.Name,
                       Communities = s.School.CommunitySchoolRelations.Select(c => c.Community.Name)
                   }),
                   Id = x.ID,
                   FirstName = x.UserInfo.FirstName,
                   LastName = x.UserInfo.LastName,
                   CoachId = x.CoachId,
                   CoachingModel = x.CoachAssignmentWay,
                   YearsInProjectId = x.YearsInProjectId,
                   ECircle = x.ECIRCLEAssignmentWay,
               }).FirstOrDefault();
            if (model != null && model.CoachId > 0)
            {
                model.Coach = GetUserBaseModel(model.CoachId);
            }
            return model;
        }

        /// <summary>
        /// Cot报表中不同用户登录，看到不同的报表个数，因为不同用户可以看到的School和community不一样
        /// </summary>
        /// <param name="teacherId"></param>
        /// <param name="user">登录者</param>
        /// <returns></returns>
        public TeacherModel GetTeacherModel(int teacherId, UserBaseEntity user)
        {
            var isSchoolLevel = user.Role == Role.TRS_Specialist
                                    || user.Role == Role.TRS_Specialist_Delegate
                                    || user.Role == Role.School_Specialist
                                    || user.Role == Role.School_Specialist_Delegate
                                    || user.Role == Role.Principal
                                    || user.Role == Role.Principal_Delegate;

            var isCommunityLevel = user.Role == Role.District_Community_Specialist
                                   || user.Role == Role.Community_Specialist_Delegate
                                   || user.Role == Role.Community
                                   || user.Role == Role.District_Community_Delegate;
            List<int> userCommunityIds = new List<int>();
            List<int> userSchoolIds = new List<int>();
            if (isSchoolLevel)
            {
                userSchoolIds = schoolBusiness.GetSchoolIds(user).ToList();
            }
            if (isCommunityLevel)
            {
                userCommunityIds = communityBusiness.GetCommunityId(c => true, user);
                userSchoolIds = schoolBusiness.GetSchoolIds(user).ToList();
            }

            var model = userService.Teachers.AsExpandable()
               .Where(a => a.ID == teacherId).Select(x => new TeacherModel()
               {
                   CommunityNames = x.UserInfo.UserCommunitySchools.Select(e => e.Community.Name),
                   SchoolNames = x.UserInfo.UserCommunitySchools.Where(s => s.SchoolId > 0).Select(s => s.School.Name),
                   Communities = x.UserInfo.UserCommunitySchools
                   .Where(e => (isCommunityLevel || isSchoolLevel) ? userCommunityIds.Contains(e.CommunityId) : true)
                   .Select(e => new CommunityModel()
                   {
                       ID = e.Community.ID,
                       CommunityName = e.Community.Name
                   }).Distinct(),
                   Schools = x.UserInfo.UserCommunitySchools
                   .Where(s => s.SchoolId > 0 && (isCommunityLevel || isSchoolLevel ? userSchoolIds.Contains(s.SchoolId) : true))
                   .Select(s => new BasicSchoolModel()
                   {
                       ID = s.School.ID,
                       Name = s.School.Name,
                       Communities = s.School.CommunitySchoolRelations.Select(c => c.Community.Name)
                   }),
                   Id = x.ID,
                   FirstName = x.UserInfo.FirstName,
                   LastName = x.UserInfo.LastName,
                   CoachId = x.CoachId,
                   CoachingModel = x.CoachAssignmentWay,
                   YearsInProjectId = x.YearsInProjectId,
                   ECircle = x.ECIRCLEAssignmentWay,
               }).FirstOrDefault();
            if (model != null && model.CoachId > 0)
            {
                model.Coach = GetUserBaseModel(model.CoachId);
            }
            return model;
        }

        public List<TeacherModel> GetTeacherModels(Expression<Func<TeacherEntity, bool>> condition)
        {
            var models = userService.Teachers.Where(x => x.UserInfo.Status == EntityStatus.Active).AsExpandable()
               .Where(condition).Select(x => new TeacherModel()
               {
                   CommunityNames = x.UserInfo.UserCommunitySchools.Select(e => e.Community.Name),
                   SchoolNames = x.UserInfo.UserCommunitySchools.Where(s => s.SchoolId > 0).Select(s => s.School.Name),
                   Communities = x.UserInfo.UserCommunitySchools.Select(e => new CommunityModel()
                   {
                       ID = e.Community.ID,
                       CommunityName = e.Community.Name
                   }),
                   Schools = x.UserInfo.UserCommunitySchools.Where(s => s.SchoolId > 0).Select(s => new BasicSchoolModel()
                   {
                       ID = s.School.ID,
                       Name = s.School.Name,
                       Communities = s.School.CommunitySchoolRelations.Select(c => c.Community.Name)
                   }),
                   Id = x.ID,
                   FirstName = x.UserInfo.FirstName,
                   LastName = x.UserInfo.LastName,
                   CoachingModel = x.CoachAssignmentWay,
                   YearsInProjectId = x.YearsInProjectId,
                   ECircle = x.ECIRCLEAssignmentWay,
                   CoachId = x.CoachId
               }).ToList();
            if (models != null && models.Count > 0)
            {
                List<int> userIds = models.Select(r => r.CoachId).Distinct().ToList();
                if (userIds != null && userIds.Count > 0)
                {
                    List<UsernameModel> userModels = GetUsernames(userIds);
                    foreach (TeacherModel item in models)
                    {
                        UsernameModel userModel = userModels.Find(r => r.ID == item.CoachId);
                        item.Coach = userModel == null ? new UserBaseModel()
                        {
                            FirstName = "",
                            LastName = "",
                            UserId = 0
                        } : new UserBaseModel()
                        {
                            FirstName = userModel.Firstname,
                            LastName = userModel.Lastname,
                            UserId = userModel.ID
                        };
                    }
                }
            }
            return models;
        }

        public PrincipalEntity GetPrincipal(int principalId, UserBaseEntity userInfo, Role roleType)
        {
            if (roleType == Role.Principal || roleType == Role.Principal_Delegate)
            //查找principal
            {
                return userService.PrincipalDirectors.AsExpandable()
                    .Where(a => a.ID == principalId)
                    .Where(GetPrincipalCondition(userInfo, roleType))
                    .FirstOrDefault();
            }
            else if (roleType == Role.TRS_Specialist || roleType == Role.TRS_Specialist_Delegate)
            //查找trs specialist
            {
                return userService.PrincipalDirectors.AsExpandable()
                    .Where(a => a.ID == principalId)
                    .Where(GetTRSSpecialistCondition(userInfo, roleType))
                    .FirstOrDefault();
            }
            else if (roleType == Role.School_Specialist || roleType == Role.School_Specialist_Delegate)
            //查找school specialist
            {
                return userService.PrincipalDirectors.AsExpandable()
                    .Where(a => a.ID == principalId)
                    .Where(GetSchoolSpecialistCondition(userInfo, roleType))
                    .FirstOrDefault();
            }
            return new PrincipalEntity();
        }

        public UserBaseModel GetPrincipalModel(int principalId)
        {
            return userService.PrincipalDirectors.AsExpandable()
                .Where(a => a.ID == principalId).Select(x => new UserBaseModel()
                {
                    FirstName = x.UserInfo.FirstName,
                    LastName = x.UserInfo.LastName,
                    UserId = x.UserInfo.ID
                }).FirstOrDefault();
        }

        public CommunityUserEntity GetCommunity(int communityId, UserBaseEntity userInfo, Role roleType, bool isCommunity)
        {
            if (isCommunity)//查找communityuser
            {
                return userService.DistrictCommunitys.AsExpandable()
                    .Where(a => a.ID == communityId)
                    .Where(GetCommunityUserRoleCondition(userInfo, roleType))
                    .FirstOrDefault();
            }
            else //查找community specialist
            {
                return userService.DistrictCommunitys.AsExpandable()
                    .Where(a => a.ID == communityId)
                    .Where(GetCommunitySpecialistRoleCondition(userInfo, roleType))
                    .FirstOrDefault();
            }
        }

        public AuditorEntity GetAuditor(int auditorId)
        {
            return userService.GetAuditor(auditorId);
        }

        public StateWideEntity GetStateWide(int stateWideId)
        {
            return userService.GetStateWide(stateWideId);
        }
        #endregion

        #region Search Methods
        public List<UserModel> SearchInternalUsers(UserBaseEntity user, Expression<Func<UserBaseEntity, bool>> condition,
            string sort, string order, int first, int count, out int total)
        {
            var query = userService.BaseUsers.AsExpandable().Where(condition).Where(GetInternalUserCondition(user));

            total = query.Count();
            var list = query.Select(e => new UserModel()
            {
                ID = e.ID,
                UserId = e.ID,
                GoogleId = e.GoogleId,
                FirstName = e.FirstName,
                LastName = e.LastName,
                Status = e.Status,
                Email = e.PrimaryEmailAddress,
                Type = (InternalRole)e.Role
            }).OrderBy(sort, order).AsQueryable().Skip(first).Take(count);
            return list.ToList();
        }

        public List<DelegateModel> SearchDelegates(UserBaseEntity user, Expression<Func<UserBaseEntity, bool>> condition,
            string sort, string order, int first, int count, out int total)
        {
            var query =
                userService.BaseUsers.AsExpandable()
                    .Where(condition)
                    .Where(
                        e =>
                            e.Role == Role.District_Community_Delegate || e.Role == Role.Community_Specialist_Delegate ||
                            e.Role == Role.Principal_Delegate || e.Role == Role.School_Specialist_Delegate ||
                            e.Role == Role.TRS_Specialist_Delegate).Where(GetDelegateCondition(user));

            total = query.Count();
            var list = query.Select(e => new DelegateModel()
            {
                ID = e.ID,
                ObjectId = e.CommunityUser != null ? e.CommunityUser.ID : e.Principal.ID,
                FirstName = e.FirstName,
                LastName = e.LastName,
                Email = e.PrimaryEmailAddress,
                RoleType = e.Role,
                GoogleAccount = e.GoogleId,
                Status = e.Status,
                ParentId = e.CommunityUser != null ? e.CommunityUser.ParentId : 0
            }).OrderBy(sort, order).AsQueryable().Skip(first).Take(count).ToList();
            foreach (var entity in list)
            {
                if (entity.ParentId > 0)
                {
                    var parentUser = GetUser(entity.ParentId);
                    entity.CommunityNames = parentUser.UserCommunitySchools.Select(e => e.Community.Name);
                }
            }
            return list;
        }



        public List<UserModel> SearchTeachers(UserBaseEntity user, Expression<Func<TeacherEntity, bool>> condition,
            string sort, string order, int first, int count, out int total)
        {
            var query = userService.Teachers.AsExpandable().Where(condition).Where(GetTeacherCondition(user));

            total = query.Count();
            var list = query.Select(e => new UserModel()
            {
                ID = e.ID,
                UserId = e.UserInfo.ID,
                CommunityNames = e.UserInfo.UserCommunitySchools.Select(x => x.Community.Name),
                FirstName = e.UserInfo.FirstName,
                LastName = e.UserInfo.LastName,
                TeacherNumber = e.UserInfo.InternalID,
                SchoolNames = e.UserInfo.UserCommunitySchools.Select(o => o.School.Name),
                TeacherType = e.TeacherType,
                Status = e.UserInfo.Status,
                GoogleId = e.UserInfo.GoogleId,
                Gmail = e.UserInfo.Gmail,
                CommunityIds = e.UserInfo.UserCommunitySchools.Select(o => o.CommunityId),
                SchoolIds = e.UserInfo.UserCommunitySchools.Select(o => o.SchoolId)
            }).OrderBy(sort, order).AsQueryable().Skip(first).Take(count).ToList();
            foreach (UserModel model in list)
            {
                if (user.Role == Role.Community || user.Role == Role.District_Community_Specialist ||
                    user.Role == Role.Statewide)
                {
                    List<int> primarySchools =
                        schoolBusiness.GetPrimarySchoolsByComId(
                            user.UserCommunitySchools.Select(e => e.CommunityId).ToList()).Select(e => e.ID).ToList();
                    if (primarySchools.Any(e => model.SchoolIds.Contains(e)) ||
                        (!model.SchoolIds.Any() && user.UserCommunitySchools.Any(f => model.CommunityIds.Contains(f.CommunityId))))
                    {
                        model.AccessType = AccessType.FullAccess;
                    }
                    else
                    {
                        model.AccessType = AccessType.ReadOnly;
                    }
                }
                else if (user.Role == Role.District_Community_Delegate ||
                         user.Role == Role.Community_Specialist_Delegate)
                {
                    var parentCommunityUser = GetUser(user.CommunityUser.ParentId);
                    List<int> primarySchools =
                        schoolBusiness.GetPrimarySchoolsByComId(
                            parentCommunityUser.UserCommunitySchools.Select(e => e.CommunityId).ToList()).Select(e => e.ID).ToList();
                    if (primarySchools.Any(e => model.SchoolIds.Contains(e)) ||
                        (!model.SchoolIds.Any() && parentCommunityUser.UserCommunitySchools.Any(f => model.CommunityIds.Contains(f.CommunityId))))
                    {
                        model.AccessType = AccessType.FullAccess;
                    }
                    else
                    {
                        model.AccessType = AccessType.ReadOnly;
                    }
                }
                else
                {
                    model.AccessType = AccessType.FullAccess;
                }
            }
            return list;
        }

        public List<TeacherTransactionModel> SearchTeacherTransactions(Expression<Func<TeacherTransactionEntity, bool>> condition,
            string sort = "CreatedOn", string order = "Desc")
        {
            var query = userService.TeacherTransactions.AsExpandable().Where(condition);

            var list = query.Select(e => new TeacherTransactionModel()
            {
                TransactionType = e.TransactionType,
                Amount = e.Amount,
                TISessionsAttended = e.TISessionsAttended,
                TITotalSessions = e.TITotalSessions,
                TICLIFundingId = e.TICLIFundingId,
                FundingYear = e.FundingYear,
                CreatedOn = e.CreatedOn
            }).OrderBy(sort, order);
            return list.ToList();
        }

        public List<PrincipalUserModel> SearchPrincipals(UserBaseEntity userInfo, Role roleType,
            Expression<Func<PrincipalEntity, bool>> condition,
            string sort, string order, int first, int count, out int total, bool isprincipal)
        {
            IQueryable<PrincipalEntity> query;

            if (roleType == Role.Principal || roleType == Role.Principal_Delegate)
            //查找principal
            {
                query =
                    userService.PrincipalDirectors.AsExpandable()
                        .Where(condition)
                        .Where(GetPrincipalCondition(userInfo, roleType));
            }
            else if (roleType == Role.TRS_Specialist || roleType == Role.TRS_Specialist_Delegate)
            //查找trs specialist
            {
                query =
                    userService.PrincipalDirectors.AsExpandable()
                        .Where(condition)
                        .Where(GetTRSSpecialistCondition(userInfo, roleType));
            }
            else
            {
                query =
                    userService.PrincipalDirectors.AsExpandable()
                        .Where(condition)
                        .Where(GetSchoolSpecialistCondition(userInfo, roleType));
            }

            total = query.Count();
            var list = query.Select(e => new PrincipalUserModel()
            {
                ID = e.ID,
                UserId = e.UserInfo.ID,
                FirstName = e.UserInfo.FirstName,
                MiddleName = e.UserInfo.MiddleName,
                PositionId = e.PositionId,
                PrimaryLanguageId = e.PrimaryLanguageId,
                LastName = e.UserInfo.LastName,
                SchoolNames = e.UserInfo.UserCommunitySchools.Select(x => x.School.Name),
                Status = e.UserInfo.Status,
                Gender = e.Gender,
                GoogleId = e.UserInfo.GoogleId,
                Gmail = e.UserInfo.Gmail,
                PreviousLastName = e.UserInfo.PreviousLastName,
                BirthDate = e.BirthDate,
                Address = e.Address,
                PrimaryPhoneNumber = e.UserInfo.PrimaryPhoneNumber,
                PrimaryNumberType = e.UserInfo.PrimaryNumberType,
                SecondPhoneNumber = e.UserInfo.SecondaryPhoneNumber,
                SecondNumberType = e.UserInfo.SecondaryNumberType,
                PrimaryEmail = e.UserInfo.PrimaryEmailAddress,
                SecondEmail = e.UserInfo.SecondaryEmailAddress,
                IsSameAddress = e.IsSameAddress == 1,
                FaxNumber = e.UserInfo.FaxNumber,
                SchoolIds = e.UserInfo.UserCommunitySchools.Select(o => o.SchoolId),
                SchoolId = e.UserInfo.UserCommunitySchools.FirstOrDefault(o => o.SchoolId > 0) == null ? 0 : e.UserInfo.UserCommunitySchools.FirstOrDefault().SchoolId,
                CommunityId = e.UserInfo.UserCommunitySchools.FirstOrDefault(o => o.CommunityId > 0) == null ? 0 : e.UserInfo.UserCommunitySchools.FirstOrDefault().CommunityId,

            }).OrderBy(sort, order).AsQueryable().Skip(first).Take(count).ToList();

            foreach (PrincipalUserModel model in list)
            {
                if (userInfo.Role == Role.Community || userInfo.Role == Role.District_Community_Specialist ||
                    userInfo.Role == Role.Statewide)
                {
                    List<int> primarySchools = new List<int>();
                    primarySchools =
                        schoolBusiness.GetPrimarySchoolsByComId(
                            userInfo.UserCommunitySchools.Select(e => e.CommunityId).ToList()).Select(e => e.ID).ToList();
                    if (primarySchools.Any(e => model.SchoolIds.Contains(e)))
                    {
                        model.AccessType = AccessType.FullAccess;
                    }
                    else
                    {
                        model.AccessType = AccessType.ReadOnly;
                    }
                }
                else if (userInfo.Role == Role.District_Community_Delegate ||
                         userInfo.Role == Role.Community_Specialist_Delegate)
                {
                    List<int> primarySchools = new List<int>();
                    var parentCommunityUser = GetUser(userInfo.CommunityUser.ParentId);
                    primarySchools =
                        schoolBusiness.GetPrimarySchoolsByComId(
                            parentCommunityUser.UserCommunitySchools.Select(e => e.CommunityId).ToList()).Select(e => e.ID).ToList();
                    if (primarySchools.Any(e => model.SchoolIds.Contains(e)))
                    {
                        model.AccessType = AccessType.FullAccess;
                    }
                    else
                    {
                        model.AccessType = AccessType.ReadOnly;
                    }
                }
                else
                {
                    model.AccessType = AccessType.FullAccess;
                }
            }
            return list;
        }

        public List<UserModel> SearchCommunityUsers(UserBaseEntity userInfo, Role roleType,
            Expression<Func<CommunityUserEntity, bool>> condition,
            string sort, string order, int first, int count, out int total, bool iscommunity)
        {
            IQueryable<CommunityUserEntity> query;
            if (iscommunity) //查找community user
            {
                query = userService.DistrictCommunitys.AsExpandable().Where(condition)
                    .Where(GetCommunityUserRoleCondition(userInfo, roleType));
            }
            else //查找community specialist
            {
                query = userService.DistrictCommunitys.AsExpandable().Where(condition)
                    .Where(GetCommunitySpecialistRoleCondition(userInfo, roleType));
            }

            total = query.Count();
            var list = query.Select(e => new UserModel()
            {
                ID = e.ID,
                UserId = e.UserInfo.ID,
                FirstName = e.UserInfo.FirstName,
                LastName = e.UserInfo.LastName,
                Status = e.UserInfo.Status,
                CommunityNames = e.UserInfo.UserCommunitySchools.Select(x => x.Community.BasicCommunity.Name),
                GoogleId = e.UserInfo.GoogleId,
                Gmail = e.UserInfo.Gmail
            }).OrderBy(sort, order).AsQueryable().Skip(first).Take(count);
            return list.ToList();
        }

        public List<UserBaseModel> SearchCommunitySpecialists(UserBaseEntity user,
            Expression<Func<CommunityUserEntity, bool>> condition,
            string sort, string order, int first, int count, out int total)
        {

            var query = userService.DistrictCommunitys.AsExpandable()
                .Where(condition).Where(o => o.UserInfo.Role == Role.District_Community_Specialist)
                .Select(o => new UserBaseModel()
                {
                    UserId = o.UserInfo.ID,
                    FirstName = o.UserInfo.FirstName,
                    LastName = o.UserInfo.LastName
                });
            total = query.Count();
            var list = query.OrderBy(sort, order).Skip(first).Take(count);
            return list.ToList();
        }

        public List<CommunitySpecialistUserModel> SearchCommunitySpecialistsForBES(UserBaseEntity user,
          Expression<Func<CommunityUserEntity, bool>> condition,
          string sort, string order, int first, int count, out int total)
        {

            var query = userService.DistrictCommunitys.AsExpandable()
                .Where(condition)
                .Select(e => new CommunitySpecialistUserModel()
                {
                    ID = e.ID,
                    UserId = e.UserInfo.ID,
                    PositionId = e.PositionId,
                    FirstName = e.UserInfo.FirstName,
                    MiddleName = e.UserInfo.MiddleName,
                    LastName = e.UserInfo.LastName,
                    PreviousLastName = e.UserInfo.PreviousLastName,
                    BirthDate = e.BirthDate,
                    Gender = e.Gender,
                    PrimaryLanguageId = e.PrimaryLanguageId,
                    CommunityNames = e.UserInfo.UserCommunitySchools.Select(x => x.Community.Name),
                    Status = e.UserInfo.Status,
                    IsSameAddress = e.IsSameAddress == 1,
                    GoogleId = e.UserInfo.GoogleId,
                    Gmail = e.UserInfo.Gmail,
                    Address = e.Address,
                    PrimaryPhoneNumber = e.UserInfo.PrimaryPhoneNumber,
                    PrimaryNumberType = e.UserInfo.PrimaryNumberType,
                    SecondPhoneNumber = e.UserInfo.SecondaryPhoneNumber,
                    SecondNumberType = e.UserInfo.SecondaryNumberType,
                    PrimaryEmail = e.UserInfo.PrimaryEmailAddress,
                    SecondEmail = e.UserInfo.SecondaryEmailAddress,
                    FaxNumber = e.UserInfo.FaxNumber,
                    FirstCommunityName = e.UserInfo.UserCommunitySchools.FirstOrDefault(o => o.CommunityId > 0) == null ? "" : e.UserInfo.UserCommunitySchools.FirstOrDefault(o => o.CommunityId > 0).Community.Name,
                    CommunityId = e.UserInfo.UserCommunitySchools.FirstOrDefault(o => o.CommunityId > 0) == null ? 0 : e.UserInfo.UserCommunitySchools.FirstOrDefault().CommunityId,

                });
            total = query.Count();
            var list = query.OrderBy(sort, order).Skip(first).Take(count);
            return list.ToList();
        }

        private Expression<Func<CommunityUserEntity, bool>> AssignCommunitySpecialistCondition(UserBaseEntity userInfo)
        {
            Expression<Func<CommunityUserEntity, bool>> condition = o => true;
            if (userInfo == null)
                return condition;
            switch (userInfo.Role)
            {
                case Role.Teacher:
                case Role.Parent:
                case Role.Principal:
                case Role.Principal_Delegate:
                case Role.TRS_Specialist:
                case Role.TRS_Specialist_Delegate:
                case Role.School_Specialist:
                case Role.School_Specialist_Delegate:
                    condition = o => false;
                    break;
                default:
                    condition = o => false;
                    break;
            }
            return condition;
        }

        public List<UserModel> SearchStatewides(Expression<Func<StateWideEntity, bool>> condition,
            string sort, string order, int first, int count, out int total)
        {
            var query = userService.StateWides.AsExpandable().Where(condition);

            total = query.Count();
            var list = query.Select(e => new UserModel()
            {
                ID = e.ID,
                UserId = e.UserInfo.ID,
                FirstName = e.UserInfo.FirstName,
                LastName = e.UserInfo.LastName,
                Status = e.UserInfo.Status,
                GoogleId = e.UserInfo.GoogleId,
                Gmail = e.UserInfo.Gmail
            }).OrderBy(sort, order).AsQueryable().Skip(first).Take(count);
            return list.ToList();
        }

        public List<UserModel> SearchAuditors(Expression<Func<AuditorEntity, bool>> condition,
            string sort, string order, int first, int count, out int total)
        {
            var query = userService.Auditors.AsExpandable().Where(condition);

            total = query.Count();
            var list = query.Select(e => new UserModel()
            {
                ID = e.ID,
                UserId = e.UserInfo.ID,
                FirstName = e.UserInfo.FirstName,
                LastName = e.UserInfo.LastName,
                Status = e.UserInfo.Status,
                GoogleId = e.UserInfo.GoogleId,
                Gmail = e.UserInfo.Gmail
            }).OrderBy(sort, order).AsQueryable().Skip(first).Take(count);
            return list.ToList();
        }


        public List<SelectItemModel> GetTeachers(Expression<Func<TeacherEntity, bool>> condition, string sort = "UserInfo.FirstName", string order = "ASC")
        {
            return userService.Teachers.Where(condition).OrderBy(sort, order).Select(x => new SelectItemModel()
            {
                ID = x.ID,
                Name = x.UserInfo.FirstName + " " + x.UserInfo.LastName
            }).ToList();
        }
        #endregion

        #region New Methods
        public OperationResult InsertParent(ParentEntity parent)
        {
            OperationResult result = new OperationResult(OperationResultType.Success);
            result = userService.InsertParent(parent);
            return result;
        }

        public OperationResult InsertParentStudentRelation(ParentStudentRelationEntity parentStudent)
        {
            OperationResult result = new OperationResult(OperationResultType.Success);
            result = userService.InsertParentStudentRelation(parentStudent);
            return result;
        }

        public OperationResult InsertUser(UserBaseEntity user)
        {
            OperationResult result = new OperationResult(OperationResultType.Success);
            result = userService.InsertUser(user);
            return result;
        }

        public OperationResult InsertCoordCoach(CoordCoachEntity coordCoach, List<int> certificates)
        {
            OperationResult result = new OperationResult(OperationResultType.Success);
            CertificateEntity certificate = new CertificateEntity();
            if (certificates != null)
            {
                coordCoach.User.Certificates = new List<CertificateEntity>();
                foreach (var item in certificates)
                {
                    certificate = userService.GetCertificate(item);
                    coordCoach.User.Certificates.Add(certificate);
                }
            }
            result = userService.InsertCoordCoach(coordCoach);
            return result;
        }

        public OperationResult InsertTeacher(TeacherEntity teacher, int[] ageGroups, int[] chkPDs
            , List<int> certificates, int[] chkPackages, int communityId, int schoolId, int currentUserId)
        {
            ProfessionalDevelopmentEntity professionalDevelopment = new ProfessionalDevelopmentEntity();
            List<TeacherAgeGroupEntity> listTeacherAgeGroup = new List<TeacherAgeGroupEntity>();
            CertificateEntity certificate = new CertificateEntity();

            teacher.UserInfo.PermissionRoles = new List<PermissionRoleEntity>();
            if (chkPackages != null)
            {
                foreach (var item in chkPackages)
                {
                    PermissionRoleEntity packageEntity = new PermissionRoleEntity();
                    packageEntity = permissionBusiness.GetRole(Convert.ToInt32(item));
                    teacher.UserInfo.PermissionRoles.Add(packageEntity);
                }
            }
            if (ageGroups != null)
            {
                teacher.TeacherAgeGroups = new List<TeacherAgeGroupEntity>();
                foreach (var item in ageGroups)
                {
                    TeacherAgeGroupEntity teacherAgeGroup = new TeacherAgeGroupEntity();
                    teacherAgeGroup.AgeGroup = item;
                    teacherAgeGroup.AgeGroupOther = "";
                    if ((AgeGroup)item == AgeGroup.Other)
                        teacherAgeGroup.AgeGroupOther = teacher.AgeGroupOther;
                    teacher.TeacherAgeGroups.Add(teacherAgeGroup);
                }
            }
            if (chkPDs != null)
            {
                teacher.UserInfo.ProfessionalDevelopments = new List<ProfessionalDevelopmentEntity>();
                foreach (var item in chkPDs)
                {
                    professionalDevelopment = userService.GetProfessionalDevelopment(item);
                    teacher.UserInfo.ProfessionalDevelopments.Add(professionalDevelopment);
                }
            }
            if (certificates != null)
            {
                teacher.UserInfo.Certificates = new List<CertificateEntity>();
                foreach (var item in certificates)
                {
                    certificate = userService.GetCertificate(item);
                    teacher.UserInfo.Certificates.Add(certificate);
                }
            }

            teacher.UserInfo.UserCommunitySchools = new List<UserComSchRelationEntity>();
            teacher.UserInfo.UserCommunitySchools.Add(new UserComSchRelationEntity()
            {
                CommunityId = communityId,
                SchoolId = schoolId,
                Status = EntityStatus.Active,
                CreatedBy = currentUserId,
                UpdatedBy = currentUserId
            });

            OperationResult result = new OperationResult(OperationResultType.Success);
            result = userService.InsertTeacher(teacher);
            permissionBusiness.ClearCache();
            return result;
        }

        public OperationResult InsertPrincipal(PrincipalEntity principal, int[] chkPDs, List<int> certificates,
            int[] chkPackages, int schoolId, int currentUserId)
        {
            principal.UserInfo.PermissionRoles = new List<PermissionRoleEntity>();
            UserBaseEntity userParentPackages = new UserBaseEntity();
            if (principal.UserInfo.Role == Role.Principal_Delegate
                || principal.UserInfo.Role == Role.TRS_Specialist_Delegate
                || principal.UserInfo.Role == Role.School_Specialist_Delegate)
            {
                userParentPackages = GetUser(principal.ParentId);
                principal.UserInfo.PermissionRoles = userParentPackages.PermissionRoles;
            }
            if (chkPackages != null)
            {
                foreach (var item in chkPackages)
                {
                    PermissionRoleEntity packageEntity = new PermissionRoleEntity();
                    packageEntity = permissionBusiness.GetRole(Convert.ToInt32(item));
                    principal.UserInfo.PermissionRoles.Add(packageEntity);
                }
            }
            ProfessionalDevelopmentEntity professionalDevelopment = new ProfessionalDevelopmentEntity();
            CertificateEntity certificate = new CertificateEntity();
            if (chkPDs != null)
            {
                principal.UserInfo.ProfessionalDevelopments = new List<ProfessionalDevelopmentEntity>();
                foreach (var item in chkPDs)
                {
                    professionalDevelopment = userService.GetProfessionalDevelopment(item);
                    principal.UserInfo.ProfessionalDevelopments.Add(professionalDevelopment);
                }
            }
            if (certificates != null)
            {
                principal.UserInfo.Certificates = new List<CertificateEntity>();
                foreach (var item in certificates)
                {
                    certificate = userService.GetCertificate(item);
                    principal.UserInfo.Certificates.Add(certificate);
                }
            }
            if (schoolId > 0)
            {
                principal.UserInfo.UserCommunitySchools = new List<UserComSchRelationEntity>();
                principal.UserInfo.UserCommunitySchools.Add(new UserComSchRelationEntity()
                {
                    SchoolId = schoolId,
                    Status = EntityStatus.Active,
                    CreatedBy = currentUserId,
                    UpdatedBy = currentUserId
                });
            }
            OperationResult result = new OperationResult(OperationResultType.Success);
            result = userService.InsertPrincipal(principal);
            permissionBusiness.ClearCache();
            return result;
        }

        public OperationResult InsertCommunityUser(CommunityUserEntity communityUser, int communityId,
            List<int> certificates, int[] chkPackages, int currentUserId)
        {
            UserBaseEntity userParentPackages = new UserBaseEntity();
            communityUser.UserInfo.PermissionRoles = new List<PermissionRoleEntity>();
            if (communityUser.UserInfo.Role == Role.District_Community_Delegate
                || communityUser.UserInfo.Role == Role.Community_Specialist_Delegate)
            {
                userParentPackages = GetUser(communityUser.ParentId);
                communityUser.UserInfo.PermissionRoles = userParentPackages.PermissionRoles;
            }
            if (chkPackages != null)
            {
                foreach (var item in chkPackages)
                {
                    PermissionRoleEntity packageEntity = new PermissionRoleEntity();
                    packageEntity = permissionBusiness.GetRole(Convert.ToInt32(item));
                    communityUser.UserInfo.PermissionRoles.Add(packageEntity);
                }
            }
            CertificateEntity certificate = new CertificateEntity();
            if (certificates != null)
            {
                communityUser.UserInfo.Certificates = new List<CertificateEntity>();
                foreach (var item in certificates)
                {
                    certificate = userService.GetCertificate(item);
                    communityUser.UserInfo.Certificates.Add(certificate);
                }
            }
            if (communityId > 0)
            {
                communityUser.UserInfo.UserCommunitySchools = new List<UserComSchRelationEntity>();
                communityUser.UserInfo.UserCommunitySchools.Add(new UserComSchRelationEntity()
                {
                    CommunityId = communityId,
                    SchoolId = 0,
                    Status = EntityStatus.Active,
                    CreatedBy = currentUserId,
                    UpdatedBy = currentUserId
                });
            }
            OperationResult result = new OperationResult(OperationResultType.Success);
            result = userService.InsertCommunity(communityUser);
            permissionBusiness.ClearCache();
            return result;
        }

        public OperationResult InsertStateWide(StateWideEntity stateWide, int[] chkPackages)
        {
            stateWide.UserInfo.PermissionRoles = new List<PermissionRoleEntity>();
            if (chkPackages != null)
            {
                foreach (var item in chkPackages)
                {
                    PermissionRoleEntity packageEntity = new PermissionRoleEntity();
                    packageEntity = permissionBusiness.GetRole(Convert.ToInt32(item));
                    stateWide.UserInfo.PermissionRoles.Add(packageEntity);
                }
            }
            OperationResult result = new OperationResult(OperationResultType.Success);
            result = userService.InsertStateWide(stateWide);
            permissionBusiness.ClearCache();
            return result;
        }

        public OperationResult InsertAuditor(AuditorEntity auditor, int[] chkPackages)
        {
            auditor.UserInfo.PermissionRoles = new List<PermissionRoleEntity>();
            if (chkPackages != null)
            {
                foreach (var item in chkPackages)
                {
                    PermissionRoleEntity packageEntity = new PermissionRoleEntity();
                    packageEntity = permissionBusiness.GetRole(Convert.ToInt32(item));
                    auditor.UserInfo.PermissionRoles.Add(packageEntity);
                }
            }
            OperationResult result = new OperationResult(OperationResultType.Success);
            result = userService.InsertAuditor(auditor);
            permissionBusiness.ClearCache();
            return result;
        }
        #endregion

        #region Update Methods
        public OperationResult UpdateUser(UserBaseEntity user, bool isSave = true, bool isSyncLms = true)
        {
            OperationResult result = new OperationResult(OperationResultType.Success);
            result = userService.UpdateUser(user, isSave, isSyncLms);
            return result;
        }

        public void InsertUserMail(int userId, string email)
        {
            userService.InsertUserMail(userId, email);
        }

        /// <summary>
        /// 此方法仅用于更新Super_admin,Content_personnel,Statisticians,Administrative_personnel,
        /// Intervention_manager,Intervention_support_personnel角色的用户
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public OperationResult UpdateInternalUser(UserBaseEntity user)
        {
            OperationResult result = new OperationResult(OperationResultType.Success);

            UserBaseEntity userEntity = GetUser(user.ID);
            //如果以前的角色是Coach或Coordinator,则需要删除子表CoachCoords中的记录
            if (userEntity.Role == Role.Mentor_coach || userEntity.Role == Role.Coordinator)
            {
                while (userEntity.Certificates.Count > 0)
                {
                    userEntity.Certificates.Remove(userEntity.Certificates.First());
                }
                userService.DeleteCoordCoach(userEntity.CoordCoach.ID, false);
            }
            //如果以前的角色是VideoCoding，则需要删除子表VideoCodings中的记录
            else if (userEntity.Role == Role.Video_coding_analyst)
            {
                userService.DeleteVideoCoding(userEntity.VideoCoding.ID, false);
            }
            userEntity = user;
            result = userService.UpdateUser(userEntity);
            return result;
        }

        public OperationResult UpdateVideoCoding(UserBaseEntity user)
        {
            OperationResult result = new OperationResult(OperationResultType.Success);
            UserBaseEntity userEntity = GetUser(user.ID);

            //为了处理角色转换为VideoCoding时,子表VideoCodings中无数据的情况
            if (userEntity.Role != Role.Video_coding_analyst)
            {
                user.VideoCoding.ID = 0;
                //如果以前的角色是Coach或Coordinator,则需要删除子表CoachCoords中的记录
                if (userEntity.Role == Role.Mentor_coach || userEntity.Role == Role.Coordinator)
                {
                    while (userEntity.Certificates.Count > 0)
                    {
                        userEntity.Certificates.Remove(userEntity.Certificates.First());
                    }
                    userService.DeleteCoordCoach(userEntity.CoordCoach.ID, false);
                }
                userEntity.VideoCoding = user.VideoCoding;
                userService.UpdateUser(userEntity, false);
                result = userService.UpdateUser(user, true);
            }
            else
            {
                userEntity = user;
                userService.UpdateUser(userEntity, false);
                result = userService.UpdateVideoCoding(user.VideoCoding, true);
            }
            return result;
        }

        public OperationResult UpdateCoordCoach(UserBaseEntity user, List<int> certificates)
        {
            UserBaseEntity userEntity = GetUser(user.ID);
            while (userEntity.Certificates.Count > 0)
            {
                userEntity.Certificates.Remove(userEntity.Certificates.First());
            }
            CertificateEntity certificate = new CertificateEntity();
            if (certificates != null)
            {
                foreach (var item in certificates)
                {
                    certificate = userService.GetCertificate(item);
                    userEntity.Certificates.Add(certificate);
                }
            }

            OperationResult result = new OperationResult(OperationResultType.Success);
            //为了处理角色转换为Coach或Coordination时,子表CoachCoords中无数据的情况
            if (userEntity.Role != Role.Mentor_coach && userEntity.Role != Role.Coordinator)
            {
                user.CoordCoach.ID = 0;
                //如果以前的角色是VideoCoding，则需要删除子表VideoCodings中的记录
                if (userEntity.Role == Role.Video_coding_analyst)
                {
                    userService.DeleteVideoCoding(userEntity.VideoCoding.ID, false);
                }
                userEntity.CoordCoach = user.CoordCoach;
                userEntity.CoordCoach.User = null;
                userService.UpdateUser(userEntity, false);
                result = userService.UpdateUser(user, true);
            }
            else
            {
                userEntity = user;
                userService.UpdateUser(userEntity, false);
                result = userService.UpdateCoordCoach(user.CoordCoach);
            }
            return result;
        }

        public OperationResult CoordCoachEquipment(int coordCoachId, string[] serialNumber, string[] uTHealthTag,
            int[] chkEquipment, int[] isAssessmentEquipment)
        {
            CoordCoachEntity coordCoach = GetCoordCoach(coordCoachId);
            userService.DeleteCoordCoachEquipment(coordCoach.CoordCoachEquipments.ToList());
            coordCoach.CoordCoachEquipments = new List<CoordCoachEquipmentEntity>();
            if (isAssessmentEquipment != null)
            {
                if (isAssessmentEquipment[0] == 1)
                {
                    coordCoach.IsAssessmentEquipment = true;
                    if (chkEquipment != null)
                    {
                        for (int i = 0; i < chkEquipment.Length; i++)
                        {
                            CoordCoachEquipmentEntity coordCoachEquipment = new CoordCoachEquipmentEntity();
                            coordCoachEquipment.CoordCoachId = coordCoachId;
                            coordCoachEquipment.EquipmentId = chkEquipment[i];
                            coordCoachEquipment.SerialNumber = serialNumber[i];
                            coordCoachEquipment.UTHealthTag = uTHealthTag[i];
                            coordCoach.CoordCoachEquipments.Add(coordCoachEquipment);
                        }
                    }
                }
            }

            OperationResult result = new OperationResult(OperationResultType.Success);
            result = userService.UpdateCoordCoach(coordCoach);
            return result;
        }

        public OperationResult UpdateTeacher(TeacherEntity teacher, int[] ageGroups, int[] chkPDs,
            List<int> certificates, int[] chkPackages, Role role)
        {
            TeacherRoleEntity teacherRoleEntity = GetTeacherRoleEntity(role);
            InitByRole(teacher, teacherRoleEntity);
            TeacherEntity teacherEntity = GetTeacher(teacher.ID, null);

            while (teacherEntity.UserInfo.PermissionRoles.Count > 0)
            {
                teacherEntity.UserInfo.PermissionRoles.Remove(teacherEntity.UserInfo.PermissionRoles.First());
            }
            teacherEntity.UserInfo.PermissionRoles = new List<PermissionRoleEntity>();
            if (chkPackages != null)
            {
                foreach (var item in chkPackages)
                {
                    PermissionRoleEntity packageEntity = new PermissionRoleEntity();
                    packageEntity = permissionBusiness.GetRole(Convert.ToInt32(item));
                    teacherEntity.UserInfo.PermissionRoles.Add(packageEntity);
                }
            }
            while (teacherEntity.UserInfo.Certificates.Count > 0)
            {
                teacherEntity.UserInfo.Certificates.Remove(teacherEntity.UserInfo.Certificates.First());
            }
            while (teacherEntity.UserInfo.ProfessionalDevelopments.Count > 0)
            {
                teacherEntity.UserInfo.ProfessionalDevelopments.Remove(teacherEntity.UserInfo.ProfessionalDevelopments.First());
            }

            userService.DeleteTeacherAgeGroup(teacherEntity.TeacherAgeGroups.ToList(), false);

            ProfessionalDevelopmentEntity professionalDevelopment = new ProfessionalDevelopmentEntity();
            CertificateEntity certificate = new CertificateEntity();
            if (ageGroups != null)
            {
                teacherEntity.TeacherAgeGroups = new List<TeacherAgeGroupEntity>();
                foreach (var item in ageGroups)
                {
                    TeacherAgeGroupEntity teacherAgeGroup = new TeacherAgeGroupEntity();
                    teacherAgeGroup.AgeGroup = item;
                    teacherAgeGroup.AgeGroupOther = "";
                    if ((AgeGroup)item == AgeGroup.Other)
                        teacherAgeGroup.AgeGroupOther = teacher.AgeGroupOther;
                    teacherEntity.TeacherAgeGroups.Add(teacherAgeGroup);
                }
            }
            if (chkPDs != null)
            {
                teacherEntity.UserInfo.ProfessionalDevelopments = new List<ProfessionalDevelopmentEntity>();
                foreach (var item in chkPDs)
                {
                    professionalDevelopment = userService.GetProfessionalDevelopment(item);
                    teacherEntity.UserInfo.ProfessionalDevelopments.Add(professionalDevelopment);
                }
            }
            if (certificates != null)
            {
                foreach (var item in certificates)
                {
                    certificate = userService.GetCertificate(item);
                    teacherEntity.UserInfo.Certificates.Add(certificate);
                }
            }

            OperationResult result = new OperationResult(OperationResultType.Success);
            userService.UpdateTeacher(teacherEntity, false);
            userService.UpdateUser(teacher.UserInfo, false);
            result = userService.UpdateTeacher(teacher);
            permissionBusiness.ClearCache();
            return result;
        }

        public OperationResult UpdatePrincipal(PrincipalEntity principal, int[] chkPDs, List<int> certificates, int[] chkPackages)
        {
            PrincipalEntity principalEntity = userService.GetPrincipal(principal.ID);
            if (principal.UserInfo.Role == Role.Principal || principal.UserInfo.Role == Role.TRS_Specialist)
            {
                while (principalEntity.UserInfo.PermissionRoles.Count > 0)
                {
                    principalEntity.UserInfo.PermissionRoles.Remove(principalEntity.UserInfo.PermissionRoles.First());
                }
                principalEntity.UserInfo.PermissionRoles = new List<PermissionRoleEntity>();
                if (chkPackages != null)
                {
                    foreach (var item in chkPackages)
                    {
                        PermissionRoleEntity packageEntity = new PermissionRoleEntity();
                        packageEntity = permissionBusiness.GetRole(Convert.ToInt32(item));
                        principalEntity.UserInfo.PermissionRoles.Add(packageEntity);
                    }
                }
            }

            while (principalEntity.UserInfo.Certificates.Count > 0)
            {
                principalEntity.UserInfo.Certificates.Remove(principalEntity.UserInfo.Certificates.First());
            }
            while (principalEntity.UserInfo.ProfessionalDevelopments.Count > 0)
            {
                principalEntity.UserInfo.ProfessionalDevelopments.Remove(principalEntity.UserInfo.ProfessionalDevelopments.First());
            }
            ProfessionalDevelopmentEntity professionalDevelopment = new ProfessionalDevelopmentEntity();
            CertificateEntity certificate = new CertificateEntity();
            if (chkPDs != null)
            {
                principalEntity.UserInfo.ProfessionalDevelopments = new List<ProfessionalDevelopmentEntity>();
                foreach (var item in chkPDs)
                {
                    professionalDevelopment = userService.GetProfessionalDevelopment(item);
                    principalEntity.UserInfo.ProfessionalDevelopments.Add(professionalDevelopment);
                }
            }
            if (certificates != null)
            {
                foreach (var item in certificates)
                {
                    certificate = userService.GetCertificate(item);
                    principalEntity.UserInfo.Certificates.Add(certificate);
                }
            }

            OperationResult result = new OperationResult(OperationResultType.Success);
            userService.UpdatePrincipal(principalEntity, false);
            userService.UpdateUser(principal.UserInfo, false);
            result = userService.UpdatePrincipal(principal);

            //update principal delegate permission
            if (principalEntity.UserInfo.Role == Role.Principal
                || principalEntity.UserInfo.Role == Role.TRS_Specialist
                || principalEntity.UserInfo.Role == Role.School_Specialist)
            {
                List<PrincipalEntity> listPrincipalDelegate = new List<PrincipalEntity>();
                listPrincipalDelegate = userService.PrincipalDirectors.Where(e => e.ParentId == principal.UserInfo.ID
                    && e.UserInfo.Status == EntityStatus.Active).ToList();
                int count = 0;
                if (listPrincipalDelegate.Count > 0)
                {
                    foreach (var item in listPrincipalDelegate)
                    {
                        count++;
                        while (item.UserInfo.PermissionRoles.Count > 0)
                        {
                            item.UserInfo.PermissionRoles.Remove(item.UserInfo.PermissionRoles.First());
                        }
                        item.UserInfo.PermissionRoles = principalEntity.UserInfo.PermissionRoles;
                        if (count == listPrincipalDelegate.Count())
                            userService.UpdatePrincipal(item, true);
                        else
                            userService.UpdatePrincipal(item, false);
                    }
                }
            }
            permissionBusiness.ClearCache();
            return result;
        }

        public OperationResult UpdateCommunityUser(CommunityUserEntity communityUser, List<int> certificates, int[] chkPackages)
        {
            CommunityUserEntity communityUserEntity = userService.GetCommunity(communityUser.ID);

            if (communityUser.UserInfo.Role == Role.Community || communityUser.UserInfo.Role == Role.District_Community_Specialist)
            {
                while (communityUserEntity.UserInfo.PermissionRoles.Count > 0)
                {
                    communityUserEntity.UserInfo.PermissionRoles.Remove(communityUserEntity.UserInfo.PermissionRoles.First());
                }
                communityUserEntity.UserInfo.PermissionRoles = new List<PermissionRoleEntity>();
                if (chkPackages != null)
                {
                    foreach (var item in chkPackages)
                    {
                        PermissionRoleEntity packageEntity = new PermissionRoleEntity();
                        packageEntity = permissionBusiness.GetRole(Convert.ToInt32(item));
                        communityUserEntity.UserInfo.PermissionRoles.Add(packageEntity);
                    }
                }
            }

            while (communityUserEntity.UserInfo.Certificates.Count > 0)
            {
                communityUserEntity.UserInfo.Certificates.Remove(communityUserEntity.UserInfo.Certificates.First());
            }
            CertificateEntity certificate = new CertificateEntity();
            if (certificates != null)
            {
                foreach (var item in certificates)
                {
                    certificate = userService.GetCertificate(item);
                    communityUserEntity.UserInfo.Certificates.Add(certificate);
                }
            }

            OperationResult result = new OperationResult(OperationResultType.Success);
            userService.UpdateCommunity(communityUserEntity, false);
            userService.UpdateUser(communityUser.UserInfo, false);
            result = userService.UpdateCommunity(communityUser);

            //update principal delegate permission
            if (communityUserEntity.UserInfo.Role == Role.Community || communityUserEntity.UserInfo.Role == Role.District_Community_Specialist)
            {
                List<CommunityUserEntity> listDelegate = new List<CommunityUserEntity>();
                listDelegate = userService.DistrictCommunitys.Where(e => e.ParentId == communityUserEntity.UserInfo.ID
                    && e.UserInfo.Status == EntityStatus.Active).ToList();
                int count = 0;
                if (listDelegate.Count > 0)
                {
                    foreach (var item in listDelegate)
                    {
                        count++;
                        while (item.UserInfo.PermissionRoles.Count > 0)
                        {
                            item.UserInfo.PermissionRoles.Remove(item.UserInfo.PermissionRoles.First());
                        }
                        item.UserInfo.PermissionRoles = communityUserEntity.UserInfo.PermissionRoles;
                        if (count == listDelegate.Count())
                            userService.UpdateCommunity(item, true);
                        else
                            userService.UpdateCommunity(item, false);
                    }
                }
            }
            permissionBusiness.ClearCache();
            return result;
        }

        public OperationResult UpdateStateWide(StateWideEntity stateWide, int[] chkPackages)
        {
            StateWideEntity stateWideEntity = GetStateWide(stateWide.ID);
            while (stateWideEntity.UserInfo.PermissionRoles.Count > 0)
            {
                stateWideEntity.UserInfo.PermissionRoles.Remove(stateWideEntity.UserInfo.PermissionRoles.First());
            }
            stateWideEntity.UserInfo.PermissionRoles = new List<PermissionRoleEntity>();
            if (chkPackages != null)
            {
                foreach (var item in chkPackages)
                {
                    PermissionRoleEntity packageEntity = new PermissionRoleEntity();
                    packageEntity = permissionBusiness.GetRole(Convert.ToInt32(item));
                    stateWideEntity.UserInfo.PermissionRoles.Add(packageEntity);
                }
            }
            OperationResult result = new OperationResult(OperationResultType.Success);
            result = userService.UpdateStateWide(stateWideEntity, false);
            userService.UpdateUser(stateWide.UserInfo, false);
            result = userService.UpdateStateWide(stateWide);
            permissionBusiness.ClearCache();
            return result;
        }

        public OperationResult UpdateAuditor(AuditorEntity auditor, int[] chkPackages)
        {
            AuditorEntity auditorEntity = GetAuditor(auditor.ID);
            while (auditorEntity.UserInfo.PermissionRoles.Count > 0)
            {
                auditorEntity.UserInfo.PermissionRoles.Remove(auditorEntity.UserInfo.PermissionRoles.First());
            }
            auditorEntity.UserInfo.PermissionRoles = new List<PermissionRoleEntity>();
            if (chkPackages != null)
            {
                foreach (var item in chkPackages)
                {
                    PermissionRoleEntity packageEntity = new PermissionRoleEntity();
                    packageEntity = permissionBusiness.GetRole(Convert.ToInt32(item));
                    auditorEntity.UserInfo.PermissionRoles.Add(packageEntity);
                }
            }
            OperationResult result = new OperationResult(OperationResultType.Success);
            userService.UpdateAuditor(auditorEntity, false);
            userService.UpdateUser(auditor.UserInfo, false);
            result = userService.UpdateAuditor(auditor);
            permissionBusiness.ClearCache();
            return result;
        }

        public OperationResult SaveEquipment(int teacherId, string[] serialNumber, string[] uTHealthTag,
            int[] chkEquipment, int[] isAssessmentEquipment)
        {
            TeacherEntity teacher = GetTeacher(teacherId, null);
            userService.DeleteTeacherEquipment(teacher.TeacherEquipmentRelations.ToList());
            teacher.TeacherEquipmentRelations = new List<TeacherEquipmentRelationEntity>();
            if (isAssessmentEquipment != null)
            {
                if (isAssessmentEquipment[0] == 1)
                {
                    teacher.IsAssessmentEquipment = true;
                    if (chkEquipment != null)
                    {
                        for (int i = 0; i < chkEquipment.Length; i++)
                        {
                            TeacherEquipmentRelationEntity teacherEquipment = new TeacherEquipmentRelationEntity();
                            teacherEquipment.TeacherId = teacherId;
                            teacherEquipment.EquipmentId = chkEquipment[i];
                            teacherEquipment.SerialNumber = serialNumber[i];
                            teacherEquipment.UTHealthTag = uTHealthTag[i];
                            teacher.TeacherEquipmentRelations.Add(teacherEquipment);
                        }
                    }
                }
            }

            OperationResult result = new OperationResult(OperationResultType.Success);
            result = userService.UpdateTeacher(teacher);
            return result;
        }

        public OperationResult SaveTransaction(TeacherTransactionEntity transaction)
        {
            TeacherEntity teacher = GetTeacher(transaction.TeacherId, null);
            teacher.TeacherTransactions = new List<TeacherTransactionEntity>();
            teacher.TeacherTransactions.Add(transaction);
            OperationResult result = new OperationResult(OperationResultType.Success);
            result = userService.UpdateTeacher(teacher);
            return result;
        }

        public OperationResult AssignClass(int teacherId, int[] chkClasses)
        {
            TeacherEntity teacher = GetTeacher(teacherId, null);
            while (teacher.Classes.Count > 0)
            {
                teacher.Classes.Remove(teacher.Classes.First());
            }
            if (chkClasses != null)
            {
                teacher.Classes = new List<ClassEntity>();
                foreach (var item in chkClasses)
                {
                    ClassEntity classEntity = new ClassEntity();
                    classEntity = classBusiness.GetClass(Convert.ToInt32(item));
                    teacher.Classes.Add(classEntity);
                }
            }
            OperationResult result = new OperationResult(OperationResultType.Success);
            result = userService.UpdateTeacher(teacher);
            return result;
        }

        public OperationResult AssignPermission(int userId, int[] chkPackages, int[] chkDisablePackages, bool isAdmin)
        {
            UserBaseEntity user = GetUser(userId);
            while (user.PermissionRoles.Count > 0)
            {
                user.PermissionRoles.Remove(user.PermissionRoles.First());
            }
            user.PermissionRoles = new List<PermissionRoleEntity>();
            if (chkPackages != null)
            {
                foreach (var item in chkPackages)
                {
                    PermissionRoleEntity packageEntity = new PermissionRoleEntity();
                    packageEntity = permissionBusiness.GetRole(Convert.ToInt32(item));
                    user.PermissionRoles.Add(packageEntity);
                }
            }

            if (isAdmin) //只有管理员可操作DisabledUserRole选项
            {
                permissionBusiness.DeleteDisabledUserRole(user.DisabledUsrRoles.ToList(), false);
                if (chkDisablePackages != null && chkDisablePackages.Length > 0)
                {
                    user.DisabledUsrRoles = new List<DisabledUserRoleEntity>();
                    foreach (var item in chkDisablePackages)
                    {
                        DisabledUserRoleEntity disableEntity = new DisabledUserRoleEntity();
                        disableEntity.RoleId = item;
                        user.DisabledUsrRoles.Add(disableEntity);
                    }
                }
            }

            OperationResult result = new OperationResult(OperationResultType.Success);
            result = userService.UpdateUser(user);

            //update current user all delegate permission
            if (user.Role == Role.Principal || user.Role == Role.TRS_Specialist
                || user.Role == Role.School_Specialist)
            {
                List<PrincipalEntity> listPrincipalDelegate = new List<PrincipalEntity>();
                listPrincipalDelegate = userService.PrincipalDirectors.Where(e => e.ParentId == user.ID
                    && e.UserInfo.Status == EntityStatus.Active).ToList();
                int count = 0;
                if (listPrincipalDelegate.Count > 0)
                {
                    foreach (var item in listPrincipalDelegate)
                    {
                        count++;
                        while (item.UserInfo.PermissionRoles.Count > 0)
                        {
                            item.UserInfo.PermissionRoles.Remove(item.UserInfo.PermissionRoles.First());
                        }
                        item.UserInfo.PermissionRoles = user.PermissionRoles;

                        if (isAdmin)  //更改DisabledUserRole
                        {
                            permissionBusiness.DeleteDisabledUserRole(item.UserInfo.DisabledUsrRoles.ToList(), false);
                            if (chkDisablePackages != null && chkDisablePackages.Length > 0)
                            {
                                item.UserInfo.DisabledUsrRoles = new List<DisabledUserRoleEntity>();
                                foreach (var disabled in chkDisablePackages)
                                {
                                    DisabledUserRoleEntity disableEntity = new DisabledUserRoleEntity();
                                    disableEntity.RoleId = disabled;
                                    item.UserInfo.DisabledUsrRoles.Add(disableEntity);
                                }
                            }
                        }

                        if (count == listPrincipalDelegate.Count())
                            result = userService.UpdatePrincipal(item, true);
                        else
                            userService.UpdatePrincipal(item, false);
                    }
                }
            }
            else if (user.Role == Role.Community || user.Role == Role.District_Community_Specialist)
            {
                List<CommunityUserEntity> listCommunityDelegate = new List<CommunityUserEntity>();
                listCommunityDelegate = userService.DistrictCommunitys.Where(e => e.ParentId == user.ID
                    && e.UserInfo.Status == EntityStatus.Active).ToList();
                int count = 0;
                if (listCommunityDelegate.Count > 0)
                {
                    foreach (var item in listCommunityDelegate)
                    {
                        count++;
                        while (item.UserInfo.PermissionRoles.Count > 0)
                        {
                            item.UserInfo.PermissionRoles.Remove(item.UserInfo.PermissionRoles.First());
                        }
                        item.UserInfo.PermissionRoles = user.PermissionRoles;

                        if (isAdmin) //更改DisabledUserRole
                        {
                            permissionBusiness.DeleteDisabledUserRole(item.UserInfo.DisabledUsrRoles.ToList(), false);
                            if (chkDisablePackages != null && chkDisablePackages.Length > 0)
                            {
                                item.UserInfo.DisabledUsrRoles = new List<DisabledUserRoleEntity>();
                                foreach (var disabled in chkDisablePackages)
                                {
                                    DisabledUserRoleEntity disableEntity = new DisabledUserRoleEntity();
                                    disableEntity.RoleId = disabled;
                                    item.UserInfo.DisabledUsrRoles.Add(disableEntity);
                                }
                            }
                        }

                        if (count == listCommunityDelegate.Count())
                            result = userService.UpdateCommunity(item, true);
                        else
                            userService.UpdateCommunity(item, false);
                    }
                }
            }
            permissionBusiness.ClearCache();
            return result;
        }
        #endregion

        #region Delete Methods
        public OperationResult DeleteUser(int id)
        {
            var result = new OperationResult(OperationResultType.Success);
            var entity = userService.GetUser(id);
            if (entity == null)
            {
                return result;
            }
            entity.Status = EntityStatus.Inactive;
            entity.IsDeleted = true;
            entity.GoogleId = entity.GoogleId + "_InternalUserDelete";
            result = userService.UpdateUser(entity);
            return result;
        }

        public OperationResult DeleteApplicant(int id)
        {
            var result = new OperationResult(OperationResultType.Success);
            var entity = userService.GetAppliant(id);
            if (entity == null)
            {
                return result;
            }
            entity.IsDeleted = true;
            result = userService.UpdateAppliant(entity);
            return result;
        }
        #endregion

        #region General User Code Method

        /// <summary>
        /// community 
        /// </summary>
        /// <returns></returns>
        public string CommunityUserCode()
        {
            long count = userService.GetUserCode();
            return string.Format("EDD{0}{1}", CommonAgent.Year.ToString().Substring(2, 2), count.ToString().PadLeft(5, '0'));
        }

        public string CommunityDelegateCode()
        {
            long count = userService.GetUserCode();
            return string.Format("EDE{0}{1}", CommonAgent.Year.ToString().Substring(2, 2), count.ToString().PadLeft(5, '0'));
        }


        public string CommunitySpecialistCode()
        {
            long count = userService.GetUserCode();
            return string.Format("EED{0}{1}", CommonAgent.Year.ToString().Substring(2, 2), count.ToString().PadLeft(5, '0'));
        }

        public string CommunitySpecialistDelegateCode()
        {
            long count = userService.GetUserCode();
            return string.Format("EDE{0}{1}", CommonAgent.Year.ToString().Substring(2, 2), count.ToString().PadLeft(5, '0'));
        }

        /// <summary>
        /// Principal
        /// </summary>
        /// <returns></returns>
        public string PrincipalCode()
        {
            long count = userService.GetUserCode();
            return string.Format("EPR{0}{1}", CommonAgent.Year.ToString().Substring(2, 2), count.ToString().PadLeft(5, '0'));
        }

        public string SchoolDelegateCode()
        {
            long count = userService.GetUserCode();
            return string.Format("EDE{0}{1}", CommonAgent.Year.ToString().Substring(2, 2), count.ToString().PadLeft(5, '0'));
        }

        public string SchoolSpecialistCode()
        {
            long count = userService.GetUserCode();
            return string.Format("EED{0}{1}", CommonAgent.Year.ToString().Substring(2, 2), count.ToString().PadLeft(5, '0'));
        }

        public string SchoolSpecialistDelegateCode()
        {
            long count = userService.GetUserCode();
            return string.Format("EDE{0}{1}", CommonAgent.Year.ToString().Substring(2, 2), count.ToString().PadLeft(5, '0'));
        }

        public string StatewideCode()
        {
            long count = userService.GetUserCode();
            return string.Format("EST{0}{1}", CommonAgent.Year.ToString().Substring(2, 2), count.ToString().PadLeft(5, '0'));
        }

        public string AuditorCode()
        {
            long count = userService.GetUserCode();
            return string.Format("ECO{0}{1}", CommonAgent.Year.ToString().Substring(2, 2), count.ToString().PadLeft(5, '0'));
        }

        #endregion

        #region Email
        public void SendEmail(string to, string subject, string emailBody)
        {
            userService.SendToUserEmail(to, subject, emailBody);
        }

        public void InsertEmailLog(EmailLogEntity emailLog)
        {
            userService.InsertEmailLog(emailLog);
        }
        #endregion

        #region Role Visit Permission Control
        public PrincipalRoleEntity GetPrincipalRoleEntity(Role role)
        {
            Role newRole = role;
            switch (role)
            {
                case Role.District_Community_Delegate:
                    newRole = Role.Community;
                    break;
                case Role.Principal_Delegate:
                    newRole = Role.Principal;
                    break;
                case Role.TRS_Specialist_Delegate:
                    newRole = Role.TRS_Specialist;
                    break;
                case Role.School_Specialist_Delegate:
                    newRole = Role.School_Specialist;
                    break;
                case Role.Community_Specialist_Delegate:
                    newRole = Role.District_Community_Specialist;
                    break;
                default:
                    newRole = role;
                    break;
            }
            return userService.GetPrincipalRole(newRole);
        }

        public CoordCoachRoleEntity GetCoordCoachRoleEntity(Role role)
        {
            Role newRole = role;
            switch (role)
            {
                case Role.District_Community_Delegate:
                    newRole = Role.Community;
                    break;
                case Role.Principal_Delegate:
                    newRole = Role.Principal;
                    break;
                case Role.TRS_Specialist_Delegate:
                    newRole = Role.TRS_Specialist;
                    break;
                case Role.School_Specialist_Delegate:
                    newRole = Role.School_Specialist;
                    break;
                case Role.Community_Specialist_Delegate:
                    newRole = Role.District_Community_Specialist;
                    break;
                default:
                    newRole = role;
                    break;
            }
            return userService.GetCoordCoachRole(role);
        }

        public TeacherRoleEntity GetTeacherRoleEntity(Role role)
        {
            Role newRole = role;
            switch (role)
            {
                case Role.District_Community_Delegate:
                    newRole = Role.Community;
                    break;
                case Role.Principal_Delegate:
                    newRole = Role.Principal;
                    break;
                case Role.TRS_Specialist_Delegate:
                    newRole = Role.TRS_Specialist;
                    break;
                case Role.School_Specialist_Delegate:
                    newRole = Role.School_Specialist;
                    break;
                case Role.Community_Specialist_Delegate:
                    newRole = Role.District_Community_Specialist;
                    break;
                default:
                    newRole = role;
                    break;
            }
            return userService.GetTeacherRole(role);
        }

        /// <summary>
        /// 查找CommunityUser
        /// </summary>
        /// <param name="userInfo"></param>
        /// <returns></returns>
        private Expression<Func<CommunityUserEntity, bool>> GetCommunityUserRoleCondition(UserBaseEntity userInfo,
            Role roleType)
        {
            Expression<Func<CommunityUserEntity, bool>> condition = o => true;
            if (userInfo == null)
                return condition;
            UserBaseEntity baseUser = GetUser(userInfo.ID);
            List<int> comIds = baseUser.UserCommunitySchools.Where(o => o.CommunityId > 0).Select(o => o.CommunityId).ToList();
            switch (baseUser.Role)
            {
                case Role.Content_personnel:
                case Role.Statisticians:
                case Role.Administrative_personnel:
                case Role.Intervention_manager:
                case Role.Video_coding_analyst:
                case Role.Intervention_support_personnel:
                case Role.Coordinator:
                case Role.Mentor_coach:
                    if (roleType == Role.Community)
                    {
                        condition = PredicateBuilder.And(condition,
                        o => o.UserInfo.Role == Role.Community && o.UserInfo.UserCommunitySchools.Any(q => comIds.Contains(q.CommunityId)));
                    }
                    else
                        condition = o => true;
                    break;

                case Role.Statewide:
                    if (baseUser.StateWide != null)
                        if (roleType == Role.Community)
                            condition = PredicateBuilder.And(condition,
                                o =>
                                    o.UserInfo.Role == Role.Community &&
                                    o.UserInfo.UserCommunitySchools.Any(
                                        p => p.Community.UserCommunitySchools.Any(q => q.UserId == userInfo.ID)));
                        else
                            condition = o => false;
                    else
                        condition = o => false;
                    break;
                case Role.Community: //communityuser可以看到自己的delegate
                    if (baseUser.CommunityUser != null)
                        if (roleType == Role.Community)
                            condition = PredicateBuilder.And(condition,
                                o =>
                                    o.UserInfo.Role == Role.Community &&
                                    o.UserInfo.UserCommunitySchools.Any(
                                        p => p.Community.UserCommunitySchools.Any(q => q.UserId == userInfo.ID)));
                        else if (roleType == Role.District_Community_Delegate)
                            condition = PredicateBuilder.And(condition,
                                o => o.UserInfo.Role == Role.District_Community_Delegate
                                     && o.ParentId == userInfo.ID);
                        else
                            condition = o => false;
                    else
                        condition = o => false;
                    break;
                case Role.District_Community_Delegate:
                    if (baseUser.CommunityUser != null)
                        if (roleType == Role.Community)
                            condition = PredicateBuilder.And(condition, o => o.UserInfo.Role == Role.Community
                                                                             &&
                                                                             o.UserInfo.ID ==
                                                                             userInfo.CommunityUser.ParentId);
                        else if (roleType == Role.District_Community_Delegate)
                            condition = PredicateBuilder.And(condition,
                                o => o.UserInfo.Role == Role.District_Community_Delegate
                                     && o.UserInfo.ID == userInfo.ID);
                        else
                            condition = o => false;
                    else
                        condition = o => false;
                    break;
                //其余外部用户都不能看到            
                case Role.Teacher:
                case Role.Parent:
                case Role.Principal:
                case Role.Principal_Delegate:
                case Role.TRS_Specialist:
                case Role.TRS_Specialist_Delegate:
                case Role.School_Specialist:
                case Role.School_Specialist_Delegate:
                case Role.District_Community_Specialist:
                case Role.Community_Specialist_Delegate:
                    condition = o => false;
                    break;
                default:
                    if (roleType == Role.Community)
                        condition = PredicateBuilder.And(condition, o => o.UserInfo.Role == Role.Community);
                    else
                        condition = PredicateBuilder.And(condition, o => o.UserInfo.Role == Role.District_Community_Delegate);
                    break;
            }

            return condition;
        }

        /// <summary>
        /// 查找community specialist
        /// </summary>
        /// <param name="userInfo"></param>
        /// <returns></returns>
        private Expression<Func<CommunityUserEntity, bool>> GetCommunitySpecialistRoleCondition(UserBaseEntity userInfo, Role roleType)
        {
            Expression<Func<CommunityUserEntity, bool>> condition = o => true;
            if (userInfo == null)
                return condition;

            UserBaseEntity baseUser = GetUser(userInfo.ID);
            List<int> comIds = baseUser.UserCommunitySchools.Where(o => o.CommunityId > 0).Select(o => o.CommunityId).ToList();
            switch (baseUser.Role)
            {
                case Role.Content_personnel:
                case Role.Statisticians:
                case Role.Administrative_personnel:
                case Role.Intervention_manager:
                case Role.Video_coding_analyst:
                case Role.Intervention_support_personnel:
                case Role.Coordinator:
                case Role.Mentor_coach:
                    if (roleType == Role.District_Community_Specialist)
                    {
                        condition = PredicateBuilder.And(condition,
                         o => o.UserInfo.UserCommunitySchools.Any(q => comIds.Contains(q.CommunityId)));
                    }
                    else
                    {
                        condition = o => true;
                    }
                    break;

                case Role.District_Community_Specialist: //communityspecialistuser可以看到自己的delegate
                    if (baseUser.CommunityUser != null)
                        if (roleType == Role.District_Community_Specialist)
                            condition = PredicateBuilder.And(condition,
                                o => o.UserInfo.Role == Role.District_Community_Specialist
                                     &&
                                     o.UserInfo.UserCommunitySchools.Any(
                                         p => p.Community.UserCommunitySchools.Any(q => q.UserId == userInfo.ID)));
                        else if (roleType == Role.Community_Specialist_Delegate)
                            condition = PredicateBuilder.And(condition, o => o.UserInfo.Role == Role.Community_Specialist_Delegate
                               && o.ParentId == userInfo.ID);
                        else
                            condition = o => false;
                    else
                        condition = o => false;
                    break;
                case Role.Community_Specialist_Delegate:
                    if (baseUser.CommunityUser != null)
                        if (roleType == Role.District_Community_Specialist)
                            condition = PredicateBuilder.And(condition, o => o.UserInfo.Role == Role.District_Community_Specialist
                           && o.UserInfo.ID == userInfo.CommunityUser.ParentId);
                        else if (roleType == Role.Community_Specialist_Delegate)
                            condition = PredicateBuilder.And(condition, o => o.UserInfo.Role == Role.Community_Specialist_Delegate
                               && o.UserInfo.ID == userInfo.ID);
                        else
                            condition = o => false;
                    else
                        condition = o => false;
                    break;
                case Role.Statewide:
                case Role.Community:
                    if (baseUser.CommunityUser != null || baseUser.StateWide != null)
                    {
                        if (roleType == Role.District_Community_Specialist)
                            condition = PredicateBuilder.And(condition,
                                o => o.UserInfo.Role == Role.District_Community_Specialist
                                     &&
                                     o.UserInfo.UserCommunitySchools.Any(
                                         p => p.Community.UserCommunitySchools.Any(q => q.UserId == userInfo.ID)));
                        else
                            condition = o => false;
                    }
                    else
                        condition = o => false;
                    break;
                case Role.District_Community_Delegate:
                    if (baseUser.CommunityUser != null)
                    {
                        if (roleType == Role.District_Community_Specialist)
                            condition = PredicateBuilder.And(condition,
                                o => o.UserInfo.Role == Role.District_Community_Specialist
                                     &&
                                     o.UserInfo.UserCommunitySchools.Any(
                                         p =>
                                             p.Community.UserCommunitySchools.Any(
                                                 q => q.UserId == userInfo.CommunityUser.ParentId)));
                        else
                            condition = o => false;
                    }
                    else
                        condition = o => false;
                    break;

                //其余外部用户都不能看到            
                case Role.Teacher:
                case Role.Parent:
                case Role.Principal:
                case Role.Principal_Delegate:
                case Role.TRS_Specialist:
                case Role.TRS_Specialist_Delegate:
                case Role.School_Specialist:
                case Role.School_Specialist_Delegate:
                    condition = o => false;
                    break;
                default:
                    if (roleType == Role.District_Community_Specialist)
                        condition = PredicateBuilder.And(condition, o => o.UserInfo.Role == Role.District_Community_Specialist);
                    else
                        condition = PredicateBuilder.And(condition, o => o.UserInfo.Role == Role.Community_Specialist_Delegate);
                    break;
            }

            return condition;
        }


        private Expression<Func<TeacherEntity, bool>> GetTeacherCondition(UserBaseEntity userInfo)
        {
            Expression<Func<TeacherEntity, bool>> condition = o => true;
            if (userInfo == null)
                return condition;
            UserBaseEntity baseUser = GetUser(userInfo.ID);

            switch (userInfo.Role)
            {
                case Role.Content_personnel:
                case Role.Statisticians:
                case Role.Administrative_personnel:
                case Role.Intervention_manager:
                case Role.Video_coding_analyst:
                case Role.Intervention_support_personnel:
                case Role.Coordinator:
                case Role.Mentor_coach:
                    List<int> comIds = baseUser.UserCommunitySchools.Where(o => o.CommunityId > 0).Select(o => o.CommunityId).ToList();
                    condition = PredicateBuilder.And(condition,
                           o => o.UserInfo.UserCommunitySchools.Any(p => comIds.Contains(p.CommunityId)));
                    break;

                case Role.Teacher:
                    if (baseUser.TeacherInfo != null)
                        condition = PredicateBuilder.And(condition, o => o.ID == baseUser.TeacherInfo.ID);
                    else
                        condition = o => false;
                    break;
                case Role.Principal:
                case Role.TRS_Specialist:
                case Role.School_Specialist:
                    if (baseUser.Principal != null)
                    {
                        condition = PredicateBuilder.And(condition,
                            o => o.UserInfo.UserCommunitySchools.Any(p => p.School.UserCommunitySchools.Any(q => q.UserId == userInfo.ID)));
                    }
                    else
                        condition = o => false;
                    break;
                case Role.Principal_Delegate:
                case Role.TRS_Specialist_Delegate:
                case Role.School_Specialist_Delegate:
                    if (baseUser.Principal != null)
                    {
                        condition = PredicateBuilder.And(condition,
                            o =>
                                o.UserInfo.UserCommunitySchools.Any(
                                    p => p.School.UserCommunitySchools.Any(q => q.UserId == userInfo.Principal.ParentId)));
                    }
                    else
                        condition = o => false;
                    break;
                case Role.Statewide:
                case Role.Community:
                case Role.District_Community_Specialist:
                    if (baseUser.CommunityUser != null || baseUser.StateWide != null)
                        condition = PredicateBuilder.And(condition,
                            o =>
                                o.UserInfo.UserCommunitySchools.Any(
                                    p => p.Community.UserCommunitySchools.Any(q => q.UserId == userInfo.ID)));
                    else
                        condition = o => false;
                    break;
                case Role.District_Community_Delegate:
                case Role.Community_Specialist_Delegate:
                    if (baseUser.CommunityUser != null)
                        condition = PredicateBuilder.And(condition,
                            o =>
                                o.UserInfo.UserCommunitySchools.Any(
                                    p =>
                                        p.Community.UserCommunitySchools.Any(q => q.UserId == userInfo.CommunityUser.ParentId)));
                    else
                        condition = o => false;
                    break;
                case Role.Parent:
                    condition = o => false;
                    break;
            }
            return condition;
        }

        private Expression<Func<ParentEntity, bool>> GetParentCondition(UserBaseEntity userInfo)
        {
            Expression<Func<ParentEntity, bool>> condition = o => true;
            if (userInfo == null)
                return condition;
            UserBaseEntity baseUser = GetUser(userInfo.ID);
            List<int> comIds = baseUser.UserCommunitySchools.Where(o => o.CommunityId > 0).Select(o => o.CommunityId).ToList();

            switch (userInfo.Role)
            {
                case Role.Content_personnel:
                case Role.Statisticians:
                case Role.Administrative_personnel:
                case Role.Intervention_manager:
                case Role.Video_coding_analyst:
                case Role.Intervention_support_personnel:
                case Role.Coordinator:
                case Role.Mentor_coach:
                    List<int> stuIds = studentBusiness.GetStudentsBySchoolIdCommId(comIds, 0);
                    List<int> parIds = GetParentIDsbyStudentIds(stuIds);
                    condition = PredicateBuilder.And(condition, o => parIds.Contains(o.ID));
                    break;

                case Role.Parent:
                    if (baseUser.Parent != null)
                        condition = PredicateBuilder.And(condition, o => o.ID == baseUser.Parent.ID);
                    else
                        condition = o => false;
                    break;
                case Role.Teacher:
                    if (baseUser.TeacherInfo != null)
                    {
                        List<int> classesIds = baseUser.TeacherInfo.Classes.Where(e => e.IsDeleted == false).Select(e => e.ID).ToList<int>();
                        List<int> studentIds = studentBusiness.GetStudentsByClassIds(classesIds);
                        List<int> parentIds = GetParentIDsbyStudentIds(studentIds);
                        condition = PredicateBuilder.And(condition, o => parentIds.Contains(o.ID));
                    }
                    else
                        condition = o => false;
                    break;
                case Role.Principal:
                case Role.TRS_Specialist:
                case Role.School_Specialist:
                    if (baseUser.Principal != null)
                    {
                        List<int> schoolIds = baseUser.UserCommunitySchools.Select(e => e.SchoolId).ToList();
                        var classIds = classBusiness.GetClassesBySchoolId(schoolIds, userInfo).Select(e => e.ClassId).ToList();
                        classIds.AddRange(baseUser.UserClasses.Where(e => e.Class.IsDeleted == false).Select(e => e.ClassId).ToList());
                        List<int> studentIds =
                            studentBusiness.GetStudentsByClassIds(classIds);
                        List<int> parentIds = GetParentIDsbyStudentIds(studentIds);
                        condition = PredicateBuilder.And(condition, o => parentIds.Contains(o.ID));
                    }
                    else
                        condition = o => false;
                    break;
                case Role.Principal_Delegate:
                case Role.TRS_Specialist_Delegate:
                case Role.School_Specialist_Delegate:
                    if (baseUser.Principal != null)
                    {
                        var delegateUser = GetUser(baseUser.Principal.ParentId);
                        List<int> schoolIds = delegateUser.UserCommunitySchools.Select(e => e.SchoolId).ToList();
                        var classIds = classBusiness.GetClassesBySchoolId(schoolIds, userInfo).Select(e => e.ClassId).ToList();
                        classIds.AddRange(delegateUser.UserClasses.Where(e => e.Class.IsDeleted == false).Select(e => e.ClassId).ToList());
                        List<int> studentIds =
                            studentBusiness.GetStudentsByClassIds(classIds);
                        List<int> parentIds = GetParentIDsbyStudentIds(studentIds);
                        condition = PredicateBuilder.And(condition, o => parentIds.Contains(o.ID));
                    }
                    else
                        condition = o => false;
                    break;
                case Role.Statewide:
                case Role.Community:
                case Role.District_Community_Specialist:
                    if (baseUser.CommunityUser != null || baseUser.StateWide != null)
                    {
                        var communityIds = baseUser.UserCommunitySchools.Select(e => e.CommunityId).ToList();
                        var primarySchoolIds = schoolBusiness.GetPrimarySchoolsByComId(communityIds).Select(e => e.ID).ToList();
                        var classIds = classBusiness.GetClassesBySchoolId(primarySchoolIds, userInfo).Select(e => e.ClassId).ToList();
                        classIds.AddRange(baseUser.UserClasses.Where(e => e.Class.IsDeleted == false).Select(e => e.ClassId).ToList());
                        List<int> studentIds =
                            studentBusiness.GetStudentsByClassIds(classIds);
                        List<int> parentIds = GetParentIDsbyStudentIds(studentIds);
                        condition = PredicateBuilder.And(condition, o => parentIds.Contains(o.ID));
                    }
                    else
                        condition = o => false;
                    break;
                case Role.District_Community_Delegate:
                case Role.Community_Specialist_Delegate:
                    if (baseUser.CommunityUser != null)
                    {
                        var parentCommunityUser = GetUser(baseUser.CommunityUser.ParentId);
                        var communityIds = parentCommunityUser.UserCommunitySchools.Select(e => e.CommunityId).ToList();
                        var primarySchoolIds = schoolBusiness.GetPrimarySchoolsByComId(communityIds).Select(e => e.ID).ToList();
                        var classIds = classBusiness.GetClassesBySchoolId(primarySchoolIds, userInfo).Select(e => e.ClassId).ToList();
                        classIds.AddRange(parentCommunityUser.UserClasses.Where(c => c.Class.IsDeleted == false).Select(e => e.ClassId).ToList());
                        List<int> studentIds =
                            studentBusiness.GetStudentsByClassIds(classIds);
                        List<int> parentIds = GetParentIDsbyStudentIds(studentIds);
                        condition = PredicateBuilder.And(condition, o => parentIds.Contains(o.ID));
                    }
                    else
                        condition = o => false;
                    break;
            }
            return condition;
        }

        private Expression<Func<ParentStudentRelationEntity, bool>> GetParentStudentCondition(UserBaseEntity userInfo)
        {
            Expression<Func<ParentStudentRelationEntity, bool>> condition = o => true;
            if (userInfo == null)
                return condition;
            UserBaseEntity baseUser = GetUser(userInfo.ID);
            List<int> comIds = baseUser.UserCommunitySchools.Where(o => o.CommunityId > 0).Select(o => o.CommunityId).ToList();

            switch (userInfo.Role)
            {
                case Role.Content_personnel:
                case Role.Statisticians:
                case Role.Administrative_personnel:
                case Role.Intervention_manager:
                case Role.Video_coding_analyst:
                case Role.Intervention_support_personnel:
                case Role.Coordinator:
                case Role.Mentor_coach:
                    List<int> stuIds = studentBusiness.GetStudentsBySchoolIdCommId(comIds, 0);
                    condition = PredicateBuilder.And(condition, o => stuIds.Contains(o.StudentId));
                    break;

                case Role.Parent:
                    if (baseUser.Parent != null)
                        condition = PredicateBuilder.And(condition, o => o.ParentId == baseUser.Parent.ID);
                    else
                        condition = o => false;
                    break;
                case Role.Teacher:
                    if (baseUser.TeacherInfo != null)
                    {
                        List<int> classesIds = baseUser.TeacherInfo.Classes.Where(e => e.IsDeleted == false).Select(e => e.ID).ToList<int>();
                        List<int> studentIds = studentBusiness.GetStudentsByClassIds(classesIds);
                        condition = PredicateBuilder.And(condition, o => studentIds.Contains(o.StudentId));
                    }
                    else
                        condition = o => false;
                    break;
                case Role.Principal:
                case Role.TRS_Specialist:
                case Role.School_Specialist:
                    if (baseUser.Principal != null)
                    {
                        List<int> schoolIds = baseUser.UserCommunitySchools.Select(e => e.SchoolId).ToList();
                        var classIds = classBusiness.GetClassesBySchoolId(schoolIds, userInfo).Select(e => e.ClassId).ToList();
                        classIds.AddRange(baseUser.UserClasses.Where(e => e.Class.IsDeleted == false).Select(e => e.ClassId).ToList());
                        List<int> studentIds =
                            studentBusiness.GetStudentsByClassIds(classIds);
                        condition = PredicateBuilder.And(condition, o => studentIds.Contains(o.StudentId));
                    }
                    else
                        condition = o => false;
                    break;
                case Role.Principal_Delegate:
                case Role.TRS_Specialist_Delegate:
                case Role.School_Specialist_Delegate:
                    if (baseUser.Principal != null)
                    {
                        var delegateUser = GetUser(baseUser.Principal.ParentId);
                        List<int> schoolIds = delegateUser.UserCommunitySchools.Select(e => e.SchoolId).ToList();
                        var classIds = classBusiness.GetClassesBySchoolId(schoolIds, userInfo).Select(e => e.ClassId).ToList();
                        classIds.AddRange(delegateUser.UserClasses.Where(e => e.Class.IsDeleted == false).Select(e => e.ClassId).ToList());
                        List<int> studentIds =
                            studentBusiness.GetStudentsByClassIds(classIds);
                        condition = PredicateBuilder.And(condition, o => studentIds.Contains(o.StudentId));
                    }
                    else
                        condition = o => false;
                    break;
                case Role.Statewide:
                case Role.Community:
                case Role.District_Community_Specialist:
                    if (baseUser.CommunityUser != null || baseUser.StateWide != null)
                    {
                        var communityIds = baseUser.UserCommunitySchools.Select(e => e.CommunityId).ToList();
                        var primarySchoolIds = schoolBusiness.GetPrimarySchoolsByComId(communityIds).Select(e => e.ID).ToList();
                        var classIds = classBusiness.GetClassesBySchoolId(primarySchoolIds, userInfo).Select(e => e.ClassId).ToList();
                        classIds.AddRange(baseUser.UserClasses.Where(e => e.Class.IsDeleted == false).Select(e => e.ClassId).ToList());
                        List<int> studentIds =
                            studentBusiness.GetStudentsByClassIds(classIds);
                        condition = PredicateBuilder.And(condition, o => studentIds.Contains(o.StudentId));
                    }
                    else
                        condition = o => false;
                    break;
                case Role.District_Community_Delegate:
                case Role.Community_Specialist_Delegate:
                    if (baseUser.CommunityUser != null)
                    {
                        var parentCommunityUser = GetUser(baseUser.CommunityUser.ParentId);
                        var communityIds = parentCommunityUser.UserCommunitySchools.Select(e => e.CommunityId).ToList();
                        var primarySchoolIds = schoolBusiness.GetPrimarySchoolsByComId(communityIds).Select(e => e.ID).ToList();
                        var classIds = classBusiness.GetClassesBySchoolId(primarySchoolIds, userInfo).Select(e => e.ClassId).ToList();
                        classIds.AddRange(parentCommunityUser.UserClasses.Where(c => c.Class.IsDeleted == false).Select(e => e.ClassId).ToList());
                        List<int> studentIds =
                            studentBusiness.GetStudentsByClassIds(classIds);
                        condition = PredicateBuilder.And(condition, o => studentIds.Contains(o.StudentId));
                    }
                    else
                        condition = o => false;
                    break;
            }
            return condition;
        }

        /// <summary>
        /// 查找TRSSpecialist
        /// </summary>
        /// <param name="userInfo"></param>
        /// <returns></returns>
        private Expression<Func<PrincipalEntity, bool>> GetTRSSpecialistCondition(UserBaseEntity userInfo, Role roleType)
        {
            Expression<Func<PrincipalEntity, bool>> condition = o => true;
            if (userInfo == null)
                return condition;
            UserBaseEntity baseUser = GetUser(userInfo.ID);
            List<int> comIds = baseUser.UserCommunitySchools.Where(o => o.CommunityId > 0).Select(o => o.CommunityId).ToList();
            switch (userInfo.Role)
            {
                case Role.Content_personnel:
                case Role.Statisticians:
                case Role.Administrative_personnel:
                case Role.Intervention_manager:
                case Role.Video_coding_analyst:
                case Role.Intervention_support_personnel:
                case Role.Coordinator:
                case Role.Mentor_coach:
                    if (roleType == Role.TRS_Specialist)
                    {
                        condition = PredicateBuilder.And(condition,
                          o => o.UserInfo.Role == Role.TRS_Specialist && o.UserInfo.UserCommunitySchools.Where(relation => relation.SchoolId > 0)
                              .Any(q => (q.School.CommunitySchoolRelations.Any(s => comIds.Contains(s.CommunityId)))));
                    }
                    else
                        condition = o => true;
                    break;

                case Role.Teacher:
                case Role.Parent:
                    condition = o => false;
                    break;

                case Role.TRS_Specialist: //可查看trs specialist and delegate
                    if (baseUser.Principal != null)
                        if (roleType == Role.TRS_Specialist)
                            condition = PredicateBuilder.And(condition, o => o.UserInfo.Role == Role.TRS_Specialist
                                                                             &&
                                                                             o.UserInfo.UserCommunitySchools.Any(
                                                                                 p => p.UserId == userInfo.ID));
                        else if (roleType == Role.TRS_Specialist_Delegate)
                            condition = PredicateBuilder.And(condition, o => o.UserInfo.Role == Role.TRS_Specialist_Delegate
                               && o.ParentId == userInfo.ID);
                        else
                            condition = o => false;
                    else
                        condition = o => false;
                    break;

                case Role.TRS_Specialist_Delegate:
                    if (baseUser.Principal != null)
                        if (roleType == Role.TRS_Specialist)
                            condition = PredicateBuilder.And(condition, o => o.UserInfo.Role == Role.TRS_Specialist
                           && o.UserInfo.ID == userInfo.Principal.ParentId);
                        else if (roleType == Role.TRS_Specialist_Delegate)
                            condition = PredicateBuilder.And(condition, o => o.UserInfo.Role == Role.TRS_Specialist_Delegate
                               && o.UserInfo.ID == userInfo.ID);
                        else
                            condition = o => false;
                    else
                        condition = o => false;
                    break;

                case Role.Principal://可查看trs specialist
                case Role.School_Specialist:
                    if (baseUser.Principal != null)
                    {
                        if (roleType == Role.TRS_Specialist)
                            condition = PredicateBuilder.And(condition,
                                o => o.UserInfo.Role == Role.TRS_Specialist
                                     && o.UserInfo.UserCommunitySchools.Any(
                                         p => p.School.UserCommunitySchools.Any(q => q.UserId == userInfo.ID)));
                        else
                            condition = o => false;
                    }
                    else
                        condition = o => false;
                    break;

                case Role.Principal_Delegate:
                case Role.School_Specialist_Delegate:
                    if (baseUser.Principal != null)
                    {
                        if (roleType == Role.TRS_Specialist)
                            condition = PredicateBuilder.And(condition,
                                o => o.UserInfo.Role == Role.TRS_Specialist
                                     && o.UserInfo.UserCommunitySchools.Any(
                                         p => p.School.UserCommunitySchools.Any(q => q.UserId == userInfo.Principal.ParentId)));
                        else
                            condition = o => false;
                    }
                    else
                        condition = o => false;
                    break;

                case Role.Statewide:
                case Role.Community:
                case Role.District_Community_Specialist:
                    if (baseUser.CommunityUser != null || userInfo.StateWide != null)
                    {
                        if (roleType == Role.TRS_Specialist)
                            condition = PredicateBuilder.And(condition,
                                o =>
                                    o.UserInfo.Role == Role.TRS_Specialist &&
                                    o.UserInfo.UserCommunitySchools.Any(
                                        m =>
                                            m.School.CommunitySchoolRelations.Any(
                                                n => n.Community.UserCommunitySchools.Any(p => p.UserId == userInfo.ID))));
                        else
                            condition = o => false;
                    }
                    else
                        condition = o => false;
                    break;
                case Role.District_Community_Delegate:
                case Role.Community_Specialist_Delegate:
                    if (baseUser.CommunityUser != null)
                    {
                        if (roleType == Role.TRS_Specialist)
                            condition = PredicateBuilder.And(condition,
                                o =>
                                    o.UserInfo.Role == Role.TRS_Specialist &&
                                    o.UserInfo.UserCommunitySchools.Any(
                                        m =>
                                            m.School.CommunitySchoolRelations.Any(
                                                n =>
                                                    n.Community.UserCommunitySchools.Any(
                                                        p => p.UserId == userInfo.CommunityUser.ParentId))));
                        else
                            condition = o => false;
                    }
                    else
                        condition = o => false;
                    break;
                default:
                    if (roleType == Role.TRS_Specialist)
                        condition = PredicateBuilder.And(condition, o => o.UserInfo.Role == Role.TRS_Specialist);
                    else
                        condition = PredicateBuilder.And(condition, o => o.UserInfo.Role == Role.TRS_Specialist_Delegate);
                    break;
            }
            return condition;
        }

        /// <summary>
        /// 查找SchoolSpecialist
        /// </summary>
        /// <param name="userInfo"></param>
        /// <returns></returns>
        private Expression<Func<PrincipalEntity, bool>> GetSchoolSpecialistCondition(UserBaseEntity userInfo, Role roleType)
        {
            Expression<Func<PrincipalEntity, bool>> condition = o => true;
            if (userInfo == null)
                return condition;
            UserBaseEntity baseUser = GetUser(userInfo.ID);
            List<int> comIds = baseUser.UserCommunitySchools.Where(o => o.CommunityId > 0).Select(o => o.CommunityId).ToList();
            switch (userInfo.Role)
            {
                case Role.Content_personnel:
                case Role.Statisticians:
                case Role.Administrative_personnel:
                case Role.Intervention_manager:
                case Role.Video_coding_analyst:
                case Role.Intervention_support_personnel:
                case Role.Coordinator:
                case Role.Mentor_coach:
                    if (roleType == Role.School_Specialist)
                    {
                        condition = PredicateBuilder.And(condition,
                        o => o.UserInfo.Role == Role.School_Specialist && o.UserInfo.UserCommunitySchools.Where(relation => relation.SchoolId > 0)
                            .Any(q => (q.School.CommunitySchoolRelations.Any(s => comIds.Contains(s.CommunityId)))));
                    }
                    else
                    {
                        condition = o => true;
                    }
                    break;

                case Role.Teacher:
                case Role.Parent:
                    condition = o => false;
                    break;

                case Role.School_Specialist: //可查看school specialist and delegate
                    if (baseUser.Principal != null)
                        if (roleType == Role.School_Specialist)
                            condition = PredicateBuilder.And(condition, o => o.UserInfo.Role == Role.School_Specialist
                                                                             &&
                                                                             o.UserInfo.UserCommunitySchools.Any(
                                                                                 p => p.UserId == userInfo.ID));
                        else if (roleType == Role.School_Specialist_Delegate)
                            condition = PredicateBuilder.And(condition, o => o.UserInfo.Role == Role.School_Specialist_Delegate
                               && o.ParentId == userInfo.ID);
                        else
                            condition = o => false;
                    else
                        condition = o => false;
                    break;

                case Role.School_Specialist_Delegate:
                    if (baseUser.Principal != null)
                        if (roleType == Role.School_Specialist)
                            condition = PredicateBuilder.And(condition, o => o.UserInfo.Role == Role.School_Specialist
                           && o.UserInfo.ID == userInfo.Principal.ParentId);
                        else if (roleType == Role.School_Specialist_Delegate)
                            condition = PredicateBuilder.And(condition, o => o.UserInfo.Role == Role.School_Specialist_Delegate
                               && o.UserInfo.ID == userInfo.ID);
                        else
                            condition = o => false;
                    else
                        condition = o => false;
                    break;

                case Role.Principal://可查看school specialist
                case Role.TRS_Specialist:
                    if (baseUser.Principal != null)
                    {
                        if (roleType == Role.School_Specialist)
                            condition = PredicateBuilder.And(condition,
                                o => o.UserInfo.Role == Role.School_Specialist
                                     && o.UserInfo.UserCommunitySchools.Any(
                                         p => p.School.UserCommunitySchools.Any(q => q.UserId == userInfo.ID)));
                        else
                            condition = o => false;
                    }
                    else
                        condition = o => false;
                    break;

                case Role.Principal_Delegate:
                case Role.TRS_Specialist_Delegate:
                    if (baseUser.Principal != null)
                    {
                        if (roleType == Role.School_Specialist)
                            condition = PredicateBuilder.And(condition,
                                o => o.UserInfo.Role == Role.School_Specialist
                                     && o.UserInfo.UserCommunitySchools.Any(
                                         p => p.School.UserCommunitySchools.Any(q => q.UserId == userInfo.Principal.ParentId)));
                        else
                            condition = o => false;
                    }
                    else
                        condition = o => false;
                    break;

                case Role.Statewide:
                case Role.Community:
                case Role.District_Community_Specialist:
                    if (baseUser.CommunityUser != null || userInfo.StateWide != null)
                    {
                        var primarySchoolIds = schoolBusiness.GetPrimarySchoolsByComId(
                            userInfo.UserCommunitySchools.Select(e => e.CommunityId).ToList()).Select(x => x.ID).ToList();
                        if (roleType == Role.School_Specialist)
                            condition = PredicateBuilder.And(condition,
                                o =>
                                    o.UserInfo.Role == Role.School_Specialist &&
                                    o.UserInfo.UserCommunitySchools.Any(p => primarySchoolIds.Contains(p.SchoolId)));
                        else
                            condition = o => false;
                    }
                    else
                        condition = o => false;
                    break;
                case Role.District_Community_Delegate:
                case Role.Community_Specialist_Delegate:
                    if (baseUser.CommunityUser != null)
                    {
                        var parentCommunityUser = GetUser(userInfo.ID);
                        var primarySchoolIds = schoolBusiness.GetPrimarySchoolsByComId(
                            parentCommunityUser.UserCommunitySchools.Select(e => e.CommunityId).ToList()).Select(x => x.ID).ToList();
                        if (roleType == Role.School_Specialist)
                            condition = PredicateBuilder.And(condition,
                                o =>
                                    o.UserInfo.Role == Role.School_Specialist &&
                                    o.UserInfo.UserCommunitySchools.Any(p => primarySchoolIds.Contains(p.SchoolId)));
                        else
                            condition = o => false;
                    }
                    else
                        condition = o => false;
                    break;
                default:
                    if (roleType == Role.School_Specialist)
                        condition = PredicateBuilder.And(condition, o => o.UserInfo.Role == Role.School_Specialist);
                    else
                        condition = PredicateBuilder.And(condition, o => o.UserInfo.Role == Role.School_Specialist_Delegate);
                    break;
            }
            return condition;
        }

        /// <summary>
        /// 查找principal
        /// </summary>
        /// <param name="userInfo"></param>
        /// <returns></returns>
        private Expression<Func<PrincipalEntity, bool>> GetPrincipalCondition(UserBaseEntity userInfo, Role roleType)
        {
            Expression<Func<PrincipalEntity, bool>> condition = o => true;
            if (userInfo == null)
                return condition;
            //UserBaseEntity baseUser = GetUser(userInfo.ID);
            List<int> comIds = userInfo.UserCommunitySchools.Where(o => o.CommunityId > 0).Select(o => o.CommunityId).ToList();
            switch (userInfo.Role)
            {
                case Role.Content_personnel:
                case Role.Statisticians:
                case Role.Administrative_personnel:
                case Role.Intervention_manager:
                case Role.Video_coding_analyst:
                case Role.Intervention_support_personnel:
                case Role.Coordinator:
                case Role.Mentor_coach:
                    if (roleType == Role.Principal)
                    {
                        condition = PredicateBuilder.And(condition,
                          o => o.UserInfo.UserCommunitySchools.Where(relation => relation.SchoolId > 0)
                              .Any(q => (q.School.CommunitySchoolRelations.Any(s => comIds.Contains(s.CommunityId)))));
                    }
                    else
                    {
                        condition = o => true;
                    }
                    break;
                    break;

                case Role.Teacher:
                case Role.Parent:
                    condition = o => false;
                    break;

                case Role.Principal://可查看principal delegate
                    if (userInfo.Principal != null)
                        if (roleType == Role.Principal)
                            condition = PredicateBuilder.And(condition, o => o.UserInfo.Role == Role.Principal
                                                                             &&
                                                                             o.UserInfo.UserCommunitySchools.Any(
                                                                                 p => p.UserId == userInfo.ID));
                        else if (roleType == Role.Principal_Delegate)
                            condition = PredicateBuilder.And(condition, o => o.UserInfo.Role == Role.Principal_Delegate
                               && o.ParentId == userInfo.ID);
                        else
                            condition = o => false;
                    else
                        condition = o => false;
                    break;

                case Role.Principal_Delegate:
                    if (roleType == Role.Principal)
                        condition = PredicateBuilder.And(condition, o => o.UserInfo.Role == Role.Principal
                       && o.UserInfo.ID == userInfo.Principal.ParentId);
                    else if (roleType == Role.Principal_Delegate)
                        condition = PredicateBuilder.And(condition, o => o.UserInfo.Role == Role.Principal_Delegate
                           && o.UserInfo.ID == userInfo.ID);
                    else
                        condition = o => false;
                    break;

                case Role.TRS_Specialist:
                case Role.School_Specialist:
                    if (userInfo.Principal != null)
                    {
                        condition = PredicateBuilder.And(condition,
                            o =>
                                o.UserInfo.Role == Role.Principal &&
                                o.UserInfo.UserCommunitySchools.Any(p => p.School.UserCommunitySchools.Any(q => q.UserId == userInfo.ID)));
                    }
                    else
                        condition = o => false;
                    break;
                case Role.TRS_Specialist_Delegate:
                case Role.School_Specialist_Delegate:
                    if (userInfo.Principal != null)
                    {
                        condition = PredicateBuilder.And(condition,
                            o =>
                                o.UserInfo.Role == Role.Principal &&
                                o.UserInfo.UserCommunitySchools.Any(
                                    p => p.School.UserCommunitySchools.Any(q => q.UserId == userInfo.Principal.ParentId)));
                    }
                    else
                        condition = o => false;
                    break;

                case Role.Statewide://可查看principal
                case Role.Community:
                case Role.District_Community_Specialist:
                    if (userInfo.CommunityUser != null || userInfo.StateWide != null)
                    {
                        if (roleType == Role.Principal)
                            condition = PredicateBuilder.And(condition,
                                o =>
                                    o.UserInfo.Role == Role.Principal &&
                                    o.UserInfo.UserCommunitySchools.Any(
                                        e =>
                                            e.School.CommunitySchoolRelations.Any(
                                                m => m.Community.UserCommunitySchools.Any(n => n.UserId == userInfo.ID))));
                        else
                            condition = o => false;
                    }
                    else
                        condition = o => false;
                    break;
                case Role.District_Community_Delegate:
                case Role.Community_Specialist_Delegate:
                    if (userInfo.CommunityUser != null || userInfo.StateWide != null)
                    {
                        if (roleType == Role.Principal)
                            condition = PredicateBuilder.And(condition,
                                o =>
                                    o.UserInfo.Role == Role.Principal &&
                                    o.UserInfo.UserCommunitySchools.Any(
                                        e =>
                                            e.School.CommunitySchoolRelations.Any(
                                                m =>
                                                    m.Community.UserCommunitySchools.Any(
                                                        n => n.UserId == userInfo.CommunityUser.ParentId))));
                        else
                            condition = o => false;
                    }
                    else
                        condition = o => false;
                    break;
                default:
                    if (roleType == Role.Principal)
                        condition = PredicateBuilder.And(condition, o => o.UserInfo.Role == Role.Principal);
                    else
                        condition = PredicateBuilder.And(condition, o => o.UserInfo.Role == Role.Principal_Delegate);
                    break;
            }
            return condition;
        }

        private Expression<Func<UserBaseEntity, bool>> GetDelegateCondition(UserBaseEntity userInfo)
        {
            Expression<Func<UserBaseEntity, bool>> condition = o => true;
            if (userInfo == null)
                return condition;
            switch (userInfo.Role)
            {
                case Role.Content_personnel:
                case Role.Statisticians:
                case Role.Administrative_personnel:
                case Role.Intervention_manager:
                case Role.Video_coding_analyst:
                case Role.Intervention_support_personnel:
                case Role.Coordinator:
                case Role.Mentor_coach:
                    var communityIds = userInfo.UserCommunitySchools.Select(e => e.CommunityId).ToList();
                    var schoolIds = schoolBusiness.GetSchoolIds(communityIds);
                    var userIdsCommunity =
                        userService.BaseUsers.Where(
                            e =>
                                communityIds.Any(f => e.UserCommunitySchools.Any(g => g.CommunityId == f)) &&
                                (e.Role == Role.Community || e.Role == Role.District_Community_Specialist)).Select(e => e.ID).ToList();
                    var delegateUserIdsCommunity =
                        userService.BaseUsers.Where(
                            o =>
                                (o.Role == Role.District_Community_Delegate ||
                                 o.Role == Role.Community_Specialist_Delegate) &&
                                userIdsCommunity.Contains(o.CommunityUser.ParentId)).Select(e => e.ID).ToList();

                    var userIdsPrincipal =
                        userService.BaseUsers.Where(
                            e =>
                                e.UserCommunitySchools.Any(f => schoolIds.Contains(f.SchoolId)) &&
                                (e.Role == Role.Principal || e.Role == Role.School_Specialist ||
                                 e.Role == Role.TRS_Specialist)).Select(e => e.ID).ToList();
                    var delegateUserIdsPrincipal =
                        userService.BaseUsers.Where(
                            o =>
                                (o.Role == Role.Principal_Delegate || o.Role == Role.School_Specialist_Delegate ||
                                 o.Role == Role.TRS_Specialist_Delegate) &&
                                userIdsPrincipal.Contains(o.Principal.ParentId)).Select(e => e.ID).ToList();
                    condition = PredicateBuilder.And(condition,
                        o => delegateUserIdsCommunity.Contains(o.ID) || delegateUserIdsPrincipal.Contains(o.ID));
                    break;
                default:
                    condition = o => true;
                    break;
            }
            return condition;
        }

        private Expression<Func<UserBaseEntity, bool>> GetInternalUserCondition(UserBaseEntity userInfo)
        {
            Expression<Func<UserBaseEntity, bool>> condition = o => true;
            if (userInfo == null)
                return condition;
            UserBaseEntity baseUser = GetUser(userInfo.ID);
            List<int> comIds = userInfo.UserCommunitySchools.Where(o => o.CommunityId > 0).Select(o => o.CommunityId).ToList();
            switch (userInfo.Role)
            {
                case Role.Super_admin:
                    condition = o => true;
                    break;
                case Role.Statisticians: //可以看到 Video Coding Analyst CLI
                    condition = PredicateBuilder.And(condition, o => o.Role == Role.Video_coding_analyst && o.UserCommunitySchools.Any(r => comIds.Contains(r.CommunityId)));
                    condition = PredicateBuilder.Or(condition, o => o.ID == userInfo.ID);
                    break;

                case Role.Administrative_personnel: //可以看到 Intervention Support Personnel CLI
                    condition = PredicateBuilder.And(condition, o => o.Role == Role.Intervention_support_personnel && o.UserCommunitySchools.Any(r => comIds.Contains(r.CommunityId)));
                    condition = PredicateBuilder.Or(condition, o => o.ID == userInfo.ID);
                    break;

                case Role.Intervention_manager: //可以看到 Intervention Support Personnel CLI、Coordinator、Mentor/Coach
                    condition = o => false;
                    condition = PredicateBuilder.Or(condition, o => o.Role == Role.Intervention_support_personnel && o.UserCommunitySchools.Any(r => comIds.Contains(r.CommunityId)));
                    condition = PredicateBuilder.Or(condition, o => o.Role == Role.Coordinator && o.UserCommunitySchools.Any(r => comIds.Contains(r.CommunityId)));
                    condition = PredicateBuilder.Or(condition, o => o.Role == Role.Mentor_coach && o.UserCommunitySchools.Any(r => comIds.Contains(r.CommunityId)));
                    condition = PredicateBuilder.Or(condition, o => o.ID == userInfo.ID);
                    break;

                case Role.Coordinator: //可以看到Mentor/Coach
                    condition = PredicateBuilder.And(condition, o => o.Role == Role.Mentor_coach && o.UserCommunitySchools.Any(r => comIds.Contains(r.CommunityId)));
                    condition = PredicateBuilder.Or(condition, o => o.ID == userInfo.ID);
                    break;

                case Role.Content_personnel://不能看到任何用户
                case Role.Video_coding_analyst:
                case Role.Intervention_support_personnel:
                case Role.Mentor_coach:
                    condition = o => false;
                    condition = PredicateBuilder.Or(condition, o => o.ID == userInfo.ID);
                    break;
                default:
                    condition = o => false;
                    break;
            }

            return condition;
        }

        public TeacherEntity InitByRole(TeacherEntity teacher, TeacherRoleEntity role)
        {
            TeacherEntity oldEntity = new TeacherEntity();

            if (teacher.ID > 0)
                oldEntity = userService.GetTeacher(teacher.ID);

            Type r = role.GetType();
            Type c = teacher.GetType();
            Type o = oldEntity.GetType();
            foreach (PropertyInfo pi in r.GetProperties())
            {
                string name = pi.Name;
                string value1 = pi.GetValue(role, null).ToString();
                if (value1 != "RW")
                {
                    if (o.GetProperty(name) != null)
                    {
                        object oldValue = o.GetProperty(name).GetValue(oldEntity);
                        c.GetProperty(name).SetValue(teacher, oldValue);
                    }
                }
            }
            return teacher;
        }
        #endregion

        public Dictionary<int, List<CECTeacherCommunityAndSchoolNameModel>> GetTeacherCommunityName(params int[] userIds)
        {
            var query = userService.UserComSchRelations.Where(x => userIds.Contains(x.UserId)).GroupBy(r => r.UserId, r => new CECTeacherCommunityAndSchoolNameModel()
            {
                UserId = r.UserId,
                CommunityName = r.Community.Name
            });
            return query.ToDictionary(x => x.Key, x => x.ToList());
        }

        public Dictionary<int, List<CECTeacherCommunityAndSchoolNameModel>> GetTeacherSchoolName(params int[] userIds)
        {
            var query = userService.UserComSchRelations.Where(x => userIds.Contains(x.UserId) && x.SchoolId > 0).GroupBy(r => r.UserId, r => new CECTeacherCommunityAndSchoolNameModel()
            {
                UserId = r.UserId,
                SchoolName = r.School.Name
            });
            return query.ToDictionary(x => x.Key, x => x.ToList());
        }

        private readonly Expression<Func<UserBaseEntity, UserModel>> userEntityToModel = e => new UserModel()
        {
            ID = e.ID,
            UserId = e.ID,
            CommunityNames = e.UserCommunitySchools.Select(x => x.Community.Name).Distinct(),
            FirstName = e.FirstName,
            LastName = e.LastName,
            TeacherNumber = e.Role == Role.Teacher ? e.InternalID : "",
            SchoolNames = e.UserCommunitySchools.Where(s => s.SchoolId > 0).Select(o => o.School.Name).Distinct(),
            TeacherType = e.Role == Role.Teacher ? e.TeacherInfo.TeacherType : 0,
            Status = e.Status,
            GoogleId = e.GoogleId,
            Gmail = e.Gmail
        };

        public UserModel GetUserModel(int userId)
        {
            var query = userService.BaseUsers.Where(u => userId == u.ID).Select(userEntityToModel);
            return query.FirstOrDefault();
        }

        public UserModel GetUserModelByTeacherId(int teacherId)
        {
            var query = userService.Teachers.Where(u => teacherId == u.ID).Select(e => new UserModel()
            {
                ID = e.ID,
                UserId = e.ID,
                CommunityNames = e.UserInfo.UserCommunitySchools.Select(x => x.Community.Name),
                FirstName = e.UserInfo.FirstName,
                LastName = e.UserInfo.LastName,
                TeacherNumber = e.UserInfo.InternalID,
                SchoolNames = e.UserInfo.UserCommunitySchools.Where(s => s.SchoolId > 0).Select(o => o.School.Name),
                TeacherType = e.TeacherType,
                Status = e.UserInfo.Status,
                GoogleId = e.UserInfo.GoogleId,
                Gmail = e.UserInfo.Gmail
            });
            return query.FirstOrDefault();
        }

        public Dictionary<int, UserModel> GetUserModels(params int[] userIds)
        {
            var query = userService.BaseUsers.Where(u => userIds.Contains(u.ID)).Select(userEntityToModel).GroupBy(x => x.ID);
            return query.ToDictionary(x => x.Key, x => x.First());
        }

        #region Bes

        public OperationResult UpdatePrincipal(PrincipalEntity principal)
        {
            OperationResult result = new OperationResult(OperationResultType.Success);
            userService.UpdateUser(principal.UserInfo, false);
            result = userService.UpdatePrincipal(principal);
            return result;
        }
        public List<TeacherUserModel> SearchTeachersForBES(UserBaseEntity user, Expression<Func<TeacherEntity, bool>> condition,
           string sort, string order, int first, int count, out int total)
        {
            var query = userService.Teachers.AsExpandable().Where(condition).Where(GetTeacherCondition(user));

            total = query.Count();
            var list = query.Select(e => new TeacherUserModel()
            {
                ID = e.ID,
                CommunityNames = e.UserInfo.UserCommunitySchools.Select(x => x.Community.Name),
                SchoolNames = e.UserInfo.UserCommunitySchools.Select(o => o.School.Name),
                UserId = e.UserInfo.ID,
                TeacherId = e.TeacherId,
                SchoolYear = e.SchoolYear,

                FirstName = e.UserInfo.FirstName,
                MiddleName = e.UserInfo.MiddleName,
                LastName = e.UserInfo.LastName,
                PreviousLastName = e.UserInfo.PreviousLastName,
                BirthDate = e.BirthDate,
                Gender = e.Gender,
                Ethnicity = e.Ethnicity,
                VendorCode = e.VendorCode,
                PrimaryLanguageId = e.PrimaryLanguageId,
                SecondLanguageId = e.SecondaryLanguageId,
                EmployedBy = e.EmployedBy,
                CLIFundingID = e.CLIFundingID,
                MediaRelease = e.MediaRelease,
                Status = e.UserInfo.Status,
                TeacherNumber = e.UserInfo.InternalID,
                HomeMailingAddress = e.HomeMailingAddress,
                PrimaryPhoneNumber = e.UserInfo.PrimaryPhoneNumber,
                PrimaryNumberType = e.UserInfo.PrimaryNumberType,
                SecondPhoneNumber = e.UserInfo.SecondaryPhoneNumber,
                SecondNumberType = e.UserInfo.SecondaryNumberType,
                FaxNumber = e.UserInfo.FaxNumber,
                PrimaryEmail = e.UserInfo.PrimaryEmailAddress,
                SecondEmail = e.UserInfo.SecondaryEmailAddress

            }).OrderBy(sort, order).AsQueryable().Skip(first).Take(count);
            return list.ToList();
        }

        #endregion


        #region BUP

        public List<UserBaseEntity> GetUsers(List<int> userIds)
        {
            return userService.BaseUsers.Where(r => userIds.Contains(r.ID)).ToList();
        }

        public TeacherEntity GetTeacher(string firstName, string lastName, string engageId)
        {
            return userService.Teachers.FirstOrDefault(
                e => e.UserInfo.FirstName == firstName && e.UserInfo.LastName == lastName && e.TeacherId == engageId);
        }

        #endregion

        #region Ade
        public List<SelectItemModel> GetUserSearchModels(Expression<Func<UserBaseEntity, bool>> expression)
        {
            return userService.BaseUsers.AsExpandable().Where(expression)
                .Select(r => new SelectItemModel
                {
                    ID = r.ID,
                    Name = r.FirstName + " " + r.LastName
                }).ToList();
        }
        #endregion

        public List<SelectItemModel> GetTeacherBySchoolId(int schoolId)
        {
            return userService.UserComSchRelations.Where(
                x => x.SchoolId == schoolId
                    && x.SchoolId > 0
                     && x.User.Role == Role.Teacher
                     && x.User.IsDeleted == false
                     && x.User.Status == EntityStatus.Active)
                .Select(x => new SelectItemModel()
                {
                    ID = x.UserId,
                    Name = x.User.FirstName + " " + x.User.LastName
                }).DistinctBy(x => x.ID).OrderBy(x => x.Name).ToList();
        }
    }
}
