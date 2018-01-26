using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/**************************************************************************
 * Developer: 		JackZhang
 * Computer:		JACKZ
 * Domain:			JackZ
 * CreatedOn:		1/13 2015 9:29:04
 * Description:		Please input class summary
 * Version History:	Created,1/13 2015 9:29:04
 * 
 * 
 **************************************************************************/
using System.Xml;
using DocumentFormat.OpenXml.Office2010.ExcelAc;
using LinqKit;
using Newtonsoft.Json;
using Sunnet.Cli.Business.Common;
using Sunnet.Cli.Core.Schools.Enums;
using Sunnet.Cli.Core.Trs;
using Sunnet.Cli.Core.Trs.Enums;
using Sunnet.Framework.Extensions;
using Sunnet.Cli.Core.Users.Enums;
using Sunnet.Cli.Core.Users.Entities;

namespace Sunnet.Cli.Business.Trs.Models
{
    public class TrsAssessmentModel : TrsAssessmentModelBase
    {
        public TrsAssessmentModel()
        {
            Status = TRSStatusEnum.Initialized;
            Type = 0;
        }

        private IEnumerable<TrsItemModel> _items;

        [JsonIgnore]
        public new IEnumerable<TrsItemModel> Items
        {
            get { return _items ?? (_items = new List<TrsItemModel>()); }
            set { _items = value; }
        }

        private IEnumerable<TrsAssessmentClassModel> _assessmentClasses;

        [JsonIgnore]
        public IEnumerable<TrsAssessmentClassModel> AssessmentClasses
        {
            get { return _assessmentClasses ?? (_assessmentClasses = new List<TrsAssessmentClassModel>()); }
            set { _assessmentClasses = value; }
        }

        private Dictionary<TRSCategoryEnum, Dictionary<int, List<TrsItemModel>>> _categories;

        public new Dictionary<TRSCategoryEnum, Dictionary<int, List<TrsItemModel>>> Categories
        {
            get { return _categories ?? (_categories = new Dictionary<TRSCategoryEnum, Dictionary<int, List<TrsItemModel>>>()); }
        }

        private Dictionary<TRSCategoryEnum, Dictionary<int, List<TrsOfflineItemModel>>> _categories_offline;

        public new Dictionary<TRSCategoryEnum, Dictionary<int, List<TrsOfflineItemModel>>> Categories_Offline
        {
            get { return _categories_offline ?? (_categories_offline = new Dictionary<TRSCategoryEnum, Dictionary<int, List<TrsOfflineItemModel>>>()); }
        }

        public Dictionary<TRSCategoryEnum, string> Category { get; set; }

        internal override void Prepare(bool showNAItem = false)
        {

        }

