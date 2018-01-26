using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sunnet.Cli.Business.Cpalls.Models;
using Sunnet.Cli.Core.Ade.Entities;
using Newtonsoft.Json;

namespace Sunnet.Cli.Business.Cpalls.Group
{
    public class GroupDataModel
    {
        public int MeasureId { get; set; }

        public string Name { get; set; }

        public int ParentId { get; set; }

        public string ParentName { get; set; }

        public List<AdeLinkEntity> Links { get; set; }

        //public List<GroupStudentModel> StudentList { get; set; }

        [JsonIgnore]
        public Dictionary<int, GroupBenchamrkModel> DicBenchmarkList { get; set; }

        public List<GroupBenchamrkModel> BenchmarkList
        {
            get
            {
                List<GroupBenchamrkModel> _benchmarkList = new List<GroupBenchamrkModel>();
                foreach (KeyValuePair<int, GroupBenchamrkModel> model in DicBenchmarkList)
                {
                    _benchmarkList.Add(model.Value);
                }
                return _benchmarkList.OrderBy(b => b.LowerScore).ToList();
            }
        }

        public int Sort { get; set; }
        public int ParentSort { get; set; }

        public int SubSort
        {
            get
            {
                if (ParentId == 1)
                {
                    return -1;
                }
                else
                {
                    return Sort;
                }
            }
        }
        public string Note { get; set; }

        public bool GroupByLabel { get; set; }

    }

    public class GroupBenchamrkModel
    {
        public int BenchmarkId { get; set; }

        public string LabelText { get; set; }

        public string Color { get; set; }

        public decimal LowerScore { get; set; }

        public List<GroupStudentModel> StudentList { get; set; }
    }
}
