using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StructureMap;
using Sunnet.Framework.Log;

namespace DataManagement.App
{
    public static class LogManager
    {
       static readonly ISunnetLog Logger = ObjectFactory.GetInstance<ISunnetLog>();

        public static void Info(string info)
        {
            Logger.Info(info);
        }

        public static void Debug(string debug)
        {
            Logger.Debug(debug);
        }

        public static void InfoAndConsole(string info)
        {
            Logger.Info(info);
            Console.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss ") + info);
        }

        public static void DebugAndConsole(string debug)
        {
            Logger.Debug(debug);
            Console.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss ") + debug);
        }
    }
}
