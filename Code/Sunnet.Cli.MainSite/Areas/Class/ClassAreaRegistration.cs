using System.Web.Mvc;

namespace Sunnet.Cli.MainSite.Areas.Class
{
    public class ClassAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "Class";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "Class_default",
                "Class/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}