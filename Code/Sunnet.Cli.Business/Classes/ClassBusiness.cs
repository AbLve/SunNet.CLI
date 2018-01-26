using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
/**************************************************************************
 * Developer: 		Leason
 * Computer:		Leason-PC
 * Domain:			Leason-pc
 * CreatedOn:		2014/8/22 17:20:20
 * Description:		Create ClassBusiness
 * Version History:	Created,2014/8/22 17:20:20
 * 
 * 
 **************************************************************************/
using Sunnet.Cli.Business.Classes.Models;
using Sunnet.Cli.Business.Common;
using Sunnet.Cli.Business.Common.Enum;
using Sunnet.Cli.Business.Communities;
using Sunnet.Cli.Business.Schools;
using Sunnet.Cli.Business.Students.Models;
using Sunnet.Cli.Business.Users.Models;
using Sunnet.Cli.Core;
using Sunnet.Cli.Core.Classes;
using Sunnet.Cli.Core.Classes.Entites;
using Sunnet.Cli.Core.Common.Enums;
using Sunnet.Cli.Core.Communities.Entities;
using Sunnet.Cli.Core.MasterData;
using Sunnet.Cli.Core.MasterData.Entities;
using Sunnet.Cli.Core.Users;
using Sunnet.Cli.Core.Users.Entities;
using Sunnet.Cli.Core.Users.Enums;
using Sunnet.Framework.Core.Tool;
using Sunnet.Framework.Core.Extensions;
using Sunnet.Framework.Extensions;
using LinqKit;
using Sunnet.Cli.Business.Cpalls.Models;
using Sunnet.Cli.Core.Students.Entities;
using Sunnet.Cli.Business.Users;
using Sunnet.Cli.Business.Trs.Models;
using Sunnet.Cli.Core.Classrooms.Entites;
using Sunnet.Cli.Business.Classrooms;
using Sunnet.Cli.Core.Schools.Entities;
using Sunnet.Cli.Business.Students;
using Sunnet.Framework.Helpers;
using Sunnet.Cli.Business.BUP.Model;
using Sunnet.Cli.Core.Ade;

namespace Sunnet.Cli.Business.Classes
{
    public class ClassBusiness
    {

        #region Private Field
        private readonly IClassContract _contract;
        private readonly IMasterDataContract _masterDataContract;
        private readonly IUserContract _userContract;
        private readonly SchoolBusiness _schoolBusiness;
        private readonly ClassroomBusiness _classroomBussiness;
        private readonly CommunityBusiness _communityBuss;
        private StudentBusiness _studentBusiness
        {
            get { return new StudentBusiness(_unit); }
        }

        private EFUnitOfWorkContext _unit
        { get; set; }

        #endregion

        #region Public Contruction
        public ClassBusiness(EFUnitOfWorkContext unit = null)
        {
            _unit = unit;
            _contract = DomainFacade.CreateClassService(unit);
            _masterDataContract = DomainFacade.CreateMasterDataServer(unit);
            _userContract = DomainFacade.CreateUserService(unit);
            _schoolBusiness = new SchoolBusiness(unit);
            _classroomBussiness = new ClassroomBusiness(unit);
            _communityBuss = new CommunityBusiness(unit);
        }
        #endregion

        #region Class New/Insert/Update/Get
        public ClassEntityModel NewClassModel()
        {
            ClassEntityModel entity = new ClassEntityModel();
            entity.StatusDate = DateTime.Now;
            entity.SchoolYear = CommonAgent.SchoolYear;
            entity.CurriculumOther = string.Empty;
            entity.SupplementalCurriculumOther = string.Empty;
            entity.MonitoringToolOther = string.Empty;
            entity.EquipmentNumber = string.Empty;
            entity.Notes = string.Empty;
            return entity;
        }
        public ClassEntity NewClassEntity()
        {
            ClassEntity entity = new ClassEntity();
            entity.StatusDate = DateTime.Now;
            entity.SchoolYear = CommonAgent.SchoolYear;
            entity.CurriculumOther = string.Empty;
            entity.SupplementalCurriculumOther = string.Empty;
            entity.MonitoringToolOther = string.Empty;
            entity.EquipmentNumber = string.Empty;
            entity.Notes = string.Empty;
            return entity;
        }
        //private OperationResult AddUserClassRelation(int schoolId, int classId, UserBaseEntity user)
        //{
        //    //User Community School Relations
        //    var userBusiness = new UserBusiness();
        //    OperationResult res = new OperationResult(OperationResultType.Success);
        //    int total;
        //    List<int> ListcommunityIds = new List<int>();
        //    Expression<Func<UserBaseEntity, bool>> condition = o => true;
        //    SchoolEntity school = _schoolBusiness.GetSchool(schoolId);
        //    if (school != null)
        //    {
        //        ListcommunityIds = _communityBuss.GetCommunityListByBasicId(school.BasicSchool.CommunityId).Select(o => o.ID).ToList();

        //    }
        //    condition = PredicateHelper.And(condition, (r => r.UserCommunitySchools.Any(c => ListcommunityIds.Contains(c.CommunityId))));
        //    condition = PredicateHelper.And(condition, (r => (r.Role == Role.Community || r.Role == Role.District_Community_Specialist || r.Role == Role.Statewide)));
        //    condition = PredicateHelper.And(condition, (r => !(r.UserClasses.Any(c => c.ClassId == classId))));

