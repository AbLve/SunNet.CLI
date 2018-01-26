using System.Web.Mvc;

namespace Sunnet.Cli.Assessment.Areas.Cpalls
{
    public class CpallsAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "Cpalls";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "Cpalls_default",
                "Cpalls/{controller}/{action}/{id}",
                new { controller = "School", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}