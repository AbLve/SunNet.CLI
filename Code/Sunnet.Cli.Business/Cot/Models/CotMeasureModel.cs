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
using Sunnet.Cli.Core.Ade.Entities;

namespace Sunnet.Cli.Business.Cot.Models
{
    public class CotMeasureModel : CotChildMeasureModel
    {
        private List<CotItemModel> _itemsAll;
        private IEnumerable<CotChildMeasureModel> _children;
        private IEnumerable<CotItemModel> _items1;

        public IEnumerable<CotChildMeasureModel> Children
        {
            get
            {
                if (_children != null)
                    return _children.OrderBy(x => x.Sort);
                return _children;
            }
            set { _children = value; }
        }

        public new IEnumerable<CotItemModel> Items
        {
            get
            {
                if (_items1 == null)
                    _items1 = new List<CotItemModel>();
                return _items1.OrderBy(x => x.Sort);
            }
            set
            {
                _itemsAll = null;
                _items1 = value;
            }
        }

        [JsonIgnore]
        private IEnumerable<CotItemModel> AllItems
        {
            get
            {
                if (_itemsAll == null)
                {
                    _itemsAll = new List<CotItemModel>();
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
    public class CotChildMeasureModel
    {
        private IEnumerable<CotItemModel> _items;

        public int MeasureId { get; set; }

        public string Name { get; set; }

        public IEnumerable<CotItemModel> Items
        {
            get
            {
                if (_items == null)
                    _items = new List<CotItemModel>();
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
}
