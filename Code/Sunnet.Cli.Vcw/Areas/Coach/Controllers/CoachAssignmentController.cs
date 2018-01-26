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
using Sunnet.Cli.Business.Vcw.Models;
using Sunnet.Cli.Business.Vcw;
using Sunnet.Framework.Helpers;
using Sunnet.Cli.Core.Vcw.Entities;
using Sunnet.Cli.Core.Vcw.Enums;
using Sunnet.Cli.UIBase.Models;
using System.Linq.Expressions;
using Sunnet.Cli.Business.Common;
using Sunnet.Framework.Core.Tool;
using Sunnet.Cli.UIBase.Filters;
using Sunnet.Framework.Permission;
using Sunnet.Cli.Business.Users;
using Sunnet.Framework.Extensions;

namespace Sunnet.Cli.Vcw.Areas.Coach.Controllers
{
    public class CoachAssignmentController : BaseController
    {
        VcwBusiness _vcwBusiness;
        UserBusiness _userBusiness;
        VCW_MasterDataBusiness _vcwMasterDataBusiness;
        public CoachAssignmentController()
        {
            _userBusiness = new UserBusiness();
            _vcwBusiness = new VcwBusiness(VcwUnitWorkContext);
            _vcwMasterDataBusiness = new VCW_MasterDataBusiness(VcwUnitWorkContext);
        }


        //
        // GET: /Coach/CoachAssignment/
        [CLIUrlAuthorizeAttribute(Account = Authority.Index, PageId = PagesModel.CoachAssignment, Anonymity = Anonymous.Verified)]
        public ActionResult Index()
        {
            return View();
        }

        [CLIUrlAuthorizeAttribute(Account = Authority.Index, PageId = PagesModel.CoachAssignment, Anonymity = Anonymous.Verified)]
        public string SearchAssignment(
            string sort = "ID", string order = "Asc", int first = 0, int count = 10)
        {
            var total = 0;
            var expression = PredicateHelper.True<AssignmentEntity>();

            expression = expression.And(o => o.ReceiveUserId == (UserInfo.ID) &&
                o.AssignmentType == AssignmentTypeEnum.CoachAssignment && o.IsDelete == false);

            var list = _vcwBusiness.GetCoachAssignmentsByCoach(expression, sort, order, first, count, out total);

            if (list.Count > 0)
            {
                List<int> senderIds = list.Select(s => s.SendUserId).Distinct().ToList();
                List<UsernameModel> userNames = _userBusiness.GetUsernames(senderIds);
                List<SelectItemModel> UploadTypes = _vcwMasterDataBusiness.GetActiveUploadTypes();
                List<SelectItemModel> Strategies = _vcwMasterDataBusiness.GetActiveCoachingStrategy_Datas();
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

        [CLIUrlAuthorizeAttribute(Account = Authority.Index, PageId = PagesModel.CoachAssignment, Anonymity = Anonymous.Verified)]
        public ActionResult View(int id)
        {
            AssignmentModel entity = _vcwBusiness.GetCoachAssignment(id);
            if (!entity.IsVisited)
            {
                VcwUnitWorkContext.DbContext.Database.ExecuteSqlCommand("update Assignments set IsVisited=1 where ID=" + entity.ID + "");
            }
            return View(entity);
        }

        [CLIUrlAuthorizeAttribute(Account = Authority.Index, PageId = PagesModel.CoachAssignment, Anonymity = Anonymous.Verified)]
        public ActionResult ViewAssignmentFiles(int id)
        {
            ViewBag.AssignmentId = id;
            VcwUnitWorkContext.DbContext.Database.ExecuteSqlCommand("update Assignments set IsVisited=1 where ID=" + id + "");
            return View();
        }

        [CLIUrlAuthorizeAttribute(Account = Authority.Index, PageId = PagesModel.CoachAssignment, Anonymity = Anonymous.Verified)]
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
                IEnumerable<SelectItemModel> Strategies = _vcwMasterDataBusiness.GetActiveCoachingStrategy_Datas();

                foreach (FileListModel item in list)
                {
                    item.Strategies = Strategies.Where(r => item.StrategyIds.Contains(r.ID));
                    item.Contents = Contents.Where(r => item.ContentIds.Contains(r.ID));
                }
            }

            var result = new { total = total, data = list };
            return JsonHelper.SerializeObject(result);
        }

