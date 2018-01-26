using Sunnet.Framework;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CutpointRescoring
{
    class Program
    {
        static void Main(string[] args)
        {
            IoC.Init();
            Config.Instance.Logger.Info("Running ...");
            QueueManager manager = new QueueManager();
            manager.Start();
        } 
    }
}
