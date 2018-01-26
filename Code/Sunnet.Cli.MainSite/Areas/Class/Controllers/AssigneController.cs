using Sunnet.Cli.Business.Classes;
using Sunnet.Cli.Business.Communities;
using Sunnet.Cli.Business.Schools;
using Sunnet.Cli.Business.Users;
using Sunnet.Cli.Core.Classes.Entites;
using Sunnet.Cli.Core.Users.Entities;
using Sunnet.Cli.Core.Users.Enums;
using Sunnet.Cli.MainSite.Controllers;
using Sunnet.Cli.UIBase.Models;
using Sunnet.Framework.Core.Tool;
using Sunnet.Framework.Extensions;
using Sunnet.Framework.Helpers;
using Sunnet.Cli.Core.Students.Entities;
using Sunnet.Cli.Core.Common.Enums;
using Sunnet.Cli.Business.Students;
using Sunnet.Cli.Business.Students.Models;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;
using Sunnet.Cli.UIBase.Filters;
using Sunnet.Framework.Permission;
using Sunnet.Framework.Log;
using StructureMap;

namespace Sunnet.Cli.MainSite.Areas.Class.Controllers
{
    public class AssigneController : BaseController
    {
        #region Private Field

        private readonly ClassBusiness _classBusiness;
        private readonly UserBusiness _userBusiness;
        private readonly StudentBusiness _studentBusiness;
        private readonly ISunnetLog _logger;

        #endregion

        public AssigneController()
        {
            _classBusiness = new ClassBusiness(UnitWorkContext);
            _userBusiness = new UserBusiness(UnitWorkContext);
            _studentBusiness = new StudentBusiness(UnitWorkContext);
            _logger = ObjectFactory.GetInstance<ISunnetLog>();

        }
        //#region Community Specialist



        //// GET: /Class/Assigne/
        //[CLIUrlAuthorize(Account = Authority.Assign, PageId = PagesModel.Class_Management, Anonymity = Anonymous.Verified)]
        //public ActionResult CommunitySpecialist(int classId)
        //{
        //    ClassEntity entity = _classBusiness.GetClass(classId);
        //    return View(entity);
        //}

        //[CLIUrlAuthorize(Account = Authority.Assign, PageId = PagesModel.Class_Management, Anonymity = Anonymous.Verified)]
        //public string SearchUnAssignedCommunitySpecialist(int schoolId = 0, int classId = 0, string name = "",
        //    string sort = "FirstName", string order = "ASC", int first = 0, int count = 10)
        //{
        //    int total = 0;
        //    Expression<Func<CommunityUserEntity, bool>> condition = PredicateHelper.True<CommunityUserEntity>();
        //    if (name.Trim() != string.Empty)
        //        condition = condition.And(r => r.UserInfo.FirstName.Contains(name.Trim()) || r.UserInfo.LastName.Contains(name.Trim()));
        //    condition = condition.And(r => r.UserInfo.UserCommunitySchools.Any(c => c.Community.CommunitySchoolRelations.Any(s => s.SchoolId == schoolId)));

        //    condition = condition.And(r => !(r.UserInfo.UserClasses.Any(c => c.ClassId == classId)));

        //    var list = _userBusiness.SearchCommunitySpecialists(UserInfo, condition, sort, order, first, count, out total);

        //    var result = new { total = total, data = list };
        //    return JsonHelper.SerializeObject(result);
        //}

        //[CLIUrlAuthorize(Account = Authority.Assign, PageId = PagesModel.Class_Management, Anonymity = Anonymous.Verified)]
        //public string AssignCommunitySpecialist(int classId, int schoolId, int[] userIds)
        //{
        //    var response = new PostFormResponse();
        //    OperationResult result = new OperationResult(OperationResultType.Success);
        //    result = _userBusiness.InsertUserClassRelationsMoreUser(userIds, UserInfo.ID, classId);
        //    response.Success = result.ResultType == OperationResultType.Success;
        //    response.Data = "";
        //    response.Message = result.Message;
        //    return JsonHelper.SerializeObject(response);
        //}

