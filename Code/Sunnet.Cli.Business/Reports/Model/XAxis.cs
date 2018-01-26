using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sunnet.Cli.Business.Reports.Model
{

    public class XAxis : YAxis
    {
        static XAxis()
        {
            Code = new Dictionary<int, char>();
            Code.Add(0, 'A');
            for (int i = 1; i <= 26; i++)
            {
                Code.Add(i, (char)(i + 64));
            }
        }
        public XAxis()
        {
            Info("XAxis:{0}", Current);
        }

        private static Dictionary<int, char> Code { get; set; }

        private string GetColumnByIndex(int index)
        {
            if (Code.ContainsKey(index))  // 1-26
                return Code[index].ToString();

            var first = index / 26;
            var second = index % 26;
            if (second == 0)
            {
                first--;
                second = 26;
            }
            return GetColumnByIndex(first) + GetColumnByIndex(second);
        }

        public new string Next
        {
            get
            {
                Info("XAxis.Next:{0}", Current);
                return GetColumnByIndex(base.Next);
            }
        }

        public new string StepBack(int steps)
        {
            Info("XAxis.StepBack:{0}", Current);
            return GetColumnByIndex(base.StepBack(steps));
        }

        public new string Step(int steps)
        {
            Info("XAxis.Step:{0}", Current);
            return GetColumnByIndex(base.Step(steps));
        }


        public new string GetByStep(int steps)
        {
            var index = base.GetByStep(steps);
            Info("XAxis.GetByStep:{0}", index);
            return GetColumnByIndex(index);
        }

        public int Raw
        {
            get { return Current; }
        }
    }

}
