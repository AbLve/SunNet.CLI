using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

/**************************************************************************
 * Developer: 		Sam
 * Computer:		SAM-PC
 * Domain:			sam-pc
 * CreatedOn:		2014/8/19 19:07:20
 * Description:		Please input class summary
 * Version History:	Created,2014/8/19 19:07:20
 * 
 * 
 **************************************************************************/
using System.Reflection.Emit;
using System.Security.Cryptography;
using DocumentFormat.OpenXml.Office2010.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.SqlServer.Server;
using Newtonsoft.Json;
using StructureMap;
using Sunnet.Cli.Business.Common;
using Sunnet.Cli.Business.Common.Enum;
using Sunnet.Cli.Business.Communities;
using Sunnet.Cli.Business.Communities.Enums;
using Sunnet.Cli.Business.MasterData;
using Sunnet.Cli.Business.Schools.Models;
using Sunnet.Cli.Business.StatusTracking;
using Sunnet.Cli.Business.Students.Models;
using Sunnet.Cli.Business.Trs;
using Sunnet.Cli.Business.Trs.Models;
using Sunnet.Cli.Business.TRSClasses;
using Sunnet.Cli.Business.Users;
using Sunnet.Cli.Business.Users.Models;
using Sunnet.Cli.Core;
using Sunnet.Cli.Core.Ade;
using Sunnet.Cli.Core.Classes;
using Sunnet.Cli.Core.Classes.Interfaces;
using Sunnet.Cli.Core.Common.Enums;
using Sunnet.Cli.Core.Communities;
using Sunnet.Cli.Core.Communities.Entities;
using Sunnet.Cli.Core.MasterData.Configurations;
using Sunnet.Cli.Core.MasterData.Entities;
using Sunnet.Cli.Core.Schools;
using Sunnet.Cli.Core.Schools.Entities;
using Sunnet.Cli.Core.Schools.Enums;
using Sunnet.Cli.Core.StatusTracking.Entities;
using Sunnet.Cli.Core.StatusTracking.Enums;
using Sunnet.Cli.Core.Trs;
using Sunnet.Cli.Core.Users.Entities;
using Sunnet.Cli.Core.Users.Enums;
using Sunnet.Cli.Core.Students;
using Sunnet.Cli.Core.Students.Entities;
using Sunnet.Framework;
using Sunnet.Framework.Core.Tool;
using Sunnet.Framework.EmailSender;
using Sunnet.Framework.Encrypt;
using Sunnet.Framework.Extensions;
using Sunnet.Framework.Core.Extensions;
using LinqKit;
using Sunnet.Framework.Helpers;
using Sunnet.Cli.Business.Cpalls.Models;
using Sunnet.Cli.Business.Vcw.Models;
using Sunnet.Framework.Log;
using Sunnet.Cli.Business.Students;
using Sunnet.Cli.Business.Classes;
using Sunnet.Cli.Business.BUP.Model;
using Sunnet.Cli.Core.Trs.Entities;

namespace Sunnet.Cli.Business.Schools
{
    public class SchoolBusiness
    {
        private readonly ISchoolContract _schoolService;
        private ClassBusiness _classBus
        {
            get { return new ClassBusiness(); }
        }

        private TRSClassBusiness _trsClassBusiness
        {
            get { return new TRSClassBusiness(); }
        }

        private StudentBusiness _studentBus
        {
            get { return new StudentBusiness(); }
        }
        private readonly CommunityBusiness _communityBus;
        private UserBusiness _userBusiness
        {
            get { return new UserBusiness(); }
        }

        private TrsBusiness TrsBusiness { get { return new TrsBusiness(); } }

        public SchoolBusiness(EFUnitOfWorkContext unit = null)
        {
            _schoolService = DomainFacade.CreateSchoolService(unit);
            _communityBus = new CommunityBusiness(unit);
        }

        public SchoolEntity NewSchoolEntity()
        {
            SchoolEntity school = _schoolService.NewSchoolEntity();

            school.SchoolYear = CommonAgent.SchoolYear;
            school.StatusDate = DateTime.Now;
            school.TrsLastStatusChange = DateTime.Now;
            school.PhysicalAddress2 = "";
            school.MailingAddress2 = "";
            school.Latitude = "";
            school.Longitude = "";
            school.IspOther = "";
            school.Notes = "";
            school.SecondaryName = "";
            school.SecondaryEmail = "";
            school.SecondaryPhoneNumber = "";
            school.SchoolNumber = "";
            school.TrsAssessorId = 0;
            //    school.TrsProviderId = 0;
            school.StarDate = CommonAgent.MinDate;
            //   school.TrsReviewDate = CommonAgent.MinDate;
            school.TrsLastStatusChange = CommonAgent.MinDate;
            school.SchoolId = string.Empty;

            school.CreateBy = 0;
            school.UpdateBy = 0;
            school.CreateFrom = "";
            school.UpdateFrom = "";

            #region TRS
            school.TrsAssessorId = 0;
            school.TrsTaStatus = "";
            school.StarStatus = 0;
            school.DfpsNumber = "";
            school.OwnerFirstName = "";
            school.OwnerLastName = "";
            school.OwnerEmail = "";
            school.OwnerPhone = "";
            school.StarAssessmentType = 0;
            #endregion

            return school;
        }


        #region Status Track
        private OperationResult InsertStatusTrack(int communityId, int schoolId, int currentUserId, int receiverId, string link)
        {
            StatusTrackingBusiness statusTrackBus = new StatusTrackingBusiness();
            StatusTrackingEntity track = new StatusTrackingEntity();
            track.RequestorId = currentUserId;
            track.SupposedApproverId = receiverId;
            track.Status = StatusEnum.Pending;
            track.RequestTime = DateTime.Now;
            track.CommunityId = communityId;
            track.SchoolId = schoolId;
            track.ProcessAddress = link;
            track.ExpiredTime = DateTime.Now.AddDays(SFConfig.ExpirationTime);
            track.Type = StatusType.AddSchool;
            track.CreatedBy = currentUserId;
            track.UpdatedBy = currentUserId;
            track.CreatedOn = DateTime.Now;
            track.UpdatedOn = DateTime.Now;
            OperationResult res = statusTrackBus.AddTracking(track);
            return res;
        }
        private OperationResult UpdateStatusTrack(int communityId, int schoolId, int currentUserId, int receiverId, string link)
        {
            StatusTrackingBusiness statusTrackBus = new StatusTrackingBusiness();
            OperationResult res = new OperationResult(OperationResultType.Success);
            StatusTrackingEntity track = statusTrackBus.GetTrackingByUrl(link);
            if (track != null)
            {
                int trackID = track.ID;
                res = statusTrackBus.Resend(trackID, currentUserId);
            }
            return res;
        }
        private OperationResult ApproveStatusTrack(int currentUserId, string link)
        {
            StatusTrackingBusiness statusTrackBus = new StatusTrackingBusiness();
            OperationResult res = new OperationResult(OperationResultType.Success);
            StatusTrackingEntity track = statusTrackBus.GetTrackingByUrl(link);
            if (track != null)
            {
                int trackID = track.ID;
                res = statusTrackBus.Approve(trackID, currentUserId);
            }
            return res;
        }
        private OperationResult DenyStatusTrack(int currentUserId, string link)
        {
            StatusTrackingBusiness statusTrackBus = new StatusTrackingBusiness();
            OperationResult res = new OperationResult(OperationResultType.Success);
            StatusTrackingEntity track = statusTrackBus.GetTrackingByUrl(link);
            if (track != null)
            {
                int trackID = track.ID;
                res = statusTrackBus.Deny(trackID, currentUserId);
            }
            return res;
        }
        private bool IsPendingTrack(string link)
        {
            StatusTrackingBusiness statusTrackBus = new StatusTrackingBusiness();
            StatusTrackingEntity track = statusTrackBus.GetTrackingByUrl(link);
            if (track != null && track.Status == StatusEnum.Pending)
            {
                return true;
            }
            return false;
        }
        #endregion

        #region  Approve  Deny...

        /// <summary>
        ///  Process for School Not Found
        /// </summary>
        /// <param name="school"></param>
        /// <returns></returns>

        /// <summary>
        ///  Update Basic School CommunityId as the selected Community
        /// </summary>
        /// <returns></returns>
        private OperationResult UpdateBasicCommunityId(int communityId, BasicSchoolEntity basicSchool)
        {
            var result = new OperationResult(OperationResultType.Success);
            var community = _communityBus.GetCommunity(communityId);
            if (basicSchool == null)
            {
                result.ResultType = OperationResultType.Error;
                result.Message = "Basic School can not be null.";
            }
            else if (community == null)
            {
                result.ResultType = OperationResultType.Error;
                result.Message = "Community With No Basic Community.";
            }
            else
            {
                basicSchool.CommunityId = community.BasicCommunityId;
                result = UpdateBasicSchool(basicSchool);
            }
            return result;
        }
        //Send the request for approval to principal
        public OperationResult SendRequest(UserBaseEntity user, int schoolId, int communityId)
        {
            OperationResult res = new OperationResult(OperationResultType.Success);
            string emailSubject = user.FirstName + " " + user.LastName + " added a new school information";
            string approveUrl = "";
            string emailBody = GenerateEmailContent2(user, schoolId, communityId, out approveUrl);

            List<UserModel> principals = _userBusiness.GetPrincipalBySchoolId(schoolId);
            if (principals.Count > 0)
            {
                List<string> emailList = principals.Select(o => o.Email).Distinct().ToList();
                if (emailList.Count > 0)
                {
                    string emailTo = string.Join(";", emailList.ToArray());
                    res = SendEmail(emailTo, emailSubject, emailBody, false);
                    if (res.ResultType == OperationResultType.Success)
                    {
                        res = InsertStatusTrack(communityId, schoolId, user.ID, principals.FirstOrDefault().UserId, approveUrl);
                    }
                }
                else
                {
                    res = SendEmailToAdmin(communityId, schoolId, user.ID, approveUrl, emailSubject, emailBody, false);

                }
            }
            else
            {
                res = SendEmailToAdmin(communityId, schoolId, user.ID, approveUrl, emailSubject, emailBody, false);
            }
            return res;
        }
        public OperationResult ResendSelectedSchool(string encryptStr, UserBaseEntity user)
        {
            OperationResult res = new OperationResult(OperationResultType.Success);
            string emailSubject = user.FirstName + " " + user.LastName + " added a new school information";
            string approveUrl = "";
            var encrypt = ObjectFactory.GetInstance<IEncrypt>();
            string schoolIdStr = encrypt.Decrypt(encryptStr);
            string[] fields = schoolIdStr.Split('&');//basicSchoolId & creatorId & Expiration Time
            int schoolId = 0, selectedCommunityId = 0,
                createrId = 0;

            if (fields.Length != 4)
            {
                res.ResultType = OperationResultType.Error;
                res.Message = "School Status error.";
                return res;
            }
            else
            {
                int.TryParse(fields[0], out schoolId);
                int.TryParse(fields[1], out selectedCommunityId);
                int.TryParse(fields[2], out createrId);
            }
            string emailBody = GenerateEmailContent2(user, schoolId, selectedCommunityId, out approveUrl);
            List<UserModel> principals = _userBusiness.GetPrincipalBySchoolId(schoolId);
            if (principals.Count > 0)
            {
                List<string> emailList = principals.Select(o => o.Email).Distinct().ToList();
                if (emailList.Count > 0)
                {
                    string emailTo = string.Join(";", emailList.ToArray());
                    res = SendEmail(emailTo, emailSubject, emailBody, false);
                    if (res.ResultType == OperationResultType.Success)
                    {
                        res = UpdateStatusTrack(selectedCommunityId, schoolId, user.ID, principals.FirstOrDefault().UserId, approveUrl);
                    }
                }
                else
                {
                    res = ReSendEmailToAdmin(selectedCommunityId, schoolId, user.ID, approveUrl, emailSubject, emailBody, false);

                }
            }
            else
            {
                res = ReSendEmailToAdmin(selectedCommunityId, schoolId, user.ID, approveUrl, emailSubject, emailBody, false);
            }
            return res;
        }



        public OperationResult ResendBasicSchool(string encryptStr, UserBaseEntity user)
        {
            OperationResult result = new OperationResult(OperationResultType.Success);
            IEncrypt encrypt = ObjectFactory.GetInstance<IEncrypt>();

            int communityId = 0, basicSchoolId = 0, creatorId = 0;
            string[] fields = encrypt.Decrypt(encryptStr).Split('&');
            if (fields.Length != 4)
            {
                result.ResultType = OperationResultType.Error;
                result.Message = "Basic School Status error.";
                return result;
            }
            try
            {
                int.TryParse(fields[0], out communityId);
                int.TryParse(fields[1], out basicSchoolId);
                int.TryParse(fields[2], out creatorId);

                BasicSchoolEntity basicSchool = GetBasicSchoolById(basicSchoolId);
                if (basicSchool != null && basicSchool.Status == SchoolStatus.Pending)
                {
                    string approveUrl = "";
                    string emailSubject = "A new Basic School information is currently awaiting approval.";
                    string emailBody = GenerateEmailContent(user, communityId, basicSchool.ID, out approveUrl);
                    result = ReSendEmailToAdmin(communityId, 0, user.ID, approveUrl, emailSubject, emailBody, false);
                }
                else
                {
                    result.ResultType = OperationResultType.Error;
                    result.Message = "Basic School Status error.";
                }
            }
            catch (Exception ex)
            {
                result.ResultType = OperationResultType.Error;
                result.Message = ex.ToString();
            }

            return result;
        }

        public OperationResult ApproveBasicSchool(string encryptStr, string approveStr, int currentUserId)
        {
            OperationResult result = new OperationResult(OperationResultType.Success);
            IEncrypt encrypt = ObjectFactory.GetInstance<IEncrypt>();

            int communityId = 0, basicSchoolId = 0, creatorId = 0;
            string[] fields = encrypt.Decrypt(encryptStr).Split('&');
            if (fields.Length != 4)
            {
                result.ResultType = OperationResultType.Error;
                result.Message = "Basic School Status error.";
                return result;
            }
            if (!IsPendingTrack(approveStr))
            {
                result.ResultType = OperationResultType.Error;
                result.Message = "Request status error.";
                return result;
            }
            try
            {
                int.TryParse(fields[0], out communityId);
                int.TryParse(fields[1], out basicSchoolId);
                int.TryParse(fields[2], out creatorId);

                BasicSchoolEntity basicSchool = GetBasicSchoolById(basicSchoolId);
                if (basicSchool != null && basicSchool.Status == SchoolStatus.Pending)
                {
                    basicSchool.Status = SchoolStatus.Active;
                    basicSchool.UpdatedOn = DateTime.Now;
                    result = UpdateBasicSchool(basicSchool);
                    if (result.ResultType == OperationResultType.Success)
                    {
                        // Send Email Notification to Creator
                        UserBaseEntity creator = _userBusiness.GetUser(creatorId);
                        if (creator != null && !string.IsNullOrEmpty(creator.PrimaryEmailAddress))
                        {
                            string emailBody = EmailNoticeContent(creator, basicSchool.Name, "approved");
                            result = SendEmail(creator.PrimaryEmailAddress, "Your apply about new School is approved",
                                       emailBody, true);
                            result = ApproveStatusTrack(currentUserId, approveStr);
                        }
                    }
                }
                else
                {
                    result.ResultType = OperationResultType.Error;
                    result.Message = "Basic School Status error.";
                }
            }
            catch (Exception ex)
            {
                result.ResultType = OperationResultType.Error;
                result.Message = ex.ToString();
            }

            return result;
        }

