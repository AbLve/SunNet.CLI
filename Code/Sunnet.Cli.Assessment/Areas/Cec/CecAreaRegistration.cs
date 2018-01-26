using System.Web.Mvc;

namespace Sunnet.Cli.Assessment.Areas.Cec
{
    public class CecAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "Cec";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "Cec_default",
                "Cec/{controller}/{action}/{id}",
                new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}