        [CLIUrlAuthorizeAttribute(Account = Authority.Index, PageId = PagesModel.CoachAssignment, Anonymity = Anonymous.Verified)]
        public ActionResult UploadVideo(int AssignmentID)
        {
            CoachFileModel model = new CoachFileModel();
            model.AssignmentId = AssignmentID;
            ViewBag.Languages = _vcwMasterDataBusiness.GetActiveVideo_Language_Datas();
            ViewBag.Contents = _vcwMasterDataBusiness.GetActiveVideo_Content_Datas();
            ViewBag.Contexts = _vcwMasterDataBusiness.GetActiveContext_Datas();
            ViewBag.Strategies = _vcwMasterDataBusiness.GetActiveCoachingStrategy_Datas();
            return View(model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [CLIUrlAuthorizeAttribute(Account = Authority.Index, PageId = PagesModel.CoachAssignment, Anonymity = Anonymous.Verified)]
        public string UploadVideo(CoachFileModel model, int[] language, int[] Content,
            int[] Context, int[] Strategy, string uploadfiles)
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
                    }
                }
                if (string.IsNullOrEmpty(model.FileName))
                {
                    response.Success = false;
                    return JsonHelper.SerializeObject(response);
                }
                model.OwnerId = UserInfo.ID;
                OperationResult result = _vcwBusiness.InsertFileEntity(model, language, Content, Context, Strategy, UploadUserTypeEnum.Coach, UserInfo);
                response.Success = result.ResultType == OperationResultType.Success;
                response.Message = result.Message;
                if (response.Success)//更新Assignment的Status
                {
                    _vcwBusiness.ChangeStatus(ChangeStatusEnum.AddFile, model.AssignmentId);
                }
            }
            response.ModelState = ModelState;
            return JsonHelper.SerializeObject(response);
        }


        [CLIUrlAuthorizeAttribute(Account = Authority.All, PageId = PagesModel.Coach, Anonymity = Anonymous.Verified)]
        public ActionResult EditFile(int id, string redirect = "")
        {
            ViewBag.Redirect = redirect;
            CoachFileModel model = _vcwBusiness.GetCoachGeneralFileModel(id);
            ViewBag.Languages = _vcwMasterDataBusiness.GetActiveVideo_Language_Datas();
            ViewBag.Contents = _vcwMasterDataBusiness.GetActiveVideo_Content_Datas();
            ViewBag.Contexts = _vcwMasterDataBusiness.GetActiveContext_Datas();
            ViewBag.Strategies = _vcwMasterDataBusiness.GetActiveCoachingStrategy_Datas();
            return View(model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [CLIUrlAuthorizeAttribute(Account = Authority.All, PageId = PagesModel.Coach, Anonymity = Anonymous.Verified)]
        public string Edit(CoachFileModel model, string uploadfiles, int[] language
            , int[] Content, int[] Context, int[] Strategy)
        {
            var response = new PostFormResponse();
            if (ModelState.IsValid)
            {
                Vcw_FileEntity entity = _vcwBusiness.GetFileEntity(model.ID);

                if (entity.FileContents != null)
                {
                    _vcwBusiness.DeleteFileContent(entity.FileContents.ToList(), false);
                }

                if (!string.IsNullOrEmpty(uploadfiles))
                {
                    string[] files = uploadfiles.Split('|');
                    if (files.Length == 2)
                    {
                        entity.FileName = files[0];
                        entity.FilePath = files[1];
                    }
                }
                entity.IdentifyFileName = model.IdentifyFileName;
                entity.DateRecorded = model.DateRecorded.Value;
                entity.StrategyOther = model.StrategyOther;
                entity.ContentOther = model.ContentOther;
                entity.ContextId = Context == null ? 0 : Context[0];
                entity.ContextOther = model.ContextOther;
                entity.Objectives = model.Objectives;
                entity.Effectiveness = model.Effectiveness;
                entity.LanguageId = language == null ? 0 : language[0];
                entity.UpdatedOn = DateTime.Now;
                if (Content != null)
                {
                    List<FileContentEntity> fileContents = new List<FileContentEntity>();
                    foreach (int item in Content)
                    {
                        FileContentEntity FileContent = new FileContentEntity();
                        FileContent.ContentId = item;
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

                OperationResult result = _vcwBusiness.UpdateFile(entity);
                response.Success = result.ResultType == OperationResultType.Success;
                response.Message = result.Message;
            }
            response.ModelState = ModelState;
            return JsonHelper.SerializeObject(response);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [CLIUrlAuthorizeAttribute(Account = Authority.Index, PageId = PagesModel.CoachAssignment, Anonymity = Anonymous.Verified)]
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
    }
}