using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Sunnet.Cli.Core.Schools.Entities;
using Sunnet.Cli.Core.Schools.Enums;
using Sunnet.Cli.Business.Common;
using Sunnet.Cli.Core.Trs;
using Sunnet.Cli.Core.Users.Entities;
using Newtonsoft.Json;
using System.ComponentModel;
using Sunnet.Cli.Core.Trs.Entities;
using Sunnet.Framework.Extensions;
using Sunnet.Cli.Core.Trs.Enums;
using System.Collections.ObjectModel;

namespace Sunnet.Cli.Business.Trs.Models
{
    public class TrsResultReportModel : TrsAssessmentModelBase
    {
        public new TrsSchoolReportModel School { get; set; }

        public new List<TrsClassReportModel> Classes { get; set; }

        public IEnumerable<TrsCategoryStarModel> CategoryStars { get; set; }

        public Dictionary<int, TrsClassReportModel> ClassDic
        {
            get
            {
                var class_list = new Dictionary<int, TrsClassReportModel>();
                if (Classes != null)
                {
                    class_list = Classes.DistinctBy(a => a.ID).OrderBy(a => a.Name)
                        .ToDictionary(x => x.ID, x => x);
                }
                return class_list;
            }
        }

        [JsonIgnore]
        public new IEnumerable<TrsItemReportModel> Items { get; set; }

        public Dictionary<TRSCategoryEnum, Dictionary<int, List<TrsItemReportModel>>> StructuralCategories { get; protected set; }

        public Dictionary<TRSCategoryEnum, Dictionary<int, List<TrsItemReportModel>>> ProcessCategories { get; protected set; }

        public DateTime EffectiveDate
        {
            get { return ApproveDate.EnsureMinDate(); }
        }

        public DateTime TerminationDate
        {
            get
            {
                return RecertificatedBy;
            }
        }

        public DateTime ApprovalOnDate
        {
            get { return ApproveDate.EnsureMinDate(); }
        }

        public DateTime ExpireOnDate
        {
            get { return RecertificatedBy; }
        }

        public string MinAgeGroup { get; set; }

        public string MaxAgeGroup { get; set; }