        //[CLIUrlAuthorize(Account = Authority.Assign, PageId = PagesModel.Class_Management, Anonymity = Anonymous.Verified)]
        //public string DeleteCommunitySpecialist(int classId, int schoolId, int[] userIds)
        //{
        //    var response = new PostFormResponse();
        //    OperationResult result = new OperationResult(OperationResultType.Success);
        //    result = _userBusiness.DeleteUserClassRelationsMoreUser(classId, userIds);
        //    response.Success = result.ResultType == OperationResultType.Success;
        //    response.Data = "";
        //    response.Message = result.Message;
        //    return JsonHelper.SerializeObject(response);
        //}

        //[CLIUrlAuthorize(Account = Authority.Assign, PageId = PagesModel.Class_Management, Anonymity = Anonymous.Verified)]
        //public string SearchAssignedCommunitySpecialist(int schoolId = 0, int classId = 0, string name = "",
        //    string sort = "FirstName", string order = "ASC", int first = 0, int count = 10)
        //{
        //    int total = 0;
        //    Expression<Func<CommunityUserEntity, bool>> condition = PredicateHelper.True<CommunityUserEntity>();
        //    if (name.Trim() != string.Empty)
        //        condition = condition.And(r => r.UserInfo.FirstName.Contains(name.Trim()) || r.UserInfo.LastName.Contains(name.Trim()));

        //    condition = condition.And(r => r.UserInfo.UserCommunitySchools.Any(c => c.Community.CommunitySchoolRelations.Any(s => s.SchoolId == schoolId)));

        //    condition = condition.And(r => r.UserInfo.UserClasses.Any(c => c.ClassId == classId));

        //    var list = _userBusiness.SearchCommunitySpecialists(UserInfo, condition, sort, order, first, count, out total);

        //    var result = new { total = total, data = list };
        //    return JsonHelper.SerializeObject(result);
        //}
        //#endregion

        #region Assign Users To Class
        // GET: /Class/Assigne/
        [CLIUrlAuthorize(Account = Authority.Assign, PageId = PagesModel.Class_Management, Anonymity = Anonymous.Verified)]
        public ActionResult AssignUsersToClass(int classId)
        {
            var list = new List<SelectListItem>();
            list.Add(new SelectListItem() { Text = "Please select...", Value = "0" });
            list.Add(new SelectListItem() { Text = Role.Community.ToDescription(), Value = Role.Community.GetValue().ToString() });
            list.Add(new SelectListItem() { Text = Role.District_Community_Specialist.ToDescription(), Value = Role.District_Community_Specialist.GetValue().ToString() });
            list.Add(new SelectListItem() { Text = Role.Statewide.ToDescription(), Value = Role.Statewide.GetValue().ToString() });
            ViewBag.RoleOptions = list;
            ClassEntity entity = _classBusiness.GetClass(classId);
            return View(entity);
        }

