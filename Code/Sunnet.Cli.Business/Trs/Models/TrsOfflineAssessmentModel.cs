using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/**************************************************************************
 * Developer: 		Joe
 * Computer:		Joe-PC
 * Domain:			Joe-pc
 * CreatedOn:		2015/06/24
 * Description:		Create AuthorityEntity
 * Version History:	Created,2015/06/24
 * 
 * 
 **************************************************************************/
using Sunnet.Cli.Core.Trs;
using Sunnet.Cli.Business.Common;
using Newtonsoft.Json;
using Sunnet.Cli.Core.Users.Entities;
using Sunnet.Framework.Extensions;
using Sunnet.Cli.Core.Users.Enums;

namespace Sunnet.Cli.Business.Trs.Models
{
    public class TrsOfflineAssessmentModel
    {
        private DateTime _approveDate;
        private string _taStatusString;

        public int Id { get; set; }

        public int SchoolId { get; set; }

        public TRSStatusEnum Status { get; set; }

        public TRSStarEnum Star { get; set; }

        public TRSStarEnum VerifiedStar { get; set; }

        public TrsAssessmentType Type { get; set; }

        public DateTime VisitDate { get; set; }

        public DateTime DiscussDate { get; set; }

        [JsonIgnore]
        internal string TaStatusString
        {
            private get { return _taStatusString ?? (_taStatusString = string.Empty); }
            set { _taStatusString = value; }
        }

        public IEnumerable<TrsTaStatus> TaStatuses
        {
            get
            {
                return TaStatusString.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries)
                    .Select(t => (TrsTaStatus)int.Parse(t));
            }
        }

        public DateTime ApproveDate
        {
            get
            {
                if (_approveDate <= CommonAgent.MinDate)
                {
                    _approveDate = CommonAgent.TrsMinDate;
                    RecertificatedBy = _approveDate.AddYears(3);
                }
                return _approveDate;
            }
            set { _approveDate = value; }
        }

        public DateTime RecertificatedBy { get; set; }

        public DateTime UpdatedOn { get; set; }

        public DateTime CreatedOn { get; set; }

        public IEnumerable<TrsOfflineItemModel> Items { get; set; }

        private IEnumerable<TrsAssessmentClassModel> _assessmentClasses;

        [JsonIgnore]
        public IEnumerable<TrsAssessmentClassModel> AssessmentClasses
        {
            get { return _assessmentClasses ?? (_assessmentClasses = new List<TrsAssessmentClassModel>()); }
            set { _assessmentClasses = value; }
        }

        public TrsOfflineSchoolModel School { get; set; }

        public List<TrsClassModel> Classes { get; set; }

        private Dictionary<TRSCategoryEnum, Dictionary<int, List<TrsOfflineItemModel>>> _categories;

        public new Dictionary<TRSCategoryEnum, Dictionary<int, List<TrsOfflineItemModel>>> Categories
        {
            get { return _categories ?? (_categories = new Dictionary<TRSCategoryEnum, Dictionary<int, List<TrsOfflineItemModel>>>()); }
        }

        public Dictionary<TRSCategoryEnum, string> Category { get; set; }

        public Dictionary<int, TrsSubcategoryModel> SubCategory { get; set; }