        //    List<UserBaseModel> userList = userBusiness.GetUsers(user, condition, "UserId", "ASC", 0, int.MaxValue, out total);
        //    var userIdList = userList.Select(o => o.UserId).Distinct().ToList();
        //    if (userIdList != null && userIdList.Count > 0)
        //    {
        //        res = userBusiness.InsertUserClassRelationsMoreUser(userIdList.ToArray(), user.ID, classId);
        //    }
        //    return res;
        //}
        /// <summary>
        /// 向Class里添加数据
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="languageSelectList"></param>
        /// <param name="dic"></param>
        /// <returns></returns>
        public OperationResult InsertClass(ClassEntity entity, int[] languageSelectList, Role role
            , int classroomId, UserBaseEntity userInfo, string code = "")
        {
            if (languageSelectList != null && languageSelectList.Any())
            {
                entity.Languages = new Collection<LanguageEntity>();
                if (languageSelectList != null)
                {
                    foreach (var id in languageSelectList)
                    {
                        if (id > 0)
                        {
                            var language = _masterDataContract.GetLanguage(id);
                            entity.Languages.Add(language);
                        }
                    }
                }

            }


            entity.Previous_Teacher_TEA_ID = role == Role.Teacher ? code : string.Empty;
            entity.SchoolYear = CommonAgent.SchoolYear;
            entity.StatusDate = DateTime.Now;
            ClassRoleEntity classRole = GetClassRole(role);
            entity = InitByRole(entity, classRole);

            if (classroomId > 0)
            {
                ClassroomEntity classroomEntity = _classroomBussiness.GetClassroom(classroomId);
                if (classroomEntity != null && classroomEntity.SchoolId == entity.SchoolId)
                {
                    entity.ClassroomClasses = new List<ClassroomClassEntity>();
                    entity.ClassroomClasses.Add(new ClassroomClassEntity()
                    {
                        Class = entity,
                        Classroom = classroomEntity,
                        CreatedBy = userInfo.ID,
                        UpdatedBy = userInfo.ID,
                        Status = EntityStatus.Active
                    });
                }
            }

            if (entity.ClassInternalID == null) entity.ClassInternalID = string.Empty;

            int userId = 0;
            switch (userInfo.Role)
            {
                case Role.Teacher:
                    entity.Teachers = new List<TeacherEntity>();
                    entity.Teachers.Add(userInfo.TeacherInfo);
                    break;
                case Role.District_Community_Specialist:
                case Role.District_Community_Delegate:
                    userId = userInfo.ID;
                    if (userInfo.Role == Role.District_Community_Delegate)
                        userId = userInfo.CommunityUser.ParentId;
                    entity.UserClasses = new List<UserClassRelationEntity>();
                    entity.UserClasses.Add(new UserClassRelationEntity()
                    {
                        UserId = userId,
                        CreatedBy = userInfo.ID,
                        CreatedOn = DateTime.Now,
                        UpdatedBy = userInfo.ID,
                        UpdatedOn = DateTime.Now,
                        Status = EntityStatus.Active
                    });
                    break;
                case Role.School_Specialist:
                case Role.School_Specialist_Delegate:
                    userId = userInfo.ID;
                    if (userInfo.Role == Role.School_Specialist_Delegate)
                        userId = userInfo.CommunityUser.ParentId;
                    if (_contract.Classes.Count(r => r.UserClasses.Any(c => c.Class.SchoolId == entity.SchoolId) && r.IsDeleted == false) > 0)
                    {
                        entity.UserClasses = new List<UserClassRelationEntity>();
                        entity.UserClasses.Add(new UserClassRelationEntity()
                        {
                            UserId = userId,
                            CreatedBy = userInfo.ID,
                            CreatedOn = DateTime.Now,
                            UpdatedBy = userInfo.ID,
                            UpdatedOn = DateTime.Now,
                            Status = EntityStatus.Active
                        });
                    }
                    break;
            }

            OperationResult result = _contract.InsertClass(entity);
            return result;
        }

        public OperationResult UpdateClass(ClassEntity entity)
        {
            return _contract.UpdateClass(entity);
        }

        public OperationResult UpdateClass(ClassEntity entity, int[] languageSelectList, Role role)
        {
            ClassEntity classEntity = _contract.GetClass(entity.ID);
            classEntity.UpdatedOn = DateTime.Now;
            if (classEntity.Languages != null)
            {
                while (classEntity.Languages.Any())
                    classEntity.Languages.Remove(classEntity.Languages.First());
            }

            if (languageSelectList != null && languageSelectList.Any())
            {
                foreach (var id in languageSelectList)
                {
                    if (id > 0)
                    {
                        var language = _masterDataContract.GetLanguage(id);
                        classEntity.Languages.Add(language);
                    }
                }
            }


            ClassRoleEntity classRole = GetClassRole(role);
            entity = InitByRole(entity, classRole);
            _contract.UpdateClass(classEntity, false);

            entity.Previous_Teacher_TEA_ID = entity.Previous_Teacher_TEA_ID ?? string.Empty;

            OperationResult result = _contract.UpdateClass(entity);
            if (result.ResultType == OperationResultType.Success)
            {
                //status is inactive
                if (entity.Status == EntityStatus.Inactive)
                    (new CommunityBusiness()).InactiveEnity(ModelName.Class, entity.ID, EntityStatus.Inactive, CommonAgent.SchoolYear);
            }
            return result;
        }

        public ClassEntity GetClass(int id)
        {
            var entity = _contract.GetClass(id);
            return entity;
        }

        /// <summary>
        /// True 表示是 demo
        /// </summary>
        public bool CheckSchoolTypeIsDemo(int classId)
        {
            return _contract.Classes.Where(r => r.ID == classId && r.School.SchoolType.Name.Contains("demo")).Count() > 0;
        }

        public OperationResult UpdateClassPlayground(int playgroundId, int[] classIds = null)
        {
            return _contract.UpdateClassPlayground(playgroundId, classIds);
        }

        public List<ClassEntity> GetClassList(Expression<Func<ClassEntity, bool>> condition)
        {
            return _contract.Classes.AsExpandable().Where(condition).ToList();
        }

        public bool IsClassExist(string name, int id = 0, int schoolId = 0)
        {
            return _contract.Classes.Any(c => c.Name == name && c.ID != id && c.SchoolId == schoolId);
        }
        #endregion

        #region MonitoringTool
        public MonitoringToolEntity GetMonitoringTool(int id)
        {
            return _contract.GetMonitoringTool(id);
        }

        /// <summary>
        /// 获取MonitoringTool下拉框的值
        /// </summary>
        /// <returns>MonitoringTool下拉框的值</returns>
        public List<SelectItemModel> GetMonitoringToolSelectList(bool isActive = true)
        {
            IQueryable<MonitoringToolEntity> query;

            if (isActive)
                query = _contract.MonitoringTools.Where(r => r.Status == EntityStatus.Active);
            else
                query = _contract.MonitoringTools;

            return query.Select(o => new SelectItemModel()
            {
                ID = o.ID,
                Name = o.Name
            }).ToList();
        }

        public IEnumerable<SelectItemModelOther> GetMonitoringToolSelectListOther()
        {
            return _contract.MonitoringTools.Select(o => new SelectItemModelOther()
            {
                ID = o.ID,
                Name = o.Name,
                Status = o.Status
            });
        }

        /// <summary>
        /// 获取MonitoringTool下拉框的值
        /// </summary>
        /// <param name="expression">针对MonitoringTool的Lambda</param>
        /// <returns>MonitoringTool下拉框的值</returns>
        public List<SelectItemModel> GetMonitoringToolSelectList(Expression<Func<MonitoringToolEntity, bool>> expression)
        {
            return _contract.MonitoringTools.Where(expression).Select(o => new SelectItemModel()
            {
                ID = o.ID,
                Name = o.Name
            }).ToList();
        }

        public OperationResult InsertMonitoringTool(MonitoringToolEntity entity)
        {
            return _contract.InsertMonitoringTool(entity);
        }

        public OperationResult UpdateMonitoringTool(MonitoringToolEntity entity)
        {
            return _contract.UpdateMonitoringTool(entity);
        }

        #endregion

