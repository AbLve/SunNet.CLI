using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using StructureMap;
using Sunnet.Cli.Business.Cot;
using Sunnet.Cli.Business.Observable.Models;
using Sunnet.Cli.Business.Reports;
using Sunnet.Cli.Business.Reports.Model;
using Sunnet.Cli.Business.Students;
using Sunnet.Cli.Business.Students.Models;
using Sunnet.Cli.Business.Users;
using Sunnet.Cli.Core.Common.Enums;
using Sunnet.Cli.Core.Reports.Entities;
using Sunnet.Cli.Core.Students.Entities;
using Sunnet.Cli.Core.Users.Entities;
using Sunnet.Cli.Core.Users.Enums;
using Sunnet.Cli.MainSite.Controllers;
using Sunnet.Cli.UIBase.Filters;
using Sunnet.Cli.UIBase.Models;
using Sunnet.Framework.Core.Tool;
using Sunnet.Framework.Permission;
using Sunnet.Framework.Helpers;
using Sunnet.Cli.Business.Common;
using Sunnet.Cli.Business.Permission.Models;
using Sunnet.Cli.Business.Permission;
using Sunnet.Framework.Encrypt;
using Sunnet.Framework.Extensions;

namespace Sunnet.Cli.MainSite.Areas.Parent.Controllers
{
    public class ParentController : BaseController
    {
        private ObservableBusiness _observableBusiness;
        private ReportBusiness _reportBusiness;
        private StudentBusiness _studentBusiness;
        private UserBusiness _userBusiness;
        private PermissionBusiness _permissionBusiness ;
        public ParentController()
        {
            _observableBusiness = new ObservableBusiness(AdeUnitWorkContext);
            _studentBusiness = new StudentBusiness(UnitWorkContext);
            _reportBusiness = new ReportBusiness(UnitWorkContext);
            _userBusiness = new UserBusiness(UnitWorkContext);
            _permissionBusiness =new PermissionBusiness(UnitWorkContext);
            ViewBag.LMSUrl = GeneralLmsUrl();
        }
        private string GeneralLmsUrl()
        {
          var  _encrypt = ObjectFactory.GetInstance<IEncrypt>();
            string lmsurl = string.Format(
                "{0}/auth/cliauth/cli_redirect.php?clirole={1}&useremail={2}&firstname={3}&lastname={4}&userid={5}&status={6}&roletext={7}",
                DomainHelper.LMSDomain, _encrypt.Encrypt(((byte)UserInfo.Role).ToString()),
                _encrypt.Encrypt(UserInfo.PrimaryEmailAddress),
                _encrypt.Encrypt(UserInfo.FirstName),
                _encrypt.Encrypt(UserInfo.LastName),
                _encrypt.Encrypt(UserInfo.ID.ToString()),
                _encrypt.Encrypt(UserInfo.Status.ToString()),
                _encrypt.Encrypt(UserInfo.Role.ToDescription()));

            if (UserInfo.Role == Role.Community || UserInfo.Role == Role.District_Community_Delegate
                || UserInfo.Role == Role.District_Community_Specialist ||
                UserInfo.Role == Role.Community_Specialist_Delegate)
            {
                lmsurl = lmsurl +
                         string.Format("&objectId={0}", _encrypt.Encrypt(UserInfo.CommunityUser.CommunityUserId));
            }
            else if (UserInfo.Role == Role.Principal || UserInfo.Role == Role.Principal_Delegate
                     || UserInfo.Role == Role.TRS_Specialist || UserInfo.Role == Role.TRS_Specialist_Delegate
                     || UserInfo.Role == Role.School_Specialist || UserInfo.Role == Role.School_Specialist_Delegate)
            {
                lmsurl = lmsurl + string.Format("&objectId={0}", _encrypt.Encrypt(UserInfo.Principal.PrincipalId));
            }
            else if (UserInfo.Role == Role.Teacher)
            {
                lmsurl = lmsurl +
                         string.Format("&objectId={0}", _encrypt.Encrypt(UserInfo.TeacherInfo.TeacherId));
            }
            return lmsurl;
        }
        //
        // GET: /Parent/Parent/
        public ActionResult Index(int studentId = 0, int childId = 0, int assessmentId =0)
        {
            int total = 0;
            IList<ChildListModel> list = new List<ChildListModel>();

            if (UserInfo.Role == Role.Parent)
            {
                list = _studentBusiness.SearchParentChilds(UserInfo.Parent.ID, "FirstName", "ASC", 0, 10, out total);

            }
            if (studentId == 0 && childId == 0 && list.Count > 0)
                return new RedirectResult("index?childId=" + list[0].ID + "&studentId=" + list[0].StudentId+ "&assessmentId=" + assessmentId);
            List<int> allPageIds = _permissionBusiness.CheckPage(UserInfo);
            List<PageModel> pageModel = _permissionBusiness.GetChildPagesList(allPageIds, (int)PagesModel.ParentFeature);
            ViewBag.ParentFeatures = pageModel;
            ViewBag.currentStudentId = studentId;
            ViewBag.currentChildId = childId;
            ViewBag.StudentList = list;
            ViewBag.assessmentId = assessmentId;
            return View();
        }

