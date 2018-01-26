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
using Sunnet.Cli.Business.Users.Models;
using Sunnet.Framework.Extensions;
using Sunnet.Cli.Business.Vcw.Models;
using System.Linq.Expressions;
using Sunnet.Framework.Helpers;
using Sunnet.Cli.Core.Vcw.Entities;
using Sunnet.Cli.Core.Vcw.Enums;
using Sunnet.Framework.Core.Tool;
using Sunnet.Cli.Business.Common;
using Sunnet.Cli.Vcw.Models;
using System.Web.Script.Serialization;

namespace Sunnet.Cli.Vcw.Areas.PM.Controllers
{
    public class CoachGeneralController : BaseController
    {
        VcwBusiness _vcwBusiness;
        UserBusiness _userBusiness;
        VCW_MasterDataBusiness _vcwMasterDataBusiness;
        public CoachGeneralController()
        {
            _vcwBusiness = new VcwBusiness(VcwUnitWorkContext);
            _userBusiness = new UserBusiness();
            _vcwMasterDataBusiness = new VCW_MasterDataBusiness(VcwUnitWorkContext);
        }

        //
        // GET: /PM/CoacherGeneral/
        [CLIUrlAuthorizeAttribute(Account = Authority.Index, PageId = PagesModel.PMCoaches, Anonymity = Anonymous.Verified)]
        public ActionResult Index()
        {
            List<object> Dropdown = GetDropDownItems.GetItemsByPM(UserInfo.ID);
            ViewBag.Coaches = Dropdown[0];
            ViewBag.Communities = Dropdown[1];
            return View();
        }


        [CLIUrlAuthorizeAttribute(Account = Authority.Index, PageId = PagesModel.PMCoaches, Anonymity = Anonymous.Verified)]
        public string Search(int community = -1, int coach = -1, int status = -1, string number = "",
            string sort = "ID", string order = "Desc", int first = 0, int count = 10)
        {
            int total = 0;

            List<FileListModel> list = new List<FileListModel>();

            List<int> SharedFiles = _vcwBusiness.GetSharedFilesByUser(UserInfo.ID, SharedUserTypeEnum.PM);
            if (SharedFiles != null)
            {
                if (SharedFiles.Count > 0)
                {
                    Expression<Func<Vcw_FileEntity, bool>> fileContition = PredicateHelper.True<Vcw_FileEntity>();
                    if (community > 0)
                    {
                        List<int> user_Ids = _userBusiness.GetAssignedUsersByCommunity(community).Select(a => a.UserId).ToList();
                        fileContition = fileContition.And(r => user_Ids.Contains(r.OwnerId));
                    }
                    if (coach > 0)
                        fileContition = fileContition.And(r => r.OwnerId == coach);
                    if (status > 0)
                        fileContition = fileContition.And(r => r.Status == (FileStatus)status);
                    if (!string.IsNullOrEmpty(number))
                        fileContition = fileContition.And(GetDropDownItems.GetNumberExpression(number));
                    fileContition = fileContition.And(r => SharedFiles.Contains(r.ID));
                    fileContition = fileContition.And(r => r.IsDelete == false);
                    fileContition = fileContition.And(r => r.VideoType == FileTypeEnum.CoachGeneral);

                    list = _vcwBusiness.GetSharedFiles(fileContition, sort, order, first, count, out total);

                    if (list.Count > 0)
                    {
                        List<SelectItemModel> Strategies = _vcwMasterDataBusiness.GetActiveCoachingStrategy_Datas();
                        foreach (FileListModel item in list)
                        {
                            item.Strategies = Strategies.Where(r => item.StrategyIds.Contains(r.ID));
                        }
                    }
                }
            }

            var result = new { total = total, data = list };
            return JsonHelper.SerializeObject(result);
        }

