using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

using Sunnet.Cli.Assessment.Controllers;
using Sunnet.Cli.Business.Ade;
using Sunnet.Cli.Business.Ade.Models;
using Sunnet.Cli.Business.Users;
using Sunnet.Cli.Core.Ade;
using Sunnet.Cli.Core.Ade.Entities;
using Sunnet.Cli.Core.Ade.Models;
using Sunnet.Cli.Core.Common.Enums;
using Sunnet.Cli.UIBase.Controllers;
using Sunnet.Cli.UIBase.Models;
using Sunnet.Framework;
using Sunnet.Framework.Core.Tool;
using Sunnet.Framework.Extensions;
using Sunnet.Framework.Helpers;
using Sunnet.Cli.UIBase.Filters;
using Sunnet.Framework.Permission;
using Sunnet.Cli.Core.Ade.Enums;
using System.Text;
using ItemModel = Sunnet.Cli.Business.Ade.Models.ItemModel;

namespace Sunnet.Cli.Assessment.Areas.Ade.Controllers
{
    public class ItemController : BaseController
    {
        private AdeBusiness _adeBusiness;
        private UserBusiness _userBuss = new UserBusiness();

        private IEnumerable<SelectListItem> numberOfImages;

        public ItemController()
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
        }

        [CLIUrlAuthorizeAttribute(Account = Authority.All, PageId = PagesModel.ADE, Anonymity = Anonymous.Verified)]
        public string Search(int assessmentId, int measureId, string description, string label, string createdBy,
            int status = -1, string sort = "Sort", string order = "asc", int first = 0, int count = 10)
        {
            var users = _userBuss.SearchUserIds(createdBy);
            var total = 0;
            var searchCriteria = PredicateHelper.True<ItemBaseEntity>()
                .And(x => x.IsDeleted == false);
            if (assessmentId > -1)
                searchCriteria = searchCriteria.And(x => x.Measure.AssessmentId == assessmentId);
            if (measureId > 0)
                searchCriteria = searchCriteria.And(x => x.MeasureId == measureId);
            if (!string.IsNullOrEmpty(label = label.Trim()))
                searchCriteria = searchCriteria.And(x => x.Label.Contains(label));
            if (!string.IsNullOrEmpty(description = description.Trim()))
                searchCriteria = searchCriteria.And(x => x.Description.Contains(description));
            if (users != null)
                searchCriteria = searchCriteria.And(x => users.Contains(x.CreatedBy));
            if (status > -1)
                searchCriteria = searchCriteria.And(x => (int)x.Status == status);
            var list = _adeBusiness.SearchItem(searchCriteria,
                out total, sort, order, first, count);
            var result = new { total = total, data = list };
            return JsonHelper.SerializeObject(result);
        }

        [CLIUrlAuthorizeAttribute(Account = Authority.All, PageId = PagesModel.ADE, Anonymity = Anonymous.Verified)]
        public ActionResult Select(int measureId)
        {
            var model = _adeBusiness.GetMeasureModel(measureId);
            if (model.AssessmentType == AssessmentType.Cpalls)
            {
                var list = ItemTypeHelper.GetCpallsTypes(model.ItemType).Select(x => new SelectListItem
                {
                    Value = x.ToString(),
                    Text = x.ToDescription()
                }).AddDefaultItem("Select an item template.", "-1");

                ViewBag.ItemTypeOptions = list;
            }
            else
            {
                //下拉框只有一个选项时，默认选中
                var list = ItemTypeHelper.Types[model.AssessmentType].Select(x => new SelectListItem
                {
                    Value = x.ToString(),
                    Text = x.ToDescription()
                });
                if (model.AssessmentType == AssessmentType.UpdateObservables)
                {
                    list = list.AddDefaultItem("Select an item template.", "-1");
                }
                ViewBag.ItemTypeOptions = list;
            }
            return View(measureId);
        }

