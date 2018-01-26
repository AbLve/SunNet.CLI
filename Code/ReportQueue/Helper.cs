using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sunnet.Framework.Helpers;

namespace ReportQueue
{
    internal static class Helper
    {
        internal static EmailTemplete GetReportTemplete(string template = "ReportQueueMsg.xml")
        {
            return XmlHelper.GetEmailTemplete(Config.Instance.TemplatePath, template);
        }
    }
}
