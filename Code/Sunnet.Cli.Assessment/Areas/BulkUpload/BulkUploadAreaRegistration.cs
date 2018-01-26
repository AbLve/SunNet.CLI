using System.Web.Mvc;

namespace Sunnet.Cli.Assessment.Areas.BulkUpload
{
    public class BulkUploadAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "BulkUpload";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "BulkUpload_default",
                "BulkUpload/{controller}/{action}/{id}",
                new { controller = "ItemBup", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}