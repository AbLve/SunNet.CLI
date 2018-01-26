using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Sunnet.Cli.Assessment.Areas.Cot.Models
{
    public static class CotHelper
    {
        public static SelectList SpentTimes
        {
            get
            {
                var list = new List<SelectListItem>();
                var start = 0.00f;
                while (start <= 8.00f)
                {
                    list.Add(new SelectListItem()
                    {
                        Text = start.ToString("F2"),
                        Value = start - 0.00f < 0.01 ? "" : start.ToString("F2")
                    });
                    start += 0.25f;
                }
                return new SelectList(list, "Value", "Text");
            }
        }
    }
}