        internal override void Prepare(bool showNAItem = false)
        {
            IEnumerable<TrsItemReportModel> struturalItems = this.Items.Where(a => a.ItemType == TRSItemType.Structural & a.SubcategoryType == TRSItemType.Structural)
                .Where(School.ShouldVisible);
            List<TRSCategoryEnum> categories_structural = struturalItems.Select(x => x.Category).Distinct().ToList();


            //添加StructuralCategory

            StructuralCategories = new Dictionary<TRSCategoryEnum, Dictionary<int, List<TrsItemReportModel>>>();
            List<TRSCategoryEnum> categoriesStructural = new List<TRSCategoryEnum>();

            //添加Category
            Type enumType = typeof(TRSCategoryEnum);
            Array enumArrays = Enum.GetValues(enumType);
            for (int i = 0; i < enumArrays.Length; i++)
            {
                TRSCategoryEnum category = (TRSCategoryEnum)enumArrays.GetValue(i);
                if (categories_structural.Contains(category))
                {
                    categoriesStructural.Add(category);
                }
            }


            foreach (TRSCategoryEnum category in categoriesStructural)
            {
                List<TrsItemReportModel> items = struturalItems.Where(x => x.Id > 0 && x.AnswerId > 0 && x.Category == category).OrderBy(o => o.ItemId).ThenBy(o => o.Score).ToList();
                List<TrsItemReportModel> items_distinct = items.DistinctBy(x => x.ItemId).OrderBy(i => i.SubCategorySort).ThenBy(i => i.Sort).ToList();
                if (category != TRSCategoryEnum.Category1 && category != TRSCategoryEnum.Category5)
                {
                    //添加item.ClassItems
                    foreach (TrsItemReportModel item in items_distinct)
                    {
                        item.ClassItems = new List<TrsClassItemModel>();
                        foreach (var item_class in ClassDic)
                        {
                            TrsClassReportModel classmodel = item_class.Value;
                            TrsItemReportModel model = items.Where(a => a.ItemId == item.ItemId && a.ClassId == classmodel.ID)
                                .Where(classmodel.ShouldVisible).FirstOrDefault();
                            bool isHaveItem = model == null;//class是否有该item
                            TrsClassItemModel classmodel_add = new TrsClassItemModel { ClassId = classmodel.ID, ClassName = classmodel.Name };
                            if (isHaveItem)
                            {
                                classmodel_add.AnswerId = 0;
                                classmodel_add.Comments = "";
                                classmodel_add.AgeGroup = 0;
                                classmodel_add.CaregiversNo = 0;
                                classmodel_add.GroupSize = 0;
                            }
                            else
                            {
                                classmodel_add.AnswerId = model.AnswerId;
                                classmodel_add.AnswerText = model.AnswerText;
                                classmodel_add.Comments = model.Comments;
                                classmodel_add.AgeGroup = model.AgeGroup;
                                classmodel_add.CaregiversNo = model.CaregiversNo;
                                classmodel_add.GroupSize = model.GroupSize;
                            }
                            item.ClassItems.Add(classmodel_add);
                        }
                        if (item.ClassItems.Count(r => r.AnswerId > 0) <= 0 && !showNAItem)  //若class对item都不可见，则不显示
                        {
                            items_distinct = items_distinct.Where(r => r.Id != item.Id).ToList();
                        }
                    }
                }
                IEnumerable<IGrouping<int, TrsItemReportModel>> groups = items_distinct.OrderBy(i => i.SubCategorySort).ThenBy(i => i.Sort)
                    .GroupBy(x => x.SubCategoryId, x => x);
                StructuralCategories.Add(category, groups.ToDictionary(x => x.Key, x => x.ToList()));
            }



            //添加ProcessCategory

            IEnumerable<TrsItemReportModel> processItems = this.Items.Where(a => a.ItemType == TRSItemType.Process & a.SubcategoryType == TRSItemType.Process)
                .Where(School.ShouldVisible);
            List<TRSCategoryEnum> categories_process = processItems.Select(x => x.Category).Distinct().ToList();


            ProcessCategories = new Dictionary<TRSCategoryEnum, Dictionary<int, List<TrsItemReportModel>>>();
            List<TRSCategoryEnum> categoriesProcess = new List<TRSCategoryEnum>();

            //添加Category
            for (int i = 0; i < enumArrays.Length; i++)
            {
                TRSCategoryEnum category = (TRSCategoryEnum)enumArrays.GetValue(i);
                if (categories_process.Contains(category))
                {
                    categoriesProcess.Add(category);
                }
            }

            foreach (TRSCategoryEnum category in categoriesProcess)
            {
                List<TrsItemReportModel> items = processItems.Where(x => x.Id > 0 && x.AnswerId > 0 && x.Category == category).ToList();
                List<TrsItemReportModel> items_distinct = items.DistinctBy(x => x.ItemId).OrderBy(i => i.SubCategorySort).ThenBy(i => i.Sort).ToList();

                if (category != TRSCategoryEnum.Category1 && category != TRSCategoryEnum.Category5)
                {
                    //添加item.ClassItems
                    foreach (TrsItemReportModel item in items_distinct)
                    {
                        item.ClassItems = new List<TrsClassItemModel>();
                        foreach (var item_class in ClassDic)
                        {
                            TrsClassReportModel classmodel = item_class.Value;
                            TrsItemReportModel model = items.Where(a => a.ItemId == item.ItemId && a.ClassId == classmodel.ID)
                                .Where(classmodel.ShouldVisible).FirstOrDefault();
                            bool isHaveItem = model == null;//class是否有该item
                            TrsClassItemModel classmodel_add = new TrsClassItemModel { ClassId = classmodel.ID, ClassName = classmodel.Name };
                            classmodel_add.Score = !isHaveItem ? model.Score : -1;
                            classmodel_add.Comments = !isHaveItem ? model.Comments : "";
                            classmodel_add.AgeGroup = !isHaveItem ? model.AgeGroup : 0;
                            classmodel_add.CaregiversNo = !isHaveItem ? model.CaregiversNo : 0;
                            classmodel_add.GroupSize = !isHaveItem ? model.GroupSize : 0;
                            item.ClassItems.Add(classmodel_add);
                        }
                        if (item.ClassItems.Count(r => r.Score > -1) <= 0 && !showNAItem)  //若class对item都不可见，则不显示
                        {
                            items_distinct = items_distinct.Where(r => r.Id != item.Id).ToList();
                        }
                    }
                }
                IEnumerable<IGrouping<int, TrsItemReportModel>> groups = items_distinct.OrderBy(i => i.SubCategorySort).ThenBy(i => i.Sort)
                    .GroupBy(x => x.SubCategoryId, x => x);
                ProcessCategories.Add(category, groups.ToDictionary(x => x.Key, x => x.ToList()));
            }

        }
    }
}
