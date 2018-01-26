using System.Web.Mvc;

namespace Sunnet.Cli.MainSite.Areas.DataProcess
{
    public class DataProcessAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "DataProcess";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "DataProcess_default",
                "DataProcess/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}