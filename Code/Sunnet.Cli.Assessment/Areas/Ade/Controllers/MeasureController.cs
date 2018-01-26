using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using Sunnet.Cli.Business.Ade;
using Sunnet.Cli.Business.Ade.Models;
using Sunnet.Cli.Business.Users;
using Sunnet.Cli.Core.Ade;
using Sunnet.Cli.Core.Ade.Entities;
using Sunnet.Cli.Core.Users.Entities;
using Sunnet.Cli.UIBase.Controllers;
using Sunnet.Cli.UIBase.Models;
using Sunnet.Framework.Core.Tool;
using Sunnet.Framework.Extensions;
using Sunnet.Cli.Assessment.Controllers;
using Sunnet.Cli.UIBase.Filters;
using Sunnet.Framework.Helpers;
using Sunnet.Framework.Permission;
using Sunnet.Cli.Core.Cpalls;
using Sunnet.Cli.Core.Common.Enums;

namespace Sunnet.Cli.Assessment.Areas.Ade.Controllers
{
    public class MeasureController : BaseController
    {
        private AdeBusiness _adeBusiness;
        private UserBusiness _userBuss = new UserBusiness();
        public MeasureController()
        {
            _adeBusiness = new AdeBusiness(AdeUnitWorkContext);
        }



        [CLIUrlAuthorizeAttribute(Account = Authority.All, PageId = PagesModel.ADE, Anonymity = Anonymous.Verified)]
        public ActionResult New(int assessmentId)
        {
            AssessmentEntity assessmentEntity = _adeBusiness.GetAssessment(assessmentId);
            if (assessmentEntity == null) return new EmptyResult();

            ViewBag.AssessmentId = assessmentId;
            var parents = _adeBusiness.GetParentMeasuresForAssessment(assessmentId);
            ViewBag.Parents = parents.Select(x => new SelectListItem()
            {
                Text = x.NamePrefix + x.Label,
                Value = x.ID.ToString()
            });

            ViewBag.DefaultData = new List<SelectListItem>
            {
                new SelectListItem()
                {
                    Text = ViewTextHelper.DefaultPleaseSelectText,
                    Value = "0"
                }
            };
            AssessmentEntity otherAssessment = _adeBusiness.GetTheOtherLanguageAssessment(assessmentId);
            if (otherAssessment == null)
            {
                ViewBag.ParentData = JsonConvert.SerializeObject(false);
                ViewBag.ChildData = JsonConvert.SerializeObject(false);
            }
            else
            {
                List<RelateModel> parenList = _adeBusiness.GetMeasuresForAssessment(true, otherAssessment.ID);
                List<RelateModel> childList = _adeBusiness.GetMeasuresForAssessment(false, otherAssessment.ID);
                ViewBag.ParentData = JsonConvert.SerializeObject(parenList);
                ViewBag.ChildData = JsonConvert.SerializeObject(childList);
            }

            var model = new MeasureModel();
            model.ApplyToWave = new List<Core.Cpalls.Wave>();
            model.ApplyToWave.Add(Wave.BOY);
            model.ApplyToWave.Add(Wave.EOY);
            model.ApplyToWave.Add(Wave.MOY);
            if (assessmentEntity.Type == AssessmentType.Cpalls)
            {
                ViewBag.Benchmarks = JsonHelper.SerializeObject(_adeBusiness.GetBenchmarksForSelect(assessmentId));
                model.AssessmentId = assessmentId;
                model.AssessmentLabel = assessmentEntity.Label;
                return View(model);
            }
            else if (assessmentEntity.Type == AssessmentType.Cec)
            {
                return View("New_CEC", model);
            }
            else
            {
                return View("New_COT", model);
            }
        }