        [CLIUrlAuthorizeAttribute(Account = Authority.All, PageId = PagesModel.PM, Anonymity = Anonymous.Verified)]
        public ActionResult View(int id, string redirect = "")
        {
            CoachFileModel model = _vcwBusiness.GetCoachGeneralFileModel(id);
            if (model.UploadUserId != UserInfo.ID)
            {
                if (model.StrategyIds.Count() > 0)
                {
                    List<SelectItemModel> Strategies = _vcwMasterDataBusiness.GetActiveCoachingStrategy_Datas();
                    model.Strategies = Strategies.Where(r => model.StrategyIds.Contains(r.ID));
                }
            }
            ViewBag.Redirect = redirect;
            ViewBag.Strategies = _vcwMasterDataBusiness.GetActiveCoachingStrategy_Datas();
            List<FileSharedEntity> fileShareds = model.FileShareds.ToList();
            //Coach
            List<int> coachIds = fileShareds.Where(r => r.Type == SharedUserTypeEnum.Coach).Select(r => r.UserId).ToList();
            List<CoachListModel> selectedCoaches = _userBusiness.GetFileSharedCoaches(coachIds);
            ViewBag.SelectedCoaches = selectedCoaches;
            ViewBag.IsPM = model.UploadUserId == UserInfo.ID;
            //PM     
            ViewBag.PM = _userBusiness.GetPMByCoach(model.OwnerId);
            ViewBag.SelectedPMs = fileShareds.Where(r => r.Type == SharedUserTypeEnum.PM).Select(r => r.UserId).ToList();

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
        public string View(CoachFileModel model, string uploadfile_feedback,
            string uploadfile, int[] pm, int[] coach, int[] Strategy, bool isPM = false)
        {
            Vcw_FileEntity File = _vcwBusiness.GetFileEntity(model.ID);
            if (isPM)
            {
                File.IdentifyFileName = model.IdentifyFileName;
                File.DateRecorded = model.DateRecorded.Value;
                File.Objectives = model.Objectives;
                File.Effectiveness = model.Effectiveness;

                File.StrategyOther = model.StrategyOther;
                if (!string.IsNullOrEmpty(uploadfile))
                {
                    string[] files = uploadfile.Split('|');
                    if (files.Length == 2)
                    {
                        File.FileName = files[0];
                        File.FilePath = files[1];
                    }
                }

                //删除FileShareds关系表
                if (File.FileShareds != null)
                {
                    _vcwBusiness.DeleteFileShareds(File.FileShareds.ToList(), false);
                }

                //添加FileShareds关系表
                if (pm != null)
                {
                    if (pm.Length > 0)
                    {
                        foreach (int item in pm)
                        {
                            FileSharedEntity FileShared = new FileSharedEntity();
                            FileShared.UserId = item;
                            FileShared.Type = SharedUserTypeEnum.PM;
                            File.FileShareds.Add(FileShared);
                        }
                    }
                }
                if (coach != null)
                {
                    if (coach.Length > 0)
                    {
                        foreach (int item in coach)
                        {
                            FileSharedEntity FileShared = new FileSharedEntity();
                            FileShared.UserId = item;
                            FileShared.Type = SharedUserTypeEnum.Coach;
                            File.FileShareds.Add(FileShared);
                        }
                    }
                }

                //添加FileStrategies关系表
                if (File.FileStrategies != null)
                {
                    _vcwBusiness.DeleteFileStrategies(File.FileStrategies.ToList(), false);
                }
                if (Strategy != null)
                {
                    if (Strategy.Length > 0)
                    {
                        foreach (int item in Strategy)
                        {
                            FileStrategyEntity FileStrategy = new FileStrategyEntity();
                            FileStrategy.StrategyId = item;
                            FileStrategy.CreatedOn = DateTime.Now;
                            File.FileStrategies.Add(FileStrategy);
                        }
                    }
                }
            }
            if (!string.IsNullOrEmpty(uploadfile_feedback))
            {
                string[] file = uploadfile_feedback.Split('|');
                File.FeedbackFileName = file[0];
                File.FeedbackFilePath = file[1];
            }

            File.FeedbackText = model.FeedbackText;
            File.UpdatedBy = UserInfo.ID;
            File.UpdatedOn = DateTime.Now;
            var response = new PostFormResponse();
            OperationResult result = _vcwBusiness.UpdateFile(File);
            response.Success = result.ResultType == OperationResultType.Success;
            response.Message = result.Message;
            return JsonHelper.SerializeObject(response);
        }

        [CLIUrlAuthorizeAttribute(Account = Authority.Index, PageId = PagesModel.PMCoaches, Anonymity = Anonymous.Verified)]
        public ActionResult Upload()
        {
            ViewBag.CoachesBehalf = _userBusiness.GetCoachByPM(UserInfo.ID).ToSelectList()
                .AddDefaultItem(ViewTextHelper.DefaultPleaseSelectText, "").ToList();
            ViewBag.Strategies = _vcwMasterDataBusiness.GetActiveCoachingStrategy_Datas();
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [CLIUrlAuthorizeAttribute(Account = Authority.Index, PageId = PagesModel.PMCoaches, Anonymity = Anonymous.Verified)]
        public string Upload(CoachFileModel model, string uploadfiles, int[] pm, int[] coach, int[] Strategy)
        {
            var response = new PostFormResponse();
            if (ModelState.IsValid)
            {
                if (!string.IsNullOrEmpty(uploadfiles))
                {
                    Vcw_FileEntity File = new Vcw_FileEntity();
                    File.IdentifyFileName = model.IdentifyFileName;
                    string[] files = uploadfiles.Split('|');
                    if (files.Length == 2)
                    {
                        File.FileName = files[0];
                        File.FilePath = files[1];
                    }

                    //添加File信息表                    
                    File.AssignmentId = 0;
                    File.DateRecorded = model.DateRecorded.Value;
                    File.StrategyOther = model.StrategyOther;
                    File.Objectives = model.Objectives;
                    File.Effectiveness = model.Effectiveness;
                    File.OwnerId = model.OwnerId;
                    File.UploadUserId = UserInfo.ID;
                    File.UploadUserType = UploadUserTypeEnum.PM;
                    File.VideoType = FileTypeEnum.CoachGeneral;
                    File.Status = FileStatus.Submitted;
                    //----------------------------------------
                    File.CreatedBy = UserInfo.ID;
                    File.UpdatedBy = UserInfo.ID;
                    File.UploadDate = DateTime.Now;
                    File.UploadUserId = UserInfo.ID;
                    File.IsDelete = false;
                    File.TBRSDate = CommonAgent.MinDate;

                    File.FileShareds = new List<FileSharedEntity>();
                    File.FileStrategies = new List<FileStrategyEntity>();

                    //添加FileShareds关系表
                    if (pm != null)
                    {
                        if (pm.Length > 0)
                        {
                            foreach (int item in pm)
                            {
                                FileSharedEntity FileShared = new FileSharedEntity();
                                FileShared.UserId = item;
                                FileShared.Type = SharedUserTypeEnum.PM;
                                File.FileShareds.Add(FileShared);
                            }
                        }
                    }
                    if (coach != null)
                    {
                        if (coach.Length > 0)
                        {
                            foreach (int item in coach)
                            {
                                FileSharedEntity FileShared = new FileSharedEntity();
                                FileShared.UserId = item;
                                FileShared.Type = SharedUserTypeEnum.Coach;
                                File.FileShareds.Add(FileShared);
                            }
                        }
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
                                File.FileStrategies.Add(FileStrategy);
                            }
                        }
                    }
                    OperationResult result = _vcwBusiness.InsertFileEntity(File);
                    response.Success = result.ResultType == OperationResultType.Success;
                    response.Message = result.Message;
                }
            }
            response.ModelState = ModelState;
            return JsonHelper.SerializeObject(response);
        }


