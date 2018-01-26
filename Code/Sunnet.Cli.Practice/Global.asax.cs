using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using StructureMap;
using Sunnet.Cli.Practice;
using Sunnet.Cli.Practice.Controllers;
using Sunnet.Cli.UIBase;
using Sunnet.Cli.UIBase.Models;
using Sunnet.Framework;
using Sunnet.Framework.Log;

namespace Sunnet.Cli.Assessment
{
    public class MvcApplication : System.Web.HttpApplication
    {
        public override void Init()
        {
            base.Init();

            EndRequest += MvcApplication_EndRequest;
        }

        void MvcApplication_EndRequest(object sender, EventArgs e)
        {
            // Response.Cache.SetNoStore(); 
        }

        protected void Application_Start()
        {
            IoC.Init();
            AreaRegistration.RegisterAllAreas();

            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            //GlobalConfiguration.Configure(WebApiConfig.Register);

            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            ModelBinders.Binders.DefaultBinder = new EensureEmptyIfNullModelBinder();
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

        protected void Application_Error(object sender, EventArgs e)
        {
            Exception exception = Server.GetLastError();
            // Log the exception.
            var logger = ObjectFactory.GetInstance<ISunnetLog>();
            logger.Debug(exception);

            Response.Clear();

            HttpException httpException = exception as HttpException;

            RouteData routeData = new RouteData();
            routeData.Values.Add("controller", "Error");

            if (httpException == null)
            {
                routeData.Values.Add("action", "Index");
            }
            else //It's an Http Exception, Let's handle it.
            {
                switch (httpException.GetHttpCode())
                {
                    case 404:
                        // Page not found.
                        routeData.Values.Add("action", "NotFound");
                        // Pass exception details to the target error View.
                        routeData.Values.Add("error", exception);
                        // Clear the error on server.
                        Server.ClearError();
                        // Avoid IIS7 getting in the middle
                        Response.TrySkipIisCustomErrors = true;
                        // Call target Controller and pass the routeData.
                        IController errorController = new ErrorController();
                        errorController.Execute(new RequestContext(
                             new HttpContextWrapper(Context), routeData));
                        break;
                    case 500:
                        // Server error.
                        routeData.Values.Add("action", "Error");
                        break;
                    // Here you can handle Views to other error codes.
                    // I choose a General error template  
                    default:
                        routeData.Values.Add("action", "General");
                        break;
                }
            }
        }
    }
}
