using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
/**************************************************************************
 * Developer: 		Joe
 * Computer:		JOE-PC
 * Domain:			Joe
 * CreatedOn:		2015/11/8
 * Description:		Add TxkeaReceptive Item
 * Version History:	Created,2015/11/8
 * 
 * 
 **************************************************************************/
using Sunnet.Cli.Business.Ade.Models;
using Sunnet.Cli.Business.Ade;
using Sunnet.Cli.Assessment.Controllers;
using Sunnet.Framework.Extensions;
using Sunnet.Cli.UIBase.Models;
using Sunnet.Framework.Helpers;
using Sunnet.Cli.Core.Ade.Entities;
using Sunnet.Framework.Core.Tool;
using Sunnet.Cli.Core.Users.Enums;
using Sunnet.Cli.Business.Users;
using Sunnet.Cli.Business.Common;
using Sunnet.Cli.UIBase.Filters;
using Sunnet.Framework.Permission;
using Sunnet.Cli.UIBase;
using Sunnet.Cli.Core.Ade.Enums;
using Sunnet.Framework;
using Sunnet.Cli.Core.Users.Entities;

namespace Sunnet.Cli.Assessment.Areas.Layout.Controllers
{
    public class LayoutController : BaseController
    {
        private AdeBusiness _adeBusiness;
        private IEnumerable<SelectListItem> numberOfImages;
        private UserBusiness _userBusiness;
        private string savePath = SFConfig.UploadFile + "TxkeaLayout/";

        public LayoutController()
        {
            numberOfImages = new List<SelectListItem>();
            numberOfImages.AddDefaultItem("9", 9);
            numberOfImages.AddDefaultItem("8", 8);
            numberOfImages.AddDefaultItem("7", 7);
            numberOfImages.AddDefaultItem("6", 6);
            numberOfImages.AddDefaultItem("5", 5);
            numberOfImages.AddDefaultItem("4", 4);
            numberOfImages.AddDefaultItem("3", 3);
            numberOfImages.AddDefaultItem("2", 2);
            numberOfImages.AddDefaultItem("1", 1);

            _adeBusiness = new AdeBusiness(AdeUnitWorkContext);
            _userBusiness = new UserBusiness();
        }

        // GET: Layout/Layout
        [CLIUrlAuthorizeAttribute(Account = Authority.All, PageId = PagesModel.LDE, Anonymity = Anonymous.Verified)]
        public ActionResult Index()
        {
            return View();
        }

        [CLIUrlAuthorizeAttribute(Account = Authority.All, PageId = PagesModel.LDE, Anonymity = Anonymous.Verified)]
        public string Search(string name, string numberOfImages, string operationUserName, int operationUserId = 0,
            string sort = "ID", string order = "DESC", int first = 0, int count = 10)
        {
            var total = 0;
            var searchCriteria = PredicateHelper.True<TxkeaLayoutEntity>();
            if (!string.IsNullOrEmpty(name))
                searchCriteria = searchCriteria.And(r => r.Name.Contains(name));
            //numberOfImages格式 1;2;3
            if (!string.IsNullOrEmpty(numberOfImages) && numberOfImages.Split(';').Length > 0)
            {
                List<int> imageCount = numberOfImages.Split(';').Select(r => int.Parse(r)).ToList();
                searchCriteria = searchCriteria.And(r => imageCount.Contains(r.NumberOfImages));
            }

            if (operationUserId > 0)
            {
                searchCriteria = searchCriteria.And(searchCriteria.And(r => r.CreatedBy == operationUserId)
                    .Or(r => r.UpdatedBy == operationUserId));
            }
            else
            {
                if (!string.IsNullOrEmpty(operationUserName))
                {
                    var userExpression = PredicateHelper.True<UserBaseEntity>();
                    userExpression = userExpression.And(r => r.FirstName.Contains(operationUserName));
                    userExpression = userExpression.Or(r => r.LastName.Contains(operationUserName));
                    userExpression = userExpression.Or(r => r.MiddleName.Contains(operationUserName));
                    List<int> mappedUserIds = _userBusiness.GetUserSearchModels(userExpression).Select(r => r.ID).ToList();
                    searchCriteria = searchCriteria.And(searchCriteria.And(r => mappedUserIds.Contains(r.CreatedBy))
                        .Or(r => mappedUserIds.Contains(r.UpdatedBy)));
                }
            }

            var list = _adeBusiness.SearchLayouts(searchCriteria, out total, sort, order, first, count);
            List<int> userIds = list.Select(r => r.CreatedBy).Union<int>(list.Select(r => r.UpdatedBy)).Distinct().ToList();
            List<UsernameModel> userNames = _userBusiness.GetUsernames(userIds);
            bool isAdmin = UserInfo.Role == Role.Super_admin;
            foreach (TxkeaLayoutModel item in list)
            {
                UsernameModel createdUser = userNames.Find(r => r.ID == item.CreatedBy);
                UsernameModel updatedUser = userNames.Find(r => r.ID == item.UpdatedBy);
                item.CreatedUserName = createdUser == null ? "" : createdUser.Firstname + " " + createdUser.Lastname;
                item.UpdatedUserName = updatedUser == null ? "" : updatedUser.Firstname + " " + updatedUser.Lastname;
                item.UpdatedOnConvert = item.UpdatedOn.ToString("MM/dd/yyyy HH:mm");
            }
            var result = new { total = total, data = list };
            return JsonHelper.SerializeObject(result);
        }

