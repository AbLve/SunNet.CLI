using System.Web.Mvc;

namespace Sunnet.Cli.MainSite.Areas.Invitation
{
    public class InvitationAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "Invitation";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "Invitation_default",
                "Invitation/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}