        public void PrepareWithAccess(UserBaseEntity user)
        {
            var categories = this.Items.Select(x => x.Category).Distinct().ToList();
            Category = categories.ToDictionary(x => x, x => x.GetDisplayName());

            _categories = new Dictionary<TRSCategoryEnum, Dictionary<int, List<TrsItemModel>>>();

            var categoriesForSchool = new List<TRSCategoryEnum>();
            if (categories.Contains(TRSCategoryEnum.Category1))
                categoriesForSchool.Add(TRSCategoryEnum.Category1);
            if (categories.Contains(TRSCategoryEnum.Category5))
                categoriesForSchool.Add(TRSCategoryEnum.Category5);

            categoriesForSchool.ForEach(category =>
            {
                var items = this.Items.Where(x => x.Id > 0 && x.Category == category).Where(School.ShouldVisible).ToList(); // 已填写
                items.AddRange(this.Items.Where(x => x.Id == 0 && x.Category == category).Where(School.ShouldVisible));// 未填写
                items = items.DistinctBy(x => x.ItemId).OrderBy(i => i.SubCategorySort).ThenBy(i => i.Sort).ToList();
                IEnumerable<IGrouping<int, TrsItemModel>> groups = items.OrderBy(i => i.SubCategorySort).ThenBy(i => i.Sort)
                    .GroupBy(x => x.SubCategoryId, x => x);
                if (user.ID > 0)
                {
                    bool ifCanAccess = this.School.Assessor.UserId == user.ID || user.Role == Role.Super_admin;
                    foreach (TrsItemModel item in items)
                    {
                        item.IfCanAccess = ifCanAccess;
                    }
                }
                _categories.Add(category, groups.ToDictionary(x => x.Key, x => x.ToList()));
            });

            var categories2 = new List<TRSCategoryEnum>();

            if (categories.Contains(TRSCategoryEnum.Category2))
                categories2.Add(TRSCategoryEnum.Category2);
            if (categories.Contains(TRSCategoryEnum.Category3))
                categories2.Add(TRSCategoryEnum.Category3);
            if (categories.Contains(TRSCategoryEnum.Category4))
                categories2.Add(TRSCategoryEnum.Category4);

            categories2.ForEach(category => this.Classes.ForEach(class1 =>
            {
                var itemsOther = this.Items.Where(x => x.Id > 0 && x.ClassId == class1.Id && x.Category == category)
                    .Where(School.ShouldVisible).Where(class1.ShouldVisible).ToList(); //已填写
                itemsOther.AddRange(this.Items.Where(x => x.Id == 0
                                                          && x.ClassId == 0
                                                          && x.Category == category
                                                          && itemsOther.Select(i => i.ItemId).Contains(x.ItemId) == false)
                    .Where(School.ShouldVisible).Where(class1.ShouldVisible)); //未填写
                itemsOther =
                    itemsOther.DistinctBy(x => x.ItemId).OrderBy(i => i.SubCategorySort).ThenBy(i => i.Sort).ToList();

                List<TrsItemModel> items_new = new List<TrsItemModel>();    //若是新加时，无法区分数据，需重新实例化并通过ClassId区分
                foreach (TrsItemModel item in itemsOther.Where(r => r.ClassId == 0))
                {
                    TrsItemModel model = new TrsItemModel();
                    model.Description = item.Description;
                    model.KeyBehavior = item.KeyBehavior;
                    model.IsRequired = item.IsRequired;
                    model.Item = item.Item;
                    model.ItemId = item.ItemId;
                    model.LinkingDocument = item.LinkingDocument;
                    model.Score = -1;
                    model.SubCategoryId = item.SubCategoryId;
                    model.TAItemInstructions = item.TAItemInstructions;
                    model.TAPlanItem = item.TAPlanItem;
                    model.TAPlanItemType = item.TAPlanItemType;
                    model.Text = item.Text;
                    model.Type = item.Type;
                    model.SyncAnswer = item.SyncAnswer;
                    model.Answers = item.Answers;
                    model.ClassId = class1.Id;
                    items_new.Add(model);
                }

                itemsOther.RemoveAll(r => r.ClassId == 0);
                itemsOther.AddRange(items_new);

                if (user.ID > 0)
                {
                    if (this.School.Assessor.UserId == user.ID || user.Role == Role.Super_admin)
                    {
                        class1.IfCanAccessObservation = true;
                        foreach (TrsItemModel item in itemsOther)
                        {
                            item.IfCanAccess = true;
                        }
                    }
                    else
                    {
                        bool ifCanAccess = class1.TrsAssessorId == user.ID;
                        class1.IfCanAccessObservation = ifCanAccess;
                        foreach (TrsItemModel item in itemsOther)
                        {
                            item.IfCanAccess = ifCanAccess;
                        }
                    }
                }
                class1.Categories.Add(category,
                    itemsOther.GroupBy(x => x.SubCategoryId, x => x).ToDictionary(x => x.Key, x => x.ToList()));
            }));
        }

