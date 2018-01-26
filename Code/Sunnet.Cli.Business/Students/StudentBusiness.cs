using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
/**************************************************************************
 * Developer: 		Damon
 * Computer:		Damon-PC
 * Domain:			Damon-pc
 * CreatedOn:		2014/8/30 14:28:23
 * Description:		Please input class summary
 * Version History:	Created,2014/8/30 14:28:23
 **************************************************************************/
using Sunnet.Cli.Business.Ade;
using Sunnet.Cli.Business.Classes;
using Sunnet.Cli.Business.Communities;
using Sunnet.Cli.Business.Cpalls;
using Sunnet.Cli.Business.MasterData;
using Sunnet.Cli.Business.Schools;
using Sunnet.Cli.Business.Schools.Models;
using Sunnet.Cli.Business.Students.Models;
using Sunnet.Cli.Business.Users;
using Sunnet.Cli.Core;
using Sunnet.Cli.Core.Ade;
using Sunnet.Cli.Core.Classes;
using Sunnet.Cli.Core.Classes.Entites;
using Sunnet.Cli.Core.Common.Enums;
using Sunnet.Cli.Core.Communities.Entities;
using Sunnet.Cli.Core.Students.Enums;
using Sunnet.Cli.Core.Students.Model;
using Sunnet.Cli.Core.Users;
using Sunnet.Cli.Core.Users.Entities;
using Sunnet.Cli.Core.Users.Enums;
using Sunnet.Framework;
using Sunnet.Framework.Core.Tool;
using Sunnet.Cli.Core.Students.Entities;
using Sunnet.Cli.Core.Students;
using Sunnet.Cli.Business.Common;
using Sunnet.Framework.Core.Extensions;
using Sunnet.Framework.Extensions;
using Sunnet.Framework.Helpers;
using Sunnet.Framework.Log;
using Sunnet.Framework.PDF;
using Sunnet.Cli.Business.Cpalls.Models;
using LinqKit;
using Sunnet.Cli.Business.BUP.Model;
using Sunnet.Cli.Business.Cpalls.Group;
using Sunnet.Cli.Core.Schools.Entities;
using Sunnet.Cli.Core.Schools;
using Sunnet.Cli.Core.Communities;

namespace Sunnet.Cli.Business.Students
{
    public partial class StudentBusiness
    {
        private readonly IStudentContract _studentContract;
        private readonly IUserContract _userContract;
        private readonly ClassBusiness _classBusiness;
        private readonly SchoolBusiness _schoolBusiness;
        private readonly CommunityBusiness _communityBusiness;

        public StudentBusiness(EFUnitOfWorkContext unit = null)
        {
            _studentContract = DomainFacade.CreateStudentService(unit);
            _userContract = DomainFacade.CreateUserService(unit);
            _classBusiness = new ClassBusiness(unit);
            _communityBusiness = new CommunityBusiness(unit);
            _schoolBusiness = new SchoolBusiness(unit);
        }


        public StudentEntity GetStudentByCode(string parentId, string firstName, string lastName)
        {
            return _studentContract.Students.Where(e =>
                e.ParentCode == parentId
                && e.FirstName == firstName
                && e.LastName == lastName).FirstOrDefault();
        }

        public StudentEntity GetStudentByPIN(string pinCode, string firstName, string lastName, DateTime birthdate)
        {
            return _studentContract.Students.FirstOrDefault(e =>
                e.ParentCode == pinCode
                && e.FirstName == firstName
                && e.LastName == lastName
                && e.BirthDate == birthdate);
        }

        public StudentEntity GetStudent(string firstName, string lastName, DateTime birthdate)
        {
            return _studentContract.Students.FirstOrDefault(e =>
                e.FirstName == firstName
                && e.LastName == lastName
                && e.BirthDate == birthdate);
        }

        public StudentEntity GetStudentByCodeOrSchool(StudentListModel parentChild, int schoolId)
        {
            Expression<Func<StudentEntity, bool>> expression = o => true;
            expression = PredicateBuilder.And(expression,
                e => e.ParentCode == parentChild.ParentId || e.SchoolRelations.Any(s => s.SchoolId == schoolId));
            expression = PredicateBuilder.And(expression, e => e.FirstName == parentChild.ChildFirstName);
            expression = PredicateBuilder.And(expression, e => e.LastName == parentChild.ChildLastName);
            expression = PredicateBuilder.And(expression, e => e.BirthDate == parentChild.BirthDate);
            if (parentChild.PrimaryLanguageId > 0)
            {
                expression = PredicateBuilder.And(expression, e => e.PrimaryLanguageId == parentChild.PrimaryLanguageId);
            }
            if ((int)parentChild.GradeLevel > 0)
            {
                expression = PredicateBuilder.And(expression, e => e.GradeLevel == parentChild.GradeLevel);
            }
            var list = _studentContract.Students.AsExpandable().Where(expression).ToList();
            return list.FirstOrDefault();
        }

        public OperationResult InsertStudent(UserBaseEntity user, StudentEntity entity, int[] chkClasses)
        {
            Random rad = new Random();
            entity.ParentCode = rad.Next(1, 100000000).ToString("00000000");
            if (chkClasses != null)
            {
                entity.Classes = new List<ClassEntity>();
                foreach (var item in chkClasses)
                {
                    ClassEntity classEntity = _classBusiness.GetClass(item);
                    entity.Classes.Add(classEntity);
                }
            }
            entity = InitStudentByRole(entity, null, user.Role);


            return _studentContract.InsertStudent(entity);
        }
        public OperationResult InsertStudent(int communityId, UserBaseEntity user, StudentEntity entity, int[] chkClasses)
        {
            Random rad = new Random();
            entity.ParentCode = rad.Next(1, 100000000).ToString("00000000");
            if (chkClasses != null)
            {
                entity.Classes = new List<ClassEntity>();
                foreach (var item in chkClasses)
                {
                    ClassEntity classEntity = _classBusiness.GetClass(item);
                    entity.Classes.Add(classEntity);
                }
            }
            entity = InitStudentByRole(entity, null, user.Role);

            StudentEntity findEntity = GetStudent(communityId, entity.FirstName, entity.LastName, entity.BirthDate);
            if (findEntity != null)
            {
                if (findEntity.ID != entity.ID)
                {

                    OperationResult res = new OperationResult(OperationResultType.Success);
                    res.ResultType = OperationResultType.Error;
                    res.Message = findEntity.FirstName + " " + findEntity.LastName + " " + findEntity.StudentId + "exists.";
                }
            }
            return _studentContract.InsertStudent(entity);
        }
        public StudentEntity GetStudent(int communityId, string firstName, string lastName, DateTime birthDay)
        {
            return _studentContract.Students.FirstOrDefault(
                 o =>
                     o.SchoolRelations.Any(m => m.School.CommunitySchoolRelations.Any(n => n.CommunityId == communityId))
                     && o.FirstName == firstName && o.LastName == lastName && o.BirthDate == birthDay);
        }