        [CLIUrlAuthorizeAttribute(Account = Authority.All, PageId = PagesModel.LDE, Anonymity = Anonymous.Verified)]
        public ActionResult New()
        {
            ViewBag.NumImages = numberOfImages;
            TxkeaLayoutModel model = new TxkeaLayoutModel();
            ViewBag.Backgroud = "";
            ViewBag.BackgroudType = Core.Ade.Enums.BackgroundFillType.Color;
            return View(model);
        }

        [CLIUrlAuthorizeAttribute(Account = Authority.All, PageId = PagesModel.LDE, Anonymity = Anonymous.Verified)]
        public ActionResult Edit(int id)
        {
            ViewBag.NumImages = numberOfImages;
            TxkeaLayoutModel model = _adeBusiness.GetLayoutModel(id);
            ViewBag.Layout = model.Layout;
            ViewBag.Backgroud = "";
            if (model.BackgroundFillType == Core.Ade.Enums.BackgroundFillType.Color)
            {
                if (model.BackgroundFill != string.Empty)
                    ViewBag.Backgroud = model.BackgroundFill;
            }
            else
                if (model.BackgroundFill.Trim() != string.Empty)
                    ViewBag.Backgroud = FileHelper.GetPreviewPathofUploadFile(model.BackgroundFill).Replace("\\", "/");

            ViewBag.BackgroudType = (byte)model.BackgroundFillType;
            if (model.RelatedItemsCount > 0)
            {
                ViewBag.ImageUrl = SFConfig.StaticDomain + "upload/TxkeaLayout/" + model.ID + ".png";
                return View("View", model);
            }
            return View("New", model);
        }

        [HttpPost]
        [CLIUrlAuthorizeAttribute(Account = Authority.All, PageId = PagesModel.LDE, Anonymity = Anonymous.Verified)]
        public string SaveLayout(TxkeaLayoutModel model, string layoutPng, string BackgroundFillColor = "")
        {
            model.CreatedBy = UserInfo.ID;
            model.UpdatedBy = UserInfo.ID;
            var response = new PostFormResponse();
            if (BackgroundFillColor.Trim() != string.Empty)
            {
                model.BackgroundFillType = BackgroundFillType.Color;
                model.BackgroundFill = BackgroundFillColor;
            }
            else
            {
                if (model.BackgroundFill.Trim() != string.Empty)
                {
                    model.BackgroundFillType = BackgroundFillType.Image;
                }
                else
                {
                    model.BackgroundFillType = BackgroundFillType.Color;
                }
            }
            var result = _adeBusiness.SaveLayout(model, layoutPng, savePath);
            response.Update(result);
            return JsonHelper.SerializeObject(response);
        }

        [CLIUrlAuthorizeAttribute(Account = Authority.All, PageId = PagesModel.LDE, Anonymity = Anonymous.Verified)]
        public string Delete(int id)
        {
            var response = new PostFormResponse();
            var result = _adeBusiness.DeleteLayout(id, savePath);
            response.Success = result.ResultType == OperationResultType.Success;
            response.Message = result.Message;
            return JsonHelper.SerializeObject(response);
        }

        [CLIUrlAuthorizeAttribute(Account = Authority.All, PageId = PagesModel.LDE, Anonymity = Anonymous.Verified)]
        public ActionResult Copy()
        {
            return View();
        }

        [CLIUrlAuthorizeAttribute(Account = Authority.All, PageId = PagesModel.LDE, Anonymity = Anonymous.Verified)]
        public string SaveAs(int id, string name)
        {
            OperationResult result = new OperationResult(OperationResultType.Success);
            if (id > 0 && !string.IsNullOrEmpty(name))
            {
                TxkeaLayoutEntity preEntity = _adeBusiness.GetLayout(id);
                if (preEntity != null)
                {
                    TxkeaLayoutEntity entity = new TxkeaLayoutEntity();
                    entity.Name = name;
                    entity.NumberOfImages = preEntity.NumberOfImages;
                    entity.BackgroundFill = preEntity.BackgroundFill;
                    entity.BackgroundFillType = preEntity.BackgroundFillType;
                    entity.IsDeleted = false;
                    entity.Layout = preEntity.Layout;
                    entity.ScreenWidth = preEntity.ScreenWidth;
                    entity.CreatedBy = UserInfo.ID;
                    entity.UpdatedBy = UserInfo.ID;
                    result = _adeBusiness.SaveLayout(entity, id, savePath);
                    result.AppendData = entity.ID;
                }
                else
                {
                    result.ResultType = OperationResultType.Error;
                    result.Message = "Arguments error.";
                }
            }
            else
            {
                result.ResultType = OperationResultType.Error;
                result.Message = "Arguments error.";
            }
            return JsonHelper.SerializeObject(result);
        }

        [CLIUrlAuthorizeAttribute(Account = Authority.All, PageId = PagesModel.LDE, Anonymity = Anonymous.Verified)]
        public ActionResult PreviewLayout(int id = 0)
        {
            TxkeaLayoutModel model = new TxkeaLayoutModel();
            if (id > 0)
            {
                model = _adeBusiness.GetLayoutModel(id);
            }
            return View(model);
        }

        public string GetOperationUser(string keyword)
        {
            List<int> userIds = _adeBusiness.GetAllOperationUsers();
            var expression = PredicateHelper.True<UserBaseEntity>();
            if (userIds != null && userIds.Count > 0)
            {
                expression = expression.And(r => userIds.Contains(r.ID));
                var list = _userBusiness.GetUserSearchModels(expression);
                return JsonHelper.SerializeObject(list);
            }
            else
                return string.Empty;
        }
    }
}