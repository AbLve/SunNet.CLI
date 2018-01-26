using System.Web.Mvc;

namespace Sunnet.Cli.MainSite.Areas.Report
{
    public class ReportAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "Report";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "Report_default",
                "Report/{controller}/{action}/{id}",
                new { controller = "Index", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}