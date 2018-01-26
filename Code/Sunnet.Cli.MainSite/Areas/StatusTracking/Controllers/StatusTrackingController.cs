using Sunnet.Cli.Business.StatusTracking;
using Sunnet.Cli.Business.StatusTracking.Models;
using Sunnet.Cli.Business.Users;
using Sunnet.Cli.Core.StatusTracking.Entities;
using Sunnet.Cli.Core.Users.Entities;
using Sunnet.Cli.Core.Users.Enums;
using Sunnet.Cli.MainSite.Controllers;
using Sunnet.Cli.UIBase.Filters;
using Sunnet.Cli.UIBase.Models;
using Sunnet.Framework.Helpers;
using Sunnet.Framework.Permission;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web.Mvc;
using Sunnet.Cli.Core.StatusTracking.Enums;
using Sunnet.Framework.Core.Tool;
using Sunnet.Framework;
using Sunnet.Cli.MainSite.Models;
using Sunnet.Cli.Business.Communities;
using Sunnet.Cli.Business.Schools;
using Sunnet.Cli.Core.Communities.Entities;
using Sunnet.Cli.Core.Schools.Entities;

namespace Sunnet.Cli.MainSite.Areas.StatusTracking.Controllers
{
    public class StatusTrackingController : BaseController
    {
        StatusTrackingBusiness _statusTrackingBusiness;
        UserBusiness _userBusiness;
        CommunityBusiness _communityBusiness;
        SchoolBusiness _schoolBusiness;
        public StatusTrackingController()
        {
            _statusTrackingBusiness = new StatusTrackingBusiness();
            _userBusiness = new UserBusiness();
            _communityBusiness = new CommunityBusiness();
            _schoolBusiness = new SchoolBusiness();
        }


        [CLIUrlAuthorizeAttribute(Account = Authority.Index, PageId = PagesModel.StatusTracking, Anonymity = Anonymous.Verified)]
        public ActionResult Index()
        {
            return View();
        }

