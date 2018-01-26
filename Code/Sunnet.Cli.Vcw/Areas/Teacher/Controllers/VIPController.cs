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
using Sunnet.Cli.Core.Vcw.Entities;
using System.Linq.Expressions;
using Sunnet.Cli.Business.Users;
using Sunnet.Cli.Business.Vcw;
using Sunnet.Framework.Helpers;
using Sunnet.Cli.UIBase.Models;
using Sunnet.Cli.Core.Vcw.Enums;
using Sunnet.Framework.Extensions;
using Sunnet.Cli.UIBase;
using Sunnet.Framework.Core.Tool;
using Sunnet.Cli.UIBase.Filters;
using Sunnet.Framework.Permission;
using Sunnet.Cli.Business.Common;

namespace Sunnet.Cli.Vcw.Areas.Teacher.Controllers
{
    public class VIPController : BaseController
    {
        UserBusiness _userBusiness;
        VcwBusiness _vcwBusiness;
        VCW_MasterDataBusiness _vcwMasterDataBusiness;
        public VIPController()
        {
            _userBusiness = new UserBusiness();
            _vcwBusiness = new VcwBusiness(VcwUnitWorkContext);
            _vcwMasterDataBusiness = new VCW_MasterDataBusiness(VcwUnitWorkContext);
        }

        // GET: /Teacher/VIP/
        [CLIUrlAuthorizeAttribute(Account = Authority.Index, PageId = PagesModel.TeacherVip, Anonymity = Anonymous.Verified)]
        public ActionResult Index()
        {
            List<VIPDocumentEntity> VIPDicList = _vcwBusiness.GetVIPDocuments();
            ViewBag.VIPDicList = VIPDicList;
            return View();
        }

        [CLIUrlAuthorizeAttribute(Account = Authority.Index, PageId = PagesModel.TeacherVip, Anonymity = Anonymous.Verified)]
        public string Search(string sort = "DueDate", string order = "Desc", int first = 0, int count = 10)
        {
            int total = 0;

            List<AssignmentListModel> list = new List<AssignmentListModel>();

            Expression<Func<AssignmentEntity, bool>> assignmentContition = PredicateHelper.True<AssignmentEntity>();
            assignmentContition = assignmentContition.And(r => r.ReceiveUserId == UserInfo.ID);
            assignmentContition = assignmentContition.And(r => r.AssignmentType == AssignmentTypeEnum.TeacherVIP);
            assignmentContition = assignmentContition.And(r => r.IsDelete == false);

            list = _vcwBusiness.GetVIPByTeacher(assignmentContition, sort, order, first, count, out total);

            if (list.Count > 0)
            {
                List<int> senderIds = list.Select(s => s.SendUserId).Distinct().ToList();
                List<UsernameModel> userNames = _userBusiness.GetUsernames(senderIds);

                IEnumerable<SelectItemModel> Contents = _vcwMasterDataBusiness.GetActiveAssignment_Content_Datas();
                IEnumerable<SelectItemModel> Contexts = _vcwMasterDataBusiness.GetActiveContext_Datas();

                foreach (AssignmentListModel item in list)
                {
                    UsernameModel username = userNames.Find(r => r.ID == item.SendUserId);
                    if (username != null)
                    {
                        item.SendUserName = username.Firstname + " " + username.Lastname;
                    }
                    item.Contents = Contents.Where(r => item.ContentIds.Contains(r.ID));
                    item.Contexts = Contexts.Where(r => item.ContextIds.Contains(r.ID));
                }
            }

            var result = new { total = total, data = list };
            return JsonHelper.SerializeObject(result);
        }

        [CLIUrlAuthorizeAttribute(Account = Authority.Index, PageId = PagesModel.TeacherVip, Anonymity = Anonymous.Verified)]
        public ActionResult View(int id)
        {
            AssignmentModel Assignment = _vcwBusiness.GetTeacherAssignment(id);
            if (Assignment != null)
            {
                ViewBag.DueDate = Assignment.DueDate.FormatDateString();
                ViewBag.Wave = Assignment.WaveText;
                ViewBag.Status = Assignment.Status.ToDescription();
                ViewBag.Content = Assignment.Content;
                ViewBag.Context = Assignment.Context;
                ViewBag.AssignmentId = Assignment.ID;
                ViewBag.Description = Assignment.Description;
                ViewBag.Documents = _vcwBusiness.GetVIPDocuments();
                if (!Assignment.IsVisited)
                {
                    VcwUnitWorkContext.DbContext.Database.ExecuteSqlCommand("update Assignments set IsVisited=1 where ID=" + Assignment.ID + "");
                }
            }
            else
            {
                ViewBag.AssignmentId = -1;
            }
            return View();
        }


