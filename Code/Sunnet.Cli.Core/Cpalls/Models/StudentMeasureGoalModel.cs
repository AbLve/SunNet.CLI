namespace Sunnet.Cli.Core.Cpalls.Models
{
    /// <summary>
    /// 专给 GetStudentMeasureGoal 方法使，执行Sql查询返回结果集
    /// </summary>
    public class StudentMeasureGoalModel
    {
        private decimal _goal;
        public int StudentId { get; set; }

        public int MeasureId { get; set; }

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

        public bool TotalScored { get; set; }
    }

}
