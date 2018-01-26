using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Sunnet.Cli.Business.Classes.Models;
using Sunnet.Cli.Business.Common;
using Sunnet.Cli.Business.Communities;
using Sunnet.Cli.Business.Schools;
using Sunnet.Cli.Business.TRSClasses.Models;
using Sunnet.Cli.Core;
using Sunnet.Cli.Core.Classes.Entites;
using Sunnet.Cli.Core.Common.Enums;
using Sunnet.Cli.Core.TRSClasses;
using Sunnet.Cli.Core.TRSClasses.Entites;
using Sunnet.Cli.Core.Users;
using Sunnet.Cli.Core.Users.Entities;
using Sunnet.Cli.Core.Users.Enums;
using Sunnet.Framework.Core.Tool;
using Sunnet.Framework.Core.Extensions;
using Sunnet.Framework.Extensions;
using LinqKit;
using Sunnet.Cli.Business.Trs.Models;
using ChildrenTypeModel = Sunnet.Cli.Business.Classes.Models.ChildrenTypeModel;

namespace Sunnet.Cli.Business.TRSClasses
{
    public class TRSClassBusiness
    {
        private readonly ITRSClassContract _contract;
        private readonly IUserContract _userContract;
        private readonly SchoolBusiness _schoolBusiness;
        private readonly CommunityBusiness _communityBuss;

        public TRSClassBusiness(EFUnitOfWorkContext unit = null)
        {
            _contract = DomainFacade.CreateTRSClassService(unit);
            _userContract = DomainFacade.CreateUserService(unit);
            _schoolBusiness = new SchoolBusiness(unit);
            _communityBuss = new CommunityBusiness(unit);
        }

        #region Class New/Insert/Update/Get
        public TRSClassEntity NewTRSClassModel()
        {
            TRSClassEntity entity = new TRSClassEntity();
            entity.StatusDate = DateTime.Now;
            entity.SchoolYear = CommonAgent.SchoolYear;
            entity.Notes = string.Empty;
            return entity;
        }
        public TRSClassEntity NewClassEntity()
        {
            TRSClassEntity entity = new TRSClassEntity();
            entity.StatusDate = DateTime.Now;
            entity.SchoolYear = CommonAgent.SchoolYear;
            entity.Notes = string.Empty;
            return entity;
        }

        public OperationResult InsertTRSClass(TRSClassEntity entity, List<CHChildrenResultEntity> trsResultList)
        {
            entity.SchoolYear = CommonAgent.SchoolYear;
            entity.StatusDate = DateTime.Now;
            entity.TrsAssessorId = entity.TrsAssessorId;
            entity.TrsMentorId = entity.TrsMentorId == null ? 0 : entity.TrsMentorId;

            OperationResult result = _contract.InsertTRSClass(entity);

            if (result.ResultType == OperationResultType.Success)
            {
                if (trsResultList != null)
                {
                    trsResultList.ForEach(r => r.TRSClassId = entity.ID);
                    return _contract.InsertResult(trsResultList);
                }
                else
                    return result;
            }
            else
                return result;
        }

        public OperationResult UpdateTRSClass(TRSClassEntity entity)
        {
            return _contract.UpdateTRSClass(entity);
        }

        public OperationResult UpdateTRSClass(TRSClassEntity entity, List<CHChildrenResultEntity> trsResultList)
        {
            TRSClassEntity classEntity = _contract.GetTRSClass(entity.ID);
            classEntity.UpdatedOn = DateTime.Now;
            bool showAgeofChildren =
                _communityBuss.CheckShowAgeofChildren(
                    classEntity.School.CommunitySchoolRelations.Select(r => r.CommunityId).ToList());
            if (showAgeofChildren)
            {
                if (trsResultList != null && trsResultList.Any())
                {
                    _contract.DeleteResult(entity.ID, false);
                    _contract.InsertResult(trsResultList, false);
                }
            }
            entity.CreatedOn = classEntity.CreatedOn;
            OperationResult result = _contract.UpdateTRSClass(entity);
            return result;
        }

        public TRSClassEntity GetTRSClass(int id)
        {
            var entity = _contract.GetTRSClass(id);
            return entity;
        }

        /// <summary>
        /// True 表示是 demo
        /// </summary>
        public bool CheckSchoolTypeIsDemo(int classId)
        {
            return _contract.TRSClasses.Any(r => r.ID == classId && r.School.SchoolType.Name.Contains("demo"));
        }

        public OperationResult UpdateTRSClassPlayground(int playgroundId, int[] classIds = null)
        {
            return _contract.UpdateTRSClassPlayground(playgroundId, classIds);
        }

