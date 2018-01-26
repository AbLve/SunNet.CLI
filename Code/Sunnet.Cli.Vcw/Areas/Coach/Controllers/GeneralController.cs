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
using Sunnet.Cli.Business.Vcw;
using Sunnet.Cli.Business.Vcw.Models;
using System.Linq.Expressions;
using Sunnet.Framework.Helpers;
using Sunnet.Cli.Core.Vcw.Entities;
using Sunnet.Cli.Core.Vcw.Enums;
using Sunnet.Cli.UIBase.Models;
using Sunnet.Framework.Core.Tool;
using Sunnet.Cli.Business.Users;
using Sunnet.Cli.Business.Common;
using Sunnet.Cli.Core.Users.Enums;
using Sunnet.Cli.UIBase.Filters;
using Sunnet.Framework.Permission;
using Sunnet.Framework.Extensions;

namespace Sunnet.Cli.Vcw.Areas.Coach.Controllers
{

    public class GeneralController : BaseController
    {
        VcwBusiness _vcwBusiness;
        VCW_MasterDataBusiness _vcwMasterDataBusiness;
        private readonly UserBusiness _userBusiness;
        public GeneralController()
        {
            _vcwBusiness = new VcwBusiness(VcwUnitWorkContext);
            _userBusiness = new UserBusiness();
            _vcwMasterDataBusiness = new VCW_MasterDataBusiness(VcwUnitWorkContext);
        }


        //
        // GET: /Coach/General/
        [CLIUrlAuthorizeAttribute(Account = Authority.Index, PageId = PagesModel.CoachGeneral, Anonymity = Anonymous.Verified)]
        public ActionResult Index()
        {
            return View();
        }

        [CLIUrlAuthorizeAttribute(Account = Authority.Index, PageId = PagesModel.CoachGeneral, Anonymity = Anonymous.Verified)]
        public string Search(string sort = "ID", string order = "Desc", int first = 0, int count = 10)
        {
            int total = 0;

            List<FileListModel> list = new List<FileListModel>();

            Expression<Func<Vcw_FileEntity, bool>> fileContition = PredicateHelper.True<Vcw_FileEntity>();
            fileContition = fileContition.And(r => r.OwnerId == UserInfo.ID);
            fileContition = fileContition.And(r => r.IsDelete == false);
            fileContition = fileContition.And(r => r.VideoType == FileTypeEnum.CoachGeneral);
            list = _vcwBusiness.GetSummaryList(fileContition, sort, order, first, count, out total);
            if (list.Count > 0)
            {
                List<SelectItemModel> Strategies = _vcwMasterDataBusiness.GetActiveCoachingStrategy_Datas();
                foreach (FileListModel item in list)
                {
                    item.Strategies = Strategies.Where(r => item.StrategyIds.Contains(r.ID));
                }
            }

            var result = new { total = total, data = list };
            return JsonHelper.SerializeObject(result);
        }

        [CLIUrlAuthorizeAttribute(Account = Authority.Index, PageId = PagesModel.CoachGeneral, Anonymity = Anonymous.Verified)]
        public ActionResult Upload()
        {
            Role role = UserInfo.Role;
            if (role == Role.Mentor_coach || role == Role.Coordinator)
            {
                ViewBag.PM = _userBusiness.GetPMByCoach(UserInfo.ID);
            }
            else
            {
                ViewBag.PM = new List<SelectItemModel>();
            }
            ViewBag.Strategies = _vcwMasterDataBusiness.GetActiveCoachingStrategy_Datas();
            return View();
        }

        [CLIUrlAuthorizeAttribute(Account = Authority.All, PageId = PagesModel.Coach, Anonymity = Anonymous.Verified)]
        public ActionResult View(int id, bool isShared = false)
        {
            ViewBag.IsShared = isShared;
            CoachFileModel model = _vcwBusiness.GetCoachGeneralFileModel(id);
            if (model.StrategyIds.Count() > 0)
            {
                List<SelectItemModel> Strategies = _vcwMasterDataBusiness.GetActiveCoachingStrategy_Datas();
                model.Strategies = Strategies.Where(r => model.StrategyIds.Contains(r.ID));
            }
            return View(model);
        }

        [CLIUrlAuthorizeAttribute(Account = Authority.Index, PageId = PagesModel.CoachGeneral, Anonymity = Anonymous.Verified)]
        public ActionResult SharedFiles()
        {
            return View();
        }

