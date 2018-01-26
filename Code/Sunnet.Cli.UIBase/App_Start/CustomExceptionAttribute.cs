using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using StructureMap;
using Sunnet.Framework.Log;

namespace Sunnet.Cli.UIBase
{
    public class CustomExceptionAttribute : FilterAttribute, IExceptionFilter
    {
        private ISunnetLog logger;

        public CustomExceptionAttribute()
        {
            logger = ObjectFactory.GetInstance<ISunnetLog>();
        }

        public void OnException(ExceptionContext filterContext)
        {
#if !DEBUG
            logger.Debug(filterContext.Exception);
#endif

            if (filterContext.HttpContext.Request.IsAjaxRequest())
            {
                filterContext.ExceptionHandled = false;
            }
            else
            {
                filterContext.ExceptionHandled = true;
                HttpException httpException = filterContext.Exception as HttpException;
                if (httpException != null)
                {
                    filterContext.Controller.ViewBag.UrlRefer = filterContext.HttpContext.Request.UrlReferrer;
                    if (httpException.GetHttpCode() == 404)
                    {
                        filterContext.HttpContext.Response.Redirect("~/404");
                    }
                    else if (httpException.GetHttpCode() == 500)
                    {
#if !DEBUG
                        filterContext.HttpContext.Response.Redirect("~/Error/Error");
#endif

                    }
                }
                else
                {
#if !DEBUG
                    filterContext.HttpContext.Response.Redirect("~/Error/Error");
#endif
                }
            }
#if DEBUG

            filterContext.HttpContext.Response.Write(
                string.Format("<p style='color:red;'>Exception:{0}<br>InnerException:{2}<br>StackTrace:{1}<p>",
                filterContext.Exception.Message,
                filterContext.Exception.StackTrace,
                filterContext.Exception.InnerException));
#endif
        }
    }
}