        public List<TRSClassEntity> GetTRSClassList(Expression<Func<TRSClassEntity, bool>> condition)
        {
            return _contract.TRSClasses.AsExpandable().Where(condition).ToList();
        }

        public bool IsTRSClassExist(string name, int id = 0, int schoolId = 0)
        {
            return _contract.TRSClasses.Any(c => c.Name == name && c.ID != id && c.SchoolId == schoolId);
        }
        #endregion

        #region GetSelectList

        public List<SelectItemModel> GetTRSClassId(Expression<Func<TRSClassEntity, bool>> condition)
        {
            return _contract.TRSClasses.AsExpandable().Where(condition).Select(o => new SelectItemModel()
            {
                ID = o.ID,
                Name = o.TRSClassId
            }).ToList();
        }

        public List<SelectItemModel> GetTRSClassSelectList(Expression<Func<TRSClassEntity, bool>> condition, UserBaseEntity user)
        {
            return _contract.TRSClasses.AsExpandable().Where(condition).Where(GetRoleCondition(user)).Select(o => new SelectItemModel()
            {
                ID = o.ID,
                Name = o.Name
            }).ToList();
        }

        public List<SelectItemModel> GetTRSClassSelectList(UserBaseEntity user, Expression<Func<TRSClassEntity, bool>> condition)
        {
            return
                _contract.TRSClasses.AsExpandable()
                    .Where(condition)
                    .Where(GetRoleCondition(user))
                    .Select(o => new SelectItemModel()
                    {
                        ID = o.ID,
                        Name = o.Name
                    }).OrderBy(o => o.Name.Trim()).ToList();
        }

        public List<TRSClassIndexModel> SearchTRSClasses(UserBaseEntity userInfo,
            Expression<Func<TRSClassEntity, bool>> condition,
            string sort, string order, int first, int count, out int total)
        {
            var query = _contract.TRSClasses.AsExpandable().Where(condition).Where(GetRoleCondition(userInfo)).
                Select(o => new TRSClassIndexModel()
                {
                    ID = o.ID,
                    TRSClassId = o.TRSClassId,
                    TRSClassName = o.Name,
                    Status = o.Status,
                    PlaygroundId = o.PlaygroundId
                });
            total = query.Count();
            var list = query.OrderBy(sort, order).Skip(first).Take(count);
            return list.ToList();
        }
        #endregion

        #region Private Function
        /// <summary>
        /// 根据不同的角色查找对应的数据源
        /// </summary>
        /// <param name="userInfo"></param>
        /// <returns></returns>
        private Expression<Func<TRSClassEntity, bool>> GetRoleCondition(UserBaseEntity userInfo)
        {
            Expression<Func<TRSClassEntity, bool>> condition = o => true;
            if (userInfo == null)
                return condition;
            var parent = new UserBaseEntity();
            if (userInfo.Role == Role.Community_Specialist_Delegate || userInfo.Role == Role.District_Community_Delegate)
            {
                parent = _userContract.GetUser(userInfo.CommunityUser.ParentId);
            }

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
                case Role.Teacher:
                    condition = o => false;
                    break;

                case Role.Principal:
                case Role.School_Specialist:
                case Role.TRS_Specialist:
                    condition = PredicateBuilder.And(condition, r => r.School.UserCommunitySchools.Any(u => u.UserId == userInfo.ID));
                    break;
                case Role.Principal_Delegate:
                case Role.School_Specialist_Delegate:
                case Role.TRS_Specialist_Delegate:
                    condition = PredicateBuilder.And(condition, r => r.School.UserCommunitySchools.Any(u => u.UserId == userInfo.Principal.ParentId));
                    break;
                case Role.Community:
                case Role.District_Community_Specialist:
                case Role.Statewide:
                    condition = PredicateBuilder.And(condition, r =>
                                (basicComIds.Contains(r.School.BasicSchool.CommunityId))
                              ||
                              (
                                  r.School.CommunitySchoolRelations.Any(c => comIds.Contains(c.CommunityId))
                               )
                           );
                    break;
                case Role.District_Community_Delegate:
                case Role.Community_Specialist_Delegate:
                    List<int> parentBasicComIds = parent.UserCommunitySchools.Where(o => o.CommunityId > 0).Select(o => o.Community.BasicCommunityId).ToList();
                    List<int> parentComIds = parent.UserCommunitySchools.Where(o => o.CommunityId > 0).Select(o => o.CommunityId).ToList();
                    condition = PredicateBuilder.And(condition, r =>
                                (parentBasicComIds.Contains(r.School.BasicSchool.CommunityId))
                              ||
                              (
                                  r.School.CommunitySchoolRelations.Any(c => parentComIds.Contains(c.CommunityId))
                               )
                           );
                    break;
            }

            return condition;
        }
        #endregion

        #region TRS

        public string GetChildrenTypeName(int iD)
        {
            CHChildrenEntity child = _contract.CHChildrens.FirstOrDefault(o => o.ID == iD);
            if (child != null)
                return child.Name;
            else
            {
                return string.Empty;
            }
        }

        public List<ChildrenTypeModel> GetChildrenType()
        {
            return _contract.CHChildrens.Where(o => o.Status == EntityStatus.Active).ToList().Select(e => new ChildrenTypeModel()
            {
                ID = e.ID,
                Name = e.Name,
                AgeArea = e.AgeArea,
                AgeSort = e.AgeSort,
                Status = e.Status
            }).ToList();
        }


        public List<CHChildrenResultEntity> GetResultList(int trsClassId)
        {
            return _contract.CHChildrenResults.Where(o => o.TRSClassId == trsClassId).ToList();
        }

        public List<CHChildrenTRSResultModel> GetResultModelList(int classId)
        {
            return _contract.CHChildrenResults.Where(r => r.TRSClassId == classId)
                .Select(r => new CHChildrenTRSResultModel()
                {
                    ID = r.ID,
                    TypeofChildren = r.CHChildren.Name,
                    NumofChildren = r.ChildrenNumber,
                    Checked = true,
                    CaregiversNumber = r.CaregiversNumber
                }).ToList();
        }


        public List<TrsClassModel> GetTrsClasses(UserBaseEntity user, params int[] schoolIds)
        {
            var classes =
                _contract.TRSClasses.AsExpandable()
                    .Where(x => schoolIds.Contains(x.SchoolId))
                    .Where(x => x.Status == EntityStatus.Active)
                    .Where(GetRoleCondition(user))
                    .Select(x => new TrsClassModel()
                    {
                        Id = x.ID,
                        Name = x.Name,
                        TypeOfClass = x.TypeOfClass,
                        PlaygroundId = x.PlaygroundId,
                        TrsAssessorId = x.TrsAssessorId,
                        TrsMentorId = x.TrsMentorId.Value
                    }).ToList();
            var classIds = classes.Select(x => x.Id).ToList();
            var ageRecords = _contract.CHChildrenResults.Where(x => classIds.Contains(x.TRSClassId)).ToList();
            var ageRecordIds = ageRecords.Select(ar => ar.CHChildrenId).ToList();
            var ages = _contract.CHChildrens.Where(x => ageRecordIds.Contains(x.ID))
                    .ToList();

            classes.ForEach(class1 =>
            {
                class1.Ages = new List<TrsAgeModel>();
                var records = ageRecords.Where(x => x.TRSClassId == class1.Id);
                records.ForEach(ageRecord =>
                {
                    var age = ages.Find(x => x.ID == ageRecord.CHChildrenId);
                    if (age != null)
                    {
                        class1.Ages.Add(new TrsAgeModel()
                        {
                            Id = age.ID,
                            NumberOfChildren = ageRecord.ChildrenNumber,
                            TypeOfChildren = age.Name,
                            AgeArea = age.AgeArea
                        });
                    }
                });
            });
            return classes;
        }

        //Create by steven,for ticket 2485
        public List<TrsClassModel> GetTrsClasses(params int[] schoolIds)
        {
            var classes =
                _contract.TRSClasses.AsExpandable()
                    .Where(x => schoolIds.Contains(x.SchoolId))
                    .Where(x => x.Status == EntityStatus.Active)
                    .Select(x => new TrsClassModel()
                    {
                        Id = x.ID,
                        Name = x.Name,
                        TypeOfClass = x.TypeOfClass,
                        PlaygroundId = x.PlaygroundId,
                        TrsAssessorId = x.TrsAssessorId,
                        TrsMentorId = x.TrsMentorId.Value
                    }).ToList();
            var classIds = classes.Select(x => x.Id).ToList();
            var ageRecords = _contract.CHChildrenResults.Where(x => classIds.Contains(x.TRSClassId)).ToList();
            var ageRecordIds = ageRecords.Select(ar => ar.CHChildrenId).ToList();
            var ages = _contract.CHChildrens.Where(x => ageRecordIds.Contains(x.ID))
                    .ToList();

            classes.ForEach(class1 =>
            {
                class1.Ages = new List<TrsAgeModel>();
                var records = ageRecords.Where(x => x.TRSClassId == class1.Id);
                records.ForEach(ageRecord =>
                {
                    var age = ages.Find(x => x.ID == ageRecord.CHChildrenId);
                    if (age != null)
                    {
                        class1.Ages.Add(new TrsAgeModel()
                        {
                            Id = age.ID,
                            NumberOfChildren = ageRecord.ChildrenNumber,
                            TypeOfChildren = age.Name,
                            AgeArea = age.AgeArea
                        });
                    }
                });
            });
            return classes;
        }

        public List<TrsClassReportModel> GetTrsClassesReport(UserBaseEntity user, int schoolId, out string minAgeGroup,
            out string maxAgeGroup)
        {
            minAgeGroup = "";
            maxAgeGroup = "";
            List<TrsClassReportModel> classes = _contract.TRSClasses.AsExpandable()
                .Where(GetRoleCondition(user))
                .Where(c => c.SchoolId == schoolId && c.Status == EntityStatus.Active)
                .Select(a => new TrsClassReportModel
                {
                    ID = a.ID,
                    Name = a.Name,
                    TypeOfClass = a.TypeOfClass,
                    PlaygroundId = a.PlaygroundId
                }).ToList();
            var classIds = classes.Select(x => x.ID).Distinct().ToList();
            var ageRecords = _contract.CHChildrenResults.Where(x => classIds.Contains(x.TRSClassId)).ToList();
            var ageRecordIds = ageRecords.Select(ar => ar.CHChildrenId).Distinct().ToList();
            var ages = _contract.CHChildrens.Where(x => ageRecordIds.Contains(x.ID))
                .Select(c => new TrsAgeReportModel
                {
                    ID = c.ID,
                    Name = c.Name,
                    AgeSort = c.AgeSort,
                    AgeArea = c.AgeArea
                }).OrderBy(d => d.AgeSort).ToList();
            if (ages != null)
            {
                if (ages.Count > 0)
                {
                    minAgeGroup = ages.First().Name;
                    maxAgeGroup = ages.Last().Name;
                    classes.ForEach(class1 =>
                    {
                        class1.Ages = new List<TrsAgeReportModel>();
                        var records = ageRecords.Where(x => x.TRSClassId == class1.ID);
                        records.ForEach(ageRecord =>
                        {
                            var age = ages.Find(x => x.ID == ageRecord.CHChildrenId);
                            if (age != null)
                            {
                                class1.Ages.Add(age);
                            }
                        });
                    });
                }
            }
            return classes.ToList();
        }

        public OperationResult DeleteResultBySchoolId(int schoolId, bool isSave)
        {
            return _contract.DeleteResultBySchoolId(schoolId, isSave);
        }

        public Permission.Permission TRSAccess(Role role)
        {
            //内部用户,School Specialist可以编辑
            if (role < Role.Auditor
                || role == Role.TRS_Specialist
                || role == Role.TRS_Specialist_Delegate
                || role == Role.School_Specialist
                || role == Role.School_Specialist_Delegate
                || role == Role.Community
                || role == Role.District_Community_Delegate
                || role == Role.District_Community_Specialist
                || role == Role.Community_Specialist_Delegate)
                return Permission.Permission.Editable;

            else if (role == Role.Statewide
                || role == Role.Principal
                || role == Role.Principal_Delegate)
                return Permission.Permission.Readonly;
            return Permission.Permission.Invisible;
        }

        public Permission.Permission TRSAccess(Role role, int schoolId)
        {
            var authority = TRSAccess(role);
            if (authority == Permission.Permission.Invisible)
                return authority;
            var schoolStatus = _schoolBusiness.ShowTrs(schoolId);
            if (schoolStatus)
                return authority;
            return Permission.Permission.Invisible;
        }

        /// <summary>
        /// 统计每个学校下，各年龄段的 Class个数
        /// </summary>
        /// <param name="schoolId"></param>
        /// <returns></returns>
        public List<ClassCountAgeModel> GetTRSClassCountAge(int schoolId)
        {
            List<int> classIds = _contract.TRSClasses.Where(r => r.School.ID == schoolId && (int)r.School.TrsAssessorId > 0 && r.Status == EntityStatus.Active)
                .Select(r => r.ID).ToList();

            return _contract.CHChildrenResults.Where(r => classIds.Contains(r.TRSClassId)).GroupBy(r => r.CHChildren.AgeArea)
                     .Select(r => new ClassCountAgeModel { AgeArea = r.Key, Number = r.Count() }).ToList();
        }
        #endregion

        public OperationResult DeleteResultBySchoolId(int schoolId)
        {
            return _contract.DeleteResultBySchoolId(schoolId);
        }
    }
}