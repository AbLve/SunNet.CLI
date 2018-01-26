using System.Text;
using Newtonsoft.Json;
using Sunnet.Framework.Extensions;

namespace Sunnet.Cli.Core.Cpalls.Models
{
    /// <summary>
    /// 专给 GetSchoolMeasureGoal 方法使，执行Sql查询返回结果集
    /// </summary>
    public class SchoolMeasureGoalModel
    {
        private decimal _goal;
        public int SchoolId { get; set; }

        public int MeasureId { get; set; }

        /// <summary>
        /// 总分.
        /// </summary> 
        public decimal Goal
        {
            get
            {
                if (_goal < 0)
                    return 0;
                return _goal;
            }
            set { _goal = value; }
        }

        public int Amount { get; set; }

        /// <summary>
        /// 用于显示总分列的值
        /// </summary> 
        [JsonIgnore]
        public decimal? Total { get; set; }

        /// <summary>
        /// 提供平均分给Parent Measure以供计算平均分 的 和.
        /// </summary>
        [JsonIgnore]
        public decimal AverageOrDefault
        {
            get
            {
                if (Amount == 0)
                    return 0;
                return Goal / Amount;
            }
        }
        public string Average
        {
            get
            {
                if (Total.HasValue)
                    return Total.Value.ToPrecisionString(2);
                if (Amount == 0)
                    return "-";
                if (TotalScored)
                    return (Goal / Amount).ToPrecisionString(2);
                // √
                return Encoding.Unicode.GetString(new byte[] { 26, 34 });
            }
        }

        public bool TotalScored { get; set; }
    }
}