        public bool GetStudentByCommunityIdAndLocalId(int communityId, string localId)
        {
            return _studentContract.Students.Any(
                o =>
                    o.SchoolRelations.Any(m => m.School.CommunitySchoolRelations.Any(n => n.CommunityId == communityId))
                    && o.LocalStudentID == localId);
        }

        public StudentEntity GetStudent(string firstName, string lastName, DateTime birthDay,
            string localStudentId)
        {
            return _studentContract.Students.FirstOrDefault(
                o => o.FirstName == firstName && o.LastName == lastName && o.BirthDate == birthDay &&
                     (o.LocalStudentID == localStudentId || localStudentId == ""));
        }

        public StudentEntity GetStudent(int id)
        {
            return _studentContract.GetStudent(id);
        }

        public StudentModelForQueue GetStudentForQueue(int id)
        {
            return _studentContract.Students.Where(c => c.ID == id).Select(s => new StudentModelForQueue() { ID = id, BirthDate = s.BirthDate, SchoolYear = s.SchoolYear }).FirstOrDefault();
        }

        public List<StudentEntity> GetStudentsByIds(List<int> stuIds)
        {
            return _studentContract.Students.Where(x => stuIds.Contains(x.ID) && x.IsDeleted == false).ToList();
        }

        public OperationResult UpdateStudent(UserBaseEntity user, StudentEntity entity, int[] chkClasses)
        {
            StudentEntity oldEntity = _studentContract.GetStudent(entity.ID);
            if (oldEntity.Classes.Any())
            {
                while (oldEntity.Classes.Any())
                    oldEntity.Classes.Remove(oldEntity.Classes.FirstOrDefault());
            }
            if (chkClasses != null)
            {
                foreach (var item in chkClasses)
                {
                    ClassEntity classEntity = _classBusiness.GetClass(item);
                    oldEntity.Classes.Add(classEntity);
                    if (_schoolBusiness.CheckSchooStudentRelations(classEntity.SchoolId, entity.ID) == false)
                    {
                        _schoolBusiness.InserSchoolStudentRelations(new List<int>() { entity.ID }, classEntity.SchoolId, user);
                    }
                }
            }

            entity = InitStudentByRole(entity, oldEntity, user.Role);
            _studentContract.UpdateStudent(oldEntity);
            var result = _studentContract.UpdateStudent(entity);
            //TODO:Delete SchoolId in StudentEntity
            //if (result.ResultType == OperationResultType.Success)
            //    CpallsBusiness.UpdateMeasureCache(entity.SchoolId, entity.ID);
            return result;
        }

        public OperationResult UpdateStudents(List<StudentEntity> entities)
        {
            return _studentContract.UpdateStudents(entities);
        }

        public List<StudentClassModel> GetClassStudentList(Expression<Func<StudentEntity, bool>> condition, UserBaseEntity userInfo
            , string sort, string order, int first, int count, out int total)
        {
            var query = _studentContract.Students.AsExpandable().Where(GetRoleCondition(userInfo)).Where(condition)
                .Select(r => new StudentClassModel()
                {
                    StudentId = r.ID,
                    Name = r.FirstName + " " + r.LastName,
                    StudentStatus = r.Status
                });
            total = query.Count();
            return query.OrderBy(sort, order).Skip(first).Take(count).ToList();
        }

        public List<SelectItemModel> GetStudentList(UserBaseEntity userInfo)
        {
            var query = _studentContract.Students.AsExpandable().Where(GetRoleCondition(userInfo))
                .Select(r => new SelectItemModel()
                {
                    ID = r.ID,
                    Name = r.FirstName + " " + r.LastName
                });
            return query.ToList();
        }

        public List<StudentModel> GetStudentsGetIds(List<int> studentIds)
        {
            return _studentContract.Students.Where(e => studentIds.Contains(e.ID)).Select(e => new StudentModel()
            {
                ID = e.ID,
                SchoolNameList = e.SchoolRelations.Select(x => x.School.Name),
                StudentName = e.FirstName + " " + e.LastName,
                BirthDate = e.BirthDate
            }).ToList();
        }

        public List<ParentStudentListModel> GetStudentsForParentActivities(List<int> studentIds)
        {
            return
                _studentContract.Students.Where(e => studentIds.Contains(e.ID)).Select(e => new ParentStudentListModel()
                {
                    ChildFirstName = e.FirstName,
                    ChildLastName = e.LastName,
                    StudentId = e.ID,
                    BirthDate = e.BirthDate,
                    ParentPrimaryEmail =
                        e.ParentStudents.FirstOrDefault() != null
                            ? e.ParentStudents.FirstOrDefault().Parent.UserInfo.PrimaryEmailAddress
                            : "",
                }).OrderBy(s => s.ChildLastName).ToList();
        }

        public List<int> GetStudentsByClassIds(List<int> classIds)
        {
            List<int> listStudentIds = new List<int>();
            IList<ClassEntity> classList = _classBusiness.GetClassList(o => classIds.Contains(o.ID));
            foreach (ClassEntity classEntity in classList)
            {
                foreach (StudentEntity studentEntity in classEntity.Students.Where(e => e.IsDeleted == false))
                {
                    if (!listStudentIds.Contains(studentEntity.ID))
                        listStudentIds.Add(studentEntity.ID);
                }
            }
            return listStudentIds;
        }

        /// <summary>
        ///  language == Bilingual 时 获取所有学生
        /// </summary>
        public List<int> GetStudentIdListByClassIdList(int classId, StudentAssessmentLanguage language, DateTime dobStart, DateTime dobEnd)
        {
            if (language != StudentAssessmentLanguage.Bilingual)
            {
                return _studentContract.Students.Where(o => o.Classes.Where(e => e.IsDeleted == false).Select(c => c.ID).Contains(classId)
                                                            &&
                                                            (o.AssessmentLanguage == language ||
                                                             o.AssessmentLanguage == StudentAssessmentLanguage.Bilingual)
                                                            && o.Status == EntityStatus.Active
                                                             && o.BirthDate >= dobStart && o.BirthDate <= dobEnd)
                    .Select(x => x.ID)
                    .ToList();
            }
            else
            {
                return _studentContract.Students.Where(o => o.Classes.Where(e => e.IsDeleted == false).Select(c => c.ID).Contains(classId)
                && o.Status == EntityStatus.Active && o.AssessmentLanguage > 0)
                .Select(x => x.ID)
                .ToList();
            }
        }

