using Sunnet.Framework.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;

namespace Sunnet.Framework.Log
{
    /// <summary>
    /// web项目专用
    /// </summary>
    internal class WebLogAgent : ISunnetLog
    {

        public LogConfig Config
        {
            get;
            set;
        }

        /// <summary>
        /// Write an exception object to log.
        /// </summary>
        /// <param name="ex">The exception data.</param>
        public void Debug(Exception ex)
        {
            Debug(ex.ToString());
        }

        /// <summary>
        /// Write a new log message.
        /// </summary>
        /// <param name="message">Message body to log.</param>
        public void Debug(string message)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("Time:{0}\r\n", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            if (HttpContext.Current != null && HttpContext.Current.Request != null)
            {
                sb.AppendFormat("IP:{0}\r\n", CommonHelper.GetIPAddress(HttpContext.Current.Request))
                    .AppendFormat("Url:http://{0}{1}\r\n", HttpContext.Current.Request.Url.Host, HttpContext.Current.Request.RawUrl)
                    .AppendFormat("UrlRef:{0}\r\n", HttpContext.Current.Request.UrlReferrer == null ? "" : HttpContext.Current.Request.UrlReferrer.ToString())
                    .AppendFormat("Message:{0}\r\n\r\n", message);
            }
            else
            {
                sb.Append(message)
                    .Append("\r\n");
            }

            string fileName = "";
            if (fileName == null || fileName == "")
            {
                fileName = AppDomain.CurrentDomain.BaseDirectory + "/Log.txt";
            }
            using (StreamWriter write = new StreamWriter(fileName, true, Encoding.Default))
            {
                write.WriteLine(sb.ToString());
            }
        }

        public void Debug(string format, params object[] args)
        {
            Debug(string.Format(format, args));
        }

        public void Info(string format, params object[] args)
        {
            Debug(string.Format(format, args));
        }

        public void Info(string message)
        {
            Debug(message);
        }
    }
}
