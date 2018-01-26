using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Web;
using System.Web.Caching;
using System.Web.Mvc;
using Antlr.Runtime;
using Antlr.Runtime.Tree;
using Microsoft.Ajax.Utilities;
using Newtonsoft.Json;
/**************************************************************************
 * Developer: 		Leason
 * Computer:		Leason-PC
 * Domain:			Leason-pc
 * CreatedOn:		2014/8/22 17:20:20
 * Description:		Create CommunityController
 * Version History:	Created,2014/8/22 17:20:20
 * 
 * 
 **************************************************************************/
using Sunnet.Cli.Business.Ade;
using Sunnet.Cli.Business.Communities;
using Sunnet.Cli.Business.Communities.Enums;
using Sunnet.Cli.Business.Communities.Models;
using Sunnet.Cli.Business.Cpalls.Models;
using Sunnet.Cli.Business.MasterData;
using Sunnet.Cli.Business.Schools;
using Sunnet.Cli.Business.Users;
using Sunnet.Cli.Core.Ade;
using Sunnet.Cli.Core.Common.Enums;
using Sunnet.Cli.Core.Communities.Entities;
using Sunnet.Cli.Core.Users.Entities;
using Sunnet.Cli.Core.Users.Enums;
using Sunnet.Cli.MainSite.Controllers;
using Sunnet.Cli.UIBase.Models;
using Sunnet.Framework;
using Sunnet.Framework.Core.Tool;
using Sunnet.Framework.Extensions;
using Sunnet.Framework.Helpers;
using Sunnet.Cli.Core.Permission.Entities;
using Sunnet.Cli.Business.Permission;
using Sunnet.Framework.Permission;
using Sunnet.Cli.UIBase.Filters;
using Sunnet.Cli.Business.Permission.Models;
using Sunnet.Framework.File;

namespace Sunnet.Cli.MainSite.Areas.Community.Controllers
{
   

    public class CommunityController : BaseController
    {

        #region Private Field
        private readonly CommunityBusiness _communityBusiness = null;
        private readonly SchoolBusiness _schoolBusiness = null;

        private readonly MasterDataBusiness _masterDataBusiness = null;
        private readonly UserBusiness _userBusiness = null;
        private readonly AdeBusiness _adeBusiness = null;
        #endregion

        #region Public Contruction
        public CommunityController()
        {
            _communityBusiness = new CommunityBusiness(UnitWorkContext);
            _masterDataBusiness = new MasterDataBusiness(UnitWorkContext);
            _userBusiness = new UserBusiness(UnitWorkContext);
            _schoolBusiness = new SchoolBusiness(UnitWorkContext);

            _adeBusiness = new AdeBusiness(AdeUnitWorkContext);
        }
        #endregion

        #region Index/New/Edit/View

        [CLIUrlAuthorizeAttribute(Account = Authority.Index, PageId = PagesModel.Community_Management, Anonymity = Anonymous.Verified)]
        public ActionResult Index()
        {

            InitAccessOperation();
            var fundingList = _masterDataBusiness.GetFundingSelectList();
            ViewBag.FundingOptions = new SelectList(fundingList, "ID", "Name").AddDefaultItem("All", -1);
            ViewBag.FeatureQueryString = UserInfo.IsCLIUser ? "FeaturesCli" : "Features";
            ViewBag.IsCliUser = UserInfo.IsCLIUser ;
            return View();
        }

        [CLIUrlAuthorizeAttribute(Account = Authority.View, PageId = PagesModel.Community_Management, Anonymity = Anonymous.Verified)]
        public ActionResult View(int id)
        {
            CommunityRoleEntity roleEntity = _communityBusiness.GetCommunityRoleEntity(UserInfo.Role);
            ViewBag.Role = JsonHelper.SerializeObject(roleEntity);
            var entity = _communityBusiness.GetCommunity(id);
            InitViewParameters(entity);
            ViewBag.communityName = entity.BasicCommunity.Name;
            List<CpallsAssessmentModel> listAssessments = _adeBusiness.GetAssessmentFeatureList(o => o.IsDeleted == false).ToList();
            var localAssessments  = LocalAssessment.ADE.ToList().Select(x => (LocalAssessment)x).ToList();

            ViewBag.localAssessments = localAssessments;
            ViewBag.listAssessments = listAssessments;
            int[] assignedIds = _communityBusiness.GetAssignedApprovedAssessmentIds(id);
            ViewBag.assignedIds = assignedIds;

            return View(entity);
        }

