using System.Web.Mvc;

namespace Sunnet.Cli.Vcw.Areas.Coach
{
    public class CoachAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "Coach";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "Coach_default",
                "Coach/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}