        #region GetSelectList
        /// <summary>
        /// 根据Class的主键，获取ClassModel
        /// </summary>
        /// <param name="classId"></param>
        /// <returns></returns>
        public IEnumerable<AssigenStudentClassModel> GetClassList(UserBaseEntity userInfoint, int classId = 0)
        {
            if (classId == 0)
                return _contract.Classes.Where(o => o.Status == EntityStatus.Active).Select(o => new AssigenStudentClassModel()
                {
                    ClassId = classId,
                    ClassName = o.Name,
                    DayType = o.DayType,
                    //TODO:Classroom and Class has already many to many
                    //ClassroomName = o.Classroom.Name
                });
            else
                return _contract.Classes.Where(o => o.ID == classId && o.Status == EntityStatus.Active).Select(o => new AssigenStudentClassModel()
                {
                    ClassId = classId,
                    ClassName = o.Name,
                    DayType = o.DayType,
                    //TODO:Classroom and Class has already many to many
                    //ClassroomName = o.Classroom.Name
                });
        }

        public List<SelectItemModel> GetClassId(Expression<Func<ClassEntity, bool>> condition)
        {
            return _contract.Classes.AsExpandable().Where(condition).Select(o => new SelectItemModel()
            {
                ID = o.ID,
                Name = o.ClassId
            }).ToList();
        }

        public List<SelectItemModel> GetClassSelectList(Expression<Func<ClassEntity, bool>> condition, UserBaseEntity user)
        {
            return _contract.Classes.AsExpandable().Where(condition).Where(GetRoleCondition(user)).Select(o => new SelectItemModel()
            {
                ID = o.ID,
                Name = o.Name
            }).ToList();
        }

        public List<SelectItemModel> GetClassSelectList(UserBaseEntity user, Expression<Func<ClassEntity, bool>> condition)
        {
            return
                _contract.Classes.AsExpandable()
                    .Where(condition)
                    .Where(GetRoleCondition(user))
                    .Select(o => new SelectItemModel()
                    {
                        ID = o.ID,
                        Name = o.Name,
                        Props = new
                        {
                            classDayType = o.DayType,
                            leadTeacherId = o.LeadTeacherId
                        }
                    }).OrderBy(o => o.Name.Trim()).ToList();
        }

        public List<SelectItemModel> GetClassSelectListForCache(UserBaseEntity user, int schoolId)
        {
            List<SelectItemModel> classList = new List<SelectItemModel>();
            if (schoolId < 1)
                return classList;
            var key = "CpallsClass_schoolId_" + schoolId;
            var allClasses = CacheHelper.Get<List<CpallsClassModel>>(key);
            if (allClasses == null)
            {
                classList = GetClassSelectList(user,
                     r => r.SchoolId == schoolId && r.Status == EntityStatus.Active &&
                          r.SchoolYear == CommonAgent.SchoolYear);
            }
            else
            {
                var list = allClasses.ToList();
                if (user.Role != Role.Super_admin)
                {
                    var userClasslIds = CacheHelper.Get<List<int>>("UserClasslIds" + user.ID);
                    if (userClasslIds == null)
                    {
                        userClasslIds = _contract.Classes.AsExpandable().Where(GetRoleCondition(user)).Select(o => o.ID).ToList();
                    }
                    classList = list.Where(c => userClasslIds.Contains(c.ID)).Select(c => new SelectItemModel()
                    {
                        ID = c.ID,
                        Name = c.Name
                    }).ToList();
                }
                else
                {
                    classList = list.Select(c => new SelectItemModel()
                    {
                        ID = c.ID,
                        Name = c.Name
                    }).ToList();
                }
            }
            return classList;
        }


        public List<SelectItemModel> GetClassSelectListByCreatedDESC(UserBaseEntity user, Expression<Func<ClassEntity, bool>> condition)
        {
            return
                _contract.Classes.AsExpandable()
                    .Where(condition)
                    .Where(GetRoleCondition(user)).OrderByDescending(o => o.CreatedOn)
                    .Select(o => new SelectItemModel()
                    {
                        ID = o.ID,
                        Name = o.Name,
                        Props = new
                        {
                            classDayType = o.DayType,
                            leadTeacherId = o.LeadTeacherId
                        }
                    }).ToList();
        }

        public List<int> GetClassesIds(Expression<Func<ClassEntity, bool>> condition, UserBaseEntity user
            , string sort, string order, int first, int count, out int total)
        {
            var query = _contract.Classes.AsExpandable().Where(condition).Where(GetRoleCondition(user)).OrderBy(sort, order).Select(r => r.ID);
            total = query.Count();
            return query.Skip(first).Take(count).ToList();
        }

        public List<int> GetClassIdsForReport(int comId, int assessmentId)
        {
            var schoolYear = CommonAgent.SchoolYear;
            var query = _contract.Classes.AsExpandable().Where(c => c.School.CommunitySchoolRelations.Any(r => r.CommunityId == comId) && c.SchoolYear == schoolYear);
            //var classlevlIds = _communityBuss.GetAssignedClassLevelsByComId(assessmentId, comId);
            //if (!classlevlIds.Contains(0))//All and missing
            //{
            //    query = query.Where(c => classlevlIds.Contains(c.Classlevel));
            //}
            return query.Select(c => c.ID).ToList();
        }
        public List<int> GetClassIdsForReport(UserBaseEntity user, int assessmentId)
        {
            var schoolYear = CommonAgent.SchoolYear;
            var query = _contract.Classes.AsExpandable().Where(c => c.SchoolYear == schoolYear);
            //var classlevlIds = _communityBuss.GetAssignedClassLevels(assessmentId, user);
            //if (!classlevlIds.Contains(0))//All and missing
            //{
            //    query = query.Where(c => classlevlIds.Contains(c.Classlevel));
            //}
            if (user != null)
                query = query.Where(GetRoleCondition(user));
            return query.Select(c => c.ID).ToList();
        }

