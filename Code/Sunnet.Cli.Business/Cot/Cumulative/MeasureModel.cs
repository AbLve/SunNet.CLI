using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/**************************************************************************
 * Developer: 		JackZhang
 * Computer:		JACKZ
 * Domain:			JackZ
 * CreatedOn:		2/7 2015 14:12:47
 * Description:		Please input class summary
 * Version History:	Created,2/7 2015 14:12:47
 * 
 * 
 **************************************************************************/
using LinqKit;
using Newtonsoft.Json;

namespace Sunnet.Cli.Business.Cot.Cumulative
{
    public class MeasureModel
    {
        private IEnumerable<AdeCotItemModel> _items;
        private IEnumerable<AdeCotItemModel> _itemsAll;
        private IEnumerable<IEnumerable<AdeCotItemModel>> _itemsOfSubMeasure;

        public int ID { get; set; }

        public string Name { get; set; }

        public string ShortName { get; set; }

        public int ParentId { get; set; }

        public int Sort { get; set; }

        public IEnumerable<AdeCotItemModel> Items
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
        public IEnumerable<IEnumerable<AdeCotItemModel>> ItemsOfSubMeasure
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
