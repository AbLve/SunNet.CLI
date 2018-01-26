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
using Sunnet.Cli.Core.Users.Entities;
using Sunnet.Cli.Core.Vcw.Enums;
using Sunnet.Framework.Extensions;
using Sunnet.Framework.Helpers;
using Sunnet.Cli.Business.Vcw.Models;
using System.Linq.Expressions;
using Sunnet.Cli.Core.Vcw.Entities;
using Sunnet.Cli.Business.Schools;
using System.IO;
using Sunnet.Framework.PDF;
using Sunnet.Cli.Vcw.Models;
using Sunnet.Cli.Core.Users.Enums;

namespace Sunnet.Cli.Vcw.Areas.Coach.Controllers
{
    public class SummaryController : BaseController
    {
        UserBusiness _userBusiness;
        VcwBusiness _vcwBusiness;
        SchoolBusiness _schoolBusiness;
        VcwReport _vcwReport;
        VCW_MasterDataBusiness _vcwMasterDataBusiness;
        public SummaryController()
        {
            _vcwBusiness = new VcwBusiness(VcwUnitWorkContext);
            _userBusiness = new UserBusiness();
            _schoolBusiness = new SchoolBusiness();
            _vcwReport = new VcwReport(VcwUnitWorkContext);
            _vcwMasterDataBusiness = new VCW_MasterDataBusiness(VcwUnitWorkContext);
        }

        //
        // GET: /Coach/Summary/
        [CLIUrlAuthorizeAttribute(Account = Authority.Index, PageId = PagesModel.CoachSummary, Anonymity = Anonymous.Verified)]
        public ActionResult Index()
        {
            List<SelectListItem> Uploadby = new List<SelectListItem>();
            Uploadby.Add(new SelectListItem { Text = ViewTextHelper.DefaultAllText, Value = "-1" });
            Uploadby.Add(new SelectListItem { Text = UploadUserTypeEnum.Coach.ToDescription(), Value = UploadUserTypeEnum.Coach.GetValue().ToString() });
            Uploadby.Add(new SelectListItem { Text = UploadUserTypeEnum.PM.ToDescription(), Value = UploadUserTypeEnum.PM.GetValue().ToString() });

            List<SelectListItem> VideoType = new List<SelectListItem>();
            VideoType.Add(new SelectListItem { Text = ViewTextHelper.DefaultAllText, Value = "-1" });
            VideoType.Add(new SelectListItem { Text = FileTypeEnum.CoachGeneral.ToDescription(), Value = FileTypeEnum.CoachGeneral.GetValue().ToString() });
            VideoType.Add(new SelectListItem { Text = FileTypeEnum.CoachAssignment.ToDescription(), Value = FileTypeEnum.CoachAssignment.GetValue().ToString() });

            ViewBag.UploadBy = Uploadby;
            ViewBag.VideoType = VideoType;
            ViewBag.UserId = UserInfo.ID;
            return View();
        }