        public List<int> GetClassIdsForReport(int schoolId, UserBaseEntity user, int assessmentId)
        {
            var schoolYear = CommonAgent.SchoolYear;
            var query = _contract.Classes.AsExpandable().Where(c => c.SchoolId == schoolId && c.SchoolYear == schoolYear);
            //var classlevlIds = _communityBuss.GetAssignedClassLevels(assessmentId, schoolId);
            //if (!classlevlIds.Contains(0))//All and missing
            //{
            //    query = query.Where(c => classlevlIds.Contains(c.Classlevel));
            //}
            if (user != null)
                query = query.Where(GetRoleCondition(user));
            return query.Select(c => c.ID).ToList();
        }
        public List<int> GetClassIdsForReport(IList<int> schoolIds, UserBaseEntity user, int assessmentId)
        {
            var schoolYear = CommonAgent.SchoolYear;
            var query = _contract.Classes.AsExpandable().Where(c => schoolIds.Contains(c.SchoolId) && c.SchoolYear == schoolYear);
            //var classlevlIds = _communityBuss.GetAssignedClassLevels(assessmentId, schoolIds);
            //if (!classlevlIds.Contains(0))//All and missing
            //{
            //    query = query.Where(c => classlevlIds.Contains(c.Classlevel));
            //}
            if (user != null)
                query = query.Where(GetRoleCondition(user));
            return query.Select(c => c.ID).ToList();
        }
        public List<int> GetClassIdsForCircleReport(int communityId, int schoolId, UserBaseEntity user, int assessmentId)
        {
            var schoolYear = CommonAgent.SchoolYear;
            var query = _contract.Classes.AsExpandable().Where(c => c.School.CommunitySchoolRelations.Any(r => r.CommunityId == communityId) && c.SchoolYear == schoolYear);
            //var classlevlIds = _communityBuss.GetAssignedClassLevels(assessmentId, schoolId);
            //if (!classlevlIds.Contains(0))//All and missing
            //{
            //    query = query.Where(c => classlevlIds.Contains(c.Classlevel));
            //}
            if (schoolId > 0)
            {
                query = query.Where(c => c.SchoolId == schoolId);
            }
            query = query.Where(GetRoleCondition(user));
            return query.Select(c => c.ID).ToList();
        }
        public List<ClassEntity> GetClassesForCircleReport(int communityId, int schoolId, UserBaseEntity user, int assessmentId)
        {
            var query = _contract.Classes.AsExpandable().Where(c => c.School.CommunitySchoolRelations.Any(r => r.CommunityId == communityId));
            //  List<int> classlevlIds = new List<int>();
            //if (communityId > 0)
            //{
            //    classlevlIds = _communityBuss.GetAssignedClassLevelsByComId(assessmentId, communityId);
            //}
            //else if (communityId == 0 && schoolId > 0)
            //{
            //    classlevlIds = _communityBuss.GetAssignedClassLevels(assessmentId, schoolId);
            //}
            //if (!classlevlIds.Contains(0))//All and missing
            //{
            //    query = query.Where(c => classlevlIds.Contains(c.Classlevel));
            //}
            if (schoolId > 0)
            {
                query = query.Where(c => c.SchoolId == schoolId);
            }
            query = query.Where(GetRoleCondition(user));
            return query.ToList();
        }
        public List<CpallsClassModel> GetClassList(Expression<Func<ClassEntity, bool>> condition, UserBaseEntity user
            , string sort, string order, int first, int count, out int total)
        {
            var query = _contract.Classes.AsExpandable().Where(condition).Where(GetRoleCondition(user)).OrderBy(sort, order).Select(r => new CpallsClassModel()
            {
                ID = r.ID,
                Name = r.Name,
                StudentIds = r.Students.Where(stu => stu.Status == EntityStatus.Active && stu.IsDeleted == false).Select(s => s.ID)
            });
            total = query.Count();
            return query.Skip(first).Take(count).ToList();
        }
        public List<CpallsClassModel> GetClassList(Expression<Func<ClassEntity, bool>> condition, UserBaseEntity user,
            DateTime dobStartDate, DateTime dobEndDate, StudentAssessmentLanguage language)
        {
            var query = _contract.Classes.AsExpandable().Where(condition).Where(GetRoleCondition(user)).Select(r => new CpallsClassModel()
            {
                ID = r.ID,
                Name = r.Name,
                StudentIds = r.Students.Where(stu => stu.Status == EntityStatus.Active && stu.IsDeleted == false
                && (stu.BirthDate >= dobStartDate && stu.BirthDate <= dobEndDate) 
                && (stu.AssessmentLanguage ==language || stu.AssessmentLanguage == StudentAssessmentLanguage.Bilingual))
                .Select(s => s.ID),
                Teacher = r.Teachers.Select(t=>t.UserInfo.FirstName+" "+t.UserInfo.LastName)
            });
            return query.ToList();
        }
        #endregion

        #region GetClasses
        /// <summary>
        /// 根据School的主键，获取ClassModel
        /// </summary>
        /// <param name="schoolId"></param>
        /// <returns></returns>
        public IEnumerable<AssigenStudentClassModel> GetClassesBySchoolId(List<int> schoolIds, UserBaseEntity userInfo)
        {
            return
                _contract.Classes.AsExpandable().Where(o => schoolIds.Contains(o.SchoolId) && o.Status == EntityStatus.Active)
                .Where(GetRoleCondition(userInfo))
                    .Select(o => new AssigenStudentClassModel()
                    {
                        ClassId = o.ID,
                        ClassName = o.Name,
                        DayType = o.DayType,
                        ClasroomNameList = o.ClassroomClasses.Select(r => r.Classroom.Name),
                        ClassCode = o.ClassId
                    });
        }

        public List<int> GetClassIdListBySchoolId(int schoolId)
        {
            return _contract.Classes.Where(o => o.SchoolId == schoolId && o.Status == EntityStatus.Active)
                .Select(o => o.ID).ToList();
        }

        /// <summary>
        /// 根据School的主键，获取ClassModel
        /// </summary>
        /// <param name="schoolId"></param>
        /// <returns></returns>
        public IEnumerable<AssigenStudentClassModel> GetClassesBySchoolIdClassId(IList<int> schoolIds, IList<int> classIds, UserBaseEntity userInfo)
        {
            if (classIds.Count > 0)
            {
                return _contract.Classes.AsExpandable().Where(
                        o => (schoolIds.Contains(o.SchoolId) && o.Status == EntityStatus.Active)
                         || (classIds.Contains(o.ID))
                         ).Where(GetRoleCondition(userInfo))
                 .Select(o => new AssigenStudentClassModel()
                 {
                     ClassId = o.ID,
                     ClassName = o.Name,
                     DayType = o.DayType,
                     ClasroomNameList = o.ClassroomClasses.Select(r => r.Classroom.Name),
                     SchoolName = o.School.Name,
                     ClassCode = o.ClassId
                 });
            }
            else
            {
                return
             _contract.Classes.Where(o => schoolIds.Contains(o.SchoolId) && o.Status == EntityStatus.Active)
                 .Select(o => new AssigenStudentClassModel()
                 {
                     ClassId = o.ID,
                     ClassName = o.Name,
                     DayType = o.DayType,
                     ClasroomNameList = o.ClassroomClasses.Select(r => r.Classroom.Name),
                     SchoolName = o.School.Name,
                     ClassCode = o.ClassId
                 });
            }

        }