        [CLIUrlAuthorize(Account = Authority.Assign, PageId = PagesModel.Class_Management, Anonymity = Anonymous.Verified)]
        public string SearchUnAssignedUsers(int communityId = 0, string communityName = "", Role role = (Role)0, int schoolId = 0, int classId = 0, string name = "",
            string sort = "FirstName", string order = "ASC", int first = 0, int count = 10)
        {
            int total = 0;
            Expression<Func<UserBaseEntity, bool>> condition = PredicateHelper.True<UserBaseEntity>();
            //string debugStr = "SearchUnAssignedUsers(communityId=" + communityId + ",communityName=" + ",Role=" + role
            //    + communityName + ",schoolId=" + schoolId + ",classId=" + classId + ",name=" + name;
            //_logger.Debug("A." + debugStr);
            var basicComId = (new SchoolBusiness()).GetSchool(schoolId).BasicSchool.CommunityId;
            //_logger.Debug("B. basicComId=" + basicComId);
            List<int> ListComId = new List<int>();
            if (basicComId > 0)
            {
                ListComId = (new CommunityBusiness()).GetCommunityListByBasicId(basicComId).Select(o => o.ID).ToList();
               // _logger.Debug("BB. ListComId=" + string.Join(",", ListComId));
            }

            if (name.Trim() != string.Empty)
                condition = condition.And(r => r.FirstName.Contains(name.Trim()) || r.LastName.Contains(name.Trim()));
            if (communityId > 0)
            {
                condition = condition.And(r => r.UserCommunitySchools.Any(c => c.Community.CommunitySchoolRelations.Any(s => s.CommunityId == communityId)));
            }
            if (communityName.Trim() != "")
            {
                condition = condition.And(r => r.UserCommunitySchools.Any(c => c.Community.CommunitySchoolRelations.Any(s => s.Community.Name.IndexOf(communityName) >= 0)));
            }
            if (role != (Role)0)
            {
                condition = condition.And(r => r.Role == role);
            }
            else
            {
                condition = condition.And(r => (r.Role == Role.Community || r.Role == Role.District_Community_Specialist || r.Role == Role.Statewide));
            }

            // condition = condition.And(r => r.UserCommunitySchools.Any(c => c.SchoolId == schoolId  && c.AccessType != AccessType.ReadOnly));
            condition = condition.And(r => r.UserCommunitySchools.Any(c => c.Community.CommunitySchoolRelations.Any(s => s.SchoolId == schoolId)));


            condition = condition.And(r => !(r.UserClasses.Any(c => c.ClassId == classId && c.Class.IsDeleted == false)));
            condition = condition.And(r => (r.Role == Role.Community || r.Role == Role.District_Community_Specialist || r.Role == Role.Statewide));
            if (ListComId.Count > 0)
                condition = condition.And(r => !(r.UserCommunitySchools.Any(o => ListComId.Contains(o.CommunityId))));//David 06/22/2017 Fixed Ticket 2744

            var list = _userBusiness.GetUsers(UserInfo, condition, sort, order, first, count, out total);

            var result = new { total = total, data = list };
            return JsonHelper.SerializeObject(result);
        }

        [CLIUrlAuthorize(Account = Authority.Assign, PageId = PagesModel.Class_Management, Anonymity = Anonymous.Verified)]
        public string AssignUsers(int classId, int[] userIds)
        {
            var response = new PostFormResponse();
            OperationResult result = new OperationResult(OperationResultType.Success);
            result = _userBusiness.InsertUserClassRelationsMoreUser(userIds, UserInfo.ID, classId);
            response.Success = result.ResultType == OperationResultType.Success;
            response.Data = "";
            response.Message = result.Message;
            return JsonHelper.SerializeObject(response);
        }

        [CLIUrlAuthorize(Account = Authority.Assign, PageId = PagesModel.Class_Management, Anonymity = Anonymous.Verified)]
        public string UnassignUsers(int classId, int[] userIds)
        {
            var response = new PostFormResponse();
            OperationResult result = new OperationResult(OperationResultType.Success);
            result = _userBusiness.DeleteUserClassRelationsMoreUser(classId, userIds);
            response.Success = result.ResultType == OperationResultType.Success;
            response.Data = "";
            response.Message = result.Message;
            return JsonHelper.SerializeObject(response);
        }

        [CLIUrlAuthorize(Account = Authority.Assign, PageId = PagesModel.Class_Management, Anonymity = Anonymous.Verified)]
        public string SearchAssignedUsers(int communityId = 0, string communityName = "", Role role = (Role)0, int schoolId = 0, int classId = 0, string name = "",
            string sort = "FirstName", string order = "ASC", int first = 0, int count = 10)
        {
            int total = 0;
            Expression<Func<UserBaseEntity, bool>> condition = PredicateHelper.True<UserBaseEntity>();
            if (name.Trim() != string.Empty)
                condition = condition.And(r => r.FirstName.Contains(name.Trim()) || r.LastName.Contains(name.Trim()));
            if (communityId > 0)
            {
                condition = condition.And(r => r.UserCommunitySchools.Any(c => c.Community.CommunitySchoolRelations.Any(s => s.CommunityId == communityId)));
            }
            if (communityName.Trim() != "")
            {
                condition = condition.And(r => r.UserCommunitySchools.Any(c => c.Community.CommunitySchoolRelations.Any(s => s.Community.Name.IndexOf(communityName) >= 0)));
            }
            if (role != (Role)0)
            {
                condition = condition.And(r => r.Role == role);
            }
            else
            {
                condition = condition.And(r => (r.Role == Role.Community || r.Role == Role.District_Community_Specialist || r.Role == Role.Statewide));
            }
            // condition = condition.And(r => r.UserCommunitySchools.Any(c => c.Community.CommunitySchoolRelations.Any(s => s.SchoolId == schoolId)));


            condition = condition.And(r => r.UserClasses.Any(c => c.ClassId == classId && c.Class.IsDeleted == false));
            condition = condition.And(r => (r.Role == Role.Community || r.Role == Role.District_Community_Specialist || r.Role == Role.Statewide));
            var list = _userBusiness.GetUsers(UserInfo, condition, sort, order, first, count, out total);

            var result = new { total = total, data = list };
            return JsonHelper.SerializeObject(result);
        }
        #endregion


