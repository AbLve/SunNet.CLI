using System.Web.Mvc;

namespace Sunnet.Cli.Assessment.Areas.Cot
{
    public class CotAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "Cot";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "Cot_default",
                "Cot/{controller}/{action}/{id}",
                new { controller = "Index", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}