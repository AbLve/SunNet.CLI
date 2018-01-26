using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StructureMap;
using Sunnet.Framework.Log;

namespace Sunnet.Cli.Business.Reports.Model
{
    public class YAxis
    {
        protected readonly ISunnetLog _logger;
        private int initVlaue = 0;
        public YAxis()
            : this(0)
        {

        }

        public YAxis(int init)
        {
            initVlaue = init;
            Current = initVlaue;
            try
            {
                _logger = ObjectFactory.GetInstance<ISunnetLog>();
            }
            catch
            {
            }
            Info("YAxis:{0}", Current);
        }

        protected void Info(string format, params object[] args)
        {
            //if (_logger != null)
            //{
            //    _logger.Info(format, args);
            //}
        }

        protected int Current { get; set; }

        public int Next
        {
            get
            {
                Current++;
                Info("YAxis.Next:{0}", Current);
                return Current;
            }
        }

        public void Reset()
        {
            Current = initVlaue;
            Info("YAxis.Reset:{0}", Current);
        }

        public int StepBack(int steps)
        {
            Current -= steps + 1;
            if (Current < 0)
                Current = 0;
            Info("YAxis.StepBack:{0}", Current);
            return Next;
        }

        public int Step(int steps)
        {
            Current += steps - 1;
            Info("YAxis.Step:{0}", Current);
            return Next;
        }

        public int GetByStep(int steps)
        {
            var result = Current + steps;
            Info("YAxis.GetByStep:{0}", Current);
            return result;
        }
    }
}
