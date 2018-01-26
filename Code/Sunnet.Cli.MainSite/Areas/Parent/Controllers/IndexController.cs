using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using StructureMap;
using StructureMap.Query;
using Sunnet.Cli.Business.Common;
using Sunnet.Cli.Business.Communities;
using Sunnet.Cli.Business.Schools;
using Sunnet.Cli.Business.Schools.Models;
using Sunnet.Cli.Business.Students;
using Sunnet.Cli.Business.Students.Models;
using Sunnet.Cli.Business.Users;
using Sunnet.Cli.Core.Common.Enums;
using Sunnet.Cli.Core.Students.Entities;
using Sunnet.Cli.Core.Students.Enums;
using Sunnet.Cli.Core.Users.Entities;
using Sunnet.Cli.Core.Users.Enums;
using Sunnet.Cli.MainSite.Controllers;
using Sunnet.Cli.UIBase.Filters;
using Sunnet.Cli.UIBase.Models;
using Sunnet.Framework;
using Sunnet.Framework.Core.Tool;
using Sunnet.Framework.EmailSender;
using Sunnet.Framework.Encrypt;
using Sunnet.Framework.Extensions;
using Sunnet.Framework.Helpers;
using Sunnet.Framework.Permission;

namespace Sunnet.Cli.MainSite.Areas.Parent.Controllers
{
    public class IndexController : BaseController
    {
        private readonly UserBusiness _userBusiness;
        private readonly StudentBusiness _studentBusiness;
        public IndexController()
        {
            _userBusiness = new UserBusiness(UnitWorkContext);
            _studentBusiness = new StudentBusiness(UnitWorkContext);
        }
        //
        // GET: /Parent/Index/
        #region Parent Management

        public ActionResult Index()
        {

            return View();
        }

        [CLIUrlAuthorizeAttribute(Account = Authority.Index, PageId = PagesModel.Parent_Management,
            Anonymity = Anonymous.Verified)]
        [ValidateInput(false)]
        public string SearchParentStudents(int communityId = -1, string communityName = "", int schoolId = -1,
            string schoolName = "", int classId = -1, string studentName = "", int parentStatus = -1, string parentName = "",
            string sort = "Name", string order = "Asc", int first = 0, int count = 10)
        {
            var total = 0;
            var expression = PredicateHelper.True<ParentStudentRelationEntity>();
            if (communityId >= 1)
                expression = expression.And(s => s.Student.SchoolRelations.Count
                    (r => r.School.CommunitySchoolRelations.Count
                        (c => c.CommunityId == communityId) > 0) > 0);
            else if (communityName != null && communityName.Trim() != string.Empty)
            {
                communityName = communityName.Trim();
                expression = expression.And(s => s.Student.SchoolRelations
                    .Count(r => r.School.CommunitySchoolRelations
                        .Count(c => c.Community.Name.Contains(communityName)) > 0) > 0);
            }
            if (schoolId >= 1)
                expression = expression.And(s => s.Student.SchoolRelations.Count(r => r.SchoolId == schoolId) > 0);
            else if (schoolName != null && schoolName.Trim() != string.Empty)
            {
                schoolName = schoolName.Trim();
                expression = expression.And(s => s.Student.SchoolRelations
                    .Count(r => r.School.Name.Contains(schoolName)) > 0);
            }
            if (classId >= 1)
                expression =
                    expression.And(s => s.Student.Classes.Count(o => o.ID == classId && o.IsDeleted == false) > 0);
            if (studentName != null && studentName.Trim() != string.Empty)
            {
                studentName = studentName.Trim();
                expression = expression.And(s => s.Student.FirstName.Contains(studentName)
                                                 || s.Student.MiddleName.Contains(studentName)
                                                 || s.Student.LastName.Contains(studentName));
            }
            if (parentStatus >= 1)
                expression = expression.And(s => (int)s.Parent.ParentStatus == parentStatus);
            if (parentName != null && parentName.Trim() != string.Empty)
            {
                parentName = parentName.Trim();
                expression = expression.And(s => s.Parent.UserInfo.FirstName.Contains(parentName)
                                                 || s.Parent.UserInfo.MiddleName.Contains(parentName)
                                                 || s.Parent.UserInfo.LastName.Contains(parentName));
            }
            var list = _userBusiness.SearchParentStudents(UserInfo, expression, sort, order, first, count, out total);
            var result = new { total = total, data = list };
            return JsonHelper.SerializeObject(result);
        }

