using System.Web.Mvc;

namespace Sunnet.Cli.Practice.Areas.Offline
{
    public class OfflineAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "Offline";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "Offline_default",
                "Offline/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}