        public OperationResult DenyBasicSchool(string encryptStr, string urlStr, int currentUserId)
        {
            OperationResult result = new OperationResult(OperationResultType.Success);
            IEncrypt encrypt = ObjectFactory.GetInstance<IEncrypt>();
            int basicSchoolId = 0, creatorId = 0, communityId = 0;
            string[] fields = encrypt.Decrypt(encryptStr).Split('&');
            if (fields.Length != 4)
            {
                result.ResultType = OperationResultType.Error;
                result.Message = "Basic School Status error.";
                return result;
            }
            if (!IsPendingTrack(urlStr))
            {
                result.ResultType = OperationResultType.Error;
                result.Message = "Request status error.";
                return result;
            }
            try
            {
                int.TryParse(fields[0], out communityId);
                int.TryParse(fields[1], out basicSchoolId);
                int.TryParse(fields[2], out creatorId);
                BasicSchoolEntity basicSchool = GetBasicSchoolById(basicSchoolId);
                if (basicSchool != null && basicSchool.Status == SchoolStatus.Pending)
                {
                    result = DeleteBasicSchool(basicSchool);
                    if (result.ResultType == OperationResultType.Success)
                    {
                        // Send Email Notification to Creator
                        UserBaseEntity creator = _userBusiness.GetUser(creatorId);
                        if (creator != null && !string.IsNullOrEmpty(creator.PrimaryEmailAddress))
                        {
                            string emailBody = EmailNoticeContent(creator, basicSchool.Name, "denied");
                            result = SendEmail(creator.PrimaryEmailAddress, "Your apply about new School is denied",
                                       emailBody, true);
                            result = DenyStatusTrack(currentUserId, urlStr);
                        }
                    }
                }
                else
                {
                    result.ResultType = OperationResultType.Error;
                    result.Message = "Basic School Status error.";
                }
            }
            catch (Exception ex)
            {
                result.ResultType = OperationResultType.Error;
                result.Message = ex.ToString();
            }

            return result;
        }

        public OperationResult ApproveSelectedSchool(string encryptStr, UserBaseEntity user, string urlStr)
        {
            OperationResult result = new OperationResult(OperationResultType.Success);
            IEncrypt encrypt = ObjectFactory.GetInstance<IEncrypt>();

            int schoolId = 0, selectedCommunityId = 0, creatorId = 0;
            string[] fields = encrypt.Decrypt(encryptStr).Split('&');

            if (fields.Length != 4 && (IsPendingTrack(urlStr)))
            {
                result.ResultType = OperationResultType.Error;
                result.Message = " School Status error.";
                return result;
            }
            if (!IsPendingTrack(urlStr))
            {
                result.ResultType = OperationResultType.Error;
                result.Message = "Request status error.";
                return result;
            }
            try
            {
                int.TryParse(fields[0], out schoolId);
                int.TryParse(fields[1], out selectedCommunityId);
                int.TryParse(fields[2], out creatorId);

                SchoolModel school = GetSchoolEntity(schoolId, user);
                if (school != null)
                {
                    int[] ids = GetAssignedCommIds(schoolId);
                    if (ids.Contains(selectedCommunityId))
                    {
                        result.ResultType = OperationResultType.Error;
                        result.Message = "The school and community relationship exsits.";
                    }
                    else
                    {
                        result = InsertCommunitySchoolRelations(user.ID, schoolId, new[] { selectedCommunityId });
                        if (result.ResultType == OperationResultType.Success)
                        {
                            if (school.Status == SchoolStatus.Pending)
                            {
                                school.Status = SchoolStatus.Active;
                                result = UpdateSchool(school, user.Role);
                            }

                        }
                        if (result.ResultType == OperationResultType.Success)
                        {
                            // Send Email Notification to Creator
                            UserBaseEntity creator = _userBusiness.GetUser(creatorId);
                            if (creator != null && !string.IsNullOrEmpty(creator.PrimaryEmailAddress))
                            {
                                string emailBody = EmailNoticeContent(creator, school.SchoolName, "approved");
                                SendEmail(creator.PrimaryEmailAddress, "Your apply about new School is approved",
                                           emailBody, true);
                                result = ApproveStatusTrack(user.ID, urlStr);
                            }
                        }
                    }

                }
                else
                {
                    result.ResultType = OperationResultType.Error;
                    result.Message = "School Status error.";
                }
            }
            catch (Exception ex)
            {
                result.ResultType = OperationResultType.Error;
                result.Message = ex.ToString();
            }

            return result;
        }

        public OperationResult DenySelectedSchool(string encryptStr, UserBaseEntity user, string urlStr)
        {
            OperationResult result = new OperationResult(OperationResultType.Success);
            IEncrypt encrypt = ObjectFactory.GetInstance<IEncrypt>();
            int schoolId = 0, creatorId = 0, selectedCommunityId = 0;
            string[] fields = encrypt.Decrypt(encryptStr).Split('&');
            if (fields.Length != 4)
            {
                result.ResultType = OperationResultType.Error;
                result.Message = "School Status error.";
                return result;
            }
            if (!IsPendingTrack(urlStr))
            {
                result.ResultType = OperationResultType.Error;
                result.Message = "Request status error.";
                return result;
            }
            try
            {
                int.TryParse(fields[0], out schoolId);
                int.TryParse(fields[1], out selectedCommunityId);
                int.TryParse(fields[2], out creatorId);
                SchoolModel school = GetSchoolEntity(schoolId, user);
                if (school != null)
                {
                    int[] ids = GetAssignedCommIds(schoolId);
                    if (ids.Contains(selectedCommunityId))
                    {
                        result.ResultType = OperationResultType.Error;
                        result.Message = "The school and community relationship exsits.";
                    }
                    else
                    {
                        // Change School and Basic School Status

                        // Send Email Notification to Creator
                        UserBaseEntity creator = _userBusiness.GetUser(creatorId);
                        if (creator != null && !string.IsNullOrEmpty(creator.PrimaryEmailAddress))
                        {
                            string emailBody = EmailNoticeContent(creator, school.SchoolName, "denied");
                            SendEmail(creator.PrimaryEmailAddress, "Your apply about new School is denied",
                                       emailBody, true);
                            result = DenyStatusTrack(user.ID, urlStr);
                        }
                        if (school.Status == SchoolStatus.Pending)
                        {
                            BasicSchoolEntity basicSchool = GetBasicSchoolById(school.BasicSchoolId);
                            basicSchool.Status = SchoolStatus.Active;
                            result = DeleteSchool(school.ID);
                            if (result.ResultType == OperationResultType.Success)
                                result = UpdateBasicSchool(basicSchool);
                        }

                    }
                }
                else
                {
                    result.ResultType = OperationResultType.Error;
                    result.Message = "School Status error.";
                }
            }
            catch (Exception ex)
            {
                result.ResultType = OperationResultType.Error;
                result.Message = ex.ToString();
            }

            return result;
        }

        #endregion

        #region Insert Methods

        private OperationResult AddUserSchoolRelation(int communityId, int schoolId, UserBaseEntity user)
        {
            //User Community School Relations

            OperationResult res = new OperationResult(OperationResultType.Success);
            int total;
            Expression<Func<UserBaseEntity, bool>> condition = o => true;
            condition = PredicateHelper.And(condition, (r => r.UserCommunitySchools.Any(c => c.CommunityId == communityId)));
            condition = PredicateHelper.And(condition, (r => r.Role == Role.Community || r.Role == Role.District_Community_Specialist || r.Role == Role.Statewide));
            condition = PredicateHelper.And(condition, (r => !(r.UserCommunitySchools.Any(c => c.SchoolId == schoolId))));

            List<UserBaseModel> userList = _userBusiness.GetUsers(user, condition, "UserId", "ASC", 0, int.MaxValue, out total);
            var userIdList = userList.Select(o => o.UserId).Distinct().ToList();
            if (userIdList != null && userIdList.Count > 0)
            {
                res = _userBusiness.InsertUserSchoolRelations(userIdList.ToArray(), user.ID, schoolId, AccessType.Primary);
            }
            return res;
        }

        public OperationResult AddNewSchool(int communityId, SchoolModel school, UserBaseEntity user)
        {
            //School not Found?
            OperationResult result = new OperationResult(OperationResultType.Success);
            if (school.BasicSchoolId == 0)//not Found
            {

                //result = InsertBasicSchool(communityId, school, user);//School.BasicSchoolId will be assigned to the new Id.
                //if (result.ResultType == OperationResultType.Success && user.Role == Role.Super_admin)
                //{
                //    AddNewSchool(communityId, school, user);//Process for claiming school
                //    // InsertRelationWithSchool(communityId, school, user);
                //}
                result.ResultType = OperationResultType.Error;
                result.Message = "Basic School is not found.";

            }
            else
            {
                BasicSchoolEntity basicSchool = GetBasicSchoolById(school.BasicSchoolId);

                // Does the selected school have a pre-define community?
                if (basicSchool.CommunityId <= 0)// has no pre-define community
                {
                    result = UpdateBasicCommunityId(communityId, basicSchool);// Update Basic School CommunityId as the selected Community
                    if (result.ResultType == OperationResultType.Success)
                    {
                        result = InsertRelationWithSchool(communityId, school, user);

                        //if (result.ResultType == OperationResultType.Success)
                        //    result = AddUserSchoolRelation(communityId, school.ID, user);
                    }
                }
                else
                {
                    //Is the pre-define community same with selected community
                    if (_communityBus.IsSameCommunity(communityId, basicSchool)) // Same
                    {
                        result = InsertRelationWithSchool(communityId, school, user);
                        //if (result.ResultType == OperationResultType.Success)
                        //    result = AddUserSchoolRelation(communityId, school.ID,user);
                    }
                    else
                    {
                        //Check permission
                        if ((int)user.Role <= 100)
                        {
                            result = InsertRelationWithSchool(communityId, school, user);
                        }
                        else
                        {
                            result.ResultType = OperationResultType.Error;
                            result.Message = "This school does not enroll into Engage, please contact administrator.";
                            //school.Status = SchoolStatus.Pending;
                            //result = InsertSchool(school, user.Role);
                            //if (result.ResultType == OperationResultType.Success)
                            //{
                            //    string arroveUrl = "";
                            //    string emailSubject = user.FirstName + " " + user.LastName + " added a new school information";
                            //    string emailBody = GenerateEmailContent2(user, school.ID, communityId, out arroveUrl);
                            //    result = SendEmailToAdmin(communityId, school.ID, user.ID, arroveUrl, emailSubject, emailBody, false);
                            //}
                        }
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// Insert School And Insert new relations
        /// </summary>
        /// <param name="school"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        private OperationResult InsertRelationWithSchool(int communityId, SchoolModel school, UserBaseEntity user)
        {
            OperationResult result = InsertSchool(school, user.Role);


            if (result.ResultType == OperationResultType.Success)
            {
                result = InsertCommunitySchoolRelations(user.ID, school.ID, new[] { communityId });
            }
            return result;
        }

        /// <summary>
        /// Insert basic school 
        /// </summary>
        /// <param name="basicSchool"></param>
        /// <returns></returns>
        public OperationResult InsertBasicSchool(BasicSchoolEntity basicSchool)
        {
            OperationResult result = new OperationResult(OperationResultType.Success);
            if (_schoolService.BasicSchools.Any(o => o.Name == basicSchool.Name))
            {
                result.ResultType = OperationResultType.Error;
                result.Message = "Basic School exsits.";
            }
            else
            {
                result = _schoolService.InsertBasicSchool(basicSchool);
            }
            return result;
        }
        /// <summary>
        /// Insert basic school and send email
        /// </summary>
        /// <param name="school">School Model</param>
        /// <param name="currentRole">Current User Role</param>
        /// <returns></returns>
        private OperationResult InsertBasicSchool(int communityId, SchoolModel school, UserBaseEntity user)
        {
            OperationResult result = new OperationResult(OperationResultType.Success);

            CommunityEntity community = _communityBus.GetCommunity(communityId);
            int basicCommunityId = 0;
            if (community != null)
                basicCommunityId = community.BasicCommunityId;
            BasicSchoolEntity basicSchool = new BasicSchoolEntity();
            basicSchool.Name = school.SchoolName;
            basicSchool.Status = SchoolStatus.Pending;
            basicSchool.Address1 = school.PhysicalAddress1;
            basicSchool.City = school.City;
            basicSchool.Zip = school.Zip;
            basicSchool.Phone = school.PhoneNumber;
            basicSchool.CountyId = school.CountyId;
            basicSchool.StateId = school.StateId;
            basicSchool.CommunityId = basicCommunityId;
            basicSchool.SchoolNumber = school.SchoolNumber;
            basicSchool.Status = (user.Role == Role.Super_admin) ? SchoolStatus.Active : SchoolStatus.Pending;
            result = InsertBasicSchool(basicSchool);

            //School BasicSchoolId will be assigned to the new Id.
            school.BasicSchoolId = basicSchool.ID;
            //SendEmail
            if (result.ResultType == OperationResultType.Success && (int)user.Role > 100)
            {
                string approveUrl = "";
                // Send Email
                string emailSubject = "A new Basic School information is currently awaiting approval.";
                string emailBody = GenerateEmailContent(user, communityId, basicSchool.ID, out approveUrl);
                result = SendEmailToAdmin(communityId, 0, user.ID, approveUrl, emailSubject, emailBody, false);
            }
            return result;
        }

        public OperationResult InsertSchool(SchoolModel model, Role role)
        {
            OperationResult result = new OperationResult(OperationResultType.Success);
            SchoolEntity school = NewSchoolEntity();
            model = InitSchoolByRole(model, role);
            school = SchoolModelToEntity(model, school);
            school.CreatedOn = DateTime.Now;
            school.UpdatedOn = DateTime.Now;


            BasicSchoolEntity basicSchool = _schoolService.GetBasicSchool(school.BasicSchoolId);
            SchoolEntity findSchool = _schoolService.Schools.FirstOrDefault(o => o.Name == school.Name
                                                                                 && o.SchoolNumber == model.SchoolName &&
                                                                                 o.StateId == model.StateId);
            if (basicSchool == null)
            {
                result.ResultType = OperationResultType.Error;
                result.Message = "School Name error.";
            }
            else
                if (basicSchool.Status == SchoolStatus.Inactive || findSchool != null)
                {
                    result.ResultType = OperationResultType.Error;
                    result.Message = "School Name has been used.";
                }
                else
                {
                    model.SchoolName = basicSchool.Name;
                    school.Name = basicSchool.Name;
                    result = _schoolService.InsertSchool(school);
                    model.ID = school.ID;
                    model.SchoolName = school.BasicSchool.Name;
                    if (result.ResultType == OperationResultType.Success)
                    {
                        basicSchool.Status = SchoolStatus.Inactive;
                        result = UpdateBasicSchool(basicSchool, school);
                        if (school.VSDesignation > 0)
                        {
                            // 添加时如果设置Verified Star, 插入一个空的TrsAssessment记录
                            TrsBusiness.InsertRecordOfVerifiedStarChanged(school.ID, school.VSDesignation,
                                school.StarDate, school.RecertificatedBy);
                        }
                    }
                }
            return result;
        }

        private OperationResult DeleteSchool(int schoolId)
        {
            SchoolEntity school = GetSchool(schoolId);
            if (school != null)
                return _schoolService.DeleteSchool(school);
            else
            {
                OperationResult res = new OperationResult(OperationResultType.Success);
                res.ResultType = OperationResultType.Error;
                res.Message = "School is null";
                return res;
            }
        }

        #endregion

        #region Update Methods
        /// <summary>
        /// Update Basic School and change BasicCommunity to the selected community 
        /// </summary>
        /// <returns></returns>
        private OperationResult UpdateBasicSchool(int communityId, BasicSchoolEntity basicSchool)
        {
            CommunityEntity community = _communityBus.GetCommunity(communityId);
            basicSchool.CommunityId = community.BasicCommunityId;
            OperationResult result = UpdateBasicSchool(basicSchool);
            return result;
        }
        public OperationResult UpdateBasicSchool(BasicSchoolEntity basicSchool)
        {
            OperationResult result = new OperationResult(OperationResultType.Success);
            result = _schoolService.UpdateBasicSchool(basicSchool);
            return result;
        }
        public OperationResult UpdateBasicSchool(BasicSchoolEntity basicSchool, SchoolEntity entity)
        {
            basicSchool.UpdatedOn = entity.UpdatedOn;
            basicSchool.Address1 = entity.PhysicalAddress1;
            basicSchool.City = entity.City;
            basicSchool.Zip = entity.Zip;
            basicSchool.Phone = entity.PhoneNumber;
            basicSchool.CountyId = entity.CountyId;
            basicSchool.StateId = entity.StateId == null ? 0 : entity.StateId.Value;
            basicSchool.SchoolNumber = entity.SchoolNumber;
            return UpdateBasicSchool(basicSchool);
        }
        public OperationResult UpdateSchool(SchoolModel model, Role role)
        {
            bool isFacilityTypeChanged = false;
            OperationResult result = new OperationResult(OperationResultType.Success);
            SchoolEntity entity = _schoolService.GetSchool(model.ID);

            if (model.FacilityType != entity.FacilityType)
            {
                isFacilityTypeChanged = true;
            }
            //var insertTrsAssessment = model.VSDesignation != entity.VSDesignation;
            model = InitSchoolByRole(model, role);
            entity = SchoolModelToEntity(model, entity);
            entity.UpdatedOn = DateTime.Now;
            //status is inactive
            if (model.Status == SchoolStatus.Inactive)
                (new CommunityBusiness()).InactiveEnity(ModelName.School, entity.ID, EntityStatus.Inactive, CommonAgent.SchoolYear);

            result = _schoolService.UpdateSchool(entity);

            //根据 facility type 更新 classes表状态
            if (result.ResultType == OperationResultType.Success && isFacilityTypeChanged)
            {
                result = _trsClassBusiness.DeleteResultBySchoolId(model.ID);
            }
            if (result.ResultType == OperationResultType.Success)
            {
                //Ticket 2294 Item 34.1中将此功能去掉
                //if (insertTrsAssessment)
                //{
                //    // 编辑时如果修改Verified Star, 插入一个空的TrsAssessment记录
                //    TrsBusiness.InsertRecordOfVerifiedStarChanged(entity.ID, entity.VSDesignation, entity.StarDate, entity.RecertificatedBy);
                //}

                BasicSchoolEntity basicSchool = GetBasicSchoolById(entity.BasicSchoolId);
                result = UpdateBasicSchool(basicSchool, entity);
            }
            return result;
        }
        /// <summary>
        /// 根据角色 限制某些字段的值的更新
        /// </summary>
        /// <param name="school"></param>
        /// <param name="role"></param>
        /// <returns></returns>
        public SchoolModel InitSchoolByRole(SchoolModel school, Role role)
        {
            SchoolEntity oldSchool = NewSchoolEntity();
            if (school.ID > 0)
            {
                oldSchool = _schoolService.GetSchool(school.ID);
            }
            switch (role)
            {
                case Role.Super_admin:

                    break;
                case Role.Coordinator:
                    #region Coodinator
                    if (school.ID < 1)
                    {
                        school.Status = SchoolStatus.Active;
                        school.SchoolTypeId = 1;
                        school.EscName = 0;
                        //  school.TrsProviderId = 2;
                    }
                    else
                    {
                        school.Status = oldSchool.Status;
                        school.EscName = oldSchool.EscName;
                        school.SchoolTypeId = oldSchool.SchoolTypeId;
                        school.FundingId = oldSchool.FundingId;
                        // school.TrsProviderId = oldSchool.TrsProviderId;
                        school.Notes = oldSchool.Notes;
                    }
                    #endregion
                    break;
                case Role.Mentor_coach://"mentor/coach":
                    #region mentor
                    if (school.ID < 1)
                    {

                        school.Status = SchoolStatus.Active;

                        school.ParentAgencyId = 1;
                        school.PhysicalAddress1 = "***";
                        school.PhysicalAddress2 = "";
                        school.City = "***";
                        school.StateId = 2;
                        school.CountyId = 1;
                        school.Zip = "00000";

                        school.MailingAddress1 = "***";
                        school.MailingAddress2 = "";
                        school.MailingCity = "***";
                        school.MailingCountyId = 1;
                        school.MailingStateId = 2;
                        school.MailingZip = "00000";

                        school.EscName = 0;
                        school.SchoolTypeId = 1;
                        school.FundingId = 0;
                        //  school.TrsProviderId = 1;
                        school.Notes = "";

                    }
                    else
                    {
                        school.BasicSchoolId = oldSchool.BasicSchoolId;
                        school.Status = oldSchool.Status;
                        school.SchoolYear = oldSchool.SchoolYear;
                        school.ParentAgencyId = oldSchool.ParentAgencyId;
                        school.PhysicalAddress1 = oldSchool.PhysicalAddress1;
                        school.PhysicalAddress2 = oldSchool.PhysicalAddress2;
                        school.City = oldSchool.City;
                        school.CountyId = oldSchool.CountyId;
                        school.StateId = oldSchool.StateId.Value;
                        school.Zip = oldSchool.Zip;

                        school.MailingAddress1 = oldSchool.MailingAddress1;
                        school.MailingAddress2 = oldSchool.MailingAddress2;
                        school.MailingCity = oldSchool.MailingCity;
                        school.MailingCountyId = oldSchool.MailingCountyId;
                        school.MailingStateId = oldSchool.MailingStateId;
                        school.MailingZip = oldSchool.MailingZip;

                        school.EscName = oldSchool.EscName;
                        school.SchoolTypeId = oldSchool.SchoolTypeId;
                        school.FundingId = oldSchool.FundingId;
                        //    school.TrsProviderId = oldSchool.TrsProviderId;
                        school.Notes = oldSchool.Notes;
                    }
                    #endregion
                    break;
                case Role.Community:
                case Role.District_Community_Specialist:
                case Role.Community_Specialist_Delegate:
                case Role.District_Community_Delegate:
                    #region
                    if (school.ID < 1)
                    {
                        if (school.Status != SchoolStatus.Pending)
                            school.Status = SchoolStatus.Active;
                    }
                    else
                    {
                        school.Status = oldSchool.Status;
                    }
                    #endregion
                    break;
                case Role.Principal:
                case Role.Principal_Delegate:
                    #region
                    if (school.ID < 1)
                    {
                        if (school.Status != SchoolStatus.Pending)
                            school.Status = SchoolStatus.Active;
                        school.FundingId = 0;
                        school.Notes = "";
                    }
                    else
                    {
                        school.Status = oldSchool.Status;
                        school.FundingId = oldSchool.FundingId;
                        school.Notes = oldSchool.Notes;
                    }
                    #endregion
                    break;
                case Role.TRS_Specialist:
                case Role.TRS_Specialist_Delegate:
                case Role.School_Specialist:
                case Role.School_Specialist_Delegate:
                    #region
                    if (school.ID < 1)
                    {
                        if (school.Status != SchoolStatus.Pending)
                            school.Status = SchoolStatus.Active;
                        school.FundingId = 0;
                        school.Notes = "";
                    }
                    else
                    {
                        school.Status = oldSchool.Status;
                        school.FundingId = oldSchool.FundingId;
                        school.Notes = oldSchool.Notes;
                    }
                    #endregion
                    break;
                case Role.Auditor:
                case Role.Statewide:
                case Role.Teacher:
                case Role.Parent:
                    if (school.ID < 1)
                    {
                        school.DfpsNumber = "";
                        school.TrsTaStatus = "";
                        if (school.Status != SchoolStatus.Pending)
                            school.Status = SchoolStatus.Active;
                        school.FundingId = 0;
                        school.Notes = "";
                        school.MailingCity = "";
                        school.MailingZip = "";
                        school.OwnerFirstName = "";
                        school.OwnerFirstName = "";
                        school.OwnerLastName = "";
                        school.OwnerEmail = "";
                        school.OwnerPhone = "";
                    }
                    else
                    {
                        school.OwnerFirstName = oldSchool.OwnerFirstName;
                        school.DfpsNumber = oldSchool.DfpsNumber;
                        school.TrsTaStatus = oldSchool.TrsTaStatus;
                        school.Status = oldSchool.Status;
                        school.FundingId = oldSchool.FundingId;
                        school.Notes = oldSchool.Notes;
                        school.MailingCity = oldSchool.MailingCity;
                        school.MailingZip = oldSchool.MailingZip;
                        school.OwnerFirstName = oldSchool.OwnerFirstName;
                        school.OwnerLastName = oldSchool.OwnerLastName;
                        school.OwnerEmail = oldSchool.OwnerEmail;
                        school.OwnerPhone = oldSchool.OwnerPhone;
                    }
                    break;
                default:

                    break;
            }
            return school;
        }
        #endregion

        #region Get Methods

        public List<int> SearchSchoolIdsByUserIds(UserBaseEntity user)
        {
            var query = _schoolService.Schools.AsExpandable().Where(GetRoleCondition(user))
                 .Select(c => c.ID);
            var list = query.ToList();
            return list;
        }

        public SchoolEntity SchoolExists(int basicSchoolId)
        {
            return _schoolService.Schools.FirstOrDefault(o => o.BasicSchoolId == basicSchoolId);
        }

        public SchoolModel GetSchoolEntity(int schoolId, UserBaseEntity user)
        {
            //List<SchoolEntity> schoolList = _schoolService.Schools.AsExpandable().Where(o => o.ID == schoolId).Where(GetRoleCondition(user)).ToList();

            SchoolModel school =
                _schoolService.Schools.AsExpandable().Where(o => o.ID == schoolId).Where(GetRoleCondition(user))
                .Select(SchoolEntityToModelForEdit).FirstOrDefault();
            if (school == null)
                return null;

            if (user != null)
            {
                if (user.Role == Role.Community || user.Role == Role.District_Community_Specialist ||
              user.Role == Role.Statewide)
                {
                    if (school.PrimaryCommunityId > 0 && user.UserCommunitySchools.Any(o => o.CommunityId == school.PrimaryCommunityId))
                    {
                        school.SchoolAccess = AccessType.Primary;
                    }
                    else if (user.UserCommunitySchools.Any(o => o.SchoolId == school.ID
                               && (o.AccessType == AccessType.FullAccess || o.AccessType == AccessType.Primary)))
                    {
                        school.SchoolAccess = AccessType.FullAccess;
                    }
                    else
                    {
                        school.SchoolAccess = AccessType.ReadOnly;
                    }
                }
                else
                {
                    school.SchoolAccess = AccessType.FullAccess;
                }
            }
            return school;
        }

        public SchoolModel GetActiveSchoolEntity(int schoolId)
        {
            SchoolModel school =
                _schoolService.Schools.AsExpandable().Where(o => o.ID == schoolId && o.Status == SchoolStatus.Active)
                .Select(SchoolEntityToModelForEdit).FirstOrDefault();
            return school;
        }
        public int[] GetAssignedSchoolIds(int comId)
        {
            return _schoolService.GetSchoolIds(comId);
        }

        public List<int> GetAssignedSchoolIdsWithoutDemo(int comId)
        {
            return
                _schoolService.CommunitySchoolRelations.Where(
                    o => o.CommunityId == comId
                        && o.School.Status == SchoolStatus.Active
                        && o.School.SchoolType.Name.StartsWith("Demo") == false)
                    .Select(e => e.SchoolId)
                    .ToList();
        }

        public IList<CommunitySchoolRelationModel> GetAssignedSchools(Expression<Func<CommunitySchoolRelationsEntity, bool>> condition,
           string sort, string order, int first, int count, out int total)
        {
            var query = _schoolService.CommunitySchoolRelations.AsExpandable().Where(condition).Select(o => new CommunitySchoolRelationModel()
            {
                ID = o.ID,
                CommunityId = o.CommunityId,
                SchoolId = o.SchoolId,
                CommunityName = o.Community.BasicCommunity.Name,
                SchoolName = o.School.BasicSchool.Name

            });
            total = query.Count();
            var list = query.OrderBy(sort, order).Skip(first).Take(count);
            return list.ToList();
        }

        public int GetAtRiskPercent(int schoolId)
        {
            string AtRiskPercent = _schoolService.Schools.Where(o => o.ID == schoolId).Select(o => o.AtRiskPercent).FirstOrDefault().ToString();
            if (!AtRiskPercent.IsNullOrEmpty())
            {
                return int.Parse(AtRiskPercent);
            }
            else
            {
                return 0;
            }
        }
        public string GetSchoolType(int schoolId)
        {
            return _schoolService.Schools.Where(o => o.ID == schoolId).Select(o => o.SchoolType.Name).FirstOrDefault();
        }
        public string GetSchoolTypeName(int typeId)
        {
            return _schoolService.SchoolTypes.Where(o => o.ID == typeId).Select(o => o.Name).FirstOrDefault();
        }

        public string GetSchoolFacilityType(int schoolId)
        {
            List<FacilityType> list = _schoolService.Schools.Where(o => o.ID == schoolId && (int)o.TrsAssessorId > 0)
                .Select(r => r.FacilityType).ToList();
            if (list.Count > 0) return list[0].ToString();
            else return string.Empty;
        }

        public List<SchoolListModel> SearchSchools(UserBaseEntity user, Expression<Func<SchoolEntity, bool>> condition,
            string sort, string order, int first, int count, out int total)
        {
            var parent = new UserBaseEntity();
            if (user.Role == Role.Community_Specialist_Delegate || user.Role == Role.District_Community_Delegate)
            {
                parent = _userBusiness.GetUser(user.CommunityUser.ParentId);
            }
            var query = _schoolService.Schools.AsExpandable().Where(condition).Where(GetRoleCondition(user))
                 .Select(SchoolEntityToListModel);
            total = query.Count();
            var queryList = query.OrderBy(sort, order).Skip(first).Take(count);
            var list = queryList.ToList();

            foreach (SchoolListModel model in list)
            {
                if (user.Role == Role.Community || user.Role == Role.District_Community_Specialist || user.Role == Role.Statewide)
                {
                    if (model.PrimaryCommunityId > 0 && user.UserCommunitySchools.Any(o => o.CommunityId == model.PrimaryCommunityId))
                    {
                        model.SchoolAccess = AccessType.Primary;
                    }
                    else if (user.UserCommunitySchools.Any(o => o.SchoolId == model.ID
                               && (o.AccessType == AccessType.FullAccess || o.AccessType == AccessType.Primary)))
                    {
                        model.SchoolAccess = AccessType.FullAccess;
                    }
                    else
                    {
                        model.SchoolAccess = AccessType.ReadOnly;
                    }
                }
                else if (user.Role == Role.Community_Specialist_Delegate || user.Role == Role.District_Community_Delegate)
                {
                    if (model.PrimaryCommunityId > 0 && parent.UserCommunitySchools.Any(o => o.CommunityId == model.PrimaryCommunityId))
                    {
                        model.SchoolAccess = AccessType.Primary;
                    }
                    else if (parent.UserCommunitySchools.Any(o => o.SchoolId == model.ID
                               && (o.AccessType == AccessType.FullAccess || o.AccessType == AccessType.Primary)))
                    {
                        model.SchoolAccess = AccessType.FullAccess;
                    }
                    else
                    {
                        model.SchoolAccess = AccessType.ReadOnly;
                    }
                }
                else
                {
                    model.SchoolAccess = AccessType.FullAccess;
                }
                if (user.Role == Role.Community || user.Role == Role.District_Community_Specialist)
                {
                    if (
                    user.UserCommunitySchools.Any(
                        e =>
                            e.Community.CommunityAssessmentRelations.Any(
                                s => ((LocalAssessment)s.AssessmentId == LocalAssessment.TexasRisingStar))))
                    {
                        model.IsTrsAccess = true;
                    }
                    else
                    {
                        model.IsTrsAccess = false;
                    }
                }
                else if (user.Role == Role.TRS_Specialist)
                {
                    if (
                    user.UserCommunitySchools.Any(
                        e =>
                            e.School.CommunitySchoolRelations.Any(
                                t =>
                                    t.Community.CommunityAssessmentRelations.Any(
                                        s => ((LocalAssessment)s.AssessmentId == LocalAssessment.TexasRisingStar)))))
                    {
                        model.IsTrsAccess = true;
                    }
                    else
                        model.IsTrsAccess = false;
                }
            }
            return list;
        }

        public List<AssignSchoolModel> SearchUnassigedSchools(UserBaseEntity user, Expression<Func<SchoolEntity, bool>> condition,
            string sort, string order, int first, int count, out int total)
        {
            var query = _schoolService.Schools.AsExpandable().Where(condition).Where(GetPrimaryRoleCondition(user))
                .Select(o => new AssignSchoolModel()
                {
                    ID = o.ID,
                    SchoolName = o.Name,
                    CommunityNames = o.CommunitySchoolRelations.Select(e => e.Community.Name)
                });
            total = query.Count();
            var list = query.OrderBy(sort, order).Skip(first).Take(count);
            return list.ToList();
        }

        public List<AssignSchoolModel> SearchTrsSpecialistUnassigedSchools(UserBaseEntity user, Expression<Func<SchoolEntity, bool>> condition,
            string sort, string order, int first, int count, out int total)
        {
            var query = _schoolService.Schools.AsExpandable().Where(condition).Where(GetTrsSpecialistCondition(user))
                .Select(o => new AssignSchoolModel()
                {
                    ID = o.ID,
                    SchoolName = o.Name,
                    CommunityNames = o.CommunitySchoolRelations.Select(e => e.Community.Name)
                });
            total = query.Count();
            var list = query.OrderBy(sort, order).Skip(first).Take(count);
            return list.ToList();
        }

        public IEnumerable<BasicSchoolModel> GetSchoolModels(UserBaseEntity user,
            Expression<Func<SchoolEntity, bool>> condition)
        {
            var query = _schoolService.Schools.AsExpandable().Where(GetRoleCondition(user)).Where(condition)
               .Select(x => new BasicSchoolModel()
               {
                   Communities = x.CommunitySchoolRelations.Select(c => c.Community.Name),
                   ID = x.ID,
                   Name = x.BasicSchool.Name,
                   SchoolType = x.SchoolType.Name
               });
            return query.ToList();
        }

        public IList<SchoolEntity> GetPrimarySchoolsByComId(int communityId)
        {
            IList<SchoolEntity> list = new List<SchoolEntity>();
            CommunityEntity community = _communityBus.GetCommunity(communityId);
            list = _schoolService.Schools.Where(o => o.BasicSchool.CommunityId == community.BasicCommunityId &&
                o.CommunitySchoolRelations.Any(re => re.CommunityId == communityId)).ToList();
            return list;
        }
        public IList<SchoolEntity> GetPrimarySchoolsByComId(List<int> communityIds)
        {
            IList<SchoolEntity> list = new List<SchoolEntity>();
            List<int> basicComIds = _communityBus.GetBasicComIdByIds(communityIds);
            list = _schoolService.Schools.Where(o => basicComIds.Contains(o.BasicSchool.CommunityId)
                && o.CommunitySchoolRelations.Any(re => communityIds.Contains(re.CommunityId))).ToList();
            return list;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="isActive">True,返回有效状态的值；False，返回全部的值</param>
        /// <returns></returns>
        public IEnumerable<SchoolSelectItemModel> GetSchoolsSelectList(UserBaseEntity user, Expression<Func<SchoolEntity, bool>> expression, bool isActive = false)
        {
            return _schoolService.Schools.AsExpandable()
                .Where(expression).Where(o => o.Status == SchoolStatus.Active || isActive == false)
                .Where(GetRoleCondition(user))
                .Select(o => new SchoolSelectItemModel()
                {
                    ID = o.ID,
                    Name = o.Name,
                    //TODO:School CommunityId DEL
                    //CommunityId = o.CommunityId,
                    CommunityIds = o.CommunitySchoolRelations.Select(c => c.CommunityId),
                    City = " " + o.City,
                    County = "",
                    CountyId = o.CountyId,
                    SchoolId = o.SchoolId,
                    Address = o.PhysicalAddress1,
                    Address2 = o.PhysicalAddress2,
                    StateId = o.StateId.Value,
                    State = (o.StateId == 0 ? "" : o.State.Name),
                    Zip = o.Zip,
                    PhoneNumber = o.PhoneNumber
                });
        }

        public IEnumerable<SchoolSelectItemModel> GetSchoolsSelectListForCache(UserBaseEntity user, int communityId, string schoolName, bool isActive = false)
        {
            var key = "AllSchoolSelectList" + communityId;
            var allSchools = CacheHelper.Get<List<SchoolSelectItemModel>>(key);
            if (allSchools == null)
            {
                lock (CacheHelper.Synchronize)
                {
                    allSchools = CacheHelper.Get<List<SchoolSelectItemModel>>(key);
                    if (allSchools == null)
                    {
                        allSchools = _schoolService.Schools.AsExpandable().Where(o => (o.Status == SchoolStatus.Active || isActive == false)
                                                    && (o.CommunitySchoolRelations.Any(c => c.CommunityId == communityId) || communityId <= 0))
                                                    .Select(o => new SchoolSelectItemModel()
                                                    {
                                                        ID = o.ID,
                                                        Name = o.Name,
                                                        CommunityIds = o.CommunitySchoolRelations.Where(x => x.Status == EntityStatus.Active).Select(c => c.CommunityId),
                                                        City = " " + o.City,
                                                        County = "",
                                                        CountyId = o.CountyId,
                                                        SchoolId = o.SchoolId,
                                                        Address = o.PhysicalAddress1,
                                                        Address2 = o.PhysicalAddress2,
                                                        StateId = o.StateId.Value,
                                                        State = (o.StateId == 0 ? "" : o.State.Name),
                                                        Zip = o.Zip,
                                                        PhoneNumber = o.PhoneNumber
                                                    }).OrderBy(c => c.Name).ToList();
                        CacheHelper.Add(key, allSchools, CacheHelper.DefaultExpiredSeconds);
                    }
                }
            }
            if (allSchools != null)
            {
                var list = allSchools.ToList();
                if (user.Role != Role.Super_admin)
                {
                    var userSchoolIds = CacheHelper.Get<List<int>>("UserSchoolIds" + user.ID);
                    if (userSchoolIds == null)
                    {
                        userSchoolIds = CacheHelper.Get<List<int>>(key);
                        if (userSchoolIds == null)
                        {
                            userSchoolIds = _schoolService.Schools.AsExpandable().Where(GetRoleCondition(user)).Select(o => o.ID).ToList();
                            CacheHelper.Add(key, userSchoolIds, CacheHelper.DefaultExpiredSeconds);
                        }
                    }
                    list = list.Where(c => userSchoolIds.Contains(c.ID)).ToList();
                }
                if (communityId > 0)
                    list = list.Where(c => c.CommunityIds.Contains(communityId)).ToList();
                if (schoolName != null && schoolName.Trim() != string.Empty)
                    list = list.Where(c => c.Name.Contains(schoolName)).ToList();
                return list;
            }
            else
            {
                return new List<SchoolSelectItemModel>();
            }

        }


        public IEnumerable<SchoolSelectItemModel> GetSchoolsSelectListForCpalls(UserBaseEntity user, int communityId, int schoolId,
            string sort, string order, int first, int count, out int total)
        {
            total = 0;
            var key = "AllSchoolSelectList" + communityId;
            var allSchools = CacheHelper.Get<List<SchoolSelectItemModel>>(key);
            if (allSchools == null)
            {
                lock (CacheHelper.Synchronize)
                {
                    allSchools = CacheHelper.Get<List<SchoolSelectItemModel>>(key);
                    if (allSchools == null)
                    {
                        allSchools = _schoolService.Schools.AsExpandable().Where(o => (o.Status == SchoolStatus.Active)
                             && (o.CommunitySchoolRelations.Any(c => c.CommunityId == communityId) || communityId <= 0))
                                                    .Select(o => new SchoolSelectItemModel()
                                                    {
                                                        ID = o.ID,
                                                        Name = o.Name,
                                                        CommunityIds = o.CommunitySchoolRelations.Where(x => x.Status == EntityStatus.Active).Select(c => c.CommunityId),
                                                    }).OrderBy(c => c.Name).ToList();
                        CacheHelper.Add(key, allSchools, CacheHelper.DefaultExpiredSeconds);
                    }
                }
            }
            if (allSchools != null)
            {
                var list = allSchools.ToList();
                if (user.Role != Role.Super_admin)
                {
                    var userSchoolIds = CacheHelper.Get<List<int>>("UserSchoolIds" + user.ID);
                    if (userSchoolIds == null)
                    {
                        userSchoolIds = CacheHelper.Get<List<int>>("UserSchoolIds" + user.ID);
                        if (userSchoolIds == null)
                        {
                            userSchoolIds = _schoolService.Schools.AsExpandable().Where(GetRoleCondition(user)).Select(o => o.ID).ToList();
                            CacheHelper.Add("UserSchoolIds" + user.ID, userSchoolIds, CacheHelper.DefaultExpiredSeconds);
                        }
                    }
                    list = list.Where(c => userSchoolIds.Contains(c.ID)).ToList();
                }
                if (communityId > 0)
                    list = list.Where(c => c.CommunityIds.Contains(communityId)).ToList();
                if (schoolId > 0)
                    list = list.Where(c => c.ID == schoolId).ToList();

                total = list.Count;
                return list.OrderBy(sort, order).Skip(first).Take(count);
            }
            else
            {
                return new List<SchoolSelectItemModel>();
            }

        }


        public void SetUserSchoolIdsCache(UserBaseEntity user)
        {
            if (user.Role != Role.Super_admin)
            {
                var userSchoolIds = CacheHelper.Get<List<int>>("UserSchoolIds" + user.ID);
                if (userSchoolIds == null)
                {
                    userSchoolIds = _schoolService.Schools.AsExpandable().Where(GetRoleCondition(user)).Select(o => o.ID).ToList();
                    CacheHelper.Add("UserSchoolIds" + user.ID, userSchoolIds, CacheHelper.DefaultExpiredSeconds);
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="isActive">True,返回有效状态的值；False，返回全部的值</param>
        /// <returns></returns>
        public IEnumerable<SchoolSelectItemModel> GetPrimarySchoolsSelectList(UserBaseEntity user, Expression<Func<SchoolEntity, bool>> expression, bool isActive = false)
        {
            return _schoolService.Schools.AsExpandable()
                .Where(expression).Where(o => o.Status == SchoolStatus.Active || isActive == false)
                .Where(GetPrimaryRoleCondition(user))
                .Select(o => new SchoolSelectItemModel()
                {
                    ID = o.ID,
                    Name = o.Name,
                    City = " " + o.City,
                    County = "",
                    CountyId = o.CountyId,
                    SchoolId = o.SchoolId,
                    Address = o.PhysicalAddress1,
                    Address2 = o.PhysicalAddress2,
                    StateId = o.StateId.Value,
                    State = (o.StateId == 0 ? "" : o.State.Name),
                    Zip = o.Zip,
                    PhoneNumber = o.PhoneNumber
                });
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="isActive">True,返回有效状态的值；False，返回全部的值</param>
        /// <returns></returns>
        public IEnumerable<SchoolSelectItemModel> GetAllSchoolsSelectList(Expression<Func<SchoolEntity, bool>> expression, bool isActive = false)
        {
            return _schoolService.Schools.AsExpandable()
                .Where(expression).Where(o => o.Status == SchoolStatus.Active || isActive == false)
                .Select(o => new SchoolSelectItemModel()
                {
                    ID = o.ID,
                    Name = o.Name,
                    City = " " + o.City,
                    County = "",
                    CountyId = o.CountyId,
                    SchoolId = o.SchoolId,
                    Address = o.PhysicalAddress1,
                    Address2 = o.PhysicalAddress2,
                    StateId = o.StateId.Value,
                    State = (o.StateId == 0 ? "" : o.State.Name),
                    Zip = o.Zip,
                    PhoneNumber = o.PhoneNumber
                });
        }
        /// <summary>
        /// 获取正常的学校的名字ID
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public IEnumerable<SelectItemModel> GetSchoolsSelectList(UserBaseEntity user, Expression<Func<SchoolEntity, bool>> expression)
        {
            bool isActive = true;
            return _schoolService.Schools.AsExpandable()
                .Where(expression).Where(o => o.Status == SchoolStatus.Active || isActive == false)
                .Where(GetRoleCondition(user))
                .Select(o => new SelectItemModel()
                {
                    ID = o.ID,
                    Name = o.BasicSchool.Name
                });
        }

        public IEnumerable<SchoolSelectItemModel> GetBasicSchoolSelectList(string keyword, int basicCommId = 0)
        {
            IList<SchoolSelectItemEntity> entityList = _schoolService.GetSchoolNameList(basicCommId, keyword);
            return entityList.OrderBy(o => o.Name).Select(e => new SchoolSelectItemModel
            {
                ID = e.ID,
                Name = e.Name,
                City = " " + e.City,
                State = e.State,
                County = e.County,
                CountyId = e.CountyId,
                Address = e.Address,
                StateId = e.StateId,
                Zip = e.Zip,
                PhoneNumber = e.Phone,
                SchoolNumber = e.SchoolNumber

            }).ToList<SchoolSelectItemModel>();
        }

        /// <summary>
        /// 可以根据 name zip  city 搜索 school
        /// </summary>
        /// <param name="keyword"></param>
        /// <param name="basicCommId"></param>
        /// <returns></returns>
        public IEnumerable<SchoolSelectItemModel> GetBasicSchoolSelectListByKey(string keyword, int communityId = 0)
        {
            IList<SchoolSelectItemEntity> entityList = _schoolService.GetSchoolListByKey(communityId, keyword);
            return entityList.Select(e => new SchoolSelectItemModel
            {
                ID = e.ID,
                Name = e.Name,
                City = " " + e.City,
                County = e.County,
                State = e.State,
                CountyId = e.CountyId,
                Address = e.Address,
                StateId = e.StateId,
                Zip = e.Zip,
                PhoneNumber = e.Phone,
                SchoolNumber = e.SchoolNumber

            }).ToList<SchoolSelectItemModel>();
        }

        public List<SelectItemModel> GetBasicSchoolList(Expression<Func<SchoolEntity, bool>> condition)
        {
            return _schoolService.Schools.AsExpandable().Where(condition).Select(r => new SelectItemModel()
            {
                ID = r.ID,
                Name = r.BasicSchool.Name
            }).ToList();
        }

        /// <summary>
        /// 如果 basic school 已经被 注册为新的school 则表示 为 true
        /// </summary>
        /// <param name="basicSchoolId"></param>
        /// <returns></returns>
        public SchoolModel IsVerified(int basicSchoolId = 0)
        {
            SchoolModel school = _schoolService.Schools.Where(o => o.BasicSchoolId == basicSchoolId
                && o.Status == SchoolStatus.Active).
                Select(SchoolEntityToModelForEdit).FirstOrDefault();
            return school;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="basicSchoolId"></param>
        /// <returns> SelectItemModel</returns>
        public SchoolSelectItemModel GetBasicSchoolByBasicId(int basicSchoolId = 0)
        {
            SchoolSelectItemModel school = _schoolService.BasicSchools.Where(o => o.ID == basicSchoolId).
                Select(e => new SchoolSelectItemModel
                {
                    ID = e.ID,
                    Name = e.Name,
                    Address = e.Address1,
                    StateId = e.StateId,
                    City = e.City,
                    Zip = e.Zip
                }).FirstOrDefault();
            return school;
        }

        public BasicSchoolEntity GetBasicSchoolById(int basicId)
        {
            return _schoolService.BasicSchools.FirstOrDefault(o => o.ID == basicId);
        }

        #region Other entities

        public IEnumerable<SelectItemModel> GetBasicSchoolsList(bool isActive = true)
        {
            return _schoolService.BasicSchools.Where(o => o.Status == SchoolStatus.Active || isActive == false)
                .Select(e => new SelectItemModel()
                {
                    ID = e.ID,
                    Name = e.Name
                });
        }

        public IEnumerable<SelectItemModel> GetIspList(bool isActive = true)
        {
            IQueryable<IspEntity> query = isActive
                ? _schoolService.Isps.Where(o => o.Status == EntityStatus.Active)
                : _schoolService.Isps;

            return query.Select(e => new SelectItemModel()
            {
                ID = e.ID,
                Name = e.Name
            });
        }

        public IEnumerable<SelectItemModelOther> GetIspListOther()
        {
            return _schoolService.Isps.Select(e => new SelectItemModelOther()
            {
                ID = e.ID,
                Name = e.Name,
                Status = e.Status
            });
        }

        public string GetIspName(int id)
        {
            return _schoolService.Isps.Where(o => o.ID == id).Select(o => o.Name).FirstOrDefault();
        }

        public IEnumerable<SelectItemModel> GetTrsProviderList(bool isActive = true)
        {
            IQueryable<TrsProviderEntity> query = isActive
                ? _schoolService.TrsProviders.Where(o => o.Status == EntityStatus.Active)
                : _schoolService.TrsProviders;

            return query.Select(e => new SelectItemModel()
            {
                ID = e.ID,
                Name = e.Name
            });
        }

        public IEnumerable<SelectItemModelOther> GetTrsProviderListOther()
        {
            return _schoolService.TrsProviders.Select(e => new SelectItemModelOther()
            {
                ID = e.ID,
                Name = e.Name,
                Status = e.Status
            });
        }

        public string GetTrsName(int id)
        {
            return _schoolService.TrsProviders.Where(o => o.ID == id).Select(o => o.Name).FirstOrDefault();
        }

        public List<SchoolTypeEntity> GetSchoolTypeList(int parentId)
        {
            return _schoolService.SchoolTypes.Where(o => o.ParentId == parentId && o.ID > 0 && o.Status == EntityStatus.Active).OrderBy(r => r.Name).ToList();
        }

        public List<SchoolTypeEntity> GetSchoolTypeList()
        {
            // ID = 0, 特殊记录
            return _schoolService.SchoolTypes.Where(o => o.ID > 0 && o.Status == EntityStatus.Active).OrderBy(r => r.Name).ToList();
        }

        public IEnumerable<SelectItemModel> GetSchoolTypeSelectList(int parentId = -1, bool isActive = true)
        {
            return _schoolService.SchoolTypes.Where(o => (o.ParentId == parentId || parentId == -1) && o.ID > 0)
                .Where(o => o.Status == EntityStatus.Active || isActive == false).Select(e => new SelectItemModel()
                {
                    ID = e.ID,
                    Name = e.Name
                });
        }

        public IEnumerable<SelectItemModelOther> GetSchoolTypeSelectListOther(int parentId = -1)
        {
            return _schoolService.SchoolTypes.Where(o => (parentId == -1 || o.ParentId == parentId) && o.ID > 0).ToList()
                .Select(e => new SelectItemModelOther()
                {
                    ID = e.ID,
                    Name = e.Name,
                    Status = e.Status,
                    Other = e.ParentId == 0 ? "None" : GetSchoolType_(e.ParentId).Name,
                    OtherId = e.ParentId
                });
        }


        public IEnumerable<SelectItemModel> GetParentAgencyList(bool isActive = true)
        {
            IQueryable<ParentAgencyEntity> query = isActive
                ? _schoolService.ParentAgencies.Where(o => o.Status == EntityStatus.Active)
                : _schoolService.ParentAgencies;

            return query.Select(e => new SelectItemModel()
            {
                ID = e.ID,
                Name = e.Name
            });
        }

        public IEnumerable<SelectItemModelOther> GetParentAgencyListOther()
        {
            return _schoolService.ParentAgencies.Select(e => new SelectItemModelOther()
            {
                ID = e.ID,
                Name = e.Name,
                Status = e.Status
            });
        }


        public List<SchoolCommunityModel> GetSchoolCommunity(List<int> schoolIds)
        {
            List<SchoolCommunityModel> list_school = new List<SchoolCommunityModel>();
            list_school = _schoolService.Schools.Where(a => schoolIds.Contains(a.ID))
                      .Select(o => new SchoolCommunityModel
                      {
                          SchoolId = o.ID,
                          SchoolName = o.BasicSchool.Name,
                          //TODO:School CommunityId DEL
                          //TODO:School CommunityId DEL
                          //CommunityID = o.CommunityId,
                          //CommunityName = o.Community.BasicCommunity.Name
                      }).ToList();
            return list_school;
        }

        public SchoolRoleEntity GetSchoolRoleEntity(Role role)
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
            return _schoolService.GetSchoolRole(newRole);
        }

        #endregion

        #endregion

        #region Delete Methods

        private OperationResult DeleteBasicSchool(BasicSchoolEntity basicSchool)
        {
            return _schoolService.DeleteBasicSchool(basicSchool);
        }

        #endregion

        /// <summary>
        /// Email Body for new basic school apply
        /// </summary>
        /// <param name="user"></param>
        /// <param name="basicSchoolId"></param>
        /// <returns></returns>
        private string GenerateEmailContent(UserBaseEntity user, int selectedCommunityId, int basicSchoolId, out string approveUrl)
        {
            IEncrypt encrypt = ObjectFactory.GetInstance<IEncrypt>();
            approveUrl = SFConfig.MainSiteDomain + "/school/School/verifyBasicschool?msg="
              + encrypt.Encrypt(selectedCommunityId + "&" + basicSchoolId.ToString() + "&" + user.ID + "&"
              + DateTime.Now.AddDays(SFConfig.ExpirationTime).ToString("yyyy-MM-dd"));

            string emailTemplateName = "AddSchool_To_InternalUser.xml";
            EmailTemplete template = XmlHelper.GetEmailTemplete(emailTemplateName);
            string emailBody = "";
            emailBody = template.Body.Replace("{CreatorName}", user.FirstName + " " + user.LastName)
                .Replace("{StaticDomain}", SFConfig.StaticDomain).Replace("{approveUrl}", approveUrl);
            return emailBody;

        }

        private string GenerateEmailContent2(UserBaseEntity user, int schoolId, int selectedCommunityId, out string approveUrl)
        {

            IEncrypt encrypt = ObjectFactory.GetInstance<IEncrypt>();
            approveUrl = SFConfig.MainSiteDomain + "/school/School/VerifySelectedSchool?msg="
              + encrypt.Encrypt(
                      schoolId.ToString()
                    + "&" + selectedCommunityId
                    + "&" + user.ID
                    + "&" + DateTime.Now.AddDays(SFConfig.ExpirationTime).ToString("yyyy-MM-dd")
              );

            string emailTemplateName = "AddSchoolRelation_To_InternalUser.xml";
            EmailTemplete template = XmlHelper.GetEmailTemplete(emailTemplateName);
            string emailBody = "";
            emailBody = template.Body.Replace("{CreatorName}", user.FirstName + " " + user.LastName)
                .Replace("{StaticDomain}", SFConfig.StaticDomain).Replace("{approveUrl}", approveUrl);
            return emailBody;

        }
        /// <summary>
        /// Email to BasicSchool Creator
        /// </summary>
        /// <param name="user"></param>
        /// <param name="basicSchoolId"></param>
        /// <returns></returns>
        private string EmailNoticeContent(UserBaseEntity user, string basicSchoolName, string status)
        {
            string emailTemplateName = "AddSchool_Notification_Creator.xml";
            EmailTemplete template = XmlHelper.GetEmailTemplete(emailTemplateName);
            string emailBody = "";
            emailBody = template.Body.Replace("{CreatorName}", user.FirstName + " " + user.LastName)
                .Replace("{StaticDomain}", SFConfig.StaticDomain).Replace("{BasicSchoolName}", basicSchoolName)
                .Replace("{status}", status);
            return emailBody;

        }

        public delegate void SendHandler();
        private OperationResult ReSendEmailToAdmin(int communityId, int schoolId, int currentId, string url, string subject, string body, bool isAsync)
        {
            OperationResult res = new OperationResult(OperationResultType.Success);
            List<UserModel> userList = _userBusiness.GetUsersByRole(Role.Super_admin);
            string emailTo = "";
            foreach (UserModel userModel in userList)
            {
                if (userModel.Email.Trim() != "")
                {
                    emailTo += userModel.Email.Trim();
                    emailTo += ";";
                }
            }
            if (userList.Count > 0)
            {
                res = SendEmail(emailTo, subject, body, isAsync);
                if (res.ResultType == OperationResultType.Success)
                {
                    res = UpdateStatusTrack(communityId, schoolId, currentId, userList.FirstOrDefault().UserId, url);
                }
            }
            else
            {
                res.ResultType = OperationResultType.Error;
                res.Message = "There is no super admin account.";
            }
            return res;
        }

        private OperationResult SendEmailToAdmin(int communityId, int schoolId, int currentId, string url, string subject, string body, bool isAsync)
        {
            OperationResult res = new OperationResult(OperationResultType.Success);
            List<UserModel> adminUserList = _userBusiness.GetUsersByRole(Role.Super_admin);
            string emailTo = "";
            foreach (UserModel userModel in adminUserList)
            {
                if (userModel.Email.Trim() != "")
                {
                    emailTo += userModel.Email.Trim();
                    emailTo += ";";
                }
            }
            if (adminUserList.Count > 0)
            {
                res = SendEmail(emailTo, subject, body, isAsync);
                if (res.ResultType == OperationResultType.Success)
                {
                    var firstOrDefault = adminUserList.FirstOrDefault();
                    if (firstOrDefault != null)
                        res = InsertStatusTrack(communityId, schoolId, currentId, firstOrDefault.UserId, url);
                }
            }
            else
            {
                res.ResultType = OperationResultType.Error;
                res.Message = "There is no super admin account.";
            }
            return res;
        }

        private OperationResult SendEmail(string to, string subject, string body, bool isAsync)
        {
            OperationResult result = new OperationResult(OperationResultType.Success);

            var emailSender = ObjectFactory.GetInstance<IEmailSender>();
            emailSender.Logger = ObjectFactory.GetInstance<ISunnetLog>();
            if (isAsync)
            {
                new SendHandler(() => emailSender.SendMail(to, subject, body))
                    .BeginInvoke(null, null);
            }
            else
            {
                if (emailSender.SendMail(to, subject, body))
                {
                    result.ResultType = OperationResultType.Success;
                }
                else
                {
                    result.ResultType = OperationResultType.Error;
                    result.Message = "Send email error, Receiver:" + to;
                }
            }
            return result;
        }

        /// <summary>
        /// True 表示是 demo
        /// </summary>
        /// <param name="schoolId"></param>
        /// <returns></returns>
        public bool CheckSchoolTypeDemo(int schoolId)
        {
            return _schoolService.Schools.Count(r => r.ID == schoolId && r.SchoolType.Name.Contains("demo")) > 0;
        }

        #region Community School Relations

        #endregion

        #region Model Entity Exchange
        private static Expression<Func<SchoolEntity, SchoolModel>> SchoolEntityToModelForEdit
        {
            get
            {
                return x => new SchoolModel()
                {
                    ID = x.ID,
                    SchoolName = x.Name,
                    SchoolType = x.SchoolType.Name,
                    SchoolId = x.SchoolId,
                    CommunityNames = x.CommunitySchoolRelations.Select(e => e.Community.Name),
                    CommunityName = x.CommunitySchoolRelations.FirstOrDefault() == null ? null : x.CommunitySchoolRelations.FirstOrDefault().Community.Name,//x.BasicCommunity.Name,
                    CommunityId = x.CommunitySchoolRelations.FirstOrDefault() == null ? 0 : x.CommunitySchoolRelations.FirstOrDefault().CommunityId,//x.CommunityId,
                    BasicSchoolId = x.BasicSchoolId,
                    Status = x.Status,
                    StatusDate = x.StatusDate,
                    SchoolYear = x.SchoolYear,
                    EscName = x.EscName,
                    ParentAgencyId = x.ParentAgencyId,
                    PhysicalAddress1 = x.PhysicalAddress1,
                    PhysicalAddress2 = x.PhysicalAddress2,
                    City = x.City,
                    CountyId = x.CountyId,
                    StateId = x.StateId.Value,
                    Zip = x.Zip,
                    PhoneNumber = x.PhoneNumber,
                    PhoneType = x.PhoneType,
                    SchoolTypeId = x.SchoolTypeId,
                    SubTypeId = x.SubTypeId,
                    ClassroomCount3Years = x.ClassroomCount3Years,
                    ClassroomCount4Years = x.ClassroomCount4Years,
                    ClassroomCount34Years = x.ClassroomCount34Years,
                    ClassroomCountKinder = x.ClassroomCountKinder,
                    ClassroomCountgrade = x.ClassroomCountgrade,
                    ClassroomCountOther = x.ClassroomCountOther,
                    ClassroomCountEarly = x.ClassroomCountEarly,
                    ClassroomCountToddler = x.ClassroomCountToddler,
                    ClassroomCountInfant = x.ClassroomCountInfant,
                    ClassroomCountPreSchool = x.ClassroomCountPreSchool,
                    AtRiskPercent = x.AtRiskPercent,
                    FundingId = x.FundingId,
                    //  TrsProviderId = x.TrsProviderId,
                    TrsLastStatusChange = x.TrsLastStatusChange,
                    //   TrsReviewDate = x.TrsReviewDate,
                    PrimarySalutation = x.PrimarySalutation,
                    PrimaryName = x.PrimaryName,
                    PrimaryTitleId = x.PrimaryTitleId,
                    PrimaryPhone = x.PrimaryPhone,
                    PrimaryPhoneType = x.PrimaryPhoneType,
                    PrimaryEmail = x.PrimaryEmail.Trim(),
                    SecondarySalutation = x.SecondarySalutation,
                    SecondaryName = x.SecondaryName,
                    SecondaryTitleId = x.SecondaryTitleId,
                    SecondaryPhoneNumber = x.SecondaryPhoneNumber,
                    SecondaryPhoneType = x.SecondaryPhoneType,
                    SecondaryEmail = x.SecondaryEmail.Trim(),
                    Latitude = x.Latitude,
                    Longitude = x.Longitude,
                    IsSamePhysicalAddress = x.IsSamePhysicalAddress,
                    MailingAddress1 = x.MailingAddress1.Trim(),
                    MailingAddress2 = x.MailingAddress2.Trim(),
                    MailingCity = x.MailingCity.Trim(),
                    MailingCountyId = x.MailingCountyId,
                    MailingStateId = x.MailingStateId,
                    MailingZip = x.MailingZip.Trim(),
                    SchoolSize = x.SchoolSize,
                    IspId = x.IspId,
                    IspOther = x.IspOther,
                    InternetSpeed = x.InternetSpeed,
                    InternetType = x.InternetType,
                    WirelessType = x.WirelessType,
                    Notes = x.Notes,
                    CreatedOn = x.CreatedOn,
                    UpdatedOn = x.UpdatedOn,
                    SchoolNumber = x.SchoolNumber,

                    PrimaryCommunityId = x.CommunitySchoolRelations.FirstOrDefault(o => o.Community.BasicCommunityId == x.BasicSchool.CommunityId) == null ? 0 :
                                      x.CommunitySchoolRelations.FirstOrDefault(o => o.Community.BasicCommunityId == x.BasicSchool.CommunityId).CommunityId,

                    CreateBy = x.CreateBy,
                    UpdateBy = x.UpdateBy,
                    CreateFrom = x.CreateFrom,
                    UpdateFrom = x.UpdateFrom,
                    #region TRS
                    TrsAssessorId = x.TrsAssessorId.HasValue ? x.TrsAssessorId.Value : 0,
                    TrsTaStatus = x.TrsTaStatus,
                    StarStatus = x.StarStatus,
                    DfpsNumber = x.DfpsNumber,
                    OwnerFirstName = x.OwnerFirstName,
                    OwnerLastName = x.OwnerLastName,
                    OwnerEmail = x.OwnerEmail,
                    OwnerPhone = x.OwnerPhone,
                    FacilityType = x.FacilityType,
                    RegulatingEntity = x.RegulatingEntity,
                    // EnableAutoAssign4Star = x.EnableAutoAssign4Star, 不需要保存/读取值, 每次都是未选中状态
                    EnableAutoAssign4Star = false,
                    NAEYC = x.NAEYC,
                    CANASA = x.CANASA,
                    NECPA = x.NECPA,
                    NACECCE = x.NACECCE,
                    NAFCC = x.NAFCC,
                    ACSI = x.ACSI,

                    USMilitary = x.USMilitary,
                    QELS = x.QELS,
                    VSDesignation = x.VSDesignation,
                    RecertificatedBy = x.RecertificatedBy,
                    StarDate = x.StarDate,
                    StarAssessmentType = x.StarAssessmentType
                    #endregion

                };
            }
        }

        private static Expression<Func<SchoolEntity, SchoolModel>> SchoolEntityToModel
        {
            get
            {
                return x => new SchoolModel()
                {
                    ID = x.ID,
                    SchoolName = x.BasicSchool.Name,
                    SchoolId = x.SchoolId,
                    SchoolType = x.SchoolType.Name,
                    //SchoolType = x.SchoolTypeId == 0 ? "" : x.SchoolType.Name,
                    //TODO:School CommunityId DEL
                    CommunityNames = x.CommunitySchoolRelations.Select(e => e.Community.Name),
                    //CommunityId = x.CommunityId,
                    Status = x.Status,

                };
            }
        }

        private static Expression<Func<SchoolEntity, SchoolListModel>> SchoolEntityToListModel
        {
            get
            {
                int trsId = (int)LocalAssessment.TexasRisingStar;
                return x => new SchoolListModel()
                {
                    ID = x.ID,
                    SchoolName = x.BasicSchool.Name,
                    SchoolId = x.SchoolId,
                    SchoolNumber = x.SchoolNumber,
                    SchoolType = x.SchoolType.Name,
                    CommunityNames = x.CommunitySchoolRelations.Select(e => e.Community.Name),
                    Status = x.Status,
                    IsTrsAccess = x.CommunitySchoolRelations.Any(o => o.Community.CommunityAssessmentRelations.Any(c => c.AssessmentId == trsId)),
                    PrimaryCommunityId = x.CommunitySchoolRelations.FirstOrDefault(o => o.Community.BasicCommunityId == x.BasicSchool.CommunityId) == null ? 0 :
                                        x.CommunitySchoolRelations.FirstOrDefault(o => o.Community.BasicCommunityId == x.BasicSchool.CommunityId).CommunityId
                };
            }
        }

        private SchoolEntity SchoolModelToEntity(SchoolModel m, SchoolEntity e)
        {
            e.ID = m.ID;
            e.SchoolId = m.SchoolId;
            e.BasicSchoolId = m.BasicSchoolId;
            //   e.CommunityId = m.CommunityId;
            e.Name = m.SchoolName;
            e.Status = m.Status;
            e.StatusDate = m.StatusDate;
            e.SchoolYear = m.SchoolYear;
            e.EscName = m.EscName;
            e.ParentAgencyId = m.ParentAgencyId;
            e.PhysicalAddress1 = m.PhysicalAddress1;
            e.PhysicalAddress2 = m.PhysicalAddress2;
            e.City = m.City;
            e.CountyId = m.CountyId;
            e.StateId = m.StateId;
            e.Zip = m.Zip;
            e.PhoneNumber = m.PhoneNumber;
            e.PhoneType = m.PhoneType;
            e.SchoolTypeId = m.SchoolTypeId;
            e.SubTypeId = m.SubTypeId;
            e.ClassroomCount3Years = m.ClassroomCount3Years;
            e.ClassroomCount4Years = m.ClassroomCount4Years;
            e.ClassroomCount34Years = m.ClassroomCount34Years;
            e.ClassroomCountKinder = m.ClassroomCountKinder;
            e.ClassroomCountgrade = m.ClassroomCountgrade;
            e.ClassroomCountOther = m.ClassroomCountOther;
            e.ClassroomCountEarly = m.ClassroomCountEarly;
            e.ClassroomCountInfant = m.ClassroomCountInfant;
            e.ClassroomCountToddler = m.ClassroomCountToddler;
            e.ClassroomCountPreSchool = m.ClassroomCountPreSchool;
            e.ClassroomCountPreSchool = m.ClassroomCountPreSchool;
            e.AtRiskPercent = m.AtRiskPercent;
            e.FundingId = m.FundingId;
            //  e.TrsProviderId = m.TrsProviderId;
            e.TrsLastStatusChange = m.TrsLastStatusChange;
            // e.TrsReviewDate = m.TrsReviewDate;
            e.PrimarySalutation = m.PrimarySalutation;
            e.PrimaryName = m.PrimaryName;
            e.PrimaryTitleId = m.PrimaryTitleId;
            e.PrimaryPhone = m.PrimaryPhone;
            e.PrimaryPhoneType = m.PrimaryPhoneType;
            e.PrimaryEmail = m.PrimaryEmail;
            e.SecondarySalutation = m.SecondarySalutation;
            e.SecondaryName = m.SecondaryName;
            e.SecondaryTitleId = m.SecondaryTitleId;
            e.SecondaryPhoneNumber = m.SecondaryPhoneNumber;
            e.SecondaryPhoneType = m.SecondaryPhoneType;
            e.SecondaryEmail = m.SecondaryEmail;
            e.Latitude = m.Latitude;
            e.Longitude = m.Longitude;
            e.IsSamePhysicalAddress = m.IsSamePhysicalAddress;
            e.MailingAddress1 = m.MailingAddress1;
            e.MailingAddress2 = m.MailingAddress2;
            e.MailingCity = m.MailingCity;
            e.MailingCountyId = m.MailingCountyId;
            e.MailingStateId = m.MailingStateId;
            e.MailingZip = m.MailingZip;
            e.SchoolSize = m.SchoolSize;
            e.IspId = m.IspId;
            e.IspOther = m.IspOther;
            e.InternetSpeed = m.InternetSpeed;
            e.InternetType = m.InternetType;
            e.WirelessType = m.WirelessType;
            e.Notes = m.Notes;
            e.CreatedOn = m.CreatedOn;
            e.UpdatedOn = m.UpdatedOn;
            e.SchoolNumber = m.SchoolNumber;
            e.CreateBy = m.CreateBy;
            e.UpdateBy = m.UpdateBy;
            e.CreateFrom = m.CreateFrom;
            e.UpdateFrom = m.UpdateFrom;

            #region TRS
            e.TrsAssessorId = m.TrsAssessorId;
            e.TrsTaStatus = m.TrsTaStatus;
            e.StarStatus = m.StarStatus;
            e.DfpsNumber = m.DfpsNumber;
            e.OwnerFirstName = m.OwnerFirstName;
            e.OwnerLastName = m.OwnerLastName;
            e.OwnerEmail = m.OwnerEmail;
            e.OwnerPhone = m.OwnerPhone;
            e.FacilityType = m.FacilityType;
            e.RegulatingEntity = m.RegulatingEntity;
            e.NAEYC = m.NAEYC;
            e.CANASA = m.CANASA;
            e.NECPA = m.NECPA;
            e.NACECCE = m.NACECCE;
            e.NAFCC = m.NAFCC;
            e.ACSI = m.ACSI;
            e.USMilitary = m.USMilitary;
            e.QELS = m.QELS;
            e.VSDesignation = m.VSDesignation;
            e.RecertificatedBy = m.RecertificatedBy.EnsureMinDate();
            e.EnableAutoAssign4Star = m.EnableAutoAssign4Star;
            e.StarDate = m.StarDate;
            e.StarAssessmentType = m.StarAssessmentType;

            //Ticket 2325 School的StarDate字段只在TRS操作中改变
            //e.StarDate = DateTime.Now;
            if (e.StarDate < CommonAgent.TrsMinDate)
            {
                e.StarDate = CommonAgent.TrsMinDate;
            }
            e.RecertificatedBy = CommonAgent.GetTrsReceritificationDate();

            #endregion
            return e;
        }

        public SchoolModel EntityToModel(SchoolEntity m, SchoolModel e)
        {
            e.ID = m.ID;
            e.SchoolId = m.SchoolId;
            e.BasicSchoolId = m.BasicSchoolId;
            // e.CommunityId = m.CommunityId;
            e.Status = m.Status;
            e.StatusDate = m.StatusDate;
            e.SchoolYear = m.SchoolYear;
            e.EscName = m.EscName;
            e.ParentAgencyId = m.ParentAgencyId;
            e.PhysicalAddress1 = m.PhysicalAddress1;
            e.PhysicalAddress2 = m.PhysicalAddress2;
            e.City = m.City;
            e.CountyId = m.CountyId;
            e.StateId = m.StateId.Value;
            e.Zip = m.Zip;
            e.PhoneNumber = m.PhoneNumber;
            e.PhoneType = m.PhoneType;
            e.SchoolTypeId = m.SchoolTypeId;
            e.SubTypeId = m.SubTypeId;
            e.ClassroomCount3Years = m.ClassroomCount3Years;
            e.ClassroomCount4Years = m.ClassroomCount4Years;
            e.ClassroomCount34Years = m.ClassroomCount34Years;
            e.ClassroomCountKinder = m.ClassroomCountKinder;
            e.ClassroomCountgrade = m.ClassroomCountgrade;
            e.ClassroomCountOther = m.ClassroomCountOther;
            e.ClassroomCountEarly = m.ClassroomCountEarly;
            e.ClassroomCountInfant = m.ClassroomCountInfant;
            e.ClassroomCountToddler = m.ClassroomCountToddler;
            e.ClassroomCountPreSchool = m.ClassroomCountPreSchool;
            e.ClassroomCountPreSchool = m.ClassroomCountPreSchool;
            e.AtRiskPercent = m.AtRiskPercent;
            e.FundingId = m.FundingId;
            //   e.TrsProviderId = m.TrsProviderId;
            e.TrsLastStatusChange = m.TrsLastStatusChange;
            //   e.TrsReviewDate = m.TrsReviewDate;
            e.PrimarySalutation = m.PrimarySalutation;
            e.PrimaryName = m.PrimaryName;
            e.PrimaryTitleId = m.PrimaryTitleId;
            e.PrimaryPhone = m.PrimaryPhone;
            e.PrimaryPhoneType = m.PrimaryPhoneType;
            e.PrimaryEmail = m.PrimaryEmail;
            e.SecondarySalutation = m.SecondarySalutation;
            e.SecondaryName = m.SecondaryName;
            e.SecondaryTitleId = m.SecondaryTitleId;
            e.SecondaryPhoneNumber = m.SecondaryPhoneNumber;
            e.SecondaryPhoneType = m.SecondaryPhoneType;
            e.SecondaryEmail = m.SecondaryEmail;
            e.Latitude = m.Latitude;
            e.Longitude = m.Longitude;
            e.IsSamePhysicalAddress = m.IsSamePhysicalAddress;
            e.MailingAddress1 = m.MailingAddress1;
            e.MailingAddress2 = m.MailingAddress2;
            e.MailingCity = m.MailingCity;
            e.MailingCountyId = m.MailingCountyId;
            e.MailingStateId = m.MailingStateId;
            e.MailingZip = m.MailingZip;
            e.SchoolSize = m.SchoolSize;
            e.IspId = m.IspId;
            e.IspOther = m.IspOther;
            e.InternetSpeed = m.InternetSpeed;
            e.InternetType = m.InternetType;
            e.WirelessType = m.WirelessType;
            e.Notes = m.Notes;
            e.CreatedOn = m.CreatedOn;
            e.UpdatedOn = m.UpdatedOn;
            e.SchoolNumber = m.SchoolNumber;
            e.FacilityType = m.FacilityType;
            e.CreateBy = m.CreateBy;
            e.UpdateBy = m.UpdateBy;
            e.CreateFrom = m.CreateFrom;
            e.UpdateFrom = m.UpdateFrom;

            #region TRS
            e.TrsAssessorId = m.TrsAssessorId.HasValue ? m.TrsAssessorId.Value : 0;
            e.TrsTaStatus = m.TrsTaStatus;
            e.StarStatus = m.StarStatus;
            e.DfpsNumber = m.DfpsNumber;
            e.OwnerFirstName = m.OwnerFirstName;
            e.OwnerLastName = m.OwnerLastName;
            e.OwnerEmail = m.OwnerEmail;
            e.OwnerPhone = m.OwnerPhone;
            e.FacilityType = m.FacilityType;
            e.RegulatingEntity = m.RegulatingEntity;
            e.NAEYC = m.NAEYC;
            e.CANASA = m.CANASA;
            e.NECPA = m.NECPA;
            e.NACECCE = m.NACECCE;
            e.NAFCC = m.NAFCC;
            e.ACSI = m.ACSI;
            e.USMilitary = m.USMilitary;
            e.VSDesignation = m.VSDesignation;
            e.StarDate = m.StarDate;
            e.StarAssessmentType = m.StarAssessmentType;
            #endregion
            return e;
        }

        private Expression<Func<SchoolEntity, bool>> GetRoleCondition(UserBaseEntity userInfo)
        {
            Expression<Func<SchoolEntity, bool>> condition = o => true;
            if (userInfo == null)
                return condition;
            UserBusiness userBusiness = new UserBusiness();
            UserBaseEntity baseUser = userBusiness.GetUser(userInfo.ID);
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
                          o => o.CommunitySchoolRelations.Any(p => p.Community.UserCommunitySchools.Any(q => q.UserId == userInfo.ID)));
                    break;

                case Role.Teacher:   //teacher所在的school
                    if (baseUser.TeacherInfo != null)
                        condition = PredicateHelper.And(condition, o => o.UserCommunitySchools.Any(p => p.UserId == userInfo.ID));
                    break;
                case Role.Principal:  //当前school
                case Role.School_Specialist:
                case Role.TRS_Specialist:
                    if (baseUser.Principal != null)
                    {
                        condition = PredicateBuilder.And(condition, o => o.UserCommunitySchools.Any(p => p.UserId == baseUser.ID));
                    }
                    break;
                case Role.Principal_Delegate:
                case Role.School_Specialist_Delegate:
                case Role.TRS_Specialist_Delegate:
                    if (baseUser.Principal != null)
                    {
                        condition = PredicateBuilder.And(condition,
                            o => o.UserCommunitySchools.Any(p => p.UserId == baseUser.Principal.ParentId));
                    }
                    break;
                case Role.Statewide://当前community下的所有school
                case Role.Community:
                case Role.District_Community_Specialist:

                    if (baseUser.CommunityUser != null || baseUser.StateWide != null)
                        condition = PredicateBuilder.And(condition,
                            o => o.CommunitySchoolRelations.Any(p => p.Community.UserCommunitySchools.Any(q => q.UserId == userInfo.ID) ||
                                 p.School.UserCommunitySchools.Any(q => q.UserId == userInfo.ID)));


                    break;
                case Role.District_Community_Delegate:
                case Role.Community_Specialist_Delegate:
                    if (baseUser.CommunityUser != null)
                        condition = PredicateBuilder.And(condition,
                            o =>
                                o.CommunitySchoolRelations.Any(
                                    p =>
                                        p.Community.UserCommunitySchools.Any(
                                            q => q.UserId == userInfo.CommunityUser.ParentId)));
                    break;
                case Role.Parent:
                    condition = o => false;
                    break;
            }
            return condition;
        }

        private Expression<Func<SchoolEntity, bool>> GetPrimaryRoleCondition(UserBaseEntity userInfo)
        {
            Expression<Func<SchoolEntity, bool>> condition = o => true;
            if (userInfo == null)
                return condition;
            UserBusiness userBusiness = new UserBusiness();
            UserBaseEntity baseUser = userBusiness.GetUser(userInfo.ID);
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
                    o => o.CommunitySchoolRelations.Any(p => p.Community.UserCommunitySchools.Any(q => q.UserId == userInfo.ID)));
                    break;

                case Role.Teacher: //teacher所在的school
                    if (baseUser.TeacherInfo != null)
                        condition = PredicateHelper.And(condition,
                            o => o.UserCommunitySchools.Any(p => p.UserId == userInfo.ID));
                    break;
                case Role.Principal: //当前school
                case Role.School_Specialist:
                case Role.TRS_Specialist:
                    if (baseUser.Principal != null)
                    {
                        condition = PredicateBuilder.And(condition,
                            o => o.UserCommunitySchools.Any(p => p.UserId == baseUser.ID));
                    }
                    break;
                case Role.Principal_Delegate:
                case Role.School_Specialist_Delegate:
                case Role.TRS_Specialist_Delegate:
                    if (baseUser.Principal != null)
                    {
                        condition = PredicateBuilder.And(condition,
                            o => o.UserCommunitySchools.Any(p => p.UserId == baseUser.Principal.ParentId));
                    }
                    break;
                case Role.Statewide: //当前community下的primary school
                case Role.Community:
                case Role.District_Community_Specialist:
                    if (baseUser.CommunityUser != null || baseUser.StateWide != null)
                    {
                        var primarySchoolIds =
                            GetPrimarySchoolsByComId(baseUser.UserCommunitySchools.Select(e => e.CommunityId).ToList())
                                .Select(e => e.ID);
                        condition = PredicateBuilder.And(condition, o => primarySchoolIds.Contains(o.ID));
                    }
                    break;
                case Role.District_Community_Delegate:
                case Role.Community_Specialist_Delegate:
                    if (baseUser.CommunityUser != null)
                    {
                        var parentCommunityUser = userBusiness.GetUser(baseUser.ID);
                        var primarySchoolIds =
                            GetPrimarySchoolsByComId(parentCommunityUser.UserCommunitySchools.Select(e => e.CommunityId).ToList())
                                .Select(e => e.ID);
                        condition = PredicateBuilder.And(condition, o => primarySchoolIds.Contains(o.ID));
                    }
                    break;
                case Role.Parent:
                    condition = o => false;
                    break;
            }
            return condition;
        }

        private Expression<Func<SchoolEntity, bool>> GetTrsSpecialistCondition(UserBaseEntity userInfo)
        {
            Expression<Func<SchoolEntity, bool>> condition = o => true;
            if (userInfo == null)
                return condition;
            UserBusiness userBusiness = new UserBusiness();
            UserBaseEntity baseUser = userBusiness.GetUser(userInfo.ID);
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
                    o => o.CommunitySchoolRelations.Any(p => p.Community.UserCommunitySchools.Any(q => q.UserId == userInfo.ID)));
                    break;

                case Role.Teacher: //teacher所在的school
                    if (baseUser.TeacherInfo != null)
                        condition = PredicateHelper.And(condition,
                            o => o.UserCommunitySchools.Any(p => p.UserId == userInfo.ID));
                    break;
                case Role.Principal: //当前school
                case Role.School_Specialist:
                case Role.TRS_Specialist:
                    if (baseUser.Principal != null)
                    {
                        condition = PredicateBuilder.And(condition,
                            o => o.UserCommunitySchools.Any(p => p.UserId == baseUser.ID));
                    }
                    break;
                case Role.Principal_Delegate:
                case Role.School_Specialist_Delegate:
                case Role.TRS_Specialist_Delegate:
                    if (baseUser.Principal != null)
                    {
                        condition = PredicateBuilder.And(condition,
                            o => o.UserCommunitySchools.Any(p => p.UserId == baseUser.Principal.ParentId));
                    }
                    break;
                case Role.Statewide: //当前community下的primary school
                    if (baseUser.CommunityUser != null || baseUser.StateWide != null)
                    {
                        var primarySchoolIds =
                            GetPrimarySchoolsByComId(baseUser.UserCommunitySchools.Select(e => e.CommunityId).ToList())
                                .Select(e => e.ID);
                        condition = PredicateBuilder.And(condition, o => primarySchoolIds.Contains(o.ID));
                    }
                    break;
                case Role.Community:
                case Role.District_Community_Specialist:
                    if (baseUser.CommunityUser != null || baseUser.StateWide != null)
                    {
                        var primarySchoolIds =
                            GetPrimarySchoolsByComId(baseUser.UserCommunitySchools.Select(e => e.CommunityId).ToList())
                                .Select(e => e.ID);
                        condition = PredicateBuilder.And(condition, o => primarySchoolIds.Contains(o.ID) ||
                                                                         o.CommunitySchoolRelations.Any(
                                                                             s =>
                                                                                 s.Community.UserCommunitySchools.Any(
                                                                                     t => t.UserId == baseUser.ID)));
                    }
                    break;
                case Role.District_Community_Delegate:
                case Role.Community_Specialist_Delegate:
                    if (baseUser.CommunityUser != null)
                    {
                        var parentCommunityUser = userBusiness.GetUser(baseUser.ID);
                        var primarySchoolIds =
                            GetPrimarySchoolsByComId(parentCommunityUser.UserCommunitySchools.Select(e => e.CommunityId).ToList())
                                .Select(e => e.ID);
                        condition = PredicateBuilder.And(condition, o => primarySchoolIds.Contains(o.ID));
                    }
                    break;
                case Role.Parent:
                    condition = o => false;
                    break;
            }
            return condition;
        }
        #endregion

        #region Cpalls Methods
        public List<int> GetSchoolIds(Expression<Func<SchoolEntity, bool>> condition, UserBaseEntity user, string sort, string order, int first, int count, out int total)
        {
            var query = _schoolService.Schools.AsExpandable().Where(condition).Where(GetRoleCondition(user)).OrderBy(sort, order).Select(r => r.ID);
            total = query.Count();
            var list = query.Skip(first).Take(count);
            return list.ToList();
        }

        public List<CpallsSchoolModel> GetSchoolList(Expression<Func<SchoolEntity, bool>> condition, UserBaseEntity user, string sort, string order, int first, int count, out int total)
        {
            var query = _schoolService.Schools.AsExpandable().Where(condition).Where(GetRoleCondition(user)).OrderBy(sort, order).Select(r => new CpallsSchoolModel()
            {
                ID = r.ID,
                Name = r.Name,
                Communities = r.CommunitySchoolRelations.Where(c => c.Status == EntityStatus.Active).Select(c => new CpallsCommunityModel()
                {
                    ID = c.Community.ID,
                    Name = c.Community.Name
                })
            });
            total = query.Count();
            var list = query.Skip(first).Take(count);
            return list.ToList();
        }

        public List<CpallsSchoolCustomScoreModel> GetSchoolList(Expression<Func<SchoolEntity, bool>> condition, UserBaseEntity user, StudentAssessmentLanguage language, DateTime dobStartDate, DateTime dobEndDate)
        {
            var query = _schoolService.Schools.AsExpandable().Where(condition).Where(GetRoleCondition(user)).Select(r => new CpallsSchoolCustomScoreModel()
            {
                ID = r.ID,
                Name = r.Name,
                StudentIds = r.StudentRelations.Where(sr => sr.Student.Status == EntityStatus.Active && sr.Student.IsDeleted == false
                 && (sr.Student.BirthDate >= dobStartDate && sr.Student.BirthDate <= dobEndDate)
                 && (sr.Student.AssessmentLanguage == language || sr.Student.AssessmentLanguage == StudentAssessmentLanguage.Bilingual))
                .Select(s => s.StudentId)
            });
            var list = query;
            return list.ToList();
        }

        public CpallsSchoolModel GetCpallsSchoolModel(int schoolId)
        {
            if (schoolId < 1) return new CpallsSchoolModel();

            return _schoolService.Schools.Where(r => r.ID == schoolId).Select(r => new CpallsSchoolModel()
            {
                ID = r.ID,
                Name = r.Name,
                Communities = r.CommunitySchoolRelations.Where(c => c.Status == EntityStatus.Active).Select(c => new CpallsCommunityModel()
                {
                    ID = c.Community.ID,
                    Name = c.Community.Name
                }).OrderBy(c => c.Name)
            }).FirstOrDefault();
        }


        public IEnumerable<int> GetSchoolIds(UserBaseEntity user)
        {
            return _schoolService.Schools.AsExpandable().Where(GetRoleCondition(user)).Select(r => r.ID);
        }

        public List<int> GetSchoolIds(List<int> communityIds)
        {
            return _schoolService.GetSchoolIds(communityIds.ToArray());
        }

        public int GetMaxSchoolId()
        {
            return _schoolService.Schools.OrderByDescending(x => x.ID).Select(s => s.ID).First();
        }

        #endregion

        #region TrsProvider
        public TrsProviderEntity NewTrsProviderEntity()
        {
            return _schoolService.NewTrsProviderEntity();
        }

        public OperationResult InsertTrsProvider(TrsProviderEntity entity)
        {
            return _schoolService.InsertTrsProvider(entity);
        }

        public OperationResult UpdateTrsProvider(TrsProviderEntity entity)
        {
            return _schoolService.UpdateTrsProvider(entity);
        }

        public TrsProviderEntity GetTrsProvider(int id)
        {
            return _schoolService.GetTrsProvider(id);
        }
        #endregion

        #region TRS


        public bool CheckShowAgeofChildren(int schoolId)
        {
            int trsId = (int)LocalAssessment.TexasRisingStar;
            int beachId = (int)LocalAssessment.TexasRisingStar;
            return _schoolService.Schools.Where(r => r.ID == schoolId && r.CommunitySchoolRelations
                .Count(c => c.Community.CommunityAssessmentRelations.Select(o => o.AssessmentId).Contains(beachId)
                || c.Community.CommunityAssessmentRelations.Select(o => o.AssessmentId).Contains(trsId)) > 0).Count() > 0;

        }

        public TrsSchoolModel GetTrsSchool(int schoolId, UserBaseEntity user)
        {//
            int trsId = (int)LocalAssessment.TexasRisingStar;
            var query = _schoolService.Schools.AsExpandable()
               .Where(GetRoleCondition(user))
                .Where(s => s.TrsAssessorId > 0 && s.Status == SchoolStatus.Active
                    && s.CommunitySchoolRelations.Any(r => (r.Community.CommunityAssessmentRelations.Select(o => o.AssessmentId).Contains(trsId))))//Texas Rising Star
               .Select(SelectorEntityToTrsSchoolModel).Where(x => x.ID == schoolId);
            return query.FirstOrDefault();
        }

        private static Expression<Func<SchoolEntity, TrsSchoolModel>> SelectorEntityToTrsSchoolModel
        {
            get
            {
                return x => new TrsSchoolModel()
                {
                    ID = x.ID,
                    Assessor = new UserBaseModel()
                    {
                        UserId = x.TrsAssessorId.HasValue ? x.TrsAssessorId.Value : 0,
                        FirstName = x.TrsAssessorId > 0 ? x.Assessor.FirstName : "",
                        LastName = x.TrsAssessorId > 0 ? x.Assessor.LastName : "",
                    },
                    Principals = x.UserCommunitySchools.Where(p => p.User.Role == Role.Principal).Select(p => new UserBaseModel()
                    {
                        UserId = p.User.ID,
                        FirstName = p.User.FirstName,
                        LastName = p.User.LastName
                    }),
                    StarStatus = x.StarStatus,
                    VerifiedStar = x.VSDesignation,
                    StarDesignationDate = x.StarDate,
                    RecertificationBy = x.RecertificatedBy,
                    CommunityIds = x.CommunitySchoolRelations.Where(c => c.Status == EntityStatus.Active).Select(c => c.CommunityId),
                    Communities = x.CommunitySchoolRelations.Where(c => c.Status == EntityStatus.Active).Select(c => c.Community.BasicCommunity.Name),
                    Name = x.BasicSchool.Name,
                    DfpsNumber = x.DfpsNumber,
                    Status = x.Status,
                    FacilityType = x.FacilityType
                };
            }
        }

        /// <summary>
        /// Gets the TRS schools(内部默认条件:用户的权限,TrsAssessorId>0,TrsMentorId>0,Status:Active).
        /// </summary>
        /// <param name="condition">The condition.</param>
        /// <param name="user">The user.</param>
        /// <param name="sort">The sort.</param>
        /// <param name="order">The order.</param>
        /// <param name="first">The first.</param>
        /// <param name="count">The count.</param>
        /// <param name="total">The total.</param>
        /// <returns></returns>
        public List<TrsSchoolModel> GetTrsSchools(Expression<Func<SchoolEntity, bool>> condition, UserBaseEntity user,
            string sort, string order, int first, int count, out int total)
        {
            var query = _schoolService.Schools.AsExpandable()
                .Where(s => s.TrsAssessorId > 0 && s.Status == SchoolStatus.Active)
                .Where(condition)
                .Select(SelectorEntityToTrsSchoolModel);

            if (sort == "ActionRequired")
            {
                var schools = query.ToList();
                List<int> schoolIds = schools.Select(r => r.ID).Distinct().ToList();
                List<TRSEventLogEntity> eventLogs = TrsBusiness.GetEventLogListBySchooIds(schoolIds);
                foreach (TrsSchoolModel item in schools)
                {
                    TRSEventLogEntity eventLog = eventLogs.Where(r => r.SchoolId == item.ID).OrderByDescending(r => r.ActionRequired).FirstOrDefault();
                    item.ActionRequired = eventLog == null ? CommonAgent.MinDate : eventLog.ActionRequired;
                }
                total = query.Count();
                return schools.OrderBy(sort, order).Skip(first).Take(count).ToList();
            }
            total = query.Count();
            return query.OrderBy(sort, order).Skip(first).Take(count).ToList();
        }

        //Trs Offline时查询School信息
        public List<TrsOfflineSchoolModel> GetTrsOfflineSchools(Expression<Func<SchoolEntity, bool>> condition, UserBaseEntity user,
            string sort, string order, int first, int count, out int total)
        {
            int trsId = (int)LocalAssessment.TexasRisingStar;
            var query = _schoolService.Schools.AsExpandable()
                .Where(s => s.TrsAssessorId > 0 && s.Status == SchoolStatus.Active)
                .Where(condition)
                .Select(x => new TrsOfflineSchoolModel
                {
                    ID = x.ID,
                    Name = x.Name,
                    Assessor = new UserBaseModel()
                    {
                        UserId = x.TrsAssessorId.HasValue ? x.TrsAssessorId.Value : 0,
                        FirstName = x.TrsAssessorId > 0 ? x.Assessor.FirstName : "",
                        LastName = x.TrsAssessorId > 0 ? x.Assessor.LastName : "",
                    },
                    Principals = x.UserCommunitySchools.Where(p => p.User.Role == Role.Principal).Select(p => new UserBaseModel()
                    {
                        UserId = p.User.ID,
                        FirstName = p.User.FirstName,
                        LastName = p.User.LastName
                    }),
                    StarStatus = x.StarStatus,
                    StarDate = x.StarDate,
                    VerifiedStar = x.VSDesignation,
                    TrsLastStatusChange = x.TrsLastStatusChange,
                    RecertificationBy = x.RecertificatedBy,
                    CommunityIds = x.CommunitySchoolRelations.Where(c => c.Status == EntityStatus.Active).Select(c => c.CommunityId),
                    Communities = x.CommunitySchoolRelations.Where(c => c.Status == EntityStatus.Active).Select(c => c.Community.BasicCommunity.Name),
                    FacilityType = x.FacilityType,
                    IsCommunityTRS = x.CommunitySchoolRelations.Any(r => (r.Community.CommunityAssessmentRelations.Select(o => o.AssessmentId).Contains(trsId))),
                    //Preview 时使用
                    Address = x.PhysicalAddress1,
                    City = x.City,
                    State = x.State.Name,
                    Zip = x.Zip,
                    FacilityTelephone = x.PrimaryPhone,
                    SecondaryTelephone = x.SecondaryPhoneNumber,
                    Owner = x.OwnerFirstName + " " + x.OwnerLastName,
                    Directors = x.PrimaryName,
                    RegulatingEntity = x.RegulatingEntity,
                    NAEYC = x.NAEYC,
                    CANASA = x.CANASA,
                    NECPA = x.NECPA,
                    NACECCE = x.NACECCE,
                    NAFCC = x.NAFCC,
                    ACSI = x.ACSI,
                    DFPS = x.DfpsNumber,
                    TrsTaStatus = x.TrsTaStatus
                });
            total = query.Count();
            return query.OrderBy(sort, order).Skip(first).Take(count).ToList();
        }

        public TrsSchoolReportModel GetTrsSchoolReport(UserBaseEntity user, int schoolId)
        {
            var model = _schoolService.Schools.AsExpandable()
                .Where(GetRoleCondition(user))
                .Where(a => a.TrsAssessorId > 0 && a.ID == schoolId && a.Status == SchoolStatus.Active).Select(
                r => new TrsSchoolReportModel
                {
                    SchoolId = r.ID,
                    SchoolName = r.BasicSchool.Name,
                    CommunityIds = r.CommunitySchoolRelations.Where(c => c.Status == EntityStatus.Active).Select(c => c.CommunityId),
                    Communities = r.CommunitySchoolRelations.Where(c => c.Status == EntityStatus.Active).Select(c => c.Community.BasicCommunity.Name),
                    Address = r.PhysicalAddress1,
                    City = r.City,
                    State = r.State.Name,
                    Zip = r.Zip,
                    FacilityTelephone = r.PrimaryPhone,
                    SecondaryTelephone = r.SecondaryPhoneNumber,
                    Owner = r.OwnerFirstName + " " + r.OwnerLastName,
                    PrimaryName = r.PrimaryName,
                    RegulatingEntity = r.RegulatingEntity,
                    FacilityType = r.FacilityType,
                    Assessor = r.Assessor.FirstName + " " + r.Assessor.LastName,
                    NAEYC = r.NAEYC,
                    CANASA = r.CANASA,
                    NECPA = r.NECPA,
                    NACECCE = r.NACECCE,
                    NAFCC = r.NAFCC,
                    ACSI = r.ACSI,
                    DFPS = r.DfpsNumber
                }).FirstOrDefault();
            return model;
        }

        /// <summary>
        /// 更新School的Trs相关字段.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="status">The status.</param>
        /// <param name="updateStar">是否更新评级.</param>
        /// <param name="star">The star，为空或者One表示不更新该字段.</param>
        /// <returns></returns>
        public OperationResult SaveSchool(int id, List<TrsTaStatus> status, DateTime recertificatedOn,
            DateTime starUpdatedon, TrsAssessmentType starAssessmentType, bool updateStar = false, TRSStarEnum star = 0)
        {
            var entity = _schoolService.GetSchool(id);
            if (entity == null)
                return new OperationResult(OperationResultType.Error, "School is null.");
            entity.TrsTaStatus = string.Join(",", status.Select(x => (int)x));
            entity.RecertificatedBy = recertificatedOn;
            if (updateStar && star != 0)
            {
                entity.StarStatus = star;
                entity.StarDate = starUpdatedon;
                switch (starAssessmentType)
                {
                    case TrsAssessmentType.Initial:
                        entity.StarAssessmentType = StarAssessmentType.Initial;
                        break;
                    case TrsAssessmentType.Recertification:
                        entity.StarAssessmentType = StarAssessmentType.Recertification;
                        break;
                    case TrsAssessmentType.FacilityChanges:
                        entity.StarAssessmentType = StarAssessmentType.FacilityChanges;
                        break;
                    case TrsAssessmentType.CategoryReassessment:
                        entity.StarAssessmentType = StarAssessmentType.CategoryReassessment;
                        break;
                    case TrsAssessmentType.StarLevelEvaluation:
                        entity.StarAssessmentType = StarAssessmentType.StarLevelEvaluation;
                        break;
                    case TrsAssessmentType.AnnualMonitoring:
                        entity.StarAssessmentType = StarAssessmentType.AnnualMonitoring;
                        break;
                }
            }
            return _schoolService.UpdateSchool(entity);
        }

        public OperationResult SaveSchool(int id, TRSStarEnum verifiedStar)
        {
            var entity = _schoolService.GetSchool(id);
            if (entity == null)
                return new OperationResult(OperationResultType.Error, "School is null.");
            entity.VSDesignation = verifiedStar;
            entity.TrsLastStatusChange = DateTime.Now;
            return _schoolService.UpdateSchool(entity);
        }

        public SchoolEntity GetSchool(int id)
        {
            return _schoolService.GetSchool(id);
        }

        public SchoolEntity GetSchool(string engageId)
        {
            return _schoolService.Schools.FirstOrDefault(e => e.SchoolId == engageId && e.Status == SchoolStatus.Active);
        }

        public bool IsSchoolNameExsits(string newName, int currentSchoolId)
        {
            return _schoolService.Schools.Any(o => o.Name == newName && o.ID != currentSchoolId);
        }

        public bool IsBasicSchoolNameExsits(string newName, int currentSchoolId)
        {
            return _schoolService.BasicSchools.Any(o => o.Name == newName && o.ID != currentSchoolId);
        }

        public OperationResult UpdateSchool(SchoolEntity school)
        {
            return _schoolService.UpdateSchool(school);
        }

        public bool ShowTrs(int id)
        {
            int trsId = (int)LocalAssessment.TexasRisingStar;
            return
                _schoolService.Schools.Any(x => x.ID == id
                    && x.CommunitySchoolRelations.Count(r => r.Community.CommunityAssessmentRelations.Select(o => o.AssessmentId).Contains(trsId)) > 0);
            //任意一个community 启用了 trs 就算
        }
        #endregion

        #region ParentAgency
        public ParentAgencyEntity NewParentAgencyEntity()
        {
            return _schoolService.NewParentAgencyEntity();
        }

        public OperationResult InsertParentAgency(ParentAgencyEntity entity)
        {
            return _schoolService.InsertParentAgency(entity);
        }

        public OperationResult UpdateParentAgency(ParentAgencyEntity entity)
        {
            return _schoolService.UpdateParentAgency(entity);
        }

        public ParentAgencyEntity GetParentAgency(int id)
        {
            return _schoolService.GetParentAgency(id);
        }
        #endregion

        #region Isp
        public IspEntity NewIspEntity()
        {
            return _schoolService.NewIspEntity();
        }

        public OperationResult InsertIsp(IspEntity entity)
        {
            return _schoolService.InsertIsp(entity);
        }

        public OperationResult UpdateIsp(IspEntity entity)
        {
            return _schoolService.UpdateIsp(entity);
        }

        public IspEntity GetIsp(int id)
        {
            return _schoolService.GetIsp(id);
        }
        #endregion

        #region SchoolType
        public SchoolTypeEntity NewSchoolTypeEntity()
        {
            return _schoolService.NewSchoolTypeEntity();
        }

        public OperationResult InsertSchoolType(SchoolTypeEntity entity)
        {
            return _schoolService.InsertSchoolType(entity);
        }

        public OperationResult UpdateSchoolType(SchoolTypeEntity entity)
        {
            return _schoolService.UpdateSchoolType(entity);
        }

        public SchoolTypeEntity GetSchoolType_(int id)
        {
            return _schoolService.GetSchoolType(id);
        }
        #endregion

        #region Playgrounds
        public OperationResult InsertPlaygrounds(List<PlaygroundEntity> grounds)
        {
            OperationResult result = new OperationResult(OperationResultType.Success);
            foreach (PlaygroundEntity playgroundEntity in grounds)
            {
                result = _schoolService.InsertPlayground(playgroundEntity);
                if (result.ResultType == OperationResultType.Error)
                    break;
            }
            return result;
        }
        public OperationResult InsertPlayground(PlaygroundEntity ground)
        {
            OperationResult result = new OperationResult(OperationResultType.Success);
            result = _schoolService.InsertPlayground(ground);
            return result;
        }

        public List<PlaygroundEntity> GetPlaygrounds(int schoolId)
        {
            return _schoolService.Playgrounds.Where(o => o.SchoolId == schoolId).ToList();
        }

        public OperationResult DeletePlayground(int Id)
        {
            OperationResult result = new OperationResult(OperationResultType.Success);
            result = _schoolService.DeletePlayground(Id);
            return result;
        }

        #endregion

        #region School Community Relations

        public OperationResult InsertCommunitySchoolRelations(List<CommunitySchoolRelationsEntity> entities)
        {
            return _schoolService.InsertRelations(entities);
        }

        public OperationResult InsertCommunitySchoolRelations(int currentUserId, int schoolId, int[] comIds)
        {
            List<CommunitySchoolRelationsEntity> list = new List<CommunitySchoolRelationsEntity>();
            List<UserComSchRelationEntity> userlist = new List<UserComSchRelationEntity>();
            foreach (int comId in comIds)
            {
                CommunitySchoolRelationsEntity entity = new CommunitySchoolRelationsEntity();
                entity.SchoolId = schoolId;
                entity.CommunityId = comId;
                entity.Status = EntityStatus.Active;
                entity.CreatedBy = currentUserId;
                entity.CreatedOn = DateTime.Now;
                entity.UpdatedOn = DateTime.Now;
                entity.UpdatedBy = currentUserId;
                list.Add(entity);


            }
            return _schoolService.InsertRelations(list);
        }


        public OperationResult DeleteCommunitySchoolRelations(int comId, int[] schoolIds)
        {
            IList<CommunitySchoolRelationsEntity> list = _schoolService.GetRelationsByCommunityId(comId, schoolIds);
            return _schoolService.DelRelations(list);
        }
        public OperationResult DeleteCommunitySchoolRelations2(int schoolId, int[] comIds)
        {
            IList<CommunitySchoolRelationsEntity> list = _schoolService.GetRelationsBySchoolId(schoolId, comIds);
            return _schoolService.DelRelations(list);
        }

        public int[] GetAssignedCommIds(int schoolId)
        {
            return _schoolService.GetCommIds(schoolId);
        }
        public IList<int> GetAssignedCommIds(IList<int> schoolIds)
        {
            if (schoolIds.Count == 0)
                return new List<int>();
            return _schoolService.GetCommunityIds(schoolIds.ToArray());
        }
        public IList<CommunitySchoolRelationModel> GetAssignedCommunities(UserBaseEntity user, Expression<Func<CommunitySchoolRelationsEntity, bool>> condition,
           string sort, string order, int first, int count, out int total)
        {
            var query = _schoolService.CommunitySchoolRelations.AsExpandable().Where(condition).Where(GetSchoolCommunityRoleCondition(user)).Select(o => new CommunitySchoolRelationModel()
            {
                ID = o.ID,
                CommunityId = o.CommunityId,
                CommunityName = o.Community.BasicCommunity.Name
            });
            total = query.Count();
            var list = query.OrderBy(sort, order).Skip(first).Take(count);
            return list.ToList();
        }

        public IList<CommunitySchoolRelationModel> GetSchoolCommunitiy(UserBaseEntity user,
            Expression<Func<CommunitySchoolRelationsEntity, bool>> condition, string sort,
            string order, int first, int count, out int total)
        {
            var query =
                _schoolService.CommunitySchoolRelations.AsExpandable()
                    .Where(GetSchoolCommunityRoleCondition(user))
                    .Where(condition);
            total = query.Count();
            var list = query.Select(o => new CommunitySchoolRelationModel()
            {
                ID = o.ID,
                SchoolId = o.SchoolId,
                SchoolName = o.School.Name,
                CommunityId = o.CommunityId,
                CommunityName = o.Community.Name
            }).OrderBy(sort, order).Skip(first).Take(count);
            return list.ToList();
        }

        private Expression<Func<CommunitySchoolRelationsEntity, bool>> GetSchoolCommunityRoleCondition(UserBaseEntity userInfo)
        {
            Expression<Func<CommunitySchoolRelationsEntity, bool>> condition = o => true;
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
                    condition = PredicateBuilder.And(condition, p => p.Community.UserCommunitySchools.Any(q => q.UserId == userInfo.ID));
                    break;

                case Role.Principal:
                case Role.Principal_Delegate:
                case Role.School_Specialist:
                case Role.School_Specialist_Delegate:
                case Role.TRS_Specialist:
                case Role.TRS_Specialist_Delegate:
                    if (userInfo.Principal != null)
                    {
                        condition = PredicateBuilder.And(condition,
                            o => o.School.UserCommunitySchools.Any(p => p.UserId == userInfo.ID));
                    }
                    break;
                case Role.Statewide:
                case Role.Community:
                case Role.District_Community_Specialist:
                    if (userInfo.CommunityUser != null || userInfo.StateWide != null)
                    {
                        var userCommunityIds = userInfo.UserCommunitySchools.Select(e => e.CommunityId).ToList();
                        var primarySchoolIds = GetPrimarySchoolsByComId(userCommunityIds).Select(e => e.ID);
                        condition = PredicateBuilder.And(condition,
                            o => userCommunityIds.Contains(o.CommunityId) && primarySchoolIds.Contains(o.SchoolId));
                    }
                    break;
                case Role.District_Community_Delegate:
                case Role.Community_Specialist_Delegate:
                    if (userInfo.CommunityUser != null || userInfo.StateWide != null)
                    {
                        var parentCommunityUser = _userBusiness.GetUser(userInfo.ID);
                        var parentUserCommunityIds = parentCommunityUser.UserCommunitySchools.Select(e => e.CommunityId).ToList();
                        var primarySchoolIds = GetPrimarySchoolsByComId(parentUserCommunityIds).Select(e => e.ID);
                        condition = PredicateBuilder.And(condition,
                            o => parentUserCommunityIds.Contains(o.CommunityId) && primarySchoolIds.Contains(o.SchoolId));
                    }
                    break;
                case Role.Teacher:
                case Role.Parent:
                    condition = o => false;
                    break;
            }
            return condition;
        }
        #endregion

        #region School Student Relation

        public bool CheckSchooStudentRelations(int schoolId, int studentId)
        {
            return _schoolService.SchoolStudentRelation.Count(r => r.SchoolId == schoolId && r.StudentId == studentId) > 0;
        }

        public List<int> GetSchoolsForStudent(int studentId)
        {
            return
                _schoolService.SchoolStudentRelation.Where(x => x.StudentId == studentId)
                    .Select(x => x.SchoolId)
                    .ToList();
        }

        public OperationResult InserSchoolStudentRelations(List<int> studentIds, int schoolId, UserBaseEntity user)
        {
            List<SchoolStudentRelationEntity> schoolStudententities = new List<SchoolStudentRelationEntity>();
            foreach (int studentId in studentIds)
            {
                if (CheckSchooStudentRelations(schoolId, studentId) == false)
                {
                    SchoolStudentRelationEntity schoolStudent = new SchoolStudentRelationEntity();
                    schoolStudent.StudentId = studentId;
                    schoolStudent.SchoolId = schoolId;
                    schoolStudent.CreatedBy = user.ID;
                    schoolStudent.UpdatedBy = user.ID;
                    schoolStudent.Status = EntityStatus.Active;
                    schoolStudententities.Add(schoolStudent);
                }
            }
            return _schoolService.InsertSchoolStudentRelations(schoolStudententities);
        }

        public OperationResult DeleteRelations(List<SchoolStudentRelationEntity> entities)
        {
            return _schoolService.DeleteSchoolStudentRelations(entities);
        }

        public List<SchoolStudentRelationEntity> GetSchoolStudentRelationList(Expression<Func<SchoolStudentRelationEntity, bool>> condition)
        {
            return _schoolService.SchoolStudentRelation.AsExpandable().Where(condition).ToList();
        }
        #endregion


        #region BUP

        public List<NameModel> GetAllSchools()
        {
            return _schoolService.Schools.AsExpandable()
                .Select(s => new NameModel {InternalId = s.SchoolNumber, Name = s.Name, EngageId = s.SchoolId}).ToList();
        }

        //根据用户和Community获取对应的PrimarySchool
        public List<NameModel> GetPrimarySchools(UserBaseEntity user, int communityId)
        {
            return _schoolService.Schools.AsExpandable()
                .Where(o => o.CommunitySchoolRelations.Any(c => c.CommunityId == communityId))
                .Where(GetPrimaryRoleCondition(user))
                .Select(s => new NameModel { EngageId = s.SchoolId, InternalId = s.SchoolNumber, Name = s.Name }).ToList();
        }

        //根据用户和Community获取对应的School
        public List<NameModel> GetSchools(UserBaseEntity user, int communityId)
        {
            return _schoolService.Schools.AsExpandable()
                .Where(o => o.CommunitySchoolRelations.Any(c => c.CommunityId == communityId))
                .Where(GetRoleCondition(user))
                .Select(s => new NameModel { EngageId = s.SchoolId, InternalId = s.SchoolNumber, Name = s.Name }).ToList();
        }

        //根据用户获取对应的PrimarySchool
        public List<NameModel> GetPrimarySchools(UserBaseEntity user)
        {
            return _schoolService.Schools.AsExpandable()
                .Where(GetPrimaryRoleCondition(user))
                .Select(s => new NameModel { EngageId = s.SchoolId, InternalId = s.SchoolNumber, Name = s.Name }).ToList();
        }

        public bool IsExistTeacher(string schoolName, string engageId, int userId)
        {
            return _schoolService.Schools.FirstOrDefault(
                e =>
                    e.Name == schoolName && e.SchoolId == engageId &&
                    e.UserCommunitySchools.Any(r => r.SchoolId > 0 && r.UserId == userId)) != null;
        }

        #endregion
    }
    public class SchoolSelectItemModel : SelectItemModel
    {
        public string City { get; set; }
        public string County { get; set; }
        public string State { get; set; }

        public string SchoolId { get; set; }
        public int CommunityId { get; set; }
        public IEnumerable<int> CommunityIds { get; set; }
        public string Address { get; set; }
        public string Address2 { get; set; }

        public int CountyId { get; set; }

        public int StateId { get; set; }
        public string Zip { get; set; }
        public string PhoneNumber { get; set; }
        public string SchoolNumber { get; set; }
    }


}