        [CLIUrlAuthorizeAttribute(Account = Authority.All, PageId = PagesModel.ADE, Anonymity = Anonymous.Verified)]
        public ActionResult Edit(int id)
        {
            var model = _adeBusiness.GetMeasureModel(id);

            ViewBag.IsCEC = model.AssessmentType == AssessmentType.Cec;

            List<MeasureModel> parents = new List<MeasureModel>();
            ///一级Measure ，且有子Measure的，不能修改为 子Measure
            if (model.ParentId == 1 && model.SubMeasureCount > 0)
            {
                parents.Add(new MeasureModel() { ID = 1, Label = "None" });
            }
            else
            {
                parents = _adeBusiness.GetParentMeasuresForAssessment(model.AssessmentId).Where(x => x.ID != id).ToList();
            }
            ViewBag.Parents = parents.Select(x => new SelectListItem()
            {
                Text = x.NamePrefix + x.Label,
                Value = x.ID.ToString()
            });
            model.Parent = parents.First(x => x.ID == model.ParentId);
            ViewBag.Scores = JsonHelper.SerializeObject(model.CutOffScores);

            ViewBag.Benchmarks = JsonHelper.SerializeObject(_adeBusiness.GetBenchmarksForSelect(model.AssessmentId));

            if (model.AssessmentType == AssessmentType.Cec || model.AssessmentType == AssessmentType.Cot)
            {
                ViewBag.Scores = "";
            }
            ViewBag.ParentMeasure = model.ParentId == 1 ? 1 : 0;

            if (model.Locked)
            {
                string relatedMeasureName = _adeBusiness.GetRelatedMeasureName(model.AssessmentId, model.RelatedMeasureId);
                if (!string.IsNullOrEmpty(relatedMeasureName))
                {
                    ViewBag.RelatedMeasureName = relatedMeasureName;
                }
                if (model.AssessmentType == AssessmentType.Cpalls)
                {
                    return View("View", model);
                }
                else if (model.AssessmentType == AssessmentType.Cec)
                {
                    return View("View_CEC", model);
                }
                else
                {
                    return View("View_COT", model);
                }
            }

            AssessmentEntity otherAssessment = _adeBusiness.GetTheOtherLanguageAssessment(model.AssessmentId);
            ViewBag.RelatedMeasureId = model.RelatedMeasureId;
            if (otherAssessment != null)
            {
                List<RelateModel> parenList = _adeBusiness.GetMeasuresForAssessment(true, otherAssessment.ID,
                    model.RelatedMeasureId);
                List<RelateModel> childList = _adeBusiness.GetMeasuresForAssessment(false, otherAssessment.ID,
                    model.RelatedMeasureId);

                if (model.ParentId == 1)
                {
                    List<SelectItemModel> selectParentList = parenList.Select(r => new SelectItemModel() { ID = r.ID, Name = r.Name }).ToList();
                    ViewBag.DefaultData = selectParentList.ToSelectList(ViewTextHelper.DefaultPleaseSelectText, "0");
                }
                else
                {
                    List<SelectItemModel> selectChildList = childList.Where(r => r.ParentRelationId == model.ParentId).Select(r => new SelectItemModel() { ID = r.ID, Name = r.Name }).ToList();
                    ViewBag.DefaultData = selectChildList.ToSelectList(ViewTextHelper.DefaultPleaseSelectText, "0");
                }
                ViewBag.ParentData =
                    JsonConvert.SerializeObject(parenList);
                ViewBag.ChildData =
                    JsonConvert.SerializeObject(childList);
            }
            else
            {
                ViewBag.ParentData = JsonConvert.SerializeObject(false);
                ViewBag.ChildData = JsonConvert.SerializeObject(false);
            }
            if (model.AssessmentType == AssessmentType.Cpalls)
            {
                return View(model);
            }
            else if (model.AssessmentType == AssessmentType.Cec)
            {
                return View("Edit_CEC", model);
            }
            else
            {
                return View("Edit_COT", model);
            }
        }


        [CLIUrlAuthorizeAttribute(Account = Authority.All, PageId = PagesModel.ADE, Anonymity = Anonymous.Verified)]
        public ActionResult Detail(int id)
        {
            var model = _adeBusiness.GetMeasureModel(id);
            if (model == null)
                return View("404");
            ViewBag.IsCEC = model.AssessmentType == AssessmentType.Cec;
            ViewBag.IsCpalls = model.AssessmentType == AssessmentType.Cpalls;
            ViewBag.Observable = model.AssessmentType == AssessmentType.UpdateObservables;
            model.Parent = _adeBusiness.GetMeasureModel(model.ParentId);
            return View(model);
        }


