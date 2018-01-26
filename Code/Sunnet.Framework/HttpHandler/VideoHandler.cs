using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Sunnet.Framework.HttpHandler
{
    /// <summary>
    ///  流媒体防盗链
    /// </summary>
    public class VideoHandler : IHttpHandler
    {
        public bool IsReusable
        {
            get { return true; }
        }

        public void ProcessRequest(HttpContext context)
        {
            HttpCookie co = context.Request.Cookies["9772793214ED714F3082A04E18682F58"];
            if (co == null)
             {
                 context.Response.ContentType = "mpg4 video/mp4";
                 context.Response.WriteFile("/404.html");
             }
        }
    }
}
