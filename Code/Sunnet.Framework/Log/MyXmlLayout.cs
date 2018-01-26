using log4net.Core;
using System;
using System.Web;
using System.Xml;

namespace SF.Framework.Log
{
    public class MyXmlLayout:log4net.Layout.XmlLayoutSchemaLog4j
    {
        protected override void FormatXml(XmlWriter writer, LoggingEvent loggingEvent)
        {
            writer.WriteString("\r\n");
            writer.WriteString("\r\n");

            //< conversionPattern value = "%newline Time: %date %newline 
            //IP: %aspnet-request{REMOTE_HOST} %newline 
            //Url: %aspnet-request{Server_Name}%aspnet-request{url} 
            //%aspnet-request{QUERY_STRING} 
            //%newline urlRefer: %aspnet-request{Http_Referer}%newline 
            //%logger -Message: %newline %message %newline %newline" />


            writer.WriteStartElement("log4j:event");

            writer.WriteString("\r\n");

            writer.WriteStartElement("log4j:time");
            string time = string.Format("Time:{0}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            writer.WriteString(time);
            writer.WriteEndElement();


            writer.WriteString("\r\n");

            writer.WriteStartElement("log4j:ip");
            string ip = string.Format("IP:{0}", HttpContext.Current.Request.UserHostAddress);
            writer.WriteString(ip);
            writer.WriteEndElement();


            writer.WriteString("\r\n");

            writer.WriteStartElement("log4j:url");
            string url = string.Format("Url:http://{0}{1}", HttpContext.Current.Request.Url.Host, HttpContext.Current.Request.RawUrl);
            writer.WriteString(url);
            writer.WriteEndElement();


            writer.WriteString("\r\n");

            writer.WriteStartElement("log4j:urlRef");
            string urlRef = string.Format("UrlRef:{0}", HttpContext.Current.Request.UrlReferrer == null ? "" : HttpContext.Current.Request.UrlReferrer.ToString());
            writer.WriteString(urlRef);
            writer.WriteEndElement();


            writer.WriteString("\r\n");

            writer.WriteStartElement("log4j:message");
            writer.WriteString(loggingEvent.RenderedMessage+"\r\n");
            writer.WriteEndElement();


            writer.WriteString("\r\n");


            writer.WriteEndElement();
        }

    }
}
