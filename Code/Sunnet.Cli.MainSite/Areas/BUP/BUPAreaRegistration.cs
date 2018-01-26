using System.Web.Mvc;

namespace Sunnet.Cli.MainSite.Areas.BUP
{
    public class BUPAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "BUP";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "BUP_default",
                "BUP/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}