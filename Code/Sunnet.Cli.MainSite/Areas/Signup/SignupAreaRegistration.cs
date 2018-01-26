using System.Web.Mvc;

namespace Sunnet.Cli.MainSite.Areas.Signup
{
    public class SignupAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "Signup";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "Signup_default",
                "Signup/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}