        //Offline时使用
        public void OfflinePrepareWithAccess(UserBaseEntity user)
        {
            var categories = this.Items.Select(x => x.Category).Distinct().ToList();
            Category = categories.ToDictionary(x => x, x => x.GetDisplayName());

            _categories_offline = new Dictionary<TRSCategoryEnum, Dictionary<int, List<TrsOfflineItemModel>>>();

            var categoriesForSchool = new List<TRSCategoryEnum>();
            if (categories.Contains(TRSCategoryEnum.Category1))
                categoriesForSchool.Add(TRSCategoryEnum.Category1);
            if (categories.Contains(TRSCategoryEnum.Category5))
                categoriesForSchool.Add(TRSCategoryEnum.Category5);

            categoriesForSchool.ForEach(category =>
            {
                var items = this.Items.Where(x => x.Id > 0 && x.Category == category).Where(School.ShouldVisible).ToList(); // 已填写
                items.AddRange(this.Items.Where(x => x.Id == 0 && x.Category == category).Where(School.ShouldVisible));// 未填写
                items = items.DistinctBy(x => x.ItemId).OrderBy(i => i.SubCategorySort).ThenBy(i => i.Sort).ToList();
                if (user.ID > 0)
                {
                    bool ifCanAccess = this.School.Assessor.UserId == user.ID || user.Role == Role.Super_admin;
                    foreach (TrsItemModel item in items)
                    {
                        item.IfCanAccess = ifCanAccess;
                    }
                }
                IEnumerable<IGrouping<int, TrsOfflineItemModel>> groups = items
                    .OrderBy(i => i.SubCategorySort).ThenBy(i => i.Sort)
                    .Select(r => new TrsOfflineItemModel
                    {
                        Id = r.Id,
                        ItemId = r.ItemId,
                        AnswerId = r.AnswerId,
                        ClassId = r.ClassId,
                        Comments = r.Comments,
                        SubCategoryId = r.SubCategoryId,
                        IfCanAccess = r.IfCanAccess
                    })
                    .GroupBy(x => x.SubCategoryId, x => x);
                _categories_offline.Add(category, groups.ToDictionary(x => x.Key, x => x.ToList()));
            });

            var categories2 = new List<TRSCategoryEnum>();

            if (categories.Contains(TRSCategoryEnum.Category2))
                categories2.Add(TRSCategoryEnum.Category2);
            if (categories.Contains(TRSCategoryEnum.Category3))
                categories2.Add(TRSCategoryEnum.Category3);
            if (categories.Contains(TRSCategoryEnum.Category4))
                categories2.Add(TRSCategoryEnum.Category4);

            categories2.ForEach(category => this.Classes.ForEach(class1 =>
            {
                var itemsOther = this.Items.Where(x => x.Id > 0 && x.ClassId == class1.Id && x.Category == category)
                    .Where(School.ShouldVisible).Where(class1.ShouldVisible).ToList(); //已填写
                itemsOther.AddRange(this.Items.Where(x => x.Id == 0
                                                          && x.ClassId == 0
                                                          && x.Category == category
                                                          && itemsOther.Select(i => i.ItemId).Contains(x.ItemId) == false)
                    .Where(School.ShouldVisible).Where(class1.ShouldVisible)); //未填写
                itemsOther =
                    itemsOther.DistinctBy(x => x.ItemId).OrderBy(i => i.SubCategorySort).ThenBy(i => i.Sort).ToList();

                List<TrsItemModel> items_new = new List<TrsItemModel>();    //若是新加时，无法区分数据，需重新实例化并通过ClassId区分
                foreach (TrsItemModel item in itemsOther.Where(r => r.ClassId == 0))
                {
                    TrsItemModel model = new TrsItemModel();
                    model.Description = item.Description;
                    model.KeyBehavior = item.KeyBehavior;
                    model.IsRequired = item.IsRequired;
                    model.Item = item.Item;
                    model.ItemId = item.ItemId;
                    model.LinkingDocument = item.LinkingDocument;
                    model.Score = -1;
                    model.SubCategoryId = item.SubCategoryId;
                    model.TAItemInstructions = item.TAItemInstructions;
                    model.TAPlanItem = item.TAPlanItem;
                    model.TAPlanItemType = item.TAPlanItemType;
                    model.Text = item.Text;
                    model.Type = item.Type;
                    model.SyncAnswer = item.SyncAnswer;
                    model.Answers = item.Answers;
                    model.ClassId = class1.Id;
                    items_new.Add(model);
                }

                itemsOther.RemoveAll(r => r.ClassId == 0);
                itemsOther.AddRange(items_new);

                if (user.ID > 0)
                {
                    if (this.School.Assessor.UserId == user.ID || user.Role == Role.Super_admin)
                    {
                        class1.IfCanAccessObservation = true;
                        foreach (TrsItemModel item in itemsOther)
                        {
                            item.IfCanAccess = true;
                        }
                    }
                    else
                    {
                        bool ifCanAccess = class1.TrsAssessorId == user.ID;
                        class1.IfCanAccessObservation = ifCanAccess;
                        foreach (TrsItemModel item in itemsOther)
                        {
                            item.IfCanAccess = ifCanAccess;
                        }
                    }
                }
                class1.Categories_Offline.Add(category,
                    itemsOther.Select(r => new TrsOfflineItemModel
                    {
                        Id = r.Id,
                        ItemId = r.ItemId,
                        AnswerId = r.AnswerId,
                        ClassId = r.ClassId,
                        Comments = r.Comments,
                        SubCategoryId = r.SubCategoryId,
                        IfCanAccess = r.IfCanAccess
                    }).GroupBy(x => x.SubCategoryId, x => x).ToDictionary(x => x.Key, x => x.ToList()));
            }));
        }