        /// <summary>
        /// 根据Classroom的主键，获取ClassModel
        /// </summary>
        /// <param name="classroomId"></param>
        /// <returns></returns>
        public IEnumerable<AssignClassModel> GetClassesByClassroomId(int classroomId, UserBaseEntity userInfo)
        {
            return _contract.Classes.AsExpandable().Where(o => o.ClassroomClasses.Select(r => r.ClassroomId).Contains(classroomId) && o.Status == EntityStatus.Active)
                .Where(GetRoleCondition(userInfo))
                .Select(o => new AssignClassModel()
                {
                    ID = o.ID,
                    ClassName = o.Name,
                    ClassId = o.ClassId,
                    ClassType = o.ClassType,
                    tmpCount = o.Students.Count(m => m.Status == EntityStatus.Active && m.IsDeleted == false)
                });
        }

        public IList<ClassLevelEntity> GetClassLevels()
        {
            return _contract.ClasseLevels.ToList();
        }

        public Dictionary<int, int> GetClassCount(params int[] schoolIds)
        {
            var query = _contract.Classes.Where(x => schoolIds.Contains(x.SchoolId))
                .GroupBy(x => x.SchoolId,
                    (schoolId, classes) =>
                        new
                        {
                            schoolId = schoolId,
                            count = classes.Count(c => c.Status == EntityStatus.Active)
                        });
            return query.ToDictionary(item => item.schoolId, item => item.count);
        }

        #endregion

        #region Teacher
        /// <summary>
        /// Assign teacher for new Class
        /// </summary>
        /// <param name="teacherArray"></param>
        /// <param name="classId"></param>
        /// <returns></returns>
        public OperationResult AssignTeacherToNewClass(int[] teacherArray, ClassEntity classEntity)
        {
            OperationResult result = new OperationResult(OperationResultType.Error);

            if (teacherArray != null && teacherArray.Any())
            {
                foreach (var teacherId in teacherArray)
                {
                    TeacherEntity teacherEntity = _userContract.GetTeacher(teacherId);
                    classEntity.Teachers.Add(teacherEntity);
                }
            }
            result = _contract.UpdateClass(classEntity);

            return result;
        }


        public OperationResult AssignTeacherToClass(int[] teacherArray, int classId)
        {
            OperationResult result = new OperationResult(OperationResultType.Error);
            ClassEntity classEntity = _contract.GetClass(classId);
            if (classEntity != null)
            {
                while (classEntity.Teachers.Any())
                    classEntity.Teachers.Remove(classEntity.Teachers.First());
                if (teacherArray != null && teacherArray.Any())
                {
                    foreach (var teacherId in teacherArray)
                    {
                        TeacherEntity teacherEntity = _userContract.GetTeacher(teacherId);
                        classEntity.Teachers.Add(teacherEntity);
                    }
                }
                result = _contract.UpdateClass(classEntity);
            }
            return result;
        }


        public OperationResult AssignTeacherToClass(int teacherId, int classId)
        {
            OperationResult result = new OperationResult(OperationResultType.Error);
            ClassEntity classEntity = _contract.GetClass(classId);
            if (classEntity != null)
            {
                if (teacherId >= 0)
                {
                    TeacherEntity teacherEntity = _userContract.GetTeacher(teacherId);
                    if (classEntity.Teachers == null)
                    {
                        classEntity.Teachers = new List<TeacherEntity>();
                    }
                    classEntity.Teachers.Add(teacherEntity);
                }
                result = _contract.UpdateClass(classEntity);
            }
            return result;
        }

        public string GetTecherEmployBy(int classroomId)
        {
            string res = "";
            _contract.Classes.Where(o => o.ClassroomClasses.Select(r => r.ClassroomId).Contains(classroomId)).ForEach(
                m => m.Teachers.ForEach(
                    n =>
                    {
                        if ((res.IndexOf(n.EmployedBy.ToDescription() + ", ") < 0) && (n.EmployedBy > 0))
                            res = res + n.EmployedBy.ToDescription() + ", ";
                    })
                );
            return res.Trim().Trim(',');
        }

        public List<string> GetTeachers(int classId)
        {
            if (classId < 1)
                return new List<string>();
            return _contract.Classes.First(x => x.ID == classId).Teachers.Where(e => e.UserInfo.IsDeleted == false).Select(
                t => t.UserInfo.FirstName + " " + t.UserInfo.LastName).ToList();
        }
        #endregion

        #region Public Function
        public List<ClassIndexModel> SearchClasses(UserBaseEntity userInfo, Expression<Func<ClassEntity, bool>> condition,
        string sort, string order, int first, int count, out int total)
        {
            var query = _contract.Classes.AsExpandable().Where(condition).Where(GetRoleCondition(userInfo)).
                Select(o => new ClassIndexModel()
                {
                    ID = o.ID,
                    ClassId = o.ClassId,
                    ClassName = o.Name,
                    Status = o.Status,
                    SchoolId = o.SchoolId,
                    ClasroomNameList = o.ClassroomClasses.Select(r => r.Classroom.Name)
                });
            total = query.Count();
            var list = query.OrderBy(sort, order).Skip(first).Take(count);
            return list.ToList();
        }
        public List<CpallsClassModel> GetCpallsClassForCache(UserBaseEntity user, int schoolId, string className, string sort, string order, int first, int count, out int total)
        {
            total = 0;
            var key = "CpallsClass_schoolId_" + schoolId;
            var allClasses = CacheHelper.Get<List<CpallsClassModel>>(key);
            if (allClasses == null)
            {
                lock (CacheHelper.Synchronize)
                {
                    allClasses = CacheHelper.Get<List<CpallsClassModel>>(key);
                    if (allClasses == null)
                    {
                        allClasses = _contract.Classes.AsExpandable().Where(o => o.SchoolId == schoolId && o.Status == EntityStatus.Active && o.SchoolYear == CommonAgent.SchoolYear)
                                                    .Select(r => new CpallsClassModel()
                                                    {
                                                        ID = r.ID,
                                                        Name = r.Name
                                                    }).OrderBy(c => c.Name).ToList();
                        CacheHelper.Add(key, allClasses, CacheHelper.DefaultExpiredSeconds);
                    }
                }
            }
            if (allClasses != null)
            {
                var list = allClasses.ToList();
                if (user.Role != Role.Super_admin)
                {
                    var userClasslIds = CacheHelper.Get<List<int>>("UserClasslIds" + user.ID);
                    if (userClasslIds == null)
                    {
                        userClasslIds = CacheHelper.Get<List<int>>("UserClasslIds" + user.ID);
                        if (userClasslIds == null)
                        {
                            userClasslIds = _contract.Classes.AsExpandable().Where(GetRoleCondition(user)).Select(o => o.ID).ToList();
                            CacheHelper.Add("UserClasslIds" + user.ID, userClasslIds, CacheHelper.DefaultExpiredSeconds);
                        }
                    }
                    list = list.Where(c => userClasslIds.Contains(c.ID)).ToList();
                }
                if (!string.IsNullOrEmpty(className))
                    list = list.Where(c => c.Name.Contains(className)).ToList();

                total = list.Count;
                return list.OrderBy(sort, order).Skip(first).Take(count).ToList();
            }
            else
            {
                return new List<CpallsClassModel>();
            }
        }

