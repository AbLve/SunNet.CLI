using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Ajax;

namespace Sunnet.Cli.UIBase.Models
{
    /// <summary>
    /// 表单提交的一些公共方法类
    /// </summary>
    public static class PostFormHelper
    {
        public static string GetFormId(IView view)
        {
            var relativePath = ((RazorView)view).ViewPath;
            var paths = relativePath.Split("~/".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            var formId = string.Join("_", paths);
            return formId.Replace(".cshtml", "");
        }

        private static AjaxOptions _ajaxOption;

        /// <summary>
        /// Get the default ajax options.
        /// </summary>
        public static AjaxOptions DefaultAjaxOptions
        {
            get
            {
                if (_ajaxOption == null)
                {
                    _ajaxOption = new AjaxOptions()
                    {
                        HttpMethod = "Post",
                        OnSuccess = "OnFormSubmitSuccess",
                        OnFailure = "onFormSubmitFailure",
                        AllowCache = false
                    };
                }
                return _ajaxOption;
            }
        }

        public static DateTime DefaultMinDatetime
        {
            get
            {
                return new DateTime(1753, 1, 1);
            }
        }
    }
}