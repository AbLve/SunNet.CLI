using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/**************************************************************************
 * Developer: 		JackZhang
 * Computer:		JACKZ
 * Domain:			JackZ
 * CreatedOn:		1/30 2015 11:38:46
 * Description:		Please input class summary
 * Version History:	Created,1/30 2015 11:38:46
 * 
 * 
 **************************************************************************/
using LinqKit;
using Sunnet.Cli.Business.Cot.Cumulative;

namespace Sunnet.Cli.Business.Cot.Report
{
    public class CotMeasureModel
    {
        private IEnumerable<int> _items;
        private IEnumerable<int> _all;

        public int ID { get; set; }

        public string Name { get; set; }

        public string ShortName { get; set; }

        public int ParentId { get; set; }

        public int Sort { get; set; }

        public int ItemCount
        {
            get
            {
                var count = 0;
                if (Items == null)
                    return 0;
                return Items.Count();
            }
        }

        public IEnumerable<int> Items
        {
            get
            {
                if (_all == null)
                {
                    _all = _items;
                    if (ItemsOfSubMeasure != null && ItemsOfSubMeasure.Any())
                    {
                        ItemsOfSubMeasure.ForEach(items => _all = _all.Union(items));
                    }
                }
                return _all;
            }
            set
            {
                _all = null;
                _items = value;
            }
        }

        public IEnumerable<IEnumerable<int>> ItemsOfSubMeasure { get; set; }

        public IEnumerable<AdeCotItemModel> CotItems { get; set; }

    }
}
