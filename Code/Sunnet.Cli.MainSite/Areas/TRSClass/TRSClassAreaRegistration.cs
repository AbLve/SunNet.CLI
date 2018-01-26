using System.Web.Mvc;

namespace Sunnet.Cli.MainSite.Areas.TRSClass
{
    public class TRSClassAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "TRSClass";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "TRSClass_default",
                "TRSClass/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}