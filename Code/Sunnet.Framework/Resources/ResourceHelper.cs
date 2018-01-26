using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Text;

namespace Sunnet.Framework.Resources
{
    public class ResourceHelper
    {
        private ResourceManager _RmInformation = 
            new ResourceManager(SFConfig.ResourceFile, Assembly.Load(SFConfig.ResourceAssembly));

        public ResourceManager RmInformation
        {
            get { return _RmInformation; }
        }

         public static ResourceHelper GetRM()
        {
            return Nested.instance;
        }

         private ResourceHelper() { }

        class Nested
        {
            static Nested() { }
            internal static readonly ResourceHelper instance = new ResourceHelper();
        }

        public string GetInformation(string key)
        {
            return _RmInformation.GetObject(key).ToString();
        }

    }
}
