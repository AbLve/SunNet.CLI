using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sunnet.Framework.Log
{
    public interface ISunnetLog
    {
        LogConfig Config { get; set; }
        /// <summary>
        /// Debug日志：总是写入日志文件
        /// </summary>
        /// <param name="ex">The ex.</param>
        void Debug(Exception ex);
        /// <summary>
        /// Debug日志：总是写入日志文件
        /// </summary>
        /// <param name="message">The message.</param>
        void Debug(string message);

        /// <summary>
        /// Debug日志：总是写入日志文件
        /// </summary>
        /// <param name="format">The format.</param>
        /// <param name="args">The arguments.</param>
        void Debug(string format, params object[] args);

        /// <summary>
        /// Info日志：只在开发和测试站点写入日志文件，正式站点不写入
        /// </summary>
        /// <param name="message">The message.</param>
        void Info(string message);
        /// <summary>
        /// Info日志：只在开发和测试站点写入日志文件，正式站点不写入
        /// </summary>
        /// <param name="format">The format.</param>
        /// <param name="args">The arguments.</param>
        void Info(string format, params object[] args);

    }
}
