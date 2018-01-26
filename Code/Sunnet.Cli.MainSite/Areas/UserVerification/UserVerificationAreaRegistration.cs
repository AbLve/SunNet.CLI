using System.Web.Mvc;

namespace Sunnet.Cli.MainSite.Areas.UserVerification
{
    public class UserVerificationAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "UserVerification";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "UserVerification_default",
                "UserVerification/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}