        /// <summary>
        /// 只返回指定枚举类型的学生
        /// </summary>
        /// <param name="classId"></param>
        /// <param name="language"></param>
        /// <returns></returns>
        public List<int> GetStudentIdsByClassId(int classId, StudentAssessmentLanguage language)
        {
            //return _studentContract.Students.Where(r => r.Classes.Where(e => e.IsDeleted == false).Select(c => c.ID).Contains(classId)
            //    && r.AssessmentLanguage == language && r.Status == EntityStatus.Active)
            //    .Select(r => r.ID).ToList();

            return _studentContract.Students
               .Where(s => s.Classes.Count(c => c.ID == classId && c.IsDeleted == false) > 0 && s.AssessmentLanguage == language && s.Status == EntityStatus.Active)
               .Select(s => s.ID).ToList();
        }

        public List<int> GetStudentIdsByDOB(int classId, StudentAssessmentLanguage language, DateTime dobStart, DateTime dobEnd)
        {
            return _studentContract.Students
               .Where(s => s.Classes.Count(c => c.ID == classId && c.IsDeleted == false) > 0
               && (s.AssessmentLanguage == language || s.AssessmentLanguage == StudentAssessmentLanguage.Bilingual)
               && s.Status == EntityStatus.Active
               && s.BirthDate >= dobStart && s.BirthDate <= dobEnd)
               .Select(s => s.ID).ToList();
        }

        public List<int> GetStudentIdsByDOB(int classId, DateTime dobStart, DateTime dobEnd)
        {
            return _studentContract.Students
               .Where(s => s.Classes.Count(c => c.ID == classId && c.IsDeleted == false) > 0
               && s.Status == EntityStatus.Active
               && s.BirthDate >= dobStart && s.BirthDate <= dobEnd)
               .Select(s => s.ID).ToList();
        }

        public List<int> GetStudentIdsByClassId(int classId)
        {
            return _studentContract.Students
                .Where(s => s.Classes.Count(c => c.ID == classId && c.IsDeleted == false) > 0 && s.Status == EntityStatus.Active)
                .Select(s => s.ID).ToList();
        }
        public List<int> GetStudentIdsByClassId(IList<int> classIds, DateTime dobStartDate, DateTime dobEndDate)
        {
            return _studentContract.Students
                .Where(s => s.Classes.Any(c => classIds.Contains(c.ID) && c.IsDeleted == false)
                            && s.BirthDate >= dobStartDate && s.BirthDate <= dobEndDate)
                .Select(s => s.ID).ToList();
        }


        public List<int> GetStudentsBySchoolIds(params int[] schoolIds)
        {
            return _studentContract.Students
                .Where(o => o.SchoolRelations.Any(p => schoolIds.Contains(p.SchoolId)))
                .Select(o => o.ID).ToList();
        }
        public List<int> GetStudentsForTSDSBySchoolIds(DateTime? dobStartDate, DateTime? dobEndDate, params int[] schoolIds)
        {
            return _studentContract.Students
                .Where(o => o.GradeLevel == StudentGradeLevel.Prek && o.SchoolRelations.Any(p => schoolIds.Contains(p.SchoolId)))
                .WhereIf(o => o.BirthDate > dobStartDate, dobStartDate != null)
                .WhereIf(o => o.BirthDate < dobEndDate, dobEndDate != null)
                .Select(o => o.ID).ToList();
        }
        public List<StudentEntity> GetStudentsBySchoolIds(List<string> schoolEngageIds)
        {
            return _studentContract.Students
                .Where(o => o.SchoolRelations.Any(p => schoolEngageIds.Contains(p.School.SchoolId))).ToList();
        }

        public List<int> GetStudentsBySchoolIdCommId(List<int> communityIds, int schoolId = 0)
        {

            Expression<Func<StudentEntity, bool>> condition = r => true;
            if (communityIds.Count > 0)
                condition = PredicateBuilder.And(condition, r => r.SchoolRelations.Any(s => s.School.CommunitySchoolRelations.Any(c => communityIds.Contains(c.CommunityId))));
            if (schoolId > 0)
                condition = PredicateBuilder.And(condition, r => r.SchoolRelations.Any(s => s.SchoolId == schoolId));

            return _studentContract.Students.AsExpandable().Where(condition)
                .Select(r => r.ID).ToList();
        }

        public StudentEntity GetEntityByModel(StudentModel model)
        {
            StudentEntity studentEntity = new StudentEntity();
            //studentEntity.SchoolRelations.Select(r => r.School);

            studentEntity.StudentId = model.StudentId;
            studentEntity.FirstName = model.FirstName;
            studentEntity.MiddleName = model.MiddleName;
            studentEntity.LastName = model.LastName;
            studentEntity.Status = model.Status;
            studentEntity.StatusDate = model.StatusDate;
            studentEntity.SchoolYear = model.SchoolYear;
            studentEntity.BirthDate = model.BirthDate;
            studentEntity.Gender = model.Gender;
            studentEntity.Ethnicity = model.Ethnicity;
            studentEntity.EthnicityOther = model.EthnicityOther;
            studentEntity.PrimaryLanguageId = model.PrimaryLanguageId;
            studentEntity.PrimaryLanguageOther = model.PrimaryLanguageOther;
            studentEntity.SecondaryLanguageId = model.SecondaryLanguageId;
            studentEntity.SecondaryLanguageOther = model.SecondaryLanguageOther;
            studentEntity.IsSendParent = model.IsSendParent;
            studentEntity.IsMediaRelease = model.IsMediaRelease;
            studentEntity.Notes = model.Notes;
            studentEntity.Classes = model.Classes;
            studentEntity.AssessmentLanguage = model.AssessmentLanguage;
            studentEntity.LocalStudentID = model.LocalStudentID;
            studentEntity.TSDSStudentID = model.TSDSStudentID;
            studentEntity.GradeLevel = model.GradeLevel;
            studentEntity.ParentCode = model.ParentCode;
            return studentEntity;
        }

        public StudentModel GetStudent(int id, UserBaseEntity user)
        {
            StudentModel model = _studentContract.Students.AsExpandable()
                .Where(o => o.ID == id).Where(GetRoleCondition(user))
                .Select(EntityToModelForEdit).FirstOrDefault();
            //SchoolIdList对应的Community
            if (model != null && model.SchoolIdList != null)
            {
                IEnumerable<string> communityNameList = _communityBusiness.GetCommunityNames(r => r.CommunitySchoolRelations.Any(s => model.SchoolIdList.Contains(s.SchoolId)));

                model.CommunityName = string.Join(", ", communityNameList);
            }
            return model;
        }
        public StudentEntity GetStudentEntity(int id, UserBaseEntity user)
        {
            return _studentContract.Students.AsExpandable()
                .Where(o => o.ID == id).Where(GetRoleCondition(user)).FirstOrDefault();
        }

