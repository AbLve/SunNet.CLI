using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Sunnet.Cli.UIBase
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new CustomExceptionAttribute(), 1);
            filters.Add(new HandleErrorAttribute(), 2);
        }
    }
}