        [CLIUrlAuthorizeAttribute(Account = Authority.Index, PageId = PagesModel.StatusTracking, Anonymity = Anonymous.Verified)]
        public string Search(string approver, string requestor, string supposedapprover,
            int status = -1, int communityId = -1, int schoolId = -1, int resendnumber = -1, int type = -1,
            string sort = "ID", string order = "Desc", int first = 0, int count = 10)
        {
            int total = 0;
            int parentId = 0;
            List<StatusTrackingModel> list = new List<StatusTrackingModel>();
            Expression<Func<StatusTrackingEntity, bool>> trackingContition = PredicateHelper.True<StatusTrackingEntity>();
            #region  条件筛选
            List<int> primarySchools =
            _schoolBusiness.GetPrimarySchoolsByComId(
                UserInfo.UserCommunitySchools.Select(e => e.CommunityId).ToList()).Select(e => e.ID).ToList();
            if (UserInfo.Role > Role.Auditor)  //CLI用户可查看所有，外部用户根据所属的Community和school查找
            {
                switch (UserInfo.Role)
                {
                    case Role.Content_personnel:
                    case Role.Statisticians:
                    case Role.Administrative_personnel:
                    case Role.Intervention_manager:
                    case Role.Video_coding_analyst:
                    case Role.Intervention_support_personnel:
                    case Role.Coordinator:
                    case Role.Mentor_coach:
                        // 接收的数据
                        trackingContition = trackingContition.And(o => (o.UserInfo_SupposedApprover.Role == Role.Statewide ||
                           o.UserInfo_SupposedApprover.Role == Role.Community || o.UserInfo_SupposedApprover.Role == Role.District_Community_Specialist ||
                            o.UserInfo_SupposedApprover.Role == Role.Principal || o.UserInfo_SupposedApprover.Role == Role.TRS_Specialist ||
                            o.UserInfo_SupposedApprover.Role == Role.School_Specialist || o.UserInfo_SupposedApprover.Role == Role.Teacher) &&
                            (o.UserInfo_SupposedApprover.UserCommunitySchools.Any(p => p.Community.UserCommunitySchools.Any(x => x.UserId == UserInfo.ID))
                            || o.UserInfo_SupposedApprover.UserCommunitySchools.Any(p => p.School.CommunitySchoolRelations.Any(q => 
                                q.Community.UserCommunitySchools.Any(r => r.UserId == UserInfo.ID))))
                            && o.Type == StatusType.Invitation);
                        //自己发送的数据
                        trackingContition = trackingContition.Or(o => o.RequestorId == UserInfo.ID);
                        break;
                    case Role.Statewide:
                        // 接收的数据
                        trackingContition = trackingContition.And(o => (o.UserInfo_SupposedApprover.Role == Role.Statewide ||
                           o.UserInfo_SupposedApprover.Role == Role.Community || o.UserInfo_SupposedApprover.Role == Role.District_Community_Specialist ||
                            o.UserInfo_SupposedApprover.Role == Role.Principal || o.UserInfo_SupposedApprover.Role == Role.TRS_Specialist ||
                            o.UserInfo_SupposedApprover.Role == Role.School_Specialist || o.UserInfo_SupposedApprover.Role == Role.Teacher) &&
                            (o.UserInfo_SupposedApprover.UserCommunitySchools.Any(p => p.Community.UserCommunitySchools.Any(x => x.UserId == UserInfo.ID))
                            || o.UserInfo_SupposedApprover.UserCommunitySchools.Any(p => primarySchools.Contains(p.SchoolId)))
                            && o.Type == StatusType.Invitation);
                        //自己发送的数据
                        trackingContition = trackingContition.Or(o => o.RequestorId == UserInfo.ID);
                        break;
                    case Role.Community:
                        // 接收的数据
                        trackingContition = trackingContition.And(o =>
                           (o.UserInfo_SupposedApprover.Role == Role.Community || o.UserInfo_SupposedApprover.Role == Role.District_Community_Specialist ||
                            o.UserInfo_SupposedApprover.Role == Role.Principal || o.UserInfo_SupposedApprover.Role == Role.TRS_Specialist ||
                            o.UserInfo_SupposedApprover.Role == Role.School_Specialist || o.UserInfo_SupposedApprover.Role == Role.Teacher) &&
                            (o.UserInfo_SupposedApprover.UserCommunitySchools.Any(p => p.Community.UserCommunitySchools.Any(x => x.UserId == UserInfo.ID))
                            || o.UserInfo_SupposedApprover.UserCommunitySchools.Any(p => primarySchools.Contains(p.SchoolId)))
                            && o.Type == StatusType.Invitation);
                        //自己发送的数据
                        trackingContition = trackingContition.Or(o => o.RequestorId == UserInfo.ID);
                        break;
                    case Role.District_Community_Delegate:
                        if (UserInfo.CommunityUser != null)
                        {
                            UserBaseEntity parentUser = _userBusiness.GetUser(UserInfo.CommunityUser.ParentId);
                            List<int> primarySchoolsParent =
                            _schoolBusiness.GetPrimarySchoolsByComId(
                                parentUser.UserCommunitySchools.Select(e => e.CommunityId).ToList()).Select(e => e.ID).ToList();
                            trackingContition = trackingContition.And(o =>
                                    (o.UserInfo_SupposedApprover.Role == Role.District_Community_Specialist ||
                                     o.UserInfo_SupposedApprover.Role == Role.Principal || o.UserInfo_SupposedApprover.Role == Role.TRS_Specialist ||
                                     o.UserInfo_SupposedApprover.Role == Role.School_Specialist || o.UserInfo_SupposedApprover.Role == Role.Teacher) &&
                                     (o.UserInfo_SupposedApprover.UserCommunitySchools.Any(p => p.Community.UserCommunitySchools.Any
                                        (x => x.UserId == UserInfo.CommunityUser.ParentId))
                                        || o.UserInfo_SupposedApprover.UserCommunitySchools.Any(p => primarySchoolsParent.Contains(p.SchoolId)))
                                        && o.Type == StatusType.Invitation);
                            //自己发送的数据
                            trackingContition = trackingContition.Or(o => o.RequestorId == UserInfo.CommunityUser.ParentId);
                            parentId = UserInfo.CommunityUser.ParentId;
                        }
                        break;
                    case Role.District_Community_Specialist:
                        trackingContition = trackingContition.And(o =>
                                (o.UserInfo_SupposedApprover.Role == Role.District_Community_Specialist ||
                                 o.UserInfo_SupposedApprover.Role == Role.Principal || o.UserInfo_SupposedApprover.Role == Role.TRS_Specialist ||
                                 o.UserInfo_SupposedApprover.Role == Role.School_Specialist || o.UserInfo_SupposedApprover.Role == Role.Teacher) &&
                                 (o.UserInfo_SupposedApprover.UserCommunitySchools.Any(p => p.Community.UserCommunitySchools.Any(x => x.UserId == UserInfo.ID))
                                 || o.UserInfo_SupposedApprover.UserCommunitySchools.Any(p => primarySchools.Contains(p.SchoolId)))
                                 && o.Type == StatusType.Invitation);
                        //自己发送的数据
                        trackingContition = trackingContition.Or(o => o.RequestorId == UserInfo.ID);
                        break;
                    case Role.Community_Specialist_Delegate:
                        if (UserInfo.CommunityUser != null)
                        {
                            UserBaseEntity parentUser = _userBusiness.GetUser(UserInfo.CommunityUser.ParentId);
                            List<int> primarySchoolsParent =
                            _schoolBusiness.GetPrimarySchoolsByComId(
                                parentUser.UserCommunitySchools.Select(e => e.CommunityId).ToList()).Select(e => e.ID).ToList();
                            trackingContition = trackingContition.And(o =>
                                    (o.UserInfo_SupposedApprover.Role == Role.Principal || o.UserInfo_SupposedApprover.Role == Role.TRS_Specialist ||
                                     o.UserInfo_SupposedApprover.Role == Role.School_Specialist || o.UserInfo_SupposedApprover.Role == Role.Teacher) &&
                                     (o.UserInfo_SupposedApprover.UserCommunitySchools.Any(p => p.Community.UserCommunitySchools.Any
                                        (x => x.UserId == UserInfo.CommunityUser.ParentId))
                                        || o.UserInfo_SupposedApprover.UserCommunitySchools.Any(p => primarySchoolsParent.Contains(p.SchoolId)))
                                        && o.Type == StatusType.Invitation);
                            //自己发送的数据
                            trackingContition = trackingContition.Or(o => o.RequestorId == UserInfo.CommunityUser.ParentId);
                            parentId = UserInfo.CommunityUser.ParentId;
                        }
                        break;
                    case Role.TRS_Specialist:
                        trackingContition = trackingContition.And(o =>
                            (o.UserInfo_SupposedApprover.Role == Role.TRS_Specialist ||
                             o.UserInfo_SupposedApprover.Role == Role.Teacher)
                             && o.UserInfo_SupposedApprover.UserCommunitySchools.Any(p => p.School.UserCommunitySchools.Any(x => x.UserId == UserInfo.ID))
                             && o.Type == StatusType.Invitation);
                        //自己发送的数据
                        trackingContition = trackingContition.Or(o => o.RequestorId == UserInfo.ID);
                        break;
                    case Role.School_Specialist:
                        trackingContition = trackingContition.And(o =>
                            (o.UserInfo_SupposedApprover.Role == Role.School_Specialist ||
                             o.UserInfo_SupposedApprover.Role == Role.Teacher)
                             && o.UserInfo_SupposedApprover.UserCommunitySchools.Any(p => p.School.UserCommunitySchools.Any(x => x.UserId == UserInfo.ID))
                             && o.Type == StatusType.Invitation);
                        //自己发送的数据
                        trackingContition = trackingContition.Or(o => o.RequestorId == UserInfo.ID);
                        break;
                    case Role.TRS_Specialist_Delegate:
                    case Role.School_Specialist_Delegate:
                        if (UserInfo.Principal != null)
                        {
                            trackingContition = trackingContition.And(o =>
                                o.UserInfo_SupposedApprover.Role == Role.Teacher &&
                                o.UserInfo_SupposedApprover.UserCommunitySchools.Any(p => p.School.UserCommunitySchools.Any
                                        (x => x.UserId == UserInfo.Principal.ParentId)) && o.Type == StatusType.Invitation);

                            //自己发送的数据
                            trackingContition = trackingContition.Or(o => o.RequestorId == UserInfo.Principal.ParentId);
                            parentId = UserInfo.Principal.ParentId;
                        }
                        break;
                    case Role.Principal:
                        trackingContition = trackingContition.And(o =>
                            (o.UserInfo_SupposedApprover.Role == Role.Principal || o.UserInfo_SupposedApprover.Role == Role.TRS_Specialist
                             || o.UserInfo_SupposedApprover.Role == Role.School_Specialist || o.UserInfo_SupposedApprover.Role == Role.Teacher)
                             && o.UserInfo_SupposedApprover.UserCommunitySchools.Any(p => p.School.UserCommunitySchools.Any(x => x.UserId == UserInfo.ID))
                             && o.Type == StatusType.Invitation);
                        //自己发送的数据
                        trackingContition = trackingContition.Or(o => o.RequestorId == UserInfo.ID);

                        //查看添加School邀请的数据
                        trackingContition = trackingContition.Or(o => o.SchoolInfo == null ?
                            false : (o.SchoolInfo.UserCommunitySchools.Any(r => r.UserId == UserInfo.ID) && o.Type == StatusType.AddSchool));

                        break;
                    case Role.Principal_Delegate:
                        if (UserInfo.Principal != null)
                        {
                            trackingContition = trackingContition.And(o =>
                                (o.UserInfo_SupposedApprover.Role == Role.TRS_Specialist || o.UserInfo_SupposedApprover.Role == Role.School_Specialist
                                || o.UserInfo_SupposedApprover.Role == Role.Teacher)
                                && o.UserInfo_SupposedApprover.UserCommunitySchools.Any(p => p.School.UserCommunitySchools.Any
                                        (x => x.UserId == UserInfo.Principal.ParentId)) && o.Type == StatusType.Invitation);

                            //自己发送的数据
                            trackingContition = trackingContition.Or(o => o.RequestorId == UserInfo.Principal.ParentId);

                            //查看添加School邀请的数据
                            trackingContition = trackingContition.Or(o => o.SchoolInfo == null ?
                                true : (o.SchoolInfo.UserCommunitySchools.Any(r => r.UserId == UserInfo.Principal.ParentId) && o.Type == StatusType.AddSchool));
                            parentId = UserInfo.Principal.ParentId;
                        }
                        break;
                    case Role.Teacher:
                        trackingContition = trackingContition.And(o =>
                            o.UserInfo_SupposedApprover.Role == Role.Teacher
                            && o.UserInfo_SupposedApprover.UserCommunitySchools.Any(p => p.School.UserCommunitySchools.Any(x => x.UserId == UserInfo.ID))
                            && o.Type == StatusType.Invitation);
                        break;
                    default:
                        trackingContition = trackingContition.And(r => false);
                        break;
                }
            }
            //todo 需要Sam和PM确认
            //else
            //{
            //    //对于Invitation类型，可查看所有数据
            //    trackingContition = trackingContition.And(o => o.Type == StatusType.Invitation);

            //    //对于Add School类型，可查看没有SchoolId或者学校没有Principal的数据
            //    trackingContition = trackingContition.Or(o => o.Type == StatusType.AddSchool
            //        && (o.SchoolInfo == null || !o.SchoolInfo.UserCommunitySchools.Any(r => r.User.Role == Role.Principal)));
            //}
            if (!string.IsNullOrEmpty(approver))
                trackingContition = trackingContition.And(r => r.UserInfo_Approver.FirstName.Contains(approver) || r.UserInfo_Approver.LastName.Contains(approver)
                    || r.UserInfo_Approver.GoogleId.Contains(approver) || r.UserInfo_Approver.PrimaryEmailAddress.Contains(approver));

            if (!string.IsNullOrEmpty(requestor))
                trackingContition = trackingContition.And(r => r.UserInfo_Requestor.FirstName.Contains(requestor) || r.UserInfo_Requestor.LastName.Contains(requestor)
                    || r.UserInfo_Requestor.GoogleId.Contains(requestor) || r.UserInfo_Requestor.PrimaryEmailAddress.Contains(requestor));

            if (!string.IsNullOrEmpty(supposedapprover))
                trackingContition = trackingContition.And(r => r.UserInfo_SupposedApprover.FirstName.Contains(supposedapprover)
                    || r.UserInfo_SupposedApprover.LastName.Contains(supposedapprover)
                    || r.UserInfo_SupposedApprover.GoogleId.Contains(supposedapprover) || r.UserInfo_SupposedApprover.PrimaryEmailAddress.Contains(supposedapprover));

            if (status > 0)
            {
                if (status == (int)StatusSearchEnum.Expired)
                    trackingContition = trackingContition.And(r => r.Status == StatusEnum.Pending && r.ExpiredTime < DateTime.Now);
                else
                {
                    if (status == (int)StatusSearchEnum.Pending)
                        trackingContition = trackingContition.And(r => r.Status == StatusEnum.Pending && r.ExpiredTime >= DateTime.Now);
                    else
                        trackingContition = trackingContition.And(r => (int)r.Status == status);
                }
            }

            if (communityId > 0)
                trackingContition = trackingContition.And(r => r.CommunityId == communityId);

            if (schoolId > 0)
                trackingContition = trackingContition.And(r => r.SchoolId == schoolId);

            if (resendnumber > 0)
                trackingContition = trackingContition.And(r => r.ResendNumber == resendnumber);

            if (type > 0)
                trackingContition = trackingContition.And(r => (int)r.Type == type);

            #endregion
            list = _statusTrackingBusiness.GetTrackingList(trackingContition, UserInfo, parentId, sort, order, first, count, out total);
            var result = new { total = total, data = list };
            return JsonHelper.SerializeObject(result);
        }