        #region Community New
        [CLIUrlAuthorizeAttribute(Account = Authority.Add, PageId = PagesModel.Community_Management, Anonymity = Anonymous.Verified)]
        public ActionResult New()
        {
            CommunityEntity entity = _communityBusiness.NewCommunityEntity();
            CommunityRoleEntity roleEntity = _communityBusiness.GetCommunityRoleEntity(UserInfo.Role);
            ViewBag.Role = JsonHelper.SerializeObject(roleEntity);
            InitParameters();
            List<CpallsAssessmentModel> listAssessments = _adeBusiness.GetAssessmentFeatureList(o => o.IsDeleted == false).ToList();
            var localAssessments = LocalAssessment.ADE.ToList().Select(x => (LocalAssessment)x).ToList();
            ViewBag.localAssessments = localAssessments;
            ViewBag.listAssessments = listAssessments;
            return View(entity);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [CLIUrlAuthorizeAttribute(Account = Authority.Add, PageId = PagesModel.Community_Management, Anonymity = Anonymous.Verified)]
        public string New(CommunityEntity entity)//, int[] chkAssessment
        {
            var response = new PostFormResponse();

            OperationResult result = _communityBusiness.InsertCommunity(entity, UserInfo.Role);
            if (result.ResultType == OperationResultType.Success)
            {

                //if (UserInfo.IsCLIUser)
                //    _communityBusiness.InsertCommunityAssessmentRelations(UserInfo.ID, entity.ID, chkAssessment, false);
                //else
                //    _communityBusiness.InsertCommunityAssessmentRelations(UserInfo.ID, entity.ID, chkAssessment, true);
            }
            response.Success = result.ResultType == OperationResultType.Success;
            response.Message = result.Message;

            return JsonHelper.SerializeObject(response);
        }
        #endregion

        #region Community Edit
        [CLIUrlAuthorizeAttribute(Account = Authority.Edit, PageId = PagesModel.Community_Management, Anonymity = Anonymous.Verified)]
        public ActionResult Edit(int id)
        {
            var entity = _communityBusiness.GetCommunity(id);
            CommunityRoleEntity roleEntity = _communityBusiness.GetCommunityRoleEntity(UserInfo.Role);
            ViewBag.Role = JsonHelper.SerializeObject(roleEntity);
            InitParameters();
            ViewBag.communityName = entity.BasicCommunity.Name;
            if (entity.MouStatus == false)
            {
                entity.MouDocument = null;
            }
            List<CpallsAssessmentModel> listAssessments = _adeBusiness.GetAssessmentFeatureList(o => o.IsDeleted == false).ToList();

            var localAssessments = LocalAssessment.ADE.ToList().Select(x => (LocalAssessment)x).ToList();

            ViewBag.localAssessments = localAssessments;
            ViewBag.listAssessments = listAssessments;
            int[] assignedIds = _communityBusiness.GetAssignedApprovedAssessmentIds(id);
            ViewBag.assignedIds = assignedIds;
            return View(entity);
        }

        [HttpPost]
        [CLIUrlAuthorizeAttribute(Account = Authority.Edit, PageId = PagesModel.Community_Management, Anonymity = Anonymous.Verified)]
        public string Edit(CommunityEntity entity)//, int[] chkAssessment
        {
            var response = new PostFormResponse();

            OperationResult result = _communityBusiness.UpdateCommunity(entity, UserInfo.Role);

            if (result.ResultType == OperationResultType.Success)
            {
                //if (UserInfo.IsCLIUser)
                //    _communityBusiness.InsertCommunityAssessmentRelations(UserInfo.ID, entity.ID, chkAssessment, false);
                //else
                //{
                //    _communityBusiness.InsertCommunityAssessmentRelations(UserInfo.ID, entity.ID, chkAssessment, true);
                //}
            }
            response.Success = result.ResultType == OperationResultType.Success;
            response.Message = result.Message;
            return JsonHelper.SerializeObject(response);
        }

        [CLIUrlAuthorizeAttribute(Account = Authority.View, PageId = PagesModel.Community_Management, Anonymity = Anonymous.Verified)]
        public string GetCommunity(int communityId)
        {
            var entity = _communityBusiness.GetCommunity(communityId);
            CommunitySelectItemModel model = new CommunitySelectItemModel();
            if (entity != null)
            {
                model.Address = entity.PhysicalAddress1;
                model.Address2 = entity.PhysicalAddress2;
                model.City = entity.City;
                model.StateId = entity.StateId;
                model.CountyId = entity.CountyId;
            }
            return JsonHelper.SerializeObject(model);
        }


        [HttpPost]
        [CLIUrlAuthorizeAttribute(Account = Authority.Edit, PageId = PagesModel.Community_Management, Anonymity = Anonymous.Verified)]
        public string DeletCommunityLogo(int communityId)
        {
            var response = new PostFormResponse();
            OperationResult result = new OperationResult(OperationResultType.Success);
            CommunityEntity community = _communityBusiness.GetCommunity(communityId);
            string communityLogoUrl = SFConfig.UploadFile + community.LogoUrl;
            if (FileHelper.Delete(communityLogoUrl))
            {
                community.LogoUrl = "";
                result = _communityBusiness.UpdateCommunity(community, UserInfo.Role);
            }
            else
            {
                result.ResultType = OperationResultType.Error;
                result.Message = "Delete logo failed.";
            }


            response.Success = result.ResultType == OperationResultType.Success;
            response.Message = result.Message;
            return JsonHelper.SerializeObject(response);
        }

        #endregion

        #endregion

        #region GetSelectList
        /// <summary>
        /// 返回Community下拉框的Json字符串
        /// </summary>
        /// <param name="keyword">默认是CommunityName</param>
        /// <returns>Community下拉框的Json字符串</returns>
        [CLIUrlAuthorizeAttribute(Account = Authority.Index, PageId = PagesModel.Community_Management, Anonymity = Anonymous.Logined)]
        public string GetCommunitySelectList(string keyword, string communityName = "")
        {
            var expression = PredicateHelper.True<CommunityEntity>();
            expression = string.IsNullOrEmpty(communityName) || communityName.Trim() == string.Empty
                ? expression.And(o => true)
                : expression.And(o => o.BasicCommunity.Name.Contains(communityName));
            var communitySelectList = _communityBusiness.GetCommunitySelectList(UserInfo, expression, true);
            return JsonHelper.SerializeObject(communitySelectList);
        }

        [CLIUrlAuthorizeAttribute(Account = Authority.Index, PageId = PagesModel.Community_Management, Anonymity = Anonymous.Logined)]
        public string GetCommunitySelectListForSearch(string keyword, int communityId = -1, bool isActiveCommunity = true)
        {
            var expression = PredicateHelper.True<CommunityEntity>();
            if (communityId > 0)
                expression = expression.And(o => o.ID == communityId);
            var list = _communityBusiness.GetCommunitySelectList(UserInfo, expression, isActiveCommunity);
            return JsonHelper.SerializeObject(list);
        }

        /// <summary>
        /// 返回BasicCommunity下拉框的Json字符串
        /// </summary>
        /// <param name="keyword">默认是BasicCommunityName</param>
        /// <returns>BasicCommunity下拉框的Json字符串</returns>
        [CLIUrlAuthorizeAttribute(Account = Authority.Index, PageId = PagesModel.Community_Management, Anonymity = Anonymous.Logined)]
        public string GetBasicCommunitySelectList(string keyword, bool isActive = true, bool isBasicCommunityModel = false)
        {
            var communitySelectList = _communityBusiness.GetBasicCommunitySelectList(
                o => keyword == "-1" || o.Name.Contains(keyword), isActive, isBasicCommunityModel);
            return JsonHelper.SerializeObject(communitySelectList);
        }

        #endregion

        #region CommunityFeature   //由于profile功能需要调用Feature,所有Feature功能均不设置权限
        [CLIUrlAuthorizeAttribute(Account = Authority.Edit, PageId = PagesModel.Community_Management, Anonymity = Anonymous.Logined)]
        public ActionResult FeaturesCli(int communityId)
        {
            CommunityFeatureModel feature = _communityBusiness.GetFeatureModel(communityId);
            List<CpallsAssessmentModel> resList = new List<CpallsAssessmentModel>();

            List<CpallsAssessmentModel> listAssessments = _adeBusiness.GetAssessmentFeatureList(o => o.IsDeleted == false).ToList();

            List<CommunityAssessmentRelationsEntity> assignedList = _communityBusiness.GetAssignedAssessments(communityId).ToList();
            var assignedResList = new List<CommunityAssessmentRelationsEntity>();
            var localAssessments = LocalAssessment.ADE.ToList().Select(x => (LocalAssessment)x).ToList();
             
            foreach (CommunityAssessmentRelationsEntity assessmentRelationsEntity in assignedList)
            {
                if (assessmentRelationsEntity.AssessmentId < 0)
                {
                    foreach (LocalAssessment local in localAssessments)
                    {
                        if ((int)local == assessmentRelationsEntity.AssessmentId)
                        {
                            assessmentRelationsEntity.AssessmentName = local.ToDescription();
                            assignedResList.Add(assessmentRelationsEntity);
                            break;
                        }
                    }
                }
                else
                {
                    var firstFeature = listAssessments.FirstOrDefault(o => o.ID == assessmentRelationsEntity.AssessmentId);
                    if (firstFeature!= null)
                    {
                        assessmentRelationsEntity.AssessmentName = firstFeature.Name;
                        assignedResList.Add(assessmentRelationsEntity);
                    } 
                } 
            }
            ViewBag.localAssessments = localAssessments;
            ViewBag.listAssessments = listAssessments;
            ViewBag.assignedList = assignedResList;

            return View(feature);
        }

        [HttpPost]
        [CLIUrlAuthorizeAttribute(Account = Authority.Edit, PageId = PagesModel.Community_Management, Anonymity = Anonymous.Logined)]
        public string FeaturesCli(int comId, List<AssessmentFeatureClassLevel> listAssessment)
        {
            var response = new PostFormResponse();

            var result = _communityBusiness.InsertCommunityAssessmentRelations(UserInfo.ID, comId, listAssessment, false);

            response.Success = result.ResultType == OperationResultType.Success;
            response.Message = result.Message;
            return JsonHelper.SerializeObject(response);
        }

        [CLIUrlAuthorizeAttribute(Account = Authority.Edit, PageId = PagesModel.Community_Management, Anonymity = Anonymous.Logined)]
        public ActionResult Features(int communityId)
        {
            CommunityFeatureModel feature = _communityBusiness.GetFeatureModel(communityId);
            List<CpallsAssessmentModel> listAssessments = _adeBusiness.GetAssessmentFeatureList(o => o.IsDeleted == false).ToList();
            List<CommunityAssessmentRelationsEntity> assignedList = _communityBusiness.GetAssignedAssessments(communityId).ToList();
             List<CommunityAssessmentRelationsEntity> assignedResList = new List<CommunityAssessmentRelationsEntity>();
            var localAssessments = LocalAssessment.ADE.ToList().Select(x => (LocalAssessment)x).ToList();

            foreach (CommunityAssessmentRelationsEntity assessmentRelationsEntity in assignedList)
            {
                if (assessmentRelationsEntity.AssessmentId < 0)
                {
                    foreach (LocalAssessment local in localAssessments)
                    {
                        if ((int)local == assessmentRelationsEntity.AssessmentId)
                        {
                            assessmentRelationsEntity.AssessmentName = local.ToDescription();
                            assignedResList.Add(assessmentRelationsEntity);
                            break;
                        }
                    }
                }
                else
                {
                    var firstFeature = listAssessments.FirstOrDefault(o => o.ID == assessmentRelationsEntity.AssessmentId);
                    if (firstFeature != null)
                    {
                        assessmentRelationsEntity.AssessmentName = firstFeature.Name;
                        assignedResList.Add(assessmentRelationsEntity);
                    } 
                }


            }
            ViewBag.localAssessments = localAssessments;
            ViewBag.listAssessments = listAssessments;
            ViewBag.assignedList = assignedResList;
            return View(feature);
        }

        /// <summary>
        /// 外部用户处理Features
        /// </summary>
        /// <param name="feature"></param>
        /// <returns></returns>
        [HttpPost]
        [CLIUrlAuthorizeAttribute(Account = Authority.Edit, PageId = PagesModel.Community_Management, Anonymity = Anonymous.Logined)]
        public string Features(int comId, string[] assessmentStrs)
        {
            var response = new PostFormResponse();
            List<AssessmentSimpleModel> assessmentList = new List<AssessmentSimpleModel>();
            if (assessmentStrs != null)
            {
                foreach (string assessmentStr in assessmentStrs)
                {
                    //AssessmentSimpleModel
                    AssessmentSimpleModel model = new AssessmentSimpleModel();
                    string[] fields = assessmentStr.Split('^');
                    if (fields.Length == 2)
                    {
                        int assessmentId = 0;
                        if (int.TryParse(fields[0], out assessmentId))
                        {
                            model.AssessmentId = assessmentId;
                            model.Comment = fields[1];
                            assessmentList.Add(model);
                        }
                    }
                }
            }
            var result = _communityBusiness.InsertCommunityAssessmentRelations(UserInfo.ID, comId, assessmentList);
            if (result.ResultType == OperationResultType.Success)
            {
                SendEmailToCli(comId, assessmentList);
            }
            response.Success = result.ResultType == OperationResultType.Success;
            response.Message = result.Message;
            return JsonHelper.SerializeObject(response);
        }
        private void SendEmailToCli(int comId, List<AssessmentSimpleModel> features)
        {
            string adminFirstName = Administrator.FirstName;
            string communityFirstLastName = UserInfo.FirstName + " " + UserInfo.LastName;
            string feature = string.Empty;

            List<int> ids = features.Select(o => o.AssessmentId).ToList();
            List<CpallsAssessmentModel> list = _adeBusiness.GetAssessmentList(o => ids.Contains(o.ID)).ToList();

            CommunityEntity community = _communityBusiness.GetCommunity(comId);
            foreach (CpallsAssessmentModel assessmentModel in list)
            {
                feature += assessmentModel.Name + ", ";
            }
            if (feature.EndsWith(","))
                feature = feature.Substring(0, feature.Length - 1);

            string communtiyName = community.Name;
            string webSitUrl = string.Format("{0}", DomainHelper.MainSiteDomain.ToString());
            string to = SFConfig.CLIAdministratorEmail;
            if (string.IsNullOrEmpty(to))
                return;
            _communityBusiness.SendEmailToCli(adminFirstName, communityFirstLastName, feature, communtiyName, webSitUrl, to);
        }
        #endregion

        #region Public Function
        [CLIUrlAuthorizeAttribute(Account = Authority.Index, PageId = PagesModel.Community_Management, Anonymity = Anonymous.Verified)]
        [ValidateInput(false)]
        public string Search(string communityName = "", int communityId = -1, int fundingId = -1, int status = -1,
            string sort = "CommunityName", string order = "Asc", int first = 0, int count = 10)
        {
            var users = new List<int>() { 0, 1 };
            var total = 0;
            var expression = PredicateHelper.True<CommunityEntity>();
            if (communityId >= 1)
                expression = expression.And(o => o.ID == communityId);
            else if (communityName != null && communityName.Trim() != string.Empty)
                expression = expression.And(o => o.BasicCommunity.Name.Contains(communityName.Trim()));

            if (status >= 0)
                expression = expression.And(o => (int)o.Status == status);
            if (fundingId >= 0)
                expression = expression.And(o => o.FundingId == fundingId);
            var list = _communityBusiness.SearchCommunities(UserInfo, expression, sort, order, first, count, out total);

            var result = new { total = total, data = list };
            return JsonHelper.SerializeObject(result);
        }


        //由于profile功能用到此项，设置为不验证
        [CLIUrlAuthorizeAttribute(Account = Authority.Index, PageId = PagesModel.Community_Management, Anonymity = Anonymous.Logined)]
        public string GetCountiesByStateId(int stateId = 0)
        {
            var classList = _masterDataBusiness.GetCountySelectList(stateId).ToSelectList("County*", "");
            return JsonHelper.SerializeObject(classList);
        }
        #endregion

        #region Private Function


        /// <summary>
        /// defaultValue为String.Empty时，必填；否则，下拉框可选
        /// </summary>
        private IEnumerable<SelectListItem> ListToDDL(IEnumerable<object> list, string defaultValue = "")
        {
            return new SelectList(list, "ID", "Name").AddDefaultItem(ViewTextHelper.DefaultPleaseSelectText, defaultValue);
        }

        private void InitParameters()
        {

            ViewBag.TitleOptions = ListToDDL(_masterDataBusiness.GetTitleSelectList(1));
            ViewBag.FundingOptions = ListToDDL(_masterDataBusiness.GetFundingSelectList());
            ViewBag.BasicCommunityOptions = ListToDDL(_communityBusiness.GetBasicCommunitySelectList(o => o.Status == EntityStatus.Active));
            ViewBag.StatusOptions = EntityStatus.Active.ToSelectList();
            ViewBag.StateOptions = new SelectList(_masterDataBusiness.GetStateSelectList(), "ID", "Name").AddDefaultItem(ViewTextHelper.DefaultStateText, "");
            ViewBag.CountyOptions = new SelectList(_masterDataBusiness.GetCountySelectList(), "ID", "Name").AddDefaultItem(ViewTextHelper.DefaultCountyText, "");

            ViewBag.SecondaryTitleOptions = ListToDDL(_masterDataBusiness.GetTitleSelectList(2), "0");
        }

        private void InitViewParameters(CommunityEntity entity)
        {
            ViewBag.PrimaryTitle = entity.PrimaryTitle == null ? string.Empty : entity.PrimaryTitle.Name;
            if (entity.SecondaryTitle != null)
                ViewBag.SecondaryTitle = entity.SecondaryTitle.Name ?? string.Empty;
            ViewBag.Fundings = entity.Funding.Name;
            //BUP中，County值为空
            ViewBag.full = entity.City + ", " + (entity.County == null ? " " : entity.County.Name) + ", " + entity.State.Name + " " + entity.Zip;
            ViewBag.showNotes = true;
            switch (UserInfo.Role)
            {
                case Role.Coordinator:
                case Role.Mentor_coach:
                case Role.Video_coding_analyst:
                case Role.Statewide:
                case Role.Community:
                case Role.District_Community_Delegate:
                case Role.District_Community_Specialist:
                case Role.Community_Specialist_Delegate:
                case Role.TRS_Specialist:
                case Role.TRS_Specialist_Delegate:
                case Role.School_Specialist:
                case Role.School_Specialist_Delegate:
                case Role.Principal:
                case Role.Principal_Delegate:
                case Role.Auditor:
                    ViewBag.showNotes = false;
                    break;
            }
        }

        /// <summary>
        /// 控制按钮操作是否显示
        /// </summary>
        private void InitAccessOperation()
        {
            bool accessAdd = false;
            bool accessEdit = false;
            bool accessView = false;
            bool accessAssign = false;
            bool accessNote = false;
            if (UserInfo != null)
            {
                UserAuthorityModel userAuthority = new PermissionBusiness().GetUserAuthority(UserInfo, (int)PagesModel.Community_Management);

                if (userAuthority != null)
                {
                    if ((userAuthority.Authority & (int)Authority.Add) == (int)Authority.Add)
                    {
                        accessAdd = true;
                    }
                    if ((userAuthority.Authority & (int)Authority.Edit) == (int)Authority.Edit)
                    {
                        accessEdit = true;
                    }
                    if ((userAuthority.Authority & (int)Authority.View) == (int)Authority.View)
                    {
                        accessView = true;
                    }
                    if ((userAuthority.Authority & (int)Authority.Assign) == (int)Authority.Assign)
                    {
                        accessAssign = true;
                    }
                    if ((userAuthority.Authority & (int)Authority.Notes) == (int)Authority.Notes)
                    {
                        accessNote = true;
                    }
                }
            }
            ViewBag.accessAdd = accessAdd;
            ViewBag.accessEdit = accessEdit;
            ViewBag.accessView = accessView;
            ViewBag.accessAssign = accessAssign;
            ViewBag.accessNote = accessNote;
        }
        #endregion

        #region Profile
        [CLIUrlAuthorizeAttribute(Account = Authority.All, PageId = PagesModel.Administrative, Anonymity = Anonymous.Logined)]
        public ActionResult CommunityProfile(int id = 0)
        {
            var entity = new CommunityEntity();
            var list = new List<CommunityModel>();
            if (UserInfo.Role == Role.District_Community_Delegate || UserInfo.Role == Role.Community_Specialist_Delegate)
            {
                var user = _userBusiness.GetUser(UserInfo.CommunityUser.ParentId);
                list = user.UserCommunitySchools.Where(e => e.CommunityId > 0).Select(o => new CommunityModel()
                {
                    ID = o.CommunityId,
                    CommunityName = o.Community.Name
                }).ToList();
            }
            else
            {
                list = UserInfo.UserCommunitySchools.Where(e => e.CommunityId > 0).Select(o => new CommunityModel()
                {
                    ID = o.CommunityId,
                    CommunityName = o.Community.Name
                }).ToList();
            }

            ViewBag.CommunityList = list;

            if (id == 0)
            {
                entity = _communityBusiness.GetCommunity(list.Select(o => o.ID).FirstOrDefault());
            }
            else
            {
                entity = _communityBusiness.GetCommunity(id);
            }
            CommunityRoleEntity roleEntity = _communityBusiness.GetCommunityRoleEntity(UserInfo.Role);
            ViewBag.Role = JsonHelper.SerializeObject(roleEntity);
            InitParameters();
            ViewBag.communityName = entity.Name;
            if (entity.MouStatus == false)
            {
                entity.MouDocument = null;
            }
            ViewBag.FeatureQueryString = UserInfo.IsCLIUser ? "FeaturesCli" : "Features";

            List<CpallsAssessmentModel> listAssessments = _adeBusiness.GetAssessmentList(o => o.IsDeleted == false).ToList();
            var localAssessments = LocalAssessment.ADE.ToList().Select(x => (LocalAssessment)x).ToList();
            ViewBag.localAssessments = localAssessments;
            ViewBag.listAssessments = listAssessments;
            int[] assignedIds = _communityBusiness.GetAssignedApprovedAssessmentIds(entity.ID);
            ViewBag.assignedIds = assignedIds;


            return View(entity);
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        [CLIUrlAuthorizeAttribute(Account = Authority.All, PageId = PagesModel.Administrative, Anonymity = Anonymous.Logined)]
        public string ProfileUpdate(CommunityEntity entity)//, int[] chkAssessment
        {
            var response = new PostFormResponse();

            OperationResult result = _communityBusiness.UpdateCommunity(entity, UserInfo.Role);
            if (result.ResultType == OperationResultType.Success)
            {
                //if (UserInfo.IsCLIUser)
                //    _communityBusiness.InsertCommunityAssessmentRelations(UserInfo.ID, entity.ID, chkAssessment, false);
                //else
                //{
                //    _communityBusiness.InsertCommunityAssessmentRelations(UserInfo.ID, entity.ID, chkAssessment, true);
                //}
            }
            response.Success = result.ResultType == OperationResultType.Success;
            response.Message = result.Message;

            return JsonHelper.SerializeObject(response);
        }

        #endregion

        #region Community School Relations
        [CLIUrlAuthorizeAttribute(Account = Authority.Assign, PageId = PagesModel.Community_Management, Anonymity = Anonymous.Verified)]
        public ActionResult AssignSchools(int comId)
        {
            var entity = _communityBusiness.GetCommunity(comId);
            return View(entity);
        }
        [CLIUrlAuthorizeAttribute(Account = Authority.Assign, PageId = PagesModel.Community_Management, Anonymity = Anonymous.Verified)]
        public string SearchUnassigedSchools(int comId, string name = "", string sort = "SchoolName", string order = "Asc", int first = 0, int count = 10)
        {
            int total = 0;
            int[] assignedIds = _schoolBusiness.GetAssignedSchoolIds(comId);
            var list = _schoolBusiness.SearchSchools(UserInfo,
                o => (o.BasicSchool.Name.Contains(name) || name.Trim() == "") && (!assignedIds.Contains(o.ID) && o.Status == SchoolStatus.Active), sort, order, first, count, out total);
            var result = new { total = total, data = list };
            return JsonHelper.SerializeObject(result);
        }
        [CLIUrlAuthorizeAttribute(Account = Authority.Assign, PageId = PagesModel.Community_Management, Anonymity = Anonymous.Verified)]
        public string InsertCommunitySchoolRelations(int comId, int[] schoolIds)
        {

            var response = new PostFormResponse();
            OperationResult result = new OperationResult(OperationResultType.Success);
            result = _communityBusiness.InsertCommunitySchoolRelations(UserInfo.ID, comId, schoolIds);
            response.Success = result.ResultType == OperationResultType.Success;
            response.Data = "";
            response.Message = result.Message;
            return JsonHelper.SerializeObject(response);
        }
        [CLIUrlAuthorizeAttribute(Account = Authority.Assign, PageId = PagesModel.Community_Management, Anonymity = Anonymous.Verified)]
        public string DeleteCommunitySchoolRelations(int comId, int[] schoolIds)
        {
            var response = new PostFormResponse();
            OperationResult result = new OperationResult(OperationResultType.Success);
            result = _schoolBusiness.DeleteCommunitySchoolRelations(comId, schoolIds);
            response.Success = result.ResultType == OperationResultType.Success;
            response.Data = "";
            response.Message = result.Message;
            return JsonHelper.SerializeObject(response);
        }
        [CLIUrlAuthorizeAttribute(Account = Authority.Assign, PageId = PagesModel.Community_Management, Anonymity = Anonymous.Verified)]
        public string SearchAssignedSchools(long comId = 0, string name = "", string sort = "SchoolName", string order = "Asc", int first = 0, int count = 10)
        {
            int total = 0;
            var list = _schoolBusiness.GetAssignedSchools(
                o => (o.CommunityId == comId && (o.School.BasicSchool.Name.Contains(name) || name.Trim() == "")), sort, order, first, count, out total);
            var result = new { total = total, data = list };
            return JsonHelper.SerializeObject(result);
        }

        #endregion

        #region Community Notes

        [CLIUrlAuthorizeAttribute(Account = Authority.Notes, PageId = PagesModel.Community_Management, Anonymity = Anonymous.Verified)]
        public ViewResult ViewNotes(int comId)
        {
            CommunityEntity community = _communityBusiness.GetCommunity(comId);

            ViewBag.CommunityName = community.Name;
            ViewBag.LogoUrl = community.LogoUrl;
             
            var expression = PredicateHelper.True<CommunityNotesEntity>();
            if (comId >= 1)
                expression = expression.And(n => n.CommunityId == comId);
            var notes = _communityBusiness.GetCommunityNotes(expression);
            notes = InitList(notes);
            notes = notes.OrderBy(o => o.Status).ToList();
            ViewBag.CommunityNotes = notes;
            ViewBag.CommunityName = community.Name;
            ViewBag.CommunityId = community.ID;

            return View("ViewNotes");

        }

        [CLIUrlAuthorizeAttribute(Account = Authority.Notes, PageId = PagesModel.Community_Management, Anonymity = Anonymous.Verified)]
        public ViewResult EditNotes(int comId)
        {
            CommunityEntity community = _communityBusiness.GetCommunity(comId);
            ViewBag.CommunityName = community.Name;
            ViewBag.StatusOptions = EntityStatus.Active.ToSelectList(true);
            var notes = _communityBusiness.GetCommunityNotes(comId);
            return View("EditNotes", notes);
        }
        [CLIUrlAuthorizeAttribute(Account = Authority.Notes, PageId = PagesModel.Community_Management, Anonymity = Anonymous.Verified)]
        public ViewResult NewNote(int comId)
        {
            CommunityEntity community = _communityBusiness.GetCommunity(comId);
            ViewBag.CommunityName = community.Name;
            ViewBag.StatusOptions = EntityStatus.Active.ToSelectList(true);
            CommunityNotesEntity entity = new CommunityNotesEntity();
            entity.CommunityId = comId;
            entity.Community = community;
            return View(entity);
        }
        [CLIUrlAuthorizeAttribute(Account = Authority.Notes, PageId = PagesModel.Community_Management, Anonymity = Anonymous.Verified)]
        public ViewResult EditNote(int noteId,int communityId)
        {

            var note = _communityBusiness.GetNote(noteId);
            if (note != null && note.CommunityId == communityId)
            {
                CommunityEntity community = _communityBusiness.GetCommunity(note.CommunityId);
                ViewBag.CommunityName = community.Name;
                ViewBag.StatusOptions = EntityStatus.Active.ToSelectList(true); 
            }
            else
            {

                note = new CommunityNotesEntity();
                ViewBag.CommunityName = "";
                ViewBag.StatusOptions = EntityStatus.Active.ToSelectList(true); 
            }
            return View(note);
        }
         
        [HttpPost]
        [ValidateInput(false)]
        [ValidateAntiForgeryToken]
        [CLIUrlAuthorizeAttribute(Account = Authority.Notes, PageId = PagesModel.Community_Management, Anonymity = Anonymous.Verified)]
        public string EditNotes(CommunityNotesEntity entity,bool confirm)
        {
            var response = new PostFormResponse();

            if (!confirm)
            { 
                var expression = PredicateHelper.True<CommunityNotesEntity>();
                if (entity.CommunityId > 0)
                    expression = expression.And(n => n.CommunityId == entity.CommunityId);
                var notes = _communityBusiness.GetCommunityNotes(expression);
                notes = InitList(notes);
                foreach (var note in notes)
                {
                    if (note.Status == CommunityNoteStatus.Active && note.ID !=entity.ID)
                    {
                        if ((entity.StartOn <= note.StartOn && entity.StopOn >= note.StartOn)
                           || (entity.StartOn >= note.StartOn && entity.StartOn <= note.StopOn) )
                        {
                            response.Success = true;
                            response.Message = "One active note already exists, replace it?";
                            response.Data = new { type = "continue" };
                            return JsonConvert.SerializeObject(response);
                        }
                    }
                   
                }
            }
            OperationResult result = _communityBusiness.EditNotes(UserInfo, entity);
            response.Success = result.ResultType == OperationResultType.Success;
            response.Message = result.Message;
            return JsonConvert.SerializeObject(response);
        }
        [HttpPost]
        [ValidateInput(false)]
        [ValidateAntiForgeryToken]
        [CLIUrlAuthorizeAttribute(Account = Authority.Notes, PageId = PagesModel.Community_Management, Anonymity = Anonymous.Verified)]
        public string NewNote(CommunityNotesEntity entity, bool confirm = false)
        {
            var response = new PostFormResponse();
            OperationResult result = new OperationResult(OperationResultType.Success);

            if (!confirm)
            {
                var expression = PredicateHelper.True<CommunityNotesEntity>();
                if (entity.CommunityId > 0)
                    expression = expression.And(n => n.CommunityId == entity.CommunityId);
                var notes = _communityBusiness.GetCommunityNotes(expression);
                notes = InitList(notes);
                foreach (var note in notes)
                {
                    if ((entity.StartOn <= note.StartOn && entity.StopOn >= note.StartOn && note.Status == CommunityNoteStatus.Active)
                        || ((entity.StartOn >= note.StartOn && entity.StartOn <= note.StopOn && note.Status == CommunityNoteStatus.Active))
                        )
                    {
                        response.Success = true;
                        response.Message = "One active note already exists, replace it?";
                        response.Data = new
                        {
                            type = "continue"
                        };
                        return JsonConvert.SerializeObject(response);
                    }
                }
            }

            result = _communityBusiness.NewNote(UserInfo, entity);
            response.Success = result.ResultType == OperationResultType.Success;
            response.Message = result.Message;
            return JsonConvert.SerializeObject(response);
        }
        [CLIUrlAuthorizeAttribute(Account = Authority.Notes, PageId = PagesModel.Community_Management, Anonymity = Anonymous.Logined)]
        public ActionResult CommunityNotes()
        {
            ViewBag.CommunityNotes = _communityBusiness.GetCommunityNotes(UserInfo);
            return View();
        }

        //在static/Uploader/CKUploader中调用Url
        public void CKUploadImage(int funcNum, string fileName, string errorMsg)
        {
            //domain
            string staticDomain = SFConfig.StaticDomain;
            staticDomain = staticDomain.Remove(staticDomain.LastIndexOf('/'));
            string[] sArray = staticDomain.Split('.');
            int length = sArray.Length;
            string domain = sArray[length - 2] + "." + sArray[length - 1];

            //fileUrl
            string fileUrl = "";
            string[] fileNameArr = fileName.Split('.');
            string[] exts = { "jpg", "JPG", "png", "PNG", "gif", "GIF", "jpeg", "JPEG", "bmp", "BMP" };
            Guid g = Guid.Empty;
            if (fileNameArr.Length == 2)
            {
                if (Guid.TryParse(fileNameArr[0], out g)
                    && exts.Contains(fileNameArr[1]))
                    fileUrl = staticDomain + "/Upload/ck_images/" + fileName;
            }

            //errorMsg
            if (errorMsg.Contains("<script>") || errorMsg.Contains("</script>"))
            {
                fileUrl = "";
                errorMsg = null;
            }

            string script = @"<script type='text/javascript'>document.domain = '" + domain
                + "';window.top.CKEDITOR.tools.callFunction({0}, '{1}', '{2}');</script>";
            Response.Write(string.Format(script, funcNum, fileUrl, errorMsg));
        }

        private List<CommunityNotesModel> InitList( List<CommunityNotesModel> notes)
        {
            DateTime todayStart = DateTime.Now.Date;
            DateTime todayEnd = DateTime.Now.AddDays(1).Date.AddSeconds(-1);
            CommunityNotesModel activeNote = null;
            foreach (var note in notes)
            {
                if ((todayStart <= note.StartOn && todayEnd >= note.StartOn && note.Status == CommunityNoteStatus.Active)
                    || ((todayStart >= note.StartOn && todayStart <= note.StopOn && note.Status == CommunityNoteStatus.Active))
                    )
                  {
                     if (activeNote != null)
                    {
                        note.Status = CommunityNoteStatus.Replaced;
                    }
                    else
                    {
                        activeNote = note;
                    }
                }
                else
                {
                    note.Status = CommunityNoteStatus.Inactive;
                }
            }
            return notes;
        }

        #endregion
    }
}