        [CLIUrlAuthorizeAttribute(Account = Authority.Index, PageId = PagesModel.PMCoaches, Anonymity = Anonymous.Verified)]
        public ActionResult ViewShared(int id)
        {
            CoachFileModel model = _vcwBusiness.GetCoachGeneralFileModel(id);
            if (model.StrategyIds.Count() > 0)
            {
                List<SelectItemModel> Strategies = _vcwMasterDataBusiness.GetActiveCoachingStrategy_Datas();
                model.Strategies = Strategies.Where(r => model.StrategyIds.Contains(r.ID));
            }
            CoachesListModel coach = _userBusiness.GetCoachInfoById(model.OwnerId);
            if (coach != null)
            {
                ViewBag.Coach = coach;
            }
            return View(model);
        }

        [CLIUrlAuthorizeAttribute(Account = Authority.Index, PageId = PagesModel.PMCoaches, Anonymity = Anonymous.Verified)]
        public string UpdateShared(CoachFileModel model, string uploadfile_feedback)
        {
            Vcw_FileEntity File = _vcwBusiness.GetFileEntity(model.ID);
            if (!string.IsNullOrEmpty(uploadfile_feedback))
            {
                string[] file = uploadfile_feedback.Split('|');
                File.FeedbackFileName = file[0];
                File.FeedbackFilePath = file[1];
            }
            File.FeedbackText = model.FeedbackText;
            var response = new PostFormResponse();
            OperationResult result = _vcwBusiness.UpdateFile(File);
            response.Success = result.ResultType == OperationResultType.Success;
            response.Message = result.Message;
            return JsonHelper.SerializeObject(response);
        }


        [HttpPost]
        [CLIUrlAuthorizeAttribute(Account = Authority.Index, PageId = PagesModel.PMCoaches, Anonymity = Anonymous.Verified)]
        public string Delete(int[] video_select)
        {
            var response = new PostFormResponse();
            if (video_select != null)
            {
                List<int> deleteids = video_select.ToList();
                OperationResult result = _vcwBusiness.DeleteFileSharedRelation(deleteids, SharedUserTypeEnum.PM, UserInfo.ID);
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

        public string GetPMByCoach(int coachId)
        {
            List<SelectItemModel> pms = _userBusiness.GetPMByCoach(coachId);
            JavaScriptSerializer js = new JavaScriptSerializer();
            return js.Serialize(pms);
        }
    }
}