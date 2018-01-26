using System.Web.Mvc;

namespace Sunnet.Cli.Assessment.Areas.Offline
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
                new { Controller = "Index", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}