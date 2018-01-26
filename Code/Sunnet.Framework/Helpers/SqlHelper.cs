using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sunnet.Framework.Helpers
{
    public static class SqlHelper
    {
        #region GetDBValue
        public static object GetDBValue(string v, string defaultValue)
        {
            object o = GetDBValue(v);
            if (o == null || o == DBNull.Value)
                return defaultValue;
            return o;
        }
        public static object GetDBValue(string v)
        {
            if (v == null)
                return DBNull.Value;
            return v;
        }
        public static object GetDBValue(Nullable<bool> v, bool defaultValue)
        {
            object o = GetDBValue(v);
            if (o == null || o == DBNull.Value)
                return defaultValue;
            return o;
        }
        public static object GetDBValue(Nullable<bool> v)
        {
            if (v == null)
                return DBNull.Value;
            if (!v.HasValue)
                return DBNull.Value;
            return v.Value;
        }
        public static object GetDBValue(Nullable<DateTime> v, DateTime defaultValue)
        {
            object o = GetDBValue(v);
            if (o == null || o == DBNull.Value)
                return defaultValue;
            return o;
        }
        public static object GetDBValue(Nullable<DateTime> v)
        {
            if (v == null)
                return DBNull.Value;
            if (!v.HasValue)
                return DBNull.Value;
            if (v.Value == DateTime.MinValue)
                return DBNull.Value;
            return v.Value;
        }
        public static object GetDBValue(Nullable<int> v, int defaultValue)
        {
            object o = GetDBValue(v);
            if (o == null || o == DBNull.Value)
                return defaultValue;
            return o;
        }
        public static object GetDBValue(Nullable<int> v)
        {
            if (v == null)
                return DBNull.Value;
            if (!v.HasValue)
                return DBNull.Value;
            return v.Value;
        }
        public static object GetDBValue(Nullable<decimal> v)
        {
            if (v == null)
                return DBNull.Value;
            if (!v.HasValue)
                return DBNull.Value;
            return v.Value;
        }
        #endregion

        public static string Convert2DBArrayString(List<int> ids)
        {
            string s = "";
            foreach (int id in ids)
            {
                s += id.ToString() + ",";
            }
            return s;
        }

        /// <summary>
        /// 搜索时，用Like 需要查询的特殊字符
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public static string FilterLikeSqlString(string sql)
        {
            sql = sql.Replace("[", "[[]");
            sql = sql.Replace("'", "''");
            sql = sql.Replace("%", "[%]");
            sql = sql.Replace("_", "[_]");

            return sql;
        }

        /// <summary>
        /// 添加记录时，需要过滤掉 单引号
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public static string ReplaceSqlChar(string sql)
        {
            if (string.IsNullOrEmpty(sql))
                return "";
            sql = sql.Replace("'", "''");
            return sql;
        }

    }
}