        [CLIUrlAuthorizeAttribute(Account = Authority.Index, PageId = PagesModel.Parent_Management,
            Anonymity = Anonymous.Verified)]
        [ValidateInput(false)]
        public string SearchStudentsToAddParent(int communityId = -1, string communityName = "", int schoolId = -1,
            string schoolName = "", int classId = -1, string studentId = "", string studentName = "", int parentStatus = -1,
            string sort = "Name", string order = "Asc", int first = 0, int count = 10)
        {
            var total = 0;
            var expression = PredicateHelper.True<StudentEntity>();
            //只有entered into class的student能够add parent
            expression = expression.And(e => e.Classes.Any());
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

            if (classId >= 1)
                expression = expression.And(s => s.Classes.Count(o => o.ID == classId && o.IsDeleted == false) > 0);

            if (studentId != null && studentId.Trim() != string.Empty)
            {
                studentId = studentId.Trim();
                expression = expression.And(s => s.StudentId.Contains(studentId));
            }

            if (studentName != null && studentName.Trim() != string.Empty)
            {
                studentName = studentName.Trim();
                expression = expression.And(s => s.FirstName.Contains(studentName)
                                                 || s.MiddleName.Contains(studentName)
                                                 || s.LastName.Contains(studentName));
            }

            if (parentStatus >= 1)
                expression = expression.And(s => (int)s.Status == parentStatus);

            var list = _studentBusiness.SearchStudentsToAddParents(UserInfo, expression, sort, order, first, count,
                out total);

            var result = new { total = total, data = list };
            return JsonHelper.SerializeObject(result);
        }

        [CLIUrlAuthorizeAttribute(Account = Authority.Index, PageId = PagesModel.Parent_Management,
            Anonymity = Anonymous.Verified)]
        public ActionResult AddParents()
        {
            ViewBag.StatusJson = JsonHelper.SerializeObject(EntityStatus.Active.ToList());
            return View();
        }