        [CLIUrlAuthorizeAttribute(Account = Authority.Index, PageId = PagesModel.StatusTracking, Anonymity = Anonymous.Verified)]
        public ActionResult DetailView(int id)
        {
            StatusTrackingModel trackingModel = _statusTrackingBusiness.GetStatusTrackingModel(id);
            if (trackingModel != null)
            {
                UserBaseEntity user_Requestor = _userBusiness.GetUser(trackingModel.RequestorId);
                if (user_Requestor.Role <= Role.Statewide) //内部用户没有Community
                {
                    trackingModel.RequestorCommunities = " ";
                }
                else
                {
                    IEnumerable<string> CommunityNames = new List<string>();
                    CommunityNames = user_Requestor.UserCommunitySchools.Where(r => r.CommunityId > 0).Select(x => x.Community.Name);
                    trackingModel.RequestorCommunities = string.Join(", ", CommunityNames);
                }
            }
            return View(trackingModel);
        }


        [CLIUrlAuthorizeAttribute(Account = Authority.Index, PageId = PagesModel.StatusTracking, Anonymity = Anonymous.Verified)]
        public string Approve(int id)
        {
            StatusTrackingEntity entity = _statusTrackingBusiness.GetTracking(id);
            PostFormResponse response = new PostFormResponse();
            if (entity != null && entity.Status == StatusEnum.Pending)
            {
                StatusTrackingModel model = _statusTrackingBusiness.GetStatusTrackingModelById(id);
                entity.Status = StatusEnum.Accepted;
                entity.ApprovedTime = DateTime.Now;
                entity.ApproverId = UserInfo.ID;
                entity.UpdatedBy = UserInfo.ID;
                entity.UpdatedOn = DateTime.Now;
                OperationResult result = _statusTrackingBusiness.UpdateTracking(entity);
                if (result.ResultType == OperationResultType.Success)
                {
                    if (!(model.SupposedUserCommunitySchools
                        .Any(e => e.CommunityId == entity.CommunityId && e.SchoolId == entity.SchoolId)))
                    {
                        result = _userBusiness.InsertUserCommunitySchoolRelations(entity.SupposedApproverId, UserInfo.ID, entity.CommunityId, entity.SchoolId.Value);
                        if (result.ResultType == OperationResultType.Success)
                        {
                            EmailTemplete template =
                                XmlHelper.GetEmailTemplete("NoPermission_Invite_Result_Template.xml");
                            string emailBody = template.Body.Replace("{StaticDomain}", SFConfig.StaticDomain)
                                .Replace("{Sender}", model.UserInfo_Requestor.FirstName)
                                .Replace("{Recipient}", model.UserInfo_SupposedApprover.FirstName + " " + model.UserInfo_SupposedApprover.LastName)
                                .Replace("{Result}", "accepted");
                            string subject = template.Subject.Replace("{Result}", "accepted");
                            _userBusiness.SendEmail(model.UserInfo_Requestor.PrimaryEmailAddress, subject, emailBody);
                        }
                    }
                }
                response.Success = result.ResultType == OperationResultType.Success;
                response.Message = result.Message;
            }
            else
            {
                response.Success = false;
            }
            return JsonHelper.SerializeObject(response);
        }