        public List<StudentModel> SearchStudents(UserBaseEntity userInfo,
            Expression<Func<StudentEntity, bool>> condition,
            string sort, string order, int first, int count, out int total)
        {
            // condition = PredicateBuilder.And(condition, GetRoleCondition(userInfo));
            var query =
                _studentContract.Students.AsExpandable()
                    .Where(condition)
                    .Where(GetRoleCondition(userInfo))
                    .Select(EntityToModel);
            total = query.Count();
            var list = query.OrderBy(sort, order).Skip(first).Take(count);
            return list.ToList();
        }

        public List<ParentStudentListModel> SearchStudentsToAddParents(UserBaseEntity userInfo,
            Expression<Func<StudentEntity, bool>> condition,
            string sort, string order, int first, int count, out int total)
        {
            var query = from a in _studentContract.Students.AsExpandable()
                .Where(condition)
                .Where(GetRoleCondition(userInfo)).ToList()
                        join c in _userContract.ParentStudentRelations
                             on a.ID equals c.StudentId into gc
                        from b in gc.DefaultIfEmpty()
                        select new ParentStudentListModel
                        {
                            StudentId = a.ID,
                            ChildFirstName = a.FirstName,
                            ChildLastName = a.LastName,
                            BirthDate = a.BirthDate,
                            ParentCode = a.ParentCode,
                            ID = b != null ? b.ID : 0,
                            ParentId = b != null ? b.ParentId : 0,
                            ParentFirstName = b != null ? b.Parent.UserInfo.FirstName : "",
                            ParentLastName = b != null ? b.Parent.UserInfo.LastName : "",
                            ParentPrimaryEmail = b != null ? b.Parent.UserInfo.PrimaryEmailAddress : "",
                            Color = "",
                            Goal = ""
                        };

            //var query =
            //    _studentContract.Students.AsExpandable()
            //        .Where(condition)
            //        .Where(GetRoleCondition(userInfo))
            //        .Select(e => new ParentStudentListModel
            //        {
            //            StudentId = e.ID,
            //            ChildFirstName = e.FirstName,
            //            ChildLastName = e.LastName,
            //            BirthDate = e.BirthDate
            //        });
            total = query.Count();
            var list = query.OrderBy(sort, order).Skip(first).Take(count);
            return list.ToList();
        }

        //查找Parent所对应的student
        public List<StudentModel> SearchStudents(int parentid, string sort, string order, int first, int count,
            out int total)
        {
            List<int> studentIds = new UserBusiness().GetStudentIDbyParentId(parentid);
            var query = _studentContract.Students.Where(a => studentIds.Contains(a.ID)).Select(EntityToModel);
            total = query.Count();
            var list = query.OrderBy(sort, order).Skip(first).Take(count);
            return list.ToList();
        }

        public List<ChildListModel> SearchParentChilds(int parentid, string sort, string order, int first, int count,
            out int total)
        {
            var query = _studentContract.ParentChilds.Where(x => x.ParentId == parentid).Select(
                e => new ChildListModel()
                {
                    ID = e.ChildId,
                    FirstName = e.Child.FirstName,
                    LastName = e.Child.LastName,
                    BirthDate = e.Child.BirthDate,
                    PINCode = e.Child.PINCode,

                    StudentId = e.Child.StudentId
                });
            total = query.Count();
            var list = query.OrderBy(sort, order).Skip(first).Take(count);
            return list.ToList();
        }

        public Expression<Func<StudentEntity, bool>> GetRoleCondition(UserBaseEntity userInfo)
        {
            Expression<Func<StudentEntity, bool>> condition = o => true;
            if (userInfo == null)
                return condition;
            UserBusiness userBusiness = new UserBusiness();

            UserBaseEntity baseUser = userBusiness.GetUser(userInfo.ID);
            List<int> basicComIds = baseUser.UserCommunitySchools.Where(o => o.CommunityId > 0).Select(o => o.Community.BasicCommunityId).ToList();
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
                    condition = PredicateBuilder.And(condition,
                        c => c.Classes.Any(
                                r => r.School.CommunitySchoolRelations.Any(o => comIds.Contains(o.CommunityId)) && r.IsDeleted == false)
                                );
                    break;


                case Role.Teacher: //与teacher关联的class 的student

                    if (baseUser.TeacherInfo != null)
                    {
                        if (baseUser.TeacherInfo.Classes.Where(e => e.IsDeleted == false).ToList().Count != 0)
                        {
                            condition = o => false;
                            baseUser.TeacherInfo.Classes.Where(e => e.IsDeleted == false).ForEach(o =>
                            {
                                condition = PredicateBuilder.Or(condition,
                                    m => m.Classes.Where(e => e.IsDeleted == false).Select(x => x.ID).Contains(o.ID));
                            });
                        }
                        else condition = o => false;
                    }
                    else condition = o => false;
                    break;
                case Role.Principal: //当前school的所有学生
                case Role.TRS_Specialist:
                    condition = PredicateBuilder.And(condition, r => r.SchoolRelations.Any(s => s.School.UserCommunitySchools.Any(u => u.UserId == baseUser.ID)));
                    break;
                case Role.School_Specialist://根据已分配的class找学生，没分配，同Principal
                    if (_studentContract.Students.Count(s => s.Classes.Any(c => c.UserClasses.Any(u => u.UserId == baseUser.ID) && c.IsDeleted == false)) == 0)
                        condition = PredicateBuilder.And(condition, r => r.SchoolRelations.Any(s => s.School.UserCommunitySchools.Any(u => u.UserId == baseUser.ID)));
                    else
                        condition = PredicateBuilder.And(condition, r => r.Classes.Any(c => c.UserClasses.Any(u => u.UserId == baseUser.ID) && c.IsDeleted == false));
                    break;

                case Role.Principal_Delegate:
                case Role.TRS_Specialist_Delegate:
                case Role.School_Specialist_Delegate:
                    condition = PredicateBuilder.And(condition, r => r.SchoolRelations.Any(s => s.School.UserCommunitySchools.Any(u => u.UserId == baseUser.Principal.ParentId)));
                    break;
                case Role.Community: //当前community下的所有学生

                    condition = PredicateBuilder.And(
                                 condition, c =>
                                     c.Classes.Any(r => (basicComIds.Contains(r.School.BasicSchool.CommunityId)) && c.IsDeleted == false
                                     )
                          ||
                            (c.Classes.Any(r => r.School.CommunitySchoolRelations.Any(o => comIds.Contains(o.CommunityId))
                                 && r.UserClasses.Any(u => u.UserId == baseUser.ID) && r.IsDeleted == false)
                            )
                     );


                    break;
                case Role.District_Community_Specialist://根据已分配的class找学生
                    // condition = PredicateBuilder.And(condition, r => r.Classes.Any(c => c.UserClasses.Any(u => u.UserId == baseUser.ID)));

                    condition = PredicateBuilder.And(
                                  condition, c => c.Classes.Any(r => (basicComIds.Contains(r.School.BasicSchool.CommunityId)) && r.IsDeleted == false
                                )
                           ||
                             (//  r.School.CommunitySchoolRelations.Any(c => comIds.Contains(c.CommunityId)) &&
                                c.Classes.Any(r => r.School.CommunitySchoolRelations.Any(o => comIds.Contains(o.CommunityId))
                                 && r.UserClasses.Any(u => u.UserId == baseUser.ID) && r.IsDeleted == false)
                             )
                      );
                    break;
                case Role.District_Community_Delegate:
                case Role.Community_Specialist_Delegate:
                    condition = PredicateBuilder.And(condition, r => r.SchoolRelations.Any(s => s.School.CommunitySchoolRelations
                        .Any(c => c.Community.UserCommunitySchools.Any(u => u.UserId == baseUser.CommunityUser.ParentId))));
                    break;
                case Role.Statewide:
                    //condition = PredicateBuilder.And(condition, r => r.SchoolRelations.Any(s => s.School.CommunitySchoolRelations
                    //  .Any(c => c.Community.UserCommunitySchools.Any(u => u.UserId == baseUser.ID))));
                    condition = PredicateBuilder.And(
                                condition, c => c.Classes.Any(r => (basicComIds.Contains(r.School.BasicSchool.CommunityId)) && r.IsDeleted == false
                              )
                         ||
                           (
                               c.Classes.Any(r => r.School.CommunitySchoolRelations.Any(o => comIds.Contains(o.CommunityId))
                                 && r.UserClasses.Any(u => u.UserId == baseUser.ID) && r.IsDeleted == false)
                           )
                    );
                    break;
                case Role.Parent:
                    List<int> studentIds = new UserBusiness().GetStudentIDbyParentId(userInfo.Parent.ID);
                    condition = PredicateBuilder.And(condition, c => studentIds.Contains(c.ID));
                    break;
            }
            return condition;
        }

