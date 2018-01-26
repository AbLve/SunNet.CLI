using LinqKit;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace Sunnet.Cli.Business.Cot.Summary
{
    public class SummaryMeasureModel
    {
        private Dictionary<string, int> _countOfMonth;
        private int _itemCount;
        private IEnumerable<SummaryItemModel> _items;
        private IEnumerable<SummaryItemModel> _itemsAll;
        private IEnumerable<IEnumerable<SummaryItemModel>> _itemsOfSubMeasure;

        public int ID { get; set; }

        public string Name { get; set; }

        public string ShortName { get; set; }

        public int ItemCount
        {
            get { return _itemCount + SubItemCount; }
            set { _itemCount = value; }
        }

        public int FillColorCount
        {
            get;
            set;
        }

        internal int SubItemCount { get; set; }

        public Dictionary<string, int> CountOfMonth
        {
            get { return _countOfMonth ?? (_countOfMonth = new Dictionary<string, int>()); }
            set { _countOfMonth = value; }
        }

        public IEnumerable<SummaryItemModel> Items
        {
            get
            {
                if (_itemsAll == null)
                {
                    _itemsAll = _items;
                    if (ItemsOfSubMeasure != null && ItemsOfSubMeasure.Any())
                    {
                        ItemsOfSubMeasure.ForEach(items => _itemsAll = _itemsAll.Union(items));
                    }
                }
                return _itemsAll;
            }
            set
            {
                _itemsAll = null;
                _items = value;
            }
        }

        [JsonIgnore]
        public IEnumerable<IEnumerable<SummaryItemModel>> ItemsOfSubMeasure
        {
            get { return _itemsOfSubMeasure; }
            set
            {
                _itemsAll = null;
                _itemsOfSubMeasure = value;
            }
        }
    }
}