        [CLIUrlAuthorizeAttribute(Account = Authority.Index, PageId = PagesModel.StatusTracking, Anonymity = Anonymous.Verified)]
        public string Deny(int id)
        {
            StatusTrackingEntity entity = _statusTrackingBusiness.GetTracking(id);
            PostFormResponse response = new PostFormResponse();
            if (entity != null && entity.Status == StatusEnum.Pending)
            {
                StatusTrackingModel model = _statusTrackingBusiness.GetStatusTrackingModelById(id);
                entity.Status = StatusEnum.Denied;
                entity.DeniedTime = DateTime.Now;
                entity.UpdatedBy = UserInfo.ID;
                entity.UpdatedOn = DateTime.Now;
                entity.ApproverId = UserInfo.ID;
                OperationResult result = _statusTrackingBusiness.UpdateTracking(entity);
                if (result.ResultType == OperationResultType.Success)
                {
                    EmailTemplete template = XmlHelper.GetEmailTemplete("NoPermission_Invite_Result_Template.xml");
                    string emailBody = template.Body.Replace("{StaticDomain}", SFConfig.StaticDomain)
                        .Replace("{Sender}", model.UserInfo_Requestor.FirstName)
                        .Replace("{Recipient}", model.UserInfo_SupposedApprover.FirstName + " " + model.UserInfo_SupposedApprover.LastName)
                        .Replace("{Result}", "declined");
                    string subject = template.Subject.Replace("{Result}", "declined");
                    _userBusiness.SendEmail(model.UserInfo_Requestor.PrimaryEmailAddress, subject, emailBody);
                }
                response.Success = result.ResultType == OperationResultType.Success;
                response.Message = result.Message;
            }
            else
            {
                response.Success = false;
            }
            return JsonHelper.SerializeObject(response);
        }