        public void PrepareWithAccess(UserBaseEntity user)
        {
            //var categories = this.Items.Select(x => x.Category).Distinct().ToList();
            //Category = categories.ToDictionary(x => x, x => x.GetDisplayName());

            //_categories = new Dictionary<TRSCategoryEnum, Dictionary<int, List<TrsOfflineItemModel>>>();

            //var categoriesForSchool = new List<TRSCategoryEnum>();
            //if (categories.Contains(TRSCategoryEnum.Category1))
            //    categoriesForSchool.Add(TRSCategoryEnum.Category1);
            //if (categories.Contains(TRSCategoryEnum.Category5))
            //    categoriesForSchool.Add(TRSCategoryEnum.Category5);

            //categoriesForSchool.ForEach(category =>
            //{
            //    var items = this.Items.Where(x => x.Id > 0 && x.Category == category).Where(School.OfflineShouldVisible).ToList(); // 已填写
            //    items.AddRange(this.Items.Where(x => x.Id == 0 && x.Category == category).Where(School.OfflineShouldVisible));// 未填写
            //    items = items.DistinctBy(x => x.ItemId).OrderBy(i => i.SubCategorySort).ThenBy(i => i.Sort).ToList();
            //    IEnumerable<IGrouping<int, TrsOfflineItemModel>> groups = items.OrderBy(i => i.SubCategorySort).ThenBy(i => i.Sort)
            //        .GroupBy(x => x.SubCategoryId, x => x);
            //    if (user.ID > 0)
            //    {
            //        bool ifCanAccess = this.School.Assessor.UserId == user.ID || user.Role == Role.Super_admin;
            //        foreach (TrsOfflineItemModel item in items)
            //        {
            //            item.IfCanAccess = ifCanAccess;
            //        }
            //    }
            //    _categories.Add(category, groups.ToDictionary(x => x.Key, x => x.ToList()));
            //});

            //var categories2 = new List<TRSCategoryEnum>();

            //if (categories.Contains(TRSCategoryEnum.Category2))
            //    categories2.Add(TRSCategoryEnum.Category2);
            //if (categories.Contains(TRSCategoryEnum.Category3))
            //    categories2.Add(TRSCategoryEnum.Category3);
            //if (categories.Contains(TRSCategoryEnum.Category4))
            //    categories2.Add(TRSCategoryEnum.Category4);

            //categories2.ForEach(category => this.Classes.ForEach(class1 =>
            //{
            //    var itemsOther = this.Items.Where(x => x.Id > 0 && x.ClassId == class1.Id && x.Category == category)
            //        .Where(School.OfflineShouldVisible).Where(class1.OfflineShouldVisible).ToList(); //已填写
            //    itemsOther.AddRange(this.Items.Where(x => x.Id == 0
            //                                              && x.ClassId == 0
            //                                              && x.Category == category
            //                                              && itemsOther.Select(i => i.ItemId).Contains(x.ItemId) == false)
            //        .Where(School.OfflineShouldVisible).Where(class1.OfflineShouldVisible)); //未填写
            //    itemsOther =
            //        itemsOther.DistinctBy(x => x.ItemId).OrderBy(i => i.SubCategorySort).ThenBy(i => i.Sort).ToList();

            //    List<TrsOfflineItemModel> items_new = new List<TrsOfflineItemModel>();    //若是新加时，无法区分数据，需重新实例化并通过ClassId区分
            //    foreach (TrsOfflineItemModel item in itemsOther.Where(r => r.ClassId == 0))
            //    {
            //        TrsOfflineItemModel model = new TrsOfflineItemModel();
            //        model.ItemId = item.ItemId;
            //        model.SubCategoryId = item.SubCategoryId;
            //        model.SyncAnswer = item.SyncAnswer;
            //        model.ClassId = class1.Id;
            //        items_new.Add(model);
            //    }

            //    items_new = items_new.Count > 0 ? items_new : itemsOther;

            //    if (user.ID > 0)
            //    {
            //        if (this.School.Assessor.UserId == user.ID || user.Role == Role.Super_admin)
            //        {
            //            class1.IfCanAccessObservation = true;
            //            foreach (TrsOfflineItemModel item in items_new)
            //            {
            //                item.IfCanAccess = true;
            //            }
            //        }
            //        else
            //        {
            //            bool ifCanAccess = class1.TrsAssessorId == user.ID;
            //            class1.IfCanAccessObservation = ifCanAccess;
            //            foreach (TrsOfflineItemModel item in items_new)
            //            {
            //                item.IfCanAccess = ifCanAccess;
            //            }
            //        }
            //    }
            //    class1.Categories.Add(category,
            //        items_new.GroupBy(x => x.SubCategoryId, x => x).ToDictionary(x => x.Key, x => x.ToList()));
            //}));
        }
    }
}