        public void SetClassIdCache(UserBaseEntity user)
        {
            var userClasslIds = CacheHelper.Get<List<int>>("UserClasslIds" + user.ID);
            if (userClasslIds == null)
            {
                userClasslIds = _contract.Classes.AsExpandable().Where(GetRoleCondition(user)).Select(o => o.ID).ToList();
                CacheHelper.Add("UserClasslIds" + user.ID, userClasslIds, CacheHelper.DefaultExpiredSeconds);
            }
        }

        public List<AssignClassToSpecialistModel> AssignClass(UserBaseEntity userInfo, Expression<Func<ClassEntity, bool>> condition,
       string sort, string order, int first, int count, out int total)
        {
            var query = _contract.Classes.AsExpandable().Where(condition).Where(GetRoleCondition(userInfo)).
                Select(o => new AssignClassToSpecialistModel()
                {
                    ID = o.ID,
                    ClassId = o.ClassId,
                    ClassName = o.Name,
                    Status = o.Status,
                    SchoolName = o.School.Name,
                    CommunityNameList = o.School.CommunitySchoolRelations.Select(p => p.Community.Name)
                });
            total = query.Count();
            var list = query.OrderBy(sort, order).Skip(first).Take(count);
            return list.ToList();
        }

        public ClassModel GetClassForCpalls(int id)
        {
            if (id < 1) return new ClassModel();
            return _contract.Classes.Where(r => r.ID == id).Select(r => new ClassModel()
            {
                ID = r.ID,
                ClassName = r.Name,
                ClassLevel = r.Classlevel,
                School = new CpallsSchoolModel()
                {
                    ID = r.School.ID,
                    Name = r.School.Name,

                    Communities = r.School.CommunitySchoolRelations.Select(c => new CpallsCommunityModel()
                    {
                        ID = c.Community.ID,
                        Name = c.Community.Name
                    })
                }
            }).FirstOrDefault();
        }

        public CpallsClassModel GetClassForCpallsCache(int id, int schoolId)
        {
            var key = "CpallsClass_schoolId_" + schoolId;
            CpallsClassModel classModel = new CpallsClassModel();
            var allClasses = CacheHelper.Get<List<CpallsClassModel>>(key);
            if (allClasses != null)
            {
                classModel = allClasses.FirstOrDefault(c => c.ID == id);

            }
            else
            {
                classModel = _contract.Classes.Where(r => r.ID == id).Select(r => new CpallsClassModel()
                {
                    ID = r.ID,
                    Name = r.Name
                }).FirstOrDefault();
            }
            return classModel;
        }


        public ClassRoleEntity GetClassRole(Role role)
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
            return _contract.GetClassRole(newRole);
        }
        #endregion

