using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
/**************************************************************************
 * Developer: 		Joe
 * Computer:		JOE-PC
 * Domain:			Joe-pc
 * CreatedOn:		2015/4/17
 * Description:		Please input class summary
 * Version History:	Created,2015/4/17
 * 
 * 
 **************************************************************************/
using Sunnet.Cli.UIBase.Filters;
using Sunnet.Framework.Permission;
using Sunnet.Cli.UIBase.Models;
using Sunnet.Cli.Business.Vcw.Models;
using Sunnet.Cli.Core.Vcw.Enums;
using Sunnet.Cli.Core.Vcw.Entities;
using Sunnet.Cli.Business.Vcw;
using Sunnet.Cli.Business.Common;
using Sunnet.Framework.Core.Tool;
using Sunnet.Framework.Helpers;
using Sunnet.Cli.Business.Users;
using Sunnet.Cli.Business.Schools;
using Sunnet.Cli.Core.Users.Enums;
using Sunnet.Framework.Extensions;

namespace Sunnet.Cli.Vcw.Controllers
{
    public class LinkVideoController : BaseController
    {
        VcwBusiness _vcwBusiness;
        UserBusiness _userBusiness;
        SchoolBusiness _schoolBusiness;
        VCW_MasterDataBusiness _vcwMasterDataBusiness;
        public LinkVideoController()
        {
            _vcwBusiness = new VcwBusiness(VcwUnitWorkContext);
            _userBusiness = new UserBusiness();
            _schoolBusiness = new SchoolBusiness();
            _vcwMasterDataBusiness = new VCW_MasterDataBusiness(VcwUnitWorkContext);
        }


        /// <summary>
        /// 查找Assignment列表
        /// </summary>
        /// <param name="community"></param>
        /// <param name="school"></param>
        /// <param name="teacher"></param>
        /// <param name="status"></param>
        /// <param name="sort"></param>
        /// <param name="order"></param>
        /// <param name="first"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        [CLIUrlAuthorizeAttribute(Account = Authority.All, PageId = PagesModel.VCW, Anonymity = Anonymous.Verified)]
        public string SearchAssignment(int teacher, string sort = "ID", string order = "Asc", int first = 0, int count = 10)
        {
            var total = 0;
            var expression = PredicateHelper.True<AssignmentEntity>();
            List<AssignmentListModel> list;

            if (teacher > 0)
            {
                expression = expression.And(o => o.ReceiveUserId == teacher);

                expression = expression.And(o =>
                             o.AssignmentType == AssignmentTypeEnum.TeacherAssignment && o.IsDelete == false);

                list = _vcwBusiness.GetTeacherAssignments(expression, sort, order, first, count, out total);

                if (list.Count > 0)
                {
                    List<int> teacherIds = list.Select(a => a.TeacherId).Distinct().ToList();
                    List<TeacherListModel> list_teacher = _userBusiness.GetTeacherInfoByUserIds(teacherIds);
                    List<int> senderIds = list.Select(s => s.SendUserId).Distinct().ToList();
                    List<UsernameModel> userNames = _userBusiness.GetUsernames(senderIds);
                    IEnumerable<SelectItemModel> Contents = _vcwMasterDataBusiness.GetActiveAssignment_Content_Datas();
                    IEnumerable<SelectItemModel> Contexts = _vcwMasterDataBusiness.GetActiveContext_Datas();
                    IEnumerable<SelectItemModel> UploadTypes = _vcwMasterDataBusiness.GetActiveUploadTypes();

                    foreach (AssignmentListModel item in list)
                    {
                        TeacherListModel teacher_select = list_teacher.Where(r => r.TeacherUserId == item.TeacherId).FirstOrDefault();
                        if (teacher_select != null)
                        {
                            item.SchoolName = teacher_select.SchoolName;
                            item.CommunityName = teacher_select.CommunityName;
                        }
                        UsernameModel username = userNames.Find(r => r.ID == item.SendUserId);
                        if (username != null)
                        {
                            item.SendUserName = username.Firstname + " " + username.Lastname;
                        }
                        item.Contents = Contents.Where(r => item.ContentIds.Contains(r.ID));
                        item.Contexts = Contexts.Where(r => item.ContextIds.Contains(r.ID));
                        item.UploadTypes = UploadTypes.Where(r => item.UploadTypeIds.Contains(r.ID));
                    }
                }
            }
            else
            {
                list = new List<AssignmentListModel>();
            }

            var result = new { total = total, data = list };
            return JsonHelper.SerializeObject(result);
        }