        [CLIUrlAuthorizeAttribute(Account = Authority.Index, PageId = PagesModel.CoachSummary, Anonymity = Anonymous.Verified)]
        public string SearchCoachFiles(int uploadby, int videotype, string number, string sort = "ID", string order = "Desc", int first = 0, int count = 10)
        {
            int total = 0;

            List<FileListModel> list = new List<FileListModel>();

            Expression<Func<Vcw_FileEntity, bool>> fileContition = PredicateHelper.True<Vcw_FileEntity>();
            fileContition = fileContition.And(r => r.OwnerId == UserInfo.ID);
            fileContition = fileContition.And(r => r.IsDelete == false);

            if (videotype > 0)
                fileContition = fileContition.And(r => r.VideoType == (FileTypeEnum)videotype);
            else
                fileContition = fileContition.And(r => r.VideoType == FileTypeEnum.CoachGeneral || r.VideoType == FileTypeEnum.CoachAssignment);
            if (uploadby == UploadUserTypeEnum.Coach.GetValue())//coach上传
                fileContition = fileContition.And(r => r.UploadUserType == UploadUserTypeEnum.Coach);
            if (uploadby == UploadUserTypeEnum.PM.GetValue())//PM上传               
                fileContition = fileContition.And(r => (r.UploadUserType == UploadUserTypeEnum.PM || r.UploadUserType == UploadUserTypeEnum.Admin));

            if (!string.IsNullOrEmpty(number))
                fileContition = fileContition.And(GetDropDownItems.GetNumberExpression(number));

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

        [CLIUrlAuthorizeAttribute(Account = Authority.Index, PageId = PagesModel.CoachSummary, Anonymity = Anonymous.Verified)]
        public ActionResult TeacherFiles()
        {
            if (UserInfo.Role == Role.Coordinator)  //Coordinator角色可查看所分配的Community下的所有Teacher
            {
                List<object> DropdownItems = GetDropDownItems.GetItemsByPM_Teacher(UserInfo.ID);
                if (DropdownItems.Count >= 4)
                {
                    ViewBag.Communities = DropdownItems[0];
                    ViewBag.Schools = DropdownItems[2];
                    ViewBag.Teachers = DropdownItems[3];
                }
            }
            else if (UserInfo.Role == Role.Mentor_coach)
            {
                List<object> dropdownData = GetDropDownItems.GetItems(UserInfo.ID);

                if (dropdownData.Count >= 3)
                {
                    ViewBag.Teachers = dropdownData[0];
                    ViewBag.Schools = dropdownData[1];
                    ViewBag.Communities = dropdownData[2];
                }
            }
            else
            {
                List<object> dropdownData = GetDropDownItems.GetExternalItems(UserInfo);

                if (dropdownData.Count >= 3)
                {
                    ViewBag.Teachers = dropdownData[0];
                    ViewBag.Schools = dropdownData[1];
                    ViewBag.Communities = dropdownData[2];
                }
            }

            List<SelectListItem> Uploadby = new List<SelectListItem>();
            Uploadby.Add(new SelectListItem { Text = ViewTextHelper.DefaultAllText, Value = "-1" });
            Uploadby.Add(new SelectListItem() { Text = UploadUserTypeEnum.Coach.ToDescription(), Value = UploadUserTypeEnum.Coach.GetValue().ToString() });
            Uploadby.Add(new SelectListItem() { Text = UploadUserTypeEnum.Teacher.ToDescription(), Value = UploadUserTypeEnum.Teacher.GetValue().ToString() });

            List<SelectListItem> VideoType = new List<SelectListItem>();
            VideoType.Add(new SelectListItem { Text = ViewTextHelper.DefaultAllText, Value = "-1" });
            VideoType.Add(new SelectListItem { Text = FileTypeEnum.TeacherVIP.ToDescription(), Value = FileTypeEnum.TeacherVIP.GetValue().ToString() });
            VideoType.Add(new SelectListItem { Text = FileTypeEnum.TeacherGeneral.ToDescription(), Value = FileTypeEnum.TeacherGeneral.GetValue().ToString() });
            VideoType.Add(new SelectListItem { Text = FileTypeEnum.TeacherAssignment.ToDescription(), Value = FileTypeEnum.TeacherAssignment.GetValue().ToString() });

            ViewBag.UploadBy = Uploadby;
            ViewBag.VideoType = VideoType;

            return View();
        }

        [CLIUrlAuthorizeAttribute(Account = Authority.Index, PageId = PagesModel.CoachSummary, Anonymity = Anonymous.Verified)]
        public string SearchTeacherFiles(int community, int school, int teacher, int uploadby, int videotype, string number,
            string sort = "ID", string order = "Asc", int first = 0, int count = 10)
        {
            var total = 0;
            var expression = GetExpression.GetTeacherSummaryExpression(community, school, teacher,
                uploadby, videotype, number, UserInfo);

            List<FileListModel> list = _vcwBusiness.GetSummaryListByCoach(expression, sort, order, first, count, out total);

            if (list.Count > 0)
                list = _vcwBusiness.FormatTeacherGenerals(list);

            var result = new { total = total, data = list };
            return JsonHelper.SerializeObject(result);
        }

        [CLIUrlAuthorizeAttribute(Account = Authority.Index, PageId = PagesModel.CoachSummary, Anonymity = Anonymous.Verified)]
        public void ExportExcel_CoachFiles(int uploadby = -1, int videotype = -1, string number = "")
        {
            List<FileListModel> list = new List<FileListModel>();

            Expression<Func<Vcw_FileEntity, bool>> fileContition = PredicateHelper.True<Vcw_FileEntity>();
            fileContition = fileContition.And(r => r.OwnerId == UserInfo.ID);
            fileContition = fileContition.And(r => r.IsDelete == false);

            if (videotype > 0)
                fileContition = fileContition.And(r => r.VideoType == (FileTypeEnum)videotype);
            else
                fileContition = fileContition.And(r => r.VideoType == FileTypeEnum.CoachGeneral || r.VideoType == FileTypeEnum.CoachAssignment);
            if (uploadby == UploadUserTypeEnum.Coach.GetValue())//coach上传
                fileContition = fileContition.And(r => r.UploadUserType == UploadUserTypeEnum.Coach);
            if (uploadby == UploadUserTypeEnum.PM.GetValue())//PM上传               
                fileContition = fileContition.And(r => (r.UploadUserType == UploadUserTypeEnum.PM || r.UploadUserType == UploadUserTypeEnum.Admin));
            if (!string.IsNullOrEmpty(number))
                fileContition = fileContition.And(GetDropDownItems.GetNumberExpression(number));

            string filepath = _vcwReport.CoachFilesToExcel(fileContition);

            FileInfo fileinfo = new FileInfo(filepath);

            FileStream fs = new FileStream(filepath, FileMode.Open);
            byte[] bytes = new byte[(int)fs.Length];
            fs.Read(bytes, 0, bytes.Length);
            fs.Close();
            Response.ContentType = "application/octet-stream";
            Response.AddHeader("Content-Disposition", "attachment;filename=CoachSummary_CoachFiles" + filepath.Substring(filepath.LastIndexOf(".")));
            Response.BinaryWrite(bytes);
            Response.Flush();
            Response.End();
        }

        [CLIUrlAuthorizeAttribute(Account = Authority.Index, PageId = PagesModel.CoachSummary, Anonymity = Anonymous.Verified)]
        public ActionResult ExportPdf_CoachFiles(int uploadby = -1, int videotype = -1, string number = "", bool isExport = true)
        {
            int total = 0;

            List<FileListModel> list = new List<FileListModel>();

            Expression<Func<Vcw_FileEntity, bool>> fileContition = PredicateHelper.True<Vcw_FileEntity>();
            fileContition = fileContition.And(r => r.OwnerId == UserInfo.ID);
            fileContition = fileContition.And(r => r.IsDelete == false);

            if (videotype > 0)
                fileContition = fileContition.And(r => r.VideoType == (FileTypeEnum)videotype);
            else
                fileContition = fileContition.And(r => r.VideoType == FileTypeEnum.CoachGeneral || r.VideoType == FileTypeEnum.CoachAssignment);
            if (uploadby == UploadUserTypeEnum.Coach.GetValue())//coach上传
                fileContition = fileContition.And(r => r.UploadUserType == UploadUserTypeEnum.Coach);
            if (uploadby == UploadUserTypeEnum.PM.GetValue())//PM上传               
                fileContition = fileContition.And(r => (r.UploadUserType == UploadUserTypeEnum.PM || r.UploadUserType == UploadUserTypeEnum.Admin));
            if (!string.IsNullOrEmpty(number))
                fileContition = fileContition.And(GetDropDownItems.GetNumberExpression(number));

            list = _vcwBusiness.GetSummaryList(fileContition, "ID", "DESC", 0, int.MaxValue, out total);
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
            ViewBag.List = list;
            if (isExport)
            {
                GetPdf(GetViewHtml("ExportPdf_CoachFiles"), "CoachSummary_CoachFiles.pdf");
            }
            return View();
        }


        [CLIUrlAuthorizeAttribute(Account = Authority.Index, PageId = PagesModel.CoachSummary, Anonymity = Anonymous.Verified)]
        public void ExportExcel_TeacherFiles(int community = -1, int school = -1, int teacher = -1, int uploadby = -1, string number = "", int videotype = -1)
        {
            var total = 0;
            List<FileListModel> list = new List<FileListModel>();
            var expression = GetExpression.GetTeacherSummaryExpression(community, school, teacher,
                uploadby, videotype, number, UserInfo);

            list = _vcwBusiness.GetSummaryListByCoach(expression, "ID", "DESC", 0, int.MaxValue, out total);

            if (list.Count > 0)
                list = _vcwBusiness.FormatTeacherGenerals(list);

            string filepath = _vcwReport.TeacherFilesToExcel(list);

            FileInfo fileinfo = new FileInfo(filepath);

            FileStream fs = new FileStream(filepath, FileMode.Open);
            byte[] bytes = new byte[(int)fs.Length];
            fs.Read(bytes, 0, bytes.Length);
            fs.Close();
            Response.ContentType = "application/octet-stream";
            Response.AddHeader("Content-Disposition", "attachment;filename=CoachSummary_TeacherFiles" + filepath.Substring(filepath.LastIndexOf(".")));
            Response.BinaryWrite(bytes);
            Response.Flush();
            Response.End();
        }

        [CLIUrlAuthorizeAttribute(Account = Authority.Index, PageId = PagesModel.CoachSummary, Anonymity = Anonymous.Verified)]
        public ActionResult ExportPdf_TeacherFiles(int community = -1, int school = -1, int teacher = -1, int uploadby = -1, int videotype = -1,
            string number = "", bool isExport = true)
        {
            var total = 0;
            List<FileListModel> list = new List<FileListModel>();
            var expression = GetExpression.GetTeacherSummaryExpression(community, school, teacher,
                uploadby, videotype, number, UserInfo);

            list = _vcwBusiness.GetSummaryListByCoach(expression, "ID", "DESC", 0, int.MaxValue, out total);

            if (list.Count > 0)
                list = _vcwBusiness.FormatTeacherGenerals(list);
            ViewBag.List = list;
            if (isExport)
            {
                GetPdf(GetViewHtml("ExportPdf_TeacherFiles"), "CoachSummary_TeacherFiles.pdf");
            }
            return View();
        }


        private void GetPdf(string html, string fileName)
        {
            PdfProvider pdfProvider = new PdfProvider();
            pdfProvider.IsPortrait = false;
            pdfProvider.GeneratePDF(html, fileName);
        }

        private string GetViewHtml(string viewName)
        {
            var resultHtml = string.Empty;
            ViewEngineResult result = ViewEngines.Engines.FindView(ControllerContext, viewName, null);
            if (null == result.View)
            {
                throw new InvalidOperationException();
            }
            try
            {
                ViewContext viewContext = new ViewContext(ControllerContext, result.View, this.ViewData, this.TempData, Response.Output);
                var textWriter = new StringWriter();
                result.View.Render(viewContext, textWriter);
                resultHtml = textWriter.ToString();
            }
            finally
            {
                result.ViewEngine.ReleaseView(ControllerContext, result.View);
            }
            return resultHtml;
        }
    }
}