        public IList<int> GetClassIdsByStudentId(int studentId)
        {
            return _studentContract.GetStudent(studentId).Classes.Where(e => e.IsDeleted == false).Select(o => o.ID).ToList();
        }

        public IList<int> GetSchoolIdsByStudentId(int studentId)
        {
            return _studentContract.GetStudent(studentId).SchoolRelations.Select(o => o.SchoolId).ToList();
        }

        public StudentEntity NewStudentEntity()
        {
            StudentEntity student = _studentContract.NewStudentEntity();
            student.MiddleName = "";
            student.SchoolYear = CommonAgent.SchoolYear;
            student.StatusDate = DateTime.Now;
            student.IsMediaRelease = MediaRelease.No;
            student.Notes = "";
            student.StudentId = string.Empty;
            student.EthnicityOther = "";
            student.LocalStudentID = "";
            student.ParentCode = "";
            student.PrimaryLanguageOther = "";
            student.SecondaryLanguageOther = "";
            student.TSDSStudentID = "";
            return student;
        }

        public StudentModel NewStudentModel()
        {
            StudentModel student = new StudentModel();
            student.SchoolYear = CommonAgent.SchoolYear;
            student.StatusDate = DateTime.Now;
            student.IsMediaRelease = MediaRelease.No;
            student.Notes = "";
            student.StudentId = string.Empty;
            student.EthnicityOther = "";
            student.LocalStudentID = "";
            student.ParentCode = "";
            student.PrimaryLanguageOther = "";
            student.SecondaryLanguageOther = "";
            student.TSDSStudentID = "";
            return student;
        }

        private static Expression<Func<StudentEntity, StudentModel>> EntityToModel
        {
            get
            {
                return x => new StudentModel()
                {
                    ID = x.ID,
                    StudentId = x.StudentId,
                    FirstName = x.FirstName,
                    MiddleName = x.MiddleName,
                    BirthDate = x.BirthDate,
                    Gender = x.Gender,
                    Ethnicity = x.Ethnicity,
                    AssessmentLanguage = x.AssessmentLanguage,
                    LastName = x.LastName,
                    Status = x.Status,
                    ParentCode = x.ParentCode,
                    LocalStudentID = x.LocalStudentID,
                    TSDSStudentID = x.TSDSStudentID,
                    GradeLevel = x.GradeLevel
                };
            }
        }

        private static Expression<Func<StudentEntity, StudentModel>> EntityToModelForEdit
        {
            get
            {
                return x => new StudentModel()
                {
                    ID = x.ID,
                    SchoolId = x.SchoolRelations.Select(r => r.SchoolId).FirstOrDefault(),
                    SchoolNameList = x.SchoolRelations.Select(r => r.School.Name),
                    SchoolIdList = x.SchoolRelations.Select(r => r.SchoolId),
                    FirstSchoolName = x.SchoolRelations.Select(r => r.School.Name).FirstOrDefault(),
                    StudentId = x.StudentId,
                    FirstName = x.FirstName,
                    MiddleName = x.MiddleName,
                    LastName = x.LastName,
                    Status = x.Status,
                    StatusDate = x.StatusDate,
                    SchoolYear = x.SchoolYear,
                    BirthDate = x.BirthDate,
                    Gender = x.Gender,
                    Ethnicity = x.Ethnicity,
                    EthnicityOther = x.EthnicityOther,
                    PrimaryLanguageId = x.PrimaryLanguageId,
                    PrimaryLanguageOther = x.PrimaryLanguageOther,
                    SecondaryLanguageId = x.SecondaryLanguageId,
                    SecondaryLanguageOther = x.SecondaryLanguageOther,
                    IsSendParent = x.IsSendParent,
                    IsMediaRelease = x.IsMediaRelease,
                    Notes = x.Notes,
                    CreatedOn = x.CreatedOn,
                    UpdatedOn = x.UpdatedOn,
                    Classes = x.Classes,
                    ParentCode = x.ParentCode,
                    AssessmentLanguage = x.AssessmentLanguage,
                    LocalStudentID = x.LocalStudentID,
                    TSDSStudentID = x.TSDSStudentID,
                    GradeLevel = x.GradeLevel
                };
            }
        }