        #region Class Students

        [CLIUrlAuthorize(Account = Authority.Assign, PageId = PagesModel.Class_Management, Anonymity = Anonymous.Verified)]
        public ActionResult ClassStudents(int classId)
        {
            ClassEntity entity = _classBusiness.GetClass(classId);
            return View(entity);
        }

        [CLIUrlAuthorize(Account = Authority.Assign, PageId = PagesModel.Class_Management, Anonymity = Anonymous.Verified)]
        public string SearchAssignedStudent(int classId = 0, string name = "",
            string sort = "Name", string order = "ASC", int first = 0, int count = 10)
        {
            int total = 0;
            var expression = PredicateHelper.True<StudentEntity>();
            var studentName = name.Trim();

            expression = expression.And(r => (r.Classes.Any(c => c.ID == classId && c.IsDeleted == false)));

            if (name.Trim() != string.Empty)
            {
                expression = expression.And(r => r.FirstName.Contains(studentName)
                    || r.MiddleName.Contains(studentName)
                    || r.LastName.Contains(studentName));
            }

            var list = _classBusiness.GetAssignedStudent(UserInfo, expression, sort, order, first, count, out total);
            var result = new { total = total, data = list };
            return JsonHelper.SerializeObject(result);
        }

        [CLIUrlAuthorize(Account = Authority.Assign, PageId = PagesModel.Class_Management, Anonymity = Anonymous.Verified)]
        public string SearchUnAssignedStudent(
            int communityId = -1, string communityName = "", int schoolId = -1, string schoolName = "",
            int classId = 0, int searchClassId = 0, string name = "",
            string sort = "Name", string order = "ASC", int first = 0, int count = 10)
        {
            int total = 0;
            var expression = PredicateHelper.True<StudentEntity>();
            if (communityId >= 1)
                expression = expression.And(s => s.SchoolRelations.Count
                    (r => r.School.CommunitySchoolRelations.Count
                        (c => c.CommunityId == communityId) > 0) > 0);
            else if (communityName != null && communityName.Trim() != string.Empty)
            {
                communityName = communityName.Trim();
                expression = expression.And(s => s.SchoolRelations
                    .Count(r => r.School.CommunitySchoolRelations
                        .Count(c => c.Community.Name.Contains(communityName)) > 0) > 0);
            }

            if (schoolId >= 1)
                expression = expression.And(s => s.SchoolRelations.Count(r => r.SchoolId == schoolId) > 0);
            else if (schoolName != null && schoolName.Trim() != string.Empty)
            {
                schoolName = schoolName.Trim();
                expression = expression.And(s => s.SchoolRelations
                    .Count(r => r.School.Name.Contains(schoolName)) > 0);
            }

            if (searchClassId > 0)
            {
                expression = expression.And(s => s.Classes.Any(c => c.ID == searchClassId && c.IsDeleted == false));
            }

            if (name != null && name.Trim() != string.Empty)
            {
                name = name.Trim();
                expression = expression.And(s => s.FirstName.Contains(name)
                    || s.MiddleName.Contains(name)
                    || s.LastName.Contains(name));
            }

            //expression = expression.And(s => s.Status == EntityStatus.Active);


            var list = _classBusiness.GetUnAssignStudent
                (UserInfo, expression, classId, sort, order, first, count, out total);
            var result = new { total = total, data = list };
            return JsonHelper.SerializeObject(result);
        }