        [CLIUrlAuthorizeAttribute(Account = Authority.All, PageId = PagesModel.ADE, Anonymity = Anonymous.Verified)]
        public ActionResult AdjustOrder(int assessmentId, int parentId)
        {
            ViewBag.AssessmentId = assessmentId;
            ViewBag.ParentId = parentId;
            var assessment = _adeBusiness.GetAssessment(assessmentId);
            var list = _adeBusiness.GetMeasuresForAdjustOrder(assessmentId, parentId);

            return View(list);
        }


        [CLIUrlAuthorizeAttribute(Account = Authority.All, PageId = PagesModel.ADE, Anonymity = Anonymous.Verified)]
        public ActionResult CutoffScores(int id)
        {
            var model = _adeBusiness.GetMeasureModel(id);
            ViewBag.Scores = JsonHelper.SerializeObject(model.CutOffScores);
            return View(model);
        }


        [CLIUrlAuthorizeAttribute(Account = Authority.All, PageId = PagesModel.ADE, Anonymity = Anonymous.Verified)]
        public ActionResult Links(int id)
        {
            var model = _adeBusiness.GetMeasureModel(id);
            ViewBag.Links = JsonHelper.SerializeObject(model.Links.Where(e => e.LinkType == 0));
            if (model.Locked)
                return View("Links_View", model);
            return View(model);
        }

        [CLIUrlAuthorizeAttribute(Account = Authority.All, PageId = PagesModel.ADE, Anonymity = Anonymous.Verified)]
        public ActionResult ParentActivities(int id)
        {
            var model = _adeBusiness.GetMeasureModel(id);
            ViewBag.ParentActivities = JsonHelper.SerializeObject(model.Links.Where(e => e.LinkType == 1));
            ViewBag.StatusOptions = EntityStatus.Active.ToSelectList(true);

            if (model.Locked)
                return View("ParentActivities_View", model);
            return View(model);
        }


        [HttpPost]
        [CLIUrlAuthorizeAttribute(Account = Authority.All, PageId = PagesModel.ADE, Anonymity = Anonymous.Verified)]
        public string Delete(int id)
        {
            var response = new PostFormResponse();
            var result = _adeBusiness.DeleteMeasure(id);
            response.Success = result.ResultType == OperationResultType.Success;
            response.Message = result.Message;
            return JsonHelper.SerializeObject(response);
        }


        [HttpPost]
        [CLIUrlAuthorizeAttribute(Account = Authority.All, PageId = PagesModel.ADE, Anonymity = Anonymous.Verified)]
        public string ChangeItemStatus(int id)
        {
            var response = new PostFormResponse();
            var result = new OperationResult(OperationResultType.Success);
            var item = _adeBusiness.GetItemModel(id);
            if (item.Type == ItemType.ObservableChoice)
            {
                ObservableChoiceModel item1 = (ObservableChoiceModel)item;
                item1.IsShown = !item1.IsShown;
                result = _adeBusiness.SaveItem(item1);
            }
            if (item.Type == ItemType.ObservableResponse)
            {
                ObservableEntryModel item2 = (ObservableEntryModel)item;
                item2.IsShown = !item2.IsShown;
                result = _adeBusiness.SaveItem(item2);
            }

            response.Success = result.ResultType == OperationResultType.Success;
            response.Message = result.Message;
            return JsonHelper.SerializeObject(response);
        }

        private class CompareCutoffScore : IEqualityComparer<CutOffScoreEntity>
        {
            public bool Equals(CutOffScoreEntity x, CutOffScoreEntity y)
            {
                return x.FromYear == y.FromYear && x.FromMonth == y.FromMonth
                       && x.ToYear == y.ToYear && x.ToMonth == y.ToMonth
                       && x.Wave == y.Wave
                       && x.BenchmarkId == y.BenchmarkId
                       && x.LowerScore == y.LowerScore
                       && x.HigherScore == y.HigherScore;
            }

            public int GetHashCode(CutOffScoreEntity obj)
            {
                var score = new { obj.FromYear, obj.FromMonth, obj.ToYear, obj.ToMonth, obj.Wave, obj.CutOffScore, obj.BenchmarkId, obj.LowerScore, obj.HigherScore };
                return score.GetHashCode();
            }
        }

