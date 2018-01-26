using System.Web.Mvc;

namespace Sunnet.Cli.Vcw.Areas.STGReport
{
    public class STGReportAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "STGReport";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "STGReport_default",
                "STGReport/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}