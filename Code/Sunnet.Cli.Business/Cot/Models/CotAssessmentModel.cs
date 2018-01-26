using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
/**************************************************************************
 * Developer: 		JackZhang
 * Computer:		JACKZ
 * Domain:			Jackz
 * CreatedOn:		2014/12/17 9:37:21
 * Description:		Please input class summary
 * Version History:	Created,2014/12/17 9:37:21
 * 
 * 
 **************************************************************************/
using LinqKit;
using Newtonsoft.Json;
using Sunnet.Cli.Business.Common;
using Sunnet.Cli.Business.Users.Models;
using Sunnet.Cli.Core.Cot;
using Sunnet.Cli.Core.Cot.Entities;
using Sunnet.Cli.Core.Cpalls;

namespace Sunnet.Cli.Business.Cot.Models
{
    public class CotAssessmentModel
    {
        public CotAssessmentModel()
        {

        }

        public int ID { get; set; }

        public int TeacherId { get; set; }

        public TeacherModel Teacher { get; set; }

        public int AssessmentId { get; set; }

        public string Name { get; set; }

        [DisplayName("School Year")]
        [StringLength(10)]
        public string SchoolYear { get; set; }

        [DisplayName("School Year")]
        public string FullSchoolYear
        {
            get
            {
                if (string.IsNullOrEmpty(SchoolYear))
                    return string.Empty;
                return SchoolYear.ToFullSchoolYearString();
            }
        }

        public CotAssessmentStatus Status { get; set; }

        [DisplayName("Created By")]
        public int CreatedBy { get; set; }

        [DisplayName("Updated By")]
        public int UpdatedBy { get; set; }

        [DataType(DataType.DateTime)]
        [Range(typeof(DateTime), "1753-1-1", "2100-1-1")]
        [DisplayName("Created On")]
        public DateTime CreatedOn { get; set; }

        /// <summary>
        /// 获取或设置　修改时间；也可以用来进行　版本控制标识，用于处理并发
        /// </summary>
        [DataType(DataType.DateTime)]
        [DisplayName("Updated On")]
        [Range(typeof(DateTime), "1753-1-1", "2100-1-1")]
        public DateTime UpdatedOn { get; set; }

        public IEnumerable<CotMeasureModel> Measures
        {
            get
            {
                if (_measures != null)
                    return _measures.OrderBy(x => x.Sort);
                return null;
            }
            set { _measures = value; }
        }

        private List<CotItemModel> _items;
        private IEnumerable<CotMeasureModel> _measures;

        [JsonIgnore]
        public IEnumerable<CotItemModel> Items
        {
            get
            {
                if (_items == null)
                {
                    _items = new List<CotItemModel>();
                    if (this.Measures != null)
                    {
                        this.Measures.ForEach(mea =>
                        {
                            _items.AddRange(mea.Items);
                            if (mea.Children != null)
                            {
                                mea.Children.ForEach(c => _items.AddRange(c.Items));
                            }
                        });
                    }
                }
                return _items;
            }
        }

        public CotAssessmentModel Clone()
        {
            var newModel = new CotAssessmentModel();
            newModel.AssessmentId = this.AssessmentId;
            newModel.Name = this.Name;
            newModel.CreatedBy = this.CreatedBy;
            newModel.CreatedOn = this.CreatedOn;
            newModel.ID = this.ID;
            newModel.SchoolYear = this.SchoolYear;
            newModel.Status = this.Status;
            newModel.TeacherId = this.TeacherId;
            newModel.UpdatedBy = this.UpdatedBy;
            newModel.UpdatedOn = this.UpdatedOn;
            newModel.Measures = this.Measures.Select(m => new CotMeasureModel()
            {
                MeasureId = m.MeasureId,
                Name = m.Name,
                Items = m.Items.Select(item => new CotItemModel()
                {
                    ID = item.ID,
                    ItemId = item.ItemId,
                    Level = item.Level,
                    CircleManual = item.CircleManual,
                    CotAssessmentId = item.CotAssessmentId,
                    FullTargetText = item.FullTargetText,
                    MentoringGuide = item.MentoringGuide,
                    PrekindergartenGuidelines = item.PrekindergartenGuidelines,
                    ShortTargetText = item.ShortTargetText,
                    CotItemId = item.CotItemId,
                    Links = item.Links
                }).ToList(),
                Links = m.Links,
                Children = m.Children.Select(child => new CotChildMeasureModel()
                {
                    MeasureId = child.MeasureId,
                    Name = child.Name,
                    Items = child.Items.Select(item => new CotItemModel()
                    {
                        ID = item.ID,
                        ItemId = item.ItemId,
                        Level = item.Level,
                        CircleManual = item.CircleManual,
                        CotAssessmentId = item.CotAssessmentId,
                        FullTargetText = item.FullTargetText,
                        MentoringGuide = item.MentoringGuide,
                        PrekindergartenGuidelines = item.PrekindergartenGuidelines,
                        ShortTargetText = item.ShortTargetText,
                        CotItemId = item.CotItemId,
                        Links = item.Links
                    }).ToList(),
                    Links = child.Links
                }).ToList()
            }).ToList();
            return newModel;
        }

        public CotStgReportModel Report { get; set; }


    }
}