        [HttpPost]
        [CLIUrlAuthorizeAttribute(Account = Authority.All, PageId = PagesModel.ADE, Anonymity = Anonymous.Verified)]
        public string Save(MeasureModel model, string scores = null)
        {
            var response = new PostFormResponse() { Success = ModelState.IsValid };
            model.CreatedBy = UserInfo.ID;
            model.UpdatedBy = UserInfo.ID;
            model.Description = Server.UrlDecode(model.Description);
            if (model.Description == null)
                model.Description = "";
            if (model.StartPageHtml != null)
            {
                model.StartPageHtml = model.StartPageHtml.Trim();
            }
            else
                model.StartPageHtml = string.Empty;

            if (model.EndPageHtml != null)
            {
                model.EndPageHtml = model.EndPageHtml.Trim();
            }
            else
                model.EndPageHtml = string.Empty;

            if (model.AssessmentType == AssessmentType.Cec || model.AssessmentType == AssessmentType.Cot)
            {
                scores = "";
            }
            if (!string.IsNullOrEmpty(scores))
            {
                var scoreEntities = JsonHelper.DeserializeObject<List<CutOffScoreEntity>>(scores);
                model.CutOffScores = scoreEntities.OrderByDescending(x => x.ID).Distinct(new CompareCutoffScore()).ToList();
                model.HasCutOffScores = model.CutOffScores.Any(c => !(c.FromYear == 0 && c.FromMonth == 0 && c.ToYear == 0 && c.ToMonth == 0 && c.CutOffScore == (decimal)0.00));
                model.BOYHasCutOffScores = model.CutOffScores.Any(c => !(c.FromYear == 0 && c.FromMonth == 0 && c.ToYear == 0 && c.ToMonth == 0 && c.CutOffScore == (decimal)0.00) && c.Wave == Wave.BOY);
                model.MOYHasCutOffScores = model.CutOffScores.Any(c => !(c.FromYear == 0 && c.FromMonth == 0 && c.ToYear == 0 && c.ToMonth == 0 && c.CutOffScore == (decimal)0.00) && c.Wave == Wave.MOY);
                model.EOYHasCutOffScores = model.CutOffScores.Any(c => !(c.FromYear == 0 && c.FromMonth == 0 && c.ToYear == 0 && c.ToMonth == 0 && c.CutOffScore == (decimal)0.00) && c.Wave == Wave.EOY);
            }
            else
            {
                model.HasCutOffScores = false;
                model.BOYHasCutOffScores = false;
                model.MOYHasCutOffScores = false;
                model.EOYHasCutOffScores = false;

            }
            if (response.Success && model.ApplyToWave == null || !model.ApplyToWave.Any())
            {
                ModelState.AddModelError("ApplyToWave", "The Apply to Wave field is required");
                response.Success = false;
            }
            if (response.Success)
            {
                var result = _adeBusiness.SaveMeasure(model);
                response.Success = result.ResultType == OperationResultType.Success;
                response.Data = model;
                response.Message = result.Message;
            }

            response.ModelState = ModelState;
            return JsonHelper.SerializeObject(response);
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        [CLIUrlAuthorizeAttribute(Account = Authority.All, PageId = PagesModel.ADE, Anonymity = Anonymous.Verified)]
        public string AdjustOrder(int assessmentId, int parentId, List<int> measureIds)
        {
            var response = new PostFormResponse();
            response.Update(_adeBusiness.AdjustMeasures(assessmentId, measureIds));
            return JsonHelper.SerializeObject(response);
        }


        [System.Web.Mvc.HttpPost]
        [ValidateAntiForgeryToken]
        [CLIUrlAuthorizeAttribute(Account = Authority.All, PageId = PagesModel.ADE, Anonymity = Anonymous.Verified)]
        public string CutoffScores(int assessmentId, string scores)
        {
            var scoreEntities = JsonHelper.DeserializeObject<List<CutOffScoreEntity>>(scores);
            var response = new PostFormResponse();
            if (ModelState.IsValid)
            {
                var result = _adeBusiness.UpdateCutoffScores<AssessmentEntity>(assessmentId, scoreEntities);
                response.Update(result);
            }
            response.ModelState = ModelState;
            return JsonHelper.SerializeObject(response);
        }



        [System.Web.Mvc.HttpPost]
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
                            if (tmpS.Length == 5)
                            {
                                if (tmpS[0].Trim() != string.Empty)
                                {
                                    list.Add(new AdeLinkEntity()
                                    {
                                        Link = tmpS[0],
                                        LinkType = 0,
                                        DisplayText = tmpS[1],
                                        Status = EntityStatus.Active,
                                        MeasureWave1 = tmpS[2] == "true",
                                        MeasureWave2 = tmpS[3] == "true",
                                        MeasureWave3 = tmpS[4] == "true"
                                    });
                                }
                            }
                        }
                    }
                }
            }
            if (list.Count > 0)
            {
                var result = _adeBusiness.UpdateLinks<MeasureEntity>(id, list);
                response.Update(result);
            }
            else
                response.Success = true;

            return JsonHelper.SerializeObject(response);
        }

        [System.Web.Mvc.HttpPost]
        [ValidateAntiForgeryToken]
        [CLIUrlAuthorizeAttribute(Account = Authority.All, PageId = PagesModel.ADE, Anonymity = Anonymous.Verified)]
        public string ParentActivities(int id, string activities)
        {
            var response = new PostFormResponse();
            List<AdeLinkEntity> list = new List<AdeLinkEntity>();
            if (activities.Trim() != string.Empty)
            {
                string[] tmpLinks = activities.Split(';');
                if (tmpLinks.Length > 0)
                {
                    foreach (string s in tmpLinks)
                    {
                        if (s.Trim() != string.Empty)
                        {
                            string[] tmpS = s.Split('|');
                            if (tmpS.Length == 8)
                            {
                                if (tmpS[0].Trim() != string.Empty)
                                {
                                    list.Add(new AdeLinkEntity()
                                    {
                                        Link = tmpS[0],
                                        LinkType = 1,
                                        DisplayText = tmpS[1],
                                        Status = EntityStatus.Active,
                                        MeasureWave1 = tmpS[2] == "true",
                                        MeasureWave2 = tmpS[3] == "true",
                                        MeasureWave3 = tmpS[4] == "true",
                                        StudentWave1 = tmpS[5] == "true",
                                        StudentWave2 = tmpS[6] == "true",
                                        StudentWave3 = tmpS[7] == "true"
                                    });
                                }
                            }
                        }
                    }
                }
            }
            if (list.Count > 0)
            {
                var result = _adeBusiness.UpdateLinks<MeasureEntity>(id, list);
                response.Update(result);
            }
            else
                response.Success = true;

            return JsonHelper.SerializeObject(response);
        }


        [CLIUrlAuthorizeAttribute(Account = Authority.All, PageId = PagesModel.ADE, Anonymity = Anonymous.Verified)]
        public string Search(int assessmentId, string name, string label, string createdBy, int status = -1, string sort = "Sort", string order = "asc")
        {
            var users = _userBuss.SearchUserIds(createdBy);
            var total = 0;
            var list = _adeBusiness.SearchMeasures(assessmentId,
                x => x.IsDeleted == false
                    && x.Name.IndexOf(name.Trim(), StringComparison.CurrentCultureIgnoreCase) >= 0
                    && x.Label.IndexOf(label.Trim(), StringComparison.CurrentCultureIgnoreCase) >= 0
                    && (users == null || users.Contains(x.CreatedBy))
                    && (status < 0 || (int)x.Status == status), out total, sort, order);
            var result = new { total = total, data = list };
            return JsonHelper.SerializeObject(result);
        }

        [CLIUrlAuthorizeAttribute(Account = Authority.All, PageId = PagesModel.ADE, Anonymity = Anonymous.Verified)]
        public string GetMeasures(int assessmentId)
        {
            return JsonHelper.SerializeObject(_adeBusiness.GetMeasures(assessmentId)
                .ToSelectList(ViewTextHelper.DefaultAllText, "-1"));
        }
    }
}