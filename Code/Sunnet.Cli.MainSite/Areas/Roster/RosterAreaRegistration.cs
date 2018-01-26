using System.Web.Mvc;

namespace Sunnet.Cli.MainSite.Areas.Roster
{
    public class RosterAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "Roster";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "Roster_default",
                "Roster/{controller}/{action}/{id}",
                new { action = "Index", controller = "Index", id = UrlParameter.Optional }
            );
        }
    }
}