        [CLIUrlAuthorizeAttribute(Account = Authority.Index, PageId = PagesModel.CoachGeneral, Anonymity = Anonymous.Verified)]
        public string SearchShared(string sort = "ID", string order = "Desc", int first = 0, int count = 10)
        {
            int total = 0;

            List<FileListModel> list = new List<FileListModel>();

            List<int> SharedFiles = _vcwBusiness.GetSharedFilesByUser(UserInfo.ID, SharedUserTypeEnum.Coach);
            if (SharedFiles != null)
            {
                if (SharedFiles.Count > 0)
                {
                    Expression<Func<Vcw_FileEntity, bool>> fileContition = PredicateHelper.True<Vcw_FileEntity>();
                    fileContition = fileContition.And(r => SharedFiles.Contains(r.ID));
                    fileContition = fileContition.And(r => r.IsDelete == false);
                    fileContition = fileContition.And(r => r.VideoType == FileTypeEnum.CoachGeneral);

                    list = _vcwBusiness.GetSummaryList(fileContition, sort, order, first, count, out total);
                }
            }

            if (list.Count > 0)
            {
                List<SelectItemModel> Strategies = _vcwMasterDataBusiness.GetActiveCoachingStrategy_Datas();
                foreach (FileListModel item in list)
                {
                    item.Strategies = Strategies.Where(r => item.StrategyIds.Contains(r.ID));
                }
            }

            var result = new { total = total, data = list };
            return JsonHelper.SerializeObject(result);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [CLIUrlAuthorizeAttribute(Account = Authority.Index, PageId = PagesModel.CoachGeneral, Anonymity = Anonymous.Verified)]
        public string Delete(int[] video_select)
        {
            var response = new PostFormResponse();
            if (video_select != null)
            {
                List<int> deleteids = video_select.ToList();
                OperationResult result = _vcwBusiness.DeleteFile(deleteids);
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        [CLIUrlAuthorizeAttribute(Account = Authority.Index, PageId = PagesModel.CoachGeneral, Anonymity = Anonymous.Verified)]
        public string Upload(CoachFileModel model, string uploadfiles, int[] pm, int[] coach, int[] Strategy)
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
                OperationResult result = _vcwBusiness.InsertFileEntity(model, pm, coach, Strategy, UploadUserTypeEnum.Coach, UserInfo);
                response.Success = result.ResultType == OperationResultType.Success;
                response.Message = result.Message;
            }
            response.ModelState = ModelState;
            return JsonHelper.SerializeObject(response);
        }

        [CLIUrlAuthorizeAttribute(Account = Authority.All, PageId = PagesModel.Coach, Anonymity = Anonymous.Verified)]
        public ActionResult Edit(int id, string redirect = "")
        {
            ViewBag.Redirect = redirect;
            CoachFileModel model = _vcwBusiness.GetCoachGeneralFileModel(id);
            List<FileSharedEntity> fileShareds = model.FileShareds.ToList();
            //查找分享的coach
            List<int> coachIds = fileShareds.Where(r => r.Type == SharedUserTypeEnum.Coach).Select(r => r.UserId).ToList();
            List<CoachListModel> selectedCoaches = _userBusiness.GetFileSharedCoaches(coachIds);
            ViewBag.SelectedCoaches = selectedCoaches;
            //查找分享的coach
            ViewBag.PM = _userBusiness.GetPMByCoach(model.OwnerId);
            ViewBag.SelectedPMs = fileShareds.Where(r => r.Type == SharedUserTypeEnum.PM).Select(r => r.UserId).ToList();
            ViewBag.Strategies = _vcwMasterDataBusiness.GetActiveCoachingStrategy_Datas();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [CLIUrlAuthorizeAttribute(Account = Authority.All, PageId = PagesModel.Coach, Anonymity = Anonymous.Verified)]
        public string Edit(CoachFileModel model, string uploadfiles, int[] pm, int[] coach, int[] Strategy)
        {
            Vcw_FileEntity entity = _vcwBusiness.GetFileEntity(model.ID);
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
                entity.UploadDate = DateTime.Now;
                entity.IdentifyFileName = model.IdentifyFileName;
                entity.DateRecorded = model.DateRecorded.Value;
                entity.StrategyOther = model.StrategyOther;
                entity.Objectives = model.Objectives;
                entity.Effectiveness = model.Effectiveness;
                if (!string.IsNullOrEmpty(uploadfiles))
                {
                    entity.FileName = model.FileName;
                    entity.FilePath = model.FilePath;
                }

                if (entity.FileContents != null)
                {
                    _vcwBusiness.DeleteFileContent(entity.FileContents.ToList(), false);
                }

                //删除FileShareds关系表
                if (entity.FileShareds != null)
                {
                    _vcwBusiness.DeleteFileShareds(entity.FileShareds.ToList(), false);
                }

                if (pm != null)
                {
                    if (pm.Length > 0)
                    {
                        foreach (int item in pm)
                        {
                            FileSharedEntity FileShared = new FileSharedEntity();
                            FileShared.UserId = item;
                            FileShared.Type = SharedUserTypeEnum.PM;
                            entity.FileShareds.Add(FileShared);
                        }
                    }
                }

                //添加FileShareds关系表                
                if (coach != null)
                {
                    if (coach.Length > 0)
                    {
                        foreach (int item in coach)
                        {
                            FileSharedEntity FileShared = new FileSharedEntity();
                            FileShared.UserId = item;
                            FileShared.Type = SharedUserTypeEnum.Coach;
                            entity.FileShareds.Add(FileShared);
                        }
                    }
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
            OperationResult result = _vcwBusiness.UpdateFile(entity);
            response.Success = result.ResultType == OperationResultType.Success;
            response.Message = result.Message;
            response.ModelState = ModelState;
            return JsonHelper.SerializeObject(response);
        }
    }
}