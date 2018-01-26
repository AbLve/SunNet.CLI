using System.Web.Mvc;

namespace Sunnet.Cli.MainSite.Areas.Classroom
{
    public class ClassroomAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "Classroom";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "Classroom_default",
                "Classroom/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}