using System.Web.Mvc;

namespace Sunnet.Cli.MainSite.Areas.Help
{
    public class HelpAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "Help";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "Help_default",
                "Help/{controller}/{action}/{id}",
                new { controller = "Index", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}