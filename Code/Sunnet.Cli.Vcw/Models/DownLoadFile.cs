using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sunnet.Cli.Vcw.Models
{
    public static class DownLoadFile
    {
        public static string GetDownFilePath(string filepath, string filename)
        {
            if (!string.IsNullOrEmpty(filepath))
            {
                return "/DownLoadFile/DownLoadFile?filepath=" +
                    System.Web.HttpContext.Current.Server.UrlEncode(filepath)
                    + "&filename=" + System.Web.HttpContext.Current.Server.UrlEncode(filename) + "";
            }
            else
            {
                return "";
            }
        }
    }
}