        public void UpdateAction(UserBaseEntity user)
        {
            var isAdmin = user.Role == Role.Super_admin;
            var isAssessor = false;
            var isClassAssessor = false;
            var isClassMentor = false;
            if (user.Role == Role.TRS_Specialist_Delegate)
            {
                int parentId = user.Principal == null ? -1 : user.Principal.ParentId;
                isAssessor = this.School.Assessor.UserId == parentId;
                isClassAssessor = this.Classes.Any(r => r.TrsAssessorId == parentId);
                isClassMentor = this.Classes.Any(r => r.TrsMentorId == parentId);
            }
            else
            {
                isAssessor = this.School.Assessor.UserId == user.ID;
                isClassAssessor = this.Classes.Any(r => r.TrsAssessorId == user.ID);
                isClassAssessor = this.Classes.Any(r => r.TrsMentorId == user.ID);
            }
            if (isClassMentor)
                Action = Status == TRSStatusEnum.Completed ? "" : "viewAssessment";
            if (isClassAssessor)
                Action = Status == TRSStatusEnum.Completed ? "" : "classedit";
            if (isAdmin || isAssessor)
                Action = Status == TRSStatusEnum.Completed ? "invalidate" : "edit";
        }

        public string Action { get; set; }

        /// <summary>
        /// 各Category的星级.
        /// </summary>
        public new Dictionary<TRSCategoryEnum, TRSStarEnum> StarOfCategory { get; private set; }

