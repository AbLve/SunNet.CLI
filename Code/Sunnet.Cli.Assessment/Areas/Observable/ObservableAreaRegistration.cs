using System.Web.Mvc;

namespace Sunnet.Cli.Assessment.Areas.Observable
{
    public class ObservableAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "Observable";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "Observable_default",
                "Observable/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}