        #region Private Function
        /// <summary>
        /// 更新前，初始化某些参数
        /// </summary>
        /// <param name="Class"></param>
        /// <param name="role"></param>
        /// <returns></returns>
        private ClassEntity InitByRole(ClassEntity Class, ClassRoleEntity role)
        {
            ClassEntity oldEntity = NewClassEntity();

            if (Class.ID > 0)
                oldEntity = _contract.GetClass(Class.ID);

            Type r = role.GetType();
            Type c = Class.GetType();
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
                        c.GetProperty(name).SetValue(Class, oldValue);
                    }
                }
            }
            return Class;
        }

        /// <summary>
        /// 根据不同的角色查找对应的数据源
        /// </summary>
        /// <param name="userInfo"></param>
        /// <returns></returns>
        private Expression<Func<ClassEntity, bool>> GetRoleCondition(UserBaseEntity userInfo)
        {
            Expression<Func<ClassEntity, bool>> condition = o => true;
            if (userInfo == null)
                return condition;
            var parent = new UserBaseEntity();
            if (userInfo.Role == Role.Community_Specialist_Delegate || userInfo.Role == Role.District_Community_Delegate)
            {
                parent = _userContract.GetUser(userInfo.CommunityUser.ParentId);
            }

            //UserBusiness userBusiness = new UserBusiness();

            // UserBaseEntity baseUser = userBusiness.GetUser(userInfo.ID);
            List<int> basicComIds = userInfo.UserCommunitySchools.Where(o => o.CommunityId > 0).Select(o => o.Community.BasicCommunityId).ToList();
            List<int> comIds = userInfo.UserCommunitySchools.Where(o => o.CommunityId > 0).Select(o => o.CommunityId).ToList();
            List<int> schoolIds = new List<int>();

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
                    condition = PredicateBuilder.And(condition, r =>
                              (r.School.CommunitySchoolRelations.Any(c => comIds.Contains(c.CommunityId))));
                    break;


                case Role.Parent:
                    condition = o => false;
                    break;
                case Role.Teacher:  //与teacher关联的class          
                    if (userInfo.TeacherInfo != null)
                        condition = PredicateBuilder.And(condition, m => m.Teachers.Any(u => u.UserInfo.ID == userInfo.ID && u.UserInfo.IsDeleted == false));
                    break;

                case Role.Principal:  //当前学校下的class
                case Role.TRS_Specialist:
                    condition = PredicateBuilder.And(condition, r => r.School.UserCommunitySchools.Any(u => u.UserId == userInfo.ID));
                    break;
                case Role.Principal_Delegate:
                case Role.TRS_Specialist_Delegate:
                    condition = PredicateBuilder.And(condition, r => r.School.UserCommunitySchools.Any(u => u.UserId == userInfo.Principal.ParentId));
                    break;
                case Role.School_Specialist:
                    condition = o => false;
                    schoolIds = userInfo.UserCommunitySchools
                       .Where(r => !userInfo.UserClasses.Where(u => u.UserId == userInfo.ID && u.Class.IsDeleted == false)
                           .Select(u => u.Class.SchoolId).Distinct().Contains(r.SchoolId) && r.UserId == userInfo.ID)
                       .Select(r => r.SchoolId).Distinct().ToList();

                    condition = PredicateBuilder.Or(condition, o => schoolIds.Contains(o.SchoolId));
                    condition = PredicateBuilder.Or(condition, r => r.UserClasses.Any(u => u.UserId == userInfo.ID));
                    break;

                case Role.School_Specialist_Delegate:

                    condition = o => false;

                    schoolIds = _contract.Classes.Where(c => c.School.UserCommunitySchools.Any(u => u.UserId == userInfo.Principal.ParentId))
                        .Select(c => c.SchoolId).Distinct().ToList();

                    List<int> AssignedClassSchoolIds = _contract.Classes.Where(c => c.UserClasses.Any(u => u.UserId == userInfo.Principal.ParentId))
                        .Select(c => c.SchoolId).Distinct().ToList();
                    var allClassSchoolIds = schoolIds.Except(AssignedClassSchoolIds);

                    condition = PredicateBuilder.Or(condition, o => allClassSchoolIds.Contains(o.SchoolId));
                    condition = PredicateBuilder.Or(condition, r => r.UserClasses.Any(u => u.UserId == userInfo.Principal.ParentId));
                    break;
                case Role.Community:
                    condition = PredicateBuilder.And(condition, r =>
                                (basicComIds.Contains(r.School.BasicSchool.CommunityId))
                              ||
                              (
                                  r.School.CommunitySchoolRelations.Any(c => comIds.Contains(c.CommunityId)) &&
                                   r.UserClasses.Any(u => u.UserId == userInfo.ID)
                               )
                           );
                    break;
                case Role.District_Community_Delegate:
                    List<int> parentBasicComIds = parent.UserCommunitySchools.Where(o => o.CommunityId > 0).Select(o => o.Community.BasicCommunityId).ToList();
                    List<int> parentComIds = parent.UserCommunitySchools.Where(o => o.CommunityId > 0).Select(o => o.CommunityId).ToList();
                    condition = PredicateBuilder.And(condition, r =>
                                (parentBasicComIds.Contains(r.School.BasicSchool.CommunityId))
                              ||
                              (
                                  r.School.CommunitySchoolRelations.Any(c => parentComIds.Contains(c.CommunityId)) &&
                                   r.UserClasses.Any(u => u.UserId == userInfo.ID)
                               )
                           );
                    break;
                case Role.District_Community_Specialist:
                    condition = PredicateBuilder.And(condition, r =>
                                (basicComIds.Contains(r.School.BasicSchool.CommunityId)
                                )
                              ||
                              (
                                      r.School.CommunitySchoolRelations.Any(c => comIds.Contains(c.CommunityId)) &&
                                   r.UserClasses.Any(u => u.UserId == userInfo.ID)
                               )
                           );
                    break;
                case Role.Community_Specialist_Delegate:
                    List<int> parentBasicComIds2 = parent.UserCommunitySchools.Where(o => o.CommunityId > 0).Select(o => o.Community.BasicCommunityId).ToList();
                    List<int> parentComIds2 = parent.UserCommunitySchools.Where(o => o.CommunityId > 0).Select(o => o.CommunityId).ToList();
                    condition = PredicateBuilder.And(condition, r =>
                                (parentBasicComIds2.Contains(r.School.BasicSchool.CommunityId)
                                )
                              ||
                              (
                                      r.School.CommunitySchoolRelations.Any(c => parentComIds2.Contains(c.CommunityId)) &&
                                   r.UserClasses.Any(u => u.UserId == userInfo.ID)
                               )
                           );
                    break;
                case Role.Statewide:
                    // condition = PredicateBuilder.And(condition, r => r.School.CommunitySchoolRelations.Any(c => c.Community.UserCommunitySchools.Any(u => u.UserId == baseUser.ID)));
                    // condition = PredicateBuilder.And(condition, r => r.School.UserCommunitySchools.Any((u => u.UserId == baseUser.ID && u.SchoolId > 0)) && r.UserClasses.Any(u => u.UserId == baseUser.ID));
                    condition = PredicateBuilder.And(condition, r =>
                                (basicComIds.Contains(r.School.BasicSchool.CommunityId)
                                )
                              ||
                              (r.School.CommunitySchoolRelations.Any(c => comIds.Contains(c.CommunityId)) &&
                                   r.UserClasses.Any(u => u.UserId == userInfo.ID))
                           );
                    break;
            }

            return condition;
        }
        #endregion

        #region Classroom Class Relations

        #region 给Class分配Classroom
        public List<SelectItemModel> GetAssignedClassroom(UserBaseEntity user,
            Expression<Func<ClassroomClassEntity, bool>> condition,
            string sort, string order, int first, int count, out int total)
        {
            //有权限的Classroom
            List<int> classroomIds = _contract.Classrooms.AsExpandable()
                .Where(_classroomBussiness.GetRoleCondition(user)).Select(c => c.ID).ToList();

            var query = _contract.ClassroomClassRelations.AsExpandable()
                .Where(o => classroomIds.Contains(o.ClassroomId))
               .Where(condition)
                .Select(o => new SelectItemModel()
                {
                    ID = o.ClassroomId,
                    Name = o.Classroom.Name
                });
            total = query.Count();
            var list = query.OrderBy(sort, order).Skip(first).Take(count);
            return list.ToList();
        }
        public List<SelectItemModel> GetUnAssignClassroom(UserBaseEntity user, int schoolId, int classId, string name,
            string sort, string order, int first, int count, out int total)
        {
            int[] classroomIds = _contract.ClassroomClassRelations.Where(o => o.ClassId == classId).Select(r => r.ClassroomId).ToArray();
            var query = _contract.Classrooms.AsExpandable().Where(_classroomBussiness.GetRoleCondition(user))
                .Where(o => o.Name.Contains(name)
                && !classroomIds.Contains(o.ID) && o.SchoolId == schoolId)
                .Select(r => new SelectItemModel()
                {
                    ID = r.ID,
                    Name = r.Name
                });
            total = query.Count();
            var list = query.OrderBy(sort, order).Skip(first).Take(count);
            return list.ToList();
        }
        public OperationResult InserClassroomClassRelations(int classId, int[] classroomIds)
        {
            List<ClassroomClassEntity> list = new List<ClassroomClassEntity>();
            foreach (int classroomId in classroomIds)
            {
                ClassroomClassEntity entity = new ClassroomClassEntity();
                entity.ClassId = classId;
                entity.ClassroomId = classroomId;
                entity.Status = EntityStatus.Active;
                entity.CreatedBy = 1;
                entity.UpdatedBy = 1;
                entity.CreatedOn = DateTime.Now;
                entity.UpdatedOn = DateTime.Now;
                list.Add(entity);
            }
            return _contract.InsertRelations(list);
        }
        public OperationResult DeleteClassroomClassRelations(int classId, int[] classroomIds)
        {
            IList<ClassroomClassEntity> list = _contract.ClassroomClassRelations
                .Where(o => o.ClassId == classId && classroomIds.Contains(o.ClassroomId)).ToList();
            return _contract.DeleteRelations(list);
        }
        #endregion

        #region 给Classroom分配class
        public List<SelectItemModel> GetAssignedClass(UserBaseEntity user, Expression<Func<ClassroomClassEntity, bool>> condition,
          string sort, string order, int first, int count, out int total)
        {
            //有权限的Class
            List<int> classIds = _contract.Classes.AsExpandable().Where(GetRoleCondition(user))
                .Select(c => c.ID).ToList();

            var query = _contract.ClassroomClassRelations.AsExpandable()
                .Where(o => classIds.Contains(o.ClassId)).Where(condition)
                .Select(o => new SelectItemModel()
                {
                    ID = o.ClassId,
                    Name = o.Class.Name
                });
            total = query.Count();
            var list = query.OrderBy(sort, order).Skip(first).Take(count);
            return list.ToList();
        }


        public List<SelectItemModel> GetUnAssignClass(UserBaseEntity user, int schoolId, int classroomId, string name,
            string sort, string order, int first, int count, out int total)
        {
            int[] classIds = _contract.ClassroomClassRelations.Where(o => o.ClassroomId == classroomId).Select(r => r.ClassId).ToArray();
            var query = _contract.Classes.AsExpandable().Where(GetRoleCondition(user))
                .Where(o => o.Name.Contains(name)
                && !classIds.Contains(o.ID) && o.SchoolId == schoolId)
                .Select(r => new SelectItemModel()
                {
                    ID = r.ID,
                    Name = r.Name
                });
            total = query.Count();
            var list = query.OrderBy(sort, order).Skip(first).Take(count);
            return list.ToList();
        }
        public OperationResult InserClassroomClassRelationsOnClassroom(int classroomId, int[] classIds)
        {
            List<ClassroomClassEntity> list = new List<ClassroomClassEntity>();
            foreach (int classId in classIds)
            {
                ClassroomClassEntity entity = new ClassroomClassEntity();
                entity.ClassId = classId;
                entity.ClassroomId = classroomId;
                entity.Status = EntityStatus.Active;
                entity.CreatedBy = 1;
                entity.UpdatedBy = 1;
                list.Add(entity);
            }
            return _contract.InsertRelations(list);
        }

        public OperationResult DeleteClassroomClassRelationsOnClassroom(int classroomId, int[] classIds)
        {
            IList<ClassroomClassEntity> list = _contract.ClassroomClassRelations.Where(o => o.ClassroomId == classroomId && classIds.Contains(o.ClassId)).ToList();
            return _contract.DeleteRelations(list);
        }

        #endregion

        #endregion

        #region Class Student Relation

        public List<StudentClassModel> GetAssignedStudent(UserBaseEntity user, Expression<Func<StudentEntity, bool>> condition,
           string sort, string order, int first, int count, out int total)
        {
            return _studentBusiness.GetClassStudentList(condition, user, sort, order, first, count, out total);
        }

        public List<StudentClassModel> GetUnAssignStudent(
            UserBaseEntity user, Expression<Func<StudentEntity, bool>> condition, int classId,
            string sort, string order, int first, int count, out int total)
        {
            List<int> studentIds = _studentBusiness.GetStudentIdsByClassId(classId);
            condition = PredicateBuilder.And(condition, o => !studentIds.Contains(o.ID));
            var query = _studentBusiness.GetClassStudentList(condition, user, sort, order, first, count, out total);
            return query.ToList();
        }

        public OperationResult AssignStudentsToClass(int classId, int[] studentIds, UserBaseEntity user)
        {
            ClassEntity classEntity = GetClass(classId);
            List<int> stuIdsForNewRelation = new List<int>();
            foreach (int studentId in studentIds)
            {
                StudentEntity student = _studentBusiness.GetStudentById(studentId);
                classEntity.Students.Add(student);

                if (_schoolBusiness.CheckSchooStudentRelations(classEntity.SchoolId, studentId) == false)
                {
                    stuIdsForNewRelation.Add(studentId);
                }
            }
            _schoolBusiness.InserSchoolStudentRelations(stuIdsForNewRelation, classEntity.SchoolId, user);

            return UpdateClass(classEntity);
        }

        public OperationResult UnAssignStudentsFromClass
            (int classId, List<StudentEntity> students, UserBaseEntity user)
        {
            ClassEntity entity = _contract.GetClass(classId);
            List<SchoolStudentRelationEntity> schoolStudentEntities = new List<SchoolStudentRelationEntity>();

            List<int> otherClassIds = _contract.Classes
                    .Where(c => c.SchoolId == entity.SchoolId
                    && c.ID != entity.ID).Select(c => c.ID).ToList();
            foreach (StudentEntity student in students)
            {
                //属不属于当前学校下的其他Class
                if (student.Classes.Count(c => otherClassIds.Contains(c.ID) && c.IsDeleted == false) == 0)
                {
                    List<SchoolStudentRelationEntity> schoolStudent =
                        _schoolBusiness.GetSchoolStudentRelationList(r => r.SchoolId == entity.SchoolId
                            && r.StudentId == student.ID);
                    schoolStudent.ForEach(s => schoolStudentEntities.Add(s));
                }
                else
                {
                    /*如果当前class所在school下的其他和选中学生存在关系的class均为InActive,
                     则删除选中的学生与这些class的关系，&& 删除选中的学生与shcool的关系*/
                    List<ClassEntity> stuClassEntities = student.Classes.Where(c => otherClassIds.Contains(c.ID)).ToList();
                    if (stuClassEntities.Count(c => c.Status == EntityStatus.Active) == 0)
                    {
                        stuClassEntities.ForEach(o => student.Classes.Remove(o));

                        List<SchoolStudentRelationEntity> schoolStudent =
                            _schoolBusiness.GetSchoolStudentRelationList(r => r.SchoolId == entity.SchoolId && r.StudentId == student.ID);
                        schoolStudent.ForEach(s => schoolStudentEntities.Add(s));
                    }
                }
                entity.Students.Remove(student);
            }
            _studentBusiness.UpdateStudents(students);
            _schoolBusiness.DeleteRelations(schoolStudentEntities);
            return UpdateClass(entity);
        }



        #endregion

        #region BUP
        public List<NameModel> GetClassesBySchool(string schoolName, string schoolEngageId, UserBaseEntity user)
        {
            return _contract.Classes.AsExpandable()
                .Where(r => r.School.Name == schoolName && r.School.SchoolId == schoolEngageId)
                .Where(GetRoleCondition(user))
                .Select(r => new NameModel
                {
                    EngageId = r.ClassId,
                    Name = r.Name
                }).ToList();
        }

        public List<NameModelWithSchool> GetClassesBySchools(List<string> schoolNames, List<string> schoolEngageIds, UserBaseEntity user)
        {
            return _contract.Classes.AsExpandable()
                .Where(r => schoolNames.Contains(r.School.Name) && schoolEngageIds.Contains(r.School.SchoolId))
                .Where(GetRoleCondition(user))
                .Select(r => new NameModelWithSchool
                {
                    SchoolName = r.School.Name,
                    SchoolEngageId = r.School.SchoolId,
                    EngageId = r.ClassId,
                    Name = r.Name
                }).ToList();
        }
        #endregion
    }
}