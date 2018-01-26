using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/**************************************************************************
 * Developer: 		JackZhang
 * Computer:		JACKZ
 * Domain:			Jackz
 * CreatedOn:		2014/12/17 9:43:03
 * Description:		Please input class summary
 * Version History:	Created,2014/12/17 9:43:03
 * 
 * 
 **************************************************************************/
using DocumentFormat.OpenXml.Office.CustomUI;
using LinqKit;
using Newtonsoft.Json;
using Sunnet.Cli.Business.Ade.Models;
using Sunnet.Cli.Core.Ade;
using Sunnet.Cli.Core.Ade.Entities;

namespace Sunnet.Cli.Business.Observable.Models
{
    public class ObservableMeasureModel : ObservableChildMeasureModel
    {
        private List<ObservableItemModel> _itemsAll;
        private IEnumerable<ObservableChildMeasureModel> _children;
        private IEnumerable<ObservableItemModel> _items1;

        public IEnumerable<ObservableChildMeasureModel> Children
        {
            get
            {
                if (_children != null)
                    return _children.OrderBy(x => x.Sort);
                return _children;
            }
            set { _children = value; }
        }

        public new IEnumerable<ObservableItemModel> Items
        {
            get
            {
                if (_items1 == null)
                    _items1 = new List<ObservableItemModel>();
                return _items1.OrderBy(x => x.Sort);
            }
            set
            {
                _itemsAll = null;
                _items1 = value;
            }
        }
       
        [JsonIgnore]
        private IEnumerable<ObservableItemModel> AllItems
        {
            get
            {
                if (_itemsAll == null)
                {
                    _itemsAll = new List<ObservableItemModel>();
                    if (this.Children != null && this.Children.Any())
                    {
                        this.Children.ForEach(child => _itemsAll.AddRange(child.Items));
                    }
                    if (Items != null && Items.Any())
                        _itemsAll.AddRange(Items);
                }
                return _itemsAll;
            }
        }

        public new bool Visible
        {
            get { return this.AllItems != null && this.AllItems.Any(); }
        }
    }
    public class ObservableChildMeasureModel
    {
        private IEnumerable<ObservableItemModel> _items;

        public int MeasureId { get; set; }

        public string Name { get; set; }

        public IEnumerable<ObservableItemModel> Items
        {
            get
            {
                if (_items == null)
                    _items = new List<ObservableItemModel>();
                return _items.OrderBy(x => x.Sort);
            }
            set { _items = value; }
        }
        
        public bool Visible
        {
            get { return this.Items != null && this.Items.Any(); }
        }

        /// <summary>
        /// STG Report页面自定义排序, 使用Item中最小的Sort作为Measure的Sort
        /// </summary>
        public int Sort { get; set; }
        public IEnumerable<AdeLinkModel> Links { get; set; }
    }
    public class ObservableItemModel
    {
        public int ID { get; set; }
        public int ItemId { get; set; }
        public string Name { get; set; }
        public ItemType Type { get; set; }
        public int Sort { get; set; }
        public List<AdeLinkModel> Links { get; set; }

        public ICollection<AnswerEntity> Answers { get; set; }
        public AnswerType AnswerType { get; set; }
        public bool IsMultiChoice { get; set; }

        public string Res { get; set; }
        public bool IsShown { get; set; }
        public string Date { get; set; }

        public string FullTargetText { get; set; }

        public bool IsRequired { get; set; }
    }
}