        /// <summary>
        /// 查找对应VIP Assignment下的Files
        /// </summary>
        /// <param name="assignmentId"></param>
        /// <param name="sort"></param>
        /// <param name="order"></param>
        /// <param name="first"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        [CLIUrlAuthorizeAttribute(Account = Authority.Index, PageId = PagesModel.TeacherVip, Anonymity = Anonymous.Verified)]
        public string SearchFiles(int assignmentId, string sort = "Status", string order = "Asc", int first = 0, int count = 10)
        {
            if (assignmentId > 0)
            {
                var total = 0;
                var expression = PredicateHelper.True<Vcw_FileEntity>();
                expression = expression.And(o => o.AssignmentId == assignmentId && o.IsDelete == false);
                expression = expression.And(o => o.OwnerId == UserInfo.ID);

                var list = _vcwBusiness.GetSummaryList(expression, sort, order, first, count, out total);
                var result = new { total = total, data = list };
                return JsonHelper.SerializeObject(result);
            }
            else
            {
                var result = new { total = 0 };
                return JsonHelper.SerializeObject(result);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [CLIUrlAuthorizeAttribute(Account = Authority.Index, PageId = PagesModel.TeacherVip, Anonymity = Anonymous.Verified)]
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

        [CLIUrlAuthorizeAttribute(Account = Authority.Index, PageId = PagesModel.TeacherVip, Anonymity = Anonymous.Verified)]
        public ActionResult UploadVideo(int AssignmentID)
        {
            ViewBag.Title = "Upload Video";
            AssignmentEntity assignmentEntity = _vcwBusiness.GetAssignment(AssignmentID);
            if (assignmentEntity != null && assignmentEntity.ReceiveUserId == UserInfo.ID)
            {
                TeacherVIPFileModel model = new TeacherVIPFileModel();
                model.AssignmentID = AssignmentID;
                ViewBag.Languages = _vcwMasterDataBusiness.GetActiveVideo_Language_Datas();
                return View(model);
            }
            else
                return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [CLIUrlAuthorizeAttribute(Account = Authority.Index, PageId = PagesModel.TeacherVip, Anonymity = Anonymous.Verified)]
        public string UploadVideo(TeacherVIPFileModel model, string uploadfiles, int[] language)
        {
            // uploadfiles : Penguins.jpg;vcw/2014-10-31/537758C3A3E226AA_465969598.jpg
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
                    response.Message = GetInformation("Vcw_File_Noupload");
                    return JsonHelper.SerializeObject(response);
                }

                model.OwnerId = UserInfo.ID;
                model.Status = FileStatus.Submitted;
                model.UploadUserType = UploadUserTypeEnum.Teacher;
                model.LanguageId = language == null ? 0 : language[0];
                OperationResult result = _vcwBusiness.InsertFileEntity(model, UserInfo);
                response.Success = result.ResultType == OperationResultType.Success;
                response.Message = result.Message;
                if (response.Success)//更新Assignment的Status
                {
                    _vcwBusiness.ChangeStatus(ChangeStatusEnum.AddFile, model.AssignmentID);
                }

                response.Data = new FileListModel()
                {
                    ID = model.ID,
                    Status = model.Status,
                    UploadDate = DateTime.Now,
                    DateRecorded = model.DateRecorded.Value,
                    LanguageText = _vcwBusiness.GetLanguageText(model.LanguageId),
                    FileName = model.FileName,
                    FilePath = model.FilePath,
                    IdentifyFileName = model.IdentifyFileName
                };
            }
            response.ModelState = ModelState;
            return JsonHelper.SerializeObject(response);
        }

        [CLIUrlAuthorizeAttribute(Account = Authority.All, PageId = PagesModel.Teachers, Anonymity = Anonymous.Verified)]
        public ActionResult ViewFile(int id)
        {
            TeacherVIPFileModel model = _vcwBusiness.GetTeacherVIPFileModel(id);
            ViewBag.Languages = _vcwMasterDataBusiness.GetActiveVideo_Language_Datas();
            return View(model);
        }

        [CLIUrlAuthorizeAttribute(Account = Authority.All, PageId = PagesModel.Teachers, Anonymity = Anonymous.Verified)]
        public ActionResult EditFile(int id, string redirect = "")
        {
            ViewBag.Redirect = redirect;
            TeacherVIPFileModel model = _vcwBusiness.GetTeacherVIPFileModel(id);
            ViewBag.Languages = _vcwMasterDataBusiness.GetActiveVideo_Language_Datas();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [CLIUrlAuthorizeAttribute(Account = Authority.All, PageId = PagesModel.Teachers, Anonymity = Anonymous.Verified)]
        public string Edit(TeacherVIPFileModel model, string uploadfiles, int[] language)
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
                        entity.FileName = files[0];
                        entity.FilePath = files[1];
                    }
                }

                entity.UpdatedOn = DateTime.Now;
                entity.IdentifyFileName = model.IdentifyFileName;
                entity.DateRecorded = model.DateRecorded.Value;
                entity.LanguageId = language == null ? 0 : language[0];
                entity.Description = model.Description;
                OperationResult result = _vcwBusiness.UpdateFile(entity);
                response.Success = result.ResultType == OperationResultType.Success;
                response.Message = result.Message;
            }
            response.ModelState = ModelState;
            return JsonHelper.SerializeObject(response);
        }

        /// <summary>
        /// 用于重置Assignment的Status
        /// </summary>
        /// <returns></returns>
        [CLIUrlAuthorizeAttribute(Account = Authority.All, PageId = PagesModel.VCW, Anonymity = Anonymous.Verified)]
        public string InitAssignmentStatus()
        {
            List<FileListModel> assignments = _vcwBusiness.GetAllFiles();
            foreach (FileListModel item in assignments)
            {
                _vcwBusiness.ChangeStatus(ChangeStatusEnum.UpdateFile, item.ID, item.Status);
            }
            var response = new PostFormResponse();
            response.Success = true;
            return JsonHelper.SerializeObject(response);
        }

    }
}