        private static Expression<Func<StudentModel, StudentEntity>> ModelToEntity
        {
            get
            {
                return x => new StudentEntity()
                {
                    ID = x.ID,
                    StudentId = x.StudentId,
                    FirstName = x.FirstName,
                    MiddleName = x.MiddleName,
                    LastName = x.LastName,
                    Status = x.Status,
                    StatusDate = x.StatusDate,
                    SchoolYear = x.SchoolYear,
                    BirthDate = x.BirthDate,
                    Gender = x.Gender,
                    Ethnicity = x.Ethnicity,
                    EthnicityOther = x.EthnicityOther,
                    PrimaryLanguageId = x.PrimaryLanguageId,
                    PrimaryLanguageOther = x.PrimaryLanguageOther,
                    SecondaryLanguageId = x.SecondaryLanguageId,
                    SecondaryLanguageOther = x.SecondaryLanguageOther,
                    IsSendParent = x.IsSendParent,
                    IsMediaRelease = x.IsMediaRelease,
                    Notes = x.Notes,
                    CreatedOn = x.CreatedOn,
                    UpdatedOn = x.UpdatedOn,
                    Classes = x.Classes,
                    AssessmentLanguage = x.AssessmentLanguage,
                    LocalStudentID = x.LocalStudentID,
                    TSDSStudentID = x.TSDSStudentID,
                    GradeLevel = x.GradeLevel
                };
            }
        }

        #region

        private string GenerateEmailContent(StudentModel student, TeacherEntity teacher)
        {
            string emailTemplateName = "ParentInvitation_Template.xml";
            EmailTemplete template = XmlHelper.GetEmailTemplete(emailTemplateName);
            string emailBody = "";
            emailBody = template.Body.Replace("[Student Name]", student.FirstName + " " + student.LastName)
                .Replace("[Teacher Name]", teacher.UserInfo.FirstName)
                .Replace("[Domain]", SFConfig.StaticDomain);
            return emailBody;

        }

        public OperationResult GeneratePDF(int id, UserBaseEntity userEntity)
        {
            StudentModel student = GetStudent(id, userEntity);
            string emailPDFTemplateName = "ParentInvitationPDF_Template.xml";
            EmailTemplete template = XmlHelper.GetEmailTemplete(emailPDFTemplateName);
            string pdfContent = "";
            pdfContent = template.Body.Replace("[firstName]", student.FirstName)
                .Replace("[lastName]", student.LastName)
                .Replace("[parentCode]", student.ParentCode)
                .Replace("[staticdomain]", SFConfig.StaticDomain);
            return ParentInvitation(pdfContent, student);
        }

        public void GenerateListPDF(List<List<string>> studentList)
        {
            string pdfContent = "";
            string emailPDFTemplateName = "ParentInvitationPDF_Template.xml";
            EmailTemplete template = XmlHelper.GetEmailTemplete(emailPDFTemplateName);
            string templateContent = template.Body;
            for (int i = 0; i < studentList.Count(); i++)
            {
                pdfContent += templateContent.Replace("[firstName]", studentList[i][0])
                    .Replace("[lastName]", studentList[i][1])
                    .Replace("[parentCode]", studentList[i][2])
                    .Replace("[staticdomain]", SFConfig.StaticDomain);
            }
            string fileFullName = "Students Parent Invitations.pdf";
            PdfProvider pdfProvider = new PdfProvider();
            pdfProvider.GeneratePDF(pdfContent, fileFullName);
        }

        private OperationResult ParentInvitation(string contentHtml, StudentModel student)
        {
            int teacherCount = 0;
            var result = new OperationResult(OperationResultType.Success);
            try
            {
                PdfProvider pdfProvider = new PdfProvider();

                byte[] pdfBytes = pdfProvider.GetPdfBytes(contentHtml);
                string fileFullName = SFConfig.TempFile + student.ParentCode + " " + student.FirstName + " " +
                                      student.LastName + ".pdf";
                SaveFile(pdfBytes, fileFullName);

                if (student.Classes != null)
                {
                    foreach (ClassEntity classEntity in student.Classes.Where(e => e.IsDeleted == false))
                    {
                        if (classEntity.Teachers.Any(e => e.UserInfo.IsDeleted == false))
                        {
                            foreach (TeacherEntity teacherEntity in classEntity.Teachers.Where(e => e.UserInfo.IsDeleted == false))
                            {
                                teacherCount++;
                                string emailSubject = "Parent Invitation for " + student.FirstName + " " +
                                                      student.LastName;
                                string emailTo = teacherEntity.UserInfo.PrimaryEmailAddress;
                                string content = GenerateEmailContent(student, teacherEntity);
                                result = _studentContract.InvitationEmail(emailTo, "Cli support", emailSubject, content,
                                    fileFullName);
                            }
                        }
                    }
                }
                if (teacherCount == 0)
                {
                    result.Message = "No teacher has been found for this student.";
                    result.ResultType = OperationResultType.Warning;
                }
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;

            }

            return result;

        }





        private void SaveFile(byte[] pdfBytes, string fileName)
        {
            FileStream fs = new FileStream(fileName, FileMode.Create, FileAccess.Write, FileShare.None);
            int rbuffer = pdfBytes.Length;
            fs.Write(pdfBytes, 0, rbuffer);
            fs.Close();
        }

        #endregion

        #region Cpalls Methods

        /// <summary>
        /// 获取一个活动的学生 CPALSS+用到
        /// </summary>
        /// <returns></returns>
        public CpallsStudentModel GetStudentTopOne(Expression<Func<StudentEntity, bool>> condition)
        {
            return _studentContract.Students.AsExpandable().Where(condition)
                .Select(SelectorStudentEntityToCpallsModel).FirstOrDefault();
        }


        public CpallsStudentModel GetStudentModelForPlayer(int id, UserBaseEntity user)
        {
            var student = _studentContract.GetCpallsStudentModelForPlayMeasure(id);
            var studentModel = new CpallsStudentModel();
            if (student != null)
            {
                studentModel.ID = student.ID;
                studentModel.FirstName = student.FirstName;
                studentModel.LastName = student.LastName;
                studentModel.BirthDate = student.BirthDate;
                studentModel.ParentCode = student.ParentCode;
                studentModel.SchoolId = student.SchoolId;
            }
            return studentModel;
        }

        public CpallsStudentModel GetStudentModel(int id, UserBaseEntity user)
        {
            return _studentContract.Students.AsExpandable().Where(GetRoleCondition(user)).Where(r => r.ID == id
                                                                                                     &&
                                                                                                     r.Status ==
                                                                                                     EntityStatus.Active)
                .Select(SelectorStudentEntityToCpallsModel)
                .FirstOrDefault();
        }


