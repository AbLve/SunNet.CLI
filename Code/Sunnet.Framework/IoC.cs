using StructureMap;
using Sunnet.Framework.EmailSender;
using Sunnet.Framework.Encrypt;
using Sunnet.Framework.File;
using Sunnet.Framework.IoCConfiguration;
using Sunnet.Framework.Log;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sunnet.Framework
{
    public class IoC
    {
        public static void Init()
        {
            string l4net = AppDomain.CurrentDomain.BaseDirectory + "/log4net.config";
            log4net.Config.XmlConfigurator.ConfigureAndWatch(new System.IO.FileInfo(l4net));
        }

        private static IIoCConfigure configure = new DefaultIoCConfigure();

        static IoC()
        {
            ObjectFactory.Initialize(x =>
            {
                // Tell StructureMap to look for configuration
                // from the App.config file
                // The default is false
                //x.PullConfigurationFromAppConfig = true;

                // We put the properties for an NHibernate ISession
                // in the StructureMap.config file, so this file
                // must be there for our application to
                // function correctly
                x.UseDefaultStructureMapConfigFile = true;
            });
            IoC_Required(configure);
        }

        public static void IoC_Required(IIoCConfigure config)
        {
            configure = config;
            ObjectFactory.Configure(x =>
            {
                x.For<ISunnetLog>().Singleton().Use(configure.Log);
                x.For<IFile>().Singleton().Use(configure.File);
                x.For<IEmailSender>().Singleton().Use(configure.EmailSender);
                x.For<IEncrypt>().Singleton().Use(configure.Encrypt);
            });
        }
    }
}
