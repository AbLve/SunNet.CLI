using LinqKit;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace Sunnet.Cli.Business.Cot.Growth
{
    public class MeasureModel
    {
        private Dictionary<string, int> _countOfMonth;
        private int _itemCount;
        private IEnumerable<GrowthItemModel> _items;
        private IEnumerable<GrowthItemModel> _itemsAll;
        private IEnumerable<IEnumerable<GrowthItemModel>> _itemsOfSubMeasure;

        public int ID { get; set; }

        public string Name { get; set; }

        public string ShortName { get; set; }

        public int ItemCount
        {
            get { return _itemCount + SubItemCount; }
            set { _itemCount = value; }
        }

        internal int SubItemCount { get; set; }

        public Dictionary<string, int> CountOfMonth
        {
            get { return _countOfMonth ?? (_countOfMonth = new Dictionary<string, int>()); }
            set { _countOfMonth = value; }
        }

        public IEnumerable<GrowthItemModel> Items
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
        public IEnumerable<IEnumerable<GrowthItemModel>> ItemsOfSubMeasure
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