        /// <summary>
        /// 查找VIP列表
        /// </summary>
        /// <param name="community"></param>
        /// <param name="school"></param>
        /// <param name="teacher"></param>
        /// <param name="status"></param>
        /// <param name="sort"></param>
        /// <param name="order"></param>
        /// <param name="first"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        [CLIUrlAuthorizeAttribute(Account = Authority.All, PageId = PagesModel.VCW, Anonymity = Anonymous.Verified)]
        public string SearchVIP(int teacher, string sort = "ID", string order = "Asc", int first = 0, int count = 10)
        {
            var total = 0;
            var expression = PredicateHelper.True<AssignmentEntity>();
            List<AssignmentListModel> list;

            if (teacher > 0)
            {
                expression = expression.And(o => o.ReceiveUserId == teacher);

                expression = expression.And(o =>
                             o.AssignmentType == AssignmentTypeEnum.TeacherVIP && o.IsDelete == false);

                list = _vcwBusiness.GetTeacherAssignments(expression, sort, order, first, count, out total);

                if (list.Count > 0)
                {
                    List<int> teacherIds = list.Select(a => a.TeacherId).Distinct().ToList();
                    List<TeacherListModel> list_teacher = _userBusiness.GetTeacherInfoByUserIds(teacherIds);

                    List<int> senderIds = list.Select(s => s.SendUserId).Distinct().ToList();
                    List<UsernameModel> userNames = _userBusiness.GetUsernames(senderIds);
                    IEnumerable<SelectItemModel> Contents = _vcwMasterDataBusiness.GetActiveAssignment_Content_Datas();
                    IEnumerable<SelectItemModel> Contexts = _vcwMasterDataBusiness.GetActiveContext_Datas();
                    IEnumerable<SelectItemModel> UploadTypes = _vcwMasterDataBusiness.GetActiveUploadTypes();

                    foreach (AssignmentListModel item in list)
                    {
                        TeacherListModel teacher_select = list_teacher.Where(r => r.TeacherUserId == item.TeacherId).FirstOrDefault();
                        if (teacher_select != null)
                        {
                            item.SchoolName = teacher_select.SchoolName;
                            item.CommunityName = teacher_select.CommunityName;
                        }
                        UsernameModel username = userNames.Find(r => r.ID == item.SendUserId);
                        if (username != null)
                        {
                            item.SendUserName = username.Firstname + " " + username.Lastname;
                        }
                        item.Contents = Contents.Where(r => item.ContentIds.Contains(r.ID));
                        item.Contexts = Contexts.Where(r => item.ContextIds.Contains(r.ID));
                        item.UploadTypes = UploadTypes.Where(r => item.UploadTypeIds.Contains(r.ID));
                    }
                }
            }
            else
            {
                list = new List<AssignmentListModel>();
            }
            var result = new { total = total, data = list };
            return JsonHelper.SerializeObject(result);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [CLIUrlAuthorizeAttribute(Account = Authority.All, PageId = PagesModel.VCW, Anonymity = Anonymous.Verified)]
        public string LinkToAssignment(int[] assignment_select, string fileId, FileTypeEnum videotype)
        {
            var response = new PostFormResponse();
            if (!string.IsNullOrEmpty(fileId))
            {
                List<int> list_ids = assignment_select.ToList();
                List<LinkToAssignmentModel> list_assignment = _vcwBusiness.GetLinkedAssignments(list_ids);
                Vcw_FileEntity file = _vcwBusiness.GetFileEntity(int.Parse(fileId));
                List<Vcw_FileEntity> list_new = new List<Vcw_FileEntity>();
                foreach (int item in assignment_select)    //复制Date Recorded、Language、Comments信息，其余信息均为默认值
                {
                    Vcw_FileEntity linkedFile = new Vcw_FileEntity();
                    linkedFile.AssignmentId = item;
                    linkedFile.IdentifyFileName = string.Empty;
                    linkedFile.Status = FileStatus.Submitted;
                    linkedFile.OwnerId = list_assignment.FindAll(a => a.ID == item).Select(a => a.ReceiveUserId).FirstOrDefault();
                    linkedFile.UploadUserId = UserInfo.ID;
                    linkedFile.UploadDate = DateTime.Now;
                    linkedFile.VideoType = videotype;
                    linkedFile.FileName = file.FileName;
                    linkedFile.FilePath = file.FilePath;
                    linkedFile.DateRecorded = file.DateRecorded;
                    linkedFile.Language = file.Language;
                    linkedFile.Description = file.Description;
                    linkedFile.IdentifyFileName = file.IdentifyFileName;
                    linkedFile.TBRSDate = CommonAgent.MinDate;
                    linkedFile.CreatedBy = UserInfo.ID;
                    linkedFile.CreatedOn = DateTime.Now;
                    linkedFile.UpdatedBy = UserInfo.ID;
                    linkedFile.UpdatedOn = DateTime.Now;
                    linkedFile.IsDelete = false;
                    linkedFile.UploadUserType = UploadUserTypeEnum.Coach;
                    if (videotype == FileTypeEnum.TeacherAssignment) //TeacherAssignment类型的文件需要复制以下信息
                    {
                        linkedFile.ContextId = file.ContextId;
                        linkedFile.ContextOther = file.ContextOther;
                    }
                    list_new.Add(linkedFile);
                }
                OperationResult result = _vcwBusiness.InsertFileEntity(list_new);
                if (result.ResultType == OperationResultType.Success)
                {
                    foreach (int item in assignment_select)
                    {
                        _vcwBusiness.ChangeStatus(ChangeStatusEnum.AddFile, item);
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