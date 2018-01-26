using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sunnet.Cli.Core.Cpalls.Entities;
using Sunnet.Cli.Core.Cpalls;
using Sunnet.Cli.Business.Common;
using Newtonsoft.Json;
using Sunnet.Cli.Core.Cpalls.Models;
using Sunnet.Framework.Extensions;

namespace Sunnet.Cli.Business.Practices.Models
{
    public class PracticeAssessmentModel
    {
        public PracticeAssessmentModel()
        {
            //StudentIds = new List<int>();
            //MeasureList = new List<SchoolMeasureGoalModel>();
        }

        public int ID { get; set; }
        public string Name { get; set; }
        //public IEnumerable<int> StudentIds { get; set; }
        //[JsonIgnore]
        //public IEnumerable<SchoolMeasureGoalModel> MeasureList { get; set; }


        //public Dictionary<int, SchoolMeasureGoalModel> DicMeasure
        //{
        //    get
        //    {
        //        if (MeasureList == null || MeasureList.Count() == 0) return null;
        //        Dictionary<int, SchoolMeasureGoalModel> dic = new Dictionary<int, SchoolMeasureGoalModel>();
        //        foreach (SchoolMeasureGoalModel item in MeasureList)
        //        {
        //            if (item == null)
        //                continue;
        //            if (dic.ContainsKey(item.MeasureId))
        //                dic[item.MeasureId] = item;
        //            else
        //                dic.Add(item.MeasureId, item);
        //        }
        //        return dic;
        //    }
        //}
    }


}
