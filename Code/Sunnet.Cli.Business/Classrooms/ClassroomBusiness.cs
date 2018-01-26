using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

/**************************************************************************
 * Developer: 		Leason
 * Computer:		Leason-PC
 * Domain:			Leason-pc
 * CreatedOn:		2014/8/22 17:20:20
 * Description:		Create ClassroomBusiness
 * Version History:	Created,2014/8/22 17:20:20 
 * 
 * 
 **************************************************************************/
using System.Reflection;
using Sunnet.Cli.Business.Classes;
using Sunnet.Cli.Business.Classrooms.Models;
using Sunnet.Cli.Business.Common;
using Sunnet.Cli.Business.Common.Enum;
using Sunnet.Cli.Business.Communities;
using Sunnet.Cli.Business.Trs.Models;
using Sunnet.Cli.Business.Users;
using Sunnet.Cli.Core;
using Sunnet.Cli.Core.Classrooms;
using Sunnet.Cli.Core.Classrooms.Entites;
using Sunnet.Cli.Core.Classrooms.Enums;
using Sunnet.Cli.Core.Common.Enums;
using Sunnet.Cli.Core.Communities.Entities;
using Sunnet.Cli.Core.Users.Entities;
using Sunnet.Cli.Core.Users.Enums;
using Sunnet.Framework.Core.Tool;
using Sunnet.Framework.Core.Extensions;
using Sunnet.Framework.Extensions;
using LinqKit;
using Sunnet.Framework.Helpers;
using Sunnet.Cli.Core.Classes;
using Sunnet.Cli.Core.Classes.Entites;
using Sunnet.Cli.Business.BUP.Model;

namespace Sunnet.Cli.Business.Classrooms
{
    public class ClassroomBusiness
    {

        private readonly IClassroomContract _contract;
        private ClassBusiness _classBus
        {
            get { return new ClassBusiness(); }
        }

        public ClassroomBusiness(EFUnitOfWorkContext unit = null)
        {
            _contract = DomainFacade.CreateClassroomService(unit);
        }

        #region Get Methods
        public ClassroomEntity GetClassroom(int classroomId)
        {
            return _contract.GetClassroom(classroomId);
        }

        public ClassroomEntity NewClassroomEntity()
        {
            ClassroomEntity classroom = _contract.NewClassroomEntity();
            classroom.ClassroomInternalID = string.Empty;
            classroom.SchoolYear = CommonAgent.SchoolYear;
            classroom.StatusDate = DateTime.Now;
            classroom.CurriculumUpdatedOn = DateTime.Now;
            classroom.DevelopingTalkersKitUpdatedOn = DateTime.Now;
            classroom.FccKitUpdatedOn = DateTime.Now;
            classroom.KitUpdatedOn = DateTime.Now;
            classroom.NeedCurriculumUpdatedOn = DateTime.Now;
            classroom.Part1KitUpdatedOn = DateTime.Now;
            classroom.Part2KitUpdatedOn = DateTime.Now;
            classroom.StartupKitUpdatedOn = DateTime.Now;
            classroom.InterventionStatus = (InterventionStatus)100;
            classroom.Status = (EntityStatus)100;
            classroom.InterventionOther = "";
            classroom.MaterialsNotes = "";
            classroom.TechnologyNotes = "";
            classroom.ClassroomId = "";
            return classroom;
        }



        public ClassroomModel GetClassroom(int id, UserBaseEntity user)
        {
            ClassroomModel model = _contract.Classrooms.AsExpandable().Where(o => o.ID == id).Where(GetRoleCondition(user)).Select(EntityToModel).FirstOrDefault();
            if (model != null)
                model.RefrenceData = new ClassBusiness().GetTecherEmployBy(id);

            return model;
        }

        public List<ClassroomModel> SearchClassrooms(UserBaseEntity user, Expression<Func<ClassroomEntity, bool>> condition,
          string sort, string order, int first, int count, out int total)
        {
            var query = _contract.Classrooms.AsExpandable().Where(condition).Where(GetRoleCondition(user)).Select(ClassroomEntityToModel);
            total = query.Count();
            var list = query.OrderBy(sort, order).Skip(first).Take(count);
            return list.ToList();
        }



        public KitEntity NewKitEntity()
        {
            return _contract.NewKitEntity();
        }