        public List<CpallsStudentModel> GetClassStudents(Expression<Func<StudentEntity, bool>> condition,
            UserBaseEntity userInfo
            , string sort, string order, int first, int count, out int total)
        {
            var query = _studentContract.Students.AsExpandable().Where(condition).Where(GetRoleCondition(userInfo)).OrderBy(sort, order)
                .Select(SelectorStudentEntityToCpallsModel);

            total = query.Count();
            var list = query.Skip(first).Take(count);
            return list.ToList();
        }

        private static Expression<Func<StudentEntity, CpallsStudentModel>> SelectorStudentEntityToCpallsModelForPlayMeasure
        {
            get
            {
                return r => new CpallsStudentModel()
                {
                    ID = r.ID,
                    FirstName = r.FirstName,
                    LastName = r.LastName,
                    BirthDate = r.BirthDate,
                    ParentCode = r.ParentCode,
                    Schools = r.SchoolRelations.Select(s => new BasicSchoolModel()
                    {
                        ID = s.SchoolId
                    })
                };
            }
        }

        private static Expression<Func<StudentEntity, CpallsStudentModel>> SelectorStudentEntityToCpallsModel
        {
            get
            {
                return r => new CpallsStudentModel()
                {
                    ID = r.ID,
                    FirstName = r.FirstName,
                    LastName = r.LastName,
                    BirthDate = r.BirthDate,
                    ParentCode = r.ParentCode,
                    Schools = r.SchoolRelations.Select(s => new BasicSchoolModel()
                    {
                        ID = s.School.ID,
                        Name = s.School.Name,
                        Communities = s.School.CommunitySchoolRelations.Select(c => c.Community.Name)
                    })
                };
            }
        }

        public IEnumerable<int> GetStudentId(Expression<Func<StudentEntity, bool>> condition, UserBaseEntity userInfo, string sort, string order, int first, int count, out int total)
        {
            var query = _studentContract.Students.AsExpandable().Where(condition).Where(GetRoleCondition(userInfo)).OrderBy(sort, order)
                .Select(r => r.ID);

            total = query.Count();
            var list = query.Skip(first).Take(count);
            return list.ToList();
        }

        public IEnumerable<int> GetStudentIdForCpallsGoalSort(Expression<Func<StudentEntity, bool>> condition, UserBaseEntity userInfo, out int total)
        {
            var query = _studentContract.Students.AsExpandable().Where(condition).Where(GetRoleCondition(userInfo)).Select(r => r.ID);
            total = query.Count();
            return query.ToList();
        }

        #endregion

        private StudentEntity InitStudentByRole(StudentEntity student, StudentEntity oldEntity, Role role)
        {
            if (student.ID <= 0)
                oldEntity = NewStudentEntity();

            switch (role)
            {
                case Role.Coordinator:
                case Role.Mentor_coach:
                    student.IsMediaRelease = oldEntity.IsMediaRelease;
                    break;
                case Role.Video_coding_analyst:
                case Role.Statewide:
                    student.IsMediaRelease = oldEntity.IsMediaRelease;
                    student.Status = oldEntity.Status;
                    student.Notes = oldEntity.Notes;
                    break;
                case Role.Community:
                case Role.District_Community_Delegate:
                case Role.District_Community_Specialist:
                case Role.Community_Specialist_Delegate:
                case Role.Principal:
                case Role.Principal_Delegate:
                case Role.Teacher:
                    student.IsMediaRelease = oldEntity.IsMediaRelease;
                    student.Notes = oldEntity.Notes;
                    break;

            }
            return student;
        }

        /// <summary>
        /// 根据SchoolId和Language，获取学生数量
        /// </summary>
        /// <param name="schoolId"></param>
        /// <param name="language"></param>
        /// <returns></returns>
        public int GetStudentCountBySchoolId(int schoolId, IList<int> classIds, StudentAssessmentLanguage language, DateTime dobStartDate, DateTime dobEndDate)
        {
            if (language != StudentAssessmentLanguage.Bilingual)
            {
                return _studentContract.Students.Count(o =>
                    o.SchoolRelations.Any(s => s.SchoolId == schoolId) &&
                    o.Classes.Any(c => classIds.Contains(c.ID)) &&
                    (o.AssessmentLanguage == language || o.AssessmentLanguage == StudentAssessmentLanguage.Bilingual)
                          && o.Status == EntityStatus.Active && o.BirthDate >= dobStartDate && o.BirthDate <= dobEndDate);
            }
            else
            {
                return _studentContract.Students.Count(o =>
                    o.SchoolRelations.Any(s => s.SchoolId == schoolId) &&
                    o.Classes.Any(c => classIds.Contains(c.ID)) &&
                    o.Status == EntityStatus.Active && o.AssessmentLanguage > 0 && o.BirthDate >= dobStartDate && o.BirthDate <= dobEndDate);
            }
        }

        /// <summary>
        /// 获取Community下，满足语言的学生数量
        /// </summary>
        /// <param name="communityId"></param>
        /// <param name="language"></param>
        /// <returns></returns>
        public int GetStudentCount(int communityId, StudentAssessmentLanguage language, IList<int> classIds, DateTime dobStartDate, DateTime dobEndDate)
        {
            int total;
            List<int> schoolIds = _schoolBusiness.GetSchoolIds(s => s.CommunitySchoolRelations.Any(r => r.CommunityId == communityId) && !s.SchoolType.Name.Contains("demo") && s.Status == SchoolStatus.Active
                , null, "ID", "asc", 0, 1000, out total);

            if (schoolIds.Count == 0) return 0;

            if (language != StudentAssessmentLanguage.Bilingual)
            {
                return _studentContract.Students.Count(s => s.SchoolRelations.Any(sr => schoolIds.Contains(sr.SchoolId))
                     && s.Classes.Any(c => classIds.Contains(c.ID)) &&
                     (s.AssessmentLanguage == language || s.AssessmentLanguage == StudentAssessmentLanguage.Bilingual)
                           && s.Status == EntityStatus.Active && s.BirthDate >= dobStartDate && s.BirthDate <= dobEndDate);
            }
            else
            {
                return _studentContract.Students.Count(s => s.SchoolRelations.Any(sr => schoolIds.Contains(sr.SchoolId))
                  && s.Classes.Any(c => classIds.Contains(c.ID)) &&
                  s.AssessmentLanguage > 0 && s.Status == EntityStatus.Active && s.BirthDate >= dobStartDate && s.BirthDate <= dobEndDate);
            }
        }