        [CLIUrlAuthorize(Account = Authority.All, PageId = PagesModel.TCD, Anonymity = Anonymous.Verified)]
        [ValidateInput(false)]
        public string Search(int studentId, int childId,
             string sort = "Name", string order = "Asc", int first = 0, int count = 10)
        {
            DateTime birthDate = CommonAgent.MinDate;
            //if (studentId != 0)
            //{
            //    var student = _studentBusiness.GetStudentEntity(studentId, UserInfo);
            //    birthDate = student.BirthDate;
            //}
            if (childId != 0)
            {
                var child = _studentBusiness.GetChildById(childId, UserInfo);
                if (child != null)
                {
                    birthDate = child.BirthDate;
                }
            }

            var total = 0;
            IList<ObservableReportModel> list = new List<ObservableReportModel>();
            if (birthDate != CommonAgent.MinDate)
            {
                list = _observableBusiness.SearchObervableReports2(studentId, childId, birthDate, sort, order, first, count, out total);
            }
            var result = new { total = total, data = list };
            return JsonHelper.SerializeObject(result);
        }

        public ActionResult SchoolReport(int studentId = 0)
        {
            int total = 0;
            IList<StudentModel> list = new List<StudentModel>();
            if (UserInfo.Role == Role.Parent)
            {
                list = _studentBusiness.SearchStudents(UserInfo.Parent.ID, "FirstName", "ASC", 0, 10, out total);
            }
            if (studentId == 0 && list.Count > 0)
                return new RedirectResult("SchoolReport?studentId=" + list[0].ID);
            else
            {
                if (list.Count > 0 && UserInfo.Parent.ParentStudents.All(c => c.StudentId != studentId))
                {
                    return new RedirectResult("SchoolReport?studentId=" + list[0].ID);
                }
            }
            ViewBag.currentStudentId = studentId;
            ViewBag.StudentList = list;
            return View();
        }


        [CLIUrlAuthorize(Account = Authority.All, PageId = PagesModel.Administrative, Anonymity = Anonymous.Verified)]
        [ValidateInput(false)]
        public string SearchSchoolReport(int studentId = 0, string sort = "CreatedOn", string order = "DESC", int first = 0, int count = 10)
        {
            var total = 0;
            IList<ParentReportModel> list = new List<ParentReportModel>();
            if (studentId > 0)
            {
                var findEntity = _studentBusiness.GetStudentEntity(studentId, UserInfo);
                if (findEntity != null)
                {
                    list = _reportBusiness.SearchParentReports(findEntity.BirthDate, c => c.StudentId == studentId, sort, order, first, count, out total);
                }
            }
            var result = new { total = total, data = list };
            return JsonHelper.SerializeObject(result);
        }

        public string DeleteReport(int ID)
        {
            var res = _observableBusiness.DeleteReport(ID);
            var response = new PostFormResponse();
            response.Success = res.ResultType == OperationResultType.Success;
            response.Message = res.Message;
            return JsonHelper.SerializeObject(response);
        }

