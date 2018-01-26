using System.Web.Mvc;

namespace Sunnet.Cli.MainSite.Areas.Toolbox
{
    public class ToolboxAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "Toolbox";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "Toolbox_default",
                "Toolbox/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}