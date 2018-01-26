using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Sunnet.Cli.Core;
using Sunnet.Cli.UIBase;
using Sunnet.Framework;

namespace Sunnet.Cli.Sso
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            IoC.Init();
        }
        protected void Session_Start(object sender, EventArgs e)
        {

        }

        protected void Session_End(object sender, EventArgs e)
        {

        }
        protected void Application_PreRequestHandlerExecute(object sender, EventArgs e)
        {
        }
    }
}
