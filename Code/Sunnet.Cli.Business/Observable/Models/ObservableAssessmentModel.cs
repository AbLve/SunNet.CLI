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
using Sunnet.Cli.Core.Students.Entities;

namespace Sunnet.Cli.Business.Observable.Models
{
    public class ObservableAssessmentModel
    {
        public ObservableAssessmentModel()
        {
        }
        public int ID { get; set; }

        public int StudentId { get; set; }

        public StudentEntity Student { get; set; }

        public int AssessmentId { get; set; }

        public string Name { get; set; }

        [DisplayName("Created By")]
        public int CreatedBy { get; set; }

        [DisplayName("Updated By")]
        public int UpdatedBy { get; set; }

        [DataType(DataType.DateTime)]
        [Range(typeof(DateTime), "1753-1-1", "2100-1-1")]
        [DisplayName("Created On")]
        public DateTime CreatedOn { get; set; }

        [DataType(DataType.DateTime)]
        [DisplayName("Updated On")]
        [Range(typeof(DateTime), "1753-1-1", "2100-1-1")]
        public DateTime UpdatedOn { get; set; }

        public IEnumerable<ObservableMeasureModel> Measures
        {
            get
            {
                if (_measures != null)
                    return _measures.OrderBy(x => x.Sort);
                return null;
            }
            set { _measures = value; }
        }

        private List<ObservableItemModel> _items;
        private IEnumerable<ObservableMeasureModel> _measures;
        [JsonIgnore]
        public IEnumerable<ObservableItemModel> Items
        {
            get
            {
                if (_items == null)
                {
                    _items = new List<ObservableItemModel>();
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

        public ObservableAssessmentModel Clone()
        {
            var newModel = new ObservableAssessmentModel();
            newModel.AssessmentId = this.AssessmentId;
            newModel.Name = this.Name;
            newModel.CreatedBy = this.CreatedBy;
            newModel.CreatedOn = this.CreatedOn;
            newModel.ID = this.ID;
            newModel.UpdatedBy = this.UpdatedBy;
            newModel.UpdatedOn = this.UpdatedOn;
            newModel.Measures = this.Measures.Select(m => new ObservableMeasureModel()
            {
                MeasureId = m.MeasureId,
                Name = m.Name,
                Items = m.Items.Select(item => new ObservableItemModel()
                {
                    ID = item.ID,
                    ItemId = item.ItemId,
                    Name = item.Name,
                    Links = item.Links,
                    Answers = item.Answers.ToList(),
                    AnswerType = item.AnswerType,
                    IsMultiChoice = item.IsMultiChoice,
                    Type = item.Type,
                    IsShown = item.IsShown,
                    Res = item.Res,
                    FullTargetText = item.FullTargetText,
                    IsRequired = item.IsRequired
                }).ToList(),
                Links = m.Links,
                Children = m.Children.Select(child => new ObservableChildMeasureModel()
                {
                    MeasureId = child.MeasureId,
                    Name = child.Name,
                    Items = child.Items.Select(item => new ObservableItemModel()
                    {
                        ID = item.ID,
                        ItemId = item.ItemId,
                        Name = item.Name,
                        Links = item.Links,
                        Answers = item.Answers.ToList(),
                        AnswerType = item.AnswerType,
                        IsMultiChoice = item.IsMultiChoice,
                        Type = item.Type,
                        IsShown = item.IsShown,
                        Res = item.Res,
                        FullTargetText = item.FullTargetText,
                        IsRequired = item.IsRequired
                    }).ToList(),
                    Links = child.Links
                }).ToList()
            }).ToList();
            return newModel;
        }
    }
}