        [CLIUrlAuthorize(Account = Authority.Assign, PageId = PagesModel.Class_Management, Anonymity = Anonymous.Verified)]
        public string AssignStudentToClass(int classId, int[] studentIds)
        {
            var response = new PostFormResponse();
            ClassEntity classEntity = _classBusiness.GetClass(classId);
            OperationResult result = new OperationResult(OperationResultType.Success);
            result = _classBusiness.AssignStudentsToClass(classId, studentIds, UserInfo);
            response.Success = result.ResultType == OperationResultType.Success;
            response.Data = "";
            response.Message = result.Message;
            return JsonHelper.SerializeObject(response);
        }

        [CLIUrlAuthorize(Account = Authority.Assign, PageId = PagesModel.Class_Management, Anonymity = Anonymous.Verified)]
        public string UnAssignStudentFromClass(int classId, int[] studentIds)
        {
            var response = new PostFormResponse();
            OperationResult result = new OperationResult(OperationResultType.Success);

            ClassEntity classEntity = _classBusiness.GetClass(classId);

            if (classEntity.Status == EntityStatus.Inactive)
            {
                var condition = PredicateHelper.True<StudentEntity>();
                condition = condition.And(s => studentIds.Contains(s.ID));
                List<StudentEntity> studentEntities = _studentBusiness.GetStudentsByCondition(condition).ToList();
                result = _classBusiness.UnAssignStudentsFromClass(classId, studentEntities, UserInfo);
                response.Data = "";
                response.Success = result.ResultType == OperationResultType.Success;
                response.Message = result.Message;
                return JsonHelper.SerializeObject(response);
            }

            List<StudentClassModel> studentClassModels = _studentBusiness.GetStudentClassModel(studentIds);

            List<int> stuIdsInStudentClass = studentClassModels.Select(s => s.StudentId).ToList();


            List<int> stuWithManyClassIds = studentClassModels
                .Where(s => s.ClassCount > 1 || s.StudentStatus == EntityStatus.Inactive)
                .Select(s => s.StudentId).ToList();

            var expression = PredicateHelper.True<StudentEntity>();
            expression = expression.And(s => stuWithManyClassIds.Contains(s.ID));
            List<StudentEntity> stuWithManyClass = _studentBusiness.GetStudentsByCondition(expression).ToList();
            if (stuWithManyClass.Count > 0)
                result = _classBusiness.UnAssignStudentsFromClass(classId, stuWithManyClass, UserInfo);

            string stuOnlyOneClass = string.Empty;
            List<string> OnlyOneClassStus = studentClassModels
                .Where(s => s.ClassCount == 1 && s.StudentStatus == EntityStatus.Active)
                .Select(s => s.FirstName).ToList();
            if (OnlyOneClassStus != null)
                stuOnlyOneClass = string.Join(",\r\n", OnlyOneClassStus);

            List<int> stuOnlyOneClassIds = studentClassModels
                .Where(s => s.ClassCount == 1 && s.StudentStatus == EntityStatus.Active)
                .Select(s => s.StudentId).ToList();

            if (stuOnlyOneClass != string.Empty)
            {
                stuOnlyOneClass = stuOnlyOneClass.TrimEnd(',');
                response.Success = false;
                response.Message = GetInformation("OnlyOneClass").Replace("{StudentNames}", stuOnlyOneClass);
                response.Data = stuOnlyOneClassIds;
            }
            else
            {
                response.Data = "";
                response.Success = result.ResultType == OperationResultType.Success;
                response.Message = result.Message;
            }

            return JsonHelper.SerializeObject(response);
        }

        //用户确认后，分配掉只有当前class的students
        [CLIUrlAuthorize(Account = Authority.Assign, PageId = PagesModel.Class_Management, Anonymity = Anonymous.Verified)]
        public string UnAssignStuOnlyOneClass(int classId, int[] studentIds)
        {
            var expression = PredicateHelper.True<StudentEntity>();
            expression = expression.And(s => studentIds.Contains(s.ID));
            List<StudentEntity> students = _studentBusiness.GetStudentsByCondition(expression).ToList();
            var response = new PostFormResponse();
            OperationResult result = new OperationResult(OperationResultType.Success);
            result = _classBusiness.UnAssignStudentsFromClass(classId, students, UserInfo);
            response.Data = "";
            response.Success = result.ResultType == OperationResultType.Success;
            response.Message = result.Message;
            return JsonHelper.SerializeObject(response);
        }

        #endregion
    }
}