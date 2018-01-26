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
using Sunnet.Cli.Business.Common;
using System.Web.Script.Serialization;
using Sunnet.Cli.UIBase.Filters;
using Sunnet.Framework.Permission;
using Sunnet.Framework.Extensions;

namespace Sunnet.Cli.Vcw.Areas.Teacher.Controllers
{
    public class GeneralController : BaseController
    {
        VcwBusiness _vcwBusiness;
        VCW_MasterDataBusiness _vcwMasterDataBusiness;
        public GeneralController()
        {
            _vcwBusiness = new VcwBusiness(VcwUnitWorkContext);
            _vcwMasterDataBusiness = new VCW_MasterDataBusiness(VcwUnitWorkContext);
        }

        // GET: /Teacher/General/
        [CLIUrlAuthorizeAttribute(Account = Authority.Index, PageId = PagesModel.TeacherGeneral, Anonymity = Anonymous.Verified)]
        public ActionResult Index()
        {
            return View();
        }

        [CLIUrlAuthorizeAttribute(Account = Authority.Index, PageId = PagesModel.TeacherGeneral, Anonymity = Anonymous.Verified)]
        public string Search(int uploadby = 0, int videotype = 0,
          string sort = "ID", string order = "Desc", int first = 0, int count = 10)
        {
            int total = 0;

            List<FileListModel> list = new List<FileListModel>();

            Expression<Func<Vcw_FileEntity, bool>> fileContition = PredicateHelper.True<Vcw_FileEntity>();
            fileContition = fileContition.And(r => r.OwnerId == UserInfo.ID);
            fileContition = fileContition.And(r => r.IsDelete == false);
            fileContition = fileContition.And(r => r.VideoType == FileTypeEnum.TeacherGeneral);

            list = _vcwBusiness.GetSummaryList(fileContition, sort, order, first, count, out total);

            if (list.Count > 0)
            {
                IEnumerable<SelectItemModel> Contents = _vcwMasterDataBusiness.GetActiveVideo_Content_Datas();

                foreach (FileListModel item in list)
                {
                    item.Contents = Contents.Where(r => item.ContentIds.Contains(r.ID));
                }
            }

            var result = new { total = total, data = list };
            return JsonHelper.SerializeObject(result);
        }

        [CLIUrlAuthorizeAttribute(Account = Authority.Index, PageId = PagesModel.TeacherGeneral, Anonymity = Anonymous.Verified)]
        public ActionResult New()
        {
            ViewBag.Contents = _vcwMasterDataBusiness.GetActiveVideo_Content_Datas();
            ViewBag.Contexts = _vcwMasterDataBusiness.GetActiveContext_Datas();
            return View();
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        [CLIUrlAuthorizeAttribute(Account = Authority.Index, PageId = PagesModel.TeacherGeneral, Anonymity = Anonymous.Verified)]
        public string New(TeacherGeneralFileModel model, string uploadfiles, int[] Content, int[] Context)
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
                    model.UploadUserType = UploadUserTypeEnum.Teacher;
                }
                if (string.IsNullOrEmpty(model.FileName))
                {
                    response.Success = false;
                    response.Message = GetInformation("Vcw_File_Noupload");
                    return JsonHelper.SerializeObject(response);
                }
                OperationResult result = _vcwBusiness.InsertFileEntity(model, Content, Context, UserInfo);
                response.Success = result.ResultType == OperationResultType.Success;
                response.Message = result.Message;
            }
            response.ModelState = ModelState;
            return JsonHelper.SerializeObject(response);
        }

        [CLIUrlAuthorizeAttribute(Account = Authority.All, PageId = PagesModel.Teachers, Anonymity = Anonymous.Verified)]
        public ActionResult Edit(int id, string redirect = "")
        {
            ViewBag.Redirect = redirect;
            TeacherGeneralFileModel model = _vcwBusiness.GetTeacherGeneralFileModel(id);
            ViewBag.Contents = _vcwMasterDataBusiness.GetActiveVideo_Content_Datas();
            ViewBag.Contexts = _vcwMasterDataBusiness.GetActiveContext_Datas();
            return View(model);
        }

        [CLIUrlAuthorizeAttribute(Account = Authority.All, PageId = PagesModel.Teachers, Anonymity = Anonymous.Verified)]
        public ActionResult View(int id)
        {
            TeacherGeneralFileModel model = _vcwBusiness.GetTeacherGeneralFileModel(id);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [CLIUrlAuthorizeAttribute(Account = Authority.Index, PageId = PagesModel.TeacherGeneral, Anonymity = Anonymous.Verified)]
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
        [CLIUrlAuthorizeAttribute(Account = Authority.All, PageId = PagesModel.Teachers, Anonymity = Anonymous.Verified)]
        public string Edit(TeacherGeneralFileModel model, string uploadfiles, int[] Content, int[] Context)
        {
            Vcw_FileEntity entity = _vcwBusiness.GetFileEntity(model.ID);
            var response = new PostFormResponse();
            if (ModelState.IsValid)
            {
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
                entity.UpdatedOn = DateTime.Now;
                entity.DateRecorded = model.DateVieoRecorded.Value;
                entity.ContextId = Context == null ? 0 : Context[0];
                entity.ContextOther = model.ContextOther;
                entity.ContentOther = model.ContentOther;
                entity.Description = model.Description;
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
                OperationResult result = _vcwBusiness.UpdateFile(entity);
                response.Success = result.ResultType == OperationResultType.Success;
                response.Message = result.Message;
            }
            response.ModelState = ModelState;
            return JsonHelper.SerializeObject(response);
        }
    }
}