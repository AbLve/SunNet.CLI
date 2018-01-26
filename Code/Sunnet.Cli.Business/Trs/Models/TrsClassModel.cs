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
 * CreatedOn:		1/13 2015 9:29:16
 * Description:		Please input class summary
 * Version History:	Created,1/13 2015 9:29:16
 * 
 * 
 **************************************************************************/
using Newtonsoft.Json;
using Sunnet.Cli.Core.Schools.Enums;
using Sunnet.Cli.Core.Trs;
using Sunnet.Cli.Core.Trs.Enums;
using Sunnet.Cli.Core.TRSClasses.Enums;

namespace Sunnet.Cli.Business.Trs.Models
{
    class SubCategoryHelper
    {
        internal static List<int> MixedAgeClass
        {
            get
            {
                // todo: 硬编码：Group Size - Mixed Age Classroom 
                return new List<int>() { 6 };
            }
        }

        internal static List<int> NonMixedAgeClass
        {
            get
            {
                // todo: 硬编码：Group Size - Non-Mixed Age Classroom 
                return new List<int>() { 5 };
            }
        }
    }
    public class TrsClassModel
    {
        public int Id { get; set; }

        public string Name { get; set; }

        private Dictionary<TRSCategoryEnum, Dictionary<int, List<TrsItemModel>>> _categories;
        private IEnumerable<TrsTeacherModel> _teachers;

        public Dictionary<TRSCategoryEnum, Dictionary<int, List<TrsItemModel>>> Categories
        {
            get { return _categories ?? (_categories = new Dictionary<TRSCategoryEnum, Dictionary<int, List<TrsItemModel>>>()); }
        }

        private Dictionary<TRSCategoryEnum, Dictionary<int, List<TrsOfflineItemModel>>> _categories_offline;

        public Dictionary<TRSCategoryEnum, Dictionary<int, List<TrsOfflineItemModel>>> Categories_Offline
        {
            get { return _categories_offline ?? (_categories_offline = new Dictionary<TRSCategoryEnum, Dictionary<int, List<TrsOfflineItemModel>>>()); }
        }

        public TRSStarEnum Star { get; set; }

        [DisplayName("Type of Class")]
        public TRSClassofType TypeOfClass { get; set; }

        //[JsonIgnore]    offline时需要用到此属性
        public List<TrsAgeModel> Ages { get; set; }

        public int PlaygroundId { get; set; }

        /// <summary>
        /// 判断Items对Classroom是否可见(Category2/3/4).
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns></returns>
        public bool ShouldVisible(TrsItemModel model)
        {
            if (model.Category != TRSCategoryEnum.Category3
                && model.Category != TRSCategoryEnum.Category4
                && model.Category != TRSCategoryEnum.Category2)
            {
                return false;
            }
            if (SubCategoryHelper.MixedAgeClass.Contains(model.SubCategoryId))
                return false;
            if (SubCategoryHelper.NonMixedAgeClass.Contains(model.SubCategoryId))
                return false;

            if (model.SyncAnswer == SyncAnswerType.SamePlayground && this.PlaygroundId == 0)
                return false;
            var result = false;
            result = model.ShowByInfants && this.Ages.Any(x => x.AgeArea == TrsAgeArea.Infants)
                     || model.ShowByToddlers && this.Ages.Any(x => x.AgeArea == TrsAgeArea.Toddlers)
                     || model.ShowByPreschool && this.Ages.Any(x => x.AgeArea == TrsAgeArea.Preschool)
                     || model.ShowBySchoolAge && this.Ages.Any(x => x.AgeArea == TrsAgeArea.SchoolAge);

            return result;
        }

        public IEnumerable<TrsItemModel> StaffRatioItems { get; set; }

        public IEnumerable<TrsTeacherModel> Teachers
        {
            get { return _teachers ?? (_teachers = new List<TrsTeacherModel>()); }
            set { _teachers = value; }
        }

        public decimal ObservationLength { get; set; }

        public int TrsAssessorId { get; set; }

        public int TrsMentorId { get; set; }

        public bool IfCanAccessObservation { get; set; }
    }


    public class TrsClassReportModel
    {
        public int ID { get; set; }

        public string Name { get; set; }

        [DisplayName("Type of Class")]
        public TRSClassofType TypeOfClass { get; set; }

        public List<TrsAgeReportModel> Ages { get; set; }

        public int PlaygroundId { get; set; }

        /// <summary>
        /// 判断Items对Classroom是否可见(Category2/3/4).
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns></returns>
        public bool ShouldVisible(TrsItemReportModel model)
        {
            if (model.Category != TRSCategoryEnum.Category3
                && model.Category != TRSCategoryEnum.Category4
                && model.Category != TRSCategoryEnum.Category2)
            {
                return false;
            }
            if (SubCategoryHelper.MixedAgeClass.Contains(model.SubCategoryId) && this.TypeOfClass == TRSClassofType.NMAC)
                return false;
            if (SubCategoryHelper.NonMixedAgeClass.Contains(model.SubCategoryId) && this.TypeOfClass == TRSClassofType.MAC)
                return false;

            if (model.SyncAnswer == SyncAnswerType.SamePlayground && this.PlaygroundId == 0)
                return false;
            var result = false;
            result = model.ShowByInfants && this.Ages.Any(x => x.AgeArea == TrsAgeArea.Infants)
                     || model.ShowByToddlers && this.Ages.Any(x => x.AgeArea == TrsAgeArea.Toddlers)
                     || model.ShowByPreschool && this.Ages.Any(x => x.AgeArea == TrsAgeArea.Preschool)
                     || model.ShowBySchoolAge && this.Ages.Any(x => x.AgeArea == TrsAgeArea.SchoolAge);

            return result;
        }
    }
}