        [CLIUrlAuthorizeAttribute(Account = Authority.All, PageId = PagesModel.ADE, Anonymity = Anonymous.Verified)]
        public ActionResult Copy(int id, int measureId)
        {
            var item = _adeBusiness.GetItemModel(id);
            if (item == null)
                return View("404");
            item.Measure = _adeBusiness.GetMeasureModel(measureId);
            item.BasePath = SFConfig.StaticDomain + "Upload/";
            item.ID = 0;
            item.MeasureId = measureId;
            item.Label = "Copy " + item.Label;
            ViewBag.ModelJson = JsonHelper.SerializeObject(item);
            //TxkeaReceptive TxkeaExpressive
            ViewBag.NumImages = numberOfImages;
            ViewBag.AssessmentId = item.Measure.AssessmentId;
            ViewBag.ImageType = ImageType.Selectable.ToSelectList();
            ViewBag.CopayId = id; //TX-KEA Expressive 用到
            return View(item.Type.ToString(), item);
        }

        [CLIUrlAuthorizeAttribute(Account = Authority.All, PageId = PagesModel.ADE, Anonymity = Anonymous.Verified)]
        public ActionResult New(string id, int measureId)
        {
            var type = id.ToEnum<ItemType>();
            var item = ItemFactory.GetItemModel(type);
            item.MeasureId = measureId;
            item.Measure = _adeBusiness.GetMeasureModel(measureId);
            item.BasePath = SFConfig.StaticDomain + "Upload/";
            ViewBag.ModelJson = JsonHelper.SerializeObject(item);
            item.Status = EntityStatus.Active;
            ViewBag.ImageType = ImageType.Selectable.ToSelectList();

            ViewBag.NumImages = numberOfImages;
            ViewBag.AssessmentId = item.Measure.AssessmentId;
            return View(id, item);
        }

        [CLIUrlAuthorizeAttribute(Account = Authority.All, PageId = PagesModel.ADE, Anonymity = Anonymous.Verified)]
        public ActionResult Edit(int id)
        {
            var item = _adeBusiness.GetItemModel(id);
            if (item == null)
                return View("404");
            item.BasePath = SFConfig.StaticDomain + "Upload/";
            if (item.Type == ItemType.TxkeaExpressive || item.Type == ItemType.TxkeaReceptive)
            {
                item.BranchingItems = _adeBusiness.GetSkipItems(item.Sort, item.MeasureId);
                if (item.BranchingItems.Count > 0) //添加跳转到最后一题的选项
                    item.BranchingItems.Add(new SelectItemModel { ID = -1, Name = "End" });
            }

            ViewBag.ModelJson = JsonHelper.SerializeObject(item);
            if (item.Type == ItemType.TxkeaReceptive)
                ViewBag.Layout = ((TxkeaReceptiveItemModel)item).ItemLayout;
            else if (item.Type == ItemType.TxkeaExpressive)
                ViewBag.Layout = ((TxkeaExpressiveItemModel)item).ItemLayout;
            ViewBag.ImageType = ImageType.Selectable.ToSelectList();
            ViewBag.NumImages = numberOfImages;
            ViewBag.AssessmentId = item.Measure.AssessmentId;

            if (item.Locked)
                return View(item.Type.ToString() + "_View", item);
            return View(item.Type.ToString(), item);
        }

        [CLIUrlAuthorizeAttribute(Account = Authority.All, PageId = PagesModel.ADE, Anonymity = Anonymous.Verified)]
        public ActionResult Existing(int measureId)
        {
            var toMeasure = _adeBusiness.GetMeasureModel(measureId);
            ViewBag.toMeasure = measureId;
            ViewBag.toAssessment = toMeasure.AssessmentId;
            ViewBag.isCpalls = toMeasure.AssessmentType == AssessmentType.Cpalls;
            ViewBag.Assessments = _adeBusiness.GetAssessments(x => x.IsDeleted == false && x.Type == toMeasure.AssessmentType)
                .ToSelectList(ViewTextHelper.DefaultAllText, "-1");
            return View();
        }

