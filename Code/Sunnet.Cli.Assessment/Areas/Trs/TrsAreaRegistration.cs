using System.Web.Mvc;

namespace Sunnet.Cli.Assessment.Areas.Trs
{
    public class TrsAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "Trs";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "Trs_default",
                "Trs/{controller}/{action}/{id}",
                new { controller = "Index", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}