        public KitEntity GetKit(int id)
        {
            return _contract.GetKit(id);
        }

        public IEnumerable<SelectItemModel> GetKitsSelectList(bool isContainsNull = true, bool isActive = true)
        {
            if (!isContainsNull)
            {
                return _contract.Kits.Where(o => o.Name.ToLower() != "none")
                    .Where(o => o.Status == EntityStatus.Active || isActive == false).Select(e => new SelectItemModel()
                {
                    ID = e.ID,
                    Name = e.Name
                });
            }
            else
            {
                return _contract.Kits.Where(o => o.Status == EntityStatus.Active || isActive == false)
                    .Select(e => new SelectItemModel()
                {
                    ID = e.ID,
                    Name = e.Name
                });
            }
        }

        public IEnumerable<SelectItemModelOther> GetKitsSelectListOther()
        {
            return _contract.Kits.Select(e => new SelectItemModelOther()
                {
                    ID = e.ID,
                    Name = e.Name,
                    Status = e.Status

                });
        }

        public IEnumerable<SelectItemModel> GetClassroomSelectList(UserBaseEntity user, int communityId, int schoolId, string keyword = "-1")
        {

            if (communityId == -1 && schoolId == -1)
            {
                return
                    _contract.Classrooms.AsExpandable().Where(o => o.Status == EntityStatus.Active).
                    Where(GetRoleCondition(user)).OrderBy(o => o.Name).Select(e => new SelectItemModel()
                    {
                        ID = e.ID,
                        Name = e.Name.Trim()
                    }).OrderBy(o => o.Name);
            }
            else
            {
                return
                    _contract.Classrooms.AsExpandable().Where(o =>
                        (o.Status == EntityStatus.Active
                              && (o.School.CommunitySchoolRelations.Count(r => r.CommunityId == communityId) > 0
                              || communityId < 1)
                        && (o.SchoolId == schoolId || schoolId < 1))).Where(GetRoleCondition(user))
                        .Select(e => new SelectItemModel()
                        {
                            ID = e.ID,
                            Name = e.Name
                        }).OrderBy(r => r.Name);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="isActive">True时，返回有效数据；False时，全部数据。</param>
        /// <returns></returns>
        public IEnumerable<SelectItemModel> GetClassroomNameSelectList(UserBaseEntity user, Expression<Func<ClassroomEntity, bool>> expression, bool isActive = true)
        {
            return _contract.Classrooms.AsExpandable().Where(expression).
                Where(o => o.Status == EntityStatus.Active || isActive == false)
                .Where(GetRoleCondition(user))
                .Select(o => new SelectItemModel()
                {
                    ID = o.ID,
                    Name = o.Name
                });
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="isActive">True时，返回有效数据；False时，全部数据。</param>
        /// <returns></returns>
        public IEnumerable<SelectItemModel> GetClassroomIdSelectList(UserBaseEntity user, Expression<Func<ClassroomEntity, bool>> expression, bool isActive = true)
        {
            return _contract.Classrooms.AsExpandable().Where(expression).
                Where(o => o.Status == EntityStatus.Active || isActive == false)
                    .Where(GetRoleCondition(user))
                .Select(o => new SelectItemModel()
                {
                    ID = o.ID,
                    Name = o.ClassroomId
                });
        }


        #endregion

        #region Insert Methods
        public OperationResult InsertClassroom(ClassroomEntity entity, Role role)
        {
            ClassroomRoleEntity roleEntity = GetClassroomRoleEntity(role);
            entity = InitByRole(entity, roleEntity);
            if (entity.ClassroomInternalID == null) entity.ClassroomInternalID = string.Empty;
            return _contract.InsertClassroom(entity);
        }
        public OperationResult InsertClassroom(ClassroomEntity entity, Role role, int[] hiddenChild, int[] dpChildrenNumber)
        {
            ClassroomRoleEntity roleEntity = GetClassroomRoleEntity(role);
            entity = InitByRole(entity, roleEntity);
            return _contract.InsertClassroom(entity);
        }

        public OperationResult InsertKit(KitEntity entity)
        {
            return _contract.InsertKit(entity);
        }

        #endregion

        #region Update Methods

        public OperationResult UpdateClassroom(ClassroomEntity entity, Role role)
        {
            ClassroomRoleEntity roleEntity = GetClassroomRoleEntity(role);
            entity = InitByRole(entity, roleEntity);

            return _contract.UpdateClassroom(entity);
        }

        public OperationResult UpdateKit(KitEntity entity)
        {
            return _contract.UpdateKit(entity);
        }

        #endregion

        private static Expression<Func<ClassroomEntity, ClassroomModel>> ClassroomEntityToModel
        {
            get
            {
                return x => new ClassroomModel()
                {
                    ID = x.ID,
                    ClassroomId = x.ClassroomId,
                    Name = x.Name,
                    SchoolName = x.School.BasicSchool.Name,
                    SchoolId = x.SchoolId,
                    Status = x.Status
                };
            }
        }

        //public OperationResult DeleteKit(int id)
        //{
        //    return _contract.DeleteKit(id);
        //}

        private ClassroomEntity ModelToEntity(ClassroomModel m, ClassroomEntity e)
        {
            e.ID = m.ID;
            e.ClassroomId = m.ClassroomId;
            e.SchoolId = m.SchoolId;
            e.Name = m.Name;
            e.Status = m.Status;
            e.StatusDate = m.StatusDate;
            e.SchoolYear = m.SchoolYear;
            e.InterventionStatus = m.InterventionStatus;
            e.InterventionOther = m.InterventionOther;
            e.FundingId = m.FundingId;
            e.KitId = m.KitId;
            e.KitUpdatedOn = m.KitUpdatedOn;
            e.FcNeedKitId = m.FcNeedKitId;
            e.FcFundingId = m.FcFundingId;
            e.Part1KitId = m.Part1KitId;
            e.Part1KitUpdatedOn = m.Part1KitUpdatedOn;
            e.Part1NeedKitId = m.Part1NeedKitId;
            e.Part1FundingId = m.Part1FundingId;
            e.Part2KitId = m.Part2KitId;
            e.Part2KitUpdatedOn = m.Part2KitUpdatedOn;
            e.Part2NeedKitId = m.Part2NeedKitId;
            e.Part2FundingId = m.Part2FundingId;
            e.StartupKitId = m.StartupKitId;
            e.StartupKitUpdatedOn = m.StartupKitUpdatedOn;
            e.StartupNeedKitId = m.StartupNeedKitId;
            e.StartupKitFundingId = m.StartupKitFundingId;
            e.CurriculumId = m.CurriculumId;
            e.CurriculumUpdatedOn = m.CurriculumUpdatedOn;
            e.NeedCurriculumId = m.NeedCurriculumId;
            e.NeedCurriculumUpdatedOn = m.NeedCurriculumUpdatedOn;
            e.CurriculumFundingId = m.CurriculumFundingId;
            e.DevelopingTalkersKitId = m.DevelopingTalkersKitId;
            e.DevelopingTalkersKitUpdatedOn = m.DevelopingTalkersKitUpdatedOn;
            e.DevelopingTalkersNeedKitId = m.DevelopingTalkersNeedKitId;
            e.DevelopingTalkerKitFundingId = m.DevelopingTalkerKitFundingId;
            e.FccKitId = m.FccKitId;
            e.FccKitUpdatedOn = m.FccKitUpdatedOn;
            e.FccNeedKitId = m.FccNeedKitId;
            e.FccKitFundingId = m.FccKitFundingId;
            e.InternetSpeed = m.InternetSpeed;
            e.InternetType = m.InternetType;
            e.WirelessType = m.WirelessType;
            e.IsUsingInClassroom = m.IsUsingInClassroom;
            e.ComputerNumber = m.ComputerNumber;
            e.IsInteractiveWhiteboard = m.IsInteractiveWhiteboard;
            e.MaterialsNotes = m.MaterialsNotes;
            e.TechnologyNotes = m.TechnologyNotes;
            e.CreatedOn = m.CreatedOn;
            e.UpdatedOn = m.UpdatedOn;
            return e;
        }

        private static Expression<Func<ClassroomEntity, ClassroomModel>> EntityToModel
        {
            get
            {
                return e => new ClassroomModel()
                {
                    ID = e.ID,
                    ClassroomId = e.ClassroomId,
                    ClassroomInternalID = e.ClassroomInternalID,
                    CommunityNameList = e.School.CommunitySchoolRelations.Select(r => r.Community.Name),
                    SchoolName = e.School.BasicSchool.Name,
                    SchoolId = e.SchoolId,
                    Name = e.Name,
                    Status = e.Status,
                    StatusDate = e.StatusDate,
                    SchoolYear = e.SchoolYear,
                    InterventionStatus = e.InterventionStatus,
                    InterventionOther = e.InterventionOther,
                    FundingId = e.FundingId,
                    KitId = e.KitId,
                    KitUpdatedOn = e.KitUpdatedOn,
                    FcNeedKitId = e.FcNeedKitId,
                    FcFundingId = e.FcFundingId,
                    Part1KitId = e.Part1KitId,
                    Part1KitUpdatedOn = e.Part1KitUpdatedOn,
                    Part1NeedKitId = e.Part1NeedKitId,
                    Part1FundingId = e.Part1FundingId,
                    Part2KitId = e.Part2KitId,
                    Part2KitUpdatedOn = e.Part2KitUpdatedOn,
                    Part2NeedKitId = e.Part2NeedKitId,
                    Part2FundingId = e.Part2FundingId,
                    StartupKitId = e.StartupKitId,
                    StartupKitUpdatedOn = e.StartupKitUpdatedOn,
                    StartupNeedKitId = e.StartupNeedKitId,
                    StartupKitFundingId = e.StartupKitFundingId,
                    CurriculumId = e.CurriculumId,
                    CurriculumUpdatedOn = e.CurriculumUpdatedOn,
                    NeedCurriculumId = e.NeedCurriculumId,
                    NeedCurriculumUpdatedOn = e.NeedCurriculumUpdatedOn,
                    CurriculumFundingId = e.CurriculumFundingId,
                    DevelopingTalkersKitId = e.DevelopingTalkersKitId,
                    DevelopingTalkersKitUpdatedOn = e.DevelopingTalkersKitUpdatedOn,
                    DevelopingTalkersNeedKitId = e.DevelopingTalkersNeedKitId,
                    DevelopingTalkerKitFundingId = e.DevelopingTalkerKitFundingId,
                    FccKitId = e.FccKitId,
                    FccKitUpdatedOn = e.FccKitUpdatedOn,
                    FccNeedKitId = e.FccNeedKitId,
                    FccKitFundingId = e.FccKitFundingId,
                    InternetSpeed = e.InternetSpeed,
                    InternetType = e.InternetType,
                    WirelessType = e.WirelessType,
                    IsUsingInClassroom = e.IsUsingInClassroom,
                    ComputerNumber = e.ComputerNumber,
                    IsInteractiveWhiteboard = e.IsInteractiveWhiteboard,
                    MaterialsNotes = e.MaterialsNotes,
                    TechnologyNotes = e.TechnologyNotes,
                    CreatedOn = e.CreatedOn,
                    UpdatedOn = e.UpdatedOn,
                    SchoolType = e.School.SchoolType
                };
            }
        }

        public bool IsClassroomExist(string name, int id = 0, int schoolId = 0)
        {
            return _contract.Classrooms.Any(o => o.Name == name && o.ID != id && o.SchoolId == schoolId);
        }

        public Expression<Func<ClassroomEntity, bool>> GetRoleCondition(UserBaseEntity userInfo)
        {
            Expression<Func<ClassroomEntity, bool>> condition = o => true;
            if (userInfo == null)
                return condition;
            UserBusiness userBusiness = new UserBusiness();
            UserBaseEntity baseUser = userBusiness.GetUser(userInfo.ID);
            List<int> basicComIds = baseUser.UserCommunitySchools.Where(o => o.CommunityId > 0).Select(o => o.Community.BasicCommunityId).ToList();
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
                    condition = PredicateBuilder.And(condition,
                          o => o.School.CommunitySchoolRelations.Any(p => p.Community.UserCommunitySchools.Any(q => q.UserId == userInfo.ID)));
                    break;

                case Role.Teacher:   //teacher所在的class的classroom
                    if (baseUser.TeacherInfo != null)
                    {
                        if (baseUser.TeacherInfo.Classes.Where(e => e.IsDeleted == false).Count() == 0)
                        {
                            condition = o => false;
                        }
                        else
                        {
                            condition = o => false;
                            baseUser.TeacherInfo.Classes.Where(e => e.IsDeleted == false).ForEach(o =>
                            {
                                condition = PredicateBuilder.Or(condition, r => r.ClassroomClasses.Any(c => c.ClassId == o.ID && c.Class.IsDeleted == false));
                            });
                        }
                    }
                    else
                    {
                        condition = o => false;
                    }
                    break;
                case Role.Principal:  //当前school下的所有classroom
                case Role.TRS_Specialist:
                case Role.School_Specialist:
                    if (baseUser.Principal != null)
                    {
                        condition = PredicateBuilder.And(condition, r => r.School.UserCommunitySchools.Any(u => u.UserId == baseUser.ID));
                    }
                    else
                        condition = o => false;
                    break;
                case Role.Principal_Delegate:
                case Role.TRS_Specialist_Delegate:
                case Role.School_Specialist_Delegate:
                    if (baseUser.Principal != null)
                    {
                        condition = PredicateBuilder.And(condition, r => r.School.UserCommunitySchools.Any(u => u.UserId == baseUser.Principal.ParentId));
                    }
                    else
                        condition = o => false;
                    break;
                case Role.Community:  //当前community下的所有school的所有 classroom
                case Role.District_Community_Specialist:

                    condition = PredicateBuilder.And(condition, r => r.School.CommunitySchoolRelations
                        .Any(c => c.Community.UserCommunitySchools.Any(u => u.UserId == baseUser.ID)));

                    //condition = PredicateBuilder.And(condition, r => (comIds.Contains(r.School.BasicSchool.CommunityId))
                    //      ||
                    //      (
                    //          r.Community.UserCommunitySchools.Any((u => u.UserId == baseUser.ID))
                    //       )
                    //   );
                    break;

                case Role.District_Community_Delegate:
                case Role.Community_Specialist_Delegate:
                    condition = PredicateBuilder.And(condition, r => r.School.CommunitySchoolRelations
                        .Any(c => c.Community.UserCommunitySchools.Any(u => u.UserId == baseUser.CommunityUser.ParentId)));
                    break;
                case Role.Parent:
                    condition = o => false;
                    break;
                case Role.Statewide:
                    condition = PredicateBuilder.And(condition, r => r.School.CommunitySchoolRelations
                      .Any(c => c.Community.UserCommunitySchools.Any(u => u.UserId == baseUser.ID)));

                    //condition = PredicateBuilder.And(condition, r => (comIds.Contains(r.School.BasicSchool.CommunityId))
                    //         ||
                    //         (
                    //             r.School.UserCommunitySchools.Any((u => u.UserId == baseUser.ID && u.SchoolId > 0))
                    //          )
                    //      );
                    break;
            }
            return condition;
        }

        #region Classroom Role
        public ClassroomRoleEntity GetClassroomRoleEntity(Role role)
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
            return _contract.GetRole(newRole);
        }
        public ClassroomEntity InitByRole(ClassroomEntity classroom, ClassroomRoleEntity role)
        {
            ClassroomEntity oldEntity = NewClassroomEntity();

            if (classroom.ID > 0)
                oldEntity = _contract.GetClassroom(classroom.ID);

            Type r = role.GetType();
            Type c = classroom.GetType();
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
                        c.GetProperty(name).SetValue(classroom, oldValue);
                    }
                }
            }
            return classroom;
        }


        #endregion


        #region BUP
        public List<NameModel> GetClassroomsBySchool(string schoolName, string schoolEngageId, UserBaseEntity user)
        {
            return _contract.Classrooms.AsExpandable()
                .Where(r => r.School.Name == schoolName && r.School.SchoolId == schoolEngageId)
                .Where(GetRoleCondition(user))
                .Select(r => new NameModel
                {
                    EngageId = r.ClassroomId,
                    Name = r.Name
                }).ToList();
        }


        public List<NameModelWithSchool> GetClassroomsBySchools(List<string> schoolNames, List<string> schoolEngageIds, UserBaseEntity user)
        {
            return _contract.Classrooms.AsExpandable()
                .Where(r => schoolNames.Contains(r.School.Name) && schoolEngageIds.Contains(r.School.SchoolId))
                .Where(GetRoleCondition(user))
                .Select(r => new NameModelWithSchool
                {
                    SchoolName = r.School.Name,
                    SchoolEngageId = r.School.SchoolId,
                    EngageId = r.ClassroomId,
                    Name = r.Name
                }).ToList();
        }

        #endregion

    }
}