        [HttpPost]
        [CLIUrlAuthorizeAttribute(Account = Authority.All, PageId = PagesModel.ADE, Anonymity = Anonymous.Verified)]
        public string Delete(int id)
        {
            var response = new PostFormResponse();
            response.Update(_adeBusiness.DeleteItem(id));
            return JsonHelper.SerializeObject(response);
        }

        #region Save Item Methods
        private string Save(ItemModel model, string answers)
        {
            if (!string.IsNullOrEmpty(answers) && answers.Length > 10)
            {
                var listAnswers = JsonHelper.DeserializeObject<List<AnswerEntity>>(answers);
                model.Answers = listAnswers;
            }
            model.CreatedBy = UserInfo.ID;
            model.UpdatedBy = UserInfo.ID;
            var response = new PostFormResponse();
            var result = _adeBusiness.SaveItem(model);
            response.Update(result);
            return JsonHelper.SerializeObject(response);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        [CLIUrlAuthorizeAttribute(Account = Authority.All, PageId = PagesModel.ADE, Anonymity = Anonymous.Verified)]
        public string SaveCec(CecItemModel model, string answers)
        {
            return Save(model, answers);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        [CLIUrlAuthorizeAttribute(Account = Authority.All, PageId = PagesModel.ADE, Anonymity = Anonymous.Verified)]
        public string SaveChecklist(ChecklistItemModel model, string answers)
        {
            return Save(model, answers);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        [CLIUrlAuthorizeAttribute(Account = Authority.All, PageId = PagesModel.ADE, Anonymity = Anonymous.Verified)]
        public string SaveCot(CotItemModel model, string answers)
        {
            return Save(model, answers);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        [CLIUrlAuthorizeAttribute(Account = Authority.All, PageId = PagesModel.ADE, Anonymity = Anonymous.Verified)]
        public string SaveDirection(DirectionItemModel model, string answers)
        {
            return Save(model, answers);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        [CLIUrlAuthorizeAttribute(Account = Authority.All, PageId = PagesModel.ADE, Anonymity = Anonymous.Verified)]
        public string SaveMultiple(MultipleItemModel model, string answers)
        {
            return Save(model, answers);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        [CLIUrlAuthorizeAttribute(Account = Authority.All, PageId = PagesModel.ADE, Anonymity = Anonymous.Verified)]
        public string SavePa(PaItemModel model, string answers)
        {
            return Save(model, answers);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        [CLIUrlAuthorizeAttribute(Account = Authority.All, PageId = PagesModel.ADE, Anonymity = Anonymous.Verified)]
        public string SaveQuadrant(QuadrantItemModel model, string answers)
        {
            return Save(model, answers);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        [CLIUrlAuthorizeAttribute(Account = Authority.All, PageId = PagesModel.ADE, Anonymity = Anonymous.Verified)]
        public string SaveRapid(RapidExpressiveItemModel model, string answers)
        {
            return Save(model, answers);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        [CLIUrlAuthorizeAttribute(Account = Authority.All, PageId = PagesModel.ADE, Anonymity = Anonymous.Verified)]
        public string SaveReceptive(ReceptiveItemModel model, string answers)
        {
            return Save(model, answers);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        [CLIUrlAuthorizeAttribute(Account = Authority.All, PageId = PagesModel.ADE, Anonymity = Anonymous.Verified)]
        public string SaveReceptivePrompt(ReceptivePromptItemModel model, string answers)
        {
            return Save(model, answers);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        [CLIUrlAuthorizeAttribute(Account = Authority.All, PageId = PagesModel.ADE, Anonymity = Anonymous.Verified)]
        public string SaveTypedResponse(TypedResponseItemModel model, string answers, string responses)
        {
            var response = new PostFormResponse();

            if (!string.IsNullOrEmpty(answers) && answers.Length > 10)
            {
                var listAnswers = JsonHelper.DeserializeObject<List<AnswerEntity>>(answers);
                model.Answers = listAnswers;
            }

            if (!string.IsNullOrEmpty(responses) && responses.Length > 10)
            {
                model.Responses = JsonHelper.DeserializeObject<List<TypedResopnseModel>>(responses);
            }
            else
            {
                response.Success = false;
                response.Message = "Responses is required.";
                return JsonHelper.SerializeObject(response);
            }

            model.CreatedBy = UserInfo.ID;
            model.UpdatedBy = UserInfo.ID;

            var result = _adeBusiness.SaveItem(model);
            response.Update(result);
            return JsonHelper.SerializeObject(response);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        [CLIUrlAuthorizeAttribute(Account = Authority.All, PageId = PagesModel.ADE, Anonymity = Anonymous.Verified)]
        public string SaveKeaReceptive(TxkeaReceptiveItemModel model, string answers, int LayoutId, int radioLayout,
            string BackgroundFillColor = "", int step = 1, int copyId = 0, string branchingScoreList = "",int stepto=0)
        {
            if (!string.IsNullOrEmpty(answers) && answers.Length > 10)
            {
                var listAnswers = JsonHelper.DeserializeObject<List<AnswerEntity>>(answers);
                model.Answers = listAnswers;
            }
            //先保存为背景色，若有背景图，则替换（背景色为空时也要保存为背景色）
            model.BackgroundFillType = BackgroundFillType.Color;
            model.BackgroundFill = BackgroundFillColor;

            //SelectionType为SingleSelect时，以下选项不显示
            if (model.SelectionType == SelectionType.SingleSelect)
            {
                model.BreakCondition = BreakCondition.IncorrectResponse;
                model.StopConditionX = 0;
                model.StopConditionY = 0;
                model.Scoring = ScoringType.AllorNone;
            }
            //BreakCondition不是StopCondition时，以下选项不显示
            if (model.BreakCondition != BreakCondition.StopCondition)
            {
                model.StopConditionX = 0;
                model.StopConditionY = 0;
            }

            if (!string.IsNullOrEmpty(model.BackgroundImage))
            {
                model.BackgroundFillType = BackgroundFillType.Image;
                model.BackgroundFill = model.BackgroundImage;
            }
            if (radioLayout == 1) //custom
                model.LayoutId = 0;
            else
                model.LayoutId = LayoutId;

            model.CreatedBy = UserInfo.ID;
            model.UpdatedBy = UserInfo.ID;
            model.Step = step;
            int realAnswerCount = model.Answers.Where(r => !string.IsNullOrEmpty(r.Audio) || !string.IsNullOrEmpty(r.Picture)).Count();
            //如果没有布局或者只改变了图片数量，状态设置为inactive
            if (string.IsNullOrEmpty(model.ItemLayout) || realAnswerCount != model.NumberOfImages)
                model.Status = EntityStatus.Inactive;

            if (!string.IsNullOrEmpty(branchingScoreList) && branchingScoreList.Length > 10)
            {
                List<BranchingScoreModel> BranchingScoreList = JsonHelper.DeserializeObject<List<BranchingScoreModel>>(branchingScoreList);
                model.BranchingScores = BranchingScoreList.Where(r => r.ID > 0 || (r.ID == 0 && r.IsDeleted == false)).ToList();
            }

            var response = new PostFormResponse();
            var result = _adeBusiness.SaveItem(model);
            response.Update(result);
            if (response.Success)
            {
                if (copyId > 0)  //复制时，要替换layout中的图片信息
                {
                    List<int> preAnswers = _adeBusiness.GetItem(copyId).Answers.OrderBy(r => r.ID).Select(r => r.ID).ToList();
                    if (preAnswers != null && preAnswers.Count > 0)
                    {
                        for (int i = 0; i < model.Answers.Count; i++)
                        {
                            if (i > preAnswers.Count - 1)
                            {
                                break;
                            }
                            else
                            {
                                model.ItemLayout = model.ItemLayout.Replace("?id=" + preAnswers[i] + "&", "?id=" + model.Answers[i].ID + "&")
                                .Replace("\"id\":\"" + preAnswers[i] + "\"", "\"id\":\"" + model.Answers[i].ID + "\"");
                                model.CpallsItemLayout = model.CpallsItemLayout.Replace("?id=" + preAnswers[i] + "&", "?id=" + model.Answers[i].ID + "&")
                                    .Replace("\"id\":" + preAnswers[i], "\"id\":" + model.Answers[i].ID);
                            }
                        }
                        SaveTxkeaReceptiveItemLayout(model.ID, model.ItemLayout, model.CpallsItemLayout, model.ScreenWidth,model.ScreenHeight);
                    }
                }
                if (stepto > 0)
                {
                    response.Message = (stepto).ToString();
                    response.Data = model;
                }
                else
                {
                    response.Message = (step + 1).ToString();
                    response.Data = model;
                }
             
            }


            return JsonHelper.SerializeObject(response);
        }


        #endregion


        [CLIUrlAuthorizeAttribute(Account = Authority.All, PageId = PagesModel.ADE, Anonymity = Anonymous.Verified)]
        public ActionResult AdjustOrder(int assessmentId, int measureId)
        {
            var measure = _adeBusiness.GetMeasureModel(measureId);
            ViewBag.IsCEC = measure.AssessmentType == AssessmentType.Cec;
            int total;
            var items =
                _adeBusiness.SearchItem(
                    x => x.IsDeleted == false && x.MeasureId == measureId, out total,
                    "Sort", "Asc", 0, int.MaxValue);
            items.ForEach(x => x.Measure = measure);
            ViewBag.IsBranching = items.Any(r => r.BranchingScoresLength > 0);
            return View(items);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [CLIUrlAuthorizeAttribute(Account = Authority.All, PageId = PagesModel.ADE, Anonymity = Anonymous.Verified)]
        public string AdjustOrder(int assessmentId, int measureId, List<int> itemIds)
        {
            var response = new PostFormResponse();
            response.Update(_adeBusiness.AdjustItems(assessmentId, measureId, itemIds));
            return JsonHelper.SerializeObject(response);
        }

        [CLIUrlAuthorizeAttribute(Account = Authority.All, PageId = PagesModel.ADE, Anonymity = Anonymous.Verified)]
        public ActionResult Links(int id)
        {
            var model = _adeBusiness.GetItemListModel(id);
            ViewBag.Links = JsonHelper.SerializeObject(model.Links);
            if (model.Locked)
                return View("Links_View", model);
            return View(model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [CLIUrlAuthorizeAttribute(Account = Authority.All, PageId = PagesModel.ADE, Anonymity = Anonymous.Verified)]
        public string Links(int id, string links)
        {
            var response = new PostFormResponse();
            List<AdeLinkEntity> list = new List<AdeLinkEntity>();
            if (links.Trim() != string.Empty)
            {
                string[] tmpLinks = links.Split(';');
                if (tmpLinks.Length > 0)
                {
                    foreach (string s in tmpLinks)
                    {
                        if (s.Trim() != string.Empty)
                        {
                            string[] tmpS = s.Split('|');
                            if (tmpS.Length == 2)
                            {
                                if (tmpS[0].Trim() != string.Empty)
                                {
                                    list.Add(new AdeLinkEntity() { Link = tmpS[0], LinkType = 0, DisplayText = tmpS[1] });
                                }
                            }
                        }
                    }
                }
            }
            if (list.Count > 0)
            {
                var result = _adeBusiness.UpdateLinks<ItemBaseEntity>(id, list);
                response.Update(result);
            }
            else
                response.Success = true;

            return JsonHelper.SerializeObject(response);
        }

        #region Expressive

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        [CLIUrlAuthorizeAttribute(Account = Authority.All, PageId = PagesModel.ADE, Anonymity = Anonymous.Verified)]
        public string SaveTxkeaExpressive(TxkeaExpressiveItemModel model, string imageList = "", int step = 1,
            string responses = "", int copyId = 0, string branchingScoreList = "")
        {
            if (!string.IsNullOrEmpty(imageList) && imageList.Length > 10)
            {
                List<TxkeaExpressiveImageModel> ImageList = JsonHelper.DeserializeObject<List<TxkeaExpressiveImageModel>>(imageList);
                model.ImageList = ImageList;
            }

            if (!string.IsNullOrEmpty(responses) && responses.Length > 10)
            {
                List<TxkeaExpressiveResponseModel> ResponseList = JsonHelper.DeserializeObject<List<TxkeaExpressiveResponseModel>>(responses);
                model.Responses = ResponseList.Where(r => r.ID > 0 || (r.ID == 0 && r.IsDeleted == false)).ToList();
            }

            if (!string.IsNullOrEmpty(branchingScoreList) && branchingScoreList.Length > 10)
            {
                List<BranchingScoreModel> BranchingScoreList = JsonHelper.DeserializeObject<List<BranchingScoreModel>>(branchingScoreList);
                model.BranchingScores = BranchingScoreList.Where(r => r.ID > 0 || (r.ID == 0 && r.IsDeleted == false)).ToList();
            }


            model.CreatedBy = UserInfo.ID;
            model.UpdatedBy = UserInfo.ID;
            model.Step = step;
            var response = new PostFormResponse();
            var result = new OperationResult(OperationResultType.Success);

            if (step == 1 && copyId > 0)
            {
                var copyItem = _adeBusiness.GetItemModel(copyId);
                result = _adeBusiness.CopyTxkeaExpressive(model, (TxkeaExpressiveItemModel)copyItem);
                if (result.ResultType != OperationResultType.Success)
                {
                    response.Update(result);
                    return JsonHelper.SerializeObject(response);
                }

                if (copyId > 0)  //复制时，要替换layout中的图片信息
                {
                    List<int> preImages = ((TxkeaExpressiveItemEntity)_adeBusiness.GetItem(copyId)).ImageList.OrderBy(r => r.ID).Select(r => r.ID).ToList();
                    if (preImages != null && preImages.Count > 0)
                    {
                        for (int i = 0; i < model.ImageList.Count; i++)
                        {
                            if (i > preImages.Count - 1)
                            {
                                break;
                            }
                            else
                            {
                                model.ItemLayout = model.ItemLayout.Replace("?id=" + preImages[i] + "&", "?id=" + model.ImageList[i].ID + "&")
                                    .Replace("\"id\":\"" + preImages[i] + "\"", "\"id\":\"" + model.ImageList[i].ID + "\"");
                                model.CpallsItemLayout = model.CpallsItemLayout.Replace("?id=" + preImages[i] + "&", "?id=" + model.ImageList[i].ID + "&")
                                    .Replace("\"id\":" + preImages[i], "\"id\":" + model.ImageList[i].ID);
                            }
                        }
                        SaveTxkeaExpressiveItemLayout(model.ID, model.ItemLayout, model.CpallsItemLayout, model.ScreenWidth,model.ScreenHeight);
                    }
                }
            }
            else
                result = _adeBusiness.SaveItem(model);

            model.BasePath = SFConfig.StaticDomain + "Upload/";
            if (result.ResultType == OperationResultType.Success)
            {
                response.Success = true;
                response.Message = (step + 1).ToString();

                response.Data = model;
                return JsonHelper.SerializeObject(response);
            }
            else
            {
                response.Update(result);
                return JsonHelper.SerializeObject(response);
            }
        }

        #endregion

        public ActionResult ChooseLayout(int images = 0, string isView = "")
        {
            int total;
            List<TxkeaLayoutModel> list = _adeBusiness.SearchLayouts(r => r.NumberOfImages == images, out total, "Name", "asc", 0, 1000);
            ViewBag.List = list;
            ViewBag.SerilizedList = JsonHelper.SerializeObject(list);
            ViewBag.isView = isView;
            return View();
        }

        public ActionResult ItemLayoutPreview(int id = 0, bool IsNext = false, bool IsStop = false)
        {
            ViewBag.IsNext = IsNext;
            ViewBag.IsStop = IsStop;
            if (id > 0)
            {
                ItemModel item = _adeBusiness.GetItemModel(id);
                if (item != null)
                {
                    if (item.Type == ItemType.TxkeaReceptive)
                    {
                        TxkeaReceptiveItemModel receptiveModel = (TxkeaReceptiveItemModel)item;
                        ViewBag.Layout = receptiveModel.ItemLayout;
                        ViewBag.ScreenWidth = receptiveModel.ScreenWidth;
                        ViewBag.InstructText = receptiveModel.InstructionText;
                        ViewBag.IsNext = receptiveModel.Measure.NextButton;
                        ViewBag.IsStop = receptiveModel.Measure.StopButton;
                    }
                    if (item.Type == ItemType.TxkeaExpressive)
                    {
                        TxkeaExpressiveItemModel expressiveModel = (TxkeaExpressiveItemModel)item;
                        ViewBag.Layout = expressiveModel.ItemLayout;
                        ViewBag.ScreenWidth = expressiveModel.ScreenWidth;
                        ViewBag.InstructText = expressiveModel.InstructionText;
                    }

                }
            }
            return View();
        }

        public ActionResult TxkeaExpressResponsePreview()
        {
            return View();
        }

        public OperationResult SaveTxkeaReceptiveItemLayout(int id, string ItemLayout, string CpallsItemLayout, decimal ScreenWidth,decimal ScreenHeight)
        {
            OperationResult result = new OperationResult(OperationResultType.Success);
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("update TxkeaReceptiveItems set ItemLayout='{0}',CpallsItemLayout='{1}',ScreenWidth={2},ScreenHeight={3} where ID={4}",
                ItemLayout, CpallsItemLayout, ScreenWidth, ScreenHeight, id);
            result = _adeBusiness.ExecuteSql(sb.ToString());
            return result;
        }

        public OperationResult SaveTxkeaExpressiveItemLayout(int id, string ItemLayout, string CpallsItemLayout, decimal ScreenWidth,decimal ScreenHeight)
        {
            OperationResult result = new OperationResult(OperationResultType.Success);
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("update TxkeaExpressiveItems set ItemLayout='{0}',CpallsItemLayout='{1}',ScreenWidth={2},ScreenHeight={3} where ID={4}",
                ItemLayout, CpallsItemLayout, ScreenWidth,ScreenHeight, id);
            result = _adeBusiness.ExecuteSql(sb.ToString());
            return result;
        }

        /// <summary>
        /// 初始化新增字段 CpallsItemLayout 的值，只在发布时执行，不在其他地方调用 , 可以多次执行
        /// </summary>
        /// <returns></returns>
        [CLIUrlAuthorizeAttribute(Account = Authority.All, PageId = PagesModel.ADE, Anonymity = Anonymous.Logined)]
        public string InitCpallsItemLayout()
        {
            //Receptive item
            List<ItemModel> receptiveitems =
                _adeBusiness.GetItemModels(r => r.Type == ItemType.TxkeaReceptive).ToList();
            StringBuilder sb = new StringBuilder();
            foreach (ItemModel item in receptiveitems)
            {
                string itemLayout = ((TxkeaReceptiveItemModel)item).ItemLayout;
                if (!string.IsNullOrEmpty(itemLayout))
                {
                    var jsonLayout = JsonHelper.DeserializeObject<dynamic>(itemLayout);
                    var cpallsItemLayout = "{";
                    cpallsItemLayout += "\"objects\":[";
                    for (int i = 0; i < jsonLayout["objects"].Count; i++)
                    {
                        var jsonObject = JsonHelper.DeserializeObject<dynamic>(jsonLayout["objects"][i].ToString());
                        cpallsItemLayout += "{";
                        cpallsItemLayout += "\"id\":" + jsonObject["id"] + ",\"src\":\"" + jsonObject["src"] + "\",";
                        cpallsItemLayout += "\"top\":" + jsonObject["top"] + ",\"left\":" + jsonObject["left"] + ",";
                        cpallsItemLayout += "\"height\":" + jsonObject["height"] * jsonObject["scaleY"] + ",\"width\":" + jsonObject["width"] * jsonObject["scaleX"];
                        cpallsItemLayout += "},";
                    }
                    cpallsItemLayout = cpallsItemLayout.TrimEnd(',');
                    cpallsItemLayout += "]";
                    if (jsonLayout["background"] != null && !string.IsNullOrEmpty(jsonLayout["background"].ToString()))
                        cpallsItemLayout += ",\"background\":\"" + jsonLayout["background"] + "\"";
                    if (jsonLayout["backgroundImage"] != null && !string.IsNullOrEmpty(jsonLayout["backgroundImage"].ToString()))
                        cpallsItemLayout += ",\"backgroundImage\":{\"src\":\"" + jsonLayout["backgroundImage"]["src"] + "\"}";
                    cpallsItemLayout += "}";

                    sb.AppendFormat("update TxkeaReceptiveItems set CpallsItemLayout='{0}' where ID={1};", cpallsItemLayout, item.ID);
                }
            }
            if (sb.Length > 10)
                _adeBusiness.ExecuteSql(sb.ToString());


            //Expressive item
            List<ItemModel> expressiveitems =
                _adeBusiness.GetItemModels(r => r.Type == ItemType.TxkeaExpressive).ToList();
            StringBuilder sb_expressive = new StringBuilder();
            foreach (ItemModel item in expressiveitems)
            {
                string itemLayout = ((TxkeaExpressiveItemModel)item).ItemLayout;
                if (!string.IsNullOrEmpty(itemLayout))
                {
                    var jsonLayout = JsonHelper.DeserializeObject<dynamic>(itemLayout);
                    var cpallsItemLayout = "{";
                    cpallsItemLayout += "\"objects\":[";
                    for (int i = 0; i < jsonLayout["objects"].Count; i++)
                    {
                        var jsonObject = JsonHelper.DeserializeObject<dynamic>(jsonLayout["objects"][i].ToString());
                        cpallsItemLayout += "{";
                        cpallsItemLayout += "\"id\":" + jsonObject["id"] + ",\"src\":\"" + jsonObject["src"] + "\",";
                        cpallsItemLayout += "\"top\":" + jsonObject["top"] + ",\"left\":" + jsonObject["left"] + ",";
                        cpallsItemLayout += "\"height\":" + jsonObject["height"] * jsonObject["scaleY"] + ",\"width\":" + jsonObject["width"] * jsonObject["scaleX"];
                        cpallsItemLayout += "},";
                    }
                    cpallsItemLayout = cpallsItemLayout.TrimEnd(',');
                    cpallsItemLayout += "]";
                    if (jsonLayout["background"] != null && !string.IsNullOrEmpty(jsonLayout["background"].ToString()))
                        cpallsItemLayout += ",\"background\":\"" + jsonLayout["background"] + "\"";
                    if (jsonLayout["backgroundImage"] != null && !string.IsNullOrEmpty(jsonLayout["backgroundImage"].ToString()))
                        cpallsItemLayout += ",\"backgroundImage\":{\"src\":\"" + jsonLayout["backgroundImage"]["src"] + "\"}";
                    cpallsItemLayout += "}";

                    sb_expressive.AppendFormat("update TxkeaExpressiveItems set CpallsItemLayout='{0}' where ID={1};", cpallsItemLayout, item.ID);
                }
            }
            if (sb_expressive.Length > 10)
                _adeBusiness.ExecuteSql(sb_expressive.ToString());

            return "operation:success";
        }


        #region
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        [CLIUrlAuthorizeAttribute(Account = Authority.All, PageId = PagesModel.ADE, Anonymity = Anonymous.Verified)]
        public string SaveObservableChoice(ObservableChoiceModel model, string answers)
        {
            return Save(model, answers);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        [CLIUrlAuthorizeAttribute(Account = Authority.All, PageId = PagesModel.ADE, Anonymity = Anonymous.Verified)]
        public string SaveObservableEntry(ObservableEntryModel model)
        {
            var response = new PostFormResponse();
            model.CreatedBy = UserInfo.ID;
            model.UpdatedBy = UserInfo.ID;
            var result = _adeBusiness.SaveItem(model);
            response.Update(result);
            return JsonHelper.SerializeObject(response);
        }

        #endregion
    }
}