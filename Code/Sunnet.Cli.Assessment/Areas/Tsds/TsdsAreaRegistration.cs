using System.Web.Mvc;

namespace Sunnet.Cli.Assessment.Areas.Tsds
{
    public class TsdsAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "Tsds";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "Tsds_default",
                "Tsds/{controller}/{action}/{id}",
                new { controller = "Index", action = "Dashboard", id = UrlParameter.Optional }
            );
        }
    }
}