        [CLIUrlAuthorizeAttribute(Account = Authority.Index, PageId = PagesModel.StatusTracking, Anonymity = Anonymous.Verified)]
        public string Resend(int id)
        {
            StatusTrackingEntity entity = _statusTrackingBusiness.GetTracking(id);
            PostFormResponse response = new PostFormResponse();
            if (entity != null)
            {
                entity.Status = StatusEnum.Pending;
                entity.ExpiredTime = DateTime.Now.AddDays(SFConfig.ExpirationTime);
                entity.ResendTime = DateTime.Now;
                entity.RequestTime = DateTime.Now;
                entity.ResendNumber += 1;
                entity.UpdatedBy = UserInfo.ID;
                entity.UpdatedOn = DateTime.Now;
                OperationResult result = _statusTrackingBusiness.UpdateTracking(entity);
                if (result.ResultType == OperationResultType.Success)
                {
                    string systemObject = "";
                    if (entity.SchoolId > 0)
                    {
                        SchoolEntity school = _schoolBusiness.GetSchool(entity.SchoolId.Value);
                        if (school != null)
                            systemObject = school.Name;
                    }
                    else
                    {
                        CommunityEntity community = _communityBusiness.GetCommunity(entity.CommunityId);
                        if (community != null)
                            systemObject = community.Name;
                    }
                    if (!string.IsNullOrEmpty(systemObject))
                    {
                        EmailTemplete template = XmlHelper.GetEmailTemplete("NoPermission_Invite_Template.xml");

                        var approveLink = new LinkModel()
                        {
                            RoleType = entity.UserInfo_SupposedApprover.Role,
                            Host = SFConfig.MainSiteDomain,
                            Path = "Approve/",
                            Sender = UserInfo.ID,
                            Recipient = entity.UserInfo_SupposedApprover.ID
                        };
                        approveLink.Others.Add("CommunityId", entity.CommunityId);
                        approveLink.Others.Add("SchoolId", entity.SchoolId);

                        var denyLink = new LinkModel()
                        {
                            RoleType = entity.UserInfo_SupposedApprover.Role,
                            Host = SFConfig.MainSiteDomain,
                            Path = "Deny/",
                            Sender = UserInfo.ID,
                            Recipient = entity.UserInfo_SupposedApprover.ID
                        };
                        denyLink.Others.Add("CommunityId", entity.CommunityId);
                        denyLink.Others.Add("SchoolId", entity.SchoolId);

                        string emailBody = template.Body.Replace("{StaticDomain}", SFConfig.StaticDomain)
                            .Replace("{Recipient}", entity.UserInfo_SupposedApprover.FirstName + " " + entity.UserInfo_SupposedApprover.LastName)
                            .Replace("{Sender}", UserInfo.FirstName + " " + UserInfo.LastName)
                            .Replace("{SystemObject}", systemObject)
                            .Replace("{Approve}", approveLink.ToString())
                            .Replace("{Deny}", denyLink.ToString());
                        _userBusiness.SendEmail(entity.UserInfo_SupposedApprover.PrimaryEmailAddress, template.Subject, emailBody);
                    }
                }
                response.Success = result.ResultType == OperationResultType.Success;
                response.Message = result.Message;
            }
            else
            {
                response.Success = false;
            }
            return JsonHelper.SerializeObject(response);
        }
    }
}