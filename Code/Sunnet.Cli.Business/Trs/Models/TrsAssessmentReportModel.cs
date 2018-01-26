using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/**************************************************************************
 * Developer: 		JackZhang
 * Computer:		JACKZ
 * Domain:			JackZ
 * CreatedOn:		1/20 2015 16:03:19
 * Description:		Please input class summary
 * Version History:	Created,1/20 2015 16:03:19
 * 
 * 
 **************************************************************************/
using Sunnet.Cli.Core.Trs;
using Sunnet.Framework.Extensions;

namespace Sunnet.Cli.Business.Trs.Models
{
    public class TrsAssessmentReportModel : TrsAssessmentModelBase
    {
        const int staffRatioId = 7;

        internal override void Prepare(bool showNAItem = false)
        {
            var categories = this.Items.Select(x => x.Category).Distinct().ToList();
            Categories = new Dictionary<TRSCategoryEnum, Dictionary<int, List<TrsItemModel>>>();

            var categoriesForSchool = new List<TRSCategoryEnum>();
            if (categories.Contains(TRSCategoryEnum.Category1))
                categoriesForSchool.Add(TRSCategoryEnum.Category1);
            if (categories.Contains(TRSCategoryEnum.Category5))
                categoriesForSchool.Add(TRSCategoryEnum.Category5);

            categoriesForSchool.ForEach(category =>
            {
                var items = this.Items.Where(x => x.Id > 0 && x.AnswerId > 0 && x.Category == category)
                    .ToList(); // 已填写
                items = items.DistinctBy(x => x.ItemId).OrderBy(i => i.SubCategorySort).ThenBy(i => i.Sort).ToList();
                IEnumerable<IGrouping<int, TrsItemModel>> groups = items.OrderBy(i => i.SubCategorySort).ThenBy(i => i.Sort)
                    .GroupBy(x => x.SubCategoryId, x => x);
                Categories.Add(category, groups.ToDictionary(x => x.Key, x => x.ToList()));
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
                var itemsOther = this.Items.Where(x => x.Id > 0 && x.AnswerId > 0 && x.ClassId == class1.Id && x.Category == category)
                    .ToList(); //已填写
                itemsOther =
                    itemsOther.DistinctBy(x => x.ItemId).OrderBy(i => i.SubCategorySort).ThenBy(i => i.Sort).ToList();
                class1.Categories.Add(category,
                    itemsOther.GroupBy(x => x.SubCategoryId, x => x).ToDictionary(x => x.Key, x => x.ToList()));

                if (category == TRSCategoryEnum.Category2)
                    class1.StaffRatioItems = itemsOther.Where(x => x.SubCategoryId == staffRatioId).ToList();
            }));
        }
    }
}