        /// <summary>
        /// 给 CPALLS+ 的 Groups 用
        /// </summary>
        /// <param name="classId"></param>
        /// <returns></returns>
        public List<GroupStudentModel> GetGroupStudentList(int classId, AssessmentLanguage language)
        {
            var query = _studentContract.Students.Where(r => r.Classes.Where(e => e.IsDeleted == false).Select(c => c.ID).Contains(classId) && r.Status == EntityStatus.Active);
            if (language == AssessmentLanguage.English)
                query = query.Where(r => r.AssessmentLanguage == StudentAssessmentLanguage.English || r.AssessmentLanguage == StudentAssessmentLanguage.Bilingual);
            else
                query = query.Where(r => r.AssessmentLanguage == StudentAssessmentLanguage.Spanish || r.AssessmentLanguage == StudentAssessmentLanguage.Bilingual);

            return query.Select(r => new GroupStudentModel
            {
                ID = r.ID,
                FirstName = r.FirstName,
                LastName = r.LastName
            }).OrderBy(r => r.LastName).ToList();
        }


        public List<StudentClassModel> GetStudentClassModel(int[] studentIds)
        {
            return _studentContract.Students.Where(s => studentIds.Contains(s.ID))
                .Select(s => new StudentClassModel
                {
                    StudentId = s.ID,
                    StudentStatus = s.Status,
                    FirstName = s.FirstName,
                    LastName = s.LastName,
                    ClassCount = s.Classes.Count(c => c.Status == EntityStatus.Active && c.IsDeleted == false)
                }).ToList();
        }

        public IEnumerable<StudentEntity> GetStudentsByCondition(Expression<Func<StudentEntity, bool>> condition)
        {
            return _studentContract.Students.AsExpandable().Where(condition);
        }

        public StudentEntity GetStudentById(int id)
        {
            return _studentContract.GetStudent(id);
        }

        /// <summary>
        /// 同一个Community下是否存在FirstName，LastName，BirthDate相同的学生
        /// </summary>
        /// <param name="firstName"></param>
        /// <param name="lastName"></param>
        /// <param name="birthDate"></param>
        /// <param name="communityId"></param>
        /// <returns></returns>
        public bool IsStudentExist(string firstName, string lastName, DateTime birthDate, int communityId)
        {
            return _studentContract.Students
                .Any(o => o.FirstName == firstName
                    && o.LastName == lastName
                    && o.BirthDate == birthDate
                          &&
                          o.Classes.Any(
                              c =>
                                  c.School.CommunitySchoolRelations.Any(r => r.CommunityId == communityId) &&
                                  c.IsDeleted == false)
                );
        }

        #region StudentRole
        public StudentRoleEntity GetStudentRoleEntity(Role role)
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
            return _studentContract.GetStudentRole(newRole);
        }
        #endregion

        public void CheckStuSchool(int stuId)
        {
            StudentEntity stuEntity = GetStudentById(stuId);
            List<int> ids = stuEntity.Classes.Where(e => e.IsDeleted == false).Select(c => c.SchoolId).ToList();
            List<SchoolStudentRelationEntity> stuSchools = _schoolBusiness.GetSchoolStudentRelationList(
                r => r.StudentId == stuId && !ids.Contains(r.SchoolId));
            _schoolBusiness.DeleteRelations(stuSchools);
        }

        #region StudentDOB
        public List<StudentDOBEntity> GetStudentDobsbyStatus(StudentDOBStatus status)
        {
            List<StudentDOBEntity> processList = _studentContract.StudentDOBs.Where(s => s.Status == status && s.OldDOB != s.NewDOB).ToList();
            return processList;
        }

        public OperationResult UpdateStudentDOB(StudentDOBEntity entity)
        {
            return _studentContract.UpdateStudentDOB(entity);
        }

        public OperationResult UpdateStudentDOBs(List<StudentDOBEntity> entities)
        {
            return _studentContract.UpdateStudentDOBs(entities);
        }
        #endregion

        #region For ParentReport
        public string GeneratePDFStringForParentReport(CpallsStudentModel student)
        {
            string emailPDFTemplateName = "ParentPinPagePDF_Template.xml";
            EmailTemplete template = XmlHelper.GetEmailTemp(emailPDFTemplateName);
            string pdfContent = "";
            pdfContent = template.Body.Replace("[firstName]", student.FirstName)
                .Replace("[lastName]", student.LastName)
                .Replace("[parentCode]", student.ParentCode)
                .Replace("[staticdomain]", SFConfig.StaticDomain);
            return pdfContent;
        }
        public string GenerateParentReportEmailBody()
        {
            string emailPDFTemplateName = "ParentPinPagePDF_Template.xml";
            EmailTemplete template = XmlHelper.GetEmailTemp(emailPDFTemplateName);

            return template.Body;
        }
        public byte[] GeneratePDFForParentReport(string template, CpallsStudentModel student)
        {
            var pdfContent = template.Replace("[firstName]", student.FirstName)
                 .Replace("[lastName]", student.LastName)
                 .Replace("[parentCode]", student.ParentCode)
                 .Replace("[staticdomain]", SFConfig.StaticDomain);
            return GetPdfByte(pdfContent);
        }
        public byte[] GeneratePDFForParentReport(CpallsStudentModel student)
        {
            string emailPDFTemplateName = "ParentPinPagePDF_Template.xml";
            EmailTemplete template = XmlHelper.GetEmailTemp(emailPDFTemplateName);
            string pdfContent = "";
            pdfContent = template.Body.Replace("[firstName]", student.FirstName)
                .Replace("[lastName]", student.LastName)
                .Replace("[parentCode]", student.ParentCode)
                .Replace("[staticdomain]", SFConfig.StaticDomain);
            return GetPdfByte(pdfContent);
        }
        public byte[] GenerateCoverReport(CpallsStudentModel student)
        {
            string emailPDFTemplateName = "ParentInvitationPDF_CoverPage.xml";
            EmailTemplete template = XmlHelper.GetEmailTemp(emailPDFTemplateName);
            string pdfContent = "";
            pdfContent = template.Body.Replace("[firstName]", student.FirstName)
                .Replace("[lastName]", student.LastName)
                .Replace("[parentCode]", student.ParentCode)
                .Replace("[staticdomain]", SFConfig.StaticDomain);
            return GetPdfByte(pdfContent);
        }
        private byte[] GetPdfByte(string contentHtml)
        {
            int teacherCount = 0;
            var result = new OperationResult(OperationResultType.Success);
            try
            {
                PdfProvider pdfProvider = new PdfProvider();
                byte[] pdfBytes = pdfProvider.GetPdfBytes(contentHtml);
                return pdfBytes;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        #endregion
    }
}
