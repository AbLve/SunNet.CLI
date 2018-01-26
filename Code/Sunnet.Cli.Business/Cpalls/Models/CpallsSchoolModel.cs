using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Sunnet.Cli.Core.Cpalls.Entities;
using Newtonsoft.Json;
using Sunnet.Cli.Core.Cpalls.Models;

namespace Sunnet.Cli.Business.Cpalls.Models
{
    public class CpallsCommunityModel
    {
        public int ID { get; set; }

        public string Name { get; set; }

        public int SchoolCount { get; set; }
    }

    public class CpallsSchoolModel
    {
        public CpallsSchoolModel()
        {
            MeasureList = new List<SchoolMeasureGoalModel>();
        }

        public int ID { get; set; }

        public string Name { get; set; }

        public IEnumerable<CpallsCommunityModel> Communities { get; set; }

        public string CommunitiesText {
            get
            {
                return Communities == null ? string.Empty : string.Join(", ", Communities.Select(x => x.Name));
            }
        }

        [JsonIgnore]
        public IEnumerable<SchoolMeasureGoalModel> MeasureList { get; set; }

        public Dictionary<int, SchoolMeasureGoalModel> DicMeasure
        {
            get
            {
                if (MeasureList == null || !MeasureList.Any()) return null;
                Dictionary<int, SchoolMeasureGoalModel> dic = new Dictionary<int, SchoolMeasureGoalModel>();
                foreach (SchoolMeasureGoalModel item in MeasureList)
                {
                    if (item == null)
                        continue;
                    if (dic.ContainsKey(item.MeasureId))
                        dic[item.MeasureId] = item;
                    else
                        dic.Add(item.MeasureId, item);
                }
                return dic;
            }
        }
    }

    public class CpallsSchoolCustomScoreModel
    {
        public int ID { get; set; }

        public string Name { get; set; }

        public IEnumerable<int> StudentIds { get; set; }
        
    }
}
