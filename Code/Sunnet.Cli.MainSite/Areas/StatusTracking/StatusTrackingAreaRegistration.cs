using System.Web.Mvc;

namespace Sunnet.Cli.MainSite.Areas.StatusTracking
{
    public class StatusTrackingAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "StatusTracking";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "StatusTracking_default",
                "StatusTracking/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}