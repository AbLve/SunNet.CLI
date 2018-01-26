using System.Web.Mvc;

namespace Sunnet.Cli.Assessment.Areas.Ade
{
    public class AdeAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "Ade";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "Ade_default",
                "Ade/{controller}/{action}/{id}",
                new { controller = "Assessment", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}