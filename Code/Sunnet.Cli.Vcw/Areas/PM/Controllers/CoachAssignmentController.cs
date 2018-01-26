using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
/**************************************************************************
 * Developer: 		Joe
 * Computer:		JOE-PC
 * Domain:			Joe-pc
 * CreatedOn:		2014/10/23 12:15:10
 * Description:		Please input class summary
 * Version History:	Created,2014/10/23 12:15:10
 * 
 * 
 **************************************************************************/
using Sunnet.Cli.Vcw.Controllers;
using Sunnet.Cli.UIBase.Filters;
using Sunnet.Framework.Permission;
using Sunnet.Cli.UIBase.Models;
using Sunnet.Cli.Business.Vcw;
using Sunnet.Cli.Business.Users;
using Sunnet.Cli.Business.Vcw.Models;
using System.Linq.Expressions;
using Sunnet.Framework.Helpers;
using Sunnet.Cli.Core.Vcw.Entities;
using Sunnet.Cli.Core.Vcw.Enums;
using Sunnet.Cli.Vcw.Models;
using Sunnet.Framework.Extensions;
using Sunnet.Cli.Business.Communities.Models;
using Sunnet.Cli.Business.Communities;
using Sunnet.Framework.Core.Tool;
using Sunnet.Cli.Business.Common;

namespace Sunnet.Cli.Vcw.Areas.PM.Controllers
{
    public class CoachAssignmentController : BaseController
    {
        VcwBusiness _vcwBusiness;
        UserBusiness _userBusiness;
        VCW_MasterDataBusiness _vcwMasterDataBusiness;

        public CoachAssignmentController()
        {
            _vcwBusiness = new VcwBusiness(VcwUnitWorkContext);
            _userBusiness = new UserBusiness();
            _vcwMasterDataBusiness = new VCW_MasterDataBusiness(VcwUnitWorkContext);
        }

        //
        // GET: /PM/CoacherAssignment/
        [CLIUrlAuthorizeAttribute(Account = Authority.Index, PageId = PagesModel.PMCoaches, Anonymity = Anonymous.Verified)]
        public ActionResult Index()
        {
            List<object> Dropdown = GetDropDownItems.GetItemsByPM(UserInfo.ID);
            ViewBag.Coaches = Dropdown[0];
            ViewBag.Communities = Dropdown[1];
            return View();
        }

        [CLIUrlAuthorizeAttribute(Account = Authority.Index, PageId = PagesModel.PMCoaches, Anonymity = Anonymous.Verified)]
        public string SearchAssignment(int community, int coach, int status,
            string sort = "ID", string order = "DESC", int first = 0, int count = 10)
        {
            var total = 0;
            var expression = PredicateHelper.True<AssignmentEntity>();

            if (status > 0)
                expression = expression.And(o => o.Status == (AssignmentStatus)status);
            if (community > 0)
            {
                List<int> user_Ids = _userBusiness.GetAssignedUsersByCommunity(community).Select(a => a.UserId).ToList();
                expression = expression.And(r => user_Ids.Contains(r.ReceiveUserId));
            }
            if (coach > 0)
                expression = expression.And(o => o.ReceiveUserId == coach);

            List<int> Coache_id = _userBusiness.GetCoachByPM(UserInfo.ID).Select(a => a.ID).ToList();

            expression = expression.And(o =>
                         o.AssignmentType == AssignmentTypeEnum.CoachAssignment
                         && Coache_id.Contains(o.ReceiveUserId) && o.IsDelete == false);

            List<AssignmentListModel> list = new List<AssignmentListModel>();
            list = _vcwBusiness.GetCoachAssignments(expression, sort, order, first, count, out total);

            if (list.Count > 0)
            {
                List<int> senderIds = list.Select(s => s.SendUserId).Distinct().ToList();
                List<UsernameModel> userNames = _userBusiness.GetUsernames(senderIds);

                List<SelectItemModel> UploadTypes = _vcwMasterDataBusiness.GetActiveUploadTypes();
                List<SelectItemModel> Strategies = _vcwMasterDataBusiness.GetActiveAssignmentCoachingStrategy_Datas();
                foreach (AssignmentListModel item in list)
                {
                    UsernameModel username = userNames.Find(r => r.ID == item.SendUserId);
                    if (username != null)
                    {
                        item.SendUserName = username.Firstname + " " + username.Lastname;
                    }
                    item.UploadTypes = UploadTypes.Where(r => item.UploadTypeIds.Contains(r.ID));
                    item.Strategies = Strategies.Where(r => item.StrategyIds.Contains(r.ID));
                }
            }

            var result = new { total = total, data = list };
            return JsonHelper.SerializeObject(result);
        }