        public ActionResult FindChildReport()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [CLIUrlAuthorizeAttribute(Account = Authority.All, PageId = PagesModel.Administrative, Anonymity = Anonymous.Logined)]
        public string SaveParentStudentRelation(StudentListModel parentChild)
        {
            var response = new PostFormResponse();
            if (ModelState.IsValid)
            {
                StudentEntity student = _studentBusiness.GetStudentByCode(parentChild.ParentId, parentChild.ChildFirstName, parentChild.ChildLastName);
                if (student != null)
                {
                    OperationResult result = new OperationResult(OperationResultType.Success);
                    //保存数据到Child表
                    ChildEntity childEntity = _studentBusiness.GetChildByStudentId(student.ID);
                    if (childEntity == null)
                    {
                        childEntity = new ChildEntity();
                        childEntity.FirstName = parentChild.ChildFirstName;
                        childEntity.LastName = parentChild.ChildLastName;
                        childEntity.BirthDate = student.BirthDate;
                        childEntity.StudentId = student.ID;
                        childEntity.PINCode = parentChild.ParentId;
                        childEntity.CreatedOn = DateTime.Now;
                        childEntity.SchoolCity = "";
                        childEntity.SchoolZip = "";
                        childEntity.UpdatedOn = DateTime.Now;
                        result = _studentBusiness.InsertChild(childEntity);
                        response.Success = result.ResultType == OperationResultType.Success;
                        if (!response.Success)
                        {
                            response.Message = "Something is wrong when check child information.";
                            return JsonHelper.SerializeObject(response);
                        }
                    }

                    //保存Parent与Child关系
                    int parentId = UserInfo.Parent.ID;
                    if (!_studentBusiness.ExistParentChild(parentId, childEntity.ID))
                    {
                        ParentChildEntity parentChildRelation = new ParentChildEntity();
                        parentChildRelation.ChildId = childEntity.ID;
                        parentChildRelation.ParentId = parentId;
                        result = _studentBusiness.InsertParentChild(parentChildRelation);
                        response.Success = result.ResultType == OperationResultType.Success;
                        if (!response.Success)
                        {
                            response.Message = "Something is wrong when check the relation between child and parent.";
                            return JsonHelper.SerializeObject(response);
                        }
                    }

                    //是否已经存在该关系
                    bool ifexist = _userBusiness.IsExistStudent(parentId, student.ID);
                    if (!ifexist) //不存在时
                    {
                        ParentStudentRelationEntity parentStudent = new ParentStudentRelationEntity();
                        parentStudent.ParentId = parentId;
                        parentStudent.StudentId = student.ID;
                        parentStudent.Relation = ParentRelation.Other;
                        parentStudent.RelationOther = "";

                        foreach (var item in student.SchoolRelations)
                        {
                            var communityId = item.School.CommunitySchoolRelations.FirstOrDefault(
                                o => o.Community.BasicCommunityId == item.School.BasicSchool.CommunityId) == null
                                ? 0
                                : item.School.CommunitySchoolRelations.FirstOrDefault(
                                    o => o.Community.BasicCommunityId == item.School.BasicSchool.CommunityId)
                                    .CommunityId;
                            if (!_userBusiness.IsExistUserSchool(UserInfo.ID, communityId, item.SchoolId))
                            {
                                UserComSchRelationEntity parentCommunitySchool = new UserComSchRelationEntity();
                                parentCommunitySchool.CommunityId = communityId;
                                parentCommunitySchool.SchoolId = item.SchoolId;
                                parentCommunitySchool.Status = EntityStatus.Active;
                                parentCommunitySchool.AccessType = AccessType.FullAccess;
                                parentCommunitySchool.CreatedBy = UserInfo.ID;
                                parentCommunitySchool.UpdatedBy = UserInfo.ID;
                                parentCommunitySchool.CreatedOn = DateTime.Now;
                                parentCommunitySchool.UpdatedOn = DateTime.Now;
                                UserInfo.UserCommunitySchools.Add(parentCommunitySchool);
                                _userBusiness.UpdateParent(UserInfo.Parent);
                            }
                        }
                        result = _userBusiness.InsertParentStudentRelation(parentStudent);
                        response.Success = result.ResultType == OperationResultType.Success;
                        response.Message = result.Message;
                        response.Data = null;
                    }
                    else
                    {
                        response.Success = false;
                        response.Message = "The student has existed in your list!";
                    }
                }
                else
                {
                    response.Success = false;
                    response.Message = "The student does not exist, please enter the correct information.";
                }
            }
            return JsonHelper.SerializeObject(response);
        }
    }
}