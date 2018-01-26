using System.Data.Entity.Validation;
using log4net;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using log4net.Core;
using Sunnet.Framework.Core.Extensions;
using System.Threading.Tasks;

namespace Sunnet.Framework.Log
{
    internal class log4netProvider : ISunnetLog
    {
        private ILog _logInfo;
        private ILog _logDebug;
        public log4netProvider()
        {
            _logInfo = LogManager.GetLogger("info.Logging");
            _logDebug = LogManager.GetLogger("debug.Logging");
        }
        #region ILog Members

        private ILog GetLogger(Level level)
        {
            if (level == Level.Info)
                return _logInfo;
            return _logDebug;
        }

        public LogConfig Config
        {
            get;
            set;
        }

        public void Debug(Exception ex)
        {
            Log(Level.Debug, "", ex);
        }

        public void Debug(string message)
        {
            _logDebug.Debug(message);
        }

        public void Debug(string format, params object[] args)
        {
            _logDebug.DebugFormat(format, args);
        }

        public void Info(string message)
        {
            _logInfo.Info(message);
        }

        public void Info(string format, params object[] args)
        {
            _logInfo.InfoFormat(format, args);
        }

        private void Log(Level level, string message, Exception exception)
        {
            if (exception is AggregateException) return;
            ILog log = GetLogger(level);
            //message += " Source Code:" + exception.StackTrace;
            message += exception.ToString();
            if (exception is DbEntityValidationException)
            {
                var ex = exception as DbEntityValidationException;
                if (ex.InnerException != null)
                {
                    var sb = new StringBuilder();
                    sb.Append("Entity Validation Log:\r\n")
                        .AppendFormat("DateTime: {0}\r\n", DateTime.Now.ToString("MM/dd/yyyy HH:mm"))
                        .AppendFormat("{0}\r\n", ex.InnerException);
                    log.Debug(sb.ToString());
                }
                if (ex.EntityValidationErrors != null && ex.EntityValidationErrors.Any())
                {
                    var sb = new StringBuilder();
                    sb.Append("Entity Validation Details:\r\n")
                        .AppendFormat("DateTime: {0}\r\n", DateTime.Now.ToString("MM/dd/yyyy HH:mm"));
                    sb.Append(ex.GetDetailMessage("\r\n"));
                    log.Debug(sb.ToString());
                }
            }
            log.Debug(message, exception);
        }
        #endregion

        #region  //Extend method
        public void Log(Exception[] exceptions)
        {
            for (int i = 0; i < exceptions.Length; i++)
            {
                Debug(exceptions[i]);
            }
        }

        public void LogSQL(string procedureNameOrSQL, DbParameterCollection cmdParameters, Exception exception)
        {
            string message = string.Format("[SQLText:{0},{1}]", procedureNameOrSQL, FormatCMDParameters(cmdParameters));
            Log(Level.Debug, message, exception);
        }

        public void LogSQL(string procedureNameOrSQL, Exception exception)
        {
            string message = string.Format("[SQLText:{0}]", procedureNameOrSQL);
            Log(Level.Debug, message, exception);
        }
        #endregion

        private string FormatCMDParameters(DbParameterCollection dbParameterCollection)
        {
            StringBuilder stringBuilder = new StringBuilder("\r\nParameters-------------------------\r\n");
            foreach (DbParameter dbParameter in dbParameterCollection)
            {
                stringBuilder.AppendFormat("ParameterName:{0},Value:{1},Type:{2};\r\n", dbParameter.ParameterName
                    , (null == dbParameter.Value) ? "Null" : dbParameter.Value.ToString(), dbParameter.Direction.ToString());
            }
            stringBuilder.Append("----------------------------------\r\n");
            return stringBuilder.ToString();
        }
    }
}
