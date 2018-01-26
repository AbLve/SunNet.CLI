using System.Web.Mvc;

namespace Sunnet.Cli.MainSite.Areas.ToCac
{
    public class ToCacAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "ToCac";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "ToCac_default",
                "ToCac/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}