        [HttpPost]
        [CLIUrlAuthorizeAttribute(Account = Authority.Index, PageId = PagesModel.Parent_Management, Anonymity = Anonymous.Verified)]
        public string AddParents(string parents, bool isInvite)
        {
            var response = new PostFormResponse();
            OperationResult res = new OperationResult(OperationResultType.Success);
            List<ParentStudentListModel> listParent = JsonHelper.DeserializeObject<List<ParentStudentListModel>>(parents);

            foreach (ParentStudentListModel model in listParent)
            {
                var findParent = _userBusiness.GetParent(model.ParentFirstName, model.ParentLastName,
                    model.ParentPrimaryEmail);
                var findChild = _studentBusiness.GetChild(model.ChildFirstName, model.ChildLastName, model.BirthDate);
                ParentChildEntity findParentChildRelation = null;
                if (findParent != null && findChild != null)
                {
                    findParentChildRelation = _studentBusiness.GetParentChild(findParent.ID, findChild.ID);
                }
                //如果已经存在Student和Parent的关系，修改Parent的信息
                if (model.ID > 0)
                {
                    ParentEntity parent = _userBusiness.GetParent(model.ParentId);
                    parent.UserInfo.FirstName = model.ParentFirstName;
                    parent.UserInfo.LastName = model.ParentLastName;
                    parent.UserInfo.PrimaryEmailAddress = model.ParentPrimaryEmail;
                    res = _userBusiness.UpdateParent(parent);
                    if (res.ResultType == OperationResultType.Success && isInvite)
                    {
                        parent.ParentStatus = ParentStatus.Invited;
                        parent.UpdatedOn = DateTime.Now;
                        res = _userBusiness.UpdateParent(parent);

                        ParentInvite(parent);
                    }
                }
                else
                {
                    //如果parent 不存在，child 也不存在，创建 parent , child , parent-child relation , parent-student relation
                    #region 如果parent 不存在，child 也不存在，创建 parent , child , parent-child relation , parent-student relation
                    if (findParent == null && findChild == null)
                    {

                        /*
                         
                          var studentFirstSchool = _studentBusiness.GetStudent(model.StudentId);
                        if (studentFirstSchool != null)
                        {
                           var  school = studentFirstSchool.SchoolRelations.FirstOrDefault().School;
                            if (school != null)
                            {
                                newChild.SchoolCity = school.City;
                                newChild.SchoolZip = school.Zip;
                                newChild.PINCode = model.ParentCode;
                                newChild.StudentId = model.StudentId;
                                newChild.SchoolId = school.ID;
                            }
                            
                          
                        }
                         */

                        ChildEntity newChild = new ChildEntity();
                        newChild.FirstName = model.ChildFirstName;
                        newChild.LastName = model.ChildLastName;
                        newChild.BirthDate = model.BirthDate;
                        newChild.SchoolCity = "";
                        newChild.SchoolZip = "";
                        newChild.PINCode = model.ParentCode;
                        newChild.StudentId = model.StudentId;
                        newChild.SchoolId = 0;
                        res = _studentBusiness.InsertChild(newChild);
                        if (res.ResultType == OperationResultType.Success)
                        {
                            ParentEntity parent = new ParentEntity();
                            parent.ParentId = "";
                            parent.SchoolYear = CommonAgent.SchoolYear;
                            parent.ParentStatus = ParentStatus.NotApplicable;
                            parent.UserInfo = new UserBaseEntity();
                            parent.UserInfo.FirstName = model.ParentFirstName;
                            parent.UserInfo.LastName = model.ParentLastName;
                            parent.UserInfo.PrimaryEmailAddress = model.ParentPrimaryEmail;
                            parent.UserInfo.Status = EntityStatus.Active;
                            parent.UserInfo.StatusDate = DateTime.Now;
                            parent.UserInfo.Role = Role.Parent;

                            parent.ParentChilds = new List<ParentChildEntity>();
                            parent.ParentChilds.Add(new ParentChildEntity()
                            {
                                ChildId = newChild.ID
                            });
                            parent.ParentStudents = new List<ParentStudentRelationEntity>();
                            parent.ParentStudents.Add(new ParentStudentRelationEntity()
                            {
                                StudentId = model.StudentId,
                                Relation = 0,
                                RelationOther = ""
                            });
                            parent.UserInfo.UserCommunitySchools = new List<UserComSchRelationEntity>();
                            if (model.StudentId > 0)
                            {
                                StudentEntity student = _studentBusiness.GetStudent(model.StudentId);
                                if (student != null)
                                {
                                    foreach (var item in student.SchoolRelations)
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
                                        parentCommunitySchool.CreatedBy = UserInfo.ID;
                                        parentCommunitySchool.UpdatedBy = UserInfo.ID;
                                        parentCommunitySchool.CreatedOn = DateTime.Now;
                                        parentCommunitySchool.UpdatedOn = DateTime.Now;
                                        parent.UserInfo.UserCommunitySchools.Add(parentCommunitySchool);
                                    }
                                }
                            }
                            var community = new CommunityBusiness().GetCommunity("CLI Parent Community");
                            if (community != null)
                            {
                                UserComSchRelationEntity parentCommunitySchool = new UserComSchRelationEntity();
                                parentCommunitySchool.CommunityId = community.ID;
                                parentCommunitySchool.SchoolId = 0;
                                parentCommunitySchool.Status = EntityStatus.Active;
                                parentCommunitySchool.AccessType = AccessType.FullAccess;
                                parentCommunitySchool.CreatedBy = UserInfo.ID;
                                parentCommunitySchool.UpdatedBy = UserInfo.ID;
                                parentCommunitySchool.CreatedOn = DateTime.Now;
                                parentCommunitySchool.UpdatedOn = DateTime.Now;
                                parent.UserInfo.UserCommunitySchools.Add(parentCommunitySchool);
                            }
                            res = _userBusiness.InsertParent(parent);
                            if (res.ResultType == OperationResultType.Success && isInvite)
                            {
                                parent.ParentStatus = ParentStatus.Invited;
                                parent.UpdatedOn = DateTime.Now;
                                res = _userBusiness.UpdateParent(parent);

                                ParentInvite(parent);
                            }
                        }


                    }


                    #endregion

                    //如果parent 不存在，child  存在，Parent Child 关系不存在 ,更新child studentId字段,创建 parent， parent-child relation , parent-student relation
                    #region 如果parent 不存在，child  存在，Parent Child 关系不存在 ，更新child studentId字段,创建 parent， parent-child relation , parent-student relation
                    if (findParent == null && findChild != null)
                    {
                        findChild.StudentId = model.StudentId;
                        findChild.PINCode = model.ParentCode;
                        res = _studentBusiness.UpdateChild(findChild);

                        if (res.ResultType == OperationResultType.Success)
                        {
                            ParentEntity parent = new ParentEntity();
                            parent.ParentId = "";
                            parent.SchoolYear = CommonAgent.SchoolYear;
                            parent.ParentStatus = ParentStatus.NotApplicable;
                            parent.UserInfo = new UserBaseEntity();
                            parent.UserInfo.FirstName = model.ParentFirstName;
                            parent.UserInfo.LastName = model.ParentLastName;
                            parent.UserInfo.PrimaryEmailAddress = model.ParentPrimaryEmail;
                            parent.UserInfo.Status = EntityStatus.Active;
                            parent.UserInfo.StatusDate = DateTime.Now;
                            parent.UserInfo.Role = Role.Parent;

                            parent.ParentChilds = new List<ParentChildEntity>();
                            parent.ParentChilds.Add(new ParentChildEntity()
                            {
                                ChildId = findChild.ID
                            });
                            parent.ParentStudents = new List<ParentStudentRelationEntity>();
                            parent.ParentStudents.Add(new ParentStudentRelationEntity()
                            {
                                StudentId = model.StudentId,
                                Relation = 0,
                                RelationOther = ""
                            });
                            parent.UserInfo.UserCommunitySchools = new List<UserComSchRelationEntity>();
                            if (model.StudentId > 0)
                            {
                                StudentEntity student = _studentBusiness.GetStudent(model.StudentId);
                                if (student != null)
                                {
                                    foreach (var item in student.SchoolRelations)
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
                                        parentCommunitySchool.CreatedBy = UserInfo.ID;
                                        parentCommunitySchool.UpdatedBy = UserInfo.ID;
                                        parentCommunitySchool.CreatedOn = DateTime.Now;
                                        parentCommunitySchool.UpdatedOn = DateTime.Now;
                                        parent.UserInfo.UserCommunitySchools.Add(parentCommunitySchool);
                                    }
                                }
                            }
                            var community = new CommunityBusiness().GetCommunity("CLI Parent Community");
                            if (community != null)
                            {
                                UserComSchRelationEntity parentCommunitySchool = new UserComSchRelationEntity();
                                parentCommunitySchool.CommunityId = community.ID;
                                parentCommunitySchool.SchoolId = 0;
                                parentCommunitySchool.Status = EntityStatus.Active;
                                parentCommunitySchool.AccessType = AccessType.FullAccess;
                                parentCommunitySchool.CreatedBy = UserInfo.ID;
                                parentCommunitySchool.UpdatedBy = UserInfo.ID;
                                parentCommunitySchool.CreatedOn = DateTime.Now;
                                parentCommunitySchool.UpdatedOn = DateTime.Now;
                                parent.UserInfo.UserCommunitySchools.Add(parentCommunitySchool);
                            }
                            res = _userBusiness.InsertParent(parent);
                            if (res.ResultType == OperationResultType.Success && isInvite)
                            {
                                parent.ParentStatus = ParentStatus.Invited;
                                parent.UpdatedOn = DateTime.Now;
                                res = _userBusiness.UpdateParent(parent);

                                ParentInvite(parent);
                            }
                        }

                    }

                    #endregion

                    //如果parent 存在，child 不存在，  创建 child  parent-child relation , parent-student relation
                    #region  如果parent 存在，child 不存在，  创建 child  parent-child relation , parent-student relation

                    if (findParent != null && findChild == null)
                    {
                        ChildEntity newChild = new ChildEntity();
                        newChild.FirstName = model.ChildFirstName;
                        newChild.LastName = model.ChildLastName;
                        newChild.BirthDate = model.BirthDate;
                        newChild.SchoolCity = "";
                        newChild.SchoolZip = "";
                        newChild.SchoolId = 0;
                        newChild.PINCode = model.ParentCode;
                        newChild.StudentId = model.StudentId;
                        res = _studentBusiness.InsertChild(newChild);
                        if (res.ResultType == OperationResultType.Success)
                        {
                            ParentChildEntity childRelation = new ParentChildEntity();
                            childRelation.ParentId = findParent.ID;
                            childRelation.ChildId = newChild.ID;
                            res = _studentBusiness.InsertParentChild(childRelation);
                        }
                        if (res.ResultType == OperationResultType.Success)
                        {
                            ParentStudentRelationEntity studentRelation = new ParentStudentRelationEntity();
                            studentRelation.StudentId = model.StudentId;
                            studentRelation.ParentId = findParent.ID;
                            studentRelation.Relation = 0;
                            studentRelation.RelationOther = "";
                            res = _userBusiness.InsertParentStudentRelation(studentRelation);
                        }
                        if (res.ResultType == OperationResultType.Success && isInvite)
                        {
                            findParent.ParentStatus = ParentStatus.Invited;
                            findParent.UpdatedOn = DateTime.Now;
                            res = _userBusiness.UpdateParent(findParent);

                            //ParentInvite(findParent);已经存在 则不再发邮件
                        }


                    }

                    #endregion

                    //如果parent 存在，child  存在，Parent Child 关系不存在 ，更新child studentId字段,创建 parent-child relation , parent-student relation
                    #region 如果parent 存在，child  存在，Parent Child 关系不存在 ，更新child studentId字段,创建 parent-child relation , parent-student relation

                    if (findParent != null && findChild != null && findParentChildRelation == null)
                    {

                        findChild.StudentId = model.StudentId;
                        findChild.PINCode = model.ParentCode;
                        res = _studentBusiness.UpdateChild(findChild);
                        if (res.ResultType == OperationResultType.Success)
                        {

                            ParentChildEntity childRelation = new ParentChildEntity();
                            childRelation.ParentId = findParent.ID;
                            childRelation.ChildId = findChild.ID;
                            res = _studentBusiness.InsertParentChild(childRelation);
                        }
                        if (res.ResultType == OperationResultType.Success)
                        {
                            ParentStudentRelationEntity studentRelation = new ParentStudentRelationEntity();
                            studentRelation.StudentId = model.StudentId;
                            studentRelation.ParentId = findParent.ID;
                            studentRelation.Relation = 0;
                            studentRelation.RelationOther = "";
                            res = _userBusiness.InsertParentStudentRelation(studentRelation);
                        }
                        if (res.ResultType == OperationResultType.Success && isInvite)
                        {
                            findParent.ParentStatus = ParentStatus.Invited;
                            findParent.UpdatedOn = DateTime.Now;
                            res = _userBusiness.UpdateParent(findParent);

                            //ParentInvite(findParent);已经存在 则不再发邮件
                        }
                    }
                    #endregion

                    //如果parent 存在，child  存在，Parent Child 关系存在 ，更新child studentId字段,创建  parent-student relation
                    #region 如果parent 存在，child  存在，Parent Child 关系存在 ，更新child studentId字段,创建  parent-student relation

                    if (findParent != null && findChild != null && findParentChildRelation != null)
                    {

                        findChild.StudentId = model.StudentId;
                        findChild.PINCode = model.ParentCode;
                        res = _studentBusiness.UpdateChild(findChild);
                        if (res.ResultType == OperationResultType.Success)
                        {
                            ParentStudentRelationEntity studentRelation = new ParentStudentRelationEntity();
                            studentRelation.StudentId = model.StudentId;
                            studentRelation.ParentId = findParent.ID;
                            studentRelation.Relation = 0;
                            studentRelation.RelationOther = "";
                            res = _userBusiness.InsertParentStudentRelation(studentRelation);
                        }
                        if (res.ResultType == OperationResultType.Success && isInvite)
                        {
                            findParent.ParentStatus = ParentStatus.Invited;
                            findParent.UpdatedOn = DateTime.Now;
                            res = _userBusiness.UpdateParent(findParent);

                            // ParentInvite(findParent); 已经存在 则不再发邮件
                        }
                    }
                    #endregion

                    if (res.ResultType == OperationResultType.Error)
                        break;
                }
            }
            response.Success = res.ResultType == OperationResultType.Success;
            response.Message = res.Message;
            return JsonHelper.SerializeObject(response);
        }

        [CLIUrlAuthorizeAttribute(Account = Authority.All, PageId = PagesModel.Parent_Management, Anonymity = Anonymous.Logined)]
        public void ParentInvite(ParentEntity parent)
        {
            EmailTemplete template = XmlHelper.GetEmailTemplete("TeacherInvitation_Template.xml");
            string param = parent.UserInfo.ID.ToString() + "," + DateTime.Now.ToString();
            string encryptParam = ObjectFactory.GetInstance<IEncrypt>().Encrypt(param);
            string link = SFConfig.MainSiteDomain + "Home/InviteVerification/"
                + System.Web.HttpContext.Current.Server.UrlEncode(encryptParam);
            string emailBody = template.Body.Replace("{FirstName}", parent.UserInfo.FirstName)
            .Replace("{LastName}", parent.UserInfo.LastName)
            .Replace("{InviteLink}", "<a style='text-decoration: underline; cursor:pointer; color: #008000;' href='" + link + "'>Click here</a>")
            .Replace("{StaticDomain}", SFConfig.StaticDomain)
            .Replace("{CliEngageUrl}", SFConfig.MainSiteDomain);
            SendEmail(parent.UserInfo.PrimaryEmailAddress, template.Subject, emailBody);
        }

        [CLIUrlAuthorizeAttribute(Account = Authority.All, PageId = PagesModel.Parent_Management, Anonymity = Anonymous.Logined)]
        public string ParentInviteById(int parentId)
        {
            ParentEntity parent = _userBusiness.GetParent(parentId);
            UserBaseEntity user = parent.UserInfo;
            user.Parent.ParentStatus = ParentStatus.Invited;
            _userBusiness.UpdateUser(user);
            ParentInvite(parent);
            return user.Parent.ParentStatus.ToDescription();
        }

        public void SendEmail(string to, string subject, string emailBody)
        {
            var emailSender = ObjectFactory.GetInstance<IEmailSender>();
            new SendHandler(() => emailSender.SendMail(to, subject, emailBody)).BeginInvoke(null, null);
        }
        private delegate void SendHandler();

        [CLIUrlAuthorizeAttribute(Account = Authority.All, PageId = PagesModel.Parent_Management,
            Anonymity = Anonymous.Logined)]
        public string Disassociate(int parentId, int studentId)
        {
            ParentEntity parent = _userBusiness.GetParent(parentId);
            var response = new PostFormResponse();
            OperationResult result = new OperationResult(OperationResultType.Success);

            //先删除parent和community,school的关系
            StudentEntity student = _studentBusiness.GetStudent(studentId);
            List<int> communityIds = new List<int>();
            List<int> schoolIds = new List<int>();
            if (student != null)
            {
                foreach (var item in student.SchoolRelations)
                {
                    var communityId = item.School.CommunitySchoolRelations.FirstOrDefault(
                        o => o.Community.BasicCommunityId == item.School.BasicSchool.CommunityId) == null
                        ? 0
                        : item.School.CommunitySchoolRelations.FirstOrDefault(
                            o => o.Community.BasicCommunityId == item.School.BasicSchool.CommunityId)
                            .CommunityId;
                    if (
                        !parent.ParentStudents.Any(
                            e =>
                                e.StudentId != student.ID &&
                                e.Student.SchoolRelations.Any(s => s.SchoolId == item.SchoolId)))
                    {
                        schoolIds.Add(item.SchoolId);
                        communityIds.Add(communityId);
                    }
                }
            }
            _userBusiness.DeleteUserSchoolRelations(parent.UserInfo.ID, communityIds, schoolIds);

            result = _userBusiness.DeleteParentStudent(parentId, studentId);

            if (result.ResultType == OperationResultType.Success)
            {
                var findChild = _studentBusiness.GetChildByStudentId(studentId);
                if (findChild != null)
                {
                    findChild.StudentId = 0;
                    findChild.PINCode = "";
                    result = _studentBusiness.UpdateChild(findChild);
                }
            }


            response.Success = result.ResultType == OperationResultType.Success;
            response.Data = "";
            response.Message = result.Message;
            return JsonHelper.SerializeObject(response);
        }

        #endregion

        #region My Connections
        public ActionResult MyConnections()
        {
            if (UserInfo.Parent == null)
            {
                return Redirect("/Error/nonauthorized");
            }
            ParentEntity parentEntity = _userBusiness.GetParent(UserInfo.Parent.ID);
            List<int> studentIds = _userBusiness.GetStudentIDbyParentId(parentEntity.ID);
            ViewBag.StudentList = _studentBusiness.GetStudentsGetIds(studentIds);
            var parentCommunity = UserInfo.Parent.UserInfo.UserCommunitySchools.Select(e => new AssignSchoolModel()
            {
                SchoolName = e.School != null ? e.School.Name : "",
                CommunityName = e.Community != null ? e.Community.Name : "",
                CreatedOn = e.CreatedOn
            }).ToList();
            ViewBag.ParentCommunity = parentCommunity;
            return View(parentEntity);
        }

        [CLIUrlAuthorize(Account = Authority.All, PageId = PagesModel.Administrative, Anonymity = Anonymous.Logined)]
        public ActionResult AddChild()
        {
            ChildListModel childModel = new ChildListModel();
            return View(childModel);
        }

        [CLIUrlAuthorizeAttribute(Account = Authority.All, PageId = PagesModel.Administrative, Anonymity = Anonymous.Logined)]
        public string MyChildreds(string sort = "FirstName", string order = "Asc", int first = 0, int count = 10)
        {
            var total = 0;
            var list = _studentBusiness.SearchParentChilds(UserInfo.Parent.ID, sort, order, first, count, out total);
            var result = new { total = total, data = list };
            return JsonHelper.SerializeObject(result);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [CLIUrlAuthorizeAttribute(Account = Authority.All, PageId = PagesModel.Administrative,
            Anonymity = Anonymous.Logined)]
        public string SaveParent(ParentEntity parentEntity)
        {
            var response = new PostFormResponse();
            if (response.Success = ModelState.IsValid)
            {
                if (parentEntity.ID > 0)
                {
                    var parent = _userBusiness.GetParent(UserInfo.Parent.ID);
                    parent.UserInfo.FirstName = parentEntity.UserInfo.FirstName;
                    parent.UserInfo.LastName = parentEntity.UserInfo.LastName;
                    parent.UserInfo.MiddleName = parentEntity.UserInfo.MiddleName;
                    parent.UserInfo.PrimaryEmailAddress = parentEntity.UserInfo.PrimaryEmailAddress;
                    OperationResult result = new OperationResult(OperationResultType.Success);
                    result = _userBusiness.UpdateParent(parent);
                    response.Success = result.ResultType == OperationResultType.Success;
                    response.Message = result.Message;
                }
            }
            response.ModelState = ModelState;
            return JsonHelper.SerializeObject(response);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [CLIUrlAuthorizeAttribute(Account = Authority.All, PageId = PagesModel.Administrative, Anonymity = Anonymous.Logined)]
        public string SaveParentChild(ChildListModel model)
        {
            var response = new PostFormResponse();
            OperationResult result = new OperationResult(OperationResultType.Success);
            ModelState.Remove("SchoolId");
            if (ModelState.IsValid)
            {
                var newSchoolId = 0;
                var newSchoolName = "";
                var newSchoolCity = "";
                var newSchoolZip = "";
                StudentEntity entity = _studentBusiness.GetStudent(model.FirstName, model.LastName, model.BirthDate);
                ChildEntity findChild = new ChildEntity();
                findChild.PINCode = "";
                if (model.ID > 0)
                    findChild = _studentBusiness.GetChildById(model.ID, UserInfo);
                if (!string.IsNullOrEmpty(model.PINCode) && findChild != null && findChild.PINCode != model.PINCode)
                {
                    if (entity == null)
                    {
                        response.Success = false;
                        response.Data = "warning";
                        response.Message = "The child’s name, date of birth or PIN does not match. Please enter as shown on the PIN page provided by the teacher.";
                        return JsonHelper.SerializeObject(response);
                    }
                    if (entity.ParentCode != model.PINCode)
                    {
                        response.Success = false;
                        response.Data = "warning";
                        response.Message = "The child’s name, date of birth or PIN does not match. Please enter as shown on the PIN page provided by the teacher.";
                        return JsonHelper.SerializeObject(response);
                    }
                }

                //通过firstname、lastname,birthdate,parent code查找student实体
                StudentEntity student = _studentBusiness.GetStudentByPIN(model.PINCode, model.FirstName, model.LastName,
                    model.BirthDate);
                if (student != null)
                {
                    //是否已经存在该关系
                    bool ifexist = _userBusiness.IsExistStudent(UserInfo.Parent.ID, student.ID);
                    if (!ifexist) //不存在时
                    {
                        ParentStudentRelationEntity parentStudent = new ParentStudentRelationEntity();
                        parentStudent.ParentId = UserInfo.Parent.ID;
                        parentStudent.StudentId = student.ID;
                        parentStudent.Relation = ParentRelation.Other;
                        parentStudent.RelationOther = "";

                        foreach (var item in student.SchoolRelations)
                        {
                            newSchoolId = item.School.ID;
                            newSchoolName = item.School.Name;
                            newSchoolCity = item.School.City;
                            newSchoolZip = item.School.Zip;

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
                    }
                    else
                    {
                        response.Success = true;
                        response.Data = "warning";
                        response.Message = "The student has been exist.";
                    }
                }

                if (model.ID > 0)
                {
                    ChildEntity childEntity = _studentBusiness.ChildListModelToEntity(model);

                    if (findChild != null && findChild.PINCode != "" && findChild.PINCode != childEntity.PINCode)
                    {
                        Disassociate(UserInfo.Parent.ID, findChild.StudentId);
                        childEntity.StudentId = student != null ? student.ID : 0;

                        childEntity.SchoolCity = newSchoolCity;
                        childEntity.SchoolId = newSchoolId;
                        childEntity.SchoolZip = newSchoolZip;

                    }
                    else if (findChild != null && findChild.PINCode == "" && childEntity.PINCode != "")
                    {
                        childEntity.StudentId = student != null ? student.ID : 0;
                        childEntity.SchoolCity = newSchoolCity;
                        childEntity.SchoolId = newSchoolId;
                        childEntity.SchoolZip = newSchoolZip;
                    }
                    else
                    {
                        childEntity.StudentId = findChild.StudentId;
                        childEntity.PINCode = findChild.PINCode;

                    }
                    result = _studentBusiness.UpdateChild(childEntity);
                    response.Success = result.ResultType == OperationResultType.Success;
                }
                else
                {
                    //保存数据到Child表
                    ChildEntity childEntity = _studentBusiness.GetChild(model.FirstName, model.LastName, model.BirthDate);
                    if (childEntity == null)
                    {
                        childEntity = _studentBusiness.ChildListModelToEntity(model);
                        childEntity.StudentId = student != null ? student.ID : 0;
                        if (childEntity.StudentId > 0)
                        {
                            childEntity.SchoolCity = newSchoolCity;
                            childEntity.SchoolId = newSchoolId;
                            childEntity.SchoolZip = newSchoolZip;
                        }
                        result = _studentBusiness.InsertChild(childEntity);
                        response.Success = result.ResultType == OperationResultType.Success;
                    }
                    else
                    {

                        response.Success = true;
                        response.Data = "warning";
                        response.Message = "The child has been exist.";
                    }

                    //保存Parent与Child关系
                    int parentId = UserInfo.Parent.ID;
                    if (!_studentBusiness.ExistParentChild(parentId, childEntity.ID))
                    {
                        ParentChildEntity parentChild = new ParentChildEntity();
                        parentChild.ChildId = childEntity.ID;
                        parentChild.ParentId = parentId;
                        result = _studentBusiness.InsertParentChild(parentChild);
                        response.Success = result.ResultType == OperationResultType.Success;
                        if (response.Success)
                        {
                            response.Success = true;
                            response.Data = "";
                            response.Message = "";
                        }
                    }
                }
            }
            return JsonHelper.SerializeObject(response);
        }
        #endregion

        #region Edit View Child
        [CLIUrlAuthorize(Account = Authority.All, PageId = PagesModel.Administrative, Anonymity = Anonymous.Logined)]
        public ActionResult EditChild(int id)
        {
            ChildEntity entity = _studentBusiness.GetChildById(id, UserInfo);
            ChildListModel childModel = _studentBusiness.ChildEntityToListModel(entity);
            return View(childModel);
        }

        [CLIUrlAuthorize(Account = Authority.All, PageId = PagesModel.Administrative, Anonymity = Anonymous.Logined)]
        public ActionResult ViewChild(int id)
        {
            ChildEntity entity = _studentBusiness.GetChildById(id, UserInfo);
            ChildListModel childModel = _studentBusiness.ChildEntityToListModel(entity);
            if (entity.StudentId > 0)
            {
                var student = _studentBusiness.GetStudentEntity(entity.StudentId, UserInfo);
                ChildListModel child = new ChildListModel();
                child.FirstName = student.FirstName;
                child.LastName = student.LastName;
                child.BirthDate = student.BirthDate;
                var schoolStudentRelationEntity = student.SchoolRelations.FirstOrDefault();
                if (schoolStudentRelationEntity != null)
                    child.SchoolCity = schoolStudentRelationEntity.School.City;
                var studentRelationEntity = student.SchoolRelations.FirstOrDefault();
                if (studentRelationEntity != null)
                    child.SchoolZip = studentRelationEntity.School.Zip;
                return View(child);
            }
            else
            {
                return View(childModel);
            }




        }




        [CLIUrlAuthorize(Account = Authority.All, PageId = PagesModel.Administrative, Anonymity = Anonymous.Logined)]
        public string DeleteChild(int id)
        {
            var response = new PostFormResponse();
            OperationResult result = new OperationResult(OperationResultType.Success);
            ChildEntity entity = _studentBusiness.GetChildById(id, UserInfo);

            if (entity != null)
            {
                if (entity.StudentId > 0)
                {
                    Disassociate(UserInfo.Parent.ID, entity.StudentId);
                    entity.StudentId = 0;
                }
                entity.IsDeleted = true;
                result = _studentBusiness.UpdateChild(entity);
            }
            else
            {
                result.ResultType = OperationResultType.Error;
                result.Message = "This child does not exist.";
            }

            response.Success = result.ResultType == OperationResultType.Success;
            response.Message = result.Message;

            return JsonHelper.SerializeObject(response);

        }

        #endregion
    }
}