        /// <summary>
        /// 计算星级: 包括Assessment的星级以及Assessment各Class的星级,更新后的星级通过属性StarOfCategory访问
        /// </summary>
        public void UpdateStar()
        {
            StarOfCategory = new Dictionary<TRSCategoryEnum, TRSStarEnum>();

            List<TRSCategoryEnum> categoryList = new List<TRSCategoryEnum>();
            categoryList.Add(TRSCategoryEnum.Category1);
            categoryList.Add(TRSCategoryEnum.Category2);
            categoryList.Add(TRSCategoryEnum.Category3);
            categoryList.Add(TRSCategoryEnum.Category4);
            categoryList.Add(TRSCategoryEnum.Category5);

            categoryList.ForEach(c =>
            {
                if (!Items.Where(x => x.Category == c &&
                x.Id > 0 && x.Type == TRSItemType.Structural && x.AnswerId > 0 && x.Score >= 0).All(x => x.Score > 0))
                {
                    StarOfCategory.Add(c, TRSStarEnum.One);
                }
            });
            //if (!Items.Where(x => x.Id > 0 && x.Type == TRSItemType.Structural && x.AnswerId > 0 && x.Score >= 0).All(x => x.Score > 0))
            //{
            //    // Structural 没有全部Met, 不需要继续计算
            //    StarOfCategory.Add(TRSCategoryEnum.Category1, TRSStarEnum.One);
            //    StarOfCategory.Add(TRSCategoryEnum.Category2, TRSStarEnum.One);
            //    StarOfCategory.Add(TRSCategoryEnum.Category3, TRSStarEnum.One);
            //    StarOfCategory.Add(TRSCategoryEnum.Category4, TRSStarEnum.One);
            //    StarOfCategory.Add(TRSCategoryEnum.Category5, TRSStarEnum.One);
            //    this.Star = TRSStarEnum.One;
            //    return;
            //}

            var assessmentCategories = new List<TRSCategoryEnum>()
            {
                TRSCategoryEnum.Category1,
                TRSCategoryEnum.Category5
            };
            assessmentCategories.ForEach(category =>
            {
                if (this.Categories.ContainsKey(category) && !StarOfCategory.ContainsKey(category))
                {
                    var items = new List<TrsItemModel>();
                    this.Categories[category].Values.ForEach(subItems =>
                        items.AddRange(subItems.Where(x => x.Id > 0 && x.Type == TRSItemType.Process && x.AnswerId > 0 && x.Score >= 0)));
                    if (items.Any())
                    {
                        var totalScore = items.Sum(x => x.Score);
                        var average = (totalScore + 0m) / items.Count;
                        StarOfCategory.Add(category, StarCalculator.Get(average));
                    }
                    //else
                    //{
                    //    StarOfCategory.Add(category, TRSStarEnum.One);
                    //}
                }
            });

            var classCategories = new List<TRSCategoryEnum>()
            {
                TRSCategoryEnum.Category2,
                TRSCategoryEnum.Category3,
                TRSCategoryEnum.Category4
            };
            classCategories.ForEach(category =>
            {
                if (!StarOfCategory.ContainsKey(category))
                {
                    var scoresOfItem = new Dictionary<int, List<int>>();
                    this.Classes.ForEach(class1 =>
                    {
                        if (class1.Categories.ContainsKey(category))
                        {
                            var items = new List<TrsItemModel>();
                            class1.Categories[category].Values.ForEach(subItems =>
                                items.AddRange(
                                    subItems.Where(x => x.Id > 0 && x.Type == TRSItemType.Process && x.AnswerId > 0 && x.Score >= 0)));
                            if (items.Any())
                            {
                                var totalScore = items.Sum(x => x.Score);
                                var average = (totalScore + 0m) / items.Count;
                                class1.Star = StarCalculator.Get(average);
                                items.ForEach(item =>
                                {
                                    if (!scoresOfItem.ContainsKey(item.ItemId))
                                    {
                                        scoresOfItem.Add(item.ItemId, new List<int>());
                                    }
                                    scoresOfItem[item.ItemId].Add(item.Score);
                                });
                            }
                            else
                            {
                                class1.Star = TRSStarEnum.One;
                            }
                        }
                    });
                    var scoreOfItem = scoresOfItem.ToDictionary(x => x.Key, x => StarCalculator.Median(x.Value));
                    if (scoreOfItem.Any())
                    {
                        var average = scoreOfItem.Sum(x => x.Value) / scoreOfItem.Count;
                        StarOfCategory.Add(category, StarCalculator.Get(average));
                    }
                    //else
                    //{
                    //    StarOfCategory.Add(category, TRSStarEnum.One);
                    //}
                }
            });
            if (StarOfCategory.Any())
            {
                int numOfFour = StarOfCategory.Count(x => x.Value == TRSStarEnum.Four);
                int numOfTwo = StarOfCategory.Count(x => x.Value == TRSStarEnum.Two);
                if (numOfFour == 4 && numOfTwo == 1)
                {
                    this.Star = TRSStarEnum.Three;
                }
                else
                {
                    this.Star = StarOfCategory.Min(x => x.Value);
                }
            }
            else
                this.Star = TRSStarEnum.One;
        }

    }
}