        [CLIUrlAuthorizeAttribute(Account = Authority.Index, PageId = PagesModel.PMCoaches, Anonymity = Anonymous.Verified)]
        public ActionResult View(int id)
        {
            AssignmentModel entity = _vcwBusiness.GetCoachAssignment(id);
            CoachesListModel coach = _userBusiness.GetCoachInfoById(entity.ReceiveUserId);
            if (coach != null)
            {
                ViewBag.Coach = coach;
            }
            return View(entity);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [CLIUrlAuthorizeAttribute(Account = Authority.Index, PageId = PagesModel.PMCoaches, Anonymity = Anonymous.Verified)]
        public string EditAssignment(AssignmentModel model, string uploadfiles)
        {
            AssignmentEntity Assignment = _vcwBusiness.GetAssignment(model.ID);
            Assignment.FeedbackText = model.FeedbackText;
            Assignment.UpdatedBy = UserInfo.ID;
            Assignment.UpdatedOn = DateTime.Now;
            if (!string.IsNullOrEmpty(uploadfiles))
            {
                string[] file = uploadfiles.Split('|');
                Assignment.FeedbackFileName = file[0];
                Assignment.FeedbackFilePath = file[1];
            }

            var response = new PostFormResponse();
            OperationResult result = _vcwBusiness.UpdateAssignment(Assignment);
            response.Success = result.ResultType == OperationResultType.Success;
            response.Message = result.Message;
            return JsonHelper.SerializeObject(response);
        }

        [CLIUrlAuthorizeAttribute(Account = Authority.Index, PageId = PagesModel.PMCoaches, Anonymity = Anonymous.Verified)]
        public ActionResult ViewAssignmentFiles(int id)
        {
            ViewBag.AssignmentId = id;
            int ReceiveUserId = _vcwBusiness.GetAssignmentReceive(id);

            CoachesListModel coach = _userBusiness.GetCoachInfoById(ReceiveUserId);
            if (coach != null)
            {
                ViewBag.Coach = coach;
            }
            return View();
        }

        [CLIUrlAuthorizeAttribute(Account = Authority.Index, PageId = PagesModel.PMCoaches, Anonymity = Anonymous.Verified)]
        public string SearchAssignmentFiles(int assignmentId,
            string sort = "ID", string order = "Asc", int first = 0, int count = 10)
        {
            int total = 0;

            List<FileListModel> list = new List<FileListModel>();

            Expression<Func<Vcw_FileEntity, bool>> fileContition = PredicateHelper.True<Vcw_FileEntity>();
            fileContition = fileContition.And(r => r.AssignmentId == assignmentId);
            fileContition = fileContition.And(r => r.IsDelete == false);
            fileContition = fileContition.And(r => r.VideoType == FileTypeEnum.CoachAssignment);

            list = _vcwBusiness.GetSummaryList(fileContition, sort, order, first, count, out total);

            if (list.Count > 0)
            {
                IEnumerable<SelectItemModel> Contents = _vcwMasterDataBusiness.GetActiveVideo_Content_Datas();
                List<SelectItemModel> Strategies = _vcwMasterDataBusiness.GetActiveCoachingStrategy_Datas();
                foreach (FileListModel item in list)
                {
                    item.Contents = Contents.Where(r => item.ContentIds.Contains(r.ID));
                    item.Strategies = Strategies.Where(r => item.StrategyIds.Contains(r.ID));
                }
            }

            var result = new { total = total, data = list };
            return JsonHelper.SerializeObject(result);
        }


        [CLIUrlAuthorizeAttribute(Account = Authority.Index, PageId = PagesModel.PMCoaches, Anonymity = Anonymous.Verified)]
        public ActionResult UploadVideo(int AssignmentID)
        {
            int ReceiveUserId = _vcwBusiness.GetAssignmentReceive(AssignmentID);
            CoachFileModel model = new CoachFileModel();
            model.AssignmentId = AssignmentID;
            model.OwnerId = ReceiveUserId;
            ViewBag.Languages = _vcwMasterDataBusiness.GetActiveVideo_Language_Datas();
            ViewBag.Contents = _vcwMasterDataBusiness.GetActiveVideo_Content_Datas();
            ViewBag.Contexts = _vcwMasterDataBusiness.GetActiveContext_Datas();
            ViewBag.Strategies = _vcwMasterDataBusiness.GetActiveCoachingStrategy_Datas();
            return View(model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [CLIUrlAuthorizeAttribute(Account = Authority.Index, PageId = PagesModel.PMCoaches, Anonymity = Anonymous.Verified)]
        public string UploadVideo(CoachFileModel model, string uploadfiles, int[] language, int[] Content, int[] Context, int[] Strategy)
        {
            Vcw_FileEntity entity = new Vcw_FileEntity();
            var response = new PostFormResponse();
            if (ModelState.IsValid)
            {
                if (!string.IsNullOrEmpty(uploadfiles))
                {
                    string[] files = uploadfiles.Split('|');
                    if (files.Length == 2)
                    {
                        model.FileName = files[0];
                        model.FilePath = files[1];
                        OperationResult result = _vcwBusiness.InsertFileEntity(model, language, Content, Context, Strategy, UploadUserTypeEnum.PM, UserInfo);
                        response.Success = result.ResultType == OperationResultType.Success;
                        response.Message = result.Message;
                        if (response.Success)//更新Assignment的Status
                        {
                            _vcwBusiness.ChangeStatus(ChangeStatusEnum.AddFile, model.AssignmentId);
                        }
                    }
                }
            }
            response.ModelState = ModelState;
            return JsonHelper.SerializeObject(response);
        }

        [HttpPost]
        [CLIUrlAuthorizeAttribute(Account = Authority.Index, PageId = PagesModel.PMCoaches, Anonymity = Anonymous.Verified)]
        public string Delete(int[] video_select, int AssignmentId)
        {
            var response = new PostFormResponse();
            if (video_select != null)
            {
                List<int> deleteids = video_select.ToList();
                OperationResult result = _vcwBusiness.DeleteFile(deleteids);
                response.Success = result.ResultType == OperationResultType.Success;
                response.Message = result.Message;
                if (response.Success)//更新Assignment的Status
                {
                    _vcwBusiness.ChangeStatus(ChangeStatusEnum.DeleteFile, AssignmentId);
                }
                return JsonHelper.SerializeObject(response);
            }
            else
            {
                response.Success = false;
                return JsonHelper.SerializeObject(response);
            }
        }


        [HttpPost]
        [CLIUrlAuthorizeAttribute(Account = Authority.Index, PageId = PagesModel.PMCoaches, Anonymity = Anonymous.Verified)]
        public string DeleteAssignment(int[] assignment_select)
        {
            var response = new PostFormResponse();
            if (assignment_select != null)
            {
                List<int> deleteids = assignment_select.ToList();
                OperationResult result = _vcwBusiness.DeleteAssignment(deleteids);
                response.Success = result.ResultType == OperationResultType.Success;
                response.Message = result.Message;
                return JsonHelper.SerializeObject(response);
            }
            else
            {
                response.Success = false;
                return JsonHelper.SerializeObject(response);
            }
        }


        [CLIUrlAuthorizeAttribute(Account = Authority.All, PageId = PagesModel.PM, Anonymity = Anonymous.Verified)]
        public ActionResult ViewFile(int id, string redirect = "")
        {
            ViewBag.Redirect = redirect;
            CoachFileModel model = _vcwBusiness.GetCoachFileModelByPM(id);
            ViewBag.IsPM = model.UploadUserId == UserInfo.ID;//用于判断是否为自身上传，是否可编辑基本信息
            if (model.UploadUserId == UserInfo.ID)
            {
                ViewBag.Languages = _vcwMasterDataBusiness.GetActiveVideo_Language_Datas();
                ViewBag.Contents = _vcwMasterDataBusiness.GetActiveVideo_Content_Datas();
                ViewBag.Contexts = _vcwMasterDataBusiness.GetActiveContext_Datas();
            }
            ViewBag.SelectionList = _vcwMasterDataBusiness.GetActiveVideo_SelectionList_Datas();
            ViewBag.Strategies = _vcwMasterDataBusiness.GetActiveCoachingStrategy_Datas();
            CoachesListModel coach = _userBusiness.GetCoachInfoById(model.OwnerId);
            if (coach != null)
            {
                ViewBag.Coach = coach;
            }
            return View(model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [CLIUrlAuthorizeAttribute(Account = Authority.All, PageId = PagesModel.PM, Anonymity = Anonymous.Verified)]
        public string EditFile(CoachFileModel model, int[] language, int[] screening, int[] selectionlist,
            string uploadfiles, int[] Content, int[] Context, int[] Strategy, bool IsPM = false)
        {
            var response = new PostFormResponse();
            Vcw_FileEntity entity = _vcwBusiness.GetFileEntity(model.ID);
            if (entity != null)
            {
                if (entity.FileSelections != null)
                {
                    _vcwBusiness.DeleteFileSelection(entity.FileSelections.ToList(), false);
                }

                entity.UpdatedOn = DateTime.Now;
                entity.UpdatedBy = UserInfo.ID;

                if (IsPM)  //PM查看自己上传的文件时可编辑以下选项
                {
                    if (entity.FileContents != null)
                    {
                        _vcwBusiness.DeleteFileContent(entity.FileContents.ToList(), false);
                    }
                    entity.IdentifyFileName = model.IdentifyFileName;
                    entity.DateRecorded = model.DateRecorded.Value;
                    entity.LanguageId = language == null ? 0 : language[0];
                    entity.ContextId = Context == null ? 0 : Context[0];
                    entity.ContextOther = model.ContextOther;
                    entity.ContentOther = model.ContentOther;
                    entity.StrategyOther = model.StrategyOther;
                    entity.Objectives = model.Objectives;
                    entity.Effectiveness = model.Effectiveness;

                    if (!string.IsNullOrEmpty(uploadfiles))
                    {
                        string[] files = uploadfiles.Split('|');
                        if (files.Length == 2)
                        {
                            entity.FileName = files[0];
                            entity.FilePath = files[1];
                        }
                    }
                    if (Content != null)
                    {
                        List<FileContentEntity> fileContents = new List<FileContentEntity>();
                        foreach (int item_content in Content)
                        {
                            FileContentEntity FileContent = new FileContentEntity();
                            FileContent.ContentId = item_content;
                            fileContents.Add(FileContent);
                        }
                        entity.FileContents = fileContents;
                    }

                    //删除FileStrategies关系表
                    if (entity.FileStrategies != null)
                    {
                        _vcwBusiness.DeleteFileStrategies(entity.FileStrategies.ToList(), false);
                    }
                    //添加FileStrategies关系表
                    if (Strategy != null)
                    {
                        if (Strategy.Length > 0)
                        {
                            foreach (int item in Strategy)
                            {
                                FileStrategyEntity FileStrategy = new FileStrategyEntity();
                                FileStrategy.StrategyId = item;
                                FileStrategy.UpdatedOn = DateTime.Now;
                                entity.FileStrategies.Add(FileStrategy);
                            }
                        }
                    }
                }

                if (screening != null)
                    //选中Accept，则将Status改为Completed，否则改为Rejected
                    entity.Status = screening[0] == 1 ? FileStatus.Completed : FileStatus.Rejected;
                else
                    entity.Status = FileStatus.Submitted;

                if (selectionlist != null)
                {
                    foreach (int item in selectionlist)
                    {
                        FileSelectionEntity FileSelection = new FileSelectionEntity();
                        FileSelection.SelectionId = item;
                        entity.FileSelections.Add(FileSelection);
                    }
                }
                OperationResult result = _vcwBusiness.UpdateFile(entity);
                response.Success = result.ResultType == OperationResultType.Success;
                response.Message = result.Message;
                if (response.Success)//更新Assignment的Status
                {
                    _vcwBusiness.ChangeStatus(ChangeStatusEnum.UpdateFile, entity.AssignmentId.Value, entity.Status);
                }
                return JsonHelper.SerializeObject(response);
            }
            else
            {
                response.Success = false;
                return JsonHelper.SerializeObject(response);
            }
        }
    }
}