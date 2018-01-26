using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/**************************************************************************
 * Developer: 		Sam
 * Computer:		SAM-PC
 * Domain:			sam-pc
 * CreatedOn:		2014/8/15 14:34:02
 * Description:		Please input class summary
 * Version History:	Created,2014/8/15 14:34:02
 * 
 * 
 **************************************************************************/
using Sunnet.Cli.Business.Students.Models;
using Sunnet.Cli.Core.Users.Entities;
using Sunnet.Framework.Core.Tool;
using Sunnet.Cli.Core.Users.Enums;
using Sunnet.Cli.Core.Permission.Entities;
using Sunnet.Cli.Business.Users.Models;
using System.Linq.Expressions;
using LinqKit;
using Sunnet.Framework.Core.Extensions;
using Sunnet.Cli.Core.Students.Entities;
using Sunnet.Cli.Core.Common.Enums;
using Sunnet.Cli.Business.Communities;

namespace Sunnet.Cli.Business.Users
{
    public partial class UserBusiness
    {
        public List<ParentStudentListModel> SearchParentStudents(UserBaseEntity user,
            Expression<Func<ParentStudentRelationEntity, bool>> condition, string sort, string order, int first,
            int count, out int total)
        {
            var query =
                userService.ParentStudentRelations.AsExpandable()
                    .Where(condition)
                    .Where(GetParentStudentCondition(user));

            total = query.Count();
            var list = query.Select(e => new ParentStudentListModel()
            {
                ID = e.Parent.UserInfo.ID,
                StudentId = e.StudentId,
                ParentId = e.ParentId,
                ChildFirstName = e.Student.FirstName,
                ChildLastName = e.Student.LastName,
                BirthDate = e.Student.BirthDate,
                ParentCode = e.Student.ParentCode,
                ParentFirstName = e.Parent.UserInfo.FirstName,
                ParentLastName = e.Parent.UserInfo.LastName,
                ParentPrimaryEmail = e.Parent.UserInfo.PrimaryEmailAddress,
                ParentStatus = e.Parent.ParentStatus
            }).OrderBy(sort, order).AsQueryable().Skip(first).Take(count);
            return list.ToList();
        }

        public List<UserModel> SearchParents(UserBaseEntity user, Expression<Func<ParentEntity, bool>> condition,
     string sort, string order, int first, int count, out int total)
        {
            var query = userService.Parents.AsExpandable().Where(condition).Where(GetParentCondition(user));

            total = query.Count();
            var list = query.Select(e => new UserModel()
            {
                ID = e.ID,
                UserId = e.UserInfo.ID,
                Code = e.ParentId,
                FirstName = e.UserInfo.FirstName,
                LastName = e.UserInfo.LastName,
                Status = e.UserInfo.Status
            }).OrderBy(sort, order).AsQueryable().Skip(first).Take(count);
            return list.ToList();
        }

        public OperationResult RegisterParent(ParentEntity parent, string parentCode, string childFirstName, string childLastName)
        {
            OperationResult result = new OperationResult(OperationResultType.Success);
            result = InsertParent(parent);
            if (result.ResultType != OperationResultType.Success)
            {
                return result;
            }

            StudentEntity studentEntity = studentBusiness.GetStudentByCode(parentCode.Trim(), childFirstName.Trim(),
                childLastName.Trim());
            if (studentEntity != null)
            {
                ParentStudentRelationEntity parentStudent = new ParentStudentRelationEntity();
                parentStudent.ParentId = parent.ID;
                parentStudent.StudentId = studentEntity.ID;
                parentStudent.Relation = ParentRelation.Other;
                parentStudent.RelationOther = "";
                result = userService.InsertParentStudentRelation(parentStudent);
                if (result.ResultType != OperationResultType.Success)
                {
                    return result;
                }

                parent.UserInfo.UserCommunitySchools = new List<UserComSchRelationEntity>();
                foreach (var item in studentEntity.SchoolRelations)
                {
                    var communityId = item.School.CommunitySchoolRelations.FirstOrDefault(
                        o => o.Community.BasicCommunityId == item.School.BasicSchool.CommunityId) == null
                        ? 0
                        : item.School.CommunitySchoolRelations.FirstOrDefault(
                            o => o.Community.BasicCommunityId == item.School.BasicSchool.CommunityId)
                            .CommunityId;
                    UserComSchRelationEntity parentCommunitySchool = new UserComSchRelationEntity();
                    parentCommunitySchool.CommunityId = communityId;
                    parentCommunitySchool.SchoolId = item.SchoolId;
                    parentCommunitySchool.Status = EntityStatus.Active;
                    parentCommunitySchool.AccessType = AccessType.FullAccess;
                    parentCommunitySchool.CreatedBy = 0;
                    parentCommunitySchool.UpdatedBy = 0;
                    parentCommunitySchool.CreatedOn = DateTime.Now;
                    parentCommunitySchool.UpdatedOn = DateTime.Now;
                    parent.UserInfo.UserCommunitySchools.Add(parentCommunitySchool);
                }
                var community = new CommunityBusiness().GetCommunity("CLI Parent Community");
                if (community != null)
                {
                    UserComSchRelationEntity parentCommunitySchool = new UserComSchRelationEntity();
                    parentCommunitySchool.CommunityId = community.ID;
                    parentCommunitySchool.SchoolId = 0;
                    parentCommunitySchool.Status = EntityStatus.Active;
                    parentCommunitySchool.AccessType = AccessType.FullAccess;
                    parentCommunitySchool.CreatedBy = 0;
                    parentCommunitySchool.UpdatedBy = 0;
                    parentCommunitySchool.CreatedOn = DateTime.Now;
                    parentCommunitySchool.UpdatedOn = DateTime.Now;
                    parent.UserInfo.UserCommunitySchools.Add(parentCommunitySchool);
                }
                parent.UpdatedOn = DateTime.Now;
                result = UpdateParent(parent);
                if (result.ResultType != OperationResultType.Success)
                {
                    return result;
                }

                ChildEntity child = studentBusiness.GetChild(childFirstName, childLastName, studentEntity.BirthDate);
                if (child == null)
                {
                    child = new ChildEntity();
                    child.FirstName = childFirstName;
                    child.LastName = childLastName;
                    child.BirthDate = studentEntity.BirthDate;
                    child.PINCode = studentEntity.ParentCode;
                    child.StudentId = studentEntity.ID;
                    child.CreatedOn = DateTime.Now;
                    child.UpdatedOn = DateTime.Now;
                    child.SchoolCity = "";//David 01/27/2017
                    child.SchoolZip = "";//David 01/27/2017
                    result = studentBusiness.InsertChild(child);
                    if (result.ResultType != OperationResultType.Success)
                    {
                        return result;
                    }
                }
                else
                {
                    child.PINCode = studentEntity.ParentCode;
                    child.StudentId = studentEntity.ID;
                    child.UpdatedOn = DateTime.Now;
                    result = studentBusiness.UpdateChild(child);
                    if (result.ResultType != OperationResultType.Success)
                    {
                        return result;
                    }
                }

                if (!studentBusiness.ExistParentChild(parent.ID, child.ID))
                {
                    ParentChildEntity parentChild = new ParentChildEntity();
                    parentChild.ParentId = parent.ID;
                    parentChild.ChildId = child.ID;
                    result = studentBusiness.InsertParentChild(parentChild);
                    if (result.ResultType != OperationResultType.Success)
                    {
                        return result;
                    }
                }


            }
            return result;
        }

        public bool IsExistStudent(int parentId, int studentId)
        {
            return userService.ParentStudentRelations.Where(e => e.ParentId == parentId && e.StudentId == studentId).Count() > 0;
        }

        public ParentEntity GetParent(int id)
        {
            return userService.Parents.Where(e => e.ID == id && e.UserInfo.IsDeleted == false).FirstOrDefault();
        }
        public ParentEntity GetParent(string firstName, string lastName, string email)
        {
            return userService.Parents.FirstOrDefault(e => e.UserInfo.FirstName == firstName && e.UserInfo.LastName == lastName && e.UserInfo.PrimaryEmailAddress == email);
        }

        public List<int> GetStudentIDbyParentId(int parentId)
        {
            return userService.ParentStudentRelations.Where(e => e.ParentId == parentId).Select(e => e.StudentId).ToList<int>();
        }

        public List<int> GetParentIDsbyStudentIds(List<int> studentIds)
        {
            return userService.ParentStudentRelations.Where(e => studentIds.Contains(e.StudentId)).Select(e => e.ParentId).ToList<int>();
        }

        public OperationResult UpdateParent(ParentEntity parent)
        {
            OperationResult result = new OperationResult(OperationResultType.Success);
            userService.UpdateUser(parent.UserInfo, false);
            result = userService.UpdateParent(parent);
            return result;
        }

        public OperationResult DeleteParentStudent(int parentId, int studentId)
        {
            OperationResult result = new OperationResult(OperationResultType.Success);
          
            result = userService.DeleteParentStudent(parentId,studentId);
            return result;
        }

        public bool IsExistUserSchool(int userId, int communityId, int schoolId)
        {
            return userService.UserComSchRelations.Count(
                e => e.UserId == userId && e.CommunityId == communityId && e.SchoolId